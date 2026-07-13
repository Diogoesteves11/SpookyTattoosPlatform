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


CREATE VIEW vw_coupon_validation AS
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