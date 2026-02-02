using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Data;
using data;
namespace web._files.xml
{
    public class SitemapController : Controller
    {
        private readonly _ApplicationConnectionDb _context;
        private const int PageSize = 1000; // Her sayfada max 1000 kayıt

        public SitemapController(_ApplicationConnectionDb context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            if (page < 1) page = 1;

            var query = _context.Media
                .Where(m => m.IsDeletedStatu != true && !string.IsNullOrEmpty(m.FileUrl));

            var totalCount = await query.CountAsync();
            var mediaItems = await query
                .OrderByDescending(m => m.CreatedAt)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            var sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            // Görsel etiketleri için xmlns:image şemasını ekliyoruz
            sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\" xmlns:image=\"http://www.google.com/schemas/sitemap-image/1.1\">");

            foreach (var media in mediaItems)
            {
                // Medyanın bulunduğu sayfa (Eğer bir detay sayfanız varsa onu, yoksa dosya URL'ini yazın)
                // SEO kuralları gereği <loc> sayfa adresi, <image:loc> dosya adresi olmalıdır.
                string pageUrl = $"{Request.Scheme}://{Request.Host}/media/view/{media.Id}";

                sb.AppendLine("  <url>");
                sb.AppendLine($"    <loc>{pageUrl}</loc>");

                if (media.CreatedAt.HasValue)
                {
                    // Kesin yyyy-MM-dd formatı
                    sb.AppendLine($"    <lastmod>{media.CreatedAt.Value.ToString("yyyy-MM-dd")}</lastmod>");
                }

                sb.AppendLine("    <image:image>");
                // Resmin doğrudan URL adresi
                sb.AppendLine($"      <image:loc>{media.FileUrl}</image:loc>");
                if (!string.IsNullOrEmpty(media.FileName))
                {
                    // Dosya adını başlık olarak ekliyoruz
                    sb.AppendLine($"      <image:title><![CDATA[{media.FileName}]]></image:title>");
                }
                sb.AppendLine("    </image:image>");

                sb.AppendLine("    <changefreq>weekly</changefreq>");
                sb.AppendLine("    <priority>0.6</priority>");
                sb.AppendLine("  </url>");
            }

            sb.AppendLine("</urlset>");

            return Content(sb.ToString(), "text/xml", Encoding.UTF8);
        }
    }
}
