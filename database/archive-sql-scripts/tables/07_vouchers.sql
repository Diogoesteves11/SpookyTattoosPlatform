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



CREATE TABLE vouchers(
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    emitter INT NOT NULL,
    value NUMERIC NOT NULL,
    used BOOLEAN DEFAULT FALSE,
    generated_at TIMESTAMPTZ DEFAULT NOW(),
    expires_at TIMESTAMPTZ DEFAULT NOW() + INTERVAL '1 year',
    
    CONSTRAINT fk_vouchers_emitter
        FOREIGN KEY (emitter)
        REFERENCES clients(id)
        ON DELETE CASCADE
);

CREATE INDEX idx_vouchers_emitter ON vouchers(emitter);