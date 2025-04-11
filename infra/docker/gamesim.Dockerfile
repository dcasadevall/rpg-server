# This is a placeholder Dockerfile for the game simulation service
# The actual service will be implemented in Go and located in /gamesim-service
# This file will be updated once the Go implementation is ready

# Build stage
FROM golang:1.22-alpine AS build
WORKDIR /src

# Copy go.mod and go.sum first to leverage Docker cache
COPY ["gamesim-service/go.mod", "gamesim-service/go.sum", "./"]
RUN go mod download

# Copy the source code
COPY ["gamesim-service/.", "."]

# Build the application
RUN CGO_ENABLED=0 GOOS=linux go build -o /app/gamesim-service

# Runtime stage
FROM alpine:latest
WORKDIR /app

# Install required runtime dependencies
RUN apk add --no-cache ca-certificates tzdata

# Copy the binary
COPY --from=build /app/gamesim-service .

# Expose the UDP port for game simulation
EXPOSE 7777/udp

# Set environment variables
ENV GAME_SIM_PORT=7777
ARG ENVIRONMENT=dev
ENV GAME_SIM_ENVIRONMENT=${ENVIRONMENT}

# Start the application
ENTRYPOINT ["/app/gamesim-service"]
