using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _41 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemRoad",
                table: "ItemGallery");

            migrationBuilder.AddColumn<Guid>(
                name: "ActivityCertificate",
                table: "Stores",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AuthorizedPersonId",
                table: "Stores",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BankAccountConfirmation",
                table: "Stores",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BankStatement",
                table: "Stores",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CertificateOfIncorporation",
                table: "Stores",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CustomsRegistration",
                table: "Stores",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LetterOfAuthorization",
                table: "Stores",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProofOfBusinessAddress",
                table: "Stores",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProofOfOwnership",
                table: "Stores",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "QualityCertificates",
                table: "Stores",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SignatureCircular",
                table: "Stores",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SocialSecurityRegistration",
                table: "Stores",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TaxRegistration",
                table: "Stores",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TradeRegistryGazette",
                table: "Stores",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TrademarkCertificate",
                table: "Stores",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MediaId",
                table: "ItemGallery",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivityCertificate",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "AuthorizedPersonId",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "BankAccountConfirmation",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "BankStatement",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "CertificateOfIncorporation",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "CustomsRegistration",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "LetterOfAuthorization",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "ProofOfBusinessAddress",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "ProofOfOwnership",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "QualityCertificates",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "SignatureCircular",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "SocialSecurityRegistration",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "TaxRegistration",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "TradeRegistryGazette",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "TrademarkCertificate",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "MediaId",
                table: "ItemGallery");

            migrationBuilder.AddColumn<string>(
                name: "ItemRoad",
                table: "ItemGallery",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
