using CodingTracker.Niasua.Data;
using CodingTracker.Niasua.Models;
using CodingTracker.Niasua.UI;
using CodingTracker.Niasua.UserInput;
using Spectre.Console;
using SQLitePCL;

Batteries.Init();

CodingController.InitializeDatabases();
DatabaseSeeder.Seed();

bool exit = false;

while (!exit)
{
    Console.Clear();
    AnsiConsole.MarkupLine("[bold blue]Coding Tracker[/]");
    AnsiConsole.MarkupLine("\nSelect an option:\n");

    var choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
        .Title("Choose an [green]action[/].")
        .AddChoices(new[]
        {
            "View all sessions",
            "Add new session",
            "Delete session",
            "Delete all sessions",
            "Update session",
            "Start stopwatch session",
            "Filter coding records per period",
            "Show summary report",
            "Set coding goal",
            "Show goal progress",
            "Exit"
        }));

    switch (choice)
    {
        case "View all sessions":

            var sessions = CodingController.GetAllSessions();
            ViewAllSessions(sessions);

            break;

        case "Add new session":

            var session = UserInputHandler.GetCodingSessionFromUser();
            AddSession(session);

            break;

        case "Delete session":

            sessions = CodingController.GetAllSessions();
            DeleteSession(sessions);

            break;

        case "Delete all sessions":

            DeleteAll();

            break;

        case "Update session":

            sessions = CodingController.GetAllSessions();
            UpdateSession(sessions);

            break;

        case "Start stopwatch session":

            StopwatchSession();

            break;

        case "Filter coding records per period":

            FilterPerPeriod();

            break;

        case "Show summary report":

            TotalAndAverageReport();

            break;

        case "Set coding goal":

            SetGoal();

            break;

        case "Show goal progress":

            ShowGoalProgress();

            break;

        case "Exit":

            exit = true;

            break;
    }
}

void DeleteAll()
{
    AnsiConsole.MarkupLine("[red]Are you sure you want to delete every session?[/]\n");

    var answer = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
        .Title("Choose an [red]option[/].")
        .AddChoices(new[]
        {
            "Yes, I'm sure",
            "No, i want to go back"
        }));

    if (answer == "Yes, I'm sure")
    {
        CodingController.DeleteAll();
        AnsiConsole.MarkupLine("\n[red]All sessions successfully deleted.[/]");
    }

    Pause();
}

void ShowGoalProgress()
{
    CodingController.ShowGoalProgress();
    Pause();
}

void SetGoal()
{
    AnsiConsole.MarkupLine("Enter your [red]target[/] coding hours: ");

    if (!double.TryParse(Console.ReadLine(), out double targetHours) || targetHours <= 0)
    {
        AnsiConsole.MarkupLine("[red]Invalid input.[/]");
        Pause();
        return;
    }

    AnsiConsole.MarkupLine("Enter [green]Deadline[/] (dd-MM-yyyy)");

    if (!DateTime.TryParseExact(Console.ReadLine(), "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime deadline))
    {
        AnsiConsole.MarkupLine("[red]Invalid date format.[/]");
        Pause();
        return;
    }

    var goal = new CodingGoal
    {
        TargetHours = targetHours,
        Deadline = deadline
    };

    CodingController.InsertGoal(goal);

    AnsiConsole.MarkupLine("[green]Goal saved successfully![/]");
    Pause();
}

void FilterPerPeriod()
{
    AnsiConsole.MarkupLine("[blue]-- Filter Sessions --[/]\n");

    AnsiConsole.MarkupLine("Write the [red]start and end[/] dates of the period\n");

    var session = UserInputHandler.GetCodingSessionFromUser();

    Console.WriteLine();

    var order = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
        .Title("Choose the filtering [green]order[/].")
        .AddChoices(new[]
        {
            "ASC",
            "DESC"
        }));

    var sessions = CodingController.FilterSessionPerPeriod(session, order);

    TableDisplay.ShowSessions(sessions);

    Pause();
}

void TotalAndAverageReport()
{
    AnsiConsole.MarkupLine("[blue]-- Total and Average Report--[/]\n");

    var period = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("Choose the [green]period[/] to summarize by.")
            .AddChoices(new[] { "Daily", "Weekly", "Monthly", "Yearly" }));

    var summaries = CodingController.ShowSummaryReport(period);

    TableDisplay.ShowSumaries(summaries);

    Pause();
}

void StopwatchSession()
{
    AnsiConsole.MarkupLine("[blue]-- Stopwatch Session --[/]\n");

    var start = DateTime.Now;

    AnsiConsole.MarkupLine("[white]Tracking... Press any key to stop the session...[/]\n");

    while (!Console.KeyAvailable)
    {
        var elapsed = DateTime.Now - start;

        Console.SetCursorPosition(0, Console.CursorTop);
        AnsiConsole.Markup($"\r[bold]Elapsed time:[/] [green]{elapsed:hh\\:mm\\:ss}[/]");

        Thread.Sleep(1000);
    }

    Console.ReadKey(true);

    var end = DateTime.Now;

    AnsiConsole.MarkupLine("\n[green]Sessions correctly added.[/]");
    Pause();

    var session = new CodingSession
    {
        StartTime = start,
        EndTime = end
    };

    CodingController.InsertSession(session);
}

void UpdateSession(List<CodingSession> sessions)
{
    TableDisplay.ShowSessions(sessions);

    var id = UserInputHandler.GetSessionId();
    var sessionToUpdate = CodingController.GetSessionById(id);

    if (sessionToUpdate is null)
    {
        AnsiConsole.MarkupLine("[red]That session doesn't exist.[/]");
        Pause();
        return;
    }

    AnsiConsole.MarkupLine($"\nCurrent Start: [yellow]{sessionToUpdate.StartTime}[/]");
    AnsiConsole.MarkupLine($"Current End: [yellow]{sessionToUpdate.EndTime}[/]");

    var newStart = UserInputHandler.GetOptionalDateTime(sessionToUpdate.StartTime);

    var newEnd = UserInputHandler.GetOptionalDateTime(sessionToUpdate.EndTime);

    sessionToUpdate.StartTime = newStart;
    sessionToUpdate.EndTime = newEnd;

    CodingController.UpdateSession(sessionToUpdate);
}

static void ViewAllSessions(List<CodingSession> sessions)
{
    TableDisplay.ShowSessions(sessions);

    Pause();
}

static void AddSession(CodingSession session)
{
    CodingController.InsertSession(session);

    AnsiConsole.MarkupLine("\n[green]Session successfully added.[/]\n");
    Pause();
}

static void DeleteSession(List<CodingSession> sessions)
{
    TableDisplay.ShowSessions(sessions);

    
    var id = UserInputHandler.GetSessionId();
    var sessionToDelete = CodingController.GetSessionById(id);

    if (sessionToDelete is null)
    {
        AnsiConsole.MarkupLine("[red]That session doesn't exist.[/]");
        Pause();
        return;
    }

    CodingController.DeleteSession(sessionToDelete);

    AnsiConsole.MarkupLine("\n[green]Session successfully deleted.[/]\n");
    Pause();
}

static void Pause()
{
    AnsiConsole.MarkupLine("\n[grey]Press any key to continue...[/]");
    Console.ReadKey();
}