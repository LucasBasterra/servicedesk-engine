# Etapa 1: Build y Publish (.NET 10 SDK)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar archivo de proyecto y restaurar dependencias
COPY ["ServiceDesk.Api.csproj", "./"]
RUN dotnet restore "ServiceDesk.Api.csproj"

# Copiar todo el código fuente y compilar
COPY . .
RUN dotnet publish "ServiceDesk.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 2: Runtime (.NET 10 ASP.NET Core)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ServiceDesk.Api.dll"]