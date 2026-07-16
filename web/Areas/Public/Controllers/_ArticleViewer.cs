// ═══════════════════════════════════════════════════════════════════════
// _ArticleViewer — SEO-Optimized Article Controller
// ═══════════════════════════════════════════════════════════════════════
// _Viewer.cs (ProductProfile) ile aynı ViewData sözleşmesini kullanır:
//   ViewData["CanonicalUrl"]   → rel="canonical"
//   ViewData["Title"]          → <title> + og:title + twitter:title
//   ViewData["Description"]    → meta description + og:description
//   ViewData["Keywords"]       → meta keywords
//   ViewData["OgType"]         → og:type (article)
//   ViewData["OgImage"]        → og:image + twitter:image
//   ViewData["OgImageWidth"]   → og:image:width
//   ViewData["OgImageHeight"]  → og:image:height
//   ViewData["PreloadImage"]   → <link rel="preload" as="image">
//   ViewData["JsonLd"]         → <script type="application/ld+json">
//   ViewData["Robots"]         → meta robots
// ═══════════════════════════════════════════════════════════════════════

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using data;
using data.Articles;
using System.Text.Json;
using System.Text.Encodings.Web;
using data._Galleries;
using data._Users;

namespace web.Areas.Public.Controllers
{
    [Area("Public")]
    [Route("/Public/[controller]/[action]/{id?}")]
    [Route("/tr/Public/[controller]/[action]/{id?}")]
    [Route("/en/Public/[controller]/[action]/{id?}")]
    [Route("/az/Public/[controller]/[action]/{id?}")]
    [Route("/de/Public/[controller]/[action]/{id?}")]
    [Route("/es/Public/[controller]/[action]/{id?}")]
    [Route("/fr/Public/[controller]/[action]/{id?}")]
    [Route("/hi/Public/[controller]/[action]/{id?}")]
    [Route("/pt/Public/[controller]/[action]/{id?}")]
    [Route("/ru/Public/[controller]/[action]/{id?}")]
    [Route("/zh/Public/[controller]/[action]/{id?}")]
    public class _ArticleViewer : Controller
    {
        // ═══════════════════════════════════════════════════════════
        // 1) MAKALE LİSTESİ  →  /Public/_ArticleViewer/ArticleList?q=...
        //    Kart ızgarası + arama + sonsuz kaydırma ArticleListing.razor
        //    componentinin kendi içinde (IDbContextFactory ile) yönetilir.
        //    Bu action sadece SEO kabuğunu ve başlangıç arama terimini kurar.
        // ═══════════════════════════════════════════════════════════
        public IActionResult ArticleList(string? q = null)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var canonicalUrl = $"{baseUrl}/Public/_ArticleViewer/ArticleList";
            var hasQuery = !string.IsNullOrWhiteSpace(q);

            // Canonical her zaman temel liste URL'sine sabitlenir — arama query string'i
            // canonical'ı kirletip duplicate content sorunu yaratmasın diye.
            ViewData["CanonicalUrl"] = canonicalUrl;

            ViewData["Title"] = hasQuery ? $"\"{q}\" için Arama Sonuçları" : "Makaleler";
            ViewData["Description"] = hasQuery
                ? $"efavori.com'da \"{q}\" ile ilgili makaleler ve rehberler."
                : "efavori.com'da boya badana, tamirat tadilat, su tesisatı ve daha fazlası hakkında güncel makaleler ve rehberler.";
            ViewData["Keywords"] = "makale, rehber, blog, efavori";

            ViewData["OgType"] = "website";
            ViewData["OgImage"] = $"{baseUrl}/_files/main/logo/og-default.png";
            ViewData["OgImageWidth"] = "1200";
            ViewData["OgImageHeight"] = "630";

            // Arama sonucu sayfaları indexlenmesin — thin/duplicate content riski taşır.
            // Filtresiz ana liste sayfası her zaman indexlenir.
            ViewData["Robots"] = hasQuery
                ? "noindex, follow"
                : "index, follow, max-image-preview:large, max-snippet:-1, max-video-preview:-1";

            // JSON-LD: CollectionPage — makale koleksiyonunun kendisi için temel yapısal veri
            var collectionSchema = new Dictionary<string, object?>
            {
                ["@context"] = "https://schema.org",
                ["@type"] = "CollectionPage",
                ["name"] = hasQuery ? $"\"{q}\" için Arama Sonuçları" : "Makaleler",
                ["url"] = canonicalUrl
            };
            ViewData["JsonLd"] = $"<script type=\"application/ld+json\">\n{SerializeJsonLd(collectionSchema)}\n</script>";

            ViewBag.InitialQuery = q;

            return View();
        }

        // ═══════════════════════════════════════════════════════════
        // 2) MAKALE DETAYI  →  /Public/_ArticleViewer/ArticleProfile/{id}
        //    {id} → önce Slug ile aranır, bulunamazsa Guid Id ile yedeklenir
        //    (eski/slug'sız kayıtlar için geriye dönük uyumluluk).
        // ═══════════════════════════════════════════════════════════
        public IActionResult ArticleProfile(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return NotFound();

            using (_ApplicationConnectionDb db = new _ApplicationConnectionDb())
            {
                // ── 1. Makaleyi bul ──
                Article? article = db.Set<Article>().AsNoTracking()
                    .FirstOrDefault(a => a.Slug == id);

                if (article == null && Guid.TryParse(id, out var parsedId))
                {
                    article = db.Set<Article>().AsNoTracking()
                        .FirstOrDefault(a => a.Id == parsedId);
                }

                if (article == null || (article.IsDeleted != null && article.IsDeleted.IsDeletedStatu == true))
                {
                    return NotFound();
                }

                var articleId = article.Id;
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var slug = !string.IsNullOrWhiteSpace(article.Slug) ? article.Slug : articleId.ToString();

                // ── 2. Kapak görseli ──
                string? coverImageUrl = null;
                if (article.FeaturedImage.HasValue)
                {
                    coverImageUrl = db.Set<Media>().AsNoTracking()
                        .Where(m => m.Id == article.FeaturedImage.Value && m.IsDeletedStatu != true)
                        .Select(m => m.FileUrl_Ratio_1_2 ?? m.FileUrl)
                        .FirstOrDefault();
                }

                // ── 3. Yazar bilgisi (best-effort) ──
                string authorName = "efavori.com Editör";
                if (article.IsUser.HasValue)
                {
                    var authorUser = db.Set<Users>().AsNoTracking()
                        .Where(u => u.Id == article.IsUser.Value)
                        .Select(u => new { u.FirstName, u.LastName })
                        .FirstOrDefault();

                    if (authorUser != null)
                    {
                        var fullName = $"{authorUser.FirstName} {authorUser.LastName}".Trim();
                        if (!string.IsNullOrWhiteSpace(fullName)) authorName = fullName;
                    }
                }

                // ═══════════════════════════════════════════════════════
                // 4. SEO DEĞERLERİNİ HESAPLA
                // ═══════════════════════════════════════════════════════

                var canonicalUrl = !string.IsNullOrEmpty(article.Meta?.CanonicalUrl)
                    ? article.Meta!.CanonicalUrl
                    : $"{baseUrl}/Public/_ArticleViewer/ArticleProfile/{slug}";

                var pageTitle = !string.IsNullOrEmpty(article.Meta?.MetaTitle)
                    ? article.Meta!.MetaTitle
                    : article.Title ?? "Makale";

                var rawDescription = !string.IsNullOrEmpty(article.Meta?.MetaDescription)
                    ? article.Meta!.MetaDescription
                    : article.ShotDescription ?? $"{article.Title} — efavori.com";

                var pageDescription = rawDescription;
                if (pageDescription.Length > 160)
                {
                    var truncated = pageDescription.Substring(0, 157);
                    int lastSpace = truncated.LastIndexOf(' ');
                    pageDescription = (lastSpace > 0 ? truncated.Substring(0, lastSpace) : truncated) + "...";
                }

                var pageKeywords = article.Meta?.FocusKeywords ?? "";

                var ogImageUrl = !string.IsNullOrEmpty(coverImageUrl)
                    ? (coverImageUrl!.StartsWith("http") ? coverImageUrl : $"{baseUrl}{coverImageUrl}")
                    : $"{baseUrl}/_files/main/logo/og-default.png";

                var robotsContent = article.Meta?.RobotsIndex
                    ?? "index, follow, max-image-preview:large, max-snippet:-1, max-video-preview:-1";

                // ═══════════════════════════════════════════════════════
                // 5. JSON-LD: BreadcrumbList + BlogPosting (SSR)
                // ═══════════════════════════════════════════════════════

                var jsonLdScripts = new List<string>();

                var breadcrumbSchema = new
                {
                    @context = "https://schema.org",
                    @type = "BreadcrumbList",
                    itemListElement = new object[]
                    {
                        new { @type = "ListItem", position = 1, name = "Ana Sayfa", item = baseUrl + "/" },
                        new { @type = "ListItem", position = 2, name = "Makaleler", item = $"{baseUrl}/Public/_ArticleViewer/ArticleList" },
                        new { @type = "ListItem", position = 3, name = article.Title ?? "Makale", item = canonicalUrl }
                    }
                };
                jsonLdScripts.Add(SerializeJsonLd(breadcrumbSchema));

                var articleSchema = new Dictionary<string, object?>
                {
                    ["@context"] = "https://schema.org",
                    ["@type"] = "BlogPosting",
                    ["mainEntityOfPage"] = new Dictionary<string, object?>
                    {
                        ["@type"] = "WebPage",
                        ["@id"] = canonicalUrl
                    },
                    ["headline"] = article.Title,
                    ["description"] = pageDescription,
                    ["image"] = ogImageUrl,
                    ["datePublished"] = article.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    ["dateModified"] = (article.UpdatedAt ?? article.CreatedAt).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    ["author"] = new Dictionary<string, object?>
                    {
                        ["@type"] = "Organization",
                        ["name"] = authorName
                    },
                    ["publisher"] = new Dictionary<string, object?>
                    {
                        ["@type"] = "Organization",
                        ["name"] = "efavori.com",
                        ["logo"] = new Dictionary<string, object?>
                        {
                            ["@type"] = "ImageObject",
                            ["url"] = $"{baseUrl}/_files/main/logo/logo.png"
                        }
                    }
                };
                jsonLdScripts.Add(SerializeJsonLd(articleSchema));

                var combinedJsonLd = string.Join("\n", jsonLdScripts.Select(j =>
                    $"<script type=\"application/ld+json\">\n{j}\n</script>"));

                // ═══════════════════════════════════════════════════════
                // 6. VIEWDATA'YA AKTAR
                // ═══════════════════════════════════════════════════════

                ViewData["CanonicalUrl"] = canonicalUrl;
                ViewData["Title"] = pageTitle;
                ViewData["Description"] = pageDescription;
                ViewData["Keywords"] = pageKeywords;

                ViewData["OgType"] = article.Meta?.OgType ?? "article";
                ViewData["OgImage"] = ogImageUrl;
                ViewData["OgImageWidth"] = "1200";
                ViewData["OgImageHeight"] = "630";

                ViewData["PreloadImage"] = coverImageUrl;

                ViewData["JsonLd"] = combinedJsonLd;

                ViewData["Robots"] = robotsContent;

                // Component'e sadece Id geçirilir — component kendi verisini bağımsız yükler
                // (controller = SEO kabuğu, component = görüntüleme + etkileşim mantığı ayrımı).
                ViewBag.ArticleId = articleId;

                return View();
            }
        }

        // ═══════════════════════════════════════════════════════════════════════
        // YARDIMCI: JSON-LD Serializer (_Viewer.cs ile birebir aynı)
        // ═══════════════════════════════════════════════════════════════════════
        private static string SerializeJsonLd(object obj)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

            var json = JsonSerializer.Serialize(obj, options);

            json = json
                .Replace("\"type\":", "\"@type\":")
                .Replace("\"context\":", "\"@context\":");

            return json;
        }
    }
}
