using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

using data;
using data._Attribute;
using data._Categories;
using data.Owned;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace api
{
    // ════════════════════════════════════════════════════════════════════════
    //  AIAttributeManager — KNOWLEDGE-BASE-FIRST, IDEMPOTENT, ÖLÇEKLENEBİLİR
    //  ------------------------------------------------------------------------
    //  Felsefe (V2): AI artık attribute ÜRETMEZ; bilgi tabanını YÖNETİR.
    //  LLM son karar verici değildir, YARDIMCIDIR. Her istek ÖNCE mevcut bilgiyi
    //  (Memory → SQL → Template → Inheritance) tarar; LLM yalnızca EKSİKLER için
    //  ve yalnızca "eksik olanı döndür" bağlamıyla çağrılır.
    //
    //  Bu dosya TEK BAŞINA çalışır (tek entry point mevcut arka plan döngüsüyle
    //  uyumlu: ProcessNextPendingCategoryAsync). Koordinasyon/idempotency/retry
    //  için şemadaki AiGenerationJob (lease + IdempotencyKey + backoff) kuyruğu
    //  kullanılır — böylece tek bozuk kategori TÜM kuyruğu bloklamaz ve N worker
    //  güvenle paralel çalışır.
    //
    //  KAPSAM NOTU (dürüstlük): Embedding/Semantic-similarity, Redis (L2) ve
    //  Knowledge-Graph (Neo4j) faktörleri bu tek dosyada YOKTUR — altyapı ister ve
    //  ayrı servislerdir. Confidence, embedding'siz hesaplanabilen deterministik
    //  alt kümedir. İlgili yerler "EXTENSION POINT" ile işaretlidir.
    // ════════════════════════════════════════════════════════════════════════

    /// <summary>Test/mock ve DI için genel sözleşme.</summary>
    public interface IAIAttributeManager
    {
        /// <summary>Kuyruktan bir işi kiralayıp işler; iş yoksa keşif (enqueue) yapıp tekrar dener. Mevcut arka plan döngüsü bunu çağırır.</summary>
        Task ProcessNextPendingCategoryAsync(CancellationToken cancellationToken = default);

        /// <summary>Bekleyen kategorileri iş kuyruğuna ekler (idempotent). Keşif ile işlemeyi ayırır.</summary>
        Task<int> EnqueuePendingCategoriesAsync(CancellationToken cancellationToken = default);

        /// <summary>En çok <paramref name="maxJobs"/> işi sırayla işler (batch).</summary>
        Task<int> ProcessBatchAsync(int maxJobs, CancellationToken cancellationToken = default);

        /// <summary>Belirli bir kategoriyi doğrudan (kuyruk dışı) işler — manuel/admin tetikleme için.</summary>
        Task ProcessCategoryAsync(Guid categoryId, string categoryPath, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Yapılandırma. appsettings/secret'tan "AIAttributeManager" bölümüyle bağlanır.
    /// API anahtarı ARTIK KODA GÖMÜLÜ DEĞİLDİR.
    /// </summary>
    public sealed class AIAttributeManagerOptions
    {
        public string ApiKey { get; set; } = string.Empty;
        /// <summary>Gerçek/erişilebilir bir model adı verin (ör. "gemini-2.5-flash" / "gemini-2.5-pro"). Doğrulayın.</summary>
        public string Model { get; set; } = string.Empty;
        public int HttpTimeoutSeconds { get; set; } = 90;
        public int MaxLlmAttempts { get; set; } = 4;
        public int LeaseMinutes { get; set; } = 10;
        public int PromptCacheTtlHours { get; set; } = 12;
        public int KnowledgeIndexCacheTtlMinutes { get; set; } = 5;
        public int EnqueueBatchSize { get; set; } = 200;
        public int MaxInheritanceDepth { get; set; } = 8;
        /// <summary>
        /// Gemini structured-output (responseSchema) gönderilsin mi? Şema formatı Gemini SÜRÜMÜNE duyarlıdır;
        /// yanlış şema TÜM çağrıları 400'e düşürebilir. Bu yüzden VARSAYILAN KAPALI: responseMimeType=application/json +
        /// güçlü prompt + JSON-repair yeterlidir. Şemayı kendi Gemini sürümünle doğruladıktan sonra açabilirsin.
        /// </summary>
        public bool UseStructuredOutput { get; set; } = false;
        /// <summary>Bu güvenin ÜSTÜNDEKİ AI kayıtları otomatik onaylanır; altındakiler PendingApproval kalır.</summary>
        public decimal AutoApproveConfidence { get; set; } = 0.95m;
        /// <summary>Bu makine/instance kimliği (lease izlenebilirliği için).</summary>
        public string WorkerId { get; set; } = Environment.MachineName + ":" + Guid.NewGuid().ToString("N").Substring(0, 6);
    }

    public sealed class AIAttributeManager : IAIAttributeManager
    {
        // Kanonik dil listesi (kaynak = Tr).
        private static readonly Language[] AllLanguages =
        {
            Language.Tr, Language.En, Language.Az, Language.De, Language.Es,
            Language.Fr, Language.Hi, Language.Pt, Language.Ru, Language.Zh
        };

        // Global-unique synonym'e YAZILMAYACAK aşırı jenerik tek-kelime token'lar.
        // (Cross-domain yanlış merge'i önler: "type/size/color" gibi kelimeler tek
        //  bir attribute'a global kilitlenmemeli.) Yine de DEDUP OKUMASINDA kullanılır,
        //  ama kilit dataType-uyumu ile guard'lanır (bkz. ResolveReuseAsync).
        private static readonly HashSet<string> GenericStopTokens = new(StringComparer.Ordinal)
        {
            "type","tip","tur","size","boyut","olcu","color","renk","material","malzeme",
            "model","kapasite","capacity","value","deger","name","ad","adi","other","diger","genel","general"
        };

        private readonly HttpClient _httpClient;
        private readonly IDbContextFactory<_ApplicationConnectionDb> _dbFactory;
        private readonly ILogger<AIAttributeManager> _logger;
        private readonly IMemoryCache _cache;
        private readonly AIAttributeManagerOptions _opt;

        // Aynı süreçte yazma serileştirme (proje konvansiyonu). Çok-instance güvenliği
        // SemaphoreSlim ile DEĞİL, AiGenerationJob LEASE + RowVersion ile sağlanır.
        private static readonly SemaphoreSlim _writeLock = new(1, 1);

        public AIAttributeManager(
            HttpClient httpClient,
            IDbContextFactory<_ApplicationConnectionDb> dbFactory,
            ILogger<AIAttributeManager> logger,
            IMemoryCache cache,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _dbFactory = dbFactory;
            _logger = logger;
            _cache = cache;

            _opt = new AIAttributeManagerOptions();
            configuration.GetSection("AIAttributeManager").Bind(_opt);

            if (string.IsNullOrWhiteSpace(_opt.ApiKey))
                _opt.ApiKey = configuration["Gemini:ApiKey"] ?? string.Empty; // yaygın alternatif konum
        }

        // ════════════════════════════════════════════════════════════════════
        //  GİRİŞ NOKTALARI
        // ════════════════════════════════════════════════════════════════════

        public async Task ProcessNextPendingCategoryAsync(CancellationToken cancellationToken = default)
        {
            // 1) İş kirala. Yoksa keşif yap, tekrar kirala.
            var job = await ClaimNextJobAsync(cancellationToken);
            if (job == null)
            {
                var enqueued = await EnqueuePendingCategoriesAsync(cancellationToken);
                if (enqueued == 0) return; // yapılacak iş yok
                job = await ClaimNextJobAsync(cancellationToken);
                if (job == null) return; // başka worker kaptı
            }

            await ProcessJobAsync(job, cancellationToken);
        }

        public async Task<int> ProcessBatchAsync(int maxJobs, CancellationToken cancellationToken = default)
        {
            if (maxJobs <= 0) return 0;
            await EnqueuePendingCategoriesAsync(cancellationToken);

            int processed = 0;
            for (int i = 0; i < maxJobs; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var job = await ClaimNextJobAsync(cancellationToken);
                if (job == null) break;
                await ProcessJobAsync(job, cancellationToken);
                processed++;
            }
            return processed;
        }

        public async Task ProcessCategoryAsync(Guid categoryId, string categoryPath, CancellationToken cancellationToken = default)
        {
            // Kuyruk dışı doğrudan işleme (manuel/admin). Yine de KB-First + idempotent akış.
            var pseudoJob = new AiGenerationJob
            {
                JobType = AiJobType.CategoryAttributeAnalysis,
                Status = AiJobStatus.Processing,
                TargetCategoryId = categoryId,
                CategoryPathSnapshot = categoryPath,
                PromptVersion = PromptBuilder.SelectVersion(categoryPath),
                MaxAttempts = 1,
                AttemptCount = 1,
                LeasedBy = _opt.WorkerId,
                StartedAtUtc = DateTime.UtcNow
            };
            await RunCategoryPipelineAsync(pseudoJob, categoryId, categoryPath, persistJobState: false, cancellationToken);
        }

        // ════════════════════════════════════════════════════════════════════
        //  1) KEŞİF → İŞ KUYRUĞA EKLEME (idempotent, IdempotencyKey unique)
        // ════════════════════════════════════════════════════════════════════

        public async Task<int> EnqueuePendingCategoriesAsync(CancellationToken cancellationToken = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken);

            // Bekleyen ve HENÜZ açık işi olmayan kategoriler.
            // (Açık iş = Pending/Leased/Processing/Failed/NeedsReview olan, aynı prompt sürümü.)
            string version = PromptBuilder.CurrentDefaultVersion;

            var pending = await db.Set<CategoriesProduct>()
                .AsNoTracking()
                .Where(c => c.IsActive
                            && (c.AiAttributesIsOk == null || c.AiAttributesIsOk != true)
                            && (c.IsDeleted == null || c.IsDeleted.IsDeletedStatu != true)
                            && c.Path != null && c.Path != "")
                .OrderBy(c => c.CreatedDate)
                .Select(c => new { c.Id, c.Path })
                .Take(_opt.EnqueueBatchSize)
                .ToListAsync(cancellationToken);

            if (pending.Count == 0) return 0;

            int added = 0;
            foreach (var c in pending)
            {
                cancellationToken.ThrowIfCancellationRequested();

                string idem = ComputeIdempotencyKey(AiJobType.CategoryAttributeAnalysis, c.Path!, version);

                bool exists = await db.Set<AiGenerationJob>()
                    .AnyAsync(j => j.IdempotencyKey == idem
                                   && j.IsDeleted.IsDeletedStatu != true, cancellationToken);
                if (exists) continue;

                db.Set<AiGenerationJob>().Add(new AiGenerationJob
                {
                    JobType = AiJobType.CategoryAttributeAnalysis,
                    Status = AiJobStatus.Pending,
                    TargetCategoryId = c.Id,
                    CategoryPathSnapshot = c.Path,
                    PromptVersion = version,
                    IdempotencyKey = idem,
                    Priority = 100,
                    MaxAttempts = 5,
                    AttemptCount = 0
                });

                try
                {
                    await db.SaveChangesAsync(cancellationToken); // her iş tekil kaydedilir → biri patlarsa diğerleri kaybolmaz
                    added++;
                }
                catch (DbUpdateException) // IdempotencyKey unique yarışı → başka worker ekledi
                {
                    db.ChangeTracker.Clear();
                }
            }

            if (added > 0)
                _logger.LogInformation("AIAttr kuyruğa eklendi: {Added} kategori (sürüm {Version}).", added, version);
            return added;
        }

        // ════════════════════════════════════════════════════════════════════
        //  2) İŞ KİRALAMA (LEASE) — çok-worker güvenli (RowVersion optimistic)
        // ════════════════════════════════════════════════════════════════════

        private async Task<AiGenerationJob?> ClaimNextJobAsync(CancellationToken cancellationToken)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken);
            var now = DateTime.UtcNow;

            // Aday işler: Pending veya (Failed & retry zamanı gelmiş) veya (Leased & kirası dolmuş).
            var candidateIds = await db.Set<AiGenerationJob>()
                .AsNoTracking()
                .Where(j =>
                    j.IsDeleted.IsDeletedStatu != true
                    && (
                        j.Status == AiJobStatus.Pending
                        || (j.Status == AiJobStatus.Failed
                            && (j.NextRetryAtUtc == null || j.NextRetryAtUtc <= now)
                            && j.AttemptCount < j.MaxAttempts)
                        || ((j.Status == AiJobStatus.Leased || j.Status == AiJobStatus.Processing)
                            && j.LeasedUntilUtc != null && j.LeasedUntilUtc < now)
                    ))
                .OrderBy(j => j.Priority).ThenBy(j => j.CreatedAtUtc)
                .Select(j => j.Id)
                .Take(15)
                .ToListAsync(cancellationToken);

            foreach (var id in candidateIds)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var job = await db.Set<AiGenerationJob>().FirstOrDefaultAsync(j => j.Id == id, cancellationToken);
                if (job == null) continue;
                if (job.Status == AiJobStatus.Completed || job.Status == AiJobStatus.Cancelled) continue;

                job.Status = AiJobStatus.Processing;
                job.LeasedBy = _opt.WorkerId;
                job.LeasedUntilUtc = now.AddMinutes(_opt.LeaseMinutes);
                job.StartedAtUtc = now;
                job.AttemptCount += 1;
                job.UpdatedAtUtc = now;

                try
                {
                    await db.SaveChangesAsync(cancellationToken); // RowVersion → başka worker kaptıysa concurrency exception
                    return job;
                }
                catch (DbUpdateConcurrencyException)
                {
                    db.ChangeTracker.Clear(); // başka worker kaptı; sonraki adayı dene
                }
            }

            return null;
        }

        // ════════════════════════════════════════════════════════════════════
        //  3) İŞ İŞLEME (orkestrasyon) — hata izolasyonu + backoff
        // ════════════════════════════════════════════════════════════════════

        private async Task ProcessJobAsync(AiGenerationJob job, CancellationToken cancellationToken)
        {
            var categoryId = job.TargetCategoryId ?? Guid.Empty;
            var path = job.CategoryPathSnapshot ?? string.Empty;

            if (categoryId == Guid.Empty || string.IsNullOrWhiteSpace(path))
            {
                await CompleteJobAsync(job.Id, AiJobStatus.Skipped, "Hedef kategori/yol boş.", 0, cancellationToken);
                return;
            }

            try
            {
                await RunCategoryPipelineAsync(job, categoryId, path, persistJobState: true, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // Uygulama kapanıyor: işi bir sonraki worker'a bırak (lease dolunca yeniden alınır).
                throw;
            }
            catch (OperationCanceledException)
            {
                // Kendi zaman aşımımız → backoff ile yeniden.
                await FailJobWithBackoffAsync(job, "LLM/işlem zaman aşımı.", cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AIAttr iş hatası → {Path}", path);
                await FailJobWithBackoffAsync(job, ex.Message, cancellationToken);
            }
        }

        /// <summary>KB-First → Prompt → Retry → Validate → Self-Verify → Decision → İdempotent kalıcılık.</summary>
        private async Task RunCategoryPipelineAsync(
            AiGenerationJob job, Guid categoryId, string categoryPath, bool persistJobState, CancellationToken cancellationToken)
        {
            // ── 0) Kategori hâlâ geçerli mi? (idempotent no-op guard) ─────────
            await using (var readDb = await _dbFactory.CreateDbContextAsync(cancellationToken))
            {
                var cat = await readDb.Set<CategoriesProduct>().AsNoTracking()
                    .Where(c => c.Id == categoryId)
                    .Select(c => new { c.AiAttributesIsOk, Deleted = c.IsDeleted, c.IsActive })
                    .FirstOrDefaultAsync(cancellationToken);

                if (cat == null || !cat.IsActive || cat.Deleted?.IsDeletedStatu == true)
                {
                    if (persistJobState)
                        await CompleteJobAsync(job.Id, AiJobStatus.Skipped, "Kategori yok/pasif/silinmiş.", 0, cancellationToken);
                    return;
                }
                if (cat.AiAttributesIsOk == true)
                {
                    if (persistJobState)
                        await CompleteJobAsync(job.Id, AiJobStatus.Skipped, "Kategori zaten işlenmiş.", 0, cancellationToken);
                    return;
                }
            }

            // ── 1) KNOWLEDGE-BASE-FIRST: mevcut bilgiyi topla ─────────────────
            var kb = await BuildKnowledgeContextAsync(categoryId, categoryPath, cancellationToken);

            int reusedFromKb = 0;
            // 1a) Template + Inheritance ile ZATEN bilinenleri kategoriye bağla (LLM'siz reuse).
            reusedFromKb += await ApplyKnownLinksAsync(categoryId, categoryPath, kb, job.Id, cancellationToken);

            // ── 2) PROMPT BUILDER: yalnızca EKSİKLERİ iste ────────────────────
            string version = job.PromptVersion ?? PromptBuilder.SelectVersion(categoryPath);
            var (prompt, builtSchema) = PromptBuilder.Build(version, categoryPath, kb.KnownCanonicalCodes);
            object? responseSchema = _opt.UseStructuredOutput ? builtSchema : null; // güvenli varsayılan: gönderme
            string promptHash = ComputeHash(prompt);

            // ── 3) PROMPT CACHE → yoksa RETRY'li LLM ──────────────────────────
            AiAttributeSchema? schema = await GetSchemaCachedOrGenerateAsync(
                categoryPath, version, promptHash, prompt, responseSchema, cancellationToken);

            if (schema == null || schema.Attributes == null || schema.Attributes.Count == 0)
            {
                // LLM eksik bir şey döndürmedi. KB zaten kapsıyorsa bu BAŞARIDIR.
                await FinalizeCategoryAsync(categoryId, cancellationToken);
                if (persistJobState)
                    await CompleteJobAsync(job.Id, AiJobStatus.Completed,
                        $"KB kapsadı; LLM eksik döndürmedi. KB-link: {reusedFromKb}.", reusedFromKb, cancellationToken);
                return;
            }

            // ── 4) SELF-VERIFICATION (Pass 2): şemayı temizle/normalize et ────
            var verified = SchemaValidator.Verify(schema, categoryPath, _logger);

            // ── 5) DECISION + İDEMPOTENT KALICILIK (attribute-başına transaction) ─
            int created = 0, reused = 0, expanded = 0, ignored = 0, recordCount = 0;

            await _writeLock.WaitAsync(cancellationToken); // süreç-içi yazma serileştirme (konvansiyon)
            try
            {
                int order = kb.NextDisplayOrder;
                foreach (var aiAttr in verified.Attributes)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    // Manuel (kuyruk dışı) çalıştırmada job DB'de yok → audit'te phantom FK olmasın.
                    Guid historyJobId = persistJobState ? job.Id : Guid.Empty;
                    var outcome = await PersistAttributeDecisionAsync(
                        categoryId, categoryPath, aiAttr, order, historyJobId, promptHash, version, cancellationToken);

                    switch (outcome.Action)
                    {
                        case AttrAction.Create: created++; break;
                        case AttrAction.Reuse: reused++; break;
                        case AttrAction.Expand: expanded++; break;
                        case AttrAction.Ignore: ignored++; break;
                    }
                    recordCount += outcome.CreatedRecordCount;
                    order++;
                }
            }
            finally
            {
                _writeLock.Release();
            }

            // ── 6) Kategoriyi işlenmiş işaretle + iş tamam ────────────────────
            await FinalizeCategoryAsync(categoryId, cancellationToken);
            InvalidateKnowledgeIndex();

            string summary = $"KB-link:{reusedFromKb} | yeni:{created}, reuse:{reused}, expand:{expanded}, ignore:{ignored}, kayıt:{recordCount}";
            _logger.LogInformation("Kategori işlendi → {Path} | {Summary}", categoryPath, summary);

            if (persistJobState)
                await CompleteJobAsync(job.Id, AiJobStatus.Completed, summary, created + expanded, cancellationToken);
        }

        // ════════════════════════════════════════════════════════════════════
        //  KNOWLEDGE-BASE-FIRST — mevcut bilgi toplama
        // ════════════════════════════════════════════════════════════════════

        private sealed class KnowledgeContext
        {
            // Global dedup indeksleri (bir kez yüklenir, IMemoryCache'te tutulur).
            public Dictionary<string, AttrIndexRow> ByCanonicalCode = new(StringComparer.Ordinal);
            public Dictionary<string, AttrIndexRow> BySynonymToken = new(StringComparer.Ordinal);
            public Dictionary<string, AttrIndexRow> ByNormalizedName = new(StringComparer.Ordinal);

            // Bu kategori için "zaten bilinen" attribute'lar (template + inheritance + mevcut link).
            public HashSet<Guid> KnownAttributeIds = new();
            public List<TemplateAttributeRow> TemplateAttributes = new();
            public List<Guid> InheritedAttributeIds = new();
            public HashSet<string> KnownCanonicalCodes = new(StringComparer.Ordinal);
            public int NextDisplayOrder;
        }

        private sealed record AttrIndexRow(Guid Id, string CanonicalCode, AttributeDataType DataType, Guid? UnitGroupId);
        private sealed record TemplateAttributeRow(Guid AttributeId, int DisplayOrder, bool? IsRequired, bool? IsVariant, bool? IsFilterable, bool? IsComparable, bool? IsVisible);

        private async Task<KnowledgeContext> BuildKnowledgeContextAsync(
            Guid categoryId, string categoryPath, CancellationToken cancellationToken)
        {
            var kb = new KnowledgeContext();

            // 1) Global dedup indeksleri (L1: IMemoryCache; miss → SQL). Round-trip'leri keser.
            var idx = await GetOrLoadGlobalIndexAsync(cancellationToken);
            kb.ByCanonicalCode = idx.ByCanonicalCode;
            kb.BySynonymToken = idx.BySynonymToken;
            kb.ByNormalizedName = idx.ByNormalizedName;

            await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken);

            // 2) Bu kategoride ZATEN olan linkler (idempotency + sıralama tabanı).
            var existingLinks = await db.Set<CategoryAttribute>().AsNoTracking()
                .Where(l => l.CategoryId == categoryId
                            && l.IsDeleted.IsDeletedStatu != true)
                .Select(l => new { l.AttributeDefinitionId, l.DisplayOrder })
                .ToListAsync(cancellationToken);
            foreach (var l in existingLinks) kb.KnownAttributeIds.Add(l.AttributeDefinitionId);
            kb.NextDisplayOrder = existingLinks.Count == 0 ? 0 : existingLinks.Max(x => x.DisplayOrder) + 1;

            // 3) TEMPLATE eşleştirme: bu kategoriye (veya en yakın üst kategoriye) atanmış primary template.
            var ancestorIds = await GetAncestorCategoryIdsAsync(db, categoryId, cancellationToken);
            var scopeIds = new List<Guid> { categoryId };
            scopeIds.AddRange(ancestorIds);

            var templateId = await db.Set<TemplateCategory>().AsNoTracking()
                .Where(tc => scopeIds.Contains(tc.CategoryId)
                             && tc.IsDeleted.IsDeletedStatu != true)
                .OrderByDescending(tc => tc.IsPrimary)
                .Select(tc => (Guid?)tc.AttributeTemplateId)
                .FirstOrDefaultAsync(cancellationToken);

            if (templateId.HasValue)
            {
                kb.TemplateAttributes = await db.Set<TemplateAttribute>().AsNoTracking()
                    .Where(ta => ta.AttributeTemplateId == templateId.Value
                                 && ta.IsDeleted.IsDeletedStatu != true)
                    .Select(ta => new TemplateAttributeRow(
                        ta.AttributeDefinitionId, ta.DisplayOrder,
                        ta.IsRequiredOverride, ta.IsVariantOverride, ta.IsFilterableOverride,
                        ta.IsComparableOverride, ta.IsVisibleOverride))
                    .ToListAsync(cancellationToken);
            }

            // 4) INHERITANCE: üst kategorilerde tanımlı, bastırılmamış attribute'lar miras alınır.
            if (ancestorIds.Count > 0)
            {
                kb.InheritedAttributeIds = await db.Set<CategoryAttribute>().AsNoTracking()
                    .Where(l => ancestorIds.Contains(l.CategoryId)
                                && l.IsSuppressed == false
                                && l.IsDeleted.IsDeletedStatu != true)
                    .Select(l => l.AttributeDefinitionId)
                    .Distinct()
                    .ToListAsync(cancellationToken);
            }

            // 5) "Zaten bilinen" canonical kodları prompt'a enjekte etmek için topla.
            var knownIds = new HashSet<Guid>(kb.KnownAttributeIds);
            foreach (var t in kb.TemplateAttributes) knownIds.Add(t.AttributeId);
            foreach (var i in kb.InheritedAttributeIds) knownIds.Add(i);

            if (knownIds.Count > 0)
            {
                var codes = await db.Set<AttributeDefinition>().AsNoTracking()
                    .Where(a => knownIds.Contains(a.Id))
                    .Select(a => a.CanonicalCode)
                    .ToListAsync(cancellationToken);
                foreach (var c in codes)
                    if (!string.IsNullOrWhiteSpace(c)) kb.KnownCanonicalCodes.Add(c);
            }

            return kb;
        }

        private sealed class GlobalIndex
        {
            public Dictionary<string, AttrIndexRow> ByCanonicalCode = new(StringComparer.Ordinal);
            public Dictionary<string, AttrIndexRow> BySynonymToken = new(StringComparer.Ordinal);
            public Dictionary<string, AttrIndexRow> ByNormalizedName = new(StringComparer.Ordinal);
        }

        private const string GlobalIndexCacheKey = "aiattr:global-index";

        private async Task<GlobalIndex> GetOrLoadGlobalIndexAsync(CancellationToken cancellationToken)
        {
            if (_cache.TryGetValue(GlobalIndexCacheKey, out GlobalIndex? cached) && cached != null)
                return cached;

            await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken);

            var attrs = await db.Set<AttributeDefinition>().AsNoTracking()
                .Where(a => a.IsDeleted.IsDeletedStatu != true)
                .Select(a => new AttrIndexRow(a.Id, a.CanonicalCode, a.DataType, a.UnitGroupId))
                .ToListAsync(cancellationToken);

            var syn = await db.Set<AttributeSynonym>().AsNoTracking()
                .Where(s => s.IsDeleted.IsDeletedStatu != true)
                .Select(s => new { s.NormalizedToken, s.AttributeDefinitionId })
                .ToListAsync(cancellationToken);

            var names = await db.Set<AttributeDefinition>().AsNoTracking()
                .Where(a => a.IsDeleted.IsDeletedStatu != true)
                .Select(a => new { a.NormalizedName, a.Id })
                .ToListAsync(cancellationToken);

            var idx = new GlobalIndex();
            var byId = attrs.ToDictionary(a => a.Id);

            foreach (var a in attrs)
                if (!string.IsNullOrWhiteSpace(a.CanonicalCode))
                    idx.ByCanonicalCode[a.CanonicalCode] = a;

            foreach (var s in syn)
                if (!string.IsNullOrWhiteSpace(s.NormalizedToken) && byId.TryGetValue(s.AttributeDefinitionId, out var row))
                    idx.BySynonymToken[s.NormalizedToken] = row;

            foreach (var n in names)
                if (!string.IsNullOrWhiteSpace(n.NormalizedName) && byId.TryGetValue(n.Id, out var row))
                    idx.ByNormalizedName[n.NormalizedName] = row; // çakışmada son yazan; dedup guard dataType ile korur

            _cache.Set(GlobalIndexCacheKey, idx, TimeSpan.FromMinutes(_opt.KnowledgeIndexCacheTtlMinutes));
            return idx;
        }

        private void InvalidateKnowledgeIndex() => _cache.Remove(GlobalIndexCacheKey);

        private async Task<List<Guid>> GetAncestorCategoryIdsAsync(
            _ApplicationConnectionDb db, Guid categoryId, CancellationToken cancellationToken)
        {
            var result = new List<Guid>();
            var current = categoryId;
            for (int depth = 0; depth < _opt.MaxInheritanceDepth; depth++)
            {
                var parentId = await db.Set<CategoriesProduct>().AsNoTracking()
                    .Where(c => c.Id == current)
                    .Select(c => c.ParentId)
                    .FirstOrDefaultAsync(cancellationToken);
                if (parentId == null || parentId == Guid.Empty || result.Contains(parentId.Value)) break;
                result.Add(parentId.Value);
                current = parentId.Value;
            }
            return result;
        }

        /// <summary>Template + inheritance ile bilinen attribute'ları kategoriye bağlar (LLM'siz reuse, idempotent).</summary>
        private async Task<int> ApplyKnownLinksAsync(
            Guid categoryId, string categoryPath, KnowledgeContext kb, Guid jobId, CancellationToken cancellationToken)
        {
            int linked = 0;
            await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken);
            int order = kb.NextDisplayOrder;

            // Template attribute'ları (birincil kaynak).
            foreach (var t in kb.TemplateAttributes)
            {
                if (kb.KnownAttributeIds.Contains(t.AttributeId)) continue;
                if (await LinkExistsAsync(db, categoryId, t.AttributeId, cancellationToken)) { kb.KnownAttributeIds.Add(t.AttributeId); continue; }

                db.Set<CategoryAttribute>().Add(new CategoryAttribute
                {
                    CategoryId = categoryId,
                    AttributeDefinitionId = t.AttributeId,
                    IsInherited = false,
                    IsOverride = false,
                    IsSuppressed = false,
                    IsRequired = t.IsRequired,
                    IsVariant = t.IsVariant,
                    IsFilterable = t.IsFilterable,
                    IsComparable = t.IsComparable,
                    DisplayOrder = order++,
                    IsActive = true,
                    Ai = BuildAi(categoryPath, ContentSource.System, AiApprovalStatus.Approved)
                });
                kb.KnownAttributeIds.Add(t.AttributeId);
                linked++;
            }

            // Miras attribute'ları.
            foreach (var attrId in kb.InheritedAttributeIds)
            {
                if (kb.KnownAttributeIds.Contains(attrId)) continue;
                if (await LinkExistsAsync(db, categoryId, attrId, cancellationToken)) { kb.KnownAttributeIds.Add(attrId); continue; }

                db.Set<CategoryAttribute>().Add(new CategoryAttribute
                {
                    CategoryId = categoryId,
                    AttributeDefinitionId = attrId,
                    IsInherited = true,
                    IsOverride = false,
                    IsSuppressed = false,
                    DisplayOrder = order++,
                    IsActive = true,
                    Ai = BuildAi(categoryPath, ContentSource.System, AiApprovalStatus.Approved)
                });
                kb.KnownAttributeIds.Add(attrId);
                linked++;
            }

            if (linked > 0)
            {
                try { await db.SaveChangesAsync(cancellationToken); }
                catch (DbUpdateException) { db.ChangeTracker.Clear(); linked = 0; } // yarış → yok say, LLM aşaması yine idempotent kurar
            }

            kb.NextDisplayOrder = order;
            return linked;
        }

        // ════════════════════════════════════════════════════════════════════
        //  DECISION + İDEMPOTENT KALICILIK (attribute başına ayrı transaction)
        // ════════════════════════════════════════════════════════════════════

        private enum AttrAction { Create, Reuse, Expand, Ignore }
        private sealed record AttrOutcome(AttrAction Action, int CreatedRecordCount);

        private async Task<AttrOutcome> PersistAttributeDecisionAsync(
            Guid categoryId, string categoryPath, AiAttribute aiAttr, int order,
            Guid jobId, string promptHash, string promptVersion, CancellationToken cancellationToken)
        {
            string trName = GetName(aiAttr.Names, Language.Tr) ?? aiAttr.CanonicalCode ?? "";
            if (string.IsNullOrWhiteSpace(trName)) return new AttrOutcome(AttrAction.Ignore, 0);

            var tokens = BuildAttributeTokens(aiAttr);
            var dataType = ParseEnum(aiAttr.DataType, AttributeDataType.Text);

            await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken);
            var strategy = db.Database.CreateExecutionStrategy();

            AttrOutcome outcome = new(AttrAction.Ignore, 0);

            await strategy.ExecuteAsync(async () =>
            {
                // Retry sırasında taze context şart (aksi halde çift-tracking). Bu yüzden context'i
                // strateji delegesinin İÇİNDE oluşturuyoruz.
                await using var innerDb = await _dbFactory.CreateDbContextAsync(cancellationToken);
                await using var tx = await innerDb.Database.BeginTransactionAsync(cancellationToken);

                // ── DECISION: KB'de eşleşen var mı? (dataType-uyumu guard'lı) ──
                var reuse = await ResolveReuseAsync(innerDb, aiAttr, tokens, dataType, cancellationToken);

                Guid attributeId;
                int records = 0;
                AttrAction action;

                if (reuse.AttributeId.HasValue)
                {
                    // REUSE (+EXPAND: eksik option'ları ekle).
                    attributeId = reuse.AttributeId.Value;
                    action = AttrAction.Reuse;

                    if (aiAttr.Options is { Count: > 0 })
                    {
                        int addedOpt = await UpsertOptionsAsync(innerDb, attributeId, aiAttr, categoryPath, cancellationToken);
                        records += addedOpt;
                        if (addedOpt > 0) action = AttrAction.Expand;
                    }

                    await WriteHistoryAsync(innerDb, nameof(AttributeDefinition), attributeId, jobId,
                        action == AttrAction.Expand ? "Expanded" : "Reused", promptHash, promptVersion, reuse.Confidence, cancellationToken);
                }
                else if (reuse.Confidence >= 0.50m)
                {
                    // CREATE CANDIDATE — güven eşiğine göre onay durumu.
                    var approval = reuse.Confidence >= _opt.AutoApproveConfidence
                        ? AiApprovalStatus.Approved
                        : AiApprovalStatus.PendingApproval;

                    attributeId = await CreateAttributeAsync(innerDb, aiAttr, trName, tokens, dataType, categoryPath, order, approval, cancellationToken);
                    records += 1 + AllLanguages.Length;
                    action = AttrAction.Create;

                    if (aiAttr.Options is { Count: > 0 })
                        records += await UpsertOptionsAsync(innerDb, attributeId, aiAttr, categoryPath, cancellationToken);

                    await WriteHistoryAsync(innerDb, nameof(AttributeDefinition), attributeId, jobId,
                        "Created", promptHash, promptVersion, reuse.Confidence, cancellationToken);
                }
                else
                {
                    // IGNORE (< %50): logla, kayıt oluşturma.
                    _logger.LogDebug("Attribute yok sayıldı (düşük güven {Conf:P0}) → {Name} @ {Path}",
                        reuse.Confidence, trName, categoryPath);
                    await tx.RollbackAsync(cancellationToken);
                    outcome = new AttrOutcome(AttrAction.Ignore, 0);
                    return;
                }

                // ── Kategori ↔ attribute bağı (idempotent) ──
                if (!await LinkExistsAsync(innerDb, categoryId, attributeId, cancellationToken))
                {
                    innerDb.Set<CategoryAttribute>().Add(BuildCategoryLink(categoryId, categoryPath, attributeId, aiAttr, order));
                    records += 1;
                }

                try
                {
                    await innerDb.SaveChangesAsync(cancellationToken);
                    await tx.CommitAsync(cancellationToken);
                    outcome = new AttrOutcome(action, records);
                }
                catch (DbUpdateException ex)
                {
                    // TOCTOU: eşzamanlı bir worker aynı canonical/synonym'i ekledi.
                    // Rollback + mevcut kazananı yeniden çözüp REUSE'a düş (converge, exception fırlatma).
                    await tx.RollbackAsync(cancellationToken);
                    _logger.LogInformation(ex, "Attribute yarışı → yeniden çözülüyor: {Name}", trName);

                    await using var convergeDb = await _dbFactory.CreateDbContextAsync(cancellationToken);
                    var winner = await ResolveReuseAsync(convergeDb, aiAttr, tokens, dataType, cancellationToken);
                    if (winner.AttributeId.HasValue)
                    {
                        await using var tx2 = await convergeDb.Database.BeginTransactionAsync(cancellationToken);
                        int r2 = 0;
                        if (aiAttr.Options is { Count: > 0 })
                            r2 += await UpsertOptionsAsync(convergeDb, winner.AttributeId.Value, aiAttr, categoryPath, cancellationToken);
                        if (!await LinkExistsAsync(convergeDb, categoryId, winner.AttributeId.Value, cancellationToken))
                        {
                            convergeDb.Set<CategoryAttribute>().Add(BuildCategoryLink(categoryId, categoryPath, winner.AttributeId.Value, aiAttr, order));
                            r2++;
                        }
                        try { await convergeDb.SaveChangesAsync(cancellationToken); await tx2.CommitAsync(cancellationToken); }
                        catch (DbUpdateException) { await tx2.RollbackAsync(cancellationToken); }
                        outcome = new AttrOutcome(AttrAction.Reuse, r2);
                    }
                    else
                    {
                        outcome = new AttrOutcome(AttrAction.Ignore, 0);
                    }
                }
            });

            return outcome;
        }

        private sealed record ReuseResult(Guid? AttributeId, decimal Confidence);

        /// <summary>
        /// KB-First eşleştirme + deterministik confidence. Embedding YOK (extension point);
        /// synonym/canonical/normalized-name/translation/option sinyalleriyle skorlar.
        /// KRİTİK GUARD: synonym/name eşleşmesi ancak dataType (ölçüde ayrıca birim boyutu)
        /// UYUMLUYSA reuse sayılır → jenerik kelimelerle cross-domain yanlış merge önlenir.
        /// </summary>
        private async Task<ReuseResult> ResolveReuseAsync(
            _ApplicationConnectionDb db, AiAttribute aiAttr, HashSet<string> tokens,
            AttributeDataType dataType, CancellationToken cancellationToken)
        {
            string? canon = TextNormalizer.ToCanonicalCode(aiAttr.CanonicalCode);

            // 1) Canonical tam eşleşme (en güçlü, ayrıştırıcı zaten kodda).
            if (!string.IsNullOrWhiteSpace(canon))
            {
                var byCode = await db.Set<AttributeDefinition>().AsNoTracking()
                    .Where(a => a.CanonicalCode == canon && a.IsDeleted.IsDeletedStatu != true)
                    .Select(a => new { a.Id, a.DataType })
                    .FirstOrDefaultAsync(cancellationToken);
                if (byCode != null && DataTypeCompatible(byCode.DataType, dataType))
                    return new ReuseResult(byCode.Id, 0.97m);
            }

            // 2) Synonym token eşleşmesi (dataType guard'lı).
            if (tokens.Count > 0)
            {
                var synHit = await db.Set<AttributeSynonym>().AsNoTracking()
                    .Where(s => tokens.Contains(s.NormalizedToken)
                                && s.IsDeleted.IsDeletedStatu != true)
                    .Select(s => s.AttributeDefinitionId)
                    .FirstOrDefaultAsync(cancellationToken);
                if (synHit != Guid.Empty)
                {
                    var dt = await db.Set<AttributeDefinition>().AsNoTracking()
                        .Where(a => a.Id == synHit).Select(a => (AttributeDataType?)a.DataType).FirstOrDefaultAsync(cancellationToken);
                    if (dt.HasValue && DataTypeCompatible(dt.Value, dataType))
                        return new ReuseResult(synHit, 0.95m);
                    // Uyumsuz → jenerik token cross-domain; reuse ETME, ayrı oluştur.
                }
            }

            // 3) Normalized-name eşleşmesi (yalnızca dataType uyumlu + jenerik değilse).
            foreach (var t in tokens)
            {
                if (GenericStopTokens.Contains(t)) continue;
                var nameHit = await db.Set<AttributeDefinition>().AsNoTracking()
                    .Where(a => a.NormalizedName == t && a.IsDeleted.IsDeletedStatu != true)
                    .Select(a => new { a.Id, a.DataType })
                    .FirstOrDefaultAsync(cancellationToken);
                if (nameHit != null && DataTypeCompatible(nameHit.DataType, dataType))
                    return new ReuseResult(nameHit.Id, 0.90m);
            }

            // Eşleşme yok → yeni oluşturma adayı. Güven: adın anlamlılığına göre kaba skor.
            decimal createConf = EstimateCreateConfidence(aiAttr, tokens);
            return new ReuseResult(null, createConf);
        }

        /// <summary>Yeni attribute için kaba güven: token zenginliği + çeviri tamlığı + anlamlı ad.</summary>
        private static decimal EstimateCreateConfidence(AiAttribute aiAttr, HashSet<string> tokens)
        {
            decimal score = 0.55m; // taban: LLM önerdi
            int langCount = aiAttr.Names?.Count(kv => !string.IsNullOrWhiteSpace(kv.Value)) ?? 0;
            score += Math.Min(0.20m, langCount * 0.02m);           // çeviri tamlığı
            score += Math.Min(0.15m, (tokens.Count - 1) * 0.03m);   // ayrıştırıcı token zenginliği
            string canon = TextNormalizer.ToCanonicalCode(aiAttr.CanonicalCode);
            if (!string.IsNullOrWhiteSpace(canon) && canon.Contains('_')) score += 0.05m; // bağlam ayrıştırıcı kod
            // Anlamsız/çöp ad → düşük tutar.
            string tr = aiAttr.Names != null && aiAttr.Names.TryGetValue("tr", out var v) ? v : "";
            if (tr.Length < 2 || tr.All(char.IsDigit)) score = 0.30m;
            return Math.Clamp(score, 0m, 0.94m); // create yolu asla ≥0.95 (o REUSE bandı)
        }

        // ── Attribute oluştur (10 dil çeviri + guard'lı synonym) ──────────────
        private async Task<Guid> CreateAttributeAsync(
            _ApplicationConnectionDb db, AiAttribute aiAttr, string trName, HashSet<string> tokens,
            AttributeDataType dataType, string categoryPath, int order, AiApprovalStatus approval, CancellationToken cancellationToken)
        {
            var inputType = CoerceInputType(dataType, aiAttr.InputType);

            var attr = new AttributeDefinition
            {
                CanonicalCode = await MakeUniqueAttributeCodeAsync(db, aiAttr.CanonicalCode, trName, cancellationToken),
                NormalizedName = TextNormalizer.Normalize(trName),
                CanonicalName = trName,
                DataType = dataType,
                InputType = inputType,
                IsRequiredByDefault = aiAttr.IsRequired ?? false,
                IsVariantByDefault = aiAttr.IsVariant ?? false,
                IsFilterableByDefault = aiAttr.IsFilterable ?? false,
                IsSearchableByDefault = aiAttr.IsSearchable ?? false,
                IsComparableByDefault = aiAttr.IsComparable ?? false,
                IsKeyAttribute = aiAttr.IsKey ?? false,
                IsMultiValued = dataType == AttributeDataType.MultiSelect,
                DisplayOrder = order,
                IsActive = true,
                Version = 1,
                Ai = BuildAi(categoryPath, ContentSource.Ai, approval)
            };

            // Ölçü tipi → birim bağla (UnitGroup/BaseUnit). Eksikti; artık dolduruluyor.
            if (dataType == AttributeDataType.Measurement && !string.IsNullOrWhiteSpace(aiAttr.Unit))
            {
                var (unitGroupId, baseUnitId) = await ResolveUnitAsync(db, aiAttr.Unit!, cancellationToken);
                attr.UnitGroupId = unitGroupId;
                attr.BaseUnitId = baseUnitId;
            }

            db.Set<AttributeDefinition>().Add(attr);

            foreach (var lang in AllLanguages)
            {
                string name = GetName(aiAttr.Names, lang) ?? trName; // fallback: TR
                db.Set<AttributeTranslation>().Add(new AttributeTranslation
                {
                    AttributeDefinitionId = attr.Id,
                    Language = lang,
                    Name = name,
                    Source = ContentSource.Ai,
                    IsManuallyEdited = false
                });
            }

            // Synonym: global-unique. Jenerik tek-kelime token'ları global kilitleme.
            foreach (var token in tokens)
            {
                if (string.IsNullOrWhiteSpace(token)) continue;
                if (GenericStopTokens.Contains(token)) continue; // cross-domain merge kaynağı → yazma
                bool exists = await db.Set<AttributeSynonym>()
                    .AnyAsync(s => s.NormalizedToken == token, cancellationToken);
                if (exists) continue;

                db.Set<AttributeSynonym>().Add(new AttributeSynonym
                {
                    AttributeDefinitionId = attr.Id,
                    Token = token,
                    NormalizedToken = token,
                    Source = ContentSource.Ai
                });
            }

            return attr.Id;
        }

        // ── Option'ları ekle/genişlet (vocabulary expansion, idempotent) ──────
        private async Task<int> UpsertOptionsAsync(
            _ApplicationConnectionDb db, Guid attributeId, AiAttribute aiAttr, string categoryPath, CancellationToken cancellationToken)
        {
            int created = 0, order = 0;

            foreach (var aiOpt in aiAttr.Options!)
            {
                if (aiOpt == null) { order++; continue; }

                string trValue = GetName(aiOpt.Values, Language.Tr) ?? aiOpt.CanonicalCode ?? "";
                if (string.IsNullOrWhiteSpace(trValue)) { order++; continue; }

                var optTokens = BuildOptionTokens(aiOpt);
                if (await OptionExistsAsync(db, attributeId, aiOpt.CanonicalCode, optTokens, cancellationToken)) { order++; continue; }

                var option = new AttributeOption
                {
                    AttributeDefinitionId = attributeId,
                    CanonicalCode = await MakeUniqueOptionCodeAsync(db, attributeId, aiOpt.CanonicalCode, trValue, cancellationToken),
                    NormalizedValue = TextNormalizer.Normalize(trValue),
                    CanonicalValue = trValue,
                    ColorHex = string.IsNullOrWhiteSpace(aiOpt.ColorHex) ? null : aiOpt.ColorHex,
                    NumericValue = aiOpt.NumericValue,
                    DisplayOrder = order,
                    IsActive = true,
                    Ai = BuildAi(categoryPath, ContentSource.Ai, AiApprovalStatus.PendingApproval)
                };
                db.Set<AttributeOption>().Add(option);

                foreach (var lang in AllLanguages)
                {
                    string val = GetName(aiOpt.Values, lang) ?? trValue;
                    db.Set<AttributeOptionTranslation>().Add(new AttributeOptionTranslation
                    {
                        AttributeOptionId = option.Id,
                        Language = lang,
                        Value = val,
                        Source = ContentSource.Ai,
                        IsManuallyEdited = false
                    });
                }

                foreach (var token in optTokens)
                {
                    if (string.IsNullOrWhiteSpace(token)) continue;
                    bool synExists = await db.Set<AttributeOptionSynonym>()
                        .AnyAsync(s => s.AttributeDefinitionId == attributeId && s.NormalizedToken == token, cancellationToken);
                    if (synExists) continue;

                    db.Set<AttributeOptionSynonym>().Add(new AttributeOptionSynonym
                    {
                        AttributeOptionId = option.Id,
                        AttributeDefinitionId = attributeId,
                        Token = token,
                        NormalizedToken = token,
                        Source = ContentSource.Ai
                    });
                }

                created++;
                order++;
            }

            return created;
        }

        private static async Task<bool> OptionExistsAsync(
            _ApplicationConnectionDb db, Guid attributeId, string? canonicalCode, HashSet<string> tokens, CancellationToken cancellationToken)
        {
            if (tokens.Count > 0)
            {
                bool synHit = await db.Set<AttributeOptionSynonym>()
                    .AnyAsync(s => s.AttributeDefinitionId == attributeId && tokens.Contains(s.NormalizedToken), cancellationToken);
                if (synHit) return true;

                bool valHit = await db.Set<AttributeOption>()
                    .AnyAsync(o => o.AttributeDefinitionId == attributeId && tokens.Contains(o.NormalizedValue), cancellationToken);
                if (valHit) return true;
            }

            string? code = TextNormalizer.ToCanonicalCode(canonicalCode);
            if (!string.IsNullOrWhiteSpace(code))
                return await db.Set<AttributeOption>()
                    .AnyAsync(o => o.AttributeDefinitionId == attributeId && o.CanonicalCode == code, cancellationToken);

            return false;
        }

        // ── Birim çözümleme (ör. "mah" → UnitGroup/BaseUnit) ──────────────────
        private static async Task<(Guid? unitGroupId, Guid? baseUnitId)> ResolveUnitAsync(
            _ApplicationConnectionDb db, string unitCode, CancellationToken cancellationToken)
        {
            string code = TextNormalizer.ToCanonicalCode(unitCode);
            if (string.IsNullOrWhiteSpace(code)) return (null, null);

            var unit = await db.Set<Unit>().AsNoTracking()
                .Where(u => u.CanonicalCode == code && u.IsDeleted.IsDeletedStatu != true)
                .Select(u => new { u.Id, u.UnitGroupId })
                .FirstOrDefaultAsync(cancellationToken);
            if (unit == null) return (null, null); // birim kataloğunda yoksa null bırak (seed işi ayrı)

            var baseUnitId = await db.Set<UnitGroup>().AsNoTracking()
                .Where(g => g.Id == unit.UnitGroupId)
                .Select(g => g.BaseUnitId)
                .FirstOrDefaultAsync(cancellationToken);

            return (unit.UnitGroupId, baseUnitId ?? unit.Id);
        }

        // ════════════════════════════════════════════════════════════════════
        //  GEMINI ÇAĞRISI + PROMPT CACHE + RETRY (exponential backoff + jitter)
        // ════════════════════════════════════════════════════════════════════

        private async Task<AiAttributeSchema?> GetSchemaCachedOrGenerateAsync(
            string categoryPath, string version, string promptHash, string prompt, object? responseSchema, CancellationToken cancellationToken)
        {
            // PROMPT CACHE (L1): aynı kategori+sürüm+prompt → LLM çağrısı yok.
            string cacheKey = $"aiattr:schema:{ComputeHash($"{categoryPath}|{version}|{promptHash}")}";
            if (_cache.TryGetValue(cacheKey, out AiAttributeSchema? cached))
                return cached;

            var schema = await GenerateSchemaWithRetryAsync(categoryPath, prompt, responseSchema, cancellationToken);

            // Yalnızca anlamlı sonucu cache'le (boş sonuç kategoriyi kilitlemesin).
            if (schema?.Attributes is { Count: > 0 })
                _cache.Set(cacheKey, schema, TimeSpan.FromHours(_opt.PromptCacheTtlHours));

            return schema;
        }

        private async Task<AiAttributeSchema?> GenerateSchemaWithRetryAsync(
            string categoryPath, string prompt, object? responseSchema, CancellationToken cancellationToken)
        {
            var rnd = new Random();

            for (int attempt = 1; attempt <= _opt.MaxLlmAttempts; attempt++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    var (ok, retryable, retryAfter, rawText) = await CallGeminiAsync(prompt, responseSchema, cancellationToken);

                    if (ok && !string.IsNullOrWhiteSpace(rawText))
                    {
                        var schema = JsonValidator.ParseWithRepair(rawText, _logger, categoryPath);
                        if (schema != null) return schema;

                        // JSON tamir edilemedi → bir kez daha dene (LLM bazen düzgün döner).
                        _logger.LogWarning("AIAttr JSON tamir edilemedi (deneme {Attempt}) → {Path}", attempt, categoryPath);
                    }
                    else if (!retryable)
                    {
                        _logger.LogWarning("AIAttr LLM kalıcı hata (retry yok) → {Path}", categoryPath);
                        return null;
                    }

                    if (attempt < _opt.MaxLlmAttempts)
                        await Task.Delay(ComputeBackoff(attempt, retryAfter, rnd), cancellationToken);
                }
                catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
                {
                    _logger.LogWarning("AIAttr LLM zaman aşımı (deneme {Attempt}) → {Path}", attempt, categoryPath);
                    if (attempt < _opt.MaxLlmAttempts)
                        await Task.Delay(ComputeBackoff(attempt, null, rnd), cancellationToken);
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogWarning(ex, "AIAttr LLM HTTP hatası (deneme {Attempt}) → {Path}", attempt, categoryPath);
                    if (attempt < _opt.MaxLlmAttempts)
                        await Task.Delay(ComputeBackoff(attempt, null, rnd), cancellationToken);
                }
            }

            return null; // tüm denemeler tükendi → job backoff ile yeniden alınır
        }

        private static TimeSpan ComputeBackoff(int attempt, TimeSpan? retryAfter, Random rnd)
        {
            if (retryAfter.HasValue && retryAfter.Value > TimeSpan.Zero)
                return retryAfter.Value;
            double baseSec = Math.Pow(2, attempt);            // 2,4,8,16...
            double jitter = rnd.NextDouble() * baseSec * 0.3;  // %30 jitter
            return TimeSpan.FromSeconds(Math.Min(60, baseSec + jitter));
        }

        private async Task<(bool ok, bool retryable, TimeSpan? retryAfter, string? text)> CallGeminiAsync(
            string prompt, object? responseSchema, CancellationToken cancellationToken)
        {
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/{_opt.Model}:generateContent";

            object generationConfig = responseSchema == null
                ? new { responseMimeType = "application/json", temperature = 0.2 }
                : new { responseMimeType = "application/json", responseSchema, temperature = 0.2 };

            var requestBody = new
            {
                contents = new[] { new { parts = new[] { new { text = prompt } } } },
                generationConfig
            };

            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(TimeSpan.FromSeconds(_opt.HttpTimeoutSeconds));

            using var req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Headers.Add("x-goog-api-key", _opt.ApiKey); // anahtar header'da → URL/log'a düşmez
            req.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            using var resp = await _httpClient.SendAsync(req, timeoutCts.Token);

            if (!resp.IsSuccessStatusCode)
            {
                bool retryable = resp.StatusCode == HttpStatusCode.TooManyRequests
                                 || (int)resp.StatusCode >= 500;
                TimeSpan? retryAfter = resp.Headers.RetryAfter?.Delta;
                _logger.LogWarning("Gemini HTTP {Code} (retryable={Retry})", (int)resp.StatusCode, retryable);
                return (false, retryable, retryAfter, null);
            }

            string json = await resp.Content.ReadAsStringAsync(timeoutCts.Token);

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            if (!root.TryGetProperty("candidates", out var candidates) || candidates.GetArrayLength() == 0)
                return (true, false, null, null); // boş ama başarılı → tamir edilecek metin yok

            string? text = candidates[0].TryGetProperty("content", out var content)
                           && content.TryGetProperty("parts", out var parts) && parts.GetArrayLength() > 0
                           && parts[0].TryGetProperty("text", out var t)
                ? t.GetString()
                : null;

            return (true, false, null, text);
        }

        // ════════════════════════════════════════════════════════════════════
        //  İŞ DURUMU / KATEGORİ SONLANDIRMA / AUDIT
        // ════════════════════════════════════════════════════════════════════

        private async Task FinalizeCategoryAsync(Guid categoryId, CancellationToken cancellationToken)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken);
            var cat = await db.Set<CategoriesProduct>().FirstOrDefaultAsync(c => c.Id == categoryId, cancellationToken);
            if (cat == null) return;
            cat.AiAttributesIsOk = true;
            cat.UpdatedDate = DateTime.UtcNow;
            await db.SaveChangesAsync(cancellationToken);
        }

        private async Task CompleteJobAsync(Guid jobId, AiJobStatus status, string summary, int createdCount, CancellationToken cancellationToken)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken);
            var job = await db.Set<AiGenerationJob>().FirstOrDefaultAsync(j => j.Id == jobId, cancellationToken);
            if (job == null) return;
            job.Status = status;
            job.CompletedAtUtc = DateTime.UtcNow;
            job.UpdatedAtUtc = DateTime.UtcNow;
            job.ResultSummary = Truncate(summary, 2048);
            job.CreatedRecordCount = createdCount;
            job.LeasedBy = null;
            job.LeasedUntilUtc = null;
            try { await db.SaveChangesAsync(cancellationToken); }
            catch (DbUpdateConcurrencyException) { /* başka worker güncelledi; yok say */ }
        }

        private async Task FailJobWithBackoffAsync(AiGenerationJob job, string error, CancellationToken cancellationToken)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken);
            var fresh = await db.Set<AiGenerationJob>().FirstOrDefaultAsync(j => j.Id == job.Id, cancellationToken);
            if (fresh == null) return;

            fresh.ErrorMessage = Truncate(error, 4000);
            fresh.UpdatedAtUtc = DateTime.UtcNow;
            fresh.LeasedBy = null;
            fresh.LeasedUntilUtc = null;

            if (fresh.AttemptCount >= fresh.MaxAttempts)
            {
                // POISON İZOLASYONU: kuyruğu bloklamaz; admin incelemesine düşer.
                fresh.Status = AiJobStatus.NeedsReview;
                fresh.NextRetryAtUtc = null;
                _logger.LogWarning("AIAttr iş MaxAttempts aştı → NeedsReview: {Path}", fresh.CategoryPathSnapshot);
            }
            else
            {
                fresh.Status = AiJobStatus.Failed;
                double backoff = Math.Min(3600, Math.Pow(2, fresh.AttemptCount) * 30); // 60s,120s,240s... max 1saat
                fresh.NextRetryAtUtc = DateTime.UtcNow.AddSeconds(backoff);
            }

            try { await db.SaveChangesAsync(cancellationToken); }
            catch (DbUpdateConcurrencyException) { /* başka worker; yok say */ }
        }

        private static async Task WriteHistoryAsync(
            _ApplicationConnectionDb db, string entityType, Guid entityId, Guid jobId,
            string action, string? promptHash, string? promptVersion, decimal? confidence, CancellationToken cancellationToken)
        {
            db.Set<AiGenerationHistory>().Add(new AiGenerationHistory
            {
                EntityType = entityType,
                EntityId = entityId,
                AiGenerationJobId = jobId == Guid.Empty ? null : jobId,
                Action = action,
                PromptHash = promptHash,
                PromptVersion = promptVersion,
                Confidence = confidence,
                Source = ContentSource.Ai,
                CreatedAtUtc = DateTime.UtcNow
            });
            await Task.CompletedTask; // SaveChanges dış transaction ile yapılır
        }

        // ════════════════════════════════════════════════════════════════════
        //  YARDIMCILAR
        // ════════════════════════════════════════════════════════════════════

        private static async Task<bool> LinkExistsAsync(
            _ApplicationConnectionDb db, Guid categoryId, Guid attributeId, CancellationToken cancellationToken)
            => await db.Set<CategoryAttribute>()
                .AnyAsync(ca => ca.CategoryId == categoryId
                                && ca.AttributeDefinitionId == attributeId
                                && ca.IsDeleted.IsDeletedStatu != true, cancellationToken);

        private static CategoryAttribute BuildCategoryLink(
            Guid categoryId, string categoryPath, Guid attributeId, AiAttribute aiAttr, int order)
            => new()
            {
                CategoryId = categoryId,
                AttributeDefinitionId = attributeId,
                IsInherited = false,
                IsOverride = false,
                IsSuppressed = false,
                IsRequired = aiAttr.IsRequired,
                IsVariant = aiAttr.IsVariant,
                IsFilterable = aiAttr.IsFilterable,
                IsSearchable = aiAttr.IsSearchable,
                IsComparable = aiAttr.IsComparable,
                DisplayOrder = order,
                IsActive = true,
                Ai = BuildAi(categoryPath, ContentSource.Ai, AiApprovalStatus.PendingApproval)
            };

        private static async Task<string> MakeUniqueAttributeCodeAsync(
            _ApplicationConnectionDb db, string? rawCode, string fallbackName, CancellationToken cancellationToken)
        {
            string baseCode = TextNormalizer.ToCanonicalCode(string.IsNullOrWhiteSpace(rawCode) ? fallbackName : rawCode);
            if (string.IsNullOrWhiteSpace(baseCode)) baseCode = "attr";
            string code = baseCode;
            int i = 2;
            while (await db.Set<AttributeDefinition>().AnyAsync(a => a.CanonicalCode == code, cancellationToken))
                code = $"{baseCode}_{i++}";
            return code;
        }

        private static async Task<string> MakeUniqueOptionCodeAsync(
            _ApplicationConnectionDb db, Guid attributeId, string? rawCode, string fallbackValue, CancellationToken cancellationToken)
        {
            string baseCode = TextNormalizer.ToCanonicalCode(string.IsNullOrWhiteSpace(rawCode) ? fallbackValue : rawCode);
            if (string.IsNullOrWhiteSpace(baseCode)) baseCode = "opt";
            string code = baseCode;
            int i = 2;
            while (await db.Set<AttributeOption>()
                .AnyAsync(o => o.AttributeDefinitionId == attributeId && o.CanonicalCode == code, cancellationToken))
                code = $"{baseCode}_{i++}";
            return code;
        }

        private static HashSet<string> BuildAttributeTokens(AiAttribute aiAttr)
        {
            var set = new HashSet<string>(StringComparer.Ordinal);
            if (aiAttr.Synonyms != null) foreach (var s in aiAttr.Synonyms) AddToken(set, s);
            if (aiAttr.Names != null) foreach (var kv in aiAttr.Names) AddToken(set, kv.Value);
            AddToken(set, aiAttr.CanonicalCode);
            return set;
        }

        private static HashSet<string> BuildOptionTokens(AiOption aiOpt)
        {
            var set = new HashSet<string>(StringComparer.Ordinal);
            if (aiOpt.Synonyms != null) foreach (var s in aiOpt.Synonyms) AddToken(set, s);
            if (aiOpt.Values != null) foreach (var kv in aiOpt.Values) AddToken(set, kv.Value);
            AddToken(set, aiOpt.CanonicalCode);
            return set;
        }

        private static void AddToken(HashSet<string> set, string? raw)
        {
            string norm = TextNormalizer.Normalize(raw);
            if (!string.IsNullOrWhiteSpace(norm)) set.Add(norm);
        }

        private static string? GetName(Dictionary<string, string>? map, Language lang)
        {
            if (map == null) return null;
            string code = lang.ToCultureCode();
            if (map.TryGetValue(code, out var v) && !string.IsNullOrWhiteSpace(v)) return v.Trim();
            foreach (var kv in map)
                if (string.Equals(kv.Key, code, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(kv.Value))
                    return kv.Value.Trim();
            return null;
        }

        private static AiMetadata BuildAi(string categoryPath, ContentSource source, AiApprovalStatus approval) => new()
        {
            Source = source,
            AiModel = source == ContentSource.Ai ? "gemini" : null,
            PromptVersion = PromptBuilder.CurrentDefaultVersion,
            AiGeneratedAtUtc = DateTime.UtcNow,
            ApprovalStatus = approval,
            ReviewStatus = AiReviewStatus.NotReviewed,
            CategorySource = Truncate(categoryPath, 1024),
            IsManuallyEdited = false
        };

        private static bool DataTypeCompatible(AttributeDataType existing, AttributeDataType incoming)
        {
            if (existing == incoming) return true;
            // Seçim aileleri kendi içinde uyumlu sayılır (Dropdown vs radio farkı reuse'u engellemesin).
            bool ExistingSelect = existing is AttributeDataType.SingleSelect or AttributeDataType.MultiSelect;
            bool IncomingSelect = incoming is AttributeDataType.SingleSelect or AttributeDataType.MultiSelect;
            if (ExistingSelect && IncomingSelect) return true;
            // Metin aileleri.
            bool ExistingText = existing is AttributeDataType.Text or AttributeDataType.MultilineText or AttributeDataType.RichText;
            bool IncomingText = incoming is AttributeDataType.Text or AttributeDataType.MultilineText or AttributeDataType.RichText;
            if (ExistingText && IncomingText) return true;
            // Sayı aileleri.
            bool ExistingNum = existing is AttributeDataType.Integer or AttributeDataType.Decimal or AttributeDataType.Measurement;
            bool IncomingNum = incoming is AttributeDataType.Integer or AttributeDataType.Decimal or AttributeDataType.Measurement;
            if (ExistingNum && IncomingNum) return true;
            return false;
        }

        private static AttributeInputType CoerceInputType(AttributeDataType dataType, string? aiInput)
        {
            var parsed = ParseEnum(aiInput, AttributeInputType.TextBox);
            // DataType ↔ InputType tutarlılığı (Self-Verification'ın kalıcılık tarafı).
            return dataType switch
            {
                AttributeDataType.Color => AttributeInputType.ColorPicker,
                AttributeDataType.Boolean => AttributeInputType.Toggle,
                AttributeDataType.Date => AttributeInputType.DatePicker,
                AttributeDataType.DateTime => AttributeInputType.DateTimePicker,
                AttributeDataType.Measurement => AttributeInputType.MeasurementBox,
                AttributeDataType.Integer or AttributeDataType.Decimal => AttributeInputType.NumberBox,
                AttributeDataType.SingleSelect =>
                    parsed is AttributeInputType.Dropdown or AttributeInputType.RadioGroup ? parsed : AttributeInputType.Dropdown,
                AttributeDataType.MultiSelect =>
                    parsed is AttributeInputType.MultiSelectDropdown or AttributeInputType.CheckboxList ? parsed : AttributeInputType.MultiSelectDropdown,
                AttributeDataType.MultilineText or AttributeDataType.RichText => AttributeInputType.TextArea,
                AttributeDataType.Url => AttributeInputType.UrlBox,
                _ => parsed
            };
        }

        private static TEnum ParseEnum<TEnum>(string? value, TEnum fallback) where TEnum : struct, Enum
            => string.IsNullOrWhiteSpace(value) ? fallback
               : Enum.TryParse<TEnum>(value.Trim(), ignoreCase: true, out var parsed) ? parsed : fallback;

        private static string ComputeHash(string input)
            => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(input ?? string.Empty)));

        private static string ComputeIdempotencyKey(AiJobType type, string categoryPath, string version)
            => Truncate($"{(byte)type}:{ComputeHash($"{TextNormalizer.Normalize(categoryPath)}|{version}")}", 256);

        private static string Truncate(string? s, int max)
            => string.IsNullOrEmpty(s) ? (s ?? string.Empty) : (s.Length <= max ? s : s.Substring(0, max));

        // ════════════════════════════════════════════════════════════════════
        //  PROMPT BUILDER + VERSIONING (Gemini için verimli, "eksikleri iste")
        // ════════════════════════════════════════════════════════════════════
        internal static class PromptBuilder
        {
            public const string V2_ENRICHED = "attr-schema-v2-enriched";
            public const string V3_FEW_SHOT = "attr-schema-v3-few-shot";
            public const string V4_SELF_VERIFY = "attr-schema-v4-self-verify";
            public const string CurrentDefaultVersion = V3_FEW_SHOT;

            /// <summary>Kategori karmaşıklığına göre prompt sürümü seç.</summary>
            public static string SelectVersion(string categoryPath)
            {
                int depth = string.IsNullOrEmpty(categoryPath) ? 1 : categoryPath.Count(ch => ch == '/' || ch == '>') + 1;
                if (depth >= 5) return V4_SELF_VERIFY; // derin/teknik kategori → öz-doğrulamalı
                if (depth >= 3) return V3_FEW_SHOT;
                return V2_ENRICHED;
            }

            /// <summary>Prompt + (varsa) Gemini structured-output responseSchema üretir. KB-First: bilinenleri hariç tut.</summary>
            public static (string prompt, object? responseSchema) Build(string version, string categoryPath, HashSet<string> knownCanonicalCodes)
            {
                string known = knownCanonicalCodes.Count == 0
                    ? "(yok)"
                    : string.Join(", ", knownCanonicalCodes.OrderBy(x => x).Take(200));

                var sb = new StringBuilder(2048);
                sb.AppendLine("[ROL] Dünya çapında e-ticaret taksonomi/ürün-veri mühendisisin. Görevin: verilen kategori yolu için");
                sb.AppendLine("ürünlerin sahip olabileceği TÜM teknik/fiziksel/fonksiyonel/ticari özellikleri (attribute) KATı JSON üretmek.");
                sb.AppendLine();
                sb.AppendLine($"[KATEGORİ YOLU] {categoryPath}");
                sb.AppendLine();
                sb.AppendLine("[KNOWLEDGE-BASE-FIRST — EN ÖNEMLİ KURAL]");
                sb.AppendLine("Aşağıdaki canonicalCode'lar bu kategoride ZATEN TANIMLI. Bunları TEKRAR ÜRETME.");
                sb.AppendLine("Yalnızca listede OLMAYAN, EKSİK attribute'ları döndür. Hiç eksik yoksa boş 'attributes': [] döndür.");
                sb.AppendLine($"ZATEN VAR: {known}");
                sb.AppendLine();
                sb.AppendLine("[KURALLAR]");
                sb.AppendLine("1. Kategoriye özgü en ince ayrıntıya kadar in (ör. akıllı telefon: işlemci, RAM, depolama, ekran teknolojisi, yenileme hızı, kamera, batarya mAh, şarj gücü, IP sertifikası, ağ, NFC).");
                sb.AppendLine("2. Her attribute için 10 dilde ad: tr,en,az,de,es,fr,hi,pt,ru,zh. Kaynak dil Türkçe; diğerleri doğru çeviri.");
                sb.AppendLine("3. canonicalCode: küçük harf, İngilizce, snake_case. Aynı kavramın farklı bağlamını AYRIŞTIR (giyim bedeni→size_clothing, ekran→screen_size).");
                sb.AppendLine("4. dataType ∈ [Text,MultilineText,Integer,Decimal,Boolean,Date,DateTime,SingleSelect,MultiSelect,Measurement,Color,Url].");
                sb.AppendLine("5. inputType ∈ [TextBox,TextArea,NumberBox,Dropdown,RadioGroup,CheckboxList,MultiSelectDropdown,Toggle,DatePicker,ColorPicker,MeasurementBox,UrlBox].");
                sb.AppendLine("6. unit: ölçü tipindeyse kanonik birim kodu (mm,gr,inch,mah,ghz,wh); değilse null.");
                sb.AppendLine("7. isRequired,isVariant,isFilterable,isSearchable,isComparable,isKey → boolean. isVariant: varyant üretebilen özellik (renk,beden,hafıza).");
                sb.AppendLine("8. synonyms: dil-ötesi eşanlamlı token'lar (dedup için kritik). ram→[ram,memory,bellek,system memory].");
                sb.AppendLine("9. SingleSelect/MultiSelect/Color için options ver: canonicalCode, 10 dil values, synonyms, renk ise colorHex(#RRGGBB), sayısal ise numericValue. Diğer tiplerde options: [].");
                sb.AppendLine("10. UYDURMA yok: kesin olmayan marka/model/ölçü üretme.");
                sb.AppendLine("11. YALNIZCA geçerli JSON döndür; markdown, açıklama, emoji YOK.");

                if (version == V3_FEW_SHOT || version == V4_SELF_VERIFY)
                {
                    sb.AppendLine();
                    sb.AppendLine("[ÖRNEK ÇIKTI — yapı referansı]");
                    sb.AppendLine(FewShotExample);
                }

                if (version == V4_SELF_VERIFY)
                {
                    sb.AppendLine();
                    sb.AppendLine("[ÖZ-DOĞRULAMA — döndürmeden önce KENDİN kontrol et]");
                    sb.AppendLine("• Her attribute'un 10 dili tam mı? • dataType↔inputType uyumlu mu? • Measurement'ta unit dolu mu?");
                    sb.AppendLine("• 'ZATEN VAR' listesindeki bir kavramı tekrar üretmedin mi? • Aynı attribute'u iki kez koymadın mı?");
                    sb.AppendLine("Sorun varsa DÜZELT, sonra SADECE nihai JSON'u döndür.");
                }

                sb.AppendLine();
                sb.AppendLine("[ÇIKTI ŞEMASI] { \"attributes\": [ { canonicalCode, dataType, inputType, unit, isRequired, isVariant, isFilterable, isSearchable, isComparable, isKey, names{tr..zh}, synonyms[], options[ { canonicalCode, colorHex, numericValue, values{tr..zh}, synonyms[] } ] } ] }");
                sb.AppendLine("SADECE GEÇERLİ JSON DÖNDÜR.");

                // Structured output şeması (Gemini) → JSON geçerliliğini ciddi artırır.
                object responseSchema = BuildResponseSchema();
                return (sb.ToString(), responseSchema);
            }

            private const string FewShotExample =
                "{\"attributes\":[{\"canonicalCode\":\"screen_size\",\"dataType\":\"Measurement\",\"inputType\":\"MeasurementBox\",\"unit\":\"inch\"," +
                "\"isRequired\":true,\"isVariant\":false,\"isFilterable\":true,\"isSearchable\":false,\"isComparable\":true,\"isKey\":true," +
                "\"names\":{\"tr\":\"Ekran Boyutu\",\"en\":\"Screen Size\",\"az\":\"Ekran Ölçüsü\",\"de\":\"Bildschirmgröße\",\"es\":\"Tamaño de Pantalla\",\"fr\":\"Taille d'écran\",\"hi\":\"स्क्रीन आकार\",\"pt\":\"Tamanho da Tela\",\"ru\":\"Размер экрана\",\"zh\":\"屏幕尺寸\"}," +
                "\"synonyms\":[\"ekran boyutu\",\"screen size\",\"display size\"],\"options\":[]}]}";

            private static object BuildResponseSchema()
            {
                var langProps = AllLanguages.ToDictionary(l => l.ToCultureCode(), _ => (object)new { type = "string" });
                var namesObj = new { type = "object", properties = langProps };

                var optionItem = new
                {
                    type = "object",
                    properties = new
                    {
                        canonicalCode = new { type = "string" },
                        colorHex = new { type = "string", nullable = true },
                        numericValue = new { type = "number", nullable = true },
                        values = namesObj,
                        synonyms = new { type = "array", items = new { type = "string" } }
                    },
                    required = new[] { "canonicalCode", "values" }
                };

                var attrItem = new
                {
                    type = "object",
                    properties = new
                    {
                        canonicalCode = new { type = "string" },
                        dataType = new { type = "string" },
                        inputType = new { type = "string" },
                        unit = new { type = "string", nullable = true },
                        isRequired = new { type = "boolean" },
                        isVariant = new { type = "boolean" },
                        isFilterable = new { type = "boolean" },
                        isSearchable = new { type = "boolean" },
                        isComparable = new { type = "boolean" },
                        isKey = new { type = "boolean" },
                        names = namesObj,
                        synonyms = new { type = "array", items = new { type = "string" } },
                        options = new { type = "array", items = optionItem }
                    },
                    required = new[] { "canonicalCode", "dataType", "names" }
                };

                return new
                {
                    type = "object",
                    properties = new { attributes = new { type = "array", items = attrItem } },
                    required = new[] { "attributes" }
                };
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  JSON VALIDATION + REPAIR
        // ════════════════════════════════════════════════════════════════════
        internal static class JsonValidator
        {
            private static readonly JsonSerializerOptions Tolerant = new()
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            };

            public static AiAttributeSchema? ParseWithRepair(string raw, ILogger logger, string categoryPath)
            {
                // 1) Doğrudan (toleranslı) dene.
                var direct = TryDeserialize(raw);
                if (direct != null) return direct;

                // 2) Markdown fence / önek-sonek prose temizle, en dıştaki {...}'ı çıkar.
                string cleaned = ExtractJsonObject(raw);
                if (!string.IsNullOrWhiteSpace(cleaned) && !ReferenceEquals(cleaned, raw))
                {
                    var repaired = TryDeserialize(cleaned);
                    if (repaired != null) return repaired;
                }

                logger.LogWarning("AIAttr JSON parse edilemedi → {Path}", categoryPath);
                return null;
            }

            private static AiAttributeSchema? TryDeserialize(string s)
            {
                try
                {
                    return JsonSerializer.Deserialize<AiAttributeSchema>(s, Tolerant);
                }
                catch (JsonException) { return null; }
            }

            /// <summary>Markdown fence ve çevresel metni sıyırıp dengeli en-dış { } bloğunu döndürür.</summary>
            private static string ExtractJsonObject(string raw)
            {
                if (string.IsNullOrWhiteSpace(raw)) return raw;
                string s = raw.Trim();

                // ```json ... ``` fence temizliği
                int fence = s.IndexOf("```", StringComparison.Ordinal);
                if (fence >= 0)
                {
                    int firstBrace = s.IndexOf('{', fence);
                    int lastFence = s.LastIndexOf("```", StringComparison.Ordinal);
                    if (firstBrace >= 0 && lastFence > firstBrace)
                        s = s.Substring(firstBrace, lastFence - firstBrace);
                }

                int start = s.IndexOf('{');
                int end = s.LastIndexOf('}');
                if (start < 0 || end <= start) return raw;

                // Dengeli parantez taraması (string/escape farkında).
                int depth = 0; bool inStr = false, esc = false;
                for (int i = start; i <= end; i++)
                {
                    char c = s[i];
                    if (inStr)
                    {
                        if (esc) esc = false;
                        else if (c == '\\') esc = true;
                        else if (c == '"') inStr = false;
                    }
                    else
                    {
                        if (c == '"') inStr = true;
                        else if (c == '{') depth++;
                        else if (c == '}') { depth--; if (depth == 0) return s.Substring(start, i - start + 1); }
                    }
                }
                return s.Substring(start, end - start + 1);
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  SELF-VERIFICATION (Pass 2) — şemayı temizle/normalize et
        // ════════════════════════════════════════════════════════════════════
        internal static class SchemaValidator
        {
            public static AiAttributeSchema Verify(AiAttributeSchema schema, string categoryPath, ILogger logger)
            {
                var result = new AiAttributeSchema { Attributes = new List<AiAttribute>() };
                if (schema.Attributes == null) return result;

                var seenCodes = new HashSet<string>(StringComparer.Ordinal);
                var seenTokens = new HashSet<string>(StringComparer.Ordinal);

                foreach (var a in schema.Attributes)
                {
                    if (a == null) continue;

                    // 1) TR ad zorunlu (temel anlamlılık).
                    string tr = a.Names != null && a.Names.TryGetValue("tr", out var v) ? (v ?? "") : "";
                    if (string.IsNullOrWhiteSpace(tr) && string.IsNullOrWhiteSpace(a.CanonicalCode)) continue;

                    // 2) Intra-batch duplicate collapse (aynı canonical / aynı token iki kez).
                    string canon = TextNormalizer.ToCanonicalCode(a.CanonicalCode);
                    if (!string.IsNullOrWhiteSpace(canon))
                    {
                        if (seenCodes.Contains(canon)) continue;
                        seenCodes.Add(canon);
                    }
                    var tokens = BuildAttributeTokens(a);
                    if (tokens.Any(t => seenTokens.Contains(t)))
                    {
                        // Zaten bu batch'te aynı kavram var → atla (yinelenme engeli).
                        continue;
                    }
                    foreach (var t in tokens) seenTokens.Add(t);

                    // 3) Translation completeness → eksik dilleri TR ile doldur.
                    a.Names ??= new Dictionary<string, string>();
                    if (!string.IsNullOrWhiteSpace(tr))
                        foreach (var lang in AllLanguages)
                        {
                            string code = lang.ToCultureCode();
                            if (!a.Names.TryGetValue(code, out var val) || string.IsNullOrWhiteSpace(val))
                                a.Names[code] = tr;
                        }

                    // 4) DataType ↔ options tutarlılığı: select/color options'suzsa → basit tipe düşür (optionsuz select üretme).
                    var dt = ParseEnum(a.DataType, AttributeDataType.Text);
                    bool needsOptions = dt is AttributeDataType.SingleSelect or AttributeDataType.MultiSelect;
                    if (needsOptions && (a.Options == null || a.Options.Count == 0))
                    {
                        logger.LogDebug("Self-verify: options'suz {Dt} → Text'e düşürüldü ({Code})", dt, canon);
                        a.DataType = AttributeDataType.Text.ToString();
                    }

                    // 5) Color options'suzsa serbest renk kabul; options varsa colorHex normalize.
                    if (a.Options != null)
                        foreach (var o in a.Options)
                            if (o != null && !string.IsNullOrWhiteSpace(o.ColorHex) && !o.ColorHex.StartsWith('#'))
                                o.ColorHex = "#" + o.ColorHex.Trim();

                    result.Attributes.Add(a);
                }

                return result;
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  AI JSON → DTO'lar
        // ════════════════════════════════════════════════════════════════════
        public sealed class AiAttributeSchema
        {
            [JsonPropertyName("attributes")] public List<AiAttribute>? Attributes { get; set; }
        }

        public sealed class AiAttribute
        {
            [JsonPropertyName("canonicalCode")] public string? CanonicalCode { get; set; }
            [JsonPropertyName("dataType")] public string? DataType { get; set; }
            [JsonPropertyName("inputType")] public string? InputType { get; set; }
            [JsonPropertyName("unit")] public string? Unit { get; set; }
            [JsonPropertyName("isRequired")] public bool? IsRequired { get; set; }
            [JsonPropertyName("isVariant")] public bool? IsVariant { get; set; }
            [JsonPropertyName("isFilterable")] public bool? IsFilterable { get; set; }
            [JsonPropertyName("isSearchable")] public bool? IsSearchable { get; set; }
            [JsonPropertyName("isComparable")] public bool? IsComparable { get; set; }
            [JsonPropertyName("isKey")] public bool? IsKey { get; set; }
            [JsonPropertyName("names")] public Dictionary<string, string>? Names { get; set; }
            [JsonPropertyName("synonyms")] public List<string>? Synonyms { get; set; }
            [JsonPropertyName("options")] public List<AiOption>? Options { get; set; }
        }

        public sealed class AiOption
        {
            [JsonPropertyName("canonicalCode")] public string? CanonicalCode { get; set; }
            [JsonPropertyName("colorHex")] public string? ColorHex { get; set; }
            [JsonPropertyName("numericValue")] public decimal? NumericValue { get; set; }
            [JsonPropertyName("values")] public Dictionary<string, string>? Values { get; set; }
            [JsonPropertyName("synonyms")] public List<string>? Synonyms { get; set; }
        }
    }
}