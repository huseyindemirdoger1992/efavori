// ═══════════════════════════════════════════════════════════════════════════════
// GoogleMerchantCenter — Google Merchant Center RSS 2.0 Ürün Feed
// ═══════════════════════════════════════════════════════════════════════════════
//
//  FEED INDEX         → /feed/google/products-index.xml
//  SAYFALANMIŞ FEED   → /feed/google/products-{id}.xml
//
//  ATTRIBUTE HARİTASI (Google Merchant Center Spesifikasyonu):
//
//    ── Temel ──────────────────────────────────────────────────────
//      g:id                     → Products.Id
//      g:title                  → Products.Name                   (max 150)
//      g:description            → ShortDescription / FullDescription (max 5000)
//      g:link                   → /Public/_Viewer/ProductProfile/{slug}
//
//    ── Görseller ──────────────────────────────────────────────────
//      g:image_link             → CoverMediaId → Media.FileUrl
//      g:additional_image_link  → ItemGallery (max 10)
//
//    ── Fiyat & Stok ──────────────────────────────────────────────
//      g:price                  → ProductPrices.Price
//      g:sale_price             → ProductPrices.DiscountedPrice
//      g:availability           → in_stock / out_of_stock / preorder
//
//    ── Ürün Kimliği ──────────────────────────────────────────────
//      g:brand                  → Brands.Name
//      g:gtin                   → ProductVariants.Gtin / Ean / Upc
//      g:mpn                    → ProductVariants.Mpn
//      g:identifier_exists      → gtin/mpn/brand yoksa "no"
//
//    ── Kategori ──────────────────────────────────────────────────
//      g:google_product_category → CategoriesTr.ExternalId (Google Taxonomy ID)
//      g:product_type            → Kategori yolu (Antikalar > Korkuluklar)
//
//    ── Detay ─────────────────────────────────────────────────────
//      g:condition              → new (varsayılan)
//      g:item_group_id          → çoklu varyantlı ürünlerde Products.Id
//      g:color                  → ProductVariantValues (Code: renk/color)
//      g:size                   → ProductVariantValues (Code: beden/size)
//      g:material               → ProductVariantValues (Code: malzeme/material)
//      g:pattern                → ProductVariantValues (Code: desen/pattern)
//      g:gender                 → ProductVariantValues (Code: cinsiyet/gender)
//      g:age_group              → ProductVariantValues (Code: yas_grubu/age_group)
//      g:product_highlight      → Products.Tags (virgülle ayrılmış)
//
//    ── Fiziksel ──────────────────────────────────────────────────
//      g:product_weight         → ProductVariants.WeightKg  → "{x} kg"
//      g:shipping_weight        → ProductVariants.WeightKg  → "{x} kg"
//      g:product_length         → ProductVariants.LengthCm  → "{x} cm"
//      g:product_width          → ProductVariants.WidthCm   → "{x} cm"
//      g:product_height         → ProductVariants.HeightCm  → "{x} cm"
//
//    ── Kargo ─────────────────────────────────────────────────────
//      g:shipping               → country:TR, service:Standart Kargo
//
//    ── Satıcı (Marketplace) ──────────────────────────────────────
//      g:external_seller_id     → Store.Name (mağaza bazlı satıcı kimliği)
//
// ═══════════════════════════════════════════════════════════════════════════════

using data;
using data._Product;
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

namespace web.Areas.Sitemap.Products
{
    [Area("Sitemap")]
    [Route("/Sitemap/[controller]/[action]")]
    public class GoogleMerchantCenter : Controller
    {
        private readonly _ApplicationConnectionDb _context;

        private const int MaxItemsPerPage = 1_000;

        // Varyant özellik kodlarını Google attribute'larına eşleyen sözlük
        // Key: attribute Code veya Name (küçük harf, Türkçe/İngilizce)
        // Value: Google feed attribute adı
        private static readonly Dictionary<string, string> GoogleAttributeMap = new(StringComparer.OrdinalIgnoreCase)
        {
            // Renk
            { "renk",      "color" },
            { "color",     "color" },
            { "colour",    "color" },

            // Beden
            { "beden",     "size" },
            { "size",      "size" },
            { "boyut",     "size" },
            { "numara",    "size" },

            // Malzeme
            { "malzeme",   "material" },
            { "material",  "material" },
            { "kumas",     "material" },
            { "fabric",    "material" },

            // Desen
            { "desen",     "pattern" },
            { "pattern",   "pattern" },

            // Cinsiyet
            { "cinsiyet",  "gender" },
            { "gender",    "gender" },

            // Yaş Grubu
            { "yas_grubu",  "age_group" },
            { "yas grubu",  "age_group" },
            { "age_group",  "age_group" },
            { "age group",  "age_group" },
        };


        public GoogleMerchantCenter(_ApplicationConnectionDb context)
        {
            _context = context;
        }


        // ═══════════════════════════════════════════════════════════
        //  1. FEED INDEX  →  /feed/google/products-index.xml
        // ═══════════════════════════════════════════════════════════

        [Route("/feed/google/products-index.xml")]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> FeedIndex()
        {
            var total = await _context.Products.AsNoTracking()
                .Where(p => p.IsActive == true && p.IsApprovedByAdmin == true)
                .CountAsync();

            if (total == 0) return NotFound();

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var pages = (int)Math.Ceiling((double)total / MaxItemsPerPage);

            // Sitemap index formatında sayfa listesi
            Response.ContentType = "application/xml; charset=utf-8";
            await using var writer = XmlWriter.Create(Response.Body, XmlSettings());
            await writer.WriteStartDocumentAsync();

            await writer.WriteStartElementAsync(null, "sitemapindex", "http://www.sitemaps.org/schemas/sitemap/0.9");

            for (int i = 1; i <= pages; i++)
            {
                await writer.WriteStartElementAsync(null, "sitemap", null);
                await writer.WriteElementStringAsync(null, "loc", null,
                    $"{baseUrl}/feed/google/products-{i}.xml");
                await writer.WriteEndElementAsync();
            }

            await writer.WriteEndElementAsync(); // </sitemapindex>
            await writer.WriteEndDocumentAsync();
            return new EmptyResult();
        }


        // ═══════════════════════════════════════════════════════════
        //  2. SAYFALANMIŞ FEED  →  /feed/google/products-{id}.xml
        //     RSS 2.0 + g: namespace (Google Merchant Center)
        // ═══════════════════════════════════════════════════════════

        [Route("/feed/google/products-{id:int}.xml")]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> Feed(int id = 1)
        {
            if (id < 1) id = 1;

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            const string gNs = "http://base.google.com/ns/1.0";

            // ══════════════════════════════════════════════════════
            //  VERİ TOPLAMA (toplu sorgular — N+1 yok)
            // ══════════════════════════════════════════════════════

            // ── 2a. Ürünler (sayfalı) ─────────────────────────────
            var pagedQuery = _context.Products.AsNoTracking()
                .Where(p => p.IsActive == true && p.IsApprovedByAdmin == true)
                .OrderBy(p => p.Id)
                .Skip((id - 1) * MaxItemsPerPage)
                .Take(MaxItemsPerPage);

            var products = await (
                from p in pagedQuery
                join s in _context.ProductSeo.AsNoTracking()
                    on p.Id equals s.ProductId into seoJoin
                from seo in seoJoin.DefaultIfEmpty()
                select new
                {
                    p.Id,
                    p.Name,
                    p.ShortDescription,
                    p.FullDescription,
                    p.Tags,
                    p.CoverMediaId,
                    p.BrandId,
                    p.StoreId,
                    p.ProductType,
                    Slug = seo != null ? seo.Slug : null
                }
            ).ToListAsync();

            if (!products.Any()) return NotFound();

            var productIds = products.Select(p => p.Id).ToList();


            // ── 2b. Kapak görselleri ──────────────────────────────
            var coverMediaIds = products
                .Where(p => p.CoverMediaId.HasValue)
                .Select(p => p.CoverMediaId!.Value)
                .Distinct().ToList();

            var coverMap = coverMediaIds.Any()
                ? await _context.Set<Media>().AsNoTracking()
                    .Where(m => coverMediaIds.Contains(m.Id) && m.IsDeletedStatu != true)
                    .ToDictionaryAsync(m => m.Id, m => m.FileUrl ?? m.FileUrl_Ratio_1_2 ?? "")
                : new Dictionary<Guid, string>();


            // ── 2c. Galeri görselleri (max 10 per ürün) ───────────
            var galleryItems = await _context.Set<ItemGallery>().AsNoTracking()
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


            // ── 2d. Markalar ──────────────────────────────────────
            var brandIds = products
                .Where(p => p.BrandId.HasValue)
                .Select(p => p.BrandId!.Value)
                .Distinct().ToList();

            var brandMap = brandIds.Any()
                ? await _context.Set<Brands>().AsNoTracking()
                    .Where(b => brandIds.Contains(b.Id))
                    .ToDictionaryAsync(b => b.Id, b => b.Name ?? "")
                : new Dictionary<Guid, string>();


            // ── 2e. Mağazalar (external_seller_id) ────────────────
            var storeIds = products
                .Select(p => p.StoreId)
                .Distinct().ToList();

            var storeMap = storeIds.Any()
                ? await _context.Set<Store>().AsNoTracking()
                    .Where(s => storeIds.Contains(s.Id))
                    .ToDictionaryAsync(s => s.Id, s => s.Name ?? s.Id.ToString())
                : new Dictionary<Guid, string>();


            // ── 2f. Varsayılan varyantlar ─────────────────────────
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


            // ── 2g. Fiyatlar (güncel = EffectiveTo == null) ──────
            var variantIds = variantByProduct.Values.Select(v => v.Id).ToList();

            var priceByVariant = (await _context.Set<ProductPrices>().AsNoTracking()
                .Where(pp => variantIds.Contains(pp.VariantId) && pp.EffectiveTo == null)
                .ToListAsync())
                .GroupBy(pp => pp.VariantId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(pp => pp.Currency == "TRY").First());


            // ── 2h. Stoklar ───────────────────────────────────────
            var stockByVariant = await _context.Set<ProductStocks>().AsNoTracking()
                .Where(s => variantIds.Contains(s.VariantId) && s.TrackStock)
                .GroupBy(s => s.VariantId)
                .Select(g => new { VariantId = g.Key, Qty = g.Sum(x => x.Quantity) })
                .ToDictionaryAsync(x => x.VariantId, x => x.Qty);


            // ── 2i. Kategoriler ───────────────────────────────────
            var primaryCatByProduct = (await _context.Set<ProductCategories>().AsNoTracking()
                .Where(pc => productIds.Contains(pc.ProductId))
                .OrderByDescending(pc => pc.IsPrimary)
                .ToListAsync())
                .GroupBy(pc => pc.ProductId)
                .ToDictionary(g => g.Key, g => g.First().CategoryId);

            var allCats = await _context.Set<CategoriesTr>().AsNoTracking()
                .ToDictionaryAsync(c => c.Id, c => c);


            // ── 2j. Varyant özellik değerleri ─────────────────────
            //    (Renk, Beden, Malzeme, Desen, Cinsiyet, Yaş Grubu)
            var variantAttrData = await (
                from vv in _context.Set<ProductVariantValues>().AsNoTracking()
                join a in _context.Set<ProductAttributes>().AsNoTracking()
                    on vv.AttributeId equals a.Id
                join av in _context.Set<ProductAttributeValues>().AsNoTracking()
                    on vv.AttributeValueId equals av.Id
                where variantIds.Contains(vv.VariantId)
                select new
                {
                    vv.VariantId,
                    AttrName = a.Name,
                    AttrCode = a.Code,
                    Value = av.Value
                }
            ).ToListAsync();

            // VariantId → { "color": "Kırmızı", "size": "M", ... }
            var variantGoogleAttrs = variantAttrData
                .GroupBy(x => x.VariantId)
                .ToDictionary(
                    g => g.Key,
                    g =>
                    {
                        var dict = new Dictionary<string, string>();
                        foreach (var item in g)
                        {
                            if (string.IsNullOrEmpty(item.Value)) continue;

                            // Önce Code ile eşle, bulamazsa Name ile dene
                            string? googleAttr = null;
                            if (!string.IsNullOrEmpty(item.AttrCode) &&
                                GoogleAttributeMap.TryGetValue(item.AttrCode, out var ga1))
                            {
                                googleAttr = ga1;
                            }
                            else if (!string.IsNullOrEmpty(item.AttrName) &&
                                     GoogleAttributeMap.TryGetValue(item.AttrName, out var ga2))
                            {
                                googleAttr = ga2;
                            }

                            if (googleAttr != null && !dict.ContainsKey(googleAttr))
                                dict[googleAttr] = item.Value;
                        }
                        return dict;
                    }
                );


            // ══════════════════════════════════════════════════════
            //  RSS 2.0 XML OLUŞTUR
            // ══════════════════════════════════════════════════════

            Response.ContentType = "application/xml; charset=utf-8";
            await using var writer = XmlWriter.Create(Response.Body, XmlSettings());
            await writer.WriteStartDocumentAsync();

            // <rss xmlns:g="http://base.google.com/ns/1.0" version="2.0">
            await writer.WriteStartElementAsync(null, "rss", null);
            await writer.WriteAttributeStringAsync(null, "version", null, "2.0");
            await writer.WriteAttributeStringAsync("xmlns", "g", null, gNs);

            // <channel>
            await writer.WriteStartElementAsync(null, "channel", null);
            await writer.WriteElementStringAsync(null, "title", null, "efavori — Ürünler");
            await writer.WriteElementStringAsync(null, "link", null, baseUrl);
            await writer.WriteElementStringAsync(null, "description", null,
                "efavori.com ürün kataloğu — Google Merchant Center RSS 2.0 feed");


            // ── <item> döngüsü ────────────────────────────────────
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

                // Mağaza (external_seller_id)
                string sellerName = storeMap.TryGetValue(p.StoreId, out var sn) ? sn : "";

                // Para birimi
                var currency = NormalizeCurrency(price?.Currency);

                // Title — max 150 karakter
                var title = (p.Name ?? "Ürün");
                if (title.Length > 150) title = title[..147] + "...";

                // Description — HTML temizle, max 5000 karakter
                var desc = SanitizeHtml(p.ShortDescription ?? p.FullDescription ?? p.Name ?? "");
                if (desc.Length > 5000) desc = desc[..4997] + "...";

                // Kategori yolu (product_type)
                string categoryPath = "";
                int? googleCategoryId = null;
                if (primaryCatByProduct.TryGetValue(p.Id, out var catId))
                {
                    var parts = new List<string>();
                    var cur = allCats.GetValueOrDefault(catId);
                    int guard = 0;

                    // Yaprak kategoriden köke doğru çık
                    while (cur != null && guard++ < 20)
                    {
                        parts.Insert(0, cur.Name ?? "");

                        // İlk geçerli ExternalId = Google Taxonomy ID
                        if (googleCategoryId == null && cur.ExternalId.HasValue)
                            googleCategoryId = cur.ExternalId.Value;

                        cur = cur.ParentCategoryId.HasValue
                            ? allCats.GetValueOrDefault(cur.ParentCategoryId.Value)
                            : null;
                    }

                    categoryPath = string.Join(" > ", parts.Where(x => !string.IsNullOrEmpty(x)));
                }

                // Varyant özellik değerleri (color, size, material, vb.)
                Dictionary<string, string>? gAttrs = variant != null &&
                    variantGoogleAttrs.TryGetValue(variant.Id, out var ga) ? ga : null;


                // ── <item> başlat ─────────────────────────────────
                await writer.WriteStartElementAsync(null, "item", null);


                // ━━━ TEMEL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

                // g:id
                await writer.WriteElementStringAsync("g", "id", gNs, p.Id.ToString());

                // g:title
                await writer.WriteElementStringAsync("g", "title", gNs, title);

                // g:description
                await writer.WriteElementStringAsync("g", "description", gNs, desc);

                // g:link
                await writer.WriteElementStringAsync("g", "link", gNs, productUrl);


                // ━━━ GÖRSELLER ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

                // g:image_link
                if (!string.IsNullOrEmpty(coverImgFull))
                    await writer.WriteElementStringAsync("g", "image_link", gNs, coverImgFull);

                // g:additional_image_link (max 10, kapak hariç)
                if (productGalleryMap.TryGetValue(p.Id, out var gallery))
                {
                    foreach (var img in gallery.Take(10))
                    {
                        var full = img.StartsWith("http") ? img : $"{baseUrl}{img}";
                        if (full != coverImgFull)
                            await writer.WriteElementStringAsync("g", "additional_image_link", gNs, full);
                    }
                }


                // ━━━ FİYAT & STOK ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

                // g:availability
                await writer.WriteElementStringAsync("g", "availability", gNs,
                    stockQty > 0 ? "in_stock" : "out_of_stock");

                // g:price
                if (price != null)
                {
                    var basePrice = price.Price.ToString("F2", CultureInfo.InvariantCulture);
                    await writer.WriteElementStringAsync("g", "price", gNs,
                        $"{basePrice} {currency}");

                    // g:sale_price
                    if (price.DiscountedPrice.HasValue &&
                        price.DiscountedPrice.Value > 0 &&
                        price.DiscountedPrice.Value < price.Price)
                    {
                        var salePrice = price.DiscountedPrice.Value
                            .ToString("F2", CultureInfo.InvariantCulture);
                        await writer.WriteElementStringAsync("g", "sale_price", gNs,
                            $"{salePrice} {currency}");
                    }
                }


                // ━━━ ÜRÜN KİMLİĞİ ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

                // g:brand
                if (!string.IsNullOrEmpty(brandName))
                    await writer.WriteElementStringAsync("g", "brand", gNs, brandName);

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


                // ━━━ KATEGORİ ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

                // g:google_product_category (sayısal Google Taxonomy ID)
                if (googleCategoryId.HasValue)
                {
                    await writer.WriteElementStringAsync("g", "google_product_category", gNs,
                        googleCategoryId.Value.ToString());
                }

                // g:product_type (kendi kategori yolumuz)
                if (!string.IsNullOrEmpty(categoryPath))
                    await writer.WriteElementStringAsync("g", "product_type", gNs, categoryPath);


                // ━━━ DETAY ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

                // g:condition
                await writer.WriteElementStringAsync("g", "condition", gNs, "new");

                // g:item_group_id (çoklu varyantlı ürünler)
                if (multiVariantSet.Contains(p.Id))
                    await writer.WriteElementStringAsync("g", "item_group_id", gNs, p.Id.ToString());

                // Varyant özellikleri: g:color, g:size, g:material, g:pattern, g:gender, g:age_group
                if (gAttrs != null)
                {
                    if (gAttrs.TryGetValue("color", out var colorVal))
                        await writer.WriteElementStringAsync("g", "color", gNs, colorVal);

                    if (gAttrs.TryGetValue("size", out var sizeVal))
                        await writer.WriteElementStringAsync("g", "size", gNs, sizeVal);

                    if (gAttrs.TryGetValue("material", out var materialVal))
                        await writer.WriteElementStringAsync("g", "material", gNs, materialVal);

                    if (gAttrs.TryGetValue("pattern", out var patternVal))
                        await writer.WriteElementStringAsync("g", "pattern", gNs, patternVal);

                    if (gAttrs.TryGetValue("gender", out var genderVal))
                        await writer.WriteElementStringAsync("g", "gender", gNs, NormalizeGender(genderVal));

                    if (gAttrs.TryGetValue("age_group", out var ageVal))
                        await writer.WriteElementStringAsync("g", "age_group", gNs, NormalizeAgeGroup(ageVal));
                }

                // g:product_highlight (Tags alanından, virgülle ayrılmış)
                if (!string.IsNullOrEmpty(p.Tags))
                {
                    var highlights = p.Tags
                        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Where(t => t.Length <= 150)
                        .Take(100); // Google max 100 highlight

                    foreach (var h in highlights)
                        await writer.WriteElementStringAsync("g", "product_highlight", gNs, h);
                }


                // ━━━ FİZİKSEL BOYUTLAR ━━━━━━━━━━━━━━━━━━━━━━━━━━

                if (variant != null)
                {
                    // g:product_weight + g:shipping_weight
                    if (variant.WeightKg.HasValue && variant.WeightKg.Value > 0)
                    {
                        var w = variant.WeightKg.Value.ToString("F2", CultureInfo.InvariantCulture);
                        await writer.WriteElementStringAsync("g", "product_weight", gNs, $"{w} kg");
                        await writer.WriteElementStringAsync("g", "shipping_weight", gNs, $"{w} kg");
                    }

                    // g:product_length
                    if (variant.LengthCm.HasValue && variant.LengthCm.Value > 0)
                    {
                        var l = variant.LengthCm.Value.ToString("F1", CultureInfo.InvariantCulture);
                        await writer.WriteElementStringAsync("g", "product_length", gNs, $"{l} cm");
                    }

                    // g:product_width
                    if (variant.WidthCm.HasValue && variant.WidthCm.Value > 0)
                    {
                        var wd = variant.WidthCm.Value.ToString("F1", CultureInfo.InvariantCulture);
                        await writer.WriteElementStringAsync("g", "product_width", gNs, $"{wd} cm");
                    }

                    // g:product_height
                    if (variant.HeightCm.HasValue && variant.HeightCm.Value > 0)
                    {
                        var h = variant.HeightCm.Value.ToString("F1", CultureInfo.InvariantCulture);
                        await writer.WriteElementStringAsync("g", "product_height", gNs, $"{h} cm");
                    }
                }


                // ━━━ KARGO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

                // <g:shipping>
                await writer.WriteStartElementAsync("g", "shipping", gNs);
                await writer.WriteElementStringAsync("g", "country", gNs, "TR");
                await writer.WriteElementStringAsync("g", "service", gNs, "Standart Kargo");
                await writer.WriteElementStringAsync("g", "price", gNs, $"0.00 {currency}");
                await writer.WriteEndElementAsync(); // </g:shipping>


                // ━━━ SATICI (MARKETPLACE) ━━━━━━━━━━━━━━━━━━━━━━━━

                // g:external_seller_id
                if (!string.IsNullOrEmpty(sellerName))
                    await writer.WriteElementStringAsync("g", "external_seller_id", gNs, sellerName);


                await writer.WriteEndElementAsync(); // </item>
            }

            await writer.WriteEndElementAsync(); // </channel>
            await writer.WriteEndElementAsync(); // </rss>
            await writer.WriteEndDocumentAsync();
            return new EmptyResult();
        }


        // ═══════════════════════════════════════════════════════════
        //  YARDIMCI METODLAR
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// HTML etiketlerini temizler, sadece düz metin bırakır.
        /// </summary>
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

        /// <summary>
        /// Para birimi kodunu ISO 4217 formatına normalize eder.
        /// </summary>
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

        /// <summary>
        /// Cinsiyet değerini Google Merchant Center formatına dönüştürür.
        /// Desteklenen değerler: male, female, unisex
        /// </summary>
        private static string NormalizeGender(string value)
        {
            var lower = value.Trim().ToLowerInvariant();
            return lower switch
            {
                "erkek" or "male" or "bay" => "male",
                "kadın" or "kadin" or "female" or "bayan" => "female",
                "unisex" or "her ikisi" or "herkese" => "unisex",
                _ => lower
            };
        }

        /// <summary>
        /// Yaş grubu değerini Google Merchant Center formatına dönüştürür.
        /// Desteklenen değerler: newborn, infant, toddler, kids, adult
        /// </summary>
        private static string NormalizeAgeGroup(string value)
        {
            var lower = value.Trim().ToLowerInvariant();
            return lower switch
            {
                "yenidogan" or "yenidoğan" or "newborn" or "0-3 ay" => "newborn",
                "bebek" or "infant" or "3-12 ay" => "infant",
                "küçük çocuk" or "kucuk cocuk" or "toddler" or "1-5 yaş" or "1-5 yas" => "toddler",
                "çocuk" or "cocuk" or "kids" or "5-13 yaş" or "5-13 yas" => "kids",
                "yetişkin" or "yetiskin" or "adult" or "yetişkin" => "adult",
                _ => lower
            };
        }

        /// <summary>
        /// XmlWriter ayarları — async, UTF-8, girintili.
        /// </summary>
        private static XmlWriterSettings XmlSettings() => new()
        {
            Async = true,
            Encoding = Encoding.UTF8,
            Indent = true
        };
    }
}