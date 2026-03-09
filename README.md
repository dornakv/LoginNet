This is a starting point for a web application with authentication and user+role based authorization.

## Setup
### Devcontainer
Create a `docker/.env` file with 
- `POSTGRES_PASSWORD`
  - Will be used by the `docker/docker-compose.postgres.yml` for the user `loginnetuser`
- `ConnectionStrings__DefaultConnection`
  - Use `Host=host.docker.internal` to connect from dev container to PostgresSQL container running on host
  - Use the same password as `POSTGRES_PASSWORD`
- `Jwt__Key`
  - 256 bit key (`openssl rand -base64 32`)
Use `docker/.env.example` for inspiration.

Launch the postgres container:
```bash
docker-compose -f docker/docker-compose.postgres.yml up -d
```

Launch the dev container in VS Code:
- Press `cmd+shift+p` (macOS)/ `ctrl+shift+p` (Windows)
- Select "Dev Containers: Reopen in Container"

### Local DEV
Create a `docker/.env` file with 
- `POSTGRES_PASSWORD`
  - Will be used by the `docker/docker-compose.postgres.yml` for the user `loginnetuser`
Use `docker/.env.example` for inspiration.

Launch the postgres container:
```bash
docker-compose -f docker/docker-compose.postgres.yml up -d
```

Register dotnet secrets:
```bash
dotnet user-secrets --project src/LoginNet.WebApi/LoginNet.WebApi.csproj set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=loginnetdb;Username=loginnetuser;Password=your_strong_password_here" 
dotnet user-secrets --project src/LoginNet.WebApi/LoginNet.WebApi.csproj set "Jwt:Key" "JWT_SECRET_KEY"
```
- Replace POSTGES_PASSWORD with the password set in the `docker/env` file
- Generate JWT_SECRET_KEY `openssl rand -base64 32`

Set `Development` environment:
```
export ASPNETCORE_ENVIRONMENT=Development
```
- Dotnet secrets are used only in `Developlment` not in `Production`
- For `Production` environment, set env variables `JWT_SECRET_KEY` and `POSTGRES_PASSWORD`