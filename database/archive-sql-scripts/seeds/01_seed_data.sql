/*
Copyright 2026 Diogo Esteves, Guilherme Mattos

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

INSERT INTO admins (username, email, password) VALUES
('diogo_admin', 'diogo@spookytattoos.com', 'hashed_password_123'),
('gm_admin', 'gm@spookytattoos.com', 'hashed_password_456');

INSERT INTO clients (full_name, email, phone_number, instagram_user, ghost_points) VALUES
('Ana Silva', 'ana.silva@email.com', '912345678', '@anasilva_ink', 15),
('Bruno Costa', 'bruno.c@email.com', '965432198', '@brunoc_88', 0),
('Carla Mendes', 'carla.m@email.com', '933444555', NULL, 5);

INSERT INTO jobs (client_id, type, status, final_price, scheduled_date) VALUES
(1, 'TATTOO', 'CONCLUIDO', 150.00, NOW() - INTERVAL '30 days'),
(2, 'PIERCING', 'CONCLUIDO', 35.00, NOW() - INTERVAL '15 days'),
(1, 'TATTOO', 'AGENDADO', NULL, NOW() + INTERVAL '5 days'),
(3, 'TATTOO', 'EXECUCAO', NULL, NOW());

-- CORREÇÃO: Tabela atualizada para job_tattoo_details
INSERT INTO job_tattoo_details (job_id, size_cm, fill, shadow, detail, has_color, body_zone, style) VALUES
(1, 15.5, 3, 4, 5, true, 2, 'NEO-TRADITIONAL'),
(3, 8.0, 1, 1, 2, false, 4, 'FINE LINE'),
(4, 25.0, 5, 5, 5, false, 1, 'REALISM');

-- CORREÇÃO: Tabela atualizada para job_piercing_details
INSERT INTO job_piercing_details (job_id, body_part) VALUES
(2, 'Septum');

INSERT INTO tattoo_catalog (job_id, description, post_text, is_published) VALUES
(1, 'Dragão Neo-Traditional', 'Trabalho incrível feito no mês passado! #neotrad', true);

INSERT INTO tattoo_catalog_images (catalog_id, image_url, display_order) VALUES
(1, 'http://localhost:9000/catalog-public/mock_dragao_capa.jpg', 1),
(1, 'http://localhost:9000/catalog-public/mock_dragao_detalhe.jpg', 2);

INSERT INTO piercing_catalog (job_id, description, post_text, is_published) VALUES
(2, 'Septum Piercing Titânio', 'Perfuração limpa e segura. #septum #piercing', true);

INSERT INTO piercing_catalog_images (catalog_id, image_url, display_order) VALUES
(1, 'http://localhost:9000/catalog-public/mock_septum_capa.jpg', 1);

INSERT INTO events (event_name, start_date, end_date, description, event_img_url) VALUES
('Halloween Flash Day', '2026-10-31 10:00:00', '2026-10-31 20:00:00', 'Flash tattoos a partir de 50€.', 'http://localhost:9000/catalog-public/flash.jpg');