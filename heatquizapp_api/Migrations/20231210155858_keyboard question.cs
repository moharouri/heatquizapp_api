using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace heatquizappapi.Migrations
{
    /// <inheritdoc />
    public partial class keyboardquestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KeyboardQuestionAnswer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuestionId = table.Column<int>(type: "integer", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyboardQuestionAnswer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KeyboardQuestionAnswer_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KeyboardQuestionAnswer_QuestionBase_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "QuestionBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KeyboardQuestionWrongAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AnswerLatex = table.Column<string>(type: "text", nullable: false),
                    KeyboardQuestionId = table.Column<int>(type: "integer", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyboardQuestionWrongAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KeyboardQuestionWrongAnswers_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KeyboardQuestionWrongAnswers_QuestionBase_KeyboardQuestionId",
                        column: x => x.KeyboardQuestionId,
                        principalTable: "QuestionBase",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AbastractKeyboardAnswerElements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NumericKeyId = table.Column<int>(type: "integer", nullable: true),
                    ImageId = table.Column<int>(type: "integer", nullable: true),
                    Value = table.Column<string>(type: "text", nullable: false),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
                    AnswerId = table.Column<int>(type: "integer", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbastractKeyboardAnswerElements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbastractKeyboardAnswerElements_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AbastractKeyboardAnswerElements_KeyboardNumericKeyRelation_~",
                        column: x => x.NumericKeyId,
                        principalTable: "KeyboardNumericKeyRelation",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AbastractKeyboardAnswerElements_KeyboardQuestionAnswer_Answ~",
                        column: x => x.AnswerId,
                        principalTable: "KeyboardQuestionAnswer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AbastractKeyboardAnswerElements_KeyboardVariableKeyImageRel~",
                        column: x => x.ImageId,
                        principalTable: "KeyboardVariableKeyImageRelation",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AbastractKeyboardAnswerElements_AnswerId",
                table: "AbastractKeyboardAnswerElements",
                column: "AnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_AbastractKeyboardAnswerElements_DataPoolId",
                table: "AbastractKeyboardAnswerElements",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_AbastractKeyboardAnswerElements_ImageId",
                table: "AbastractKeyboardAnswerElements",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_AbastractKeyboardAnswerElements_NumericKeyId",
                table: "AbastractKeyboardAnswerElements",
                column: "NumericKeyId");

            migrationBuilder.CreateIndex(
                name: "IX_KeyboardQuestionAnswer_DataPoolId",
                table: "KeyboardQuestionAnswer",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_KeyboardQuestionAnswer_QuestionId",
                table: "KeyboardQuestionAnswer",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_KeyboardQuestionWrongAnswers_DataPoolId",
                table: "KeyboardQuestionWrongAnswers",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_KeyboardQuestionWrongAnswers_KeyboardQuestionId",
                table: "KeyboardQuestionWrongAnswers",
                column: "KeyboardQuestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AbastractKeyboardAnswerElements");

            migrationBuilder.DropTable(
                name: "KeyboardQuestionWrongAnswers");

            migrationBuilder.DropTable(
                name: "KeyboardQuestionAnswer");
        }
    }
}
