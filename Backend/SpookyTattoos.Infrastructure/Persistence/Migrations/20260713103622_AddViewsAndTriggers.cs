using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpookyTattoos.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddViewsAndTriggers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Adicionar colunas necessárias para os Triggers
            migrationBuilder.Sql(@"
                ALTER TABLE clients ADD COLUMN IF NOT EXISTS updated_at TIMESTAMPTZ DEFAULT NOW();
                ALTER TABLE jobs ADD COLUMN IF NOT EXISTS updated_at TIMESTAMPTZ DEFAULT NOW();
            ");

            // 2. Criar Funções para os Triggers
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION set_updated_at()
                RETURNS TRIGGER AS $$
                BEGIN
                    NEW.updated_at = NOW();
                    RETURN NEW;
                END;
                $$ LANGUAGE plpgsql;

                CREATE OR REPLACE FUNCTION update_client_last_visit()
                RETURNS TRIGGER AS $$
                BEGIN
                    IF NEW.status = 'CONCLUIDO' AND OLD.status IS DISTINCT FROM 'CONCLUIDO' THEN
                        UPDATE clients
                        SET last_job = NEW.scheduled_date
                        WHERE id = NEW.client_id;
                    END IF;
                    
                    RETURN NEW;
                END;
                $$ LANGUAGE plpgsql;
            ");

            // 3. Criar os Triggers
            migrationBuilder.Sql(@"
                CREATE TRIGGER trg_clients_updated_at
                BEFORE UPDATE ON clients
                FOR EACH ROW
                EXECUTE FUNCTION set_updated_at();

                CREATE TRIGGER trg_jobs_updated_at
                BEFORE UPDATE ON jobs
                FOR EACH ROW
                EXECUTE FUNCTION set_updated_at();

                CREATE TRIGGER trg_job_completed_update_client
                AFTER UPDATE ON jobs
                FOR EACH ROW
                EXECUTE FUNCTION update_client_last_visit();
            ");

            // 4. Criar as Views
            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW vw_clientes_stats AS
                SELECT 
                    c.id AS client_id,
                    c.full_name,
                    c.instagram_user,
                    c.last_job,
                    c.ghost_points,
                    COUNT(j.id) AS total_appointments,
                    COALESCE(SUM(j.final_price), 0) AS total_spent
                FROM clients c
                LEFT JOIN jobs j ON c.id = j.client_id AND j.status = 'CONCLUIDO'
                GROUP BY 
                    c.id, c.full_name, c.instagram_user, c.last_job;

                CREATE OR REPLACE VIEW vw_monthly_revenue AS
                SELECT 
                    TO_CHAR(scheduled_date, 'YYYY-MM') AS month_year,
                    type AS job_type,
                    COUNT(id) AS finished_jobs,
                    COALESCE(SUM(final_price), 0) AS total_revenue
                FROM jobs
                WHERE status = 'CONCLUIDO'
                GROUP BY 
                    TO_CHAR(scheduled_date, 'YYYY-MM'), type
                ORDER BY 
                    month_year DESC;

                CREATE OR REPLACE VIEW vw_public_catalog AS
                SELECT 
                    tc.id AS post_id,
                    tc.created_at,
                    tc.description,
                    tci.image_url AS cover_image_url,
                    j.type AS job_type,
                    t.size_cm,
                    t.fill,
                    t.shadow,
                    t.detail,
                    t.has_color,
                    t.body_zone,
                    t.style AS tattoo_style,
                    p.body_part
                FROM post_catalog_items tc
                INNER JOIN jobs j ON tc.job_id = j.id
                LEFT JOIN job_tattoo_details t ON j.id = t.job_id
                LEFT JOIN job_piercing_details p ON j.id = p.job_id
                INNER JOIN post_final_images tci 
                    ON tc.id = tci.post_id AND tci.display_order = 1;

                CREATE OR REPLACE VIEW vw_coupon_validation AS
                SELECT 
                    c.id AS coupon_id,
                    cl.full_name AS client_name,
                    cl.email AS client_email,
                    p.description AS promo_description,
                    p.conditions,
                    c.is_used,
                    p.start_date,
                    p.end_date,
                    CASE 
                        WHEN c.is_used = FALSE 
                             AND (p.start_date IS NULL OR p.start_date <= NOW())
                             AND (p.end_date IS NULL OR p.end_date >= NOW()) 
                        THEN TRUE
                        ELSE FALSE
                    END AS is_valid
                FROM coupons c
                JOIN promos p ON c.promo_id = p.id
                JOIN clients cl ON c.client_id = cl.id;

                CREATE OR REPLACE VIEW vw_voucher_validation AS
                SELECT 
                    v.id AS voucher_id,
                    cl.full_name AS emitter_name,
                    v.value AS voucher_value,
                    v.generated_at,
                    v.expires_at,
                    v.is_used,
                    CASE 
                        WHEN v.is_used = FALSE AND v.expires_at >= NOW() THEN TRUE
                        ELSE FALSE
                    END AS is_valid
                FROM vouchers v
                JOIN clients cl ON v.emitter_id = cl.id; -- <-- CORRIGIDO DE v.emitter PARA v.emitter_id

                CREATE OR REPLACE VIEW vw_yearly_revenue AS
                SELECT 
                    TO_CHAR(scheduled_date, 'YYYY') AS year,
                    type AS job_type,
                    COUNT(id) AS finished_jobs,
                    COALESCE(SUM(final_price), 0) AS total_revenue
                FROM jobs
                WHERE status = 'CONCLUIDO'
                GROUP BY 
                    TO_CHAR(scheduled_date, 'YYYY'), type
                ORDER BY 
                    year DESC;

                CREATE OR REPLACE VIEW vw_complete_agenda AS
                SELECT 
                    j.id AS job_id,
                    j.scheduled_date,
                    j.status,
                    j.type AS job_type,
                    j.final_price,
                    c.id AS client_id,
                    c.full_name AS client_name,
                    c.instagram_user,
                    c.phone_number,
                    t.size_cm,
                    t.has_color,
                    t.body_zone,
                    p.body_part
                FROM jobs j
                INNER JOIN clients c ON j.client_id = c.id
                LEFT JOIN job_tattoo_details t ON j.id = t.job_id
                LEFT JOIN job_piercing_details p ON j.id = p.job_id;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remover Views
            migrationBuilder.Sql(@"
                DROP VIEW IF EXISTS vw_complete_agenda;
                DROP VIEW IF EXISTS vw_yearly_revenue;
                DROP VIEW IF EXISTS vw_voucher_validation;
                DROP VIEW IF EXISTS vw_coupon_validation;
                DROP VIEW IF EXISTS vw_public_catalog;
                DROP VIEW IF EXISTS vw_monthly_revenue;
                DROP VIEW IF EXISTS vw_clientes_stats;
            ");

            // Remover Triggers
            migrationBuilder.Sql(@"
                DROP TRIGGER IF EXISTS trg_job_completed_update_client ON jobs;
                DROP TRIGGER IF EXISTS trg_jobs_updated_at ON jobs;
                DROP TRIGGER IF EXISTS trg_clients_updated_at ON clients;
            ");

            // Remover Funções
            migrationBuilder.Sql(@"
                DROP FUNCTION IF EXISTS update_client_last_visit();
                DROP FUNCTION IF EXISTS set_updated_at();
            ");
        }
    }
}