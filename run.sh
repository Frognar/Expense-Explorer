#!/bin/bash
docker compose -f res/eventstore/docker-compose.yaml up -d
docker compose -f res/postgresql/docker-compose.yaml up -d
dotnet src/ExpenseExplorer.API/bin/Release/net8.0/ExpenseExplorer.API.dll
docker compose -f res/eventstore/docker-compose.yaml stop
docker compose -f res/postgresql/docker-compose.yaml stop
