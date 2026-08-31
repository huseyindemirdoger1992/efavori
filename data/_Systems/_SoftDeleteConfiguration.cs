using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using data.Owned;

namespace data._Systems
{
    /// <summary>
    /// GLOBAL SOFT-DELETE QUERY FILTER'LARI (§40) — İSTEĞE BAĞLI (opt-in) altyapı.
    ///
    /// NE YAPAR:
    /// <c>data.Owned.IsDeleted</c> owned tipine sahip her entity için
    /// <c>HasQueryFilter(e =&gt; e.IsDeleted.IsDeletedStatu != true)</c> uygular.
    /// Böylece <c>context.Posts</c> gibi kök sorgular silinmiş satırları otomatik eler
    /// ve her sorguya elle <c>Where</c> yazma zorunluluğu ortadan kalkar.
    ///
    /// NEDEN VARSAYILAN OLARAK KAPALI:
    ///  1) Mevcut mimari, tekilliği FİLTRELİ TEKİL İNDEKSLERLE sağlar ve sorgularda
    ///     soft-delete koşulunu açıkça yazar. Filtreleri bir anda açmak, hâlihazırda
    ///     doğru çalışan sorguların davranışını sessizce değiştirir.
    ///  2) Filtre YALNIZCA kök sorgulara uygulanır. Bu projede navigation property
    ///     bulunmadığı ve JOIN'ler elle yazıldığı için, elle yazılmış JOIN'lerde
    ///     filtre DEVREYE GİRMEZ — yani filtreleri açmak "artık hiç düşünmeme gerek
    ///     yok" anlamına GELMEZ.
    ///  3) Rapor, denetim ve yönetim ekranlarının silinmiş kayıtları görmesi gerekir;
    ///     bu ekranların her sorguda <c>IgnoreQueryFilters()</c> çağırması unutulursa
    ///     eksik veri sessizce raporlanır.
    ///  4) İlişkinin bir ucunda filtre varken diğerinde yoksa EF Core uyarı üretir.
    ///
    /// NASIL AÇILIR:
    /// <see cref="Enabled"/> sabitini <c>true</c> yapmak yeterlidir. Sabit
    /// <c>const</c> olduğu için değişiklik derleme zamanındadır ve EF Core'un model
    /// önbelleğiyle çakışmaz. Açtıktan sonra yönetim/rapor sorgularında
    /// <c>IgnoreQueryFilters()</c> kullanmayı unutmayın.
    /// </summary>
    public static class _SoftDeleteConfiguration
    {
        /// <summary>
        /// Global soft-delete filtreleri etkin mi? Varsayılan: false.
        /// Derleme zamanı sabitidir; açmak için bu değeri true yapın.
        /// </summary>
        public const bool Enabled = false;

        /// <summary>
        /// <c>data.Owned.IsDeleted</c> owned tipine sahip tüm entity'lere global
        /// soft-delete query filter'ı uygular. <see cref="Enabled"/> false ise
        /// hiçbir şey yapmaz.
        ///
        /// TÜM DİĞER YAPILANDIRMALARDAN SONRA çağrılmalıdır; aksi hâlde sonradan
        /// eklenen entity'ler filtresiz kalır.
        /// </summary>
        public static void Apply(ModelBuilder modelBuilder)
        {
            if (!Enabled)
            {
                return;
            }

            foreach (var entityType in modelBuilder.Model.GetEntityTypes().ToList())
            {
                if (entityType.IsOwned())
                {
                    continue;
                }

                var clrType = entityType.ClrType;
                if (clrType is null)
                {
                    continue;
                }

                PropertyInfo? isDeletedProperty = clrType.GetProperty(
                    "IsDeleted",
                    BindingFlags.Public | BindingFlags.Instance);

                if (isDeletedProperty is null || isDeletedProperty.PropertyType != typeof(IsDeleted))
                {
                    continue;
                }

                // e => e.IsDeleted.IsDeletedStatu != true
                var parameter = Expression.Parameter(clrType, "e");
                var isDeleted = Expression.Property(parameter, isDeletedProperty);
                var statu = Expression.Property(isDeleted, nameof(IsDeleted.IsDeletedStatu));
                var body = Expression.NotEqual(statu, Expression.Constant(true, typeof(bool?)));
                var lambda = Expression.Lambda(body, parameter);

                modelBuilder.Entity(clrType).HasQueryFilter(lambda);
            }
        }
    }
}
