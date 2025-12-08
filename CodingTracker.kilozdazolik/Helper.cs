using System.Globalization;
using CodingTracker.kilozdazolik.Models;
using Spectre.Console;
namespace CodingTracker.kilozdazolik;

public class Helper
{
    public bool ConfirmMessage(string message, string element, string color = "red")
    {
        var confirm = AnsiConsole.Confirm($"Are you sure you want to {message} [{color}]{element}[/]?");

        return confirm;
    }

    public bool ValidateDate(string date, out DateTime parsedDate)
    {
        return DateTime.TryParseExact(date, "dd/MM/yyyy HH:mm:ss",
            CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate);
    }

    public bool IsSessionDatesValid(DateTime startDate, DateTime endDate)
    {
        int result = DateTime.Compare(startDate, endDate);
        
        if (result > 0)
        {
            AnsiConsole.MarkupLine("The end date must not precede the start date!");
            return false;
        }

        return true;
    }

    public void CreateTable(List<Tracker> allSessions) 
    {
        var table = new Table();
        table.Border(TableBorder.Rounded);
            
        table.AddColumn("[yellow]ID[/]");
        table.AddColumn("[yellow]Starting date[/]");
        table.AddColumn("[yellow]Ending date[/]");
        table.AddColumn("[yellow]Duration[/]");

        foreach (var session in allSessions)
        {
            table.AddRow(
                session.Id.ToString(),
                $"[blue]{session.StartTime.ToString("dd-MM-yyyy HH:mm", CultureInfo.GetCultureInfo("en-US"))}[/]",
                $"[cyan]{session.EndTime.ToString("dd-MM-yyyy HH:mm", CultureInfo.GetCultureInfo("en-US"))}[/]",
                $"[green]{session.Duration}[/]"
            );
        }
        
        AnsiConsole.Write(table);
    }

    public void ShowWithPause(Action action)
    {
        AnsiConsole.Clear();
        action();
        AnsiConsole.MarkupLine("\n[grey]Press any key to return...[/]");
        Console.ReadKey(true);
        AnsiConsole.Clear();
    }
}