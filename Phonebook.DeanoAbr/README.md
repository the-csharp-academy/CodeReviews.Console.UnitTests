# Phonebook — C# Academy (Entity Framework)

## ✅ How to run the project

**Prerequisites:** .NET 8 SDK (pinned via `global.json`).

```bash
dotnet run    # starts the app (creates phonebook.db on first run)
dotnet test   # runs the validation unit tests (16 tests)
```

The app is fully usable without any configuration. Email and SMS run in **sandbox mode** by default: they compose the message and show it on screen instead of sending. To send for real, set these environment variables before running:

| Feature | Variables |
|---|---|
| Email (SMTP) | `SMTP_HOST`, `SMTP_PORT`, `SMTP_USER`, `SMTP_PASS`, `SMTP_FROM` |
| SMS (Twilio) | `TWILIO_ACCOUNT_SID`, `TWILIO_AUTH_TOKEN`, `TWILIO_FROM_NUMBER` |

> Twilio expects phone numbers in E.164 format (e.g. `+27821234567`) for real sends.

## ✅ What the app does and how it works

A menu-driven console phonebook where you can:

- **View** all contacts (name, email, phone, category) in a table
- **Add** contacts with validation — e-mails must look like `name@example.com`, phone numbers must match one of the accepted formats (`+27 82 123 4567`, `082 123 4567`, `(011) 555-0198`, ...) — the prompt tells you what's expected and re-asks until valid
- **Update** and **Delete** contacts by Id
- **Send email** (MailKit/SMTP) and **SMS** (Twilio REST) to a contact
- Categorize contacts: **Family / Friends / Work / Other**

How it works:

- **Entity Framework Core, Code-First**: EF builds the SQLite database (`phonebook.db`) and schema from the C# model on first run (`Database.EnsureCreated()`), and seeds three starter contacts via `HasData()`.
- **SQL transparency**: every query EF sends is printed to the console (including parameter values) so you can see how your C# LINQ translates into SQL — a deliberate learning feature.
- **Error handling**: every database/EF/SMTP/provider operation is wrapped and returns a result tuple; failures show a friendly red message instead of crashing the app.

## ✅ Architectural choices

| Layer | Responsibility |
|---|---|
| `src/Models` | `Contact` entity + `ContactCategory` enum |
| `src/Data` | `AppDbContext` — SQLite connection, schema config, `HasData` seeding, SQL logging |
| `src/Services` | `ContactService` (EF CRUD), `ValidationService` (name/email/phone), `EmailService` (MailKit), `SmsService` (Twilio REST) |
| `src/Controllers` | `ContactController` — menu flow, validation loops, all use-case orchestration |
| `src/UI` | `UserInterface` — Spectre.Console prompts, tables, format hints, messages |
| `src/Program.cs` | Dependency wiring + top-level exception guard |
| `tests/Phonebook.Tests` | xUnit tests for the validation logic |

Key decisions and why:

- **Entity Framework over Dapper/ADO.NET** — the assignment requirement, and the industry-standard ORM.
- **SQLite** — zero-install, single-file database; ideal for a learning console app.
- **`EnsureCreated()` instead of migrations** — the simplest Code-First path for a fixed schema; migrations are the natural upgrade if the schema grows.
- **Seeding via `HasData()`** — EF-native: seed rows are part of the model configuration and inserted when the schema is created.
- **Categories stored as strings** (`.HasConversion<string>()`) — the DB column reads `"Family"` instead of `0`, and the enum gives compile-time safety in C#.
- **Result-tuple error pattern** — every service method returns `(Success, Data/Message)` and never lets an exception escape, so the UI can always respond gracefully.
- **Sandbox-first communication services** — SMTP/Twilio credentials come from environment variables; without them the app composes and prints the message, so the feature is demonstrable with no secrets in the repo.
- **Spectre.Console** — readable tables and menus for the console UI.
- **Layered separation** — models → data → services → controller → UI keeps concerns isolated and testable; validation is the pure-logic seam covered by unit tests.

## ✅ Reflection

<!-- Personalize this before submitting! -->

This was my first real hands-on time with an ORM, and the biggest aha moment was turning on SQL logging. Watching my LINQ queries actually turn into real `SELECT`/`INSERT`/`UPDATE` statements — parameters and all — made Entity Framework feel a lot less like a black box. I could finally see what `SaveChanges()` was actually doing behind the scenes instead of just trusting it blindly.

The Code-First workflow was way smoother than I expected. I wrote a plain C# class, and EF made a whole database table out of it, crazy. Seeing the data pop up in the table on the first run was one of those "dam, this is actually working" moments.

Change tracking is what really broke my brain, though. I ran into a bug where updating a contact right after adding it would blow up with "another instance with the same key is already being tracked." Turns out EF only keeps one tracked instance per entity key, so you've got to detach the previously-added entity before you attach an updated copy. That was basically a live example of why the "handle errors so the app doesn't crash" requirement exists in the first place — and it's why every service method in this project hands back a result instead of just throwing an exception.

If I did it again, I'd probably ditch `EnsureCreated()` and actually dig into EF Migrations, since that's closer to how real projects handle schema changes over time. Next on the list: hooking up real email and SMS with actual credentials, and maybe throwing in a search feature.

---

*Built with .NET 8, Entity Framework Core 8 (SQLite), Spectre.Console, MailKit, and xUnit.*
