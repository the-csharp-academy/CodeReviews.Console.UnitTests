using System.Diagnostics;
using CodingTracker.kilozdazolik.Services;
using Spectre.Console;
using CodingTracker.kilozdazolik.Models;
namespace CodingTracker.kilozdazolik.Controllers;

public class TrackerController
{
    private readonly ITrackerService _trackerService;
    private readonly Helper _helper = new();

    public TrackerController(ITrackerService trackerService)
    {
        _trackerService =  trackerService;
    }

    public void StartSession()
    {
        AnsiConsole.MarkupLine("Press the Enter key to begin/stop:");
        Console.ReadLine();
        DateTime startTime = DateTime.Now;
        Stopwatch sw = Stopwatch.StartNew();

        while (!Console.KeyAvailable || Console.ReadKey(true).Key != ConsoleKey.Enter)
        {
            Console.Clear();
            TimeSpan ts = sw.Elapsed;
            AnsiConsole.Markup($"Elapsed time: {ts.Hours:D2}:{ts.Minutes:D2}:{ts.Seconds:D2}");
            Thread.Sleep(1000);
        }

        sw.Stop();
        DateTime endTime = DateTime.Now;
        
        Console.Clear();
        AnsiConsole.MarkupLine($"Final elapsed time: {sw.Elapsed.Hours:D2}:{sw.Elapsed.Minutes:D2}:{sw.Elapsed.Seconds:D2}");
        
        _trackerService.InsertSession(startTime, endTime);
        
        AnsiConsole.MarkupLine("Press Any Key to Continue.");
        Console.ReadKey();
    }
    public void AddSession()
    {
        bool confirm = false;
        while (!confirm)
        {
            AnsiConsole.MarkupLine("[yellow]Add a new coding session[/]");

            var inputStartDate = AnsiConsole.Prompt(
                new TextPrompt<string>("Please write the starting date in this format: (dd/MM/yyyy HH:mm:ss)"));
            if (!_helper.ValidateDate(inputStartDate, out DateTime startDate))
            {
                AnsiConsole.MarkupLine("[red]Invalid start date format![/]");
                continue;
            }
            
            var inputEndDate = AnsiConsole.Prompt(
                new TextPrompt<string>("Please write the ending date in this format: (dd/MM/yyyy HH:mm:ss)"));
            if (!_helper.ValidateDate(inputEndDate, out DateTime endDate))
            {
                AnsiConsole.MarkupLine("[red]Invalid start date format![/]");
                continue;
            }
            
            if (_helper.IsSessionDatesValid(startDate, endDate))
            {
                _trackerService.InsertSession(startDate, endDate);
                confirm = true;
                AnsiConsole.MarkupLine("[green]Session successfully added![/]");
            }
        }
    }

    public void ViewAllSessions()
    {
        List<Tracker> allSessions = _trackerService.GetAllSession();

        if (allSessions.Any())
        {
            _helper.CreateTable(allSessions);
        }
        else
        {
            AnsiConsole.MarkupLine("I could not find any session.");
        }
        
    }
    public void DeleteSession()
    {
        List<Tracker> allSession = _trackerService.GetAllSession();

        if (allSession.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No session is available to delete.[/]");
            Console.ReadKey();
        }

        var sessionToDelete = AnsiConsole.Prompt(new SelectionPrompt<Tracker>()
            .Title("Select a [red]session[/] to delete:").UseConverter(s => $"Start: {s.StartTime} | End: {s.EndTime} - {s.Duration} elapsed.").AddChoices(allSession));

        if (_helper.ConfirmMessage("Delete", sessionToDelete.StartTime.ToString()))
        {
            _trackerService.DeleteSession(sessionToDelete);
        }
        else
        {
            AnsiConsole.MarkupLine("Deletion Canceled.");
        }
        
        AnsiConsole.MarkupLine("[green]Press Any Key to Continue.[/]");
        Console.ReadKey();
    }
    public void EditSession()
    {
        List<Tracker> allSession = _trackerService.GetAllSession();
        
        if (allSession.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No session is available to delete.[/]");
            Console.ReadKey();
        }
        
        var sessionToEdit = AnsiConsole.Prompt(new SelectionPrompt<Tracker>()
            .Title("Select a [cyan]session[/] to edit:").UseConverter(s => $"Start: {s.StartTime} | End: {s.EndTime} - {s.Duration} elapsed.").AddChoices(allSession));
        
        Console.Clear();
        
        bool confirm = false;
        while (!confirm)
        {
            var inputStartDate = AnsiConsole.Prompt(
                new TextPrompt<string>($"Enter the [green]new start time[/] (dd/MM/yyyy HH:mm:ss)\n[grey](default: {sessionToEdit.StartTime:dd/MM/yyyy HH:mm:ss})[/]"));
            if (!_helper.ValidateDate(inputStartDate, out DateTime newStartingTime))
            {
                AnsiConsole.MarkupLine("[red]Invalid start date format![/]");
                continue;
            }

            var inputEndDate = AnsiConsole.Prompt(
                new TextPrompt<string>($"Enter the [green]new end time[/] (dd/MM/yyyy HH:mm:ss)\n[grey](default: {sessionToEdit.EndTime:dd/MM/yyyy HH:mm:ss})[/]"));
            if (!_helper.ValidateDate(inputEndDate, out DateTime newEndingTime))
            {
                AnsiConsole.MarkupLine("[red]Invalid end date format![/]");
                continue;
            }

            if (_helper.IsSessionDatesValid(newStartingTime, newEndingTime))
            {
                sessionToEdit.StartTime = newStartingTime;
                sessionToEdit.EndTime = newEndingTime;
                confirm = true;
            }
        }

        _trackerService.UpdateSession(sessionToEdit);
    }

    public void ViewSessionsByDate(bool ascending)
    {
        List<Tracker> allSessions = _trackerService.GetSessionsOrderByDate(ascending);

        if (allSessions.Any())
        {
            _helper.CreateTable(allSessions);
        }
        else
        {
            AnsiConsole.MarkupLine("I could not find any session.");
        }
        
    }
}