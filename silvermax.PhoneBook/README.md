# PhoneBook

A console phone book. You add, list, update and delete contacts, and you can send a contact an email or an SMS from inside the app. Contacts are stored in SQL Server.

## What it does

The main menu offers:

- Add, update, list and delete contacts (name, email, phone number)
- Send an email to a contact (through Gmail's SMTP server)
- Send an SMS to a contact (through Twilio)

Names and phone numbers are unique. Phone numbers are validated with libphonenumber, so they have to start with `+` and a country code (for example `+442071838750`). On first start the database is created and filled with 10 sample contacts.

## Requirements

- .NET 10 SDK
- SQL Server running locally (the default connection string uses `Server=localhost` with Windows authentication)

## Setup and run

1. Clone the repository and open the `silvermax.PhoneBook` folder.
2. Check the connection string in `appsettings.json`. If your SQL Server instance isn't `localhost`, change `DefaultConnection`.
3. Run the app:

   ```
   dotnet run --project silvermax.PhoneBook.csproj
   ```

You don't need to run any migration command. At startup the app applies the EF Core migrations and creates the `Contacts` database if it's missing.

### Email and SMS (optional)

Listing, adding, updating and deleting work without any extra setup. To send email or SMS you need credentials, which are kept in .NET user secrets so they never end up in the repository:

```
dotnet user-secrets set "Email:Personal" "you@gmail.com"
dotnet user-secrets set "Email:Password" "your-gmail-app-password"
dotnet user-secrets set "Twilio:AccountSID" "..."
dotnet user-secrets set "Twilio:AuthToken" "..."
dotnet user-secrets set "Twilio:PhoneNumber" "+1..."
```

Run these from the project folder. Gmail needs an app password, not your normal password. Without these values the app shows a message like "SMTP credentials not configured" and goes back to the menu.

## Running the tests

```
dotnet test silvermax.PhoneBook.slnx
```

SQL Server has to be running. The integration tests create a temporary database named `ContactsTestDb_<random>` (using the `DefaultTestConnection` string) and drop it when they finish. They never touch your real `Contacts` database, and they never send a real email or SMS.

## Project layout

```
silvermax.PhoneBook/
  Services/                 ContactService, MailService, SMSService
  DbAcess/                  EF Core DbContext
  Migrations/               EF Core migrations
  Dtos/                     data passed between input and services
  UserInput.cs              all prompts and input validation
  UserInterface.cs          the menu loop
  DataSeeder.cs             migrates the database and seeds sample contacts
  PhoneBook.IntegrationTests/   tests against a real SQL Server database
  Phonebook.UnitTests/          tests that need no database
```

## How it works

`Program.cs` builds a host with the configuration (appsettings and user secrets) and an EF Core `DbContext`, runs the seeder, creates the three services and starts the menu in `UserInterface`. The UI uses Spectre.Console for menus, prompts and tables. `UserInput` asks the questions and validates the answers, the services do the database and sending work, and `UserInterface` only decides which service to call.

Some choices worth explaining:

- **EF Core with migrations.** The schema, including the unique indexes on name and phone number, lives in the migrations. The database enforces uniqueness, and the service also checks the phone number first to give a friendly message.
- **Services are created by hand in `Program.cs`**, not registered in the container. That was simple when I wrote the app, but it made testing harder, as the reflection below explains.
- **Secrets stay out of the repo.** Credentials are read from user secrets, and `appsettings.json` has only local connection strings.

## Tests

- **Integration tests** (`PhoneBook.IntegrationTests`) run the real services against a real SQL Server database. They cover `ContactService` (list, add, update, delete, duplicates), `DataSeeder`, and the paths in `MailService` and `SMSService` that return before any network call. A shared fixture creates one database per run and clears the table before each test. Console input is scripted with Spectre's `TestConsole`.
- **Unit tests** (`Phonebook.UnitTests`) cover `UserInput`: valid answers, and invalid emails and phone numbers followed by a valid one.

What the tests don't cover: actually sending an email or SMS. That would need the network and real credentials, or an interface in front of `SmtpClient` and Twilio that a test could fake.

## Reflection

The app was already implementd and added the tests afterwards. That order turned out to be the main lesson of this project.

The app worked, but it wasn't written to be tested. The first test I wrote failed with `The handle is invalid`, because `Console.Clear()` doesn't work when a test runner captures the output. The next one hung forever, because `Console.ReadKey()` was waiting for a key press that nobody would ever send. Both problems came from the app talking directly to the console. I ended up putting the console calls behind small helpers in `UIHelper` and reading the "press any key" input through Spectre, so a test console can answer it.

The database caused the other big block of trouble. All the tests shared one SQL Server database, and for a while they failed in different ways on each run: "database already exists", duplicate key errors from leftover rows, and prompts that ran out of input because tests were stepping on each other's global `AnsiConsole.Console`. Fixing it took several steps. I gave every run its own database name, cleared the table before each test, put all database tests in one xunit collection, and finally turned off test parallelization. I also lost time to simple mistakes, like a missing `await` that made a test assert before the service had finished, and a connection string line that ignored the unique name I had just built.

Those problems taught me more than the tests themselves did. When a test is hard to write, the code is often telling you something. If the services took an interface for user input and for sending mail and SMS, most of these tests would be five-line unit tests, and I wouldn't need a real database or a fake console for them. I didn't refactor the app for this assignment, so some of the testing is more roundabout than it should be. Next time I'll design for testing from the start.

Along the way, the tests found real issues in my app: the duplicate-name error shows a "phone number already exists" message, update and add handle name casing differently, and the delete confirmation prompt prints the literal text `{contactToDelete}` instead of the name.
