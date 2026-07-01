using data._Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace data.Articles
{
    // Liste sayfasında CreatedAt + Id üzerinden keyset (seek) pagination yapılacağı için
    // bu ikili üzerinde composite index ZORUNLUDUR — aksi halde milyonlarca kayıtta sorgu yavaşlar.
    [Index(nameof(CreatedAt), nameof(Id))]
    [Index(nameof(Slug))]
    public class Article
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? IsUser { get; set; } // bu alan eğer boş ise yapay zeka ile oluşturulmuş bir içerik olduğunu gösterir. Eğer dolu ise kullanıcı tarafından oluşturulmuş bir içerik olduğunu gösterir.
        public Guid? UserStoreId { get; set; } // bu alan eğer dolu ise kullanıcının hangi mağazaya ait olduğunu gösterir. Eğer boş ise ya yapay zeka ile oluşturulmuştur yada direk kullanıcı tarafından oluşturulmuştur.

        public int? CategoriId { get; set; } // bu alan eğer dolu ise makalenin hangi kategoriye ait olduğunu gösterir. Eğer boş ise makale kategorisizdir.
        public Guid? FeaturedImage { get; set; } // bu alan eğer dolu ise makalenin öne çıkan görselini gösterir. Eğer boş ise makalenin öne çıkan görseli yoktur.
        public string? Title { get; set; } // bu alan makalenin başlığını gösterir.
        public string? ShotDescription { get; set; } // bu alan makalenin kısa açıklamasını gösterir.
        public string? ArticleLognDescription { get; set; } // bu alan içeriği ifade eder. İçerik burada html kodları ile de oluşturulmuş olabilir düz metin olarak da oluşturulmuş olabilir.

        // SEO dostu, temiz URL için slug (örn: "duzce-boya-badana-ustasi").
        // Boş olan eski kayıtlarda controller/route katmanı otomatik olarak Id'ye düşer (fallback).
        public string? Slug { get; set; }

        // Yapısal veri (JSON-LD datePublished/dateModified) ve liste sıralaması/keyset pagination için zorunlu.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Bu makale AiTitlesForArticle kuyruğundan otomatik üretildiyse kaynak kaydın Id'sini tutar.
        public Guid? SourceAiTitleId { get; set; }

        public Meta? Meta { get; set; } = new();
        public InteractionCounts? Interaction { get; set; } = new();
        public IsDeleted? IsDeleted { get; set; } = new();
    }
}