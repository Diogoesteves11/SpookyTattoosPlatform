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