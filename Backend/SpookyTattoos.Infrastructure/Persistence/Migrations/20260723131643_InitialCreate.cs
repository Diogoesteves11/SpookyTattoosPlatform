using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SpookyTattoos.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:pg_trgm", ",,");

            migrationBuilder.CreateTable(
                name: "admins",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    username = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    password = table.Column<string>(type: "text", nullable: false),
                    active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_login = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_admins", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "clients",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    full_name = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    phone_number = table.Column<string>(type: "text", nullable: false),
                    active = table.Column<bool>(type: "boolean", nullable: false),
                    instagram_user = table.Column<string>(type: "text", nullable: true),
                    ghost_points = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_job = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_clients", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "events",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    event_name = table.Column<string>(type: "text", nullable: false),
                    start_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    end_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    image_url = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_events", x => x.id);
                    table.CheckConstraint("CK_Event_Dates", "end_date >= start_date");
                });

            migrationBuilder.CreateTable(
                name: "promos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "text", nullable: false),
                    conditions = table.Column<string>(type: "text", nullable: false),
                    ghost_points = table.Column<int>(type: "integer", nullable: false),
                    start_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    end_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_promos", x => x.id);
                    table.CheckConstraint("CK_Promo_Dates", "end_date >= start_date");
                });

            migrationBuilder.CreateTable(
                name: "jobs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    client_id = table.Column<int>(type: "integer", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    final_price = table.Column<decimal>(type: "numeric", nullable: true),
                    scheduled_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    reference_image_url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_jobs", x => x.id);
                    table.ForeignKey(
                        name: "fk_jobs_clients_client_id",
                        column: x => x.client_id,
                        principalTable: "clients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "vouchers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    emitter_id = table.Column<int>(type: "integer", nullable: false),
                    value = table.Column<decimal>(type: "numeric", nullable: false),
                    is_used = table.Column<bool>(type: "boolean", nullable: false),
                    generated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vouchers", x => x.id);
                    table.ForeignKey(
                        name: "fk_vouchers_clients_emitter_id",
                        column: x => x.emitter_id,
                        principalTable: "clients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "coupons",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    client_id = table.Column<int>(type: "integer", nullable: false),
                    promo_id = table.Column<int>(type: "integer", nullable: false),
                    is_used = table.Column<bool>(type: "boolean", nullable: false),
                    acquired_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_coupons", x => x.id);
                    table.ForeignKey(
                        name: "fk_coupons_clients_client_id",
                        column: x => x.client_id,
                        principalTable: "clients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_coupons_promos_promo_id",
                        column: x => x.promo_id,
                        principalTable: "promos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "job_piercing_details",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    job_id = table.Column<int>(type: "integer", nullable: false),
                    body_part = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_job_piercing_details", x => x.id);
                    table.ForeignKey(
                        name: "fk_job_piercing_details_jobs_job_id",
                        column: x => x.job_id,
                        principalTable: "jobs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "job_tattoo_details",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    size_cm = table.Column<decimal>(type: "numeric", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    style = table.Column<string>(type: "text", nullable: false),
                    final_tattoo_price = table.Column<decimal>(type: "numeric", nullable: false),
                    job_id = table.Column<int>(type: "integer", nullable: false),
                    fill = table.Column<int>(type: "integer", nullable: false),
                    shadow = table.Column<int>(type: "integer", nullable: false),
                    detail = table.Column<int>(type: "integer", nullable: false),
                    has_color = table.Column<bool>(type: "boolean", nullable: false),
                    body_zone = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_job_tattoo_details", x => x.id);
                    table.CheckConstraint("CK_Tattoo_BodyZoneScore", "\"body_zone\" BETWEEN 1 AND 5");
                    table.CheckConstraint("CK_Tattoo_DetailScore", "\"detail\" BETWEEN 1 AND 5");
                    table.CheckConstraint("CK_Tattoo_FillScore", "\"fill\" BETWEEN 1 AND 5");
                    table.CheckConstraint("CK_Tattoo_ShadowScore", "\"shadow\" BETWEEN 1 AND 5");
                    table.ForeignKey(
                        name: "fk_job_tattoo_details_jobs_job_id",
                        column: x => x.job_id,
                        principalTable: "jobs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "post_catalog_items",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    job_id = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    post_text = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_published = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_post_catalog_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_post_catalog_items_jobs_job_id",
                        column: x => x.job_id,
                        principalTable: "jobs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "post_final_images",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    post_id = table.Column<int>(type: "integer", nullable: false),
                    image_url = table.Column<string>(type: "text", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_post_final_images", x => x.id);
                    table.ForeignKey(
                        name: "fk_post_final_images_post_catalog_items_post_id",
                        column: x => x.post_id,
                        principalTable: "post_catalog_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "admins",
                columns: new[] { "id", "active", "created_at", "email", "last_login", "password", "username" },
                values: new object[] { 1, true, new DateTimeOffset(new DateTime(2026, 7, 13, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "admin@spookytattoos.com", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "$2a$13$pXEy4SRWCpx9uZsQy8qjjeVVGuZjz2q7nPMjJ4HT7EIW7VnN4SRzS", "admin" });

            migrationBuilder.CreateIndex(
                name: "ix_admins_active",
                table: "admins",
                column: "active");

            migrationBuilder.CreateIndex(
                name: "ix_admins_email",
                table: "admins",
                column: "email")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "ix_admins_username",
                table: "admins",
                column: "username")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "ix_clients_active",
                table: "clients",
                column: "active");

            migrationBuilder.CreateIndex(
                name: "ix_clients_email",
                table: "clients",
                column: "email")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "ix_clients_full_name",
                table: "clients",
                column: "full_name")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "ix_clients_ghost_points",
                table: "clients",
                column: "ghost_points",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "ix_clients_instagram_user",
                table: "clients",
                column: "instagram_user")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "ix_coupons_client_id",
                table: "coupons",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "ix_coupons_promo_id",
                table: "coupons",
                column: "promo_id");

            migrationBuilder.CreateIndex(
                name: "ix_events_event_name",
                table: "events",
                column: "event_name")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "ix_events_start_date",
                table: "events",
                column: "start_date");

            migrationBuilder.CreateIndex(
                name: "ix_job_piercing_details_job_id",
                table: "job_piercing_details",
                column: "job_id");

            migrationBuilder.CreateIndex(
                name: "ix_job_tattoo_details_job_id",
                table: "job_tattoo_details",
                column: "job_id");

            migrationBuilder.CreateIndex(
                name: "ix_jobs_client_id",
                table: "jobs",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "ix_post_catalog_items_created_at",
                table: "post_catalog_items",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_post_catalog_items_job_id",
                table: "post_catalog_items",
                column: "job_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_post_final_images_post_id",
                table: "post_final_images",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "ix_promos_end_date",
                table: "promos",
                column: "end_date");

            migrationBuilder.CreateIndex(
                name: "ix_promos_start_date",
                table: "promos",
                column: "start_date");

            migrationBuilder.CreateIndex(
                name: "ix_vouchers_emitter_id",
                table: "vouchers",
                column: "emitter_id");

            migrationBuilder.CreateIndex(
                name: "ix_vouchers_expires_at",
                table: "vouchers",
                column: "expires_at");

            migrationBuilder.CreateIndex(
                name: "ix_vouchers_is_used",
                table: "vouchers",
                column: "is_used");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "admins");

            migrationBuilder.DropTable(
                name: "coupons");

            migrationBuilder.DropTable(
                name: "events");

            migrationBuilder.DropTable(
                name: "job_piercing_details");

            migrationBuilder.DropTable(
                name: "job_tattoo_details");

            migrationBuilder.DropTable(
                name: "post_final_images");

            migrationBuilder.DropTable(
                name: "vouchers");

            migrationBuilder.DropTable(
                name: "promos");

            migrationBuilder.DropTable(
                name: "post_catalog_items");

            migrationBuilder.DropTable(
                name: "jobs");

            migrationBuilder.DropTable(
                name: "clients");
        }
    }
}
