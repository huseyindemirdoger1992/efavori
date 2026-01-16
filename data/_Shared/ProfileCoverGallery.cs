using Microsoft.EntityFrameworkCore;

namespace data._Shared
{
    [Owned] // Bu nitelik, EF Core'a bu sınıfın başka bir tabloya ait olduğunu söyler.
    public class ProfileCoverGallery
    {
        public string? ProfileImagePath { get; set; }
        public string? CoverImagePath { get; set; }
    }
}
