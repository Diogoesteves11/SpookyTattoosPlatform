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


CREATE TABLE promos(
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    description TEXT NOT NULL,
    start_date TIMESTAMPTZ,
    end_date TIMESTAMPTZ,
    conditions TEXT NOT NULL,
    ghost_points INT NOT NULL,

    CONSTRAINT chk_promo_dates CHECK (end_date >= start_date)
);

CREATE TABLE coupons(
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    client_id INT NOT NULL,
    promo_id INT NOT NULL,
    is_used BOOLEAN DEFAULT FALSE,
    acquired_at TIMESTAMPTZ DEFAULT NOW(),

    CONSTRAINT fk_coupon_promo
        FOREIGN KEY (promo_id)
        REFERENCES promos(id)
        ON DELETE CASCADE,

    CONSTRAINT fk_coupon_client 
        FOREIGN KEY (client_id) 
        REFERENCES clients(id) 
        ON DELETE CASCADE
);

CREATE INDEX idx_promo_start_date ON promos(start_date);
CREATE INDEX idx_coupon_client ON coupons(client_id);
CREATE INDEX idx_coupon_promo ON coupons(promo_id);

