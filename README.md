# BMT CMS — Backend (.NET 8 · PostgreSQL · Docker)

Layered (N-tier) .NET 8 Web API for the Next.js admin dashboard.
**No local .NET SDK required** — builds and runs entirely in Docker.

## Structure

```
cms-backend/
├── .docker/
│   └── Dockerfile               # multi-stage build (SDK -> aspnet runtime)
├── .github/workflows/
│   └── ci.yml                   # build + docker image on push/PR
├── Cms.API/                     # presentation: controllers, Program.cs, config
├── Cms.Service/                 # business: auth, JWT, DTOs, seeder
├── Cms.Repository/              # data: DbContext, entities, repos, migrations
├── documentation/
│   └── architecture.md
├── .dockerignore
├── .gitignore
├── docker-compose.yml           # webapi + postgres_db
├── global.json                  # pins .NET 8 SDK
└── Shopcms.sln
```

## Run

```bash
cd cms-backend
docker compose up --build
```

On startup the API waits for PostgreSQL, applies EF Core migrations
(creating the `Users` table), and seeds a default admin.

- API:     http://localhost:5080
- Swagger: http://localhost:5080/swagger
- Postgres: localhost:5432 (`cms_user` / `cms_password` / `cms_db`)

## Default admin

| Username | Password    |
|----------|-------------|
| `admin`  | `Admin@123` |

> Change this after first login.

## Endpoints

| Method | Route            | Auth        | Body / Result |
|--------|------------------|-------------|---------------|
| POST   | `/api/auth/login`| Anonymous   | `{ "username", "password" }` → `{ token, tokenType, expiresInMinutes }` |
| GET    | `/api/auth/me`   | Bearer JWT  | → `{ id, username, email, role }` |

## Calling from Next.js (http://localhost:3000)

```ts
const res = await fetch("http://localhost:5080/api/auth/login", {
  method: "POST",
  headers: { "Content-Type": "application/json" },
  body: JSON.stringify({ username: "admin", password: "Admin@123" }),
});
const { token } = await res.json();

const me = await fetch("http://localhost:5080/api/auth/me", {
  headers: { Authorization: `Bearer ${token}` },
});
```

CORS allows `http://localhost:3000`.

## Useful commands

```bash
docker compose up --build        # build + run
docker compose up --build -d     # detached
docker compose logs -f webapi    # follow API logs
docker compose down              # stop
docker compose down -v           # stop + wipe the database volume
```

## Adding future migrations (no local SDK)

Migrations are committed by hand in `Cms.Repository/Migrations`. To scaffold new
ones with a throwaway SDK container (run from the repo root):

```bash
docker run --rm -v "$(pwd):/src" -w /src mcr.microsoft.com/dotnet/sdk:8.0 bash -c "\
  dotnet tool install --global dotnet-ef && export PATH=\$PATH:/root/.dotnet/tools && \
  dotnet ef migrations add <Name> --project Cms.Repository --startup-project Cms.API"
```
