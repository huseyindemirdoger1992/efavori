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
    public class GetXmlTr : Controller
    {
        private readonly _ApplicationConnectionDb _context;

        // Güvenli bölge (10.000 - 25.000)
        private const int MaxUrlsPerSitemap = 10000;

        public GetXmlTr(_ApplicationConnectionDb context)
        {
            _context = context;
        }

        /// <summary>
        /// Arama motorlarına gönderilecek TR sitemap dizini (Sitemap Index)
        /// </summary>
        [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any)] // 1 Gün Cache
        public async Task SitemapIndex()
        {
            var totalUsers = await _context.Users
                .AsNoTracking()
                .Where(u => u.IsActive == true)
                .CountAsync();

            var totalPages = (int)Math.Ceiling((double)totalUsers / MaxUrlsPerSitemap);

            Response.ContentType = "application/xml; charset=utf-8";

            var settings = new XmlWriterSettings { Async = true, Encoding = Encoding.UTF8, Indent = true };

            await using var writer = XmlWriter.Create(Response.Body, settings);

            await writer.WriteStartDocumentAsync();
            await writer.WriteStartElementAsync(null, "sitemapindex", "http://www.sitemaps.org/schemas/sitemap/0.9");

            var baseUrl = $"{Request.Scheme}://{Request.Host}/tr/Sitemap/GetXmlTr";

            for (int i = 1; i <= totalPages; i++)
            {
                await writer.WriteStartElementAsync(null, "sitemap", null);

                await writer.WriteElementStringAsync(null, "loc", null, $"{baseUrl}/UsersSitemap/{i}");

                await writer.WriteElementStringAsync(null, "lastmod", null, DateTime.UtcNow.ToString("yyyy-MM-dd"));

                await writer.WriteEndElementAsync(); // </sitemap>
            }

            await writer.WriteEndElementAsync(); // </sitemapindex>
            await writer.WriteEndDocumentAsync();
        }

        /// <summary>
        /// Sayfalanmış alt sitemap dosyaları (TR kullanıcılar için)
        /// </summary>
        [ResponseCache(Duration = 43200, Location = ResponseCacheLocation.Any)] // 12 Saat Cache
        public async Task UsersSitemap(int id = 1)
        {
            if (id < 1) id = 1;

            var query = _context.Users
                .AsNoTracking()
                .Where(u => u.IsActive == true)
                .OrderBy(u => u.RegistrationDate)
                .Skip((id - 1) * MaxUrlsPerSitemap)
                .Take(MaxUrlsPerSitemap)
                .Select(u => new
                {
                    u.Id,
                    u.RegistrationDate
                });

            var usersChunk = await query.ToListAsync();

            Response.ContentType = "application/xml; charset=utf-8";

            var settings = new XmlWriterSettings { Async = true, Encoding = Encoding.UTF8, Indent = true };

            await using var writer = XmlWriter.Create(Response.Body, settings);

            await writer.WriteStartDocumentAsync();
            await writer.WriteStartElementAsync(null, "urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

            // Nihai sayfa linkleri için "tr" kültürünü manuel ekliyoruz
            var baseUrl = $"{Request.Scheme}://{Request.Host}/tr";

            foreach (var user in usersChunk)
            {
                await writer.WriteStartElementAsync(null, "url", null);

                // Örn: https://site.com/tr/user/profile/5
                await writer.WriteElementStringAsync(null, "loc", null, $"{baseUrl}/Public/_ProfileViewer/UserProfile/{user.Id}");

                if (user.RegistrationDate.HasValue)
                {
                    await writer.WriteElementStringAsync(null, "lastmod", null, user.RegistrationDate.Value.ToString("yyyy-MM-dd"));
                }

                await writer.WriteElementStringAsync(null, "changefreq", null, "weekly");
                await writer.WriteElementStringAsync(null, "priority", null, "0.6");

                await writer.WriteEndElementAsync(); // </url>
            }

            await writer.WriteEndElementAsync(); // </urlset>
            await writer.WriteEndDocumentAsync();
        }
    }
}