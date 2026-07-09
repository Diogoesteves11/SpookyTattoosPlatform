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

CREATE TYPE tattoo_style AS ENUM ('FLASH', 'REALISM', 'NEO-TRADITIONAL', 'FINE LINE')
CREATE TYPE piercing_type AS ENUM ('MICRODERMAL')
CREATE TYPE piercing_body_part AS ENUM ('ORELHA', 'SEPTRO', 'NARIZ', 'UMBIGO')

CREATE TABLE jobs (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    referenceImageUrl VARCHAR(500) NOT NULL,
    client_id INT NOT NULL,
    type job_type NOT NULL, 
    status job_status NOT NULL DEFAULT 'AGENDADO',
    final_price NUMERIC,
    scheduled_date TIMESTAMPTZ NOT NULL,

    CONSTRAINT fk_jobs_client 
        FOREIGN KEY (client_id) 
        REFERENCES clients(id) 
        ON DELETE RESTRICT
);

CREATE TABLE job_tattoo_details (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    job_id INT NOT NULL REFERENCES jobs(id) ON DELETE CASCADE,
    size_cm NUMERIC NOT NULL,
    fill NUMERIC NOT NULL CHECK (fill BETWEEN 1 AND 5),
    shadow NUMERIC NOT NULL CHECK (shadow BETWEEN 1 AND 5),
    detail INT NOT NULL CHECK (detail BETWEEN 1 AND 5),
    has_color BOOLEAN NOT NULL DEFAULT FALSE,
    body_zone NUMERIC NOT NULL CHECK (body_zone BETWEEN 1 AND 5),
    style tattoo_style VARCHAR(100) NOT NULL
);


CREATE TABLE job_piercing_details (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    job_id INT NOT NULL REFERENCES jobs(id) ON DELETE CASCADE,
    body_part VARCHAR(50) NOT NULL,
    type piercing_type
);

CREATE INDEX idx_job_tattoo_job_id ON job_tattoo_details(job_id);

CREATE INDEX idx_job_piercing_job_id ON job_piercing_details(job_id);

CREATE INDEX idx_job_schedule ON jobs(scheduled_date);

CREATE INDEX idx_job_status ON jobs(status);

CREATE INDEX idx_job_client ON jobs(client_id);