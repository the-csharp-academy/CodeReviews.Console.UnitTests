using System.Net.Http.Headers;
using System.Text;
using Phonebook.Models;

namespace Phonebook.Services;

/// <summary>
/// Sends SMS through Twilio's REST API. Reads credentials from environment
/// variables (TWILIO_ACCOUNT_SID, TWILIO_AUTH_TOKEN, TWILIO_FROM_NUMBER).
/// Without them the service runs in sandbox mode and returns the composed
/// message instead of sending — no credentials ever live in the repo.
/// </summary>
public class SmsService
{
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(15) };

    private readonly string? _accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
    private readonly string? _authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
    private readonly string? _fromNumber = Environment.GetEnvironmentVariable("TWILIO_FROM_NUMBER");

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(_accountSid) &&
        !string.IsNullOrWhiteSpace(_authToken) &&
        !string.IsNullOrWhiteSpace(_fromNumber);

    public async Task<(bool Success, string? Message)> SendSmsAsync(Contact to, string text)
    {
        try
        {
            if (!IsConfigured)
            {
                return (true,
                    $"SMS ready to send (sandbox: set TWILIO_ACCOUNT_SID, TWILIO_AUTH_TOKEN and TWILIO_FROM_NUMBER to send for real).\n" +
                    $"To: {to.PhoneNumber}\nMessage: {text}");
            }

            // Twilio expects E.164 numbers (e.g. +27821234567); sandbox accepts any validated format
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_accountSid}:{_authToken}"));

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"https://api.twilio.com/2010-04-01/Accounts/{_accountSid}/Messages.json")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["To"] = to.PhoneNumber,
                    ["From"] = _fromNumber!,
                    ["Body"] = text,
                }),
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            var response = await Http.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return (false, $"SMS provider returned {response.StatusCode}: {responseBody}");

            return (true, $"SMS sent to {to.PhoneNumber}.");
        }
        catch (Exception ex)
        {
            return (false, $"Could not send SMS: {ex.Message}");
        }
    }
}
