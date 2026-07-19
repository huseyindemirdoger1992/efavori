using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using data.Owned; // ortak soft-delete owned tipi (data.Owned.IsDeleted)

namespace data._Attribute
{
    /// <summary>
    /// Attribute kataloğu (V3) için EF Core Fluent yapılandırması. Mevcut
    /// _ProductModelConfiguration desenini izler. DbContext.OnModelCreating içinden
    /// tek satırla çağrılır:
    ///
    ///     protected override void OnModelCreating(ModelBuilder modelBuilder)
    ///     {
    ///         base.OnModelCreating(modelBuilder);
    ///         _AttributeModelConfiguration.Apply(modelBuilder);   // ← bu satır
    ///     }
    ///
    /// Konvansiyon: navigation property YOK; ilişkiler HasOne(typeof(X)).WithMany()
    /// .HasForeignKey("...") ile yalnızca FK üzerinden kurulur.
    /// </summary>
    public static class _AttributeModelConfiguration
    {
        /// <summary>
        /// Soft-delete filtresi. Benzersiz indekslerin, soft-delete edilmiş kayıtlarla
        /// çakışmaması için kullanılır (aynı kanonik kod tekrar oluşturulabilsin).
        /// data.Owned.IsDeleted → IsDeletedStatu alanı owned olarak "IsDeleted_IsDeletedStatu"
        /// kolonuna map edilir. EF owned kolonlarını farklı adlandırdıysan bu tek satırı güncelle.
        /// </summary>
        private const string SoftDeleteFilter = "[IsDeleted_IsDeletedStatu] = 0";

        /// <summary>Tüm attribute-kataloğu yapılandırmasını uygular.</summary>
        public static void Apply(ModelBuilder modelBuilder)
        {
            // Önce entity'ler modele kaydedilir (Entity<T> çağrıları) ...
            ApplyGroups(modelBuilder);
            ApplyDefinitions(modelBuilder);
            ApplyOptions(modelBuilder);
            ApplyUnits(modelBuilder);
            ApplyTemplates(modelBuilder);
            ApplyCategoryLinks(modelBuilder);
            ApplyRules(modelBuilder);
            ApplyAi(modelBuilder);
            ApplyIntegration(modelBuilder);

            // ... sonra ortak konvansiyonlar (owned IsDeleted/Ai, RowVersion) tüm
            // kayıtlı entity'lere tek yerden uygulanır. Sıra ÖNEMLİDİR: bu döngü
            // GetEntityTypes() üzerinde çalıştığı için entity'lerin önce kaydı gerekir.
            ApplyBaseConventions(modelBuilder);
        }

        // ── Ortak konvansiyonlar: owned tipler, rowversion, Ai owned ──────────
        private static void ApplyBaseConventions(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes().ToList())
            {
                var clr = entityType.ClrType;
                if (!typeof(AttributeEntityBase).IsAssignableFrom(clr))
                    continue;

                var b = modelBuilder.Entity(clr);

                // Soft-delete owned tipi (mevcut ortak IsDeleted tipi)
                b.OwnsOne(typeof(IsDeleted), nameof(AttributeEntityBase.IsDeleted));

                // İyimser eşzamanlılık damgası
                b.Property(nameof(AttributeEntityBase.RowVersion)).IsRowVersion();

                // AI izlenebilirlik owned tipi (yalnızca Ai property'si olan entity'lerde)
                var aiProp = clr.GetProperty("Ai");
                if (aiProp != null && aiProp.PropertyType == typeof(AiMetadata))
                {
                    b.OwnsOne(typeof(AiMetadata), "Ai", a =>
                    {
                        a.Property(nameof(AiMetadata.Confidence)).HasPrecision(5, 4);
                        a.Property(nameof(AiMetadata.AiModel)).HasMaxLength(128);
                        a.Property(nameof(AiMetadata.AiModelVersion)).HasMaxLength(64);
                        a.Property(nameof(AiMetadata.PromptHash)).HasMaxLength(128);
                        a.Property(nameof(AiMetadata.PromptVersion)).HasMaxLength(64);
                        a.Property(nameof(AiMetadata.ReviewNote)).HasMaxLength(2048);
                        a.Property(nameof(AiMetadata.CategorySource)).HasMaxLength(1024);
                    });
                }
            }
        }

        // ── AttributeGroup ────────────────────────────────────────────────────
        private static void ApplyGroups(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AttributeGroup>(b =>
            {
                b.Property(x => x.CanonicalCode).HasMaxLength(256).IsRequired();
                b.Property(x => x.CanonicalName).HasMaxLength(512).IsRequired();
                b.Property(x => x.IconCssClass).HasMaxLength(128);
                b.HasIndex(x => x.CanonicalCode).IsUnique().HasFilter(SoftDeleteFilter);
            });

            modelBuilder.Entity<AttributeGroupTranslation>(b =>
            {
                b.Property(x => x.Name).HasMaxLength(512).IsRequired();
                b.HasIndex(x => new { x.AttributeGroupId, x.Language }).IsUnique().HasFilter(SoftDeleteFilter);
                b.HasOne(typeof(AttributeGroup)).WithMany()
                    .HasForeignKey(nameof(AttributeGroupTranslation.AttributeGroupId))
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        // ── AttributeDefinition + çeviri/alias/synonym ─────────────────────────
        private static void ApplyDefinitions(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AttributeDefinition>(b =>
            {
                b.Property(x => x.CanonicalCode).HasMaxLength(256).IsRequired();
                b.Property(x => x.NormalizedName).HasMaxLength(256).IsRequired();
                b.Property(x => x.CanonicalName).HasMaxLength(512).IsRequired();
                b.Property(x => x.RegexPattern).HasMaxLength(1024);
                b.Property(x => x.DefaultValue).HasMaxLength(1024);
                b.Property(x => x.MinNumericValue).HasPrecision(38, 10);
                b.Property(x => x.MaxNumericValue).HasPrecision(38, 10);
                b.Property(x => x.NumericStep).HasPrecision(38, 10);

                b.HasIndex(x => x.CanonicalCode).IsUnique().HasFilter(SoftDeleteFilter);
                b.HasIndex(x => x.NormalizedName);
                b.HasIndex(x => x.AttributeGroupId);

                b.HasOne(typeof(AttributeGroup)).WithMany()
                    .HasForeignKey(nameof(AttributeDefinition.AttributeGroupId))
                    .OnDelete(DeleteBehavior.SetNull);
                b.HasOne(typeof(UnitGroup)).WithMany()
                    .HasForeignKey(nameof(AttributeDefinition.UnitGroupId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Unit)).WithMany()
                    .HasForeignKey(nameof(AttributeDefinition.BaseUnitId))
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<AttributeTranslation>(b =>
            {
                b.Property(x => x.Name).HasMaxLength(512).IsRequired();
                b.HasIndex(x => new { x.AttributeDefinitionId, x.Language }).IsUnique().HasFilter(SoftDeleteFilter);
                b.HasOne(typeof(AttributeDefinition)).WithMany()
                    .HasForeignKey(nameof(AttributeTranslation.AttributeDefinitionId))
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<AttributeAlias>(b =>
            {
                b.Property(x => x.Alias).HasMaxLength(512).IsRequired();
                b.Property(x => x.NormalizedAlias).HasMaxLength(256).IsRequired();
                b.HasIndex(x => new { x.AttributeDefinitionId, x.Language, x.NormalizedAlias })
                    .IsUnique().HasFilter(SoftDeleteFilter);
                b.HasOne(typeof(AttributeDefinition)).WithMany()
                    .HasForeignKey(nameof(AttributeAlias.AttributeDefinitionId))
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<AttributeSynonym>(b =>
            {
                b.Property(x => x.Token).HasMaxLength(512).IsRequired();
                b.Property(x => x.NormalizedToken).HasMaxLength(256).IsRequired();
                b.Property(x => x.Confidence).HasPrecision(5, 4);
                // Global tekil: bir token yalnızca tek attribute'a işaret eder.
                b.HasIndex(x => x.NormalizedToken).IsUnique().HasFilter(SoftDeleteFilter);
                b.HasIndex(x => x.AttributeDefinitionId);
                b.HasOne(typeof(AttributeDefinition)).WithMany()
                    .HasForeignKey(nameof(AttributeSynonym.AttributeDefinitionId))
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        // ── AttributeOption + çeviri/alias/synonym ─────────────────────────────
        private static void ApplyOptions(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AttributeOption>(b =>
            {
                b.Property(x => x.CanonicalCode).HasMaxLength(256).IsRequired();
                b.Property(x => x.NormalizedValue).HasMaxLength(256).IsRequired();
                b.Property(x => x.CanonicalValue).HasMaxLength(512).IsRequired();
                b.Property(x => x.ColorHex).HasMaxLength(16);
                b.Property(x => x.ImageUrl).HasMaxLength(1024);
                b.Property(x => x.PathCodes).HasMaxLength(2048);
                b.Property(x => x.NumericValue).HasPrecision(38, 10);

                b.HasIndex(x => new { x.AttributeDefinitionId, x.CanonicalCode }).IsUnique().HasFilter(SoftDeleteFilter);
                b.HasIndex(x => new { x.AttributeDefinitionId, x.NormalizedValue });
                b.HasIndex(x => x.ParentOptionId);

                b.HasOne(typeof(AttributeDefinition)).WithMany()
                    .HasForeignKey(nameof(AttributeOption.AttributeDefinitionId))
                    .OnDelete(DeleteBehavior.Cascade);
                // Ağaç kendine referans → döngü/çoklu-yol hatasını önlemek için NoAction.
                b.HasOne(typeof(AttributeOption)).WithMany()
                    .HasForeignKey(nameof(AttributeOption.ParentOptionId))
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<AttributeOptionTranslation>(b =>
            {
                b.Property(x => x.Value).HasMaxLength(512).IsRequired();
                b.HasIndex(x => new { x.AttributeOptionId, x.Language }).IsUnique().HasFilter(SoftDeleteFilter);
                b.HasOne(typeof(AttributeOption)).WithMany()
                    .HasForeignKey(nameof(AttributeOptionTranslation.AttributeOptionId))
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<AttributeOptionAlias>(b =>
            {
                b.Property(x => x.Alias).HasMaxLength(512).IsRequired();
                b.Property(x => x.NormalizedAlias).HasMaxLength(256).IsRequired();
                b.HasIndex(x => new { x.AttributeOptionId, x.Language, x.NormalizedAlias })
                    .IsUnique().HasFilter(SoftDeleteFilter);
                b.HasOne(typeof(AttributeOption)).WithMany()
                    .HasForeignKey(nameof(AttributeOptionAlias.AttributeOptionId))
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<AttributeOptionSynonym>(b =>
            {
                b.Property(x => x.Token).HasMaxLength(512).IsRequired();
                b.Property(x => x.NormalizedToken).HasMaxLength(256).IsRequired();
                b.Property(x => x.Confidence).HasPrecision(5, 4);
                // Attribute kapsamında tekil: aynı token aynı attribute içinde tek option'a.
                b.HasIndex(x => new { x.AttributeDefinitionId, x.NormalizedToken })
                    .IsUnique().HasFilter(SoftDeleteFilter);
                b.HasIndex(x => x.AttributeOptionId);
                b.HasOne(typeof(AttributeOption)).WithMany()
                    .HasForeignKey(nameof(AttributeOptionSynonym.AttributeOptionId))
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        // ── Unit / UnitGroup ──────────────────────────────────────────────────
        private static void ApplyUnits(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UnitGroup>(b =>
            {
                b.Property(x => x.CanonicalCode).HasMaxLength(256).IsRequired();
                b.Property(x => x.CanonicalName).HasMaxLength(512).IsRequired();
                b.HasIndex(x => x.CanonicalCode).IsUnique().HasFilter(SoftDeleteFilter);
                // Baz birim dairesel FK → NoAction.
                b.HasOne(typeof(Unit)).WithMany()
                    .HasForeignKey(nameof(UnitGroup.BaseUnitId))
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<UnitGroupTranslation>(b =>
            {
                b.Property(x => x.Name).HasMaxLength(512).IsRequired();
                b.HasIndex(x => new { x.UnitGroupId, x.Language }).IsUnique().HasFilter(SoftDeleteFilter);
                b.HasOne(typeof(UnitGroup)).WithMany()
                    .HasForeignKey(nameof(UnitGroupTranslation.UnitGroupId))
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Unit>(b =>
            {
                b.Property(x => x.CanonicalCode).HasMaxLength(256).IsRequired();
                b.Property(x => x.Symbol).HasMaxLength(32).IsRequired();
                b.Property(x => x.CanonicalName).HasMaxLength(512).IsRequired();
                b.Property(x => x.ConversionFactorToBase).HasPrecision(38, 18);
                b.Property(x => x.ConversionOffset).HasPrecision(38, 18);
                b.HasIndex(x => new { x.UnitGroupId, x.CanonicalCode }).IsUnique().HasFilter(SoftDeleteFilter);
                b.HasIndex(x => new { x.UnitGroupId, x.Symbol }).IsUnique().HasFilter(SoftDeleteFilter);
                b.HasOne(typeof(UnitGroup)).WithMany()
                    .HasForeignKey(nameof(Unit.UnitGroupId))
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UnitTranslation>(b =>
            {
                b.Property(x => x.Name).HasMaxLength(512).IsRequired();
                b.Property(x => x.PluralName).HasMaxLength(512);
                b.HasIndex(x => new { x.UnitId, x.Language }).IsUnique().HasFilter(SoftDeleteFilter);
                b.HasOne(typeof(Unit)).WithMany()
                    .HasForeignKey(nameof(UnitTranslation.UnitId))
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        // ── Template ──────────────────────────────────────────────────────────
        private static void ApplyTemplates(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AttributeTemplate>(b =>
            {
                b.Property(x => x.CanonicalCode).HasMaxLength(256).IsRequired();
                b.Property(x => x.CanonicalName).HasMaxLength(512).IsRequired();
                b.HasIndex(x => x.CanonicalCode).IsUnique().HasFilter(SoftDeleteFilter);
                b.HasOne(typeof(AttributeTemplate)).WithMany()
                    .HasForeignKey(nameof(AttributeTemplate.SupersededByTemplateId))
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<AttributeTemplateTranslation>(b =>
            {
                b.Property(x => x.Name).HasMaxLength(512).IsRequired();
                b.HasIndex(x => new { x.AttributeTemplateId, x.Language }).IsUnique().HasFilter(SoftDeleteFilter);
                b.HasOne(typeof(AttributeTemplate)).WithMany()
                    .HasForeignKey(nameof(AttributeTemplateTranslation.AttributeTemplateId))
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TemplateAttribute>(b =>
            {
                b.HasIndex(x => new { x.AttributeTemplateId, x.AttributeDefinitionId }).IsUnique().HasFilter(SoftDeleteFilter);
                b.HasIndex(x => x.AttributeDefinitionId);
                b.HasOne(typeof(AttributeTemplate)).WithMany()
                    .HasForeignKey(nameof(TemplateAttribute.AttributeTemplateId))
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(AttributeDefinition)).WithMany()
                    .HasForeignKey(nameof(TemplateAttribute.AttributeDefinitionId))
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TemplateCategory>(b =>
            {
                b.HasIndex(x => new { x.AttributeTemplateId, x.CategoryId }).IsUnique().HasFilter(SoftDeleteFilter);
                b.HasIndex(x => x.CategoryId);
                b.HasOne(typeof(AttributeTemplate)).WithMany()
                    .HasForeignKey(nameof(TemplateCategory.AttributeTemplateId))
                    .OnDelete(DeleteBehavior.Cascade);
                // CategoryId → CategoriesProduct (Guid PK): dış tabloya FK isterseniz burada
                // .HasOne(typeof(CategoriesProduct))... ekleyin (varsayılan: yalnızca indeks).
            });
        }

        // ── CategoryAttribute ─────────────────────────────────────────────────
        private static void ApplyCategoryLinks(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CategoryAttribute>(b =>
            {
                b.HasIndex(x => new { x.CategoryId, x.AttributeDefinitionId }).IsUnique().HasFilter(SoftDeleteFilter);
                b.HasIndex(x => x.AttributeDefinitionId);
                b.HasIndex(x => x.SourceTemplateId);
                b.HasOne(typeof(AttributeDefinition)).WithMany()
                    .HasForeignKey(nameof(CategoryAttribute.AttributeDefinitionId))
                    .OnDelete(DeleteBehavior.Restrict);
                b.HasOne(typeof(AttributeTemplate)).WithMany()
                    .HasForeignKey(nameof(CategoryAttribute.SourceTemplateId))
                    .OnDelete(DeleteBehavior.NoAction);
                // CategoryId → CategoriesProduct (Guid PK): dış FK isterseniz burada ekleyin.
            });
        }

        // ── Kurallar: Dependency, NormalizationRule ────────────────────────────
        private static void ApplyRules(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AttributeDependency>(b =>
            {
                b.Property(x => x.ExpectedValue).HasMaxLength(1024);
                b.HasIndex(x => new { x.CategoryId, x.SourceAttributeDefinitionId });
                b.HasIndex(x => x.TargetAttributeDefinitionId);
                // Aynı tabloya iki FK (Source/Target) → çoklu-yol hatasını önlemek için NoAction.
                b.HasOne(typeof(AttributeDefinition)).WithMany()
                    .HasForeignKey(nameof(AttributeDependency.SourceAttributeDefinitionId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(AttributeDefinition)).WithMany()
                    .HasForeignKey(nameof(AttributeDependency.TargetAttributeDefinitionId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(AttributeOption)).WithMany()
                    .HasForeignKey(nameof(AttributeDependency.ExpectedOptionId))
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<NormalizationRule>(b =>
            {
                b.Property(x => x.CanonicalCode).HasMaxLength(256).IsRequired();
                b.Property(x => x.Name).HasMaxLength(512).IsRequired();
                b.Property(x => x.Pattern).HasMaxLength(2048);
                b.Property(x => x.Replacement).HasMaxLength(2048);
                b.HasIndex(x => x.CanonicalCode).IsUnique().HasFilter(SoftDeleteFilter);
            });
        }

        // ── AI yönetişimi ─────────────────────────────────────────────────────
        private static void ApplyAi(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AiGenerationJob>(b =>
            {
                b.Property(x => x.IdempotencyKey).HasMaxLength(256).IsRequired();
                b.Property(x => x.PromptVersion).HasMaxLength(64);
                b.Property(x => x.PromptHash).HasMaxLength(128);
                b.Property(x => x.LeasedBy).HasMaxLength(128);
                b.Property(x => x.CategoryPathSnapshot).HasMaxLength(2048);
                b.Property(x => x.ResultSummary).HasMaxLength(2048);
                b.Property(x => x.ErrorMessage).HasMaxLength(4000);
                b.HasIndex(x => x.IdempotencyKey).IsUnique().HasFilter(SoftDeleteFilter);
                b.HasIndex(x => new { x.Status, x.Priority, x.NextRetryAtUtc });
                b.HasIndex(x => new { x.JobType, x.Status });
            });

            modelBuilder.Entity<AiGenerationHistory>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.EntityType).HasMaxLength(128).IsRequired();
                b.Property(x => x.Action).HasMaxLength(64).IsRequired();
                b.Property(x => x.AiModel).HasMaxLength(128);
                b.Property(x => x.AiModelVersion).HasMaxLength(64);
                b.Property(x => x.PromptHash).HasMaxLength(128);
                b.Property(x => x.PromptVersion).HasMaxLength(64);
                b.Property(x => x.Reason).HasMaxLength(2048);
                b.Property(x => x.Confidence).HasPrecision(5, 4);
                b.HasIndex(x => new { x.EntityType, x.EntityId, x.CreatedAtUtc });
                b.HasIndex(x => x.AiGenerationJobId);
            });
        }

        // ── Entegrasyon eşleştirmeleri ─────────────────────────────────────────
        private static void ApplyIntegration(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IntegrationPlatform>(b =>
            {
                b.Property(x => x.CanonicalCode).HasMaxLength(256).IsRequired();
                b.Property(x => x.Name).HasMaxLength(256).IsRequired();
                b.HasIndex(x => x.CanonicalCode).IsUnique().HasFilter(SoftDeleteFilter);
            });

            modelBuilder.Entity<AttributeMapping>(b =>
            {
                b.Property(x => x.ExternalCode).HasMaxLength(256).IsRequired();
                b.Property(x => x.ExternalId).HasMaxLength(256);
                b.Property(x => x.ExternalName).HasMaxLength(512);
                b.Property(x => x.TransformRuleJson).HasMaxLength(4000);
                b.HasIndex(x => new { x.IntegrationPlatformId, x.AttributeDefinitionId, x.Direction })
                    .IsUnique().HasFilter(SoftDeleteFilter);
                b.HasIndex(x => new { x.IntegrationPlatformId, x.ExternalCode });
                // Attribute → Cascade; Platform → NoAction (çoklu cascade-yolu engeli).
                b.HasOne(typeof(AttributeDefinition)).WithMany()
                    .HasForeignKey(nameof(AttributeMapping.AttributeDefinitionId))
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(IntegrationPlatform)).WithMany()
                    .HasForeignKey(nameof(AttributeMapping.IntegrationPlatformId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(NormalizationRule)).WithMany()
                    .HasForeignKey(nameof(AttributeMapping.NormalizationRuleId))
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<AttributeOptionMapping>(b =>
            {
                b.Property(x => x.ExternalCode).HasMaxLength(256).IsRequired();
                b.Property(x => x.ExternalValue).HasMaxLength(512);
                b.Property(x => x.ExternalId).HasMaxLength(256);
                b.HasIndex(x => new { x.IntegrationPlatformId, x.AttributeOptionId, x.Direction })
                    .IsUnique().HasFilter(SoftDeleteFilter);
                b.HasIndex(x => new { x.IntegrationPlatformId, x.AttributeDefinitionId, x.ExternalCode });
                b.HasOne(typeof(AttributeOption)).WithMany()
                    .HasForeignKey(nameof(AttributeOptionMapping.AttributeOptionId))
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(IntegrationPlatform)).WithMany()
                    .HasForeignKey(nameof(AttributeOptionMapping.IntegrationPlatformId))
                    .OnDelete(DeleteBehavior.NoAction);
                // AttributeDefinitionId denormalize → yalnızca indeks (ek FK yok).
            });
        }
    }
}
