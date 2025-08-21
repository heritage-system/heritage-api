using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cultural_Heritage_System.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    create_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    latitude = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    longitude = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    create_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "QuizCategories",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    create_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizCategories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "QuizRanks",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    create_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizRanks", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    create_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    create_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Heritages",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    category_id = table.Column<int>(type: "int", nullable: false),
                    map_url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_featured = table.Column<bool>(type: "bit", nullable: false),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    create_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Heritages", x => x.id);
                    table.ForeignKey(
                        name: "FK_Heritages_Categories_category_id",
                        column: x => x.category_id,
                        principalTable: "Categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    user_status = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    two_factor_secret = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_2fa_verified = table.Column<bool>(type: "bit", nullable: false),
                    role_id = table.Column<int>(type: "int", nullable: false),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    create_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.id);
                    table.ForeignKey(
                        name: "FK_Users_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeritageLocations",
                columns: table => new
                {
                    heritage_id = table.Column<long>(type: "bigint", nullable: false),
                    location_id = table.Column<int>(type: "int", nullable: false),
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    create_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeritageLocations", x => new { x.heritage_id, x.location_id });
                    table.ForeignKey(
                        name: "FK_HeritageLocations_Heritages_heritage_id",
                        column: x => x.heritage_id,
                        principalTable: "Heritages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HeritageLocations_Locations_location_id",
                        column: x => x.location_id,
                        principalTable: "Locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeritageMedias",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    heritage_id = table.Column<long>(type: "bigint", nullable: false),
                    media_type = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    create_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeritageMedias", x => x.id);
                    table.ForeignKey(
                        name: "FK_HeritageMedias_Heritages_heritage_id",
                        column: x => x.heritage_id,
                        principalTable: "Heritages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeritageTags",
                columns: table => new
                {
                    heritage_id = table.Column<long>(type: "bigint", nullable: false),
                    tag_id = table.Column<int>(type: "int", nullable: false),
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    create_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeritageTags", x => new { x.heritage_id, x.tag_id });
                    table.ForeignKey(
                        name: "FK_HeritageTags_Heritages_heritage_id",
                        column: x => x.heritage_id,
                        principalTable: "Heritages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HeritageTags_Tags_tag_id",
                        column: x => x.tag_id,
                        principalTable: "Tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contributors",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    bio = table.Column<string>(type: "text", nullable: true),
                    expertise = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    documents_url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    rating = table.Column<decimal>(type: "decimal(3,2)", nullable: true),
                    verified = table.Column<bool>(type: "bit", nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    create_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contributors", x => x.id);
                    table.ForeignKey(
                        name: "FK_Contributors_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExportLogs",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    format = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    filter_criteria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    create_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExportLogs", x => x.id);
                    table.ForeignKey(
                        name: "FK_ExportLogs_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Favorites",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    heritage_id = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    create_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorites", x => x.id);
                    table.ForeignKey(
                        name: "FK_Favorites_Heritages_heritage_id",
                        column: x => x.heritage_id,
                        principalTable: "Heritages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Favorites_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    resolved = table.Column<bool>(type: "bit", nullable: false),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    create_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.id);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_read = table.Column<bool>(type: "bit", nullable: false),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    create_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.id);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PasswordResets",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    code_hash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    expires_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    used = table.Column<bool>(type: "bit", nullable: false),
                    attempts = table.Column<int>(type: "int", nullable: false),
                    created_ip = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    verified_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    create_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResets", x => x.id);
                    table.ForeignKey(
                        name: "FK_PasswordResets_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    date_of_birth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    avatar_url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    create_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.id);
                    table.ForeignKey(
                        name: "FK_Profiles_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Quiz",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false),
                    updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    create_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quiz", x => x.id);
                    table.ForeignKey(
                        name: "FK_Quiz_Users_created_by",
                        column: x => x.created_by,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    heritage_id = table.Column<long>(type: "bigint", nullable: false),
                    reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    create_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.id);
                    table.ForeignKey(
                        name: "FK_Reports_Heritages_heritage_id",
                        column: x => x.heritage_id,
                        principalTable: "Heritages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reports_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    heritage_id = table.Column<long>(type: "bigint", nullable: false),
                    rating = table.Column<int>(type: "int", nullable: false),
                    comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    create_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.id);
                    table.ForeignKey(
                        name: "FK_Reviews_Heritages_heritage_id",
                        column: x => x.heritage_id,
                        principalTable: "Heritages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SystemLogs",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    details = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    create_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemLogs", x => x.id);
                    table.ForeignKey(
                        name: "FK_SystemLogs_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wallets",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    create_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.id);
                    table.ForeignKey(
                        name: "FK_Wallets_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contributions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    contributor_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    price = table.Column<decimal>(name: "price ", type: "decimal(18,2)", nullable: false),
                    media_url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    reviewed_by = table.Column<int>(type: "int", nullable: false),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    create_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contributions", x => x.id);
                    table.ForeignKey(
                        name: "FK_Contributions_Contributors_contributor_id",
                        column: x => x.contributor_id,
                        principalTable: "Contributors",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Contributions_Users_reviewed_by",
                        column: x => x.reviewed_by,
                        principalTable: "Users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "QuizQuestions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    quiz_id = table.Column<long>(type: "bigint", nullable: false),
                    question = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    option_a = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    option_b = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    option_c = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    option_d = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    correct_option = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    quiz_category_id = table.Column<int>(type: "int", nullable: false),
                    quiz_rank_id = table.Column<int>(type: "int", nullable: false),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    create_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizQuestions", x => x.id);
                    table.ForeignKey(
                        name: "FK_QuizQuestions_QuizCategories_quiz_category_id",
                        column: x => x.quiz_category_id,
                        principalTable: "QuizCategories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuizQuestions_QuizRanks_quiz_rank_id",
                        column: x => x.quiz_rank_id,
                        principalTable: "QuizRanks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuizQuestions_Quiz_quiz_id",
                        column: x => x.quiz_id,
                        principalTable: "Quiz",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizResults",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    score = table.Column<int>(type: "int", nullable: false),
                    quiz_id = table.Column<long>(type: "bigint", nullable: false),
                    completed_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    create_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizResults", x => x.id);
                    table.ForeignKey(
                        name: "FK_QuizResults_Quiz_quiz_id",
                        column: x => x.quiz_id,
                        principalTable: "Quiz",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuizResults_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "ContributionPurchases",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    contribution_id = table.Column<int>(type: "int", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    purchased_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    payment_status = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    payment_method = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    create_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContributionPurchases", x => x.id);
                    table.ForeignKey(
                        name: "FK_ContributionPurchases_Contributions_contribution_id",
                        column: x => x.contribution_id,
                        principalTable: "Contributions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContributionPurchases_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RevenueShares",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    contribution_id = table.Column<int>(type: "int", nullable: false),
                    contributor_id = table.Column<int>(type: "int", nullable: false),
                    purchase_id = table.Column<int>(type: "int", nullable: false),
                    total_amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    contributor_amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    system_amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    payout_status = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    create_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RevenueShares", x => x.id);
                    table.ForeignKey(
                        name: "FK_RevenueShares_ContributionPurchases_purchase_id",
                        column: x => x.purchase_id,
                        principalTable: "ContributionPurchases",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RevenueShares_Contributions_contribution_id",
                        column: x => x.contribution_id,
                        principalTable: "Contributions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RevenueShares_Contributors_contributor_id",
                        column: x => x.contributor_id,
                        principalTable: "Contributors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WalletTransactions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    wallet_id = table.Column<int>(type: "int", nullable: false),
                    type = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    related_purchase_id = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    update_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletTransactions", x => x.id);
                    table.ForeignKey(
                        name: "FK_WalletTransactions_ContributionPurchases_related_purchase_id",
                        column: x => x.related_purchase_id,
                        principalTable: "ContributionPurchases",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_WalletTransactions_Wallets_wallet_id",
                        column: x => x.wallet_id,
                        principalTable: "Wallets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContributionPurchases_contribution_id",
                table: "ContributionPurchases",
                column: "contribution_id");

            migrationBuilder.CreateIndex(
                name: "IX_ContributionPurchases_user_id",
                table: "ContributionPurchases",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Contributions_contributor_id",
                table: "Contributions",
                column: "contributor_id");

            migrationBuilder.CreateIndex(
                name: "IX_Contributions_reviewed_by",
                table: "Contributions",
                column: "reviewed_by");

            migrationBuilder.CreateIndex(
                name: "IX_Contributors_user_id",
                table: "Contributors",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_ExportLogs_user_id",
                table: "ExportLogs",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_heritage_id",
                table: "Favorites",
                column: "heritage_id");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_user_id",
                table: "Favorites",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_user_id",
                table: "Feedbacks",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_HeritageLocations_location_id",
                table: "HeritageLocations",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "IX_HeritageMedias_heritage_id",
                table: "HeritageMedias",
                column: "heritage_id");

            migrationBuilder.CreateIndex(
                name: "IX_Heritages_category_id",
                table: "Heritages",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_HeritageTags_tag_id",
                table: "HeritageTags",
                column: "tag_id");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_user_id",
                table: "Notifications",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResets_user_id",
                table: "PasswordResets",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_user_id",
                table: "Profiles",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Quiz_created_by",
                table: "Quiz",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestions_quiz_category_id",
                table: "QuizQuestions",
                column: "quiz_category_id");

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestions_quiz_id",
                table: "QuizQuestions",
                column: "quiz_id");

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestions_quiz_rank_id",
                table: "QuizQuestions",
                column: "quiz_rank_id");

            migrationBuilder.CreateIndex(
                name: "IX_QuizResults_quiz_id",
                table: "QuizResults",
                column: "quiz_id");

            migrationBuilder.CreateIndex(
                name: "IX_QuizResults_user_id",
                table: "QuizResults",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_heritage_id",
                table: "Reports",
                column: "heritage_id");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_user_id",
                table: "Reports",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_RevenueShares_contribution_id",
                table: "RevenueShares",
                column: "contribution_id");

            migrationBuilder.CreateIndex(
                name: "IX_RevenueShares_contributor_id",
                table: "RevenueShares",
                column: "contributor_id");

            migrationBuilder.CreateIndex(
                name: "IX_RevenueShares_purchase_id",
                table: "RevenueShares",
                column: "purchase_id");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_heritage_id",
                table: "Reviews",
                column: "heritage_id");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_user_id",
                table: "Reviews",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_SystemLogs_user_id",
                table: "SystemLogs",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_role_id",
                table: "Users",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_user_id",
                table: "Wallets",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_related_purchase_id",
                table: "WalletTransactions",
                column: "related_purchase_id");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_wallet_id",
                table: "WalletTransactions",
                column: "wallet_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExportLogs");

            migrationBuilder.DropTable(
                name: "Favorites");

            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropTable(
                name: "HeritageLocations");

            migrationBuilder.DropTable(
                name: "HeritageMedias");

            migrationBuilder.DropTable(
                name: "HeritageTags");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "PasswordResets");

            migrationBuilder.DropTable(
                name: "Profiles");

            migrationBuilder.DropTable(
                name: "QuizQuestions");

            migrationBuilder.DropTable(
                name: "QuizResults");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "RevenueShares");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "SystemLogs");

            migrationBuilder.DropTable(
                name: "WalletTransactions");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "QuizCategories");

            migrationBuilder.DropTable(
                name: "QuizRanks");

            migrationBuilder.DropTable(
                name: "Quiz");

            migrationBuilder.DropTable(
                name: "Heritages");

            migrationBuilder.DropTable(
                name: "ContributionPurchases");

            migrationBuilder.DropTable(
                name: "Wallets");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Contributions");

            migrationBuilder.DropTable(
                name: "Contributors");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}
