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
    -- Dados de Tattoo (NULL se for piercing)
    t.size_cm,
    t.has_color,
    t.body_zone,
    -- Dados de Piercing (NULL se for tattoo)
    p.body_part
FROM jobs j
INNER JOIN clients c ON j.client_id = c.id
LEFT JOIN job_tattoo_details t ON j.id = t.job_id
LEFT JOIN job_piercing_details p ON j.id = p.job_id;