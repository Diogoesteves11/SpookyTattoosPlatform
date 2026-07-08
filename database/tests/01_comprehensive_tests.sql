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



/*
======================================================
  BATERIA DE TESTES (20 CENÁRIOS)
======================================================
*/

-- [TESTE 1: LEITURA] Verificar Admins inseridos
SELECT * FROM admins;

-- [TESTE 2: INSERÇÃO] Adicionar um novo cliente
INSERT INTO clients (full_name, email, phone_number, instagram_user) 
VALUES ('Daniel Sousa', 'daniel@email.com', '919999999', '@dan_sousa');

-- [TESTE 3: RESTRIÇÃO - UNIQUE] Tentar inserir cliente com email repetido (DEVE DAR ERRO)
-- INSERT INTO clients (full_name, email, phone_number) VALUES ('Clone', 'ana.silva@email.com', '900000000');

-- [TESTE 4: ATUALIZAÇÃO] Modificar Ghost Points de um cliente
UPDATE clients SET ghost_points = 20 WHERE email = 'bruno.c@email.com';

-- [TESTE 5: LEITURA GIN INDEX] Pesquisa textual por nome (simulando barra de pesquisa do frontend)
SELECT full_name, email FROM clients WHERE full_name ILIKE '%Silva%';

-- [TESTE 6: INSERÇÃO] Criar uma nova marcação (Job)
INSERT INTO jobs (client_id, type, scheduled_date) 
VALUES (
    (SELECT id FROM clients WHERE email = 'daniel@email.com'), 
    'PIERCING', 
    '2026-12-01 15:00:00'
);

-- [TESTE 7: INSERÇÃO CASCATA] Adicionar detalhes à marcação anterior
INSERT INTO job_piercings_details (job_id, body_part) 
VALUES ((SELECT MAX(id) FROM jobs), 'Nostril');

-- [TESTE 8: RESTRIÇÃO - FOREIGN KEY] Tentar apagar cliente com marcações (DEVE DAR ERRO devido a ON DELETE RESTRICT)
-- DELETE FROM clients WHERE id = 1;

-- [TESTE 9: ATUALIZAÇÃO E TRIGGER] Concluir um trabalho (Isto deve disparar o trigger do 'last_job' no cliente, se estiver ativo)
UPDATE jobs SET status = 'CONCLUIDO', final_price = 100.00 WHERE id = 3;

-- [TESTE 10: LEITURA JOIN] Cruzar Jobs com Tattoos Details
SELECT j.id, j.status, t.size_cm, t.type AS style 
FROM jobs j 
JOIN job_tattoos_details t ON j.id = t.job_id;

-- [TESTE 11: EXCLUSÃO EM CASCATA] Apagar um job e garantir que os detalhes somem
DELETE FROM jobs 
WHERE client_id = (SELECT id FROM clients WHERE email = 'daniel@email.com') 
  AND type = 'PIERCING';
-- SELECT * FROM job_tattoos_details WHERE job_id = 4; (Deve retornar 0 linhas)

-- [TESTE 12: INSERÇÃO] Adicionar post ao catálogo de Tattoos
INSERT INTO tattoo_catalog (job_id, description, is_published) 
VALUES (3, 'Nova Fine Line', false);

-- [TESTE 13: ATUALIZAÇÃO] Publicar um post no catálogo
UPDATE tattoo_catalog SET is_published = true WHERE job_id = 3;

-- [TESTE 14: VIEW] Consultar a Agenda Completa (Deve mostrar tudo)
SELECT * FROM vw_complete_agenda ORDER BY scheduled_date ASC;

-- [TESTE 15: VIEW] Consultar Métricas Mensais (Deve refletir o Job 3 que atualizámos no Teste 9)
SELECT * FROM vw_monthly_revenue;

-- [TESTE 16: VIEW] Consultar Ranking de Clientes (Ordenado por Ghost Points)
SELECT full_name, ghost_points, total_spent 
FROM vw_clientes_stats 
ORDER BY ghost_points DESC;

-- [TESTE 17: VIEW] Consultar o Catálogo Público (Teste de Capas)
SELECT cover_image_url, tattoo_style, has_color 
FROM vw_public_catalog;

-- [TESTE 18: RESTRIÇÃO - CHECK] Inserir detalhe de tattoo com parâmetro inválido (DEVE DAR ERRO)
-- INSERT INTO job_tattoos_details (job_id, size_cm, fill, shadow, detail, has_color, body_zone, type) 
-- VALUES (1, 10, 6, 2, 2, false, 2, 'Tribal'); -- O fill 6 viola o CHECK(1 a 5)

-- [TESTE 19: RESTRIÇÃO - DATAS] Inserir evento a acabar antes de começar (DEVE DAR ERRO)
-- INSERT INTO events (event_name, start_date, end_date) 
-- VALUES ('Bug Fest', '2026-11-02', '2026-11-01');

-- [TESTE 20: PAGINAÇÃO] Simular paginação da API (offset e limit) no catálogo
SELECT id, event_name, start_date 
FROM events 
ORDER BY start_date DESC 
LIMIT 10 OFFSET 0;