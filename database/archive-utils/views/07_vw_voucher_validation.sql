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


CREATE VIEW vw_voucher_validation AS
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
JOIN clients cl ON v.emitter = cl.id;