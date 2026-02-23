# The Golden Fork — Tipping & Roster App

A small full-stack app for managing restaurant rosters and splitting tips proportionally based on hours worked each week.

---

## How to run it

The easiest way is Docker — it spins up the database, backend and frontend all at once:

```bash
docker compose up --build
```

Then open **http://localhost:3000**. The database is created and seeded with some realistic data automatically on first start.

To stop it:

```bash
docker compose down
```

Want a clean slate? Run `docker compose down -v` first to wipe the database volume, then `up --build` again.

### Running locally without Docker

You'll need .NET 10 SDK and Node 22+, plus a PostgreSQL instance running on the default port.

**Backend:**
```bash
cd backend
dotnet run --project src/TippingApp.Api
```

**Frontend** (in a separate terminal):
```bash
cd frontend
npm install
npm run dev
```

The frontend dev server proxies `/api` to `localhost:5000` automatically.

---

## Assumptions I made

- **Weeks run Monday to Sunday** (ISO week) — felt like the natural choice for a restaurant schedule.
- **Currency is EUR** — the brief used € in the examples.
- **No overnight shifts** — the app assumes a shift ends on the same day it starts.
- **Tips are a shared weekly pool** — everyone in the roster splits the week's total proportionally by hours. No per-role or per-day pools, though the data model could support it.
- **No authentication** — this is meant as an internal tool so I left auth out. It would be the first thing to add before putting it in front of real users.

---

## What I'd improve with more time

- **Auth** — even a simple username/password login would be needed in production. JWT with a manager role for editing employees/tips, read-only for everyone else.
- **Role-based tip pools** — a common real-world scenario where bartenders split bar tips separately from floor staff.
- **PDF/CSV export** — managers usually want to print or file the weekly summary for payroll.
- **Better validation feedback** — right now errors surface as snackbars; proper inline form validation would feel much nicer.
- **Mobile layout** — the roster table works on desktop but gets cramped on a phone. Worth a responsive rework.
- **Integration tests** — unit tests cover the calculation logic, but end-to-end API tests with a real database (TestContainers) would give much more confidence.
