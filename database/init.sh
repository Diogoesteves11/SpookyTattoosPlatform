#!/bin/bash
set -e

echo "A iniciar a importação da arquitetura da base de dados..."

# Array com as pastas na ordem exata de dependência
folders=(
    "/scripts/init-scripts/tables"
    "/scripts/utils/views"
    "/scripts/utils/routines"
    "/scripts/init-scripts/seeds"
)

for folder in "${folders[@]}"; do
    if [ -d "$folder" ]; then
        echo "Looking for scripts in: $folder"
        # O nullglob garante que o loop não falha se a pasta estiver vazia
        shopt -s nullglob
        for f in "$folder"/*.sql; do
            echo "-> Executing: $f"
            psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" -f "$f"
        done
        shopt -u nullglob
    fi
done

echo "Initialization finished!"