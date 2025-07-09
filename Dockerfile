FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["ExpenseExplorer.slnx", "./"]
COPY ["src/ExpenseExplorer.WebApp/ExpenseExplorer.WebApp.csproj", "src/ExpenseExplorer.WebApp/"]
COPY ["src/ExpenseExplorer.Application/ExpenseExplorer.Application.csproj", "src/ExpenseExplorer.Application/"]
COPY ["src/ExpenseExplorer.Infrastructure/ExpenseExplorer.Infrastructure.csproj", "src/ExpenseExplorer.Infrastructure/"]
COPY ["src/aspire/AppHost/AppHost.csproj", "src/aspire/AppHost/"]
COPY ["Directory.Packages.props", "./"]
COPY ["Directory.Build.props", "./"]
COPY [".editorconfig", "./"]

RUN dotnet restore "ExpenseExplorer.slnx"

COPY . .

WORKDIR "/src/src/ExpenseExplorer.WebApp"
RUN dotnet restore "ExpenseExplorer.WebApp.csproj"
RUN dotnet publish "ExpenseExplorer.WebApp.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "ExpenseExplorer.WebApp.dll"]
