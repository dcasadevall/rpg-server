# Build stage
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
WORKDIR /src

# Copy only the project file first
COPY ["rpg-character-service/src/rpg-character-service.csproj", "src/"]

# Restore dependencies in smaller chunks
WORKDIR "/src/src"
RUN dotnet restore "rpg-character-service.csproj" --runtime linux-x64 --disable-parallel --no-cache --force --verbosity minimal /m:1

# Copy the rest of the source code
COPY ["rpg-character-service/src/.", "."]

# Build with minimal verbosity and no parallel processing
RUN dotnet build "rpg-character-service.csproj" -c Release -o /app/build --no-restore --verbosity minimal --disable-parallel

# Publish with minimal verbosity and no parallel processing
RUN dotnet publish "rpg-character-service.csproj" -c Release -o /app/publish --no-restore --verbosity minimal --disable-parallel

# Runtime stage
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/aspnet:9.0-preview AS final
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
