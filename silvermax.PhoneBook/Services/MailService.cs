using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MimeKit;
using silvermax.PhoneBook.DbAcess;
using Spectre.Console;
using System.Net.Mail;

namespace silvermax.PhoneBook.Services;

public class MailService( 
    IConfiguration config, 
    UserInput userInput,
    ContactDbContext db,
    CancellationToken ct)
{
    public async Task SendMail()
    {
        var emailInfo = userInput.GetEmailInfo();

        var recipient = await db.Contacts.FirstOrDefaultAsync(c => c.Name == emailInfo.Name.ToLower() || c.Name.StartsWith(emailInfo.Name.ToLower() + " "), ct);
        UIHelper.ClearScreen();

        if (recipient is null)
        {
            AnsiConsole.MarkupLine("[red]Contact not found[/]");
            UIHelper.ContinueMessage();
            return;
        }

        var recipientAddress = recipient.Email;

        if (string.IsNullOrWhiteSpace(recipientAddress))
        {
            AnsiConsole.MarkupLine("[red]Contact has no email address configured[/]");
            UIHelper.ContinueMessage();
            return;
        }

        try
        {
            _ = new MailAddress(recipientAddress);
        }
        catch
        {
            AnsiConsole.MarkupLine("[red]Contact email is not a valid address[/]");
            UIHelper.ContinueMessage();
            return;
        }

        var sender = config["Email:Personal"]!;
        var password = config["Email:Password"]!;

        if (string.IsNullOrWhiteSpace(sender) || string.IsNullOrWhiteSpace(password))
        {
            AnsiConsole.MarkupLine("[red]SMTP credentials not configured[/]");
            UIHelper.ContinueMessage();
            return;
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Personal", sender));
        message.To.Add(new MailboxAddress("Recipient", recipientAddress));
        message.Subject = emailInfo.Subject;
        message.Body = new TextPart("plain")
        {
            Text = emailInfo.Message
        };

        using var client = new MailKit.Net.Smtp.SmtpClient();
        client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        try
        {

            client.Authenticate(sender, password);
        }
        catch (MailKit.Security.AuthenticationException)
        {
            AnsiConsole.MarkupLine("[red]SMTP authentication failed: invalid username or password.[/]");
            client.Disconnect(true);
            UIHelper.ContinueMessage();
            return;
        }
        client.Send(message);
        client.Disconnect(true);

        AnsiConsole.MarkupLine("[green]Email sent successfully[/]");
        UIHelper.ContinueMessage();
    }
}
