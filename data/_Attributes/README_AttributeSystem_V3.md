# efavori — Enterprise Marketplace Attribute System V3

Merkezî, AI-yönetişimli **ürün özellik kataloğu** katmanı. Mevcut `data._Product`
katmanının tamamlayıcısıdır; onunla çakışmaz, ona ID ile bağlanır.

## Kapsam

Bu paket **tanımsal katalog + yönetişim** katmanıdır (attribute sözlüğü, option
sözlüğü, template, birim, bağımlılık, doğrulama, AI kuyruğu/onayı/geçmişi,
import/export eşleştirme). **Ürünün gerçek attribute değerleri** senin mevcut
`data._Product.ProductAttributeValues` tablonda kalır; sadece artık
`AttributeDefinitionId` / `AttributeOptionId` alanlarıyla bu kataloğa bağlanır.

## Mevcut yapıyla ilişki (entegrasyon dikişi)

| Bu V3 katmanı | Mevcut `data._Product` |
|---|---|
| `AttributeDefinition` (kanonik sözlük, tek kayıt) | `ProductAttributeValues.AttributeDefinitionId` → bağlanır |
| `AttributeOption` (kanonik değer sözlüğü) | `ProductAttributeValues.AttributeOptionId` → bağlanır |
| `AttributeTemplate` (kategori→attribute-kümesi) | `data._Product.AttributeTemplates` **ayrı** (ürün-özellik şablonu) — karıştırma |
| `AttributeMapping` / `AttributeOptionMapping` | Mevcut `Marketplaces` mapping'inin merkezî karşılığı |
| `CategoryAttribute` | `CategoriesTr` (int PK) üzerine attribute atama |

## Tasarım kararları (özet)

1. **Sınıf adı `AttributeDefinition`** — `Attribute` adı `System.Attribute` ile çakışırdı.
2. **GUID PK** — mevcut konvansiyonla uyum (`Guid.NewGuid()` varsayılan).
3. **`CategoryId` = int** — `CategoriesTr.Id` int olduğu için.
4. **Navigation property yok** — ilişkiler yalnızca FK üzerinden (`HasOne(typeof(X))`).
5. **Dedup çapası = `CanonicalCode` (global tekil) + `AttributeSynonym.NormalizedToken` (global tekil)** — "RAM=Memory=Bellek" aynı attribute; AI ikinci kez üretmez.
6. **Option dedup = attribute kapsamında** (`AttributeDefinitionId + NormalizedToken`) — "Gray=Grey=Gri" aynı option.
7. **Alias ≠ Synonym** — Alias insana-dönük görünen etiket; Synonym makine eşleştirme tokenı.
8. **Manuel koruma** — `Ai.IsManuallyEdited` / çeviri satırlarında `IsManuallyEdited`. AI & Import bu kayıtları **asla** ezmez (`WHERE IsManuallyEdited = 0`).
9. **AI kuyruğunda lease + idempotency** — çok örnekli BackgroundService'te çift işleme yok.
10. **Immutable template evrimi** — `Version` + `SupersededByTemplateId`.
11. **Kategori bazlı varyant override** — `CategoryAttribute.IsVariant` (nullable = attribute varsayılanı).
12. **Ölçü normalizasyonu** — `Unit.ConversionFactorToBase` + `ConversionOffset` ile baz birim.
13. **Soft-delete uyumlu filtered unique index** — soft-delete sonrası aynı kod yeniden oluşturulabilir.

## Entegrasyon adımları

1. `data._Attribute` klasörüne 11 entity dosyası + `_Attribute.ModelConfiguration.cs` eklenir.
2. `_Attribute.Common.cs` başındaki `using data;` satırı, senin **IsDeleted** owned tipinin namespace'i ile doğrulanır.
3. DbContext'te DbSet'ler eklenir (aşağıda) ve `OnModelCreating` içine tek satır:
   ```csharp
   _AttributeModelConfiguration.Apply(modelBuilder);
   ```
4. `_Attribute.ModelConfiguration.cs` içindeki `SoftDeleteFilter` sabiti, senin gerçek soft-delete kolon adına göre güncellenir (varsayılan `[IsDeleted] = 0`).
5. `Add-Migration AttributeSystemV3` → `Update-Database`.
6. Soft-delete **global query filter**'ını kendi konvansiyonunla uygula (bu paket eklemez; owned tipinin iç alan adını sen bilirsin).

### Eklenecek DbSet'ler

```csharp
public DbSet<AttributeGroup> AttributeGroups => Set<AttributeGroup>();
public DbSet<AttributeGroupTranslation> AttributeGroupTranslations => Set<AttributeGroupTranslation>();
public DbSet<AttributeDefinition> AttributeDefinitions => Set<AttributeDefinition>();
public DbSet<AttributeTranslation> AttributeTranslations => Set<AttributeTranslation>();
public DbSet<AttributeAlias> AttributeAliases => Set<AttributeAlias>();
public DbSet<AttributeSynonym> AttributeSynonyms => Set<AttributeSynonym>();
public DbSet<AttributeOption> AttributeOptions => Set<AttributeOption>();
public DbSet<AttributeOptionTranslation> AttributeOptionTranslations => Set<AttributeOptionTranslation>();
public DbSet<AttributeOptionAlias> AttributeOptionAliases => Set<AttributeOptionAlias>();
public DbSet<AttributeOptionSynonym> AttributeOptionSynonyms => Set<AttributeOptionSynonym>();
public DbSet<UnitGroup> UnitGroups => Set<UnitGroup>();
public DbSet<UnitGroupTranslation> UnitGroupTranslations => Set<UnitGroupTranslation>();
public DbSet<Unit> Units => Set<Unit>();
public DbSet<UnitTranslation> UnitTranslations => Set<UnitTranslation>();
public DbSet<AttributeTemplate> AttributeTemplates_V3 => Set<AttributeTemplate>(); // adı mevcut ile çakışmasın diye _V3
public DbSet<AttributeTemplateTranslation> AttributeTemplateTranslations => Set<AttributeTemplateTranslation>();
public DbSet<TemplateAttribute> TemplateAttributes => Set<TemplateAttribute>();
public DbSet<TemplateCategory> TemplateCategories => Set<TemplateCategory>();
public DbSet<CategoryAttribute> CategoryAttributes => Set<CategoryAttribute>();
public DbSet<AttributeDependency> AttributeDependencies => Set<AttributeDependency>();
public DbSet<NormalizationRule> NormalizationRules => Set<NormalizationRule>();
public DbSet<AiGenerationJob> AiGenerationJobs => Set<AiGenerationJob>();
public DbSet<AiGenerationHistory> AiGenerationHistories => Set<AiGenerationHistory>();
public DbSet<IntegrationPlatform> IntegrationPlatforms => Set<IntegrationPlatform>();
public DbSet<AttributeMapping> AttributeMappings => Set<AttributeMapping>();
public DbSet<AttributeOptionMapping> AttributeOptionMappings => Set<AttributeOptionMapping>();
```

## Silme (delete) davranışları

| İlişki | Davranış | Gerekçe |
|---|---|---|
| *Translation/Alias/Synonym* → ana entity | Cascade | Çocuk kayıtlar |
| AttributeOption → AttributeDefinition | Cascade | Çocuk |
| AttributeOption → ParentOption | NoAction | Döngü/çoklu-yol engeli |
| AttributeDefinition → AttributeGroup | SetNull | Grup opsiyonel |
| AttributeDefinition → UnitGroup/BaseUnit | NoAction | Referans veri |
| Unit → UnitGroup | Cascade | Çocuk |
| UnitGroup → BaseUnit | NoAction | Dairesel FK |
| CategoryAttribute/TemplateAttribute → AttributeDefinition | Restrict | Kullanımdaki attribute silinemez |
| *Mapping* → Platform | NoAction | Çoklu cascade-yolu engeli |
| *Mapping* → Attribute/Option | Cascade | Çocuk |
| AttributeDependency → Source/Target/Option | NoAction | Aynı tabloya çoklu FK |

## AI akışı (iki BackgroundService)

- **SERVICE 1 — Category Analyzer**: `AiGenerationJob(JobType=CategoryAttributeAnalysis)` çeker → kategori yolunu analiz eder → önce uygun `AttributeTemplate` arar (yoksa üretir), sonra `AttributeDefinition` + `CategoryAttribute` üretir. Üretmeden önce `TextNormalizer.Normalize` ile token üretip `AttributeSynonym.NormalizedToken` içinde arar; eşleşme varsa yeni attribute açmaz.
- **SERVICE 2 — Option Generator**: `AiGenerationJob(JobType=OptionGeneration)` çeker → attribute için `AttributeOption` üretir; aynı dedup mantığı option synonym'leri için uygulanır.
- Her üretim `AiGenerationHistory`'ye iz bırakır; onay bekleyenler `Ai.ApprovalStatus = PendingApproval`.

## Performans notları

- Yüksek hacimli tablolar (option ve *_Translation): `Guid.NewGuid()` kümelenmiş PK parçalanmaya yol açar. Bu tablolar için **sequential GUID** (`NEWSEQUENTIALID()` benzeri üretici) veya ayrı kümelenmiş anahtar değerlendir.
- Tüm `NormalizedName`/`NormalizedToken` alanları indekslidir; dedup taramaları eşitlik (equality) üzerinden çalışır.
- Çeviri tabloları `(ParentId, Language)` bileşik tekil indeksle nokta okumaya optimize.
- AI kuyruğu `(Status, Priority, NextRetryAtUtc)` indeksiyle taranır.
