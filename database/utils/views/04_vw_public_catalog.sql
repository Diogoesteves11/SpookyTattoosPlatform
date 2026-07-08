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
    t.type AS tattoo_style,
    
    p.body_part

FROM tattoo_catalog tc
INNER JOIN jobs j ON tc.job_id = j.id
LEFT JOIN job_tattoos_details t ON j.id = t.job_id
LEFT JOIN job_piercings_details p ON j.id = p.job_id

INNER JOIN tattoo_catalog_images tci 
    ON tc.id = tci.catalog_id AND tci.display_order = 1;