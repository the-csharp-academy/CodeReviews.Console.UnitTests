# 📒 PhoneBook — Console Contact Management System

A console-based contact management application built with **C#, .NET, Entity Framework Core, SQL Server LocalDB, and Spectre.Console**. The application allows users to manage contacts, organize multiple phone numbers and email addresses, import existing contact data, generate reports, and send messages through email or SMS integrations.

## 📌 Features

- **Insert Contact:** Add a contact with first name, last name, organization, job title, notes, phone numbers, and email addresses.
- **View Contact:** Search for a contact and view its complete details.
- **View All Contacts:** Display saved contacts in a formatted console table.
- **Modify Contact:** Update existing contact details and add phone numbers or email addresses.
- **Delete Contact:** Find and delete a contact after confirmation.
- **Input Validation:** Validate names, phone numbers, and email addresses before accepting input.
- **Multiple Phone Numbers and Emails:** Store multiple contact details with labels such as Mobile, Home, Work, and Other.
- **Email Messaging:** Send emails through the Mailgun API.
- **SMS Messaging:** Send SMS messages through Twilio.
- **Import Contact Data:** Seed the database from supported CSV and Excel files.
- **Excel Reports:** Export contacts, phone numbers, and email addresses into separate worksheets.
- **PDF Reports:** Generate formatted PDF reports containing contact information.
- **Console Interface:** Use Spectre.Console for formatted tables, panels, prompts, and status messages.

## 🛠️ Technologies Used

| Technology | Purpose |
|---|---|
| C# | Application development |
| .NET 10 SDK | Runtime and development tools |
| Spectre.Console | Console UI and interactive prompts |
| Entity Framework Core | Database access and ORM |
| SQL Server LocalDB | Persistent contact storage |
| EF Core Migrations | Database schema management |
| ClosedXML | Excel report generation |
| CsvHelper | CSV file processing |
| ExcelDataReader | Reading Excel input files |
| QuestPDF | PDF report generation |
| Mailgun API | Email messaging |
| Twilio API | SMS messaging |
| NUnit | Automated testing |

## ⚙️ Prerequisites

Before running the project, install:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Visual Studio](https://visualstudio.microsoft.com/) with the .NET desktop development workload, or another compatible C# editor
- SQL Server LocalDB, usually installed with Visual Studio
- Git

For database migrations, install the EF Core command-line tool if it is not already available:

```bash
dotnet tool install --global dotnet-ef
```

If it is already installed, update it when needed:

```bash
dotnet tool update --global dotnet-ef
```

## 🚀 Getting Started

### 1. Clone the repository

Open a terminal and run:

```bash
git clone https://github.com/Sangeerths/PhoneBook.git
cd PhoneBook
```

### 2. Open the solution

Open `PhoneBook.slnx` in Visual Studio, or use the terminal.

Restore the dependencies:

```bash
dotnet restore
```

### 3. Configure the database

The application uses SQL Server LocalDB with the following connection string:

```text
Server=(localdb)\mssqllocaldb;Database=PhoneBookDb;Trusted_Connection=True;
```

Check `PhoneBook/Repository/PhoneBookContext.cs` to confirm the database configuration.

If the project does not automatically apply migrations on startup, run the following command from the repository root:

```bash
dotnet ef database update --project PhoneBook/PhoneBook.csproj
```

This creates or updates the database schema using the existing Entity Framework Core migrations.

### 4. Configure messaging credentials

Messaging is optional if you only want to manage contacts or generate reports.

The project includes an `env.sample` file. Copy it to `.env` in the location expected by the application and provide your own credentials.

Configure these variables:

```text
MailgunApiKey=your_mailgun_api_key
MailgunDomain=your_mailgun_domain
EmailFrom=your_sender_email

TwilioSid=your_twilio_account_sid
TwilioToken=your_twilio_auth_token
SmsFrom=your_twilio_phone_number
```

Use valid credentials from [Mailgun](https://www.mailgun.com/) and [Twilio](https://www.twilio.com/). Their trial accounts may restrict the recipients you can contact.

**Security:** Never commit your real `.env` file or API credentials to GitHub.

### 5. Run the application

From the repository root:

```bash
dotnet run --project PhoneBook/PhoneBook.csproj
```

The application opens an interactive console menu. Follow the prompts to manage contacts.

### 6. Run the tests

If the solution contains the integration test project, restore the packages and run:

```bash
dotnet test
```

Alternatively, run the specific test project:

```bash
dotnet test PhoneBook.IntegrationTests/PhoneBook.IntegrationTests.csproj
```

Ensure the test project path matches the actual project location in your checkout.

## 📂 Importing Contact Data

The application can seed an empty database from supported CSV or Excel input files.

Supported formats:

- `.csv`
- `.xls`
- `.xlsx`

The seed file should contain the columns expected by the import implementation:

```text
ContactId
FirstName
LastName
OrganizationName
JobTitle
Notes
CreatedAt
PhoneNumber
PhoneLabel
PhoneCreatedAt
EmailAddress
EmailLabel
EmailCreatedAt
EmailUpdatedAt
```

Each row represents a contact and its associated phone number and email information. A contact with multiple phone numbers or email addresses may occupy multiple rows.

Place the seed file in the location expected by `PhoneBookContext.SeedDataFromExcel()` and configure it to copy to the output directory when required.

The seeding process is intended to populate the database when it is empty, avoiding repeated insertion of the initial dataset.

## 📄 Generating Reports

The application supports two report formats:

### Excel Report

The Excel export creates a workbook with separate worksheets for:

- Contacts
- PhoneNumbers
- Emails

### PDF Report

The PDF export produces a formatted report containing the contact records and their associated phone numbers and email addresses. Generated reports are saved in the `Reports/` directory.

## 🏗️ Project Structure

```text
PhoneBook/
├── Controller/
│   └── ContactController.cs
├── Services/
│   └── ContactService.cs
├── Repository/
│   └── PhoneBookContext.cs
├── Models/
│   ├── Contact.cs
│   ├── PhoneNumberDetail.cs
│   └── EmailDetail.cs
├── Messaging/
│   └── MessageSender.cs
├── Validation/
│   └── InputValidator.cs
├── UI/
│   ├── ConsoleMenu.cs
│   └── ConsoleUI.cs
├── Migrations/
├── Program.cs
└── PhoneBook.csproj
```

The integration tests are maintained in the separate `PhoneBook.IntegrationTests` project.

## 🧱 Architectural Choices

### 1. Layered Architecture

The application separates responsibilities into controllers, services, data access, models, and UI components.

- **Controller:** Coordinates user actions, calls the service layer, and displays success or error messages.
- **Service:** Implements contact management operations and coordinates database interactions.
- **Repository/DbContext:** Uses Entity Framework Core to manage database access and entity relationships.
- **Models:** Represent contacts, phone numbers, and email addresses.
- **UI:** Handles menu navigation, prompts, and formatted console output.
- **Validation:** Keeps input validation separate from menu navigation.
- **Messaging:** Separates email and SMS integration logic from contact management.

This structure makes the code easier to understand and helps keep unrelated responsibilities separate.

### 2. Entity Framework Core

I used Entity Framework Core to work with contact data through C# models rather than writing SQL queries for every operation. Migrations help manage changes to the database schema over time.

### 3. Separate Contact Detail Entities

Phone numbers and email addresses are stored as separate entities related to a contact. This supports multiple numbers and email addresses per person without adding a fixed number of columns to the contact table.

### 4. Dedicated Validation

Input validation is separated from the console menu so that invalid contact details can be rejected before they are saved.

### 5. Separate Messaging Integration

Email and SMS functionality is organized in a dedicated messaging module. This keeps provider-specific API calls separate from ordinary contact CRUD operations.

### 6. Console UI Abstraction

Reusable console UI components help keep prompts, tables, status messages, and menu interactions consistent throughout the application.

## 🧪 Testing

The project includes an integration test project for testing application behavior alongside database operations.

Examples of useful test scenarios include:

- Inserting and retrieving a contact.
- Saving phone numbers and email addresses with a contact.
- Updating and deleting existing contacts.
- Searching for contacts by name.
- Handling searches for nonexistent contacts.
- Rejecting invalid contact details.
- Handling missing, unsupported, or corrupted import files.
- Verifying that invalid operations do not modify database records.

Tests should use an isolated test database or another controlled test environment so they do not modify personal development data.

## 💭 Reflection

While developing this project, I learned how the different parts of a C# console application work together. Working with Entity Framework Core helped me understand how models, database relationships, and migrations are connected.

I also learned the importance of separating responsibilities instead of putting all the logic inside the menu or controller. Keeping validation, database operations, console output, and messaging in separate components made the code easier to follow and maintain.

Importing contact data was another useful learning experience because files can contain missing, invalid, or incorrectly formatted values. I also explored generating Excel and PDF reports, which helped me understand how an application can present stored data in formats that are useful outside the program.

Integrating Mailgun and Twilio introduced me to external APIs and environment-based configuration. I had to understand how API credentials work and why sandbox or trial accounts may restrict sending messages.

Writing integration tests helped me see why checking console output alone is not enough. I also need to verify the actual database state and make sure tests do not depend on existing data.

Overall, this project helped me improve my understanding of C#, Entity Framework Core, layered architecture, validation, file processing, and testing. It also showed me areas I can improve further, especially dependency injection, test isolation, and making application operations easier to test.

## 🔮 Future Improvements

- Use dependency injection for controllers, services, and database contexts.
- Add more automated unit and integration tests.
- Improve validation and consistent error reporting.
- Use a dedicated test database for repeatable test execution.
- Add pagination and sorting for larger contact lists.
- Improve import error reports so users can identify invalid rows.
- Add structured logging for troubleshooting.
- Make messaging configuration and provider failures easier to diagnose.

## 👨‍💻 Author

**Sangeerth S**

- GitHub: [Sangeerths](https://github.com/Sangeerths)
- Repository: [PhoneBook](https://github.com/Sangeerths/PhoneBook)
