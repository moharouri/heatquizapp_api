using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace heatquizappapi.Migrations
{
    /// <inheritdoc />
    public partial class finalstuff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KeyboardQuestionWrongAnswers_QuestionBase_KeyboardQuestionId",
                table: "KeyboardQuestionWrongAnswers");

            migrationBuilder.DropIndex(
                name: "IX_KeyboardQuestionWrongAnswers_KeyboardQuestionId",
                table: "KeyboardQuestionWrongAnswers");

            migrationBuilder.DropColumn(
                name: "KeyboardQuestionId",
                table: "KeyboardQuestionWrongAnswers");

            migrationBuilder.AddColumn<int>(
                name: "CommentSectionId",
                table: "QuestionBase",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QuestionId",
                table: "KeyboardQuestionWrongAnswers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "KeyboardVariableKeyVariationId",
                table: "AbastractKeyboardAnswerElements",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CourseMapKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MapId = table.Column<int>(type: "integer", nullable: false),
                    Key = table.Column<string>(type: "text", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseMapKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseMapKeys_CourseMap_MapId",
                        column: x => x.MapId,
                        principalTable: "CourseMap",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseMapKeys_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DatapoolNotificationSubscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastSeen = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    DatapoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatapoolNotificationSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DatapoolNotificationSubscriptions_DataPools_DatapoolId",
                        column: x => x.DatapoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DatapoolNotificationSubscriptions_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionCommentSection",
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
                    table.PrimaryKey("PK_QuestionCommentSection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionCommentSection_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionCommentSection_QuestionBase_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "QuestionBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLinkedPlayerKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PlayerKey = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLinkedPlayerKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLinkedPlayerKeys_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CommentSectionId = table.Column<int>(type: "integer", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false),
                    AddedById = table.Column<string>(type: "text", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionComments_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionComments_QuestionCommentSection_CommentSectionId",
                        column: x => x.CommentSectionId,
                        principalTable: "QuestionCommentSection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionComments_User_AddedById",
                        column: x => x.AddedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionCommentSectionTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SectionId = table.Column<int>(type: "integer", nullable: false),
                    AddedById = table.Column<string>(type: "text", nullable: false),
                    LastSeen = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionCommentSectionTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionCommentSectionTags_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionCommentSectionTags_QuestionCommentSection_SectionId",
                        column: x => x.SectionId,
                        principalTable: "QuestionCommentSection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionCommentSectionTags_User_AddedById",
                        column: x => x.AddedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionCommentTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CommentId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionCommentTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionCommentTags_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionCommentTags_QuestionComments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "QuestionComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionCommentTags_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KeyboardQuestionWrongAnswers_QuestionId",
                table: "KeyboardQuestionWrongAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_AbastractKeyboardAnswerElements_KeyboardVariableKeyVariatio~",
                table: "AbastractKeyboardAnswerElements",
                column: "KeyboardVariableKeyVariationId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMapKeys_DataPoolId",
                table: "CourseMapKeys",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMapKeys_MapId",
                table: "CourseMapKeys",
                column: "MapId");

            migrationBuilder.CreateIndex(
                name: "IX_DatapoolNotificationSubscriptions_DatapoolId",
                table: "DatapoolNotificationSubscriptions",
                column: "DatapoolId");

            migrationBuilder.CreateIndex(
                name: "IX_DatapoolNotificationSubscriptions_UserId",
                table: "DatapoolNotificationSubscriptions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionComments_AddedById",
                table: "QuestionComments",
                column: "AddedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionComments_CommentSectionId",
                table: "QuestionComments",
                column: "CommentSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionComments_DataPoolId",
                table: "QuestionComments",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionCommentSection_DataPoolId",
                table: "QuestionCommentSection",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionCommentSection_QuestionId",
                table: "QuestionCommentSection",
                column: "QuestionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionCommentSectionTags_AddedById",
                table: "QuestionCommentSectionTags",
                column: "AddedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionCommentSectionTags_DataPoolId",
                table: "QuestionCommentSectionTags",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionCommentSectionTags_SectionId",
                table: "QuestionCommentSectionTags",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionCommentTags_CommentId",
                table: "QuestionCommentTags",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionCommentTags_DataPoolId",
                table: "QuestionCommentTags",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionCommentTags_UserId",
                table: "QuestionCommentTags",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLinkedPlayerKeys_UserId",
                table: "UserLinkedPlayerKeys",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AbastractKeyboardAnswerElements_KeyboardVariableKeyVariatio~",
                table: "AbastractKeyboardAnswerElements",
                column: "KeyboardVariableKeyVariationId",
                principalTable: "KeyboardVariableKeyVariation",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_KeyboardQuestionWrongAnswers_QuestionBase_QuestionId",
                table: "KeyboardQuestionWrongAnswers",
                column: "QuestionId",
                principalTable: "QuestionBase",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbastractKeyboardAnswerElements_KeyboardVariableKeyVariatio~",
                table: "AbastractKeyboardAnswerElements");

            migrationBuilder.DropForeignKey(
                name: "FK_KeyboardQuestionWrongAnswers_QuestionBase_QuestionId",
                table: "KeyboardQuestionWrongAnswers");

            migrationBuilder.DropTable(
                name: "CourseMapKeys");

            migrationBuilder.DropTable(
                name: "DatapoolNotificationSubscriptions");

            migrationBuilder.DropTable(
                name: "QuestionCommentSectionTags");

            migrationBuilder.DropTable(
                name: "QuestionCommentTags");

            migrationBuilder.DropTable(
                name: "UserLinkedPlayerKeys");

            migrationBuilder.DropTable(
                name: "QuestionComments");

            migrationBuilder.DropTable(
                name: "QuestionCommentSection");

            migrationBuilder.DropIndex(
                name: "IX_KeyboardQuestionWrongAnswers_QuestionId",
                table: "KeyboardQuestionWrongAnswers");

            migrationBuilder.DropIndex(
                name: "IX_AbastractKeyboardAnswerElements_KeyboardVariableKeyVariatio~",
                table: "AbastractKeyboardAnswerElements");

            migrationBuilder.DropColumn(
                name: "CommentSectionId",
                table: "QuestionBase");

            migrationBuilder.DropColumn(
                name: "QuestionId",
                table: "KeyboardQuestionWrongAnswers");

            migrationBuilder.DropColumn(
                name: "KeyboardVariableKeyVariationId",
                table: "AbastractKeyboardAnswerElements");

            migrationBuilder.AddColumn<int>(
                name: "KeyboardQuestionId",
                table: "KeyboardQuestionWrongAnswers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_KeyboardQuestionWrongAnswers_KeyboardQuestionId",
                table: "KeyboardQuestionWrongAnswers",
                column: "KeyboardQuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_KeyboardQuestionWrongAnswers_QuestionBase_KeyboardQuestionId",
                table: "KeyboardQuestionWrongAnswers",
                column: "KeyboardQuestionId",
                principalTable: "QuestionBase",
                principalColumn: "Id");
        }
    }
}
