using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace heatquizappapi.Migrations
{
    /// <inheritdoc />
    public partial class Numerickeyremovekeysimpleform : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KeySimpleForm",
                table: "NumericKeys");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KeySimpleForm",
                table: "NumericKeys",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
