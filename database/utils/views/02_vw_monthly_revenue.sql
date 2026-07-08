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