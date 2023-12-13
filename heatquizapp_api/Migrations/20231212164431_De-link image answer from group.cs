using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace heatquizappapi.Migrations
{
    /// <inheritdoc />
    public partial class Delinkimageanswerfromgroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageAnswers_ImageAnswerGroups_GroupId",
                table: "ImageAnswers");

            migrationBuilder.AlterColumn<int>(
                name: "GroupId",
                table: "ImageAnswers",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageAnswers_ImageAnswerGroups_GroupId",
                table: "ImageAnswers",
                column: "GroupId",
                principalTable: "ImageAnswerGroups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageAnswers_ImageAnswerGroups_GroupId",
                table: "ImageAnswers");

            migrationBuilder.AlterColumn<int>(
                name: "GroupId",
                table: "ImageAnswers",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ImageAnswers_ImageAnswerGroups_GroupId",
                table: "ImageAnswers",
                column: "GroupId",
                principalTable: "ImageAnswerGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
