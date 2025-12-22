# 1. Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy the project files
COPY ["src/Services/Sales/Bluewave.Sales.Api/Bluewave.Sales.Api.csproj", "src/Services/Sales/Bluewave.Sales.Api/"]
COPY ["src/Services/Sales/Bluewave.Sales.Application/Bluewave.Sales.Application.csproj", "src/Services/Sales/Bluewave.Sales.Application/"]
COPY ["src/Services/Sales/Bluewave.Sales.Domain/Bluewave.Sales.Domain.csproj", "src/Services/Sales/Bluewave.Sales.Domain/"]
COPY ["src/Services/Sales/Bluewave.Sales.Infrastructure/Bluewave.Sales.Infrastructure.csproj", "src/Services/Sales/Bluewave.Sales.Infrastructure/"]
COPY ["src/BuildingBlocks/Bluewave.Core/Bluewave.Core.csproj", "src/BuildingBlocks/Bluewave.Core/"]

# Restore
RUN dotnet restore "src/Services/Sales/Bluewave.Sales.Api/Bluewave.Sales.Api.csproj"

# Build
COPY . .
WORKDIR "/app/src/Services/Sales/Bluewave.Sales.Api"
RUN dotnet build -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# 2. Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bluewave.Sales.Api.dll"]
