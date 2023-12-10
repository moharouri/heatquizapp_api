using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace heatquizappapi.Migrations
{
    /// <inheritdoc />
    public partial class mapthings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseMapElementBadge_CourseMapElement_CourseMapElementId",
                table: "CourseMapElementBadge");

            migrationBuilder.AddColumn<bool>(
                name: "DisableDivision",
                table: "QuestionBase",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnergyBalance",
                table: "QuestionBase",
                type: "boolean",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CourseMapElementId",
                table: "CourseMapElementBadge",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageURL",
                table: "CourseMapElementBadge",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Progress",
                table: "CourseMapElementBadge",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseMapElementBadge_CourseMapElement_CourseMapElementId",
                table: "CourseMapElementBadge",
                column: "CourseMapElementId",
                principalTable: "CourseMapElement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseMapElementBadge_CourseMapElement_CourseMapElementId",
                table: "CourseMapElementBadge");

            migrationBuilder.DropColumn(
                name: "DisableDivision",
                table: "QuestionBase");

            migrationBuilder.DropColumn(
                name: "IsEnergyBalance",
                table: "QuestionBase");

            migrationBuilder.DropColumn(
                name: "ImageURL",
                table: "CourseMapElementBadge");

            migrationBuilder.DropColumn(
                name: "Progress",
                table: "CourseMapElementBadge");

            migrationBuilder.AlterColumn<int>(
                name: "CourseMapElementId",
                table: "CourseMapElementBadge",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseMapElementBadge_CourseMapElement_CourseMapElementId",
                table: "CourseMapElementBadge",
                column: "CourseMapElementId",
                principalTable: "CourseMapElement",
                principalColumn: "Id");
        }
    }
}
