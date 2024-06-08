FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Directory.Build.props", "src/"]
COPY ["src/ExpenseExplorer.API/ExpenseExplorer.API.csproj", "src/ExpenseExplorer.API/"]
COPY ["src/ExpenseExplorer.API.Contract/ExpenseExplorer.API.Contract.csproj", "src/ExpenseExplorer.API.Contract/"]
COPY ["src/ExpenseExplorer.Application/ExpenseExplorer.Application.csproj", "src/ExpenseExplorer.Application/"]
COPY ["src/ExpenseExplorer.Domain//ExpenseExplorer.Domain.csproj", "src/ExpenseExplorer.Domain/"]
COPY ["src/ExpenseExplorer.Infrastructure/ExpenseExplorer.Infrastructure.csproj", "src/ExpenseExplorer.Infrastructure/"]
COPY ["src/ExpenseExplorer.ReadModel/ExpenseExplorer.ReadModel.csproj", "src/ExpenseExplorer.ReadModel/"]
COPY ["src/CommandHub/CommandHub.csproj", "src/CommandHub/"]
COPY ["src/CommandHub.DependencyInjection/CommandHub.DependencyInjection.csproj", "src/CommandHub.DependencyInjection/"]
COPY ["src/FunctionalCore/FunctionalCore.csproj", "src/FunctionalCore/"]
RUN dotnet restore "src/ExpenseExplorer.API/ExpenseExplorer.API.csproj"
COPY . .
WORKDIR "/src/src/ExpenseExplorer.API"
RUN dotnet build "ExpenseExplorer.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ExpenseExplorer.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExpenseExplorer.API.dll"]
