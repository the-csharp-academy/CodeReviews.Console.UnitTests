using Phonebook.Controllers;
using Phonebook.Data;
using Phonebook.Services;
using Phonebook.UI;

Console.OutputEncoding = System.Text.Encoding.UTF8;

try
{
    // Code-First: EF creates the database + schema (and seed data) on first run
    using var context = new AppDbContext();
    context.Database.EnsureCreated();

    var contactService = new ContactService(context);
    var emailService = new EmailService();
    var smsService = new SmsService();

    var controller = new ContactController(contactService, emailService, smsService);

    await controller.RunAsync();
}
catch (Exception ex)
{
    UserInterface.ShowError($"The app ran into an unexpected problem: {ex.Message}");
}
