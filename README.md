# The Golden Fork — Tipping & Roster App

A full-stack web application that allows a restaurant business to manage employee rosters and calculate tip splits proportionally based on hours worked.

## Architecture

```
┌────────────┐      ┌──────────────────┐      ┌──────────────┐
│  Frontend   │──── │   Backend API    │──── │  PostgreSQL  │
│  React/MUI  │ HTTP│  ASP.NET Core    │  EF │    16        │
│  Vite/TS    │     │  C#              │ Core│              │
│  :3000      │     │  :5000           │     │  :5432       │
└────────────┘      └──────────────────┘      └──────────────┘
      nginx proxy /api → backend:8080
```

### Tech Stack

| Layer      | Technology                                      |
|------------|------------------------------------------------|
| Frontend   | React 18, TypeScript, Vite, Material UI (MUI)  |
| Backend    | ASP.NET Core 10, Entity Framework Core, C#     |
| Database   | PostgreSQL 16                                   |
| Testing    | xUnit + FluentAssertions (backend), Vitest (frontend) |
| Deployment | Docker Compose (3 services)                     |

## Quick Start

### Prerequisites

- [Docker](https://www.docker.com/) and Docker Compose installed

### Run

```bash
docker compose up --build
```

Then open **http://localhost:3000** in your browser.

The database is automatically created and seeded with realistic data on first startup.

### Stop

```bash
docker compose down
```

To reset the database (fresh seed):

```bash
docker compose down -v
docker compose up --build
```

## Running Without Docker

### Backend

Requires .NET 10 SDK and a running PostgreSQL instance.

```bash
cd backend
dotnet run --project src/TippingApp.Api
```

### Frontend

Requires Node.js 22+.

```bash
cd frontend
npm install
npm run dev
```

The Vite dev server proxies `/api` requests to `http://localhost:5000`.

## Running Tests

### Backend (xUnit)

```bash
cd backend
dotnet test
```

**13 tests** covering:
- Proportional tip split logic (7 tests): basic splits, edge cases (zero hours, single employee, rounding)
- Weekly hours calculation (6 tests): standard shifts, multi-shift sums, evening shifts, odd minutes

### Frontend (Vitest)

```bash
cd frontend
npx vitest run
```

**12 tests** covering:
- Tip split calculation (7 tests): proportional distribution, zero-hour/zero-tip edges, rounding
- Shift hours calculation (3 tests): standard, half-hour, evening
- Total hours aggregation (2 tests): multi-shift sums, empty arrays

## Features

### Roster Management
- Weekly grid view with employees as rows and days of the week as columns
- Navigate between weeks (previous/next) with current-week indicator
- Add, edit, and delete shifts via dialog
- Total hours per employee displayed per week

### Tips & Split Calculation
- View all tip entries for the selected week, grouped by day
- Add, edit, and delete tip entries with source labels (Card tips, Cash tips, Brunch tips, etc.)
- Automatic proportional tip split based on hours worked
- Visual distribution bars showing each employee's share
- Percentage breakdown per employee

### Previous Weeks (Stretch Goal)
- Full week navigation allows viewing any past or future week
- 8 weeks of realistic historical seed data available out of the box

### Realistic Seed Data
- 10 employees with diverse roles (waitresses, bartenders, hostess, barback, floor manager)
- Mix of full-time and part-time scheduling patterns
- ~250-300 shifts across 8 weeks with realistic restaurant scheduling
- Daily tip entries with card/cash split, weekend premiums, occasional private events
- Non-round amounts for realism (e.g., €147.50, €83.20)

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/employees` | List all employees |
| GET | `/api/shifts?weekStart=2026-02-23` | Shifts for a given week |
| POST | `/api/shifts` | Create a shift |
| PUT | `/api/shifts/{id}` | Update a shift |
| DELETE | `/api/shifts/{id}` | Delete a shift |
| GET | `/api/tips?weekStart=2026-02-23` | Tip entries for a week |
| POST | `/api/tips` | Create a tip entry |
| PUT | `/api/tips/{id}` | Update a tip entry |
| DELETE | `/api/tips/{id}` | Delete a tip entry |
| GET | `/api/weekly-summary?weekStart=2026-02-23` | Full weekly summary with tip splits |

## Data Model

- **Employee**: id, name, role
- **Shift**: id, employeeId, date, startTime, endTime
- **TipEntry**: id, date, amount, source

Tips are modeled as individual daily transactions (not weekly totals) for flexibility — allows different sources (card, cash, events) and per-day granularity.

## Assumptions

- **Week definition**: ISO week (Monday to Sunday)
- **Currency**: EUR (Euro), as the task used € in examples
- **No authentication**: This is a single-user internal tool; auth would be a production addition
- **No overnight shifts**: Restaurant closes by midnight; all shifts have endTime > startTime on the same day
- **Tip pool is shared**: All employees in the roster share the same tip pool proportionally — no role-based or day-based pools (though the data model supports extending this)
- **Seed data is deterministic**: Uses a fixed random seed so Docker restarts produce identical data

## What I'd Improve With More Time

- **Authentication & authorization**: Add JWT-based auth with role-based access (manager vs. staff)
- **Role-based tip pools**: Different tip pool rules (e.g., bartenders get bar tips, waitstaff get table tips)
- **Day-based tip pools**: Per-day pool assignment instead of weekly aggregate (stretch goal mentioned)
- **Real-time updates**: WebSocket/SignalR for live roster changes across multiple users
- **PDF/CSV export**: Generate weekly reports for payroll
- **Input validation**: More comprehensive server-side validation with problem details responses
- **Logging & monitoring**: Structured logging with Serilog, health checks endpoint
- **CI/CD pipeline**: GitHub Actions for build, test, and Docker image publishing
- **Mobile responsiveness**: The current UI works on desktop; would optimize for tablets/phones
- **Integration tests**: API integration tests with TestContainers for PostgreSQL
