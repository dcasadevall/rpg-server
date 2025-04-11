# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
WORKDIR /src

# Copy only the main project file first
COPY ["rpg-character-service/src/rpg-character-service.csproj", "src/"]

# Restore only the main project dependencies
WORKDIR "/src/src"
RUN dotnet restore "rpg-character-service.csproj"

# Copy the rest of the source code (only the src directory)
COPY ["rpg-character-service/src/.", "."]

# Build the application
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
ARG ENVIRONMENT=dev
ENV ASPNETCORE_ENVIRONMENT=${ENVIRONMENT}

# AWS DynamoDB configuration
ENV DYNAMODB_DB_PREFIX=${ENVIRONMENT}-
ENV DYNAMODB_SERVICE_URL=dynamodb.us-east-1.amazonaws.com

# Expose the port the app runs on
EXPOSE 80

# Start the application
ENTRYPOINT ["dotnet", "rpg-character-service.dll"]
