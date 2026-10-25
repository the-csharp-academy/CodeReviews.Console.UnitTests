using PhoneBook.Application.Categories.DTOs;
using Spectre.Console;

namespace PhoneBook.ConsoleUI.Views;

internal class CategorySelectionView
{
    public CategoryResponse Render(IEnumerable<CategoryResponse> categories)
    {
        var selection = AnsiConsole.Prompt(
                            new SelectionPrompt<CategoryResponse>()
                            .Title("Select a category from below: ")
                            .PageSize(15)
                            .WrapAround()
                            .UseConverter(c => c.Name)
                            .AddChoices(categories));

        return selection;
    }
}
