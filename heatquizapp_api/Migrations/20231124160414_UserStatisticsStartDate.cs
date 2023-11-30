using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace heatquizappapi.Migrations
{
    /// <inheritdoc />
    public partial class UserStatisticsStartDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "StatisticsStartDate",
                table: "User",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsHidden",
                table: "DataPools",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatisticsStartDate",
                table: "User");

            migrationBuilder.AlterColumn<bool>(
                name: "IsHidden",
                table: "DataPools",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");
        }
    }
}
