using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using silvermax.PhoneBook.DbAcess;
using Spectre.Console;
using Twilio;
using Twilio.Exceptions;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace silvermax.PhoneBook.Services;

public class SMSService(
    ContactDbContext db,
    IConfiguration config,
    UserInput userInput,
    CancellationToken ct)
{
    public async Task SendSMS()
    {
        var smsMessage = userInput.GetSMSInfo();

        var recipient = await db.Contacts.FirstOrDefaultAsync(c => c.Name == smsMessage.Name.ToLower() || c.Name.StartsWith(smsMessage.Name.ToLower() + " "), ct);
        UIHelper.ClearScreen();

        if (recipient is null)
        {
            AnsiConsole.MarkupLine("[red]Contact not found[/]");
            UIHelper.ContinueMessage();
            return;
        }

        var recipientNumber = recipient.PhoneNumber;

        if (string.IsNullOrWhiteSpace(recipientNumber))
        {
            AnsiConsole.MarkupLine("[red]Contact has no phone number configured[/]");
            UIHelper.ContinueMessage();
            return;
        }

        var authSid = config["Twilio:AccountSID"];
        var authToken = config["Twilio:AuthToken"];
        var phoneNumber = config["Twilio:PhoneNumber"];

        if (string.IsNullOrWhiteSpace(authSid) || string.IsNullOrWhiteSpace(authToken))
        {
            AnsiConsole.MarkupLine("[red]Twilio credentials not configured[/]");
            UIHelper.ContinueMessage();
            return;
        }

        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            AnsiConsole.MarkupLine("[red]Phone Number not configured[/]");
            UIHelper.ContinueMessage();
            return;
        }

        TwilioClient.Init(authSid, authToken);

        try
        {
            var message = MessageResource.Create(
            new PhoneNumber(recipientNumber),
            from: new PhoneNumber(phoneNumber),
            body: smsMessage.SMSMessage
            );
        }
        catch (ApiException ex)
        {
            AnsiConsole.WriteLine($"Twilio API error: {ex.Message} {ex.Code}");
            UIHelper.ContinueMessage();
            return;
        }
    }
}