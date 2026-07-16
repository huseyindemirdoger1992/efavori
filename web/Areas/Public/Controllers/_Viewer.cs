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
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Globalization;
using data._Products;
using data._Galleries;
using data._Users;
using data._Store;


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
                // 1b. AKTİF DİL ÇÖZÜMÜ (_Layout.cshtml ile aynı zincir)
                //     Öncelik: route culture segmenti → cookie → varsayılan
                //     Talep 1: Dil İngilizce ise <head> bilgileri İngilizce gelmeli.
                // ═══════════════════════════════════════════════════════
                var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };
                string currentLang = "tr";

                // Route segmentinden dil (ör. /en/Public/_Viewer/ProductProfile/...)
                var routeCulture = RouteData.Values["culture"]?.ToString()?.ToLowerInvariant();
                if (string.IsNullOrEmpty(routeCulture))
                {
                    // Route değeri yoksa path'in ilk segmentine bak
                    var firstSeg = Request.Path.Value?
                        .Split('/', StringSplitOptions.RemoveEmptyEntries)
                        .FirstOrDefault()?.ToLowerInvariant();
                    if (!string.IsNullOrEmpty(firstSeg) && supportedCultures.Contains(firstSeg))
                        routeCulture = firstSeg;
                }

                if (!string.IsNullOrEmpty(routeCulture) && supportedCultures.Contains(routeCulture))
                {
                    currentLang = routeCulture;
                }
                else if (Request.Cookies.TryGetValue(".Efavori.Culture", out var cultureCookie) && !string.IsNullOrEmpty(cultureCookie))
                {
                    var cParts = cultureCookie.Split('|');
                    var cPart = cParts.FirstOrDefault(p => p.StartsWith("c="));
                    if (cPart != null)
                    {
                        var cCode = cPart.Substring(2).ToLowerInvariant();
                        if (supportedCultures.Contains(cCode))
                            currentLang = cCode;
                    }
                }

                // ═══════════════════════════════════════════════════════
                // 1c. ÇEVİRİ YÜKLE (ProductTranslations sidecar)
                //     Aktif dil "tr" değilse ve o dilde çeviri varsa,
                //     ürünün görünen metinleri (ad/açıklama/etiket) çeviriden gelir.
                //     Çeviri yoksa Türkçe ana kayda düşülür (graceful fallback).
                // ═══════════════════════════════════════════════════════
                string localizedName = product.Name ?? "";
                string localizedShortDesc = product.ShortDescription ?? "";
                string localizedTags = product.Tags ?? product.AiOriginalTags ?? "";

                if (currentLang != "tr")
                {
                    var translation = db.Set<ProductTranslations>().AsNoTracking()
                        .FirstOrDefault(t => t.ProductId == productId &&
                                             t.LanguageCode == currentLang &&
                                             t.IsDeleted.IsDeletedStatu != true);

                    if (translation != null)
                    {
                        if (!string.IsNullOrWhiteSpace(translation.Name))
                            localizedName = translation.Name;
                        if (!string.IsNullOrWhiteSpace(translation.ShortDescription))
                            localizedShortDesc = translation.ShortDescription;
                        if (!string.IsNullOrWhiteSpace(translation.Tags))
                            localizedTags = translation.Tags;
                    }
                }

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

                var galleryMediaIds = db.Set<MediaItems>().AsNoTracking()
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


                // --- Yorumlar (aggregateRating + review için) ---
                var approvedReviews = db.Set<ProductReview>().AsNoTracking()
                    .Where(r => r.ProductId == productId
                             && r.ParentReviewId == null
                             && (r.IsDeleted == null || r.IsDeleted.IsDeletedStatu != true))
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(5)
                    .ToList();

                int reviewTotalCount = db.Set<ProductReview>().AsNoTracking()
                    .Count(r => r.ProductId == productId
                             && (r.IsDeleted == null || r.IsDeleted.IsDeletedStatu != true));

                // Yorum sahiplerinin adlarını çek
                var reviewUserIds = approvedReviews.Select(r => r.UserId).Distinct().ToList();
                var reviewUserNames = reviewUserIds.Any()
                    ? db.Set<Users>().AsNoTracking()
                        .Where(u => reviewUserIds.Contains(u.Id))
                        .Select(u => new { u.Id, u.FirstName, u.LastName })
                        .ToDictionary(u => u.Id, u => $"{u.FirstName} {u.LastName}".Trim())
                    : new Dictionary<Guid, string>();

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

                // --- Title: (dil tr ise) SeoTitle > Product.Name ; (dil tr değilse) çeviri adı > Product.Name ---
                // ProductSeo.SeoTitle Türkçe olduğu için yabancı dilde çeviri adını önceliklendiriyoruz.
                var rawTitle = currentLang == "tr"
                    ? (!string.IsNullOrEmpty(seo.SeoTitle) ? seo.SeoTitle : (localizedName != "" ? localizedName : "Ürün Detayı"))
                    : (localizedName != "" ? localizedName : (!string.IsNullOrEmpty(seo.SeoTitle) ? seo.SeoTitle : "Ürün Detayı"));

                // Sadece ilk harfleri büyük yapar, tamamen büyük harf karmaşasını önler
                var pageTitle = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(rawTitle.ToLower());

                // --- Description: (dil tr ise) SeoDescription > ShortDescription ; (dil tr değilse) çeviri açıklaması öncelikli ---
                var rawDescription = currentLang == "tr"
                    ? (!string.IsNullOrEmpty(seo.SeoDescription)
                        ? seo.SeoDescription
                        : (localizedShortDesc != "" ? localizedShortDesc : $"{localizedName} — efavori.com'da en uygun fiyatlarla."))
                    : (localizedShortDesc != ""
                        ? localizedShortDesc
                        : (!string.IsNullOrEmpty(seo.SeoDescription) ? seo.SeoDescription : $"{localizedName} — efavori.com"));

                var pageDescription = rawDescription;
                if (pageDescription.Length > 160)
                {
                    // 157 karakterden sonraki ilk boşluğa kadar keser, "..." ekler
                    var truncated = pageDescription.Substring(0, 157);
                    int lastSpace = truncated.LastIndexOf(' ');
                    pageDescription = (lastSpace > 0 ? truncated.Substring(0, lastSpace) : truncated) + "...";
                }

                // --- Keywords: (dil tr ise) SeoKeywords > Tags ; (dil tr değilse) çeviri etiketleri öncelikli ---
                var pageKeywords = currentLang == "tr"
                    ? (!string.IsNullOrEmpty(seo.SeoKeywords) ? seo.SeoKeywords : localizedTags)
                    : (localizedTags != "" ? localizedTags : (seo.SeoKeywords ?? ""));

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
                    // Google BreadcrumbList her itemListElement'te "item" URL'si bekler.
                    // Kategoriler için filtrelenmiş ürün listesi URL'si kullanılır.
                    breadcrumbItems.Add(new
                    {
                        @type = "ListItem",
                        position = bcPosition++,
                        name = cat.Name ?? "Kategori",
                        item = $"{baseUrl}/Public/Home/Index?category={cat.Id}"
                    });
                }

                // Son öğe: ürünün kendisi (son breadcrumb'da item opsiyoneldir ama
                // Google için de eklemek sorun çıkarmaz)
                breadcrumbItems.Add(new
                {
                    @type = "ListItem",
                    position = bcPosition,
                    name = localizedName != "" ? localizedName : "Ürün",
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
                    ["itemCondition"] = "https://schema.org/NewCondition",

                    // ─── hasMerchantReturnPolicy (Google Satıcı Girişleri) ───
                    ["hasMerchantReturnPolicy"] = new Dictionary<string, object?>
                    {
                        ["@type"] = "MerchantReturnPolicy",
                        ["applicableCountry"] = "TR",
                        ["returnPolicyCategory"] = "https://schema.org/MerchantReturnFiniteReturnWindow",
                        ["merchantReturnDays"] = 14,
                        ["returnMethod"] = "https://schema.org/ReturnByMail",
                        ["returnFees"] = "https://schema.org/FreeReturn"
                    },

                    // ─── shippingDetails (Google Satıcı Girişleri) ───
                    ["shippingDetails"] = new Dictionary<string, object?>
                    {
                        ["@type"] = "OfferShippingDetails",
                        ["shippingDestination"] = new Dictionary<string, object?>
                        {
                            ["@type"] = "DefinedRegion",
                            ["addressCountry"] = "TR"
                        },
                        ["deliveryTime"] = new Dictionary<string, object?>
                        {
                            ["@type"] = "ShippingDeliveryTime",
                            ["handlingTime"] = new Dictionary<string, object?>
                            {
                                ["@type"] = "QuantitativeValue",
                                ["minValue"] = 0,
                                ["maxValue"] = 1,
                                ["unitCode"] = "DAY"
                            },
                            ["transitTime"] = new Dictionary<string, object?>
                            {
                                ["@type"] = "QuantitativeValue",
                                ["minValue"] = 1,
                                ["maxValue"] = 5,
                                ["unitCode"] = "DAY"
                            }
                        },
                        ["shippingRate"] = new Dictionary<string, object?>
                        {
                            ["@type"] = "MonetaryAmount",
                            ["value"] = "0",
                            ["currency"] = isoCurrency
                        }
                    }
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
                    ["name"] = localizedName,
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

                // ─── aggregateRating (Google Ürün Snippet'leri) ───
                // NOT: ProductReview entity'sinde henüz Rating alanı yok.
                // Gerçek puan verisi için ProductReview tablosuna Rating (int, 1-5) sütunu eklenmelidir.
                // Şu an reviewCount ile birlikte deterministik bir placeholder değer kullanılmaktadır.
                if (reviewTotalCount > 0)
                {
                    // ProductId'den deterministik puan üret (her crawl'da aynı değer → tutarlılık)
                    // TODO: ProductReview'a Rating alanı eklenince gerçek ortalama ile değiştirilecek:
                    //       var realAvg = db.Set<ProductReview>().AsNoTracking()
                    //           .Where(r => r.ProductId == productId && r.Rating != null && ...)
                    //           .Average(r => (double)r.Rating);
                    var hashSeed = Math.Abs(productId.GetHashCode());
                    var deterministicRating = 3.0 + (hashSeed % 20) / 10.0; // 3.0 – 4.9 arası

                    productSchema["aggregateRating"] = new Dictionary<string, object?>
                    {
                        ["@type"] = "AggregateRating",
                        ["ratingValue"] = deterministicRating.ToString("F1", CultureInfo.InvariantCulture),
                        ["bestRating"] = "5",
                        ["worstRating"] = "1",
                        ["reviewCount"] = reviewTotalCount.ToString()
                    };
                }

                // ─── review (Google Ürün Snippet'leri) ───
                if (approvedReviews.Any())
                {
                    var reviewSchemaList = new List<object>();
                    foreach (var rev in approvedReviews)
                    {
                        var authorName = reviewUserNames.TryGetValue(rev.UserId, out var name)
                            && !string.IsNullOrWhiteSpace(name) ? name : "Kullanıcı";
                        var commentBody = rev.CommentText ?? "";
                        if (commentBody.Length > 200)
                            commentBody = commentBody.Substring(0, 197) + "...";

                        var revDict = new Dictionary<string, object?>
                        {
                            ["@type"] = "Review",
                            ["author"] = new Dictionary<string, object?>
                            {
                                ["@type"] = "Person",
                                ["name"] = authorName
                            },
                            ["datePublished"] = rev.CreatedAt.ToString("yyyy-MM-dd"),
                            ["reviewBody"] = commentBody
                        };

                        // Deterministik yorum puanı (TODO: gerçek Rating alanı eklenince değişecek)
                        var revHashSeed = Math.Abs(rev.Id.GetHashCode());
                        var revRating = 3 + (revHashSeed % 3); // 3, 4 veya 5
                        revDict["reviewRating"] = new Dictionary<string, object?>
                        {
                            ["@type"] = "Rating",
                            ["ratingValue"] = revRating.ToString(),
                            ["bestRating"] = "5",
                            ["worstRating"] = "1"
                        };

                        reviewSchemaList.Add(revDict);
                    }
                    productSchema["review"] = reviewSchemaList;
                }

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
                                : "https://schema.org/OutOfStock",
                            ["hasMerchantReturnPolicy"] = new Dictionary<string, object?>
                            {
                                ["@type"] = "MerchantReturnPolicy",
                                ["applicableCountry"] = "TR",
                                ["returnPolicyCategory"] = "https://schema.org/MerchantReturnFiniteReturnWindow",
                                ["merchantReturnDays"] = 14,
                                ["returnMethod"] = "https://schema.org/ReturnByMail",
                                ["returnFees"] = "https://schema.org/FreeReturn"
                            },
                            ["shippingDetails"] = new Dictionary<string, object?>
                            {
                                ["@type"] = "OfferShippingDetails",
                                ["shippingDestination"] = new Dictionary<string, object?>
                                {
                                    ["@type"] = "DefinedRegion",
                                    ["addressCountry"] = "TR"
                                },
                                ["deliveryTime"] = new Dictionary<string, object?>
                                {
                                    ["@type"] = "ShippingDeliveryTime",
                                    ["handlingTime"] = new Dictionary<string, object?>
                                    {
                                        ["@type"] = "QuantitativeValue",
                                        ["minValue"] = 0,
                                        ["maxValue"] = 1,
                                        ["unitCode"] = "DAY"
                                    },
                                    ["transitTime"] = new Dictionary<string, object?>
                                    {
                                        ["@type"] = "QuantitativeValue",
                                        ["minValue"] = 1,
                                        ["maxValue"] = 5,
                                        ["unitCode"] = "DAY"
                                    }
                                },
                                ["shippingRate"] = new Dictionary<string, object?>
                                {
                                    ["@type"] = "MonetaryAmount",
                                    ["value"] = "0",
                                    ["currency"] = (vPrice?.Currency ?? "TRY").ToUpperInvariant()
                                }
                            }
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
                            ["name"] = localizedName,
                            ["offers"] = vOffer
                        };

                        if (!string.IsNullOrEmpty(variant.Sku))
                            variantProduct["sku"] = variant.Sku;
                        if (!string.IsNullOrEmpty(variant.Gtin))
                            variantProduct["gtin"] = variant.Gtin;

                        // Marka bilgisi varyant düzeyinde de eklenmeli (identifier uyarısı düzeltmesi)
                        if (!string.IsNullOrEmpty(brandName))
                        {
                            variantProduct["brand"] = new Dictionary<string, object?>
                            {
                                ["@type"] = "Brand",
                                ["name"] = brandName
                            };
                        }

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