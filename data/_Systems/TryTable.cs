using data.Owned;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Systems
{
    /// <summary>
    /// Demo / test tablosu — CRUD sayfa şablonu için kullanılır.
    ///
    /// GÜNCELLENDİ: Eski <c>ProfileCoverGallery</c> owned tipi kaldırıldı (§72).
    /// Profil ve kapak görselleri artık merkezî medya deposuna FK ile bağlanır
    /// (data._Galleries.Media.Id). Fiziksel dosya yolu/URL bu tabloda TUTULMAZ.
    /// </summary>
    public class TryTable
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }

        public string? Country { get; set; }
        public string? State { get; set; }
        public string? Cities { get; set; }

        public bool? CheckSliding { get; set; }
        public bool? CheckMarked { get; set; }
        public string? RadioButton { get; set; }

        public DateTime SelectDateTime { get; set; }
        public DateTime GetDateTime { get; set; }

        // ── Medya referansları (§72 — FK only, URL tutulmaz) ──────────────────
        /// <summary>Profil görseli (data._Galleries.Media.Id).</summary>
        public Guid? ProfileMediaId { get; set; }

        /// <summary>Kapak görseli (data._Galleries.Media.Id).</summary>
        public Guid? CoverMediaId { get; set; }

        public IsDeleted? IsDeleted { get; set; }
    }
}