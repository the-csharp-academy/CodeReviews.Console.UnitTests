using Spectre.Console;
using CodingTracker.Niasua.Models;

namespace CodingTracker.Niasua.UI
{
    internal static class TableDisplay
    {
        internal static void ShowSessions(List<CodingSession> sessions)
        {
            var table = new Table();
            table.Border(TableBorder.Rounded);
            table.AddColumn("[yellow]ID[/]");
            table.AddColumn("[cyan]Start Time[/]");
            table.AddColumn("[cyan]End Time[/]");
            table.AddColumn("[green]Duration (hrs)[/]");

            foreach (var session in sessions)
            {
                var duration = TimeSpan.FromHours(session.DurationHours);

                table.AddRow(
                    session.Id.ToString(),
                    session.StartTime.ToString("dd-MM-yyyy HH:mm"),
                    session.EndTime.ToString("dd-MM-yyyy HH:mm"),
                    duration.ToString(@"hh\:mm")
                    );
            }

            AnsiConsole.Write(table);
        }
        internal static void ShowSumaries(List<CodingSummary> summaries)
        {
            var table = new Table();
            table.Border(TableBorder.Rounded);
            table.AddColumn("[yellow]Period[/]");
            table.AddColumn("[cyan]Sessions[/]");
            table.AddColumn("[green]Avg Duration[/]");

            foreach (var summary in summaries)
            {
                var duration = TimeSpan.FromHours(summary.AvgDuration);

                table.AddRow(summary.Period, summary.Sessions.ToString(), duration.ToString(@"hh\:mm"));
            }

            AnsiConsole.Write(table);
        }
    }
}
