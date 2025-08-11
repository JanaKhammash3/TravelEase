using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelEase.TravelEase.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDecimalPrecisionFixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DiscountedPrice",
                table: "Hotels",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "Hotels",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "OriginalPrice",
                table: "Hotels",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailUrl",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountedPrice",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "OriginalPrice",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "ThumbnailUrl",
                table: "Hotels");
        }
    }
}
