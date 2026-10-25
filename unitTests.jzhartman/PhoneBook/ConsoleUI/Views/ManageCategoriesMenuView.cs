using PhoneBook.ConsoleUI.Enums;
using Spectre.Console;

namespace PhoneBook.ConsoleUI.Views;

internal class ManageCategoriesMenuView
{
    internal ManageSubMenuOptions Render(ManageSubMenuOptions[] menuOptions)
    {
        Console.Clear();
        var selection = AnsiConsole.Prompt(
            new SelectionPrompt<ManageSubMenuOptions>()
            .Title("Managing Categories -- Select a menu option:")
            .UseConverter(m => m switch
            {
                ManageSubMenuOptions.Add => "Add Category",
                ManageSubMenuOptions.Delete => "Delete Category",
                ManageSubMenuOptions.Edit => "Rename Category",
                ManageSubMenuOptions.Exit => "Return to Main Menu",
                _ => m.ToString()
            })
            .AddChoices(menuOptions));

        return selection;
    }
}