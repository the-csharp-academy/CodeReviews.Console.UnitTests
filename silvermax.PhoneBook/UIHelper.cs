using Spectre.Console;

namespace silvermax.PhoneBook;

internal static class UIHelper
{
    internal static void ContinueMessage()
    {
        AnsiConsole.MarkupLine("Press Any Key to continue...");
        if (!Console.IsOutputRedirected)
            Console.ReadKey();
    }

    public static void ClearScreen()
    {
        if (!Console.IsOutputRedirected)
            Console.Clear();
    }
}
