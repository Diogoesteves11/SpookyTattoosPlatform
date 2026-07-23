#!/bin/bash
echo "A gerar nova migration..."

read -p "Nome da Migration (ex: AddClientPhone): " migration_name

dotnet ef migrations add $migration_name --project SpookyTattoos.Infrastructure --startup-project SpookyTattoos.API --output-dir Persistence/Migrations

echo "Concluído! Não te esqueças de reiniciar o contentor (docker-compose up --build -d)"