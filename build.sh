#!/bin/bash
dotnet test --configuration Release
dotnet publish src/ExpenseExplorer.API/ExpenseExplorer.API.csproj -p:PublishProfile=FolderProfile
dotnet publish src/ExpenseExplorer.GUI/ExpenseExplorer.GUI.csproj -p:PublishProfile=FolderProfile
