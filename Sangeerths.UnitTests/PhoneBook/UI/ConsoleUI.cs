using Spectre.Console;
using System.Linq;
namespace PhoneBook.UI;

public class ConsoleUI
{
    public void ShowHeader()
    {
        AnsiConsole.Clear();

        var header = new FigletText("PhoneBook")
            .Centered()
            .Color(Color.SpringGreen3);
        AnsiConsole.Write(header);

        var rule = new Rule("[bold aqua]Contact Manager[/]")
        {
            Justification = Justify.Center,
            Style = new Style(Color.Grey)
        };
        AnsiConsole.Write(rule);
        AnsiConsole.WriteLine();

        var infoPanel = new Panel(
            new Markup("[grey]Manage your contacts, search entries, and keep your phonebook organised.[/]"))
        {
            Border = BoxBorder.None,
            Padding = new Padding(0, 0, 0, 1),
            Expand = true
        };
        AnsiConsole.Write(Align.Center(infoPanel));
    }

    public void Pause()
    {
        AnsiConsole.WriteLine();

        var rule = new Rule
        {
            Style = new Style(Color.Grey35)
        };

        AnsiConsole.Write(rule);
        AnsiConsole.MarkupLine("[grey italic]Press any key to continue...[/]");

        if (!IsRunningUnderTest() && !Console.IsInputRedirected && Environment.UserInteractive && Console.KeyAvailable == false)
        {
            Console.ReadKey(true);
        }
    }

    private bool IsRunningUnderTest()
    {
        try
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Any(a => a.GetName().Name?.IndexOf("nunit.framework", StringComparison.OrdinalIgnoreCase) >= 0);
        }
        catch
        {
            return false;
        }
    }

    public void ShowSuccess(string message)
    {
        var panel = new Panel(new Markup($"[bold green]✓[/] {message}"))
        {
            Border = BoxBorder.Rounded,
            BorderStyle = new Style(Color.Green),
            Padding = new Padding(1, 0, 1, 0),
            Expand = false
        };
        AnsiConsole.WriteLine();
        AnsiConsole.Write(panel);
    }

    public void ShowError(string message)
    {
        var panel = new Panel(new Markup($"[bold red]✗[/] {message}"))
        {
            Border = BoxBorder.Rounded,
            BorderStyle = new Style(Color.Red),
            Padding = new Padding(1, 0, 1, 0),
            Expand = false
        };
        AnsiConsole.WriteLine();
        AnsiConsole.Write(panel);
    }

    public void ShowInfo(string message)
    {
        var panel = new Panel(new Markup($"[bold cyan]i[/] {message}"))
        {
            Border = BoxBorder.Rounded,
            BorderStyle = new Style(Color.Aqua),
            Padding = new Padding(1, 0, 1, 0),
            Expand = false
        };
        AnsiConsole.WriteLine();
        AnsiConsole.Write(panel);
    }

    public void ShowGoodbye()
    {
        AnsiConsole.Clear();

        var goodbyeText = new FigletText("Goodbye!")
            .Centered()
            .Color(Color.SpringGreen3);
        AnsiConsole.Write(goodbyeText);
        AnsiConsole.WriteLine();

        var panel = new Panel(
            new Markup("[bold aqua]Thank you for using PhoneBook![/]\n[grey]We hope to see you again soon to manage more contacts.[/]"))
        {
            Header = new PanelHeader("[bold green] <3 [/]", Justify.Center),
            Border = BoxBorder.Double,
            BorderStyle = new Style(Color.SpringGreen3),
            Padding = new Padding(4, 1, 4, 1),
            Expand = false
        };

        AnsiConsole.Write(Align.Center(panel));
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine();
    }
}