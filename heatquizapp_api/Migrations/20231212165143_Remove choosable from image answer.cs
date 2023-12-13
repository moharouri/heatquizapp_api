using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace heatquizappapi.Migrations
{
    /// <inheritdoc />
    public partial class Removechoosablefromimageanswer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Choosable",
                table: "ImageAnswers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Choosable",
                table: "ImageAnswers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
