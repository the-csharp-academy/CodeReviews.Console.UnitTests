using System.Net.Http.Headers;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
namespace PhoneBook.Messaging;

public enum MessageChannel
{
    Email,
    Sms
}

public class MessageSender
{
    private static readonly string MailgunApiKey = Environment.GetEnvironmentVariable("MailgunApiKey")!;
    private static readonly string MailgunDomain = Environment.GetEnvironmentVariable("MailgunDomain")!;
    private static readonly string EmailFrom = Environment.GetEnvironmentVariable("EmailFrom")!;
    private static readonly string TwilioSid = Environment.GetEnvironmentVariable("TwilioSid")!;
    private static readonly string TwilioToken = Environment.GetEnvironmentVariable("TwilioToken")!;
    private static readonly string SmsFrom = Environment.GetEnvironmentVariable("SmsFrom")!;

    private static readonly System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient();
    public static async Task SendAsync(MessageChannel channel, string recipient, string subject, string body)
    {
        if (channel == MessageChannel.Email)
        {
            var request = new HttpRequestMessage(
                System.Net.Http.HttpMethod.Post,
                $"https://api.mailgun.net/v3/{MailgunDomain}/messages");

            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"api:{MailgunApiKey}")));

            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("from", EmailFrom),
                new KeyValuePair<string, string>("to", recipient),
                new KeyValuePair<string, string>("subject", subject),
                new KeyValuePair<string, string>("text", body)
            });
            request.Content = formContent;

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Mailgun error ({response.StatusCode}): {error}");
            }
        }
        else
        {
            TwilioClient.Init(TwilioSid, TwilioToken);
            var text = string.IsNullOrWhiteSpace(subject) ? body : $"{subject}\n{body}";
            await MessageResource.CreateAsync(body: text, from: new PhoneNumber(SmsFrom), to: new PhoneNumber(recipient));
        }
    }
}