using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace heatquizappapi.Migrations
{
    /// <inheritdoc />
    public partial class newtables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KeyboardId",
                table: "QuestionBase",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CourseMap",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CourseId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    ShowBorder = table.Column<bool>(type: "boolean", nullable: false),
                    ShowSolutions = table.Column<bool>(type: "boolean", nullable: false),
                    Disabled = table.Column<bool>(type: "boolean", nullable: false),
                    ImageURL = table.Column<string>(type: "text", nullable: false),
                    ImageWidth = table.Column<int>(type: "integer", nullable: false),
                    ImageHeight = table.Column<int>(type: "integer", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseMap", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseMap_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseMap_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseMapElementImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AddedById = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Play = table.Column<string>(type: "text", nullable: false),
                    PDF = table.Column<string>(type: "text", nullable: false),
                    Video = table.Column<string>(type: "text", nullable: false),
                    Link = table.Column<string>(type: "text", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseMapElementImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseMapElementImages_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseMapElementImages_User_AddedById",
                        column: x => x.AddedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DefaultQuestionImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: false),
                    AddedById = table.Column<string>(type: "text", nullable: false),
                    ImageURL = table.Column<string>(type: "text", nullable: false),
                    ImageSize = table.Column<long>(type: "bigint", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultQuestionImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DefaultQuestionImages_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DefaultQuestionImages_User_AddedById",
                        column: x => x.AddedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Keyboards",
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
                    table.PrimaryKey("PK_Keyboards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Keyboards_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Keyboards_User_AddedById",
                        column: x => x.AddedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KeysLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: false),
                    AddedById = table.Column<string>(type: "text", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeysLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KeysLists_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KeysLists_User_AddedById",
                        column: x => x.AddedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MultipleChoiceQuestionChoice",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuestionId = table.Column<int>(type: "integer", nullable: false),
                    Latex = table.Column<string>(type: "text", nullable: false),
                    ImageURL = table.Column<string>(type: "text", nullable: false),
                    Correct = table.Column<bool>(type: "boolean", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MultipleChoiceQuestionChoice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MultipleChoiceQuestionChoice_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MultipleChoiceQuestionChoice_QuestionBase_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "QuestionBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseMapBadgeSystem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    MapId = table.Column<int>(type: "integer", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseMapBadgeSystem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseMapBadgeSystem_CourseMap_MapId",
                        column: x => x.MapId,
                        principalTable: "CourseMap",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseMapBadgeSystem_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseMapElement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MapId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    X = table.Column<int>(type: "integer", nullable: false),
                    Y = table.Column<int>(type: "integer", nullable: false),
                    Width = table.Column<int>(type: "integer", nullable: false),
                    Length = table.Column<int>(type: "integer", nullable: false),
                    BadgeX = table.Column<int>(type: "integer", nullable: false),
                    BadgeY = table.Column<int>(type: "integer", nullable: false),
                    BadgeWidth = table.Column<int>(type: "integer", nullable: false),
                    BadgeLength = table.Column<int>(type: "integer", nullable: false),
                    ExternalVideoLink = table.Column<string>(type: "text", nullable: false),
                    MapAttachmentId = table.Column<int>(type: "integer", nullable: true),
                    PDFURL = table.Column<string>(type: "text", nullable: false),
                    PDFSize = table.Column<long>(type: "bigint", nullable: false),
                    QuestionSeriesId = table.Column<int>(type: "integer", nullable: true),
                    RequiredElementId = table.Column<int>(type: "integer", nullable: true),
                    Threshold = table.Column<int>(type: "integer", nullable: false),
                    CourseMapElementImagesId = table.Column<int>(type: "integer", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseMapElement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseMapElement_CourseMapElementImages_CourseMapElementIma~",
                        column: x => x.CourseMapElementImagesId,
                        principalTable: "CourseMapElementImages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CourseMapElement_CourseMapElement_RequiredElementId",
                        column: x => x.RequiredElementId,
                        principalTable: "CourseMapElement",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CourseMapElement_CourseMap_MapId",
                        column: x => x.MapId,
                        principalTable: "CourseMap",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseMapElement_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseMapElement_QuestionSeries_QuestionSeriesId",
                        column: x => x.QuestionSeriesId,
                        principalTable: "QuestionSeries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NumericKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: false),
                    TextPresentation = table.Column<string>(type: "text", nullable: false),
                    IsInteger = table.Column<bool>(type: "boolean", nullable: false),
                    KeySimpleForm = table.Column<string>(type: "text", nullable: false),
                    KeysListId = table.Column<int>(type: "integer", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NumericKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NumericKeys_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NumericKeys_KeysLists_KeysListId",
                        column: x => x.KeysListId,
                        principalTable: "KeysLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VariableKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: false),
                    TextPresentation = table.Column<string>(type: "text", nullable: false),
                    KeysListId = table.Column<int>(type: "integer", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VariableKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VariableKeys_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VariableKeys_KeysLists_KeysListId",
                        column: x => x.KeysListId,
                        principalTable: "KeysLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseMapBadgeSystemEntity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SystemId = table.Column<int>(type: "integer", nullable: false),
                    ImageURL = table.Column<string>(type: "text", nullable: false),
                    ImageSize = table.Column<long>(type: "bigint", nullable: false),
                    Progress = table.Column<int>(type: "integer", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseMapBadgeSystemEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseMapBadgeSystemEntity_CourseMapBadgeSystem_SystemId",
                        column: x => x.SystemId,
                        principalTable: "CourseMapBadgeSystem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseMapBadgeSystemEntity_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseMapElementBadge",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CourseMapElementId = table.Column<int>(type: "integer", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseMapElementBadge", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseMapElementBadge_CourseMapElement_CourseMapElementId",
                        column: x => x.CourseMapElementId,
                        principalTable: "CourseMapElement",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CourseMapElementBadge_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseMapPDFStatistics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ElementId = table.Column<int>(type: "integer", nullable: false),
                    Player = table.Column<string>(type: "text", nullable: false),
                    OnMobile = table.Column<bool>(type: "boolean", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseMapPDFStatistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseMapPDFStatistics_CourseMapElement_ElementId",
                        column: x => x.ElementId,
                        principalTable: "CourseMapElement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseMapPDFStatistics_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MapElementLink",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ElementId = table.Column<int>(type: "integer", nullable: false),
                    MapId = table.Column<int>(type: "integer", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapElementLink", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MapElementLink_CourseMapElement_ElementId",
                        column: x => x.ElementId,
                        principalTable: "CourseMapElement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MapElementLink_CourseMap_MapId",
                        column: x => x.MapId,
                        principalTable: "CourseMap",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MapElementLink_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KeyboardNumericKeyRelation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KeyboardId = table.Column<int>(type: "integer", nullable: false),
                    NumericKeyId = table.Column<int>(type: "integer", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    KeySimpleForm = table.Column<string>(type: "text", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyboardNumericKeyRelation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KeyboardNumericKeyRelation_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KeyboardNumericKeyRelation_Keyboards_KeyboardId",
                        column: x => x.KeyboardId,
                        principalTable: "Keyboards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KeyboardNumericKeyRelation_NumericKeys_NumericKeyId",
                        column: x => x.NumericKeyId,
                        principalTable: "NumericKeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KeyboardVariableKeyRelation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KeyboardId = table.Column<int>(type: "integer", nullable: false),
                    VariableKeyId = table.Column<int>(type: "integer", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyboardVariableKeyRelation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KeyboardVariableKeyRelation_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KeyboardVariableKeyRelation_Keyboards_KeyboardId",
                        column: x => x.KeyboardId,
                        principalTable: "Keyboards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KeyboardVariableKeyRelation_VariableKeys_VariableKeyId",
                        column: x => x.VariableKeyId,
                        principalTable: "VariableKeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KeyboardVariableKeyVariation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KeyId = table.Column<int>(type: "integer", nullable: false),
                    TextPresentation = table.Column<string>(type: "text", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyboardVariableKeyVariation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KeyboardVariableKeyVariation_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KeyboardVariableKeyVariation_VariableKeys_KeyId",
                        column: x => x.KeyId,
                        principalTable: "VariableKeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KeyboardVariableKeyImageRelation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VariationId = table.Column<int>(type: "integer", nullable: false),
                    KeyboardId = table.Column<int>(type: "integer", nullable: false),
                    ReplacementCharacter = table.Column<string>(type: "text", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyboardVariableKeyImageRelation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KeyboardVariableKeyImageRelation_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KeyboardVariableKeyImageRelation_KeyboardVariableKeyVariati~",
                        column: x => x.VariationId,
                        principalTable: "KeyboardVariableKeyVariation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KeyboardVariableKeyImageRelation_Keyboards_KeyboardId",
                        column: x => x.KeyboardId,
                        principalTable: "Keyboards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBase_KeyboardId",
                table: "QuestionBase",
                column: "KeyboardId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMap_CourseId",
                table: "CourseMap",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMap_DataPoolId",
                table: "CourseMap",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMapBadgeSystem_DataPoolId",
                table: "CourseMapBadgeSystem",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMapBadgeSystem_MapId",
                table: "CourseMapBadgeSystem",
                column: "MapId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMapBadgeSystemEntity_DataPoolId",
                table: "CourseMapBadgeSystemEntity",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMapBadgeSystemEntity_SystemId",
                table: "CourseMapBadgeSystemEntity",
                column: "SystemId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMapElement_CourseMapElementImagesId",
                table: "CourseMapElement",
                column: "CourseMapElementImagesId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMapElement_DataPoolId",
                table: "CourseMapElement",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMapElement_MapId",
                table: "CourseMapElement",
                column: "MapId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMapElement_QuestionSeriesId",
                table: "CourseMapElement",
                column: "QuestionSeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMapElement_RequiredElementId",
                table: "CourseMapElement",
                column: "RequiredElementId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMapElementBadge_CourseMapElementId",
                table: "CourseMapElementBadge",
                column: "CourseMapElementId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMapElementBadge_DataPoolId",
                table: "CourseMapElementBadge",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMapElementImages_AddedById",
                table: "CourseMapElementImages",
                column: "AddedById");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMapElementImages_DataPoolId",
                table: "CourseMapElementImages",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMapPDFStatistics_DataPoolId",
                table: "CourseMapPDFStatistics",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMapPDFStatistics_ElementId",
                table: "CourseMapPDFStatistics",
                column: "ElementId");

            migrationBuilder.CreateIndex(
                name: "IX_DefaultQuestionImages_AddedById",
                table: "DefaultQuestionImages",
                column: "AddedById");

            migrationBuilder.CreateIndex(
                name: "IX_DefaultQuestionImages_DataPoolId",
                table: "DefaultQuestionImages",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_KeyboardNumericKeyRelation_DataPoolId",
                table: "KeyboardNumericKeyRelation",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_KeyboardNumericKeyRelation_KeyboardId",
                table: "KeyboardNumericKeyRelation",
                column: "KeyboardId");

            migrationBuilder.CreateIndex(
                name: "IX_KeyboardNumericKeyRelation_NumericKeyId",
                table: "KeyboardNumericKeyRelation",
                column: "NumericKeyId");

            migrationBuilder.CreateIndex(
                name: "IX_Keyboards_AddedById",
                table: "Keyboards",
                column: "AddedById");

            migrationBuilder.CreateIndex(
                name: "IX_Keyboards_DataPoolId",
                table: "Keyboards",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_KeyboardVariableKeyImageRelation_DataPoolId",
                table: "KeyboardVariableKeyImageRelation",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_KeyboardVariableKeyImageRelation_KeyboardId",
                table: "KeyboardVariableKeyImageRelation",
                column: "KeyboardId");

            migrationBuilder.CreateIndex(
                name: "IX_KeyboardVariableKeyImageRelation_VariationId",
                table: "KeyboardVariableKeyImageRelation",
                column: "VariationId");

            migrationBuilder.CreateIndex(
                name: "IX_KeyboardVariableKeyRelation_DataPoolId",
                table: "KeyboardVariableKeyRelation",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_KeyboardVariableKeyRelation_KeyboardId",
                table: "KeyboardVariableKeyRelation",
                column: "KeyboardId");

            migrationBuilder.CreateIndex(
                name: "IX_KeyboardVariableKeyRelation_VariableKeyId",
                table: "KeyboardVariableKeyRelation",
                column: "VariableKeyId");

            migrationBuilder.CreateIndex(
                name: "IX_KeyboardVariableKeyVariation_DataPoolId",
                table: "KeyboardVariableKeyVariation",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_KeyboardVariableKeyVariation_KeyId",
                table: "KeyboardVariableKeyVariation",
                column: "KeyId");

            migrationBuilder.CreateIndex(
                name: "IX_KeysLists_AddedById",
                table: "KeysLists",
                column: "AddedById");

            migrationBuilder.CreateIndex(
                name: "IX_KeysLists_DataPoolId",
                table: "KeysLists",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_MapElementLink_DataPoolId",
                table: "MapElementLink",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_MapElementLink_ElementId",
                table: "MapElementLink",
                column: "ElementId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MapElementLink_MapId",
                table: "MapElementLink",
                column: "MapId");

            migrationBuilder.CreateIndex(
                name: "IX_MultipleChoiceQuestionChoice_DataPoolId",
                table: "MultipleChoiceQuestionChoice",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_MultipleChoiceQuestionChoice_QuestionId",
                table: "MultipleChoiceQuestionChoice",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_NumericKeys_DataPoolId",
                table: "NumericKeys",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_NumericKeys_KeysListId",
                table: "NumericKeys",
                column: "KeysListId");

            migrationBuilder.CreateIndex(
                name: "IX_VariableKeys_DataPoolId",
                table: "VariableKeys",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_VariableKeys_KeysListId",
                table: "VariableKeys",
                column: "KeysListId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionBase_Keyboards_KeyboardId",
                table: "QuestionBase",
                column: "KeyboardId",
                principalTable: "Keyboards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionBase_Keyboards_KeyboardId",
                table: "QuestionBase");

            migrationBuilder.DropTable(
                name: "CourseMapBadgeSystemEntity");

            migrationBuilder.DropTable(
                name: "CourseMapElementBadge");

            migrationBuilder.DropTable(
                name: "CourseMapPDFStatistics");

            migrationBuilder.DropTable(
                name: "DefaultQuestionImages");

            migrationBuilder.DropTable(
                name: "KeyboardNumericKeyRelation");

            migrationBuilder.DropTable(
                name: "KeyboardVariableKeyImageRelation");

            migrationBuilder.DropTable(
                name: "KeyboardVariableKeyRelation");

            migrationBuilder.DropTable(
                name: "MapElementLink");

            migrationBuilder.DropTable(
                name: "MultipleChoiceQuestionChoice");

            migrationBuilder.DropTable(
                name: "CourseMapBadgeSystem");

            migrationBuilder.DropTable(
                name: "NumericKeys");

            migrationBuilder.DropTable(
                name: "KeyboardVariableKeyVariation");

            migrationBuilder.DropTable(
                name: "Keyboards");

            migrationBuilder.DropTable(
                name: "CourseMapElement");

            migrationBuilder.DropTable(
                name: "VariableKeys");

            migrationBuilder.DropTable(
                name: "CourseMapElementImages");

            migrationBuilder.DropTable(
                name: "CourseMap");

            migrationBuilder.DropTable(
                name: "KeysLists");

            migrationBuilder.DropIndex(
                name: "IX_QuestionBase_KeyboardId",
                table: "QuestionBase");

            migrationBuilder.DropColumn(
                name: "KeyboardId",
                table: "QuestionBase");
        }
    }
}
