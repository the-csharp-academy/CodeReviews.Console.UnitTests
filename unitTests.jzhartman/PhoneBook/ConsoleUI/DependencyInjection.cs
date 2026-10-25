using Microsoft.Extensions.DependencyInjection;
using PhoneBook.ConsoleUI.Input;
using PhoneBook.ConsoleUI.Output;
using PhoneBook.ConsoleUI.Services;
using PhoneBook.ConsoleUI.Services.Categories;
using PhoneBook.ConsoleUI.Services.Contacts;
using PhoneBook.ConsoleUI.Services.Email;
using PhoneBook.ConsoleUI.Views;

namespace PhoneBook.ConsoleUI;

internal static class DependencyInjection
{
    internal static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddTransient<UserInput>();
        services.AddTransient<Messages>();

        services.AddTransient<MainMenuView>();
        services.AddTransient<ContactSelectionView>();
        services.AddTransient<ContactDetailsView>();
        services.AddTransient<EditContactView>();
        services.AddTransient<CategorySelectionView>();

        services.AddTransient<MainMenuService>();
        services.AddTransient<AddContactService>();
        services.AddTransient<ViewContactService>();
        services.AddTransient<DeleteContactService>();
        services.AddTransient<ContactSelectionService>();
        services.AddTransient<EditContactService>();

        services.AddTransient<ManageCategoriesMenuView>();
        services.AddTransient<AddCategoryService>();
        services.AddTransient<DeleteCategoryService>();
        services.AddTransient<ManageCategoriesMenuService>();
        services.AddTransient<CategorySelectionService>();
        services.AddTransient<GenerateFullContactService>();
        services.AddTransient<EditCategoryService>();

        services.AddTransient<SendEmailService>();

        return services;
    }
}
