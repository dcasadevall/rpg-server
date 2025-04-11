# RPG Server

This repository contains the backend services and infrastructure for a real-time RPG game inspired by Dungeons & Dragons mechanics.
It is structured as a **monorepo** to support easier local development, deployment, and scaling.

This document explains how to run the character service.<br>
Docs answering the prompt details can be found here:

- [Question 1.a Decisions](docs/1.a-Decisions.md)
- [Question 1.a API Design](docs/1.a-API-planning.md)
- [Question 1.b TDD](docs/1.b-Extension-TDD.md)
- [Question 2 Infrastructure](docs/2-infrastructure.md)

---

## Project Structure

| Directory | Description |
|:---|:---|
| `rpg-character-service/` | C# ASP.NET Core WebAPI for player character creation and management (REST APIs). |
| `game-simulation/` (Future) | Lightweight UDP-based service responsible for simulating real-time game sessions. |
| `infra/` | Terraform configuration for deploying services and infrastructure into AWS. |

---

## Components

### 1. **RPG Character Service**
- **Tech Stack:** C#, ASP.NET Core WebAPI, DynamoDb
- **Purpose:** Manages player characters, including:
  - Character creation (race, subrace, class)
  - Rolling ability scores
  - Managing hitpoints, inventory (armor, weapons)
  - Granting and modifying currencies
- **Persistence:** Characters are stored in a DynamoDb database.
- **Endpoints:** RESTful APIs documented with Swagger.

> **Local Dev:**

First time only:
> `cp rpg-character-service/src/.env.local rpg-character-service/src/.env`

Run the service. Uses in-memory database.
> `cd rpg-character-service && dotnet run`

---

### 2. **Game Simulation Service** (Planned)
- **Tech Stack:** (Planned) C# console app (or Go)
- **Purpose:** Hosts real-time game session logic.
- **Communication:** Clients connect over **UDP**.
- **Deployment:** Will scale horizontally based on game session counts.

---

### 3. **Infrastructure**
- **Tool:** Terraform
- **Purpose:** Provision AWS infrastructure, including:
  - EC2 instances for Character Manager and Game Simulation services
  - DynamoDb database
  - S3 bucket for static assets
  - Application Load Balancer (ALB) for HTTPS traffic
- **Environment:** Infrastructure as code (IaC) supporting repeatable and reliable deployment.

## Build Instructions

### Cloud Build

See [Infra Readme](infra/README.md)

### Local Build

1. **Start a local DynamoDB instance**
    (Docker recommended):
    ```
    docker run --name dynamodb-local -p 8000:8000 -d amazon/dynamodb-local
    ```

2. **Run the Character Service API:**
   ```
   cd rpg-character-service
   dotnet run
   ```

3. **Access Swagger UI:**
   - https://localhost:5001/swagger/index.html

    This UI let's you test the REST API and see the full documentation

4. **Test with Web client**

    If you feel daring, try out https://github.com/dcasadevall/rpg-character-management-client. A web based
    client for this project.

---

## Generate Swagger Documentation:

   - Install the Swagger CLI tool (if not already installed):
     ```bash
     dotnet tool install -g Swashbuckle.AspNetCore.Cli
     ```
   - Generate the OpenAPI/Swagger JSON file:
     ```bash
     cd rpg-character-service/src
     swagger tofile --output swagger.json bin/Debug/net9.0/rpg-character-service.dll v1
     ```
   - The generated `swagger.json` file can be used to:
     - Import into API documentation tools (Swagger UI, ReDoc, Postman)
     - Generate client SDKs
     - Share API documentation with other developers
     - API testing and validation

### Convert to Markdown Documentation:**
  - Install Widdershins (converts OpenAPI/Swagger JSON to Markdown):
    ```bash
    npm install -g widdershins
    ```
  - Generate Markdown documentation:
    ```bash
    widdershins swagger.json -o ../../docs/rpg-service-api.md
    ```

### Generate Test Coverage Reports:**
  - Install the ReportGenerator tool globally:
    ```bash
    dotnet tool install -g dotnet-reportgenerator-globaltool
    ```
  - Run tests with coverage collection:
    ```bash
    dotnet test --collect:"XPlat Code Coverage" --results-directory:./TestResults --filter:"FullyQualifiedName!~Integration"
    ```
  - Generate HTML coverage report:
    ```bash
    reportgenerator "-reports:./TestResults/*/coverage.cobertura.xml" "-targetdir:./TestResults/CoverageReport" "-reporttypes:Html"
    ```
  - Open the generated report:
    ```bash
    open ./TestResults/CoverageReport/index.html
    ```

---

## Future Work

- Complete Game Simulation server.

---

## License

This project is licensed under the MIT License.
