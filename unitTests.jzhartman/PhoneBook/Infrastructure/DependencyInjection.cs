using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PhoneBook.Application.Interfaces;
using PhoneBook.Infrastructure.Database;
using PhoneBook.Infrastructure.Email;
using PhoneBook.Infrastructure.Repositories;

namespace PhoneBook.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, HostBuilderContext context)
    {
        var config = context.Configuration;

        services.AddTransient<IDatabaseInitializer, DatabaseInitializer>();
        services.AddTransient<IContactRepository, ContactRepository>();
        services.AddTransient<ICategoryRepository, CategoryRepository>();

        var connectionString = context.Configuration.GetConnectionString("PhoneBook");

        services.AddDbContext<PhoneBookDbContext>(options =>
                                options.UseSqlite(connectionString));

        services.Configure<SmtpSettings>(config.GetSection("SmtpSettings"));
        services.AddTransient<IEmailService, EmailService>();

        return services;
    }
}
