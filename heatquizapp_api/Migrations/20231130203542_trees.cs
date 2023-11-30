using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace heatquizappapi.Migrations
{
    /// <inheritdoc />
    public partial class trees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImageAnswerGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    AddedById = table.Column<string>(type: "text", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageAnswerGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImageAnswerGroups_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ImageAnswerGroups_User_AddedById",
                        column: x => x.AddedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InterpretedImageGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    AddedById = table.Column<string>(type: "text", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterpretedImageGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterpretedImageGroups_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterpretedImageGroups_User_AddedById",
                        column: x => x.AddedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JumpValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<string>(type: "text", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JumpValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JumpValues_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeftGradientValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<string>(type: "text", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeftGradientValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeftGradientValues_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RationOfGradientsValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<string>(type: "text", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RationOfGradientsValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RationOfGradientsValues_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RightGradientValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<string>(type: "text", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RightGradientValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RightGradientValues_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImageAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Choosable = table.Column<bool>(type: "boolean", nullable: false),
                    ImageURL = table.Column<string>(type: "text", nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    GroupId = table.Column<int>(type: "integer", nullable: false),
                    RootId = table.Column<int>(type: "integer", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImageAnswers_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ImageAnswers_ImageAnswerGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "ImageAnswerGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ImageAnswers_ImageAnswers_RootId",
                        column: x => x.RootId,
                        principalTable: "ImageAnswers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InterpretedImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: false),
                    LeftId = table.Column<int>(type: "integer", nullable: false),
                    RightId = table.Column<int>(type: "integer", nullable: false),
                    RationOfGradientsId = table.Column<int>(type: "integer", nullable: false),
                    JumpId = table.Column<int>(type: "integer", nullable: false),
                    ImageURL = table.Column<string>(type: "text", nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    GroupId = table.Column<int>(type: "integer", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterpretedImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterpretedImages_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterpretedImages_InterpretedImageGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "InterpretedImageGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterpretedImages_JumpValues_JumpId",
                        column: x => x.JumpId,
                        principalTable: "JumpValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterpretedImages_LeftGradientValues_LeftId",
                        column: x => x.LeftId,
                        principalTable: "LeftGradientValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterpretedImages_RationOfGradientsValues_RationOfGradients~",
                        column: x => x.RationOfGradientsId,
                        principalTable: "RationOfGradientsValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterpretedImages_RightGradientValues_RightId",
                        column: x => x.RightId,
                        principalTable: "RightGradientValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImageAnswerGroups_AddedById",
                table: "ImageAnswerGroups",
                column: "AddedById");

            migrationBuilder.CreateIndex(
                name: "IX_ImageAnswerGroups_DataPoolId",
                table: "ImageAnswerGroups",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageAnswers_DataPoolId",
                table: "ImageAnswers",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageAnswers_GroupId",
                table: "ImageAnswers",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageAnswers_RootId",
                table: "ImageAnswers",
                column: "RootId");

            migrationBuilder.CreateIndex(
                name: "IX_InterpretedImageGroups_AddedById",
                table: "InterpretedImageGroups",
                column: "AddedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterpretedImageGroups_DataPoolId",
                table: "InterpretedImageGroups",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_InterpretedImages_DataPoolId",
                table: "InterpretedImages",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_InterpretedImages_GroupId",
                table: "InterpretedImages",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_InterpretedImages_JumpId",
                table: "InterpretedImages",
                column: "JumpId");

            migrationBuilder.CreateIndex(
                name: "IX_InterpretedImages_LeftId",
                table: "InterpretedImages",
                column: "LeftId");

            migrationBuilder.CreateIndex(
                name: "IX_InterpretedImages_RationOfGradientsId",
                table: "InterpretedImages",
                column: "RationOfGradientsId");

            migrationBuilder.CreateIndex(
                name: "IX_InterpretedImages_RightId",
                table: "InterpretedImages",
                column: "RightId");

            migrationBuilder.CreateIndex(
                name: "IX_JumpValues_DataPoolId",
                table: "JumpValues",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_LeftGradientValues_DataPoolId",
                table: "LeftGradientValues",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_RationOfGradientsValues_DataPoolId",
                table: "RationOfGradientsValues",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_RightGradientValues_DataPoolId",
                table: "RightGradientValues",
                column: "DataPoolId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageAnswers");

            migrationBuilder.DropTable(
                name: "InterpretedImages");

            migrationBuilder.DropTable(
                name: "ImageAnswerGroups");

            migrationBuilder.DropTable(
                name: "InterpretedImageGroups");

            migrationBuilder.DropTable(
                name: "JumpValues");

            migrationBuilder.DropTable(
                name: "LeftGradientValues");

            migrationBuilder.DropTable(
                name: "RationOfGradientsValues");

            migrationBuilder.DropTable(
                name: "RightGradientValues");
        }
    }
}
