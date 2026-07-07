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



CREATE TYPE job_status AS ENUM ('AGENDADO', 'EXECUCAO', 'CONCLUIDO', 'CANCELADO', 'REAGENDADO');
CREATE TYPE job_type AS ENUM ('TATTOO', 'PIERCING');

CREATE TABLE jobs (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    client_id INT NOT NULL REFERENCES clients(id) ON DELETE RESTRICT,
    type job_type NOT NULL, 
    status job_status NOT NULL DEFAULT 'AGENDADO',
    final_price NUMERIC,
    scheduled_date TIMESTAMPTZ NOT NULL
);

CREATE TABLE job_tattoos_details (
    job_id INT PRIMARY KEY REFERENCES jobs(id) ON DELETE CASCADE,
    size_cm NUMERIC NOT NULL,
    fill NUMERIC NOT NULL CHECK (fill BETWEEN 1 AND 5),
    shadow NUMERIC NOT NULL CHECK (shadow BETWEEN 1 AND 5),
    detail INT NOT NULL CHECK (detail BETWEEN 1 AND 5),
    has_color BOOLEAN NOT NULL DEFAULT FALSE,
    body_zone NUMERIC NOT NULL CHECK (body_zone BETWEEN 1 AND 5),
    type VARCHAR(100) NOT NULL
);

CREATE TABLE job_piercings_details (
    job_id INT PRIMARY KEY REFERENCES jobs(id) ON DELETE CASCADE,
    body_part VARCHAR(50) NOT NULL
);

