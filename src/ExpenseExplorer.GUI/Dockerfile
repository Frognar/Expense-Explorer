FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Directory.Build.props", "src/"]
COPY ["src/ExpenseExplorer.GUI/ExpenseExplorer.GUI.csproj", "src/ExpenseExplorer.GUI/"]
COPY ["src/ExpenseExplorer.API.Contract/ExpenseExplorer.API.Contract.csproj", "src/ExpenseExplorer.API.Contract/"]
RUN dotnet restore "src/ExpenseExplorer.GUI/ExpenseExplorer.GUI.csproj"
COPY . .
WORKDIR "/src/src/ExpenseExplorer.GUI"
RUN dotnet build "ExpenseExplorer.GUI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "ExpenseExplorer.GUI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExpenseExplorer.GUI.dll"]
