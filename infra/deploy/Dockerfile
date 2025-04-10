# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
WORKDIR /src

# Copy the project files
COPY ["../../rpg-character-service/src/rpg-character-service.csproj", "src/"]
COPY ["../../rpg-character-service/rpg-character-service.sln", "./"]

# Restore dependencies
RUN dotnet restore

# Copy the rest of the source code
COPY ["../../rpg-character-service", "."]

# Build the application
WORKDIR "/src/src"
RUN dotnet build "rpg-character-service.csproj" -c Release -o /app/build

# Publish the application
RUN dotnet publish "rpg-character-service.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0-preview AS final
WORKDIR /app

# Install required system dependencies
RUN apt-get update && apt-get install -y \
    curl \
    && rm -rf /var/lib/apt/lists/*

# Copy the published application
COPY --from=build /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose the port the app runs on
EXPOSE 80

# Start the application
ENTRYPOINT ["dotnet", "rpg-character-service.dll"]
