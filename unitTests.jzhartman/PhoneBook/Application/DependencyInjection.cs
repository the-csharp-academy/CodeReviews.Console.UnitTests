using Microsoft.Extensions.DependencyInjection;
using PhoneBook.Application.Categories.AddCategory;
using PhoneBook.Application.Categories.DeleteCategory;
using PhoneBook.Application.Categories.GetAllCategories;
using PhoneBook.Application.Categories.GetCategoryById;
using PhoneBook.Application.Categories.UpdateCategory;
using PhoneBook.Application.Contacts.AddContact;
using PhoneBook.Application.Contacts.DeleteContact;
using PhoneBook.Application.Contacts.EditContact;
using PhoneBook.Application.Contacts.GetAllContacts;
using PhoneBook.Application.Contacts.GetContactsByCategoryId;
using PhoneBook.Application.Contacts.SaveChanges;
using PhoneBook.Application.Contacts.SetCategoryIdToDefault;
using PhoneBook.Application.Email;
using PhoneBook.Application.GetById;

namespace PhoneBook.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<GetAllContactsHandler>();
        services.AddTransient<GetAllContactsByCategoryIdHandler>();
        services.AddTransient<GetContactByIdHandler>();
        services.AddTransient<AddContactHandler>();
        services.AddTransient<DeleteContactHandler>();
        services.AddTransient<SaveChangesHandler>();
        services.AddTransient<EditContactHandler>();
        services.AddTransient<SetCategoryIdForContactsToDefaultHandler>();

        services.AddTransient<AddCategoryHandler>();
        services.AddTransient<DeleteCategoryByIdHandler>();
        services.AddTransient<GetCategoryByIdHandler>();
        services.AddTransient<GetAllCategoriesHandler>();
        services.AddTransient<UpdateCategoryNameHandler>();

        services.AddTransient<SendEmailHandler>();

        return services;
    }
}
