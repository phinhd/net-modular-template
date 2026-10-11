# syntax=docker/dockerfile:1

# ---- Build ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore first so the layer is cached until a project file changes.
COPY Directory.Build.props Directory.Packages.props ./
COPY src/BuildingBlocks/BuildingBlocks.Domain/BuildingBlocks.Domain.csproj src/BuildingBlocks/BuildingBlocks.Domain/
COPY src/BuildingBlocks/BuildingBlocks.Application/BuildingBlocks.Application.csproj src/BuildingBlocks/BuildingBlocks.Application/
COPY src/BuildingBlocks/BuildingBlocks.Infrastructure/BuildingBlocks.Infrastructure.csproj src/BuildingBlocks/BuildingBlocks.Infrastructure/
COPY src/Modules/Users/Users.Domain/Users.Domain.csproj src/Modules/Users/Users.Domain/
COPY src/Modules/Users/Users.Application/Users.Application.csproj src/Modules/Users/Users.Application/
COPY src/Modules/Users/Users.Infrastructure/Users.Infrastructure.csproj src/Modules/Users/Users.Infrastructure/
COPY src/Host/WebApi/WebApi.csproj src/Host/WebApi/
RUN dotnet restore src/Host/WebApi/WebApi.csproj

COPY src/ src/
RUN dotnet publish src/Host/WebApi/WebApi.csproj -c Release -o /app/publish --no-restore /p:UseAppHost=false

# ---- Runtime ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
# Run as the non-root user that ships with the .NET images.
USER $APP_UID

ENTRYPOINT ["dotnet", "WebApi.dll"]
