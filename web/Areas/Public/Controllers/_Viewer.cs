// ═══════════════════════════════════════════════════════════════════════
// ProductProfile — SEO-Optimized Controller Action
// ═══════════════════════════════════════════════════════════════════════
// Bu action, _Layout.cshtml içindeki ViewData slotlarını besler:
//   ViewData["CanonicalUrl"]   → rel="canonical"
//   ViewData["Title"]          → <title> + og:title + twitter:title
//   ViewData["Description"]    → meta description + og:description
//   ViewData["Keywords"]       → meta keywords
//   ViewData["OgType"]         → og:type (product)
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
using data._Product;
using Data;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Globalization;


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
    public class _Viewer : Controller
    {
        public IActionResult UserProfile(Guid id)
        {
            ViewBag.UserProfile = id;
            return View();
        }

        public IActionResult ProductProfile(string id)
        {
            using (_ApplicationConnectionDb db = new _ApplicationConnectionDb())
            {
                // ═══════════════════════════════════════════════════════
                // 1. ÜRÜN + SEO VERİSİNİ ÇEK
                // ═══════════════════════════════════════════════════════
                var pageData = db.ProductSeo
                    .Where(seo => seo.Slug == id)
                    .Join(
                        db.Products,
                        seo => seo.ProductId,
                        p => p.Id,
                        (seo, p) => new { Product = p, SeoInfo = seo }
                    )
                    .FirstOrDefault();

                if (pageData == null)
                {
                    return NotFound();
                }

                var product = pageData.Product;
                var seo = pageData.SeoInfo;
                var productId = product.Id;

                // ═══════════════════════════════════════════════════════
                // 2. İLİŞKİSEL VERİLERİ ÇEK (tek seferde)
                // ═══════════════════════════════════════════════════════

                // --- Kapak görseli URL ---
                string? coverImageUrl = null;
                if (product.CoverMediaId.HasValue)
                {
                    coverImageUrl = db.Set<Media>().AsNoTracking()
                        .Where(m => m.Id == product.CoverMediaId.Value && m.IsDeletedStatu != true)
                        .Select(m => m.FileUrl_Ratio_1_2 ?? m.FileUrl)
                        .FirstOrDefault();
                }

                // --- Marka ---
                string? brandName = null;
                if (product.BrandId.HasValue)
                {
                    brandName = db.Set<Brands>().AsNoTracking()
                        .Where(b => b.Id == product.BrandId.Value)
                        .Select(b => b.Name)
                        .FirstOrDefault();
                }

                // --- Mağaza (Seller) ---
                var store = db.Set<Store>().AsNoTracking()
                    .FirstOrDefault(s => s.Id == product.StoreId);

                // --- Mağaza profil görseli ---
                string? storeLogoUrl = store?.ProfileCoverGallery?.ProfileImagePath;

                // --- Breadcrumb kategorileri ---
                var primaryCatId = db.Set<ProductCategories>().AsNoTracking()
                    .Where(pc => pc.ProductId == productId)
                    .OrderByDescending(pc => pc.IsPrimary)
                    .Select(pc => (int?)pc.CategoryId)
                    .FirstOrDefault();

                var breadcrumbPath = new List<CategoriesTr>();
                if (primaryCatId.HasValue)
                {
                    var allCats = db.Set<CategoriesTr>().AsNoTracking()
                        .ToDictionary(c => c.Id, c => c);

                    var current = allCats.GetValueOrDefault(primaryCatId.Value);
                    int guard = 0;
                    while (current != null && guard++ < 20)
                    {
                        breadcrumbPath.Insert(0, current);
                        current = current.ParentCategoryId.HasValue
                            ? allCats.GetValueOrDefault(current.ParentCategoryId.Value)
                            : null;
                    }
                }

                // --- Varsayılan varyant + Fiyat bilgisi ---
                var defaultVariant = db.Set<ProductVariants>().AsNoTracking()
                    .Where(v => v.ProductId == productId &&
                                v.IsActive &&
                                (v.IsDeleted == null || v.IsDeleted.IsDeletedStatu != true))
                    .OrderByDescending(v => v.IsDefault)
                    .ThenBy(v => v.DisplayOrder)
                    .FirstOrDefault();

                decimal? currentPrice = null;
                decimal? discountedPrice = null;
                string priceCurrency = "TRY";

                if (defaultVariant != null)
                {
                    var priceRecord = db.Set<ProductPrices>().AsNoTracking()
                        .Where(pp => pp.VariantId == defaultVariant.Id && pp.EffectiveTo == null)
                        .OrderByDescending(pp => pp.Currency == "TRY") // TRY öncelikli
                        .FirstOrDefault();

                    if (priceRecord != null)
                    {
                        currentPrice = priceRecord.Price;
                        discountedPrice = priceRecord.DiscountedPrice;
                        priceCurrency = priceRecord.Currency ?? "TRY";
                    }
                }

                // --- Stok durumu ---
                string stockStatus = "https://schema.org/OutOfStock";
                if (defaultVariant != null)
                {
                    var hasStock = db.Set<ProductStocks>().AsNoTracking()
                        .Any(s => s.VariantId == defaultVariant.Id &&
                                  s.TrackStock &&
                                  s.Quantity > 0);

                    stockStatus = hasStock
                        ? "https://schema.org/InStock"
                        : "https://schema.org/OutOfStock";
                }

                // --- Tüm varyant bilgileri (ProductGroup için) ---
                var allVariants = db.Set<ProductVariants>().AsNoTracking()
                    .Where(v => v.ProductId == productId &&
                                v.IsActive &&
                                (v.IsDeleted == null || v.IsDeleted.IsDeletedStatu != true))
                    .OrderByDescending(v => v.IsDefault)
                    .ThenBy(v => v.DisplayOrder)
                    .ToList();

                var allVariantIds = allVariants.Select(v => v.Id).ToList();

                // Her varyantın fiyatları
                var allPrices = db.Set<ProductPrices>().AsNoTracking()
                    .Where(pp => allVariantIds.Contains(pp.VariantId) && pp.EffectiveTo == null)
                    .ToList();

                // Her varyantın stokları
                var allStocks = db.Set<ProductStocks>().AsNoTracking()
                    .Where(s => allVariantIds.Contains(s.VariantId) && s.TrackStock)
                    .GroupBy(s => s.VariantId)
                    .Select(g => new { VariantId = g.Key, TotalQty = g.Sum(x => x.Quantity) })
                    .ToDictionary(x => x.VariantId, x => x.TotalQty);

                // Varyant değerleri
                var variantValues = db.Set<ProductVariantValues>().AsNoTracking()
                    .Where(vv => allVariantIds.Contains(vv.VariantId))
                    .ToList();

                var attrValueIds = variantValues.Select(vv => vv.AttributeValueId).Distinct().ToList();
                var attrIds = variantValues.Select(vv => vv.AttributeId).Distinct().ToList();

                var attrNames = db.Set<ProductAttributes>().AsNoTracking()
                    .Where(a => attrIds.Contains(a.Id))
                    .ToDictionary(a => a.Id, a => a.Name ?? "Özellik");

                var attrValueNames = db.Set<ProductAttributeValues>().AsNoTracking()
                    .Where(av => attrValueIds.Contains(av.Id))
                    .ToDictionary(av => av.Id, av => av.Value ?? "—");

                // --- Galeri görselleri (ek görseller URL'leri — og:image:alt candidates) ---
                var galleryUrls = new List<string>();
                if (!string.IsNullOrEmpty(coverImageUrl))
                    galleryUrls.Add(coverImageUrl);

                var galleryMediaIds = db.Set<ItemGallery>().AsNoTracking()
                    .Where(ig => ig.ItemId == productId &&
                                 ig.IsDelete != true &&
                                 (ig.ItemType == "Product" || ig.ItemType == "ProductGallery"))
                    .OrderBy(ig => ig.ItemAddDate)
                    .Select(ig => ig.MediaId)
                    .Take(5)
                    .ToList();

                var gMediaIds = galleryMediaIds.Where(x => x.HasValue).Select(x => x!.Value).Distinct().ToList();
                if (gMediaIds.Any())
                {
                    var gMedia = db.Set<Media>().AsNoTracking()
                        .Where(m => gMediaIds.Contains(m.Id) && m.IsDeletedStatu != true)
                        .ToDictionary(m => m.Id, m => m.FileUrl_Ratio_1_2 ?? m.FileUrl ?? "");

                    foreach (var mid in gMediaIds)
                        if (gMedia.TryGetValue(mid, out var url) && !string.IsNullOrEmpty(url) && !galleryUrls.Contains(url))
                            galleryUrls.Add(url);
                }


                // ═══════════════════════════════════════════════════════
                // 3. SEO DEĞERLERİNİ HESAPLA
                // ═══════════════════════════════════════════════════════

                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var slug = seo.Slug ?? productId.ToString();

                // --- Canonical URL ---
                // ProductSeo.CanonicalUrl varsa onu kullan, yoksa slug-based URL üret
                var canonicalUrl = !string.IsNullOrEmpty(seo.CanonicalUrl)
                    ? seo.CanonicalUrl
                    : $"{baseUrl}/Public/_Viewer/ProductProfile/{slug}";

                // --- Title: SeoTitle > Product.Name ---
                var rawTitle = !string.IsNullOrEmpty(seo.SeoTitle)
                    ? seo.SeoTitle
                    : product.Name ?? "Ürün Detayı";

                // Sadece ilk harfleri büyük yapar, tamamen büyük harf karmaşasını önler
                var pageTitle = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(rawTitle.ToLower());

                // --- Description: SeoDescription > ShortDescription (max 160 karakter) ---
                var rawDescription = !string.IsNullOrEmpty(seo.SeoDescription)
                    ? seo.SeoDescription
                    : product.ShortDescription ?? $"{product.Name} — efavori.com'da en uygun fiyatlarla.";

                var pageDescription = rawDescription;
                if (pageDescription.Length > 160)
                {
                    // 157 karakterden sonraki ilk boşluğa kadar keser, "..." ekler
                    var truncated = pageDescription.Substring(0, 157);
                    int lastSpace = truncated.LastIndexOf(' ');
                    pageDescription = (lastSpace > 0 ? truncated.Substring(0, lastSpace) : truncated) + "...";
                }

                // --- Keywords: SeoKeywords > Tags > AiOriginalTags ---
                var pageKeywords = !string.IsNullOrEmpty(seo.SeoKeywords)
                    ? seo.SeoKeywords
                    : product.Tags ?? product.AiOriginalTags ?? "";

                // --- OG Image: Kapak görseli tam URL ---
                var ogImageUrl = !string.IsNullOrEmpty(coverImageUrl)
                    ? (coverImageUrl.StartsWith("http") ? coverImageUrl : $"{baseUrl}{coverImageUrl}")
                    : $"{baseUrl}/_files/main/logo/og-default.png";

                // --- Preload Image: LCP için aynı kapak görseli ---
                var preloadImageUrl = coverImageUrl; // Relative URL yeterli, tarayıcı çözümler

                // --- Robots ---
                // Aktif + Onaylı ürünler indexlensin; draft/pasif ürünler noindex
                var robotsContent = (product.IsActive && product.IsApprovedByAdmin == true)
                    ? "index, follow, max-image-preview:large, max-snippet:-1, max-video-preview:-1"
                    : "noindex, nofollow";


                // ═══════════════════════════════════════════════════════
                // 4. JSON-LD YAPISAL VERİ OLUŞTUR (SSR)
                // ═══════════════════════════════════════════════════════

                var jsonLdScripts = new List<string>();

                // ISO 4217 para birimi kodu
                var isoCurrency = priceCurrency?.ToUpperInvariant() switch
                {
                    "TRY" => "TRY",
                    "USD" => "USD",
                    "EUR" => "EUR",
                    "AZN" => "AZN",
                    "GBP" => "GBP",
                    _ => "TRY"
                };

                // ───────────────────────────────────────────────────────
                // 4a. BreadcrumbList Schema
                // ───────────────────────────────────────────────────────
                var breadcrumbItems = new List<object>
        {
            new
            {
                @type = "ListItem",
                position = 1,
                name = "Ana Sayfa",
                item = baseUrl + "/"
            },
            new
            {
                @type = "ListItem",
                position = 2,
                name = "Ürünler",
                item = baseUrl + "/Public/Home/Index"
            }
        };

                int bcPosition = 3;
                foreach (var cat in breadcrumbPath)
                {
                    breadcrumbItems.Add(new
                    {
                        @type = "ListItem",
                        position = bcPosition++,
                        name = cat.Name ?? "Kategori"
                        // item atlanıyor → son öğe (current page) için Schema.org bunu önerir
                    });
                }

                // Son öğe: ürünün kendisi
                breadcrumbItems.Add(new
                {
                    @type = "ListItem",
                    position = bcPosition,
                    name = product.Name ?? "Ürün",
                    item = canonicalUrl
                });

                var breadcrumbSchema = new
                {
                    @context = "https://schema.org",
                    @type = "BreadcrumbList",
                    itemListElement = breadcrumbItems
                };

                jsonLdScripts.Add(SerializeJsonLd(breadcrumbSchema));

                // ───────────────────────────────────────────────────────
                // 4b. Product + Offer + Seller Schema
                // ───────────────────────────────────────────────────────

                // Offer: Satıcı bilgisi + fiyat + stok
                var offerObj = new Dictionary<string, object?>
                {
                    ["@type"] = "Offer",
                    ["url"] = canonicalUrl,
                    ["priceCurrency"] = isoCurrency,
                    ["availability"] = stockStatus,
                    ["itemCondition"] = "https://schema.org/NewCondition"
                };

                // Fiyat: İndirimli varsa indirimliyi, yoksa normal fiyatı ver
                if (discountedPrice.HasValue && discountedPrice.Value > 0 && discountedPrice.Value < currentPrice)
                {
                    offerObj["price"] = discountedPrice.Value.ToString("F2", CultureInfo.InvariantCulture);

                    // Orijinal fiyat schema.org priceValidUntil ile gösterilebilir,
                    // ama Google doğrudan "price" alanını kullanır
                }
                else if (currentPrice.HasValue)
                {
                    offerObj["price"] = currentPrice.Value.ToString("F2", CultureInfo.InvariantCulture);
                }

                // Satıcı (Seller → Organization)
                if (store != null)
                {
                    var sellerObj = new Dictionary<string, object?>
                    {
                        ["@type"] = "Organization",
                        ["name"] = store.Name ?? "efavori Satıcı"
                    };

                    if (!string.IsNullOrEmpty(storeLogoUrl))
                    {
                        sellerObj["logo"] = storeLogoUrl.StartsWith("http")
                            ? storeLogoUrl
                            : $"{baseUrl}{storeLogoUrl}";
                    }

                    if (store.AddressInfo != null && !string.IsNullOrEmpty(store.AddressInfo.Country))
                    {
                        sellerObj["address"] = new
                        {
                            @type = "PostalAddress",
                            addressLocality = store.AddressInfo.City ?? "",
                            addressRegion = store.AddressInfo.State ?? "",
                            addressCountry = store.AddressInfo.Country ?? "TR"
                        };
                    }

                    offerObj["seller"] = sellerObj;
                }

                // Product schema gövdesi
                var productSchema = new Dictionary<string, object?>
                {
                    ["@context"] = "https://schema.org",
                    ["@type"] = "Product",
                    ["name"] = product.Name,
                    ["url"] = canonicalUrl,
                    ["description"] = rawDescription, // Tam açıklama (kısaltılmamış)
                    ["brand"] = !string.IsNullOrEmpty(brandName) ? new { @type = "Brand", name = brandName } : null
                };
                if (productSchema["brand"] == null) productSchema.Remove("brand");
                // Görseller
                if (galleryUrls.Any())
                {
                    productSchema["image"] = galleryUrls.Select(u =>
                        u.StartsWith("http") ? u : $"{baseUrl}{u}").ToList();
                }

                // SKU + GTIN
                if (defaultVariant != null)
                {
                    if (!string.IsNullOrEmpty(defaultVariant.Sku))
                        productSchema["sku"] = defaultVariant.Sku;
                    if (!string.IsNullOrEmpty(defaultVariant.Gtin))
                        productSchema["gtin"] = defaultVariant.Gtin;
                    if (!string.IsNullOrEmpty(defaultVariant.Mpn))
                        productSchema["mpn"] = defaultVariant.Mpn;
                    if (!string.IsNullOrEmpty(defaultVariant.Ean))
                        productSchema["gtin13"] = defaultVariant.Ean;
                    if (!string.IsNullOrEmpty(defaultVariant.Upc))
                        productSchema["gtin12"] = defaultVariant.Upc;
                    if (!string.IsNullOrEmpty(defaultVariant.Isbn))
                        productSchema["isbn"] = defaultVariant.Isbn;
                }

                // --- Marka ---
                // brandName zaten çekilmiş, schema kısmındaki "brand" bloğunu şu şekilde güncelle:

                if (!string.IsNullOrEmpty(brandName))
                {
                    productSchema["brand"] = new
                    {
                        @type = "Brand",
                        name = brandName // PowerMaster yerine veritabanından gelen değeri kullanır
                    };
                }

                // Kategori (en derin kategori adı)
                if (breadcrumbPath.Any())
                {
                    productSchema["category"] = string.Join(" > ", breadcrumbPath.Select(c => c.Name));
                }

                // Offer
                productSchema["offers"] = offerObj;

                // ───────────────────────────────────────────────────────
                // 4c. ProductGroup (varyasyonlar varsa)
                // ───────────────────────────────────────────────────────
                // Birden fazla aktif varyant varsa → Product'ı ProductGroup ile sar
                if (allVariants.Count > 1)
                {
                    // ProductGroup, "hasVariant" ile her varyantı bağımsız bir Product olarak listeler
                    var variantProducts = new List<object>();

                    foreach (var variant in allVariants)
                    {
                        var vPrice = allPrices
                            .Where(pp => pp.VariantId == variant.Id)
                            .OrderByDescending(pp => pp.Currency == "TRY")
                            .FirstOrDefault();

                        var vStockQty = allStocks.GetValueOrDefault(variant.Id, 0);

                        var vOffer = new Dictionary<string, object?>
                        {
                            ["@type"] = "Offer",
                            ["priceCurrency"] = (vPrice?.Currency ?? "TRY").ToUpperInvariant(),
                            ["availability"] = vStockQty > 0
                                ? "https://schema.org/InStock"
                                : "https://schema.org/OutOfStock"
                        };

                        if (vPrice != null)
                        {
                            var effectivePrice = (vPrice.DiscountedPrice.HasValue && vPrice.DiscountedPrice > 0 && vPrice.DiscountedPrice < vPrice.Price)
                                ? vPrice.DiscountedPrice.Value
                                : vPrice.Price;
                            vOffer["price"] = effectivePrice.ToString("F2", CultureInfo.InvariantCulture);
                        }

                        if (store != null)
                        {
                            vOffer["seller"] = new { @type = "Organization", name = store.Name ?? "efavori Satıcı" };
                        }

                        // Varyantın özellik değerleri
                        var thisVariantValues = variantValues
                            .Where(vv => vv.VariantId == variant.Id)
                            .ToList();

                        var variantSpecList = new List<object>();
                        foreach (var vv in thisVariantValues)
                        {
                            var aName = attrNames.GetValueOrDefault(vv.AttributeId, "Özellik");
                            var vName = attrValueNames.GetValueOrDefault(vv.AttributeValueId, "—");
                            variantSpecList.Add(new
                            {
                                @type = "PropertyValue",
                                name = aName,
                                value = vName
                            });
                        }

                        var variantProduct = new Dictionary<string, object?>
                        {
                            ["@type"] = "Product",
                            ["name"] = product.Name,
                            ["offers"] = vOffer
                        };

                        if (!string.IsNullOrEmpty(variant.Sku))
                            variantProduct["sku"] = variant.Sku;
                        if (!string.IsNullOrEmpty(variant.Gtin))
                            variantProduct["gtin"] = variant.Gtin;

                        if (variantSpecList.Any())
                            variantProduct["additionalProperty"] = variantSpecList;

                        variantProducts.Add(variantProduct);
                    }

                    // Ana şemayı ProductGroup olarak yeniden kurgula
                    productSchema["@type"] = "ProductGroup";
                    productSchema["productGroupID"] = productId.ToString();
                    productSchema["hasVariant"] = variantProducts;

                    // Varyant ayırıcı özellik isimleri
                    var varyingAttrs = attrIds
                        .Select(aId => attrNames.GetValueOrDefault(aId, ""))
                        .Where(n => !string.IsNullOrEmpty(n))
                        .Distinct()
                        .ToList();
                    if (varyingAttrs.Any())
                        productSchema["variesBy"] = varyingAttrs;

                    // Ana Product'taki tekil offer'ı kaldır (varyantlar kendi offer'ını taşıyor)
                    productSchema.Remove("offers");
                }

                jsonLdScripts.Add(SerializeJsonLd(productSchema));

                // ───────────────────────────────────────────────────────
                // 4d. Tüm JSON-LD'leri tek string'e birleştir
                // ───────────────────────────────────────────────────────
                var combinedJsonLd = string.Join("\n", jsonLdScripts.Select(j =>
                    $"<script type=\"application/ld+json\">\n{j}\n</script>"));


                // ═══════════════════════════════════════════════════════
                // 5. VIEWDATA'YA AKTAR
                // ═══════════════════════════════════════════════════════

                ViewData["CanonicalUrl"] = canonicalUrl;

                ViewData["Title"] = pageTitle;
                ViewData["Description"] = pageDescription;
                ViewData["Keywords"] = pageKeywords;

                ViewData["OgType"] = "og:product";
                ViewData["OgImage"] = ogImageUrl;
                ViewData["OgImageWidth"] = "1200";
                ViewData["OgImageHeight"] = "1200";

                ViewData["PreloadImage"] = preloadImageUrl;

                ViewData["JsonLd"] = combinedJsonLd;

                ViewData["Robots"] = robotsContent;

                ViewBag.ProductProfile = product;

                return View();
            }
        }

        // ═══════════════════════════════════════════════════════════════════════
        // YARDIMCI: JSON-LD Serializer
        // ═══════════════════════════════════════════════════════════════════════
        // "@" prefix'li C# property'lerini schema.org key'lerine dönüştürür.
        // Controller sınıfınıza private method olarak ekleyin.

        private static string SerializeJsonLd(object obj)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

            var json = JsonSerializer.Serialize(obj, options);

            // C# anonymous type'larda "@" kullanamadığımız için
            // property adlarındaki "type" → "@type", "context" → "@context" dönüşümü
            // Dictionary<string, object?> kullandığımızda bu otomatik çalışır.
            // Ancak anonymous type'lar için (breadcrumb gibi) düzeltme gerekir:
            json = json
                .Replace("\"type\":", "\"@type\":")
                .Replace("\"context\":", "\"@context\":");

            return json;
        }
    }
}
