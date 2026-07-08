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



CREATE TABLE events(
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    event_name VARCHAR(255) NOT NULL,
    start_date TIMESTAMPTZ NOT NULL,
    end_date TIMESTAMPTZ NOT NULL,
    description TEXT,

    event_img_url VARCHAR(500),

    CONSTRAINT chk_event_dates CHECK (end_date >= start_date)
);

CREATE INDEX idx_event_name ON events(event_name);

CREATE INDEX idx_event_start_date ON events(start_date);