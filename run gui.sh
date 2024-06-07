#!/bin/bash
docker compose -f res/fullapi/docker-compose.yml up -d
cd src/ExpenseExplorer.GUI/bin/publish
dotnet ExpenseExplorer.GUI.dll
cd ../../../..
docker compose -f res/fullapi/docker-compose.yml stop
