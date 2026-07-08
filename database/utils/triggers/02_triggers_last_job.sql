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

CREATE TRIGGER trg_job_completed_update_client
AFTER UPDATE ON jobs
FOR EACH ROW
EXECUTE FUNCTION update_client_last_visit();