using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace heatquizappapi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataPools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NickName = table.Column<string>(type: "text", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataPools", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LevelsOfDifficulty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    HexColor = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LevelsOfDifficulty", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    RegisteredOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    ProfilePicture = table.Column<string>(type: "text", nullable: true),
                    StatisticsStartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SaveStatistics = table.Column<bool>(type: "boolean", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
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
                name: "RoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
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
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    ImageURL = table.Column<string>(type: "text", nullable: false),
                    ImageSize = table.Column<long>(type: "bigint", nullable: false),
                    AddedById = table.Column<string>(type: "text", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courses_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Courses_User_AddedById",
                        column: x => x.AddedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DataPoolAccess",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataPoolAccess", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataPoolAccess_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DataPoolAccess_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
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
                name: "Information",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false),
                    AddedById = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Latex = table.Column<string>(type: "text", nullable: true),
                    PDFURL = table.Column<string>(type: "text", nullable: true),
                    PDFSize = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Information", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Information_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Information_User_AddedById",
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
                name: "QuestionSeries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: false),
                    AddedById = table.Column<string>(type: "text", nullable: false),
                    IsRandom = table.Column<bool>(type: "boolean", nullable: false),
                    RandomSize = table.Column<int>(type: "integer", nullable: false),
                    NumberOfPools = table.Column<int>(type: "integer", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionSeries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionSeries_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionSeries_User_AddedById",
                        column: x => x.AddedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Topics",
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
                    table.PrimaryKey("PK_Topics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Topics_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Topics_User_AddedById",
                        column: x => x.AddedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
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
                name: "UserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogins_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserTokens_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                    ImageSize = table.Column<long>(type: "bigint", nullable: false),
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
                name: "ImageAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ImageURL = table.Column<string>(type: "text", nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    GroupId = table.Column<int>(type: "integer", nullable: true),
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
                        principalColumn: "Id");
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

            migrationBuilder.CreateTable(
                name: "NumericKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: false),
                    TextPresentation = table.Column<string>(type: "text", nullable: false),
                    IsInteger = table.Column<bool>(type: "boolean", nullable: false),
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
                name: "QuestionSeriesStatistic",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SeriesId = table.Column<int>(type: "integer", nullable: false),
                    Player = table.Column<string>(type: "text", nullable: false),
                    MapKey = table.Column<string>(type: "text", nullable: true),
                    MapName = table.Column<string>(type: "text", nullable: true),
                    MapElementName = table.Column<string>(type: "text", nullable: true),
                    SuccessRate = table.Column<string>(type: "text", nullable: false),
                    TotalTime = table.Column<int>(type: "integer", nullable: false),
                    OnMobile = table.Column<bool>(type: "boolean", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionSeriesStatistic", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionSeriesStatistic_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionSeriesStatistic_QuestionSeries_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "QuestionSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subtopics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    TopicId = table.Column<int>(type: "integer", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subtopics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subtopics_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subtopics_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
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
                    ExternalVideoLink = table.Column<string>(type: "text", nullable: true),
                    MapAttachmentId = table.Column<int>(type: "integer", nullable: true),
                    PDFURL = table.Column<string>(type: "text", nullable: true),
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
                name: "QuestionBase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    AddedById = table.Column<string>(type: "text", nullable: false),
                    LevelOfDifficultyId = table.Column<int>(type: "integer", nullable: false),
                    SubtopicId = table.Column<int>(type: "integer", nullable: false),
                    ImageURL = table.Column<string>(type: "text", nullable: false),
                    ImageSize = table.Column<long>(type: "bigint", nullable: false),
                    ImageWidth = table.Column<int>(type: "integer", nullable: false),
                    ImageHeight = table.Column<int>(type: "integer", nullable: false),
                    Latex = table.Column<string>(type: "text", nullable: true),
                    PDFURL = table.Column<string>(type: "text", nullable: true),
                    PDFSize = table.Column<long>(type: "bigint", nullable: true),
                    InformationId = table.Column<int>(type: "integer", nullable: true),
                    CommentSectionId = table.Column<int>(type: "integer", nullable: true),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
                    IsEnergyBalance = table.Column<bool>(type: "boolean", nullable: true),
                    DisableDivision = table.Column<bool>(type: "boolean", nullable: true),
                    KeyboardId = table.Column<int>(type: "integer", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionBase", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionBase_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionBase_Information_InformationId",
                        column: x => x.InformationId,
                        principalTable: "Information",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QuestionBase_Keyboards_KeyboardId",
                        column: x => x.KeyboardId,
                        principalTable: "Keyboards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionBase_LevelsOfDifficulty_LevelOfDifficultyId",
                        column: x => x.LevelOfDifficultyId,
                        principalTable: "LevelsOfDifficulty",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionBase_Subtopics_SubtopicId",
                        column: x => x.SubtopicId,
                        principalTable: "Subtopics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionBase_User_AddedById",
                        column: x => x.AddedById,
                        principalTable: "User",
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
                    CourseMapElementId = table.Column<int>(type: "integer", nullable: false),
                    Progress = table.Column<int>(type: "integer", nullable: false),
                    ImageURL = table.Column<string>(type: "text", nullable: false),
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateTable(
                name: "ClickChart",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AnswerId = table.Column<int>(type: "integer", nullable: false),
                    QuestionId = table.Column<int>(type: "integer", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false),
                    X = table.Column<int>(type: "integer", nullable: false),
                    Y = table.Column<int>(type: "integer", nullable: false),
                    Width = table.Column<int>(type: "integer", nullable: false),
                    Height = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClickChart", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClickChart_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClickChart_InterpretedImages_AnswerId",
                        column: x => x.AnswerId,
                        principalTable: "InterpretedImages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClickChart_QuestionBase_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "QuestionBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClickImage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AnswerId = table.Column<int>(type: "integer", nullable: false),
                    QuestionId = table.Column<int>(type: "integer", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false),
                    X = table.Column<int>(type: "integer", nullable: false),
                    Y = table.Column<int>(type: "integer", nullable: false),
                    Width = table.Column<int>(type: "integer", nullable: false),
                    Height = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClickImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClickImage_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClickImage_ImageAnswers_AnswerId",
                        column: x => x.AnswerId,
                        principalTable: "ImageAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClickImage_QuestionBase_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "QuestionBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                    QuestionId = table.Column<int>(type: "integer", nullable: false),
                    AnswerLatex = table.Column<string>(type: "text", nullable: false),
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
                        name: "FK_KeyboardQuestionWrongAnswers_QuestionBase_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "QuestionBase",
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
                    Latex = table.Column<string>(type: "text", nullable: true),
                    ImageURL = table.Column<string>(type: "text", nullable: true),
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
                name: "QuestionPDFStatistic",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuestionId = table.Column<int>(type: "integer", nullable: false),
                    Player = table.Column<string>(type: "text", nullable: false),
                    Correct = table.Column<bool>(type: "boolean", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionPDFStatistic", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionPDFStatistic_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionPDFStatistic_QuestionBase_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "QuestionBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionSeriesElement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SeriesId = table.Column<int>(type: "integer", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    QuestionId = table.Column<int>(type: "integer", nullable: false),
                    PoolNumber = table.Column<int>(type: "integer", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionSeriesElement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionSeriesElement_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionSeriesElement_QuestionBase_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "QuestionBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionSeriesElement_QuestionSeries_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "QuestionSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionStatistic",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuestionId = table.Column<int>(type: "integer", nullable: false),
                    Score = table.Column<string>(type: "text", nullable: false),
                    TotalTime = table.Column<int>(type: "integer", nullable: false),
                    Correct = table.Column<bool>(type: "boolean", nullable: false),
                    Key = table.Column<string>(type: "text", nullable: true),
                    Player = table.Column<string>(type: "text", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionStatistic", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionStatistic_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionStatistic_QuestionBase_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "QuestionBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionStudentFeedback",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuestionId = table.Column<int>(type: "integer", nullable: false),
                    Player = table.Column<string>(type: "text", nullable: false),
                    FeedbackContent = table.Column<string>(type: "text", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionStudentFeedback", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionStudentFeedback_DataPools_DataPoolId",
                        column: x => x.DataPoolId,
                        principalTable: "DataPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionStudentFeedback_QuestionBase_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "QuestionBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    KeyboardVariableKeyVariationId = table.Column<int>(type: "integer", nullable: true),
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
                    table.ForeignKey(
                        name: "FK_AbastractKeyboardAnswerElements_KeyboardVariableKeyVariatio~",
                        column: x => x.KeyboardVariableKeyVariationId,
                        principalTable: "KeyboardVariableKeyVariation",
                        principalColumn: "Id");
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
                name: "IX_AbastractKeyboardAnswerElements_KeyboardVariableKeyVariatio~",
                table: "AbastractKeyboardAnswerElements",
                column: "KeyboardVariableKeyVariationId");

            migrationBuilder.CreateIndex(
                name: "IX_AbastractKeyboardAnswerElements_NumericKeyId",
                table: "AbastractKeyboardAnswerElements",
                column: "NumericKeyId");

            migrationBuilder.CreateIndex(
                name: "IX_ClickChart_AnswerId",
                table: "ClickChart",
                column: "AnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_ClickChart_DataPoolId",
                table: "ClickChart",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_ClickChart_QuestionId",
                table: "ClickChart",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ClickImage_AnswerId",
                table: "ClickImage",
                column: "AnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_ClickImage_DataPoolId",
                table: "ClickImage",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_ClickImage_QuestionId",
                table: "ClickImage",
                column: "QuestionId");

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
                name: "IX_CourseMapKeys_DataPoolId",
                table: "CourseMapKeys",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMapKeys_MapId",
                table: "CourseMapKeys",
                column: "MapId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMapPDFStatistics_DataPoolId",
                table: "CourseMapPDFStatistics",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMapPDFStatistics_ElementId",
                table: "CourseMapPDFStatistics",
                column: "ElementId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_AddedById",
                table: "Courses",
                column: "AddedById");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_DataPoolId",
                table: "Courses",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_DataPoolAccess_DataPoolId",
                table: "DataPoolAccess",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_DataPoolAccess_UserId",
                table: "DataPoolAccess",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DatapoolNotificationSubscriptions_DatapoolId",
                table: "DatapoolNotificationSubscriptions",
                column: "DatapoolId");

            migrationBuilder.CreateIndex(
                name: "IX_DatapoolNotificationSubscriptions_UserId",
                table: "DatapoolNotificationSubscriptions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DefaultQuestionImages_AddedById",
                table: "DefaultQuestionImages",
                column: "AddedById");

            migrationBuilder.CreateIndex(
                name: "IX_DefaultQuestionImages_DataPoolId",
                table: "DefaultQuestionImages",
                column: "DataPoolId");

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
                name: "IX_Information_AddedById",
                table: "Information",
                column: "AddedById");

            migrationBuilder.CreateIndex(
                name: "IX_Information_DataPoolId",
                table: "Information",
                column: "DataPoolId");

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
                name: "IX_KeyboardQuestionWrongAnswers_QuestionId",
                table: "KeyboardQuestionWrongAnswers",
                column: "QuestionId");

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
                name: "IX_LeftGradientValues_DataPoolId",
                table: "LeftGradientValues",
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
                name: "IX_QuestionBase_AddedById",
                table: "QuestionBase",
                column: "AddedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBase_DataPoolId",
                table: "QuestionBase",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBase_InformationId",
                table: "QuestionBase",
                column: "InformationId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBase_KeyboardId",
                table: "QuestionBase",
                column: "KeyboardId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBase_LevelOfDifficultyId",
                table: "QuestionBase",
                column: "LevelOfDifficultyId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBase_SubtopicId",
                table: "QuestionBase",
                column: "SubtopicId");

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
                name: "IX_QuestionPDFStatistic_DataPoolId",
                table: "QuestionPDFStatistic",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionPDFStatistic_QuestionId",
                table: "QuestionPDFStatistic",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSeries_AddedById",
                table: "QuestionSeries",
                column: "AddedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSeries_DataPoolId",
                table: "QuestionSeries",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSeriesElement_DataPoolId",
                table: "QuestionSeriesElement",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSeriesElement_QuestionId",
                table: "QuestionSeriesElement",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSeriesElement_SeriesId",
                table: "QuestionSeriesElement",
                column: "SeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSeriesStatistic_DataPoolId",
                table: "QuestionSeriesStatistic",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSeriesStatistic_SeriesId",
                table: "QuestionSeriesStatistic",
                column: "SeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionStatistic_DataPoolId",
                table: "QuestionStatistic",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionStatistic_QuestionId",
                table: "QuestionStatistic",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionStudentFeedback_DataPoolId",
                table: "QuestionStudentFeedback",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionStudentFeedback_QuestionId",
                table: "QuestionStudentFeedback",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_RationOfGradientsValues_DataPoolId",
                table: "RationOfGradientsValues",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_RightGradientValues_DataPoolId",
                table: "RightGradientValues",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Role",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Subtopics_DataPoolId",
                table: "Subtopics",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_Subtopics_TopicId",
                table: "Subtopics",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_AddedById",
                table: "Topics",
                column: "AddedById");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_DataPoolId",
                table: "Topics",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "User",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "User",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLinkedPlayerKeys_UserId",
                table: "UserLinkedPlayerKeys",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_VariableKeys_DataPoolId",
                table: "VariableKeys",
                column: "DataPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_VariableKeys_KeysListId",
                table: "VariableKeys",
                column: "KeysListId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AbastractKeyboardAnswerElements");

            migrationBuilder.DropTable(
                name: "ClickChart");

            migrationBuilder.DropTable(
                name: "ClickImage");

            migrationBuilder.DropTable(
                name: "CourseMapBadgeSystemEntity");

            migrationBuilder.DropTable(
                name: "CourseMapElementBadge");

            migrationBuilder.DropTable(
                name: "CourseMapKeys");

            migrationBuilder.DropTable(
                name: "CourseMapPDFStatistics");

            migrationBuilder.DropTable(
                name: "DataPoolAccess");

            migrationBuilder.DropTable(
                name: "DatapoolNotificationSubscriptions");

            migrationBuilder.DropTable(
                name: "DefaultQuestionImages");

            migrationBuilder.DropTable(
                name: "KeyboardQuestionWrongAnswers");

            migrationBuilder.DropTable(
                name: "KeyboardVariableKeyRelation");

            migrationBuilder.DropTable(
                name: "MapElementLink");

            migrationBuilder.DropTable(
                name: "MultipleChoiceQuestionChoice");

            migrationBuilder.DropTable(
                name: "QuestionCommentSectionTags");

            migrationBuilder.DropTable(
                name: "QuestionCommentTags");

            migrationBuilder.DropTable(
                name: "QuestionPDFStatistic");

            migrationBuilder.DropTable(
                name: "QuestionSeriesElement");

            migrationBuilder.DropTable(
                name: "QuestionSeriesStatistic");

            migrationBuilder.DropTable(
                name: "QuestionStatistic");

            migrationBuilder.DropTable(
                name: "QuestionStudentFeedback");

            migrationBuilder.DropTable(
                name: "RoleClaims");

            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "UserLinkedPlayerKeys");

            migrationBuilder.DropTable(
                name: "UserLogins");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserTokens");

            migrationBuilder.DropTable(
                name: "KeyboardNumericKeyRelation");

            migrationBuilder.DropTable(
                name: "KeyboardQuestionAnswer");

            migrationBuilder.DropTable(
                name: "KeyboardVariableKeyImageRelation");

            migrationBuilder.DropTable(
                name: "InterpretedImages");

            migrationBuilder.DropTable(
                name: "ImageAnswers");

            migrationBuilder.DropTable(
                name: "CourseMapBadgeSystem");

            migrationBuilder.DropTable(
                name: "CourseMapElement");

            migrationBuilder.DropTable(
                name: "QuestionComments");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "NumericKeys");

            migrationBuilder.DropTable(
                name: "KeyboardVariableKeyVariation");

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

            migrationBuilder.DropTable(
                name: "ImageAnswerGroups");

            migrationBuilder.DropTable(
                name: "CourseMapElementImages");

            migrationBuilder.DropTable(
                name: "CourseMap");

            migrationBuilder.DropTable(
                name: "QuestionSeries");

            migrationBuilder.DropTable(
                name: "QuestionCommentSection");

            migrationBuilder.DropTable(
                name: "VariableKeys");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "QuestionBase");

            migrationBuilder.DropTable(
                name: "KeysLists");

            migrationBuilder.DropTable(
                name: "Information");

            migrationBuilder.DropTable(
                name: "Keyboards");

            migrationBuilder.DropTable(
                name: "LevelsOfDifficulty");

            migrationBuilder.DropTable(
                name: "Subtopics");

            migrationBuilder.DropTable(
                name: "Topics");

            migrationBuilder.DropTable(
                name: "DataPools");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
