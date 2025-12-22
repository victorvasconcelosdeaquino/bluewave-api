FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy and Restore
COPY ["src/ApiGateways/Bluewave.Gateway/Bluewave.Gateway.csproj", "src/ApiGateways/Bluewave.Gateway/"]
RUN dotnet restore "src/ApiGateways/Bluewave.Gateway/Bluewave.Gateway.csproj"

# Build and Publish
COPY . .
WORKDIR "/app/src/ApiGateways/Bluewave.Gateway"
RUN dotnet build -c Release -o /app/build
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Final
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bluewave.Gateway.dll"]