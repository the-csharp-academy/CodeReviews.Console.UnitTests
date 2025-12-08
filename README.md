# Coding Tracker 🕒

A console-based application allowing users to track their coding sessions. This project helps monitor coding habits by recording start and end times, calculating durations, and saving data to a local database.

## 🚀 Features

* **Start/Stop Session:** Use a stopwatch to record a live coding session.
* **Manual Entry:** Manually input start and end times with validation.
* **View History:** View all past sessions in a formatted table.
* **Update/Delete:** Modify or remove specific sessions.
* **Filtering:** Order sessions by date (ascending/descending).
* **Data Persistence:** All data is saved in a SQLite database.

## 🛠️ Technologies & Patterns

* **Language:** C# (.NET)
* **Database:** SQLite
* **ORM:** Dapper (for efficient data access)
* **UI:** Spectre.Console (for a beautiful console interface)
* **Testing:**
    * **xUnit:** Test runner.
    * **FakeItEasy:** Used for mocking dependencies.
    * **FluentAssertions:** For readable test assertions.
* **Architecture:**
    * Implements **Dependency Injection** to decouple the Controller from the Service.
    * Separation of concerns (UI, Controller, Service, Data layers).

## 🧪 Testing

The project includes a comprehensive test suite:
1.  **Unit Tests:** Validates helper logic (date parsing) and Controller logic (using Mocks to isolate the UI from the Database).
2.  **Integration Tests:** Verifies database operations to ensure data is correctly inserted and retrieved without polluting the local file system.

## 🏁 Getting Started

1.  Clone the repository.
2.  Open the solution in Visual Studio or Rider.
3.  Ensure the `appsettings.json` (or `app.config`) contains the correct connection string for SQLite.
4.  Run the application. The database table will be created automatically on the first run.

## 🧠 Challenges & Lessons Learned

* **Testing Legacy Code:** Refactored the application to use **Dependency Injection**, transforming a hard-to-test tightly coupled architecture into a testable, modular design.
* **Mocking:** Learned how to use Mock objects to test business logic without relying on the actual database or user input.

---
*Created as part of the C# Academy curriculum.*
