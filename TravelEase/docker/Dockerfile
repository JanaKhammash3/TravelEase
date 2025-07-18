# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy source files
COPY ./src ./src
COPY ./docker/appsettings.Production.json ./src/TravelEase.API/appsettings.Production.json

# Restore & publish
WORKDIR /app/src/TravelEase.API
RUN dotnet restore
RUN dotnet publish -c Release -o /out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out .

EXPOSE 5000
ENTRYPOINT ["dotnet", "TravelEase.API.dll"]
