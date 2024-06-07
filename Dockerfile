FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

COPY . .
RUN dotnet restore src/ExpenseExplorer.API/ExpenseExplorer.API.csproj
RUN dotnet publish src/ExpenseExplorer.API/ExpenseExplorer.API.csproj -p:PublishProfile=FolderProfile

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/src/ExpenseExplorer.API/bin/publish/. .
ENTRYPOINT ["dotnet", "ExpenseExplorer.API.dll"]
