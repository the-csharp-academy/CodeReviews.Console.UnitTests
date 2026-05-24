using codingTracker._0lcm.Interfaces;
using codingTracker._0lcm.Models;
using codingTracker._0lcm.User_Interface;
using Spectre.Console;

namespace codingTracker._0lcm.User_Input;

internal class UserInputHelper(ITimeValidationService timeValidationService, ISessionService sessionService)
{
    /// <summary>
    ///     Asks user to manually input a valid start and end time and returns a new CodingSession
    ///     or updates and returns a pre-existing CodingSession if one is passed through.
    /// </summary>
    internal CodingSession GetSessionFromInput(CodingSession? session = null)
    {
        while (true)
        {
            Console.Clear();

            DisplayHelper.DisplayInfo(
                "Please Follow a 24hr HH:mm Format (00:00-23:59). Press <Enter> to Submit Input.");
            DisplayHelper.DisplayInfo(
                "Please Note That If Your End Time Is Before Your Start Time It Will Automatically Be Counted as a Cross-Midnight Session.\n");

            var startTimeInput = DisplayHelper.DisplayQuestion("Please Enter a Start Time:");
            var endTimeInput = DisplayHelper.DisplayQuestion("Please Enter an End Time:");

            if (timeValidationService.TryValidateStartAndEndTimePair(startTimeInput, endTimeInput,
                    out var startTime, out var endTime, out var errorMessage))
            {
                var duration = endTime - startTime;
                if (duration.TotalHours >= 10)
                    if (!AnsiConsole.Confirm(
                            $"[{DisplayHelper.Red}]This Session Is {duration.TotalHours:F1} Hours Long. Is That Right?[/]"))
                        continue;

                if (session != null)
                {
                    session.StartTime = startTime;
                    session.EndTime = endTime;
                    session.Duration = duration;

                    return session;
                }

                return new CodingSession(
                    startTime,
                    endTime,
                    endTime - startTime
                );
            }

            DisplayHelper.DisplayUrgent(errorMessage ?? "Invalid Input.");
            DisplayHelper.DisplayInfo("Press <Enter> To Re-enter Time Selections.");
            Console.ReadLine();
        }
    }

    internal async Task<DateOnly> GetDateInput()
    {
        while (true)
        {
            Console.Clear();

            DisplayHelper.DisplayInfo("Please Follow YYYY-MM-dd Format");

            var dateInput = DisplayHelper.DisplayQuestion("Please enter a date:");

            if (timeValidationService.TryValidateDateTime(dateInput, out var date, out var errorMessage)
                && await sessionService.CheckForExistingSessionDate(date))
                return date;

            if (errorMessage == null && !await sessionService.CheckForExistingSessionDate(date))
                errorMessage = "Date Is Not In Recorded Sessions. Please Choose A New Date.";

            DisplayHelper.DisplayUrgent(errorMessage ?? "Invalid Input.");
            DisplayHelper.DisplayInfo("Press <Enter> To Re-Enter Date Selection.");
            Console.ReadLine();
        }
    }
}