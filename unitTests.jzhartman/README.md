# C# Academy Unit Test for PhoneBook Application

This project serves as a practice for creating unit tests and integration tests. It is built off of a previous project from The C# Academy. The requirements for this unit test project can be found at the link from The C# Academy [Testing Project](https://www.thecsharpacademy.com/project/21/unit-testing).

## Application Description
This is a basic Phone Book application. It allows the user to add contacts to a database. Contacts consist of First Name, Last Name, Phone Number, Email Address and Category. Categories are user-defined and help to increase organization and simplify searches. Users are able to create, edit, or delete these contacts and categories. The user can configure SMTP email settings to allow them to directly send an email to a contact from the list.
This was built following The C# Academy [Phone Book project guidelines](https://www.thecsharpacademy.com/project/16/phonebook).

The application is paired with a testing suite that ensures only viable data can be entered into the database. Unit Tests cover both correct and incorrect inputs values for all use cases in the Application layer. They use the Moq library to easily mock all dependencies. Integration tests utilize a temporary Sqlite database. All CRUD operations from the Category repository and the Contact repository are tested for successful and failed results.

## Features
- Clean Architecture (layered as Domain → Application → Infrastructure → Presentation)
- Entity Framework Core
- SQLite database for data persistence
- Console-based UI using Spectre.Console
- Email sending to a selected contact via configurable SMTP

## Setup
The application should automatically create and seed the database when run. If there is an issue, you may need to check that the connection string is valid.
1. Open the App settings in `appsettings.json`.
2. Enter the Connection string name: `PhoneBook` — recommended SQLite example:
    - `Data Source=phonebook.db`
3. Rebuild the application.

The PhoneBook project also allows the user to directly email contacts. This requires the SMTP settings to be configured.
1. Open the App Settings in `appsettings.json`
2. Configure all SMTP settings to match your desired email account

## Application Usage
### Main Menu
Opens with a Main Menu providing the options:

<img width="1626" height="205" alt="image" src="https://github.com/user-attachments/assets/c6e96111-e2a4-44ba-8fb0-bcba04cf380b" />


### Add Contact
Selecting Add Contact will prompt the user to input the relevant data for the contact. All fields are required:

<img width="1623" height="319" alt="image" src="https://github.com/user-attachments/assets/39c5ee9c-9ab5-413e-9edf-9e2715517900" />

A confirmation message is provided, after which the contact will be added or discarded.


### View Contact
Selecting View Contact will first print the list of Categories. The user selects from the list to see only contacts in that category:

<img width="1612" height="237" alt="image" src="https://github.com/user-attachments/assets/25490e22-9cdd-42cf-81aa-bfacdfa9c650" />

Once the Category is selected, the relevant contact list is printed. The user selects the contact to view their details:

<img width="1626" height="469" alt="image" src="https://github.com/user-attachments/assets/234d6c43-c2b9-4328-ba24-8bb687f5ef79" />

Contact details are displayed below, as well as the relevant options:

<img width="1612" height="652" alt="image" src="https://github.com/user-attachments/assets/f2c1313e-4c79-4b4d-b2ad-13cbbe3c53f0" />

- Options:
  - Delete Contact: Deleted the currently displayed contact and returns to the Main Menu (requires confirmation)
  - Edit Contact: Allows any of the parameters for the contact to be changed
  - Send Email: Sends an email to the contact at their displayed email address (requires proper SMTP configuration in appsettings.json)
  - Return to Main Menu: Returns to the Main Menu

### Manage Categories
Selecting Manage Categories displays the following submenu:

<img width="1618" height="205" alt="image" src="https://github.com/user-attachments/assets/dabb83cc-600c-42b5-bc8e-35b117c34afc" />

- Options:
  - Add Category: Adds a new category to the list (must have a unique name -- case insensitive)
  - Delete Category: Select a category from the list and delete it -- This will set all contacts within that category to Uncategorized (requires confirmation)
  - Rename Category: Rename the category to whatever you want (case sensitive)
  - Return to Main Menu: Returns to the Main Menu

## Reflection
This is reflection specifically on the Unit Test portion of the project and not on the PhoneBook project as a whole.
The unit tests themselves were a bit tedious to get started. Part of this was due to my use of the Results pattern. It meant that everything had to be wrapped and pulled out. However, it was very convenient to be able to write tests that correctly demonstrated not only that correctness of a value, but also the presence of specific error messages.

Integration tests faced the same challenge. They also got fairly repetitive, so I decided to create a Database Fixture class that could be used to create a temporary test database and give the option to seed data. This cut down on a lot of the repetitive code. Throughout the testing, I was able to continue to optimize the fixture class and keep some of the tests a little cleaner.

One flaw in my application design was the reliance on Spectre.Console to handle my user input validation. Since all validation was handled by Spectre.Console, it made my input validation un-testable until it reached the use cases in the application layer. In the future, I plan to create separate validation methods for my user input and then pass them into Spectre.Console as delegates. Despite this, I was able to fully ensure that only correct data would be passed to the database through the unit tests for the use cases and integration tests for the repository methods.
