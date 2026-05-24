# About Project
This is project was created as a learning project from [CSharpAcademy's Coding Tracker project](https://thecsharpacademy.com/project/13/coding-tracker)
and follows the requirments set there.  
The main functionality of the project lies in creating and tracking the user's coding sessions, allowing them to create, view, update, and delete their sessions.  
This is a basic console project and uses the Spectre.Console library for Ui related issues, and Sqlite and Dapper ORM for the database.  

# Features - Overview
* Most menus and prompts use basic up, down, and enter keys, requiring little user typing, leading to less possibilities for input error, and an easier life for the user.
* Menus use Spectre.Console for a nice look, and easy accesibility.  
  ![Image displaying the main menu, with several options for input.](https://i.imgur.com/rF9FrGu.png)
* Viewing sessions shows a brief overview of each session, allowing the user to select any session to see more details.  
  ![Image displaying a menu along with various sessions](https://i.imgur.com/asyxmGM.png)
* The user can easily filter for specific dates, or filter sessions to show in ascending/descending order.  
  ![Image displaying a menu along with different options, including a 'filter sessions' and 'clear filters' option, along with various sessions arranged by ascending duration.](https://i.imgur.com/3a4DF6j.png)
* Each session can be indivdually selected to see more details, as well as take certain actions like deleting or updating a session.  
  ![Image displaying a menu that displays a session's details, and provides a 'Delete', 'Update' and 'Return' option.](https://i.imgur.com/xao218V.png)
* Confirmation screens are displayed for destructive actions, and common user mistakes.  
  ![Image displaying a deletion confirmation screen](https://i.imgur.com/pxlxj5O.png)
* Features a timer to that records a coding session in real time.  
  ![Image displaying a menu labeled "Press 'Q' To Stop Timer"](https://i.imgur.com/i9et7w9.png)

# Features - Detailed
## Menus and Ui
For menus with constant values such as main menus, or certain action menus, they are displayed using the DisplayHelper's DisplayMenu method, which uses the AnsiConsole.Prompt
method. This method uses a generic type, and allows Enums to be passed in. The menu is created from the Enum's items, and uses an enum extension to make items more presentable,
for example; the main menu's enum has an item called "NewSession", but using the enum extension, this is displayed as "New Session", leading to much more readable menus.  
For menus that use display flexible values instead of constant ones, DisplayHelper's DisplayPrompt method is called. This method is functionally identical to DisplayMenu,
except it takes in a `List<string>` parameter instead of using Enums.
The DisplayHelper.cs file takes care of almost all the calls to Spectre.Console's Ui methods, and uses constant strings of hex codes for different colors, allowing the asthetics
of the application to stay focused on consistency, since almost all needs for displaying Ui is passed through these helper methods. Furthermore, the constant strings use an 
`internal` access modifier, meaning any Ui needs outside of DisplayHelper.cs can be displayed with the same consistent colors. Using internal constant strings also means that 
if a color needs to be changed within the application, you can simply swap out out the hex code string value for that color, or you can even remove colors or add new colors all
by switching one section of code.  

## Viewing and Filtering Sessions
The main method for viewing sessions is the ViewSessions method within the ConsoleUi.cs class. This method displays a functionally empty loading screen to the user just to 
present the user with the idea of loading, although this only shows once per trip to the method, since it gets annoying quickly if you have to wait everytime, especially when 
filtering sessions. After the method has finished loading, it displays a menu containing a "Return to Main Menu", "Filter Sessions", and a conditional "Clear Filters" option.
It also displays each current session recorded in the database, along with it's ID, Date of creation, and duration. From there the user can either select the "Filter Sessions"
option to go to the filtering menu, or they can select any individual session to see more details and actions. Going to the filter menu allows the user to select a date to filter
by, either 'Today' for today's date, 'Other' to enter a specific date, or 'Default' to not filter by date. Afterwards theres also an 'Ascending' and 'Descending' option, as well
as another 'Default' option for not filtering by ascending/descending. Loading an invidual session allows the user to see more details of a session, such as the full: date,
start time, end time, and duration. There are also options to delete the specific session, update the session, or return to the previous menu.  

## Timer and Real Time Logging
Using the timer creates an asynchronous task, and then passes the task to DisplayHelper's DisplayAsyncSpinner method, which displays a spinner until the timer is cancelled or 
finished. A title is passed to the spinner to let the user know to press 'Q' in order to stop the timer, and the application waits unitl the Q key is pressed before cancelling 
the task and ending the spinner. Afterwards, it displays a spinner while saving the session to the database, and returns to the main menu.  

# Resources Used
[.NET (10.0)](https://learn.microsoft.com/en-us/dotnet/)  
[Spectre.Console (0.54.0)](https://spectreconsole.net) - Ui  
[Dapper (2.1.66)](https://www.learndapper.com) - ORM  
[Microsoft.Data.Sqlite (10.0.2)](https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/?tabs=net-cli) - Database  
[Microsoft.Extensions.Configuration (10.0.2)](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration?view=net-10.0-pp) - Configuration  
[Microsoft.Extensions.Configuration.EnviromentVariables (10.0.2)](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.environmentvariablesextensions?view=net-10.0-pp)  
[Microsoft.Extensions.Configuration.Json (10.0.2)](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.json?view=net-8.0-pp)  
[Microsoft.Extensions.Logging (10.0.2)](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging/overview?tabs=command-line) - Logging  
[Microsoft.Extensions.Logging.Abstractions (10.0.2)](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.abstractions?view=net-10.0-pp)  
[Microsoft.Extensions.Logging.Console (10.0.2)](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.console?view=net-8.0-pp)  

# Personal Thoughts
When I first started this project I was a little confused on how I should start, but i was able to get an idea after a while. In fact, in comparison to past projects, this one
felt a lot easier, Maybe because I in the [last project I had](https://www.thecsharpacademy.com/project/12/habit-logger), I had to figure out how to use Sqlite's database from
scratch, whereas for this one I already had a little idea of how to use it.  
By far, my favorite part of this project was learning and using Spectre.Console. It made it so easy to customize my application, and it amazed me how things that I would have no
idea how to do otherwise, could be done with just a single line of code. This also gave me the ability to turn the basic console of black and white text into real menus with
options, selections, colors, and tons of other things to make my life easier and make the app nicer.  
Overall, this project was very fun and I enjoyed it, although I'm a little nervous for the next project since I heard it gets a lot more difficult, but I'm still excited to start.
One thing I'd like to learn more of would be proper error handling, since right now my application doesn't do much other than logging it to the console and keeping it from
crashing. I think I could also learn more about Seperation of Concerns and how to keep everything clean. I tried to seperate the code into a Ui layer for actually Displaying things
to the user, a Service layer for taking care of the real logic, and a Database layer for dealing with the Sqlite commands and operations, but I think I could still improve on 
these topics.  
I definitly enjoyed this project, and the fact that the gap of knowledge wasn't as big as it felt in previous projects let me relax a little more and not worry about having to 
learn a ton of new things at once. I'm excited to start the next project, and hopefully it wont take long before I have to write a second ReadMe. 
