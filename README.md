# AIkeukenkast

AIkeukenkast consists of three parts:

- `api`: ASP.NET Core Web API for scans, products, municipalities, and storage
- `AI-model`: FastAPI prediction service that analyzes uploaded images
- `client`: SvelteKit frontend for importing scans and browsing results

## Prerequisites

Install these tools before you start:

- .NET SDK 10
- Node.js 20 or newer
- Python 3.10 or newer
- SQL Server 2022, or Docker if you want to use the provided database container

## Repository Setup

Clone the repository and open the root folder `AIkeukenkast`.

If you want to run the SQL Server locally with Docker, start the database container first:

```bash
docker compose up -d sqlserver
```

The container exposes SQL Server on port `1433` and uses the password configured in `docker-compose.yml`.

## Configure The Projects

### API

The API reads its connection strings from `api/appsettings.json` and `api/appsettings.Development.json`.

Configure these values for your environment:

- `ConnectionStrings:DefaultConnection` for SQL Server
- `ConnectionStrings:AzureBlobStorage` for Azure Blob Storage

### AI-model

The FastAPI service reads environment variables from `AI-model/.env`.

Set these variables before starting the service:

- `PREDICTION_KEY`
- `ENDPOINT_PREDICTION`

### Client

The client uses `http://localhost:5141` by default for API calls.

If your API runs elsewhere, set `PUBLIC_API_BASE_URL` in the client environment.

## Install Dependencies

### API

```bash
cd api
dotnet restore
```

### AI-model

Create and activate a virtual environment, then install the Python packages:

```bash
cd AI-model
python -m venv .venv
.venv\Scripts\activate
pip install -r requirements.txt
```

### Client

```bash
cd client
npm install
```

## Build

### API

```bash
cd api
dotnet build
```

You can also build the full solution from the repository root:

```bash
dotnet build AIkeukenkast.sln
```

### AI-model

The AI service does not need a separate build step. Installing the Python dependencies is enough.

### Client

```bash
cd client
npm run build
```

## Run Locally

Run the services in this order:

1. Start SQL Server if needed.
2. Start the AI-model service.
3. Start the API.
4. Start the client.

### 1. Start the AI-model service

From the `AI-model` folder:

```bash
uvicorn AzureCostumVision:app --reload --host 0.0.0.0 --port 8000
```

This starts the prediction endpoint used by the API at `http://127.0.0.1:8000/predict`.

### 2. Start the API

From the `api` folder:

```bash
dotnet run
```

The API runs on the configured local port, which is `5141` in the current client setup.

If you changed the database schema, apply migrations first:

```bash
dotnet ef database update
```

### 3. Start the client

From the `client` folder:

```bash
npm run dev
```

Open the app in your browser at the address shown by Vite, usually `http://localhost:5173`.

## Production Run

### API

```bash
cd api
dotnet run --configuration Release
```

### AI-model

```bash
cd AI-model
uvicorn AzureCostumVision:app --host 0.0.0.0 --port 8000
```

### Client

Build the frontend and run the generated Node server:

```bash
cd client
npm run build
npm run start
```

## Useful Commands

### API migrations

Create a new migration after changing the entity model:

```bash
cd api
dotnet ef migrations add <MigrationName>
```

Apply pending migrations:

```bash
cd api
dotnet ef database update
```

### Client checks

```bash
cd client
npm run check
```

### Client linting

```bash
cd client
npm run lint
```

## Project Notes

- The API uses SQL Server and Azure Blob Storage.
- The AI-model service is a separate FastAPI app and must be running for scan import to work.
- The frontend expects the API to be reachable on the configured base URL.
