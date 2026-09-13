#!/usr/bin/env bash

set -euo pipefail

ROOT_DIR="$(cd "$(dirname "$0")" && pwd)"
API_PROJECT="$ROOT_DIR/src/Claims.API/Claims.API.csproj"
SQL_CONTAINER="claims-management-sql"

cleanup() {
  [[ -n "${API_PID:-}" ]] && kill "$API_PID" 2>/dev/null || true
  [[ -n "${FRONTEND_PID:-}" ]] && kill "$FRONTEND_PID" 2>/dev/null || true
}
trap cleanup EXIT INT TERM

if ! docker info >/dev/null 2>&1; then
  echo "Starting Docker Desktop…"
  open -a Docker
  for _ in {1..60}; do
    docker info >/dev/null 2>&1 && break
    sleep 2
  done
fi

if ! docker info >/dev/null 2>&1; then
  echo "Docker Desktop did not start. Open it manually and run ./run.sh again."
  exit 1
fi

connection_string="$(dotnet user-secrets list --project "$API_PROJECT" | awk -F' = ' '/ConnectionStrings:ClaimsDatabase/{print $2}')"
sql_password="$(printf '%s' "$connection_string" | sed -E 's/.*(Password|Pwd)=([^;]+).*/\2/I')"

if [[ -z "$sql_password" || "$sql_password" == "$connection_string" ]]; then
  echo "The local SQL Server password is not configured in .NET user secrets."
  exit 1
fi

if ! docker container inspect "$SQL_CONTAINER" >/dev/null 2>&1; then
  echo "Creating the local SQL Server database…"
  docker run --detach \
    --name "$SQL_CONTAINER" \
    --platform linux/amd64 \
    --env ACCEPT_EULA=Y \
    --env MSSQL_SA_PASSWORD="$sql_password" \
    --publish 1433:1433 \
    mcr.microsoft.com/mssql/server:2022-latest >/dev/null
elif [[ "$(docker inspect -f '{{.State.Running}}' "$SQL_CONTAINER")" != "true" ]]; then
  docker start "$SQL_CONTAINER" >/dev/null
fi

echo "Waiting for SQL Server…"
for attempt in {1..45}; do
  if dotnet ef database update \
    --project "$ROOT_DIR/src/Claims.Infrastructure/Claims.Infrastructure.csproj" \
    --startup-project "$API_PROJECT" >/dev/null 2>&1; then
    database_ready=true
    break
  fi
  sleep 2
done

if [[ "${database_ready:-false}" != "true" ]]; then
  echo "SQL Server did not become ready. Run ./run.sh again in a minute."
  exit 1
fi

echo "Starting the API and frontend…"
ASPNETCORE_ENVIRONMENT=Development \
  dotnet run --project "$API_PROJECT" --no-launch-profile --urls http://localhost:5293 &
API_PID=$!

cd "$ROOT_DIR/frontend"
npm run dev -- --host 127.0.0.1 &
FRONTEND_PID=$!

echo
echo "Claims Management is running at http://localhost:5173"
echo "Press Control-C to stop it."
wait "$API_PID" "$FRONTEND_PID"
