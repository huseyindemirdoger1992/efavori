using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _115 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AiGenerationHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AiGenerationJobId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    AiModel = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    AiModelVersion = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Confidence = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: true),
                    PromptHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    PromptVersion = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    PreviousValueJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValueJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PerformedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Source = table.Column<byte>(type: "tinyint", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiGenerationHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AiGenerationJobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobType = table.Column<byte>(type: "tinyint", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    TargetCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TargetAttributeDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    IdempotencyKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CategoryPathSnapshot = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    PromptVersion = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    PromptHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    AttemptCount = table.Column<int>(type: "int", nullable: false),
                    MaxAttempts = table.Column<int>(type: "int", nullable: false),
                    NextRetryAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LeasedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    LeasedUntilUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResultSummary = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedRecordCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiGenerationJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttributeGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CanonicalCode = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CanonicalName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IconCssClass = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Ai_Source = table.Column<byte>(type: "tinyint", nullable: false),
                    Ai_IsManuallyEdited = table.Column<bool>(type: "bit", nullable: false),
                    Ai_ManuallyEditedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Ai_ManuallyEditedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ai_AiModel = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Ai_AiModelVersion = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Ai_Confidence = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: true),
                    Ai_PromptHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Ai_PromptVersion = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Ai_AiGeneratedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ai_ApprovalStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    Ai_ReviewStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    Ai_ReviewNote = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Ai_ReviewedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Ai_ReviewedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ai_CategorySource = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttributeTemplates_V3",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CanonicalCode = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CanonicalName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    SupersededByTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Ai_Source = table.Column<byte>(type: "tinyint", nullable: false),
                    Ai_IsManuallyEdited = table.Column<bool>(type: "bit", nullable: false),
                    Ai_ManuallyEditedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Ai_ManuallyEditedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ai_AiModel = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Ai_AiModelVersion = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Ai_Confidence = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: true),
                    Ai_PromptHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Ai_PromptVersion = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Ai_AiGeneratedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ai_ApprovalStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    Ai_ReviewStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    Ai_ReviewNote = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Ai_ReviewedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Ai_ReviewedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ai_CategorySource = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeTemplates_V3", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttributeTemplates_V3_AttributeTemplates_V3_SupersededByTemplateId",
                        column: x => x.SupersededByTemplateId,
                        principalTable: "AttributeTemplates_V3",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IntegrationPlatforms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CanonicalCode = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntegrationPlatforms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NormalizationRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CanonicalCode = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    RuleType = table.Column<byte>(type: "tinyint", nullable: false),
                    Pattern = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Replacement = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    AppliesToDataType = table.Column<byte>(type: "tinyint", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NormalizationRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttributeGroupTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Language = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Source = table.Column<byte>(type: "tinyint", nullable: false),
                    IsManuallyEdited = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeGroupTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttributeGroupTranslations_AttributeGroups_AttributeGroupId",
                        column: x => x.AttributeGroupId,
                        principalTable: "AttributeGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttributeTemplateTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Language = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Source = table.Column<byte>(type: "tinyint", nullable: false),
                    IsManuallyEdited = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeTemplateTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttributeTemplateTranslations_AttributeTemplates_V3_AttributeTemplateId",
                        column: x => x.AttributeTemplateId,
                        principalTable: "AttributeTemplates_V3",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TemplateCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    Source = table.Column<byte>(type: "tinyint", nullable: false),
                    Ai_Source = table.Column<byte>(type: "tinyint", nullable: false),
                    Ai_IsManuallyEdited = table.Column<bool>(type: "bit", nullable: false),
                    Ai_ManuallyEditedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Ai_ManuallyEditedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ai_AiModel = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Ai_AiModelVersion = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Ai_Confidence = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: true),
                    Ai_PromptHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Ai_PromptVersion = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Ai_AiGeneratedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ai_ApprovalStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    Ai_ReviewStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    Ai_ReviewNote = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Ai_ReviewedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Ai_ReviewedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ai_CategorySource = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemplateCategories_AttributeTemplates_V3_AttributeTemplateId",
                        column: x => x.AttributeTemplateId,
                        principalTable: "AttributeTemplates_V3",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttributeAliases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Language = table.Column<byte>(type: "tinyint", nullable: true),
                    Alias = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    NormalizedAlias = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Source = table.Column<byte>(type: "tinyint", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeAliases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttributeDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CanonicalCode = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CanonicalName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    AttributeGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DataType = table.Column<byte>(type: "tinyint", nullable: false),
                    InputType = table.Column<byte>(type: "tinyint", nullable: false),
                    UnitGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BaseUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsMultiValued = table.Column<bool>(type: "bit", nullable: false),
                    IsRequiredByDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsVariantByDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsFilterableByDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsSearchableByDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsComparableByDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsVisibleOnProductPageByDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsKeyAttribute = table.Column<bool>(type: "bit", nullable: false),
                    RegexPattern = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    MinNumericValue = table.Column<decimal>(type: "decimal(38,10)", precision: 38, scale: 10, nullable: true),
                    MaxNumericValue = table.Column<decimal>(type: "decimal(38,10)", precision: 38, scale: 10, nullable: true),
                    NumericStep = table.Column<decimal>(type: "decimal(38,10)", precision: 38, scale: 10, nullable: true),
                    DecimalScale = table.Column<int>(type: "int", nullable: true),
                    MinLength = table.Column<int>(type: "int", nullable: true),
                    MaxLength = table.Column<int>(type: "int", nullable: true),
                    IsUnique = table.Column<bool>(type: "bit", nullable: false),
                    DefaultValue = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsSystemLocked = table.Column<bool>(type: "bit", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    Ai_Source = table.Column<byte>(type: "tinyint", nullable: false),
                    Ai_IsManuallyEdited = table.Column<bool>(type: "bit", nullable: false),
                    Ai_ManuallyEditedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Ai_ManuallyEditedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ai_AiModel = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Ai_AiModelVersion = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Ai_Confidence = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: true),
                    Ai_PromptHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Ai_PromptVersion = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Ai_AiGeneratedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ai_ApprovalStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    Ai_ReviewStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    Ai_ReviewNote = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Ai_ReviewedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Ai_ReviewedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ai_CategorySource = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttributeDefinitions_AttributeGroups_AttributeGroupId",
                        column: x => x.AttributeGroupId,
                        principalTable: "AttributeGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AttributeMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IntegrationPlatformId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExternalCode = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ExternalName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Direction = table.Column<byte>(type: "tinyint", nullable: false),
                    RequirementLevel = table.Column<byte>(type: "tinyint", nullable: false),
                    TransformRuleJson = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    NormalizationRuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Ai_Source = table.Column<byte>(type: "tinyint", nullable: false),
                    Ai_IsManuallyEdited = table.Column<bool>(type: "bit", nullable: false),
                    Ai_ManuallyEditedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Ai_ManuallyEditedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ai_AiModel = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Ai_AiModelVersion = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Ai_Confidence = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: true),
                    Ai_PromptHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Ai_PromptVersion = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Ai_AiGeneratedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ai_ApprovalStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    Ai_ReviewStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    Ai_ReviewNote = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Ai_ReviewedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Ai_ReviewedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ai_CategorySource = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttributeMappings_AttributeDefinitions_AttributeDefinitionId",
                        column: x => x.AttributeDefinitionId,
                        principalTable: "AttributeDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttributeMappings_IntegrationPlatforms_IntegrationPlatformId",
                        column: x => x.IntegrationPlatformId,
                        principalTable: "IntegrationPlatforms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AttributeMappings_NormalizationRules_NormalizationRuleId",
                        column: x => x.NormalizationRuleId,
                        principalTable: "NormalizationRules",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AttributeOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentOptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CanonicalCode = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NormalizedValue = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CanonicalValue = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    ColorHex = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    NumericValue = table.Column<decimal>(type: "decimal(38,10)", precision: 38, scale: 10, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    PathCodes = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Ai_Source = table.Column<byte>(type: "tinyint", nullable: false),
                    Ai_IsManuallyEdited = table.Column<bool>(type: "bit", nullable: false),
                    Ai_ManuallyEditedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Ai_ManuallyEditedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ai_AiModel = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Ai_AiModelVersion = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Ai_Confidence = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: true),
                    Ai_PromptHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Ai_PromptVersion = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Ai_AiGeneratedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ai_ApprovalStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    Ai_ReviewStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    Ai_ReviewNote = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Ai_ReviewedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Ai_ReviewedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ai_CategorySource = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttributeOptions_AttributeDefinitions_AttributeDefinitionId",
                        column: x => x.AttributeDefinitionId,
                        principalTable: "AttributeDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttributeOptions_AttributeOptions_ParentOptionId",
                        column: x => x.ParentOptionId,
                        principalTable: "AttributeOptions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AttributeSynonyms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    NormalizedToken = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Language = table.Column<byte>(type: "tinyint", nullable: true),
                    Source = table.Column<byte>(type: "tinyint", nullable: false),
                    Confidence = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeSynonyms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttributeSynonyms_AttributeDefinitions_AttributeDefinitionId",
                        column: x => x.AttributeDefinitionId,
                        principalTable: "AttributeDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttributeTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Language = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HelpText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Placeholder = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Source = table.Column<byte>(type: "tinyint", nullable: false),
                    IsManuallyEdited = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttributeTranslations_AttributeDefinitions_AttributeDefinitionId",
                        column: x => x.AttributeDefinitionId,
                        principalTable: "AttributeDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CategoryAttributes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsInherited = table.Column<bool>(type: "bit", nullable: false),
                    InheritedFromCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsOverride = table.Column<bool>(type: "bit", nullable: false),
                    IsSuppressed = table.Column<bool>(type: "bit", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: true),
                    IsVariant = table.Column<bool>(type: "bit", nullable: true),
                    IsFilterable = table.Column<bool>(type: "bit", nullable: true),
                    IsSearchable = table.Column<bool>(type: "bit", nullable: true),
                    IsComparable = table.Column<bool>(type: "bit", nullable: true),
                    IsVisibleOnProductPage = table.Column<bool>(type: "bit", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    AttributeGroupOverrideId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Ai_Source = table.Column<byte>(type: "tinyint", nullable: false),
                    Ai_IsManuallyEdited = table.Column<bool>(type: "bit", nullable: false),
                    Ai_ManuallyEditedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Ai_ManuallyEditedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ai_AiModel = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Ai_AiModelVersion = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Ai_Confidence = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: true),
                    Ai_PromptHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Ai_PromptVersion = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Ai_AiGeneratedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ai_ApprovalStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    Ai_ReviewStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    Ai_ReviewNote = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Ai_ReviewedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Ai_ReviewedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ai_CategorySource = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryAttributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryAttributes_AttributeDefinitions_AttributeDefinitionId",
                        column: x => x.AttributeDefinitionId,
                        principalTable: "AttributeDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CategoryAttributes_AttributeTemplates_V3_SourceTemplateId",
                        column: x => x.SourceTemplateId,
                        principalTable: "AttributeTemplates_V3",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TemplateAttributes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsRequiredOverride = table.Column<bool>(type: "bit", nullable: true),
                    IsVariantOverride = table.Column<bool>(type: "bit", nullable: true),
                    IsFilterableOverride = table.Column<bool>(type: "bit", nullable: true),
                    IsComparableOverride = table.Column<bool>(type: "bit", nullable: true),
                    IsVisibleOverride = table.Column<bool>(type: "bit", nullable: true),
                    AttributeGroupOverrideId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Ai_Source = table.Column<byte>(type: "tinyint", nullable: false),
                    Ai_IsManuallyEdited = table.Column<bool>(type: "bit", nullable: false),
                    Ai_ManuallyEditedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Ai_ManuallyEditedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ai_AiModel = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Ai_AiModelVersion = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Ai_Confidence = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: true),
                    Ai_PromptHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Ai_PromptVersion = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Ai_AiGeneratedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ai_ApprovalStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    Ai_ReviewStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    Ai_ReviewNote = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Ai_ReviewedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Ai_ReviewedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ai_CategorySource = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateAttributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemplateAttributes_AttributeDefinitions_AttributeDefinitionId",
                        column: x => x.AttributeDefinitionId,
                        principalTable: "AttributeDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TemplateAttributes_AttributeTemplates_V3_AttributeTemplateId",
                        column: x => x.AttributeTemplateId,
                        principalTable: "AttributeTemplates_V3",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttributeDependencies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SourceAttributeDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Operator = table.Column<byte>(type: "tinyint", nullable: false),
                    ExpectedOptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExpectedValue = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    TargetAttributeDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<byte>(type: "tinyint", nullable: false),
                    ConditionGroup = table.Column<int>(type: "int", nullable: false),
                    GroupLogic = table.Column<byte>(type: "tinyint", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Ai_Source = table.Column<byte>(type: "tinyint", nullable: false),
                    Ai_IsManuallyEdited = table.Column<bool>(type: "bit", nullable: false),
                    Ai_ManuallyEditedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Ai_ManuallyEditedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ai_AiModel = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Ai_AiModelVersion = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Ai_Confidence = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: true),
                    Ai_PromptHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Ai_PromptVersion = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Ai_AiGeneratedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ai_ApprovalStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    Ai_ReviewStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    Ai_ReviewNote = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Ai_ReviewedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Ai_ReviewedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ai_CategorySource = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeDependencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttributeDependencies_AttributeDefinitions_SourceAttributeDefinitionId",
                        column: x => x.SourceAttributeDefinitionId,
                        principalTable: "AttributeDefinitions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AttributeDependencies_AttributeDefinitions_TargetAttributeDefinitionId",
                        column: x => x.TargetAttributeDefinitionId,
                        principalTable: "AttributeDefinitions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AttributeDependencies_AttributeOptions_ExpectedOptionId",
                        column: x => x.ExpectedOptionId,
                        principalTable: "AttributeOptions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AttributeOptionAliases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeOptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Language = table.Column<byte>(type: "tinyint", nullable: true),
                    Alias = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    NormalizedAlias = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Source = table.Column<byte>(type: "tinyint", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeOptionAliases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttributeOptionAliases_AttributeOptions_AttributeOptionId",
                        column: x => x.AttributeOptionId,
                        principalTable: "AttributeOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttributeOptionMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IntegrationPlatformId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeOptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExternalCode = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ExternalValue = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    ExternalId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Direction = table.Column<byte>(type: "tinyint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Ai_Source = table.Column<byte>(type: "tinyint", nullable: false),
                    Ai_IsManuallyEdited = table.Column<bool>(type: "bit", nullable: false),
                    Ai_ManuallyEditedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Ai_ManuallyEditedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ai_AiModel = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Ai_AiModelVersion = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Ai_Confidence = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: true),
                    Ai_PromptHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Ai_PromptVersion = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Ai_AiGeneratedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ai_ApprovalStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    Ai_ReviewStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    Ai_ReviewNote = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Ai_ReviewedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Ai_ReviewedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ai_CategorySource = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeOptionMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttributeOptionMappings_AttributeOptions_AttributeOptionId",
                        column: x => x.AttributeOptionId,
                        principalTable: "AttributeOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttributeOptionMappings_IntegrationPlatforms_IntegrationPlatformId",
                        column: x => x.IntegrationPlatformId,
                        principalTable: "IntegrationPlatforms",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AttributeOptionSynonyms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeOptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    NormalizedToken = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Language = table.Column<byte>(type: "tinyint", nullable: true),
                    Source = table.Column<byte>(type: "tinyint", nullable: false),
                    Confidence = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeOptionSynonyms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttributeOptionSynonyms_AttributeOptions_AttributeOptionId",
                        column: x => x.AttributeOptionId,
                        principalTable: "AttributeOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttributeOptionTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeOptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Language = table.Column<byte>(type: "tinyint", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Source = table.Column<byte>(type: "tinyint", nullable: false),
                    IsManuallyEdited = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeOptionTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttributeOptionTranslations_AttributeOptions_AttributeOptionId",
                        column: x => x.AttributeOptionId,
                        principalTable: "AttributeOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnitGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CanonicalCode = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CanonicalName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Dimension = table.Column<byte>(type: "tinyint", nullable: false),
                    BaseUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UnitGroupTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Language = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Source = table.Column<byte>(type: "tinyint", nullable: false),
                    IsManuallyEdited = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitGroupTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitGroupTranslations_UnitGroups_UnitGroupId",
                        column: x => x.UnitGroupId,
                        principalTable: "UnitGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CanonicalCode = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    CanonicalName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    ConversionFactorToBase = table.Column<decimal>(type: "decimal(38,18)", precision: 38, scale: 18, nullable: false),
                    ConversionOffset = table.Column<decimal>(type: "decimal(38,18)", precision: 38, scale: 18, nullable: false),
                    IsBaseUnit = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Units_UnitGroups_UnitGroupId",
                        column: x => x.UnitGroupId,
                        principalTable: "UnitGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnitTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Language = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    PluralName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Source = table.Column<byte>(type: "tinyint", nullable: false),
                    IsManuallyEdited = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted_IsDeletedStatu = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted_DeletedAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitTranslations_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AiGenerationHistories_AiGenerationJobId",
                table: "AiGenerationHistories",
                column: "AiGenerationJobId");

            migrationBuilder.CreateIndex(
                name: "IX_AiGenerationHistories_EntityType_EntityId_CreatedAtUtc",
                table: "AiGenerationHistories",
                columns: new[] { "EntityType", "EntityId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_AiGenerationJobs_IdempotencyKey",
                table: "AiGenerationJobs",
                column: "IdempotencyKey",
                unique: true,
                filter: "[IsDeleted_IsDeletedStatu] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_AiGenerationJobs_JobType_Status",
                table: "AiGenerationJobs",
                columns: new[] { "JobType", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_AiGenerationJobs_Status_Priority_NextRetryAtUtc",
                table: "AiGenerationJobs",
                columns: new[] { "Status", "Priority", "NextRetryAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_AttributeAliases_AttributeDefinitionId_Language_NormalizedAlias",
                table: "AttributeAliases",
                columns: new[] { "AttributeDefinitionId", "Language", "NormalizedAlias" },
                unique: true,
                filter: "[IsDeleted_IsDeletedStatu] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeDefinitions_AttributeGroupId",
                table: "AttributeDefinitions",
                column: "AttributeGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeDefinitions_BaseUnitId",
                table: "AttributeDefinitions",
                column: "BaseUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeDefinitions_CanonicalCode",
                table: "AttributeDefinitions",
                column: "CanonicalCode",
                unique: true,
                filter: "[IsDeleted_IsDeletedStatu] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeDefinitions_NormalizedName",
                table: "AttributeDefinitions",
                column: "NormalizedName");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeDefinitions_UnitGroupId",
                table: "AttributeDefinitions",
                column: "UnitGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeDependencies_CategoryId_SourceAttributeDefinitionId",
                table: "AttributeDependencies",
                columns: new[] { "CategoryId", "SourceAttributeDefinitionId" });

            migrationBuilder.CreateIndex(
                name: "IX_AttributeDependencies_ExpectedOptionId",
                table: "AttributeDependencies",
                column: "ExpectedOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeDependencies_SourceAttributeDefinitionId",
                table: "AttributeDependencies",
                column: "SourceAttributeDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeDependencies_TargetAttributeDefinitionId",
                table: "AttributeDependencies",
                column: "TargetAttributeDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeGroups_CanonicalCode",
                table: "AttributeGroups",
                column: "CanonicalCode",
                unique: true,
                filter: "[IsDeleted_IsDeletedStatu] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeGroupTranslations_AttributeGroupId_Language",
                table: "AttributeGroupTranslations",
                columns: new[] { "AttributeGroupId", "Language" },
                unique: true,
                filter: "[IsDeleted_IsDeletedStatu] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeMappings_AttributeDefinitionId",
                table: "AttributeMappings",
                column: "AttributeDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeMappings_IntegrationPlatformId_AttributeDefinitionId_Direction",
                table: "AttributeMappings",
                columns: new[] { "IntegrationPlatformId", "AttributeDefinitionId", "Direction" },
                unique: true,
                filter: "[IsDeleted_IsDeletedStatu] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeMappings_IntegrationPlatformId_ExternalCode",
                table: "AttributeMappings",
                columns: new[] { "IntegrationPlatformId", "ExternalCode" });

            migrationBuilder.CreateIndex(
                name: "IX_AttributeMappings_NormalizationRuleId",
                table: "AttributeMappings",
                column: "NormalizationRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeOptionAliases_AttributeOptionId_Language_NormalizedAlias",
                table: "AttributeOptionAliases",
                columns: new[] { "AttributeOptionId", "Language", "NormalizedAlias" },
                unique: true,
                filter: "[IsDeleted_IsDeletedStatu] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeOptionMappings_AttributeOptionId",
                table: "AttributeOptionMappings",
                column: "AttributeOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeOptionMappings_IntegrationPlatformId_AttributeDefinitionId_ExternalCode",
                table: "AttributeOptionMappings",
                columns: new[] { "IntegrationPlatformId", "AttributeDefinitionId", "ExternalCode" });

            migrationBuilder.CreateIndex(
                name: "IX_AttributeOptionMappings_IntegrationPlatformId_AttributeOptionId_Direction",
                table: "AttributeOptionMappings",
                columns: new[] { "IntegrationPlatformId", "AttributeOptionId", "Direction" },
                unique: true,
                filter: "[IsDeleted_IsDeletedStatu] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeOptions_AttributeDefinitionId_CanonicalCode",
                table: "AttributeOptions",
                columns: new[] { "AttributeDefinitionId", "CanonicalCode" },
                unique: true,
                filter: "[IsDeleted_IsDeletedStatu] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeOptions_AttributeDefinitionId_NormalizedValue",
                table: "AttributeOptions",
                columns: new[] { "AttributeDefinitionId", "NormalizedValue" });

            migrationBuilder.CreateIndex(
                name: "IX_AttributeOptions_ParentOptionId",
                table: "AttributeOptions",
                column: "ParentOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeOptionSynonyms_AttributeDefinitionId_NormalizedToken",
                table: "AttributeOptionSynonyms",
                columns: new[] { "AttributeDefinitionId", "NormalizedToken" },
                unique: true,
                filter: "[IsDeleted_IsDeletedStatu] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeOptionSynonyms_AttributeOptionId",
                table: "AttributeOptionSynonyms",
                column: "AttributeOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeOptionTranslations_AttributeOptionId_Language",
                table: "AttributeOptionTranslations",
                columns: new[] { "AttributeOptionId", "Language" },
                unique: true,
                filter: "[IsDeleted_IsDeletedStatu] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeSynonyms_AttributeDefinitionId",
                table: "AttributeSynonyms",
                column: "AttributeDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeSynonyms_NormalizedToken",
                table: "AttributeSynonyms",
                column: "NormalizedToken",
                unique: true,
                filter: "[IsDeleted_IsDeletedStatu] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeTemplates_V3_CanonicalCode",
                table: "AttributeTemplates_V3",
                column: "CanonicalCode",
                unique: true,
                filter: "[IsDeleted_IsDeletedStatu] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeTemplates_V3_SupersededByTemplateId",
                table: "AttributeTemplates_V3",
                column: "SupersededByTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeTemplateTranslations_AttributeTemplateId_Language",
                table: "AttributeTemplateTranslations",
                columns: new[] { "AttributeTemplateId", "Language" },
                unique: true,
                filter: "[IsDeleted_IsDeletedStatu] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeTranslations_AttributeDefinitionId_Language",
                table: "AttributeTranslations",
                columns: new[] { "AttributeDefinitionId", "Language" },
                unique: true,
                filter: "[IsDeleted_IsDeletedStatu] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryAttributes_AttributeDefinitionId",
                table: "CategoryAttributes",
                column: "AttributeDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryAttributes_CategoryId_AttributeDefinitionId",
                table: "CategoryAttributes",
                columns: new[] { "CategoryId", "AttributeDefinitionId" },
                unique: true,
                filter: "[IsDeleted_IsDeletedStatu] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryAttributes_SourceTemplateId",
                table: "CategoryAttributes",
                column: "SourceTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_IntegrationPlatforms_CanonicalCode",
                table: "IntegrationPlatforms",
                column: "CanonicalCode",
                unique: true,
                filter: "[IsDeleted_IsDeletedStatu] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_NormalizationRules_CanonicalCode",
                table: "NormalizationRules",
                column: "CanonicalCode",
                unique: true,
                filter: "[IsDeleted_IsDeletedStatu] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateAttributes_AttributeDefinitionId",
                table: "TemplateAttributes",
                column: "AttributeDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateAttributes_AttributeTemplateId_AttributeDefinitionId",
                table: "TemplateAttributes",
                columns: new[] { "AttributeTemplateId", "AttributeDefinitionId" },
                unique: true,
                filter: "[IsDeleted_IsDeletedStatu] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateCategories_AttributeTemplateId_CategoryId",
                table: "TemplateCategories",
                columns: new[] { "AttributeTemplateId", "CategoryId" },
                unique: true,
                filter: "[IsDeleted_IsDeletedStatu] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateCategories_CategoryId",
                table: "TemplateCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitGroups_BaseUnitId",
                table: "UnitGroups",
                column: "BaseUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitGroups_CanonicalCode",
                table: "UnitGroups",
                column: "CanonicalCode",
                unique: true,
                filter: "[IsDeleted_IsDeletedStatu] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_UnitGroupTranslations_UnitGroupId_Language",
                table: "UnitGroupTranslations",
                columns: new[] { "UnitGroupId", "Language" },
                unique: true,
                filter: "[IsDeleted_IsDeletedStatu] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Units_UnitGroupId_CanonicalCode",
                table: "Units",
                columns: new[] { "UnitGroupId", "CanonicalCode" },
                unique: true,
                filter: "[IsDeleted_IsDeletedStatu] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Units_UnitGroupId_Symbol",
                table: "Units",
                columns: new[] { "UnitGroupId", "Symbol" },
                unique: true,
                filter: "[IsDeleted_IsDeletedStatu] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_UnitTranslations_UnitId_Language",
                table: "UnitTranslations",
                columns: new[] { "UnitId", "Language" },
                unique: true,
                filter: "[IsDeleted_IsDeletedStatu] = 0");

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeAliases_AttributeDefinitions_AttributeDefinitionId",
                table: "AttributeAliases",
                column: "AttributeDefinitionId",
                principalTable: "AttributeDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeDefinitions_UnitGroups_UnitGroupId",
                table: "AttributeDefinitions",
                column: "UnitGroupId",
                principalTable: "UnitGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeDefinitions_Units_BaseUnitId",
                table: "AttributeDefinitions",
                column: "BaseUnitId",
                principalTable: "Units",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UnitGroups_Units_BaseUnitId",
                table: "UnitGroups",
                column: "BaseUnitId",
                principalTable: "Units",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Units_UnitGroups_UnitGroupId",
                table: "Units");

            migrationBuilder.DropTable(
                name: "AiGenerationHistories");

            migrationBuilder.DropTable(
                name: "AiGenerationJobs");

            migrationBuilder.DropTable(
                name: "AttributeAliases");

            migrationBuilder.DropTable(
                name: "AttributeDependencies");

            migrationBuilder.DropTable(
                name: "AttributeGroupTranslations");

            migrationBuilder.DropTable(
                name: "AttributeMappings");

            migrationBuilder.DropTable(
                name: "AttributeOptionAliases");

            migrationBuilder.DropTable(
                name: "AttributeOptionMappings");

            migrationBuilder.DropTable(
                name: "AttributeOptionSynonyms");

            migrationBuilder.DropTable(
                name: "AttributeOptionTranslations");

            migrationBuilder.DropTable(
                name: "AttributeSynonyms");

            migrationBuilder.DropTable(
                name: "AttributeTemplateTranslations");

            migrationBuilder.DropTable(
                name: "AttributeTranslations");

            migrationBuilder.DropTable(
                name: "CategoryAttributes");

            migrationBuilder.DropTable(
                name: "TemplateAttributes");

            migrationBuilder.DropTable(
                name: "TemplateCategories");

            migrationBuilder.DropTable(
                name: "UnitGroupTranslations");

            migrationBuilder.DropTable(
                name: "UnitTranslations");

            migrationBuilder.DropTable(
                name: "NormalizationRules");

            migrationBuilder.DropTable(
                name: "IntegrationPlatforms");

            migrationBuilder.DropTable(
                name: "AttributeOptions");

            migrationBuilder.DropTable(
                name: "AttributeTemplates_V3");

            migrationBuilder.DropTable(
                name: "AttributeDefinitions");

            migrationBuilder.DropTable(
                name: "AttributeGroups");

            migrationBuilder.DropTable(
                name: "UnitGroups");

            migrationBuilder.DropTable(
                name: "Units");
        }
    }
}
