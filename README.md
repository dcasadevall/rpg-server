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
| `infrastructure/` | Terraform configuration for deploying services and infrastructure into AWS. |
| `scripts/` | Helper scripts to build, deploy, and manage services locally and remotely. |

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

> **Deploy Infra:**

#### Requirements

Configure AWS Authentication

* Go to the AWS Console → IAM → Users → Your User → Security Credentials → and make sure you have valid Access Key + Secret Access Key

* Install AWS: https://aws.amazon.com/cli/

* Configure credentials:

`aws configure`

OR Setup env vars:

```
   export AWS_ACCESS_KEY_ID="your_access_key"
   export AWS_SECRET_ACCESS_KEY="your_secret_key"
   export AWS_REGION="us-east-1"
```

* Setup your IAM name:

Add this to your bash profile:
`export TF_VAR_iam_username="YourUsername"`

* PERMISSIONS NOTE:

You may need to create a root access key. This is not recommended in AWS but its fine for this project.

#### Commands

> `cd infrastructure && terraform init && terraform apply`

#### Environment-Specific Deployment

The infrastructure supports multiple environments (dev, prod) with separate resources:

1. **Development Environment**:
   ```bash
   terraform apply -var="environment=dev"
   ```
   - Creates tables: `characters-dev` and `items-dev`
   - Uses development-specific IAM roles and policies
   - Lower capacity settings

2. **Production Environment**:
   ```bash
   terraform apply -var="environment=prod"
   ```
   - Creates tables: `characters-prod` and `items-prod`
   - Uses production-specific IAM roles and policies
   - Higher capacity settings

> **Note:** Each environment has its own:
> - DynamoDB tables
> - IAM roles and policies
> - Resource tags
> - Capacity settings

---

## Local Development Setup

1. **Start a local DynamoDB instance**
   (Docker recommended):
   ```
   docker run --name dynamodb-local -p 8000:8000 -d amazon/dynamodb-local
   ```

2. **Clone the repository:**
   ```
   git clone https://github.com/dcasadevall/rpg-server.git
   cd rpg-server
   ```

3. **Update the AWS configuration** in `rpg-character-service/appsettings.Development.json`:
   ```json
   {
     "AWS": {
       "Region": "us-west-2",
       "ServiceURL": "http://localhost:8000"
     }
   }
   ```

4. **Run the Character Service API:**
   ```
   cd rpg-character-service
   dotnet run
   ```

5. **Access Swagger UI:**
   - https://localhost:5001/swagger/index.html

6. **Generate Swagger Documentation:**
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

   - **Convert to Markdown Documentation:**
     - Install Widdershins (converts OpenAPI/Swagger JSON to Markdown):
       ```bash
       npm install -g widdershins
       ```
     - Generate Markdown documentation:
       ```bash
       widdershins swagger.json -o ../../docs/rpg-service-api.md
       ```

   - **Generate Test Coverage Reports:**
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

## Build and Deployment

Scripts for building and deploying are available inside the `scripts/` directory.

| Command | Description |
|:---|:---|
| `scripts/build-all.sh` | Build all services. |
| `scripts/deploy-infra.sh` | Deploy infrastructure with Terraform. |
| `scripts/start-local-postgres.sh` | (Optional) Start local Postgres via Docker. |

> Makefile-based orchestration for even easier local and CI builds.

---

## Future Work

- Complete Game Simulation server.

---

## License

This project is licensed under the MIT License.

---

# Summary

**This monorepo contains everything needed to run, manage, and deploy an RPG backend system both locally and in AWS.**
It is designed to be modular, scalable, and easy to extend as the game evolves.
