# RPG Server

This repository contains the backend services and infrastructure for a real-time RPG game inspired by Dungeons & Dragons mechanics.
It is structured as a **monorepo** to support easier local development, deployment, and scaling.

---

## Project Structure

| Directory | Description |
|:---|:---|
| `rpg-character-service/` | C# ASP.NET Core WebAPI for player character creation and management (REST APIs). |
| `game-simulation/` | Lightweight UDP-based service responsible for simulating real-time game sessions. |
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
> `cd infrastructure && terraform init && terraform apply`

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
- Dockerize both services and deploy to AWS ECS.
- Implement CI/CD pipelines (GitHub Actions).
- Blue/Green deployments for minimal downtime.

---

## License

This project is licensed under the MIT License.

---

# Summary

**This monorepo contains everything needed to run, manage, and deploy an RPG backend system both locally and in AWS.**
It is designed to be modular, scalable, and easy to extend as the game evolves.
