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

CREATE EXTENSION IF NOT EXISTS pg_trgm;

CREATE TABLE clients (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    full_name VARCHAR(100) NOT NULL,
    email VARCHAR(150) UNIQUE,
    phone_number VARCHAR(20) NOT NULL,
    instagram_user VARCHAR(50) DEFAULT '',
    ghost_points INT NOT NULL DEFAULT 0,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    last_job TIMESTAMPTZ
);


CREATE INDEX idx_clients_instagram_search ON clients USING GIN (instagram_user gin_trgm_ops);
CREATE INDEX idx_clients_name_search ON clients USING GIN (full_name gin_trgm_ops);

CREATE INDEX idx_clients_ghost_points ON clients(ghost_points DESC);