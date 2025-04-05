# RPG Server

This repository contains the backend services and infrastructure for a real-time RPG game inspired by Dungeons & Dragons mechanics.
It is structured as a **monorepo** to support easier local development, deployment, and scaling.

---

## Project Structure

| Directory | Description |
|:---|:---|
| `character-manager/` | C# ASP.NET Core WebAPI for player character creation and management (REST APIs). |
| `game-simulation/` | Lightweight UDP-based service responsible for simulating real-time game sessions. |
| `infrastructure/` | Terraform configuration for deploying services and infrastructure into AWS. |
| `scripts/` | Helper scripts to build, deploy, and manage services locally and remotely. |

---

## Components

### 1. **Character Manager**
- **Tech Stack:** C#, ASP.NET Core WebAPI, PostgreSQL, Entity Framework Core
- **Purpose:** Manages player characters, including:
  - Character creation (race, subrace, class)
  - Rolling ability scores
  - Managing hitpoints, inventory (armor, weapons)
  - Granting and modifying currencies
- **Persistence:** Characters are stored in a PostgreSQL database.
- **Endpoints:** RESTful APIs documented with Swagger.

> **Local Dev:**  
> `cd character-manager && dotnet run`

---

### 2. **Game Simulation Service** (Planned)
- **Tech Stack:** (Planned) C# console app (or Go)
- **Purpose:** Hosts real-time game session logic.
- **Communication:** Clients connect over **UDP**.
- **Deployment:** Will scale horizontally based on game session counts.

> **Note:** Initial scaffolding is in place; core session logic will be implemented later.

---

### 3. **Infrastructure**
- **Tool:** Terraform
- **Purpose:** Provision AWS infrastructure, including:
  - EC2 instances for Character Manager and Game Simulation services
  - RDS (PostgreSQL) database
  - S3 bucket for static assets
  - Application Load Balancer (ALB) for HTTPS traffic
- **Environment:** Infrastructure as code (IaC) supporting repeatable and reliable deployment.

> **Deploy Infra:**  
> `cd infrastructure && terraform init && terraform apply`

---

## Local Development Setup

1. **Start a local PostgreSQL database**  
   (Docker recommended):
   ```
   docker run --name postgres-rpg -e POSTGRES_PASSWORD=yourpassword -p 5432:5432 -d postgres
   ```

2. **Clone the repository:**
   ```
   git clone https://github.com/dcasadevall/rpg-server.git
   cd rpg-server
   ```

3. **Update the connection string** in `character-manager/appsettings.Development.json`.

4. **Apply database migrations:**
   ```
   cd character-manager
   dotnet ef database update
   ```

5. **Run the Character Manager API:**
   ```
   dotnet run
   ```

6. **Access Swagger UI:**
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
