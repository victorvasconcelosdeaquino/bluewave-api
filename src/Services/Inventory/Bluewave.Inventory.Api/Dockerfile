# 1. Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy the project files
COPY ["src/Services/Inventory/Bluewave.Inventory.Api/Bluewave.Inventory.Api.csproj", "src/Services/Inventory/Bluewave.Inventory.Api/"]
COPY ["src/Services/Inventory/Bluewave.Inventory.Application/Bluewave.Inventory.Application.csproj", "src/Services/Inventory/Bluewave.Inventory.Application/"]
COPY ["src/Services/Inventory/Bluewave.Inventory.Domain/Bluewave.Inventory.Domain.csproj", "src/Services/Inventory/Bluewave.Inventory.Domain/"]
COPY ["src/Services/Inventory/Bluewave.Inventory.Infrastructure/Bluewave.Inventory.Infrastructure.csproj", "src/Services/Inventory/Bluewave.Inventory.Infrastructure/"]
COPY ["src/BuildingBlocks/Bluewave.Core/Bluewave.Core.csproj", "src/BuildingBlocks/Bluewave.Core/"]

# Restore
RUN dotnet restore "src/Services/Inventory/Bluewave.Inventory.Api/Bluewave.Inventory.Api.csproj"

# Build
COPY . .
WORKDIR "/app/src/Services/Inventory/Bluewave.Inventory.Api"
RUN dotnet build -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# 2. Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bluewave.Inventory.Api.dll"]
