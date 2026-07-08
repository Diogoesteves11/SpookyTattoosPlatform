#!/bin/bash
set -e

echo "Creating API database user: $API_DB_USER..."

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
    CREATE USER $API_DB_USER WITH PASSWORD '$API_DB_PASSWORD';

    GRANT CONNECT ON DATABASE $POSTGRES_DB TO $API_DB_USER;
    GRANT USAGE ON SCHEMA public TO $API_DB_USER;
EOSQL

echo "API user created!"