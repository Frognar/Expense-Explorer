FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["global.json", "Directory.Packages.props", "Directory.Build.props", ".editorconfig", "./"]
COPY ["src/ExpenseExplorer.Api/ExpenseExplorer.Api.csproj", "src/ExpenseExplorer.Api/"]
COPY ["src/ExpenseExplorer.Application/ExpenseExplorer.Application.csproj", "src/ExpenseExplorer.Application/"]
COPY ["src/ExpenseExplorer.Contracts/ExpenseExplorer.Contracts.csproj", "src/ExpenseExplorer.Contracts/"]
COPY ["src/ExpenseExplorer.Domain/ExpenseExplorer.Domain.csproj", "src/ExpenseExplorer.Domain/"]
COPY ["src/ExpenseExplorer.Infrastructure/ExpenseExplorer.Infrastructure.csproj", "src/ExpenseExplorer.Infrastructure/"]
COPY ["src/ExpenseExplorer.Web/ExpenseExplorer.Web.csproj", "src/ExpenseExplorer.Web/"]
RUN dotnet restore "src/ExpenseExplorer.Api/ExpenseExplorer.Api.csproj"

COPY src/ src/
RUN dotnet publish "src/ExpenseExplorer.Api/ExpenseExplorer.Api.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "ExpenseExplorer.Api.dll"]
