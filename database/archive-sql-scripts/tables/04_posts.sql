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


CREATE TABLE post_catalog_items (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    job_id INT NOT NULL,
    description TEXT,
    post_text TEXT,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    is_published BOOLEAN DEFAULT FALSE, 

    CONSTRAINT fk_catalog_job
        FOREIGN KEY (job_id)
        REFERENCES jobs(id)
        ON DELETE RESTRICT
);

CREATE TABLE post_final_images (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    catalog_id INT NOT NULL,
    
    -- Caminho do MinIO (container docker)
    image_url VARCHAR(500) NOT NULL, 
    
    -- Imagem de capa
    display_order INT NOT NULL DEFAULT 1 CHECK (display_order > 0),
    
    CONSTRAINT fk_piercing_images_catalog
        FOREIGN KEY (catalog_id)
        REFERENCES post_catalog_items(id)
        ON DELETE CASCADE
);

CREATE INDEX idx_post_catalog_items_job_id ON post_catalog_items(job_id);
CREATE INDEX idx_post_final_images_catalog_id ON post_final_images(catalog_id);