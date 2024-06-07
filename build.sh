#!/bin/bash
dotnet test --configuration Release
dotnet publish src/ExpenseExplorer.GUI/ExpenseExplorer.GUI.csproj -p:PublishProfile=FolderProfile
