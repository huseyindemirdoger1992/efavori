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
using data._Galleries;
using data._Product;
using data._Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace web.Areas.Sitemap.Products
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
            { "tr", "en"/*, "az", "de", "es", "fr", "hi", "pt", "ru", "zh"*/ };

        public GetXml(_ApplicationConnectionDb context)
        {
            _context = context;
        }


        // ═══════════════════════════════════════════════════════════
        //  1. MASTER INDEX  →  /sitemap.xml
        // ═══════════════════════════════════════════════════════════

        [Route("/sitemap.xml")]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> MasterIndex()
        {
            var total = await _context.Products.AsNoTracking()
                .Where(p => p.IsActive == true && p.IsApprovedByAdmin == true)
                .CountAsync();

            if (total == 0) return NotFound();

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var pages = (int)Math.Ceiling((double)total / MaxUrlsPerSitemap);

            var entries = Enumerable.Range(1, pages)
                .Select(i => $"{baseUrl}/sitemap-products-{i}.xml")
                .ToList();

            return await WriteSitemapIndex(entries);
        }


        // ═══════════════════════════════════════════════════════════
        //  2. ÜRÜN SITEMAP INDEX  →  /sitemap-products-index.xml
        // ═══════════════════════════════════════════════════════════

        [Route("/sitemap-products-index.xml")]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> ProductsSitemapIndex()
        {
            var total = await _context.Products.AsNoTracking()
                .Where(p => p.IsActive == true && p.IsApprovedByAdmin == true)
                .CountAsync();

            if (total == 0) return NotFound();

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var pages = (int)Math.Ceiling((double)total / MaxUrlsPerSitemap);

            var entries = Enumerable.Range(1, pages)
                .Select(i => $"{baseUrl}/sitemap-products-{i}.xml")
                .ToList();

            return await WriteSitemapIndex(entries);
        }


        // ═══════════════════════════════════════════════════════════
        //  3. ÜRÜN SITEMAP (sayfalanmış)  →  /sitemap-products-{id}.xml
        //     ✓ image:image   (Google Görseller)
        //     ✓ xhtml:link    (hreflang çoklu dil)
        //     ✓ lastmod       (güncelleme tarihi)
        // ═══════════════════════════════════════════════════════════

        [Route("/sitemap-products-{id:int}.xml")]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> ProductsSitemap(int id = 1)
        {
            if (id < 1) id = 1;

            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            // ── 3a. Ürün listesi ─────────────────────────────────
            var pagedProducts = _context.Products.AsNoTracking()
                .Where(p => p.IsActive == true && p.IsApprovedByAdmin == true)
                .OrderBy(p => p.Id)
                .Skip((id - 1) * MaxUrlsPerSitemap)
                .Take(MaxUrlsPerSitemap);

            var productsRaw = await (
                from p in pagedProducts
                join s in _context.ProductSeo.AsNoTracking() on p.Id equals s.ProductId into seoJoin
                from seo in seoJoin.DefaultIfEmpty()
                select new
                {
                    p.Id,
                    p.Name,
                    p.CreatedAt,
                    p.UpdatedAt,
                    p.CoverMediaId,
                    Slug = seo != null ? seo.Slug : null
                }
            ).ToListAsync();

            if (!productsRaw.Any()) return NotFound();

            // ── 3b. Kapak görselleri toplu çek ───────────────────
            var coverMediaIds = productsRaw
                .Where(p => p.CoverMediaId.HasValue)
                .Select(p => p.CoverMediaId!.Value)
                .Distinct().ToList();

            var coverMediaMap = coverMediaIds.Any()
                ? await _context.Set<Media>().AsNoTracking()
                    .Where(m => coverMediaIds.Contains(m.Id) && m.IsDeletedStatu != true)
                    .ToDictionaryAsync(m => m.Id, m => m.FileUrl ?? m.FileUrl_Ratio_1_2 ?? "")
                : new Dictionary<Guid, string>();

            // ── 3c. Galeri görselleri toplu çek ──────────────────
            var productIds = productsRaw.Select(p => p.Id).ToList();

            var galleryItems = await _context.Set<MediaItems>().AsNoTracking()
                .Where(ig => ig.ItemId.HasValue &&
                             productIds.Contains(ig.ItemId.Value) &&
                             ig.IsDelete != true &&
                             ig.MediaId.HasValue &&
                             (ig.ItemType == "Product" || ig.ItemType == "ProductGallery"))
                .OrderBy(ig => ig.ItemAddDate)
                .Select(ig => new { ItemId = ig.ItemId!.Value, MediaId = ig.MediaId!.Value })
                .ToListAsync();

            var galleryMediaIds = galleryItems.Select(g => g.MediaId).Distinct().ToList();

            var galleryMediaMap = galleryMediaIds.Any()
                ? await _context.Set<Media>().AsNoTracking()
                    .Where(m => galleryMediaIds.Contains(m.Id) && m.IsDeletedStatu != true)
                    .ToDictionaryAsync(m => m.Id, m => m.FileUrl ?? m.FileUrl_Ratio_1_2 ?? "")
                : new Dictionary<Guid, string>();

            var productGalleryMap = galleryItems
                .Where(g => galleryMediaMap.ContainsKey(g.MediaId))
                .GroupBy(g => g.ItemId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => galleryMediaMap[x.MediaId])
                          .Where(u => !string.IsNullOrEmpty(u))
                          .Take(5)
                          .ToList()
                );

            // ── 3d. XML oluştur ──────────────────────────────────
            Response.ContentType = "application/xml; charset=utf-8";

            await using var writer = XmlWriter.Create(Response.Body, XmlSettings());
            await writer.WriteStartDocumentAsync();

            await writer.WriteStartElementAsync(null, "urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");
            await writer.WriteAttributeStringAsync("xmlns", "image", null, "http://www.google.com/schemas/sitemap-image/1.1");
            await writer.WriteAttributeStringAsync("xmlns", "xhtml", null, "http://www.w3.org/1999/xhtml");

            foreach (var p in productsRaw)
            {
                var slug = !string.IsNullOrWhiteSpace(p.Slug) ? p.Slug : p.Id.ToString();
                var productPath = $"/Public/_Viewer/ProductProfile/{slug}";
                var canonicalUrl = $"{baseUrl}{productPath}";
                var lastmod = (p.UpdatedAt ?? p.CreatedAt)?.ToString("yyyy-MM-dd");

                await writer.WriteStartElementAsync(null, "url", null);

                // <loc>
                await writer.WriteElementStringAsync(null, "loc", null, canonicalUrl);

                // <lastmod>
                if (!string.IsNullOrEmpty(lastmod))
                    await writer.WriteElementStringAsync(null, "lastmod", null, lastmod);

                // ── hreflang ─────────────────────────────────────
                await WriteHreflangLink(writer, "x-default", canonicalUrl);
                foreach (var lang in SupportedLangs)
                    await WriteHreflangLink(writer, lang, $"{baseUrl}/{lang}{productPath}");

                // ── image:image ──────────────────────────────────
                string? coverUrl = p.CoverMediaId.HasValue &&
                    coverMediaMap.TryGetValue(p.CoverMediaId.Value, out var cUrl)
                    ? cUrl : null;

                if (!string.IsNullOrEmpty(coverUrl))
                    await WriteImageTag(writer, baseUrl, coverUrl, p.Name);

                if (productGalleryMap.TryGetValue(p.Id, out var gallery))
                {
                    foreach (var imgUrl in gallery)
                    {
                        if (imgUrl != coverUrl)
                            await WriteImageTag(writer, baseUrl, imgUrl, p.Name);
                    }
                }

                await writer.WriteEndElementAsync(); // </url>
            }

            await writer.WriteEndElementAsync(); // </urlset>
            await writer.WriteEndDocumentAsync();
            return new EmptyResult();
        }


        // ═══════════════════════════════════════════════════════════
        //  4. GOOGLE MERCHANT FEED INDEX  →  /feed/products-index.xml
        // ═══════════════════════════════════════════════════════════

        [Route("/feed/products-index.xml")]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> MerchantFeedIndex()
        {
            var total = await _context.Products.AsNoTracking()
                .Where(p => p.IsActive == true && p.IsApprovedByAdmin == true)
                .CountAsync();

            if (total == 0) return NotFound();

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var pages = (int)Math.Ceiling((double)total / MaxFeedItemsPerPage);

            var entries = Enumerable.Range(1, pages)
                .Select(i => $"{baseUrl}/feed/products-{i}.xml")
                .ToList();

            return await WriteSitemapIndex(entries);
        }


        // ═══════════════════════════════════════════════════════════
        //  5. GOOGLE MERCHANT FEED (sayfalanmış)  →  /feed/products-{id}.xml
        // ═══════════════════════════════════════════════════════════

        [Route("/feed/products-{id:int}.xml")]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> MerchantFeed(int id = 1)
        {
            if (id < 1) id = 1;

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            const string gNs = "http://base.google.com/ns/1.0";

            // ── Ürünleri sayfalı çek ─────────────────────────────
            var pagedProducts = _context.Products.AsNoTracking()
                .Where(p => p.IsActive == true && p.IsApprovedByAdmin == true)
                .OrderBy(p => p.Id)
                .Skip((id - 1) * MaxFeedItemsPerPage)
                .Take(MaxFeedItemsPerPage);

            var products = await (
                from p in pagedProducts
                join s in _context.ProductSeo.AsNoTracking() on p.Id equals s.ProductId into seoJoin
                from seo in seoJoin.DefaultIfEmpty()
                select new
                {
                    p.Id,
                    p.Name,
                    p.ShortDescription,
                    p.FullDescription,
                    p.CoverMediaId,
                    p.BrandId,
                    p.StoreId,
                    Slug = seo != null ? seo.Slug : null
                }
            ).ToListAsync();

            if (!products.Any()) return NotFound();

            var productIds = products.Select(p => p.Id).ToList();

            // ── Kapak görselleri ─────────────────────────────────
            var coverMediaIds = products
                .Where(p => p.CoverMediaId.HasValue)
                .Select(p => p.CoverMediaId!.Value)
                .Distinct().ToList();

            var coverMap = coverMediaIds.Any()
                ? await _context.Set<Media>().AsNoTracking()
                    .Where(m => coverMediaIds.Contains(m.Id) && m.IsDeletedStatu != true)
                    .ToDictionaryAsync(m => m.Id, m => m.FileUrl ?? m.FileUrl_Ratio_1_2 ?? "")
                : new Dictionary<Guid, string>();

            // ── Galeri görselleri ─────────────────────────────────
            var galleryItems = await _context.Set<MediaItems>().AsNoTracking()
                .Where(ig => ig.ItemId.HasValue &&
                             productIds.Contains(ig.ItemId.Value) &&
                             ig.IsDelete != true &&
                             ig.MediaId.HasValue &&
                             (ig.ItemType == "Product" || ig.ItemType == "ProductGallery"))
                .OrderBy(ig => ig.ItemAddDate)
                .Select(ig => new { ItemId = ig.ItemId!.Value, MediaId = ig.MediaId!.Value })
                .ToListAsync();

            var galleryMediaIds = galleryItems.Select(g => g.MediaId).Distinct().ToList();

            var galleryMediaMap = galleryMediaIds.Any()
                ? await _context.Set<Media>().AsNoTracking()
                    .Where(m => galleryMediaIds.Contains(m.Id) && m.IsDeletedStatu != true)
                    .ToDictionaryAsync(m => m.Id, m => m.FileUrl ?? m.FileUrl_Ratio_1_2 ?? "")
                : new Dictionary<Guid, string>();

            var productGalleryMap = galleryItems
                .Where(g => galleryMediaMap.ContainsKey(g.MediaId))
                .GroupBy(g => g.ItemId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => galleryMediaMap[x.MediaId])
                          .Where(u => !string.IsNullOrEmpty(u))
                          .Take(10).ToList()
                );

            // ── Markalar ─────────────────────────────────────────
            var brandIds = products
                .Where(p => p.BrandId.HasValue)
                .Select(p => p.BrandId!.Value)
                .Distinct().ToList();

            var brandMap = brandIds.Any()
                ? await _context.Set<Brands>().AsNoTracking()
                    .Where(b => brandIds.Contains(b.Id))
                    .ToDictionaryAsync(b => b.Id, b => b.Name ?? "")
                : new Dictionary<Guid, string>();

            // ── Varsayılan varyantlar ────────────────────────────
            var allVariants = await _context.Set<ProductVariants>().AsNoTracking()
                .Where(v => productIds.Contains(v.ProductId) &&
                            v.IsActive &&
                            (v.IsDeleted == null || v.IsDeleted.IsDeletedStatu != true))
                .OrderByDescending(v => v.IsDefault)
                .ThenBy(v => v.DisplayOrder)
                .ToListAsync();

            var variantByProduct = allVariants
                .GroupBy(v => v.ProductId)
                .ToDictionary(g => g.Key, g => g.First());

            var multiVariantSet = new HashSet<Guid>(
                allVariants.GroupBy(v => v.ProductId)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
            );

            // ── Fiyatlar ─────────────────────────────────────────
            var variantIds = variantByProduct.Values.Select(v => v.Id).ToList();

            var priceByVariant = (await _context.Set<ProductPrices>().AsNoTracking()
                .Where(pp => variantIds.Contains(pp.VariantId) && pp.EffectiveTo == null)
                .ToListAsync())
                .GroupBy(pp => pp.VariantId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(pp => pp.Currency == "TRY").First());

            // ── Stoklar ──────────────────────────────────────────
            var stockByVariant = await _context.Set<ProductStocks>().AsNoTracking()
                .Where(s => variantIds.Contains(s.VariantId) && s.TrackStock)
                .GroupBy(s => s.VariantId)
                .Select(g => new { VariantId = g.Key, Qty = g.Sum(x => x.Quantity) })
                .ToDictionaryAsync(x => x.VariantId, x => x.Qty);

            // ── Kategoriler (product_type) ────────────────────────
            var primaryCatByProduct = (await _context.Set<ProductCategories>().AsNoTracking()
                .Where(pc => productIds.Contains(pc.ProductId))
                .OrderByDescending(pc => pc.IsPrimary)
                .ToListAsync())
                .GroupBy(pc => pc.ProductId)
                .ToDictionary(g => g.Key, g => g.First().CategoryId);

            var allCats = await _context.Set<CategoriesTr>().AsNoTracking()
                .ToDictionaryAsync(c => c.Id, c => c);


            // ── RSS XML oluştur ──────────────────────────────────
            Response.ContentType = "application/xml; charset=utf-8";
            await using var writer = XmlWriter.Create(Response.Body, XmlSettings());
            await writer.WriteStartDocumentAsync();

            await writer.WriteStartElementAsync(null, "rss", null);
            await writer.WriteAttributeStringAsync(null, "version", null, "2.0");
            await writer.WriteAttributeStringAsync("xmlns", "g", null, gNs);

            await writer.WriteStartElementAsync(null, "channel", null);
            await writer.WriteElementStringAsync(null, "title", null, "efavori — Ürünler");
            await writer.WriteElementStringAsync(null, "link", null, baseUrl);
            await writer.WriteElementStringAsync(null, "description", null,
                "efavori.com ürün kataloğu — Google Merchant Center feed");

            foreach (var p in products)
            {
                var slug = !string.IsNullOrWhiteSpace(p.Slug) ? p.Slug : p.Id.ToString();
                var productUrl = $"{baseUrl}/Public/_Viewer/ProductProfile/{slug}";

                // Kapak görseli
                string coverImgFull = "";
                if (p.CoverMediaId.HasValue &&
                    coverMap.TryGetValue(p.CoverMediaId.Value, out var cUrl) &&
                    !string.IsNullOrEmpty(cUrl))
                {
                    coverImgFull = cUrl.StartsWith("http") ? cUrl : $"{baseUrl}{cUrl}";
                }

                // Varyant + Fiyat + Stok
                variantByProduct.TryGetValue(p.Id, out var variant);
                ProductPrices? price = variant != null &&
                    priceByVariant.TryGetValue(variant.Id, out var pr) ? pr : null;
                int stockQty = variant != null &&
                    stockByVariant.TryGetValue(variant.Id, out var sq) ? sq : 0;

                // Marka
                string brandName = p.BrandId.HasValue &&
                    brandMap.TryGetValue(p.BrandId.Value, out var bn) ? bn : "";

                // Kategori yolu
                string categoryPath = "";
                if (primaryCatByProduct.TryGetValue(p.Id, out var catId))
                {
                    var parts = new List<string>();
                    var cur = allCats.GetValueOrDefault(catId);
                    int guard = 0;
                    while (cur != null && guard++ < 20)
                    {
                        parts.Insert(0, cur.Name ?? "");
                        cur = cur.ParentCategoryId.HasValue
                            ? allCats.GetValueOrDefault(cur.ParentCategoryId.Value)
                            : null;
                    }
                    categoryPath = string.Join(" > ", parts.Where(x => !string.IsNullOrEmpty(x)));
                }

                // Description — HTML temizle, max 5000
                var desc = SanitizeHtml(p.ShortDescription ?? p.FullDescription ?? p.Name ?? "");
                if (desc.Length > 5000) desc = desc.Substring(0, 4997) + "...";

                // Para birimi
                var currency = NormalizeCurrency(price?.Currency);

                // Title — max 150
                var title = (p.Name ?? "Ürün");
                if (title.Length > 150) title = title.Substring(0, 147) + "...";


                // ── <item> ───────────────────────────────────────
                await writer.WriteStartElementAsync(null, "item", null);

                await writer.WriteElementStringAsync("g", "id", gNs, p.Id.ToString());
                await writer.WriteElementStringAsync("g", "title", gNs, title);
                await writer.WriteElementStringAsync("g", "description", gNs, desc);
                await writer.WriteElementStringAsync("g", "link", gNs, productUrl);

                // g:image_link
                if (!string.IsNullOrEmpty(coverImgFull))
                    await writer.WriteElementStringAsync("g", "image_link", gNs, coverImgFull);

                // g:additional_image_link
                if (productGalleryMap.TryGetValue(p.Id, out var gallery))
                {
                    foreach (var img in gallery.Take(10))
                    {
                        var full = img.StartsWith("http") ? img : $"{baseUrl}{img}";
                        if (full != coverImgFull)
                            await writer.WriteElementStringAsync("g", "additional_image_link", gNs, full);
                    }
                }

                // g:availability
                await writer.WriteElementStringAsync("g", "availability", gNs,
                    stockQty > 0 ? "in_stock" : "out_of_stock");

                // g:price + g:sale_price
                if (price != null)
                {
                    var basePrice = price.Price.ToString("F2", CultureInfo.InvariantCulture);
                    await writer.WriteElementStringAsync("g", "price", gNs, $"{basePrice} {currency}");

                    if (price.DiscountedPrice.HasValue &&
                        price.DiscountedPrice.Value > 0 &&
                        price.DiscountedPrice.Value < price.Price)
                    {
                        var sale = price.DiscountedPrice.Value.ToString("F2", CultureInfo.InvariantCulture);
                        await writer.WriteElementStringAsync("g", "sale_price", gNs, $"{sale} {currency}");
                    }
                }

                // g:brand
                if (!string.IsNullOrEmpty(brandName))
                    await writer.WriteElementStringAsync("g", "brand", gNs, brandName);

                // g:condition
                await writer.WriteElementStringAsync("g", "condition", gNs, "new");

                // g:product_type
                if (!string.IsNullOrEmpty(categoryPath))
                    await writer.WriteElementStringAsync("g", "product_type", gNs, categoryPath);

                // g:item_group_id (varyantlı ürünler)
                if (multiVariantSet.Contains(p.Id))
                    await writer.WriteElementStringAsync("g", "item_group_id", gNs, p.Id.ToString());

                // g:gtin / g:mpn / g:identifier_exists
                if (variant != null)
                {
                    var gtin = variant.Gtin ?? variant.Ean ?? variant.Upc;
                    if (!string.IsNullOrEmpty(gtin))
                        await writer.WriteElementStringAsync("g", "gtin", gNs, gtin);

                    if (!string.IsNullOrEmpty(variant.Mpn))
                        await writer.WriteElementStringAsync("g", "mpn", gNs, variant.Mpn);

                    if (string.IsNullOrEmpty(gtin) &&
                        string.IsNullOrEmpty(variant.Mpn) &&
                        string.IsNullOrEmpty(brandName))
                    {
                        await writer.WriteElementStringAsync("g", "identifier_exists", gNs, "no");
                    }
                }
                else if (string.IsNullOrEmpty(brandName))
                {
                    await writer.WriteElementStringAsync("g", "identifier_exists", gNs, "no");
                }

                // g:shipping
                await writer.WriteStartElementAsync("g", "shipping", gNs);
                await writer.WriteElementStringAsync("g", "country", gNs, "TR");
                await writer.WriteElementStringAsync("g", "service", gNs, "Standart Kargo");
                await writer.WriteElementStringAsync("g", "price", gNs, $"0.00 {currency}");
                await writer.WriteEndElementAsync(); // </g:shipping>

                await writer.WriteEndElementAsync(); // </item>
            }

            await writer.WriteEndElementAsync(); // </channel>
            await writer.WriteEndElementAsync(); // </rss>
            await writer.WriteEndDocumentAsync();
            return new EmptyResult();
        }


        // ═══════════════════════════════════════════════════════════
        //  6. ROBOTS.TXT  →  /robots.txt
        // ═══════════════════════════════════════════════════════════

        [Route("/robots.txt")]
        [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any)]
        public IActionResult RobotsTxt()
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            var sb = new StringBuilder();
            sb.AppendLine("User-agent: *");
            sb.AppendLine("Allow: /");
            sb.AppendLine();
            sb.AppendLine("Disallow: /Admin/");
            sb.AppendLine("Disallow: /api/");
            sb.AppendLine("Disallow: /Account/Login");
            sb.AppendLine("Disallow: /Account/Register");
            sb.AppendLine("Disallow: /_blazor");
            sb.AppendLine("Disallow: /_framework");
            sb.AppendLine();
            // Yalnızca gerçek XML sitemap bildirilmeli.
            // Google Merchant feed'leri (RSS 2.0) Search Console tarafından
            // sitemap olarak ayrıştırılamaz → "Desteklenmeyen dosya biçimi" hatası verir.
            // Merchant feed'ler Google Merchant Center panelinden bildirilmelidir.
            sb.AppendLine($"Sitemap: {baseUrl}/sitemap.xml");

            return Content(sb.ToString(), "text/plain", Encoding.UTF8);
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