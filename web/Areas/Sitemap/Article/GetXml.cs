// ═══════════════════════════════════════════════════════════════════════════════
// SitemapController — Ürün Sitemap + Google Merchant Feed
// ═══════════════════════════════════════════════════════════════════════════════
//
//  MASTER INDEX          → /sitemap.xml
//
//  ÜRÜN SITEMAPLERI
//    Index               → /sitemap-products-index.xml
//    Sayfalanmış         → /sitemap-products-{id}.xml
//                           ✓ <image:image>     → Google Görseller
//                           ✓ <xhtml:link>      → hreflang çoklu dil
//                           ✓ <lastmod>         → güncelleme tarihi
//
//  GOOGLE MERCHANT FEED (Alışveriş Sekmesi)
//    Feed Index          → /feed/products-index.xml
//    Sayfalanmış Feed    → /feed/products-{id}.xml
//                           ✓ g:id, g:title, g:description, g:link
//                           ✓ g:image_link, g:additional_image_link
//                           ✓ g:price, g:sale_price, g:availability
//                           ✓ g:brand, g:gtin, g:mpn, g:condition
//                           ✓ g:product_type, g:item_group_id
//                           ✓ g:shipping
//
//  ROBOTS.TXT            → /robots.txt
//
// ═══════════════════════════════════════════════════════════════════════════════

using data;
using data._Product;
using data.Articles;
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace web.Areas.Sitemap.Article
{
    [Area("Sitemap")]
    [Route("/Sitemap/[controller]/[action]")]
    public class GetXml : Controller
    {
        private readonly _ApplicationConnectionDb _context;

        private const int MaxUrlsPerSitemap = 1_000;
        private const int MaxFeedItemsPerPage = 1_000;

        // _Viewer route'larıyla birebir eşleşen diller
        private static readonly string[] SupportedLangs =
            { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };

        public GetXml(_ApplicationConnectionDb context)
        {
            _context = context;
        }

        // ═══════════════════════════════════════════════════════════
        //  3b. MAKALE SITEMAP INDEX  →  /sitemap-articles-index.xml
        // ═══════════════════════════════════════════════════════════

        [Route("/sitemap-articles-index.xml")]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> ArticlesSitemapIndex()
        {
            var total = await _context.Set<data.Articles.Article>().AsNoTracking()
                .Where(a => a.IsDeleted == null || a.IsDeleted.IsDeletedStatu == false)
                .CountAsync();

            if (total == 0) return NotFound();

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var pages = (int)Math.Ceiling((double)total / MaxUrlsPerSitemap);

            var entries = Enumerable.Range(1, pages)
                .Select(i => $"{baseUrl}/sitemap-articles-{i}.xml")
                .ToList();

            return await WriteSitemapIndex(entries);
        }


        // ═══════════════════════════════════════════════════════════
        //  3c. MAKALE SITEMAP (sayfalanmış)  →  /sitemap-articles-{id}.xml
        //      ✓ image:image   (Google Görseller — kapak görseli)
        //      ✓ xhtml:link    (hreflang çoklu dil)
        //      ✓ lastmod       (güncelleme tarihi)
        // ═══════════════════════════════════════════════════════════

        [Route("/sitemap-articles-{id:int}.xml")]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> ArticlesSitemap(int id = 1)
        {
            if (id < 1) id = 1;

            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            // ── 3c-a. Makale listesi ──────────────────────────────
            // NOT: Sitemap üretimi arka planda periyodik çalıştığı için burada Skip/Take
            // (offset pagination) kullanılması sorun yaratmaz — kullanıcıya anlık servis eden
            // ArticleListing.razor bileşeni ise keyset pagination kullanır (bkz. o dosya).
            var pagedArticles = _context.Set<data.Articles.Article>().AsNoTracking()
                .Where(a => a.IsDeleted == null || a.IsDeleted.IsDeletedStatu == false)
                .OrderBy(a => a.Id)
                .Skip((id - 1) * MaxUrlsPerSitemap)
                .Take(MaxUrlsPerSitemap);

            var articlesRaw = await pagedArticles
                .Select(a => new
                {
                    a.Id,
                    a.Slug,
                    a.Title,
                    a.CreatedAt,
                    a.UpdatedAt,
                    a.FeaturedImage
                })
                .ToListAsync();

            if (!articlesRaw.Any()) return NotFound();

            // ── 3c-b. Kapak görselleri toplu çek ───────────────────
            var mediaIds = articlesRaw
                .Where(a => a.FeaturedImage.HasValue)
                .Select(a => a.FeaturedImage!.Value)
                .Distinct().ToList();

            var mediaMap = mediaIds.Any()
                ? await _context.Set<Media>().AsNoTracking()
                    .Where(m => mediaIds.Contains(m.Id) && m.IsDeletedStatu != true)
                    .ToDictionaryAsync(m => m.Id, m => m.FileUrl ?? m.FileUrl_Ratio_1_2 ?? "")
                : new Dictionary<Guid, string>();

            // ── 3c-c. XML oluştur ──────────────────────────────────
            Response.ContentType = "application/xml; charset=utf-8";

            await using var writer = XmlWriter.Create(Response.Body, XmlSettings());
            await writer.WriteStartDocumentAsync();

            await writer.WriteStartElementAsync(null, "urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");
            await writer.WriteAttributeStringAsync("xmlns", "image", null, "http://www.google.com/schemas/sitemap-image/1.1");
            await writer.WriteAttributeStringAsync("xmlns", "xhtml", null, "http://www.w3.org/1999/xhtml");

            foreach (var a in articlesRaw)
            {
                var slug = !string.IsNullOrWhiteSpace(a.Slug) ? a.Slug : a.Id.ToString();
                var articlePath = $"/Public/_ArticleViewer/ArticleProfile/{slug}";
                var canonicalUrl = $"{baseUrl}{articlePath}";
                var lastmod = (a.UpdatedAt ?? a.CreatedAt).ToString("yyyy-MM-dd");
                // Or if you specifically use MinValue:
                // var lastmod = (a.UpdatedAt != DateTime.MinValue ? a.UpdatedAt : a.CreatedAt).ToString("yyyy-MM-dd");
                await writer.WriteStartElementAsync(null, "url", null);

                // <loc>
                await writer.WriteElementStringAsync(null, "loc", null, canonicalUrl);

                // <lastmod>
                await writer.WriteElementStringAsync(null, "lastmod", null, lastmod);

                // ── hreflang ─────────────────────────────────────
                await WriteHreflangLink(writer, "x-default", canonicalUrl);
                foreach (var lang in SupportedLangs)
                    await WriteHreflangLink(writer, lang, $"{baseUrl}/{lang}{articlePath}");

                // ── image:image ──────────────────────────────────
                string? coverUrl = a.FeaturedImage.HasValue &&
                    mediaMap.TryGetValue(a.FeaturedImage.Value, out var cUrl)
                    ? cUrl : null;

                if (!string.IsNullOrEmpty(coverUrl))
                    await WriteImageTag(writer, baseUrl, coverUrl, a.Title);

                await writer.WriteEndElementAsync(); // </url>
            }

            await writer.WriteEndElementAsync(); // </urlset>
            await writer.WriteEndDocumentAsync();
            return new EmptyResult();
        }





        // ═══════════════════════════════════════════════════════════
        //  YARDIMCI METODLAR
        // ═══════════════════════════════════════════════════════════

        private async Task<IActionResult> WriteSitemapIndex(List<string> entries)
        {
            Response.ContentType = "application/xml; charset=utf-8";
            await using var writer = XmlWriter.Create(Response.Body, XmlSettings());
            await writer.WriteStartDocumentAsync();
            await writer.WriteStartElementAsync(null, "sitemapindex", "http://www.sitemaps.org/schemas/sitemap/0.9");

            foreach (var loc in entries)
            {
                await writer.WriteStartElementAsync(null, "sitemap", null);
                await writer.WriteElementStringAsync(null, "loc", null, loc);
                await writer.WriteEndElementAsync();
            }

            await writer.WriteEndElementAsync();
            await writer.WriteEndDocumentAsync();
            return new EmptyResult();
        }

        private static async Task WriteHreflangLink(XmlWriter writer, string lang, string href)
        {
            await writer.WriteStartElementAsync("xhtml", "link", "http://www.w3.org/1999/xhtml");
            await writer.WriteAttributeStringAsync(null, "rel", null, "alternate");
            await writer.WriteAttributeStringAsync(null, "hreflang", null, lang);
            await writer.WriteAttributeStringAsync(null, "href", null, href);
            await writer.WriteEndElementAsync();
        }

        private static async Task WriteImageTag(XmlWriter writer, string baseUrl, string imgUrl, string? title)
        {
            var fullUrl = imgUrl.StartsWith("http") ? imgUrl : $"{baseUrl}{imgUrl}";
            const string ns = "http://www.google.com/schemas/sitemap-image/1.1";

            await writer.WriteStartElementAsync("image", "image", ns);
            await writer.WriteElementStringAsync("image", "loc", ns, fullUrl);

            if (!string.IsNullOrEmpty(title))
                await writer.WriteElementStringAsync("image", "title", ns, title);

            await writer.WriteEndElementAsync();
        }

        private static string SanitizeHtml(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";

            var sb = new StringBuilder(input.Length);
            bool inTag = false;

            foreach (char c in input)
            {
                if (c == '<') { inTag = true; continue; }
                if (c == '>') { inTag = false; sb.Append(' '); continue; }
                if (!inTag) sb.Append(c);
            }

            var result = sb.ToString();
            while (result.Contains("  "))
                result = result.Replace("  ", " ");

            return result.Trim();
        }

        private static string NormalizeCurrency(string? raw)
        {
            return (raw?.ToUpperInvariant()) switch
            {
                "USD" => "USD",
                "EUR" => "EUR",
                "AZN" => "AZN",
                "GBP" => "GBP",
                _ => "TRY"
            };
        }

        private static XmlWriterSettings XmlSettings() => new()
        {
            Async = true,
            Encoding = Encoding.UTF8,
            Indent = true
        };
    }
}