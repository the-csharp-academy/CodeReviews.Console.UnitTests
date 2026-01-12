using System;
using CodingTracker.Niasua.Models;
using Spectre.Console;

namespace CodingTracker.Niasua.UserInput;

internal static class UserInputHandler
{
    private const string DateTimeFormat = "dd-MM-yyyy HH:mm";

    public static CodingSession GetCodingSessionFromUser()
    {
        Console.WriteLine($"Please enter the start date and time ({DateTimeFormat}):");
        DateTime startTime = ReadValidDateTime();

        Console.WriteLine($"Please enter the end date and time ({DateTimeFormat}):");
        DateTime endTime = ReadValidDateTime();

        while (!Validator.IsValidSessionTimes(startTime, endTime))
        {
            Console.WriteLine("End time must be AFTER start time. Please enter a valid end date and time:");
            endTime = ReadValidDateTime();
        }

        return new CodingSession
        {
            StartTime = startTime,
            EndTime = endTime
        };
    }

    public static int GetSessionId()
    {
        while (true)
        {
            Console.WriteLine($"What's the Session ID you want to delete?: ");
            string input = Console.ReadLine();

            if (Validator.IsValidInt(input, out int id))
            {
                return id;
            }

            AnsiConsole.MarkupLine("[red]Please enter a valid numeric ID.[/]"); 
        }
    }

    private static DateTime ReadValidDateTime()
    {
        while (true)
        {
            string input = Console.ReadLine();

            if (Validator.IsValidDateTime(input, DateTimeFormat, out DateTime parsedDate))
            {
                return parsedDate;
            }

            AnsiConsole.MarkupLine($"\n[red]Invalid date/time format. Please use the format: {DateTimeFormat}[/]");
            AnsiConsole.MarkupLine($"\nPlease enter the start date and time ({DateTimeFormat}):");
        }
    }

    public static DateTime GetOptionalDateTime(DateTime current)
    {
        Console.WriteLine($"Enter new date/time ({DateTimeFormat}) or press ENTER to keep current [{current.ToString(DateTimeFormat)}]:");
        string input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
            return current; 

        if (Validator.IsValidDateTime(input, DateTimeFormat, out DateTime parsedDate))
            return parsedDate;

        Console.WriteLine("Invalid format, please try again.");
        return GetOptionalDateTime(current);
    }

}
