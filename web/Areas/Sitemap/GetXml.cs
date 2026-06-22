using data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace web.Areas.Sitemap
{
    [Area("Sitemap")]
    [Route("/Sitemap/[controller]/[action]")]
    public class GetXml : Controller
    {
        private readonly _ApplicationConnectionDb _context;

        // Güvenli bölge (Google max 50.000 kabul eder, 10.000 idealdir)
        private const int MaxUrlsPerSitemap = 10000;

        public GetXml(_ApplicationConnectionDb context)
        {
            _context = context;
        }

        /// <summary>
        /// Arama motorlarına gönderilecek TR sitemap dizini (Sitemap Index)
        /// URL: site.com/sitemap-tr-index.xml
        /// </summary>
        [Route("users-group-sitemap-index.xml")]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)] // 1 Saat Cache
        public async Task<IActionResult> UsersSitemapGroup()
        {
            var totalUsers = await _context.Users
                .AsNoTracking()
                .Where(u => u.IsActive == true)
                .CountAsync();

            if (totalUsers == 0) return NotFound();

            var totalPages = (int)Math.Ceiling((double)totalUsers / MaxUrlsPerSitemap);

            Response.ContentType = "application/xml; charset=utf-8";
            var settings = new XmlWriterSettings { Async = true, Encoding = Encoding.UTF8, Indent = true };

            await using var writer = XmlWriter.Create(Response.Body, settings);
            await writer.WriteStartDocumentAsync();
            await writer.WriteStartElementAsync(null, "sitemapindex", "http://www.sitemaps.org/schemas/sitemap/0.9");

            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            for (int i = 1; i <= totalPages; i++)
            {
                await writer.WriteStartElementAsync(null, "sitemap", null);

                // SEO Dostu .xml uzantılı sahte statik link
                await writer.WriteElementStringAsync(null, "loc", null, $"{baseUrl}/Sitemap/GetXml/UsersSitemap/sitemap-users-{i}.xml");
                // Not: Index dosyasında lastmod zorunlu değildir ve sahte UtcNow vermek botları yorar, bu yüzden kaldırıldı.

                await writer.WriteEndElementAsync(); // </sitemap>
            }

            await writer.WriteEndElementAsync(); // </sitemapindex>
            await writer.WriteEndDocumentAsync();

            return new EmptyResult();
        }

        /// <summary>
        /// Sayfalanmış alt sitemap dosyaları 
        /// URL: site.com/sitemap-tr-users-1.xml
        /// </summary>
        [Route("sitemap-users-{id:int}.xml")]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)] // 1 Saat Cache
        public async Task<IActionResult> UsersSitemap(int id = 1)
        {
            if (id < 1) id = 1;

            // Performans: OrderBy(Id) Clustered Index kullandığı için OrderBy(RegistrationDate)'e göre kat kat hızlıdır.
            var query = _context.Users
                .AsNoTracking()
                .Where(u => u.IsActive == true)
                .OrderBy(u => u.Id)
                .Skip((id - 1) * MaxUrlsPerSitemap)
                .Take(MaxUrlsPerSitemap)
                .Select(u => new
                {
                    u.Id,
                    u.RegistrationDate
                });

            var usersChunk = await query.ToListAsync();

            // Eğer sayfa boşsa XML oluşturma, 404 dön (Botları boş sayfalarda dolaştırma)
            if (!usersChunk.Any()) return NotFound();

            Response.ContentType = "application/xml; charset=utf-8";
            var settings = new XmlWriterSettings { Async = true, Encoding = Encoding.UTF8, Indent = true };

            await using var writer = XmlWriter.Create(Response.Body, settings);
            await writer.WriteStartDocumentAsync();
            await writer.WriteStartElementAsync(null, "urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            foreach (var user in usersChunk)
            {
                await writer.WriteStartElementAsync(null, "url", null);

                // Gerçek public profil URL'si
                await writer.WriteElementStringAsync(null, "loc", null, $"{baseUrl}/Public/_Viewer/_ProfileViewer/{user.Id}");

                if (user.RegistrationDate.HasValue)
                {
                    // Sadece W3C formatı (yyyy-MM-dd) Google için yeterli ve temizdir.
                    await writer.WriteElementStringAsync(null, "lastmod", null, user.RegistrationDate.Value.ToString("yyyy-MM-dd"));
                }

                // Crawl Budget'ı korumak adına priority ve changefreq kaldırıldı (Google botları zaten kendi karar veriyor).

                await writer.WriteEndElementAsync(); // </url>
            }

            await writer.WriteEndElementAsync(); // </urlset>
            await writer.WriteEndDocumentAsync();

            return new EmptyResult();
        }

        //-----------------------------------------------------------------------------------//

        /// <summary>
        /// Arama motorlarına gönderilecek TR sitemap dizini (Sitemap Index)
        /// URL: site.com/sitemap-tr-index.xml
        /// </summary>
        [Route("products-group-sitemap-index.xml")]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)] // 1 Saat Cache
        public async Task<IActionResult> ProductsSitemapGroup()
        {
            var totalProducts = await _context.Products
                .AsNoTracking()
                .Where(p => p.IsActive == true)
                .CountAsync();

            if (totalProducts == 0) return NotFound();

            var totalPages = (int)Math.Ceiling((double)totalProducts / MaxUrlsPerSitemap);

            Response.ContentType = "application/xml; charset=utf-8";
            var settings = new XmlWriterSettings { Async = true, Encoding = Encoding.UTF8, Indent = true };

            await using var writer = XmlWriter.Create(Response.Body, settings);
            await writer.WriteStartDocumentAsync();
            await writer.WriteStartElementAsync(null, "sitemapindex", "http://www.sitemaps.org/schemas/sitemap/0.9");

            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            for (int i = 1; i <= totalPages; i++)
            {
                await writer.WriteStartElementAsync(null, "sitemap", null);

                // SEO Dostu .xml uzantılı sahte statik link
                await writer.WriteElementStringAsync(null, "loc", null, $"{baseUrl}/Sitemap/GetXml/ProductsSitemap/sitemap-products-{i}.xml");
                // Not: Index dosyasında lastmod zorunlu değildir ve sahte UtcNow vermek botları yorar, bu yüzden kaldırıldı.

                await writer.WriteEndElementAsync(); // </sitemap>
            }

            await writer.WriteEndElementAsync(); // </sitemapindex>
            await writer.WriteEndDocumentAsync();

            return new EmptyResult();
        }


        /// <summary>
        /// Sayfalanmış alt sitemap dosyaları 
        /// URL: site.com/sitemap-tr-users-1.xml
        /// </summary>
        [Route("sitemap-products-{id:int}.xml")]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)] // 1 Saat Cache
        public async Task<IActionResult> ProductsSitemap(int id = 1)
        {
            if (id < 1) id = 1;

            // 1. ADIM: Önce ana ürünleri sayfala (Performans için Skip/Take önce yapılır)
            var pagedProducts = _context.Products
                .AsNoTracking()
                .Where(p => p.IsActive == true)
                .OrderBy(p => p.Id)
                .Skip((id - 1) * MaxUrlsPerSitemap)
                .Take(MaxUrlsPerSitemap);

            // 2. ADIM: Sadece sayfalanmış ürünler için SEO tablosuna Left Join at ve Slug'ı çek
            var query = from p in pagedProducts
                        join s in _context.ProductSeo.AsNoTracking() on p.Id equals s.ProductId into seoGroup
                        from seo in seoGroup.DefaultIfEmpty()
                        select new
                        {
                            p.Id,
                            p.CreatedAt,
                            Slug = seo != null ? seo.Slug : null // Slug verisini alıyoruz
                        };

            var productsChunk = await query.ToListAsync();

            // Eğer sayfa boşsa XML oluşturma, 404 dön (Botları boş sayfalarda dolaştırma)
            if (!productsChunk.Any()) return NotFound();

            Response.ContentType = "application/xml; charset=utf-8";
            var settings = new XmlWriterSettings { Async = true, Encoding = Encoding.UTF8, Indent = true };

            await using var writer = XmlWriter.Create(Response.Body, settings);
            await writer.WriteStartDocumentAsync();
            await writer.WriteStartElementAsync(null, "urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            foreach (var product in productsChunk)
            {
                await writer.WriteStartElementAsync(null, "url", null);

                // İŞTE DEĞİŞTİRMEK İSTEDİĞİNİZ O SATIR:
                // Slug varsa Slug'ı, yoksa fallback olarak Id'yi kullanır
                var identifier = !string.IsNullOrWhiteSpace(product.Slug) ? product.Slug : product.Id.ToString();
                await writer.WriteElementStringAsync(null, "loc", null, $"{baseUrl}/Public/_Viewer/ProductProfile/{identifier}");

                if (product.CreatedAt.HasValue)
                {
                    // Sadece W3C formatı (yyyy-MM-dd) Google için yeterli ve temizdir.
                    await writer.WriteElementStringAsync(null, "lastmod", null, product.CreatedAt.Value.ToString("yyyy-MM-dd"));
                }

                // Crawl Budget'ı korumak adına priority ve changefreq kaldırıldı (Google botları zaten kendi karar veriyor).

                await writer.WriteEndElementAsync(); // </url>
            }

            await writer.WriteEndElementAsync(); // </urlset>
            await writer.WriteEndDocumentAsync();

            return new EmptyResult();
        }
    }
}