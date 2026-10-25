using PhoneBook.Application.Email;
using PhoneBook.ConsoleUI.Input;
using PhoneBook.ConsoleUI.Models;
using PhoneBook.ConsoleUI.Output;
using PhoneBook.Domain.Validation;
using PhoneBook.Domain.Validation.Errors;

namespace PhoneBook.ConsoleUI.Services.Email;

internal class SendEmailService
{
    private readonly Messages _messages;
    private readonly UserInput _userInput;
    private readonly SendEmailHandler _sendEmailHandler;

    public SendEmailService(Messages messages, UserInput userInput, SendEmailHandler sendEmailHandler)
    {
        _messages = messages;
        _userInput = userInput;
        _sendEmailHandler = sendEmailHandler;
    }
    internal async Task RunAsync(FullContactViewModel contact)
    {
        var subject = _userInput.GetEmailSubjectFromUser();
        var body = _userInput.GetEmailBodyFromUser();

        bool confirmEmail = _userInput.GetEmailContentsConfirmationFromUser(contact, subject, body);

        if (confirmEmail)
        {
            bool retrySend = true;
            int retryCount = 0;

            while (retrySend)
            {
                if (retryCount > 0)
                    _messages.RetryingSendEmailMessage(retryCount);

                var emailResult = await _sendEmailHandler.HandleAsync(new(contact.Email, subject, body));
                var errors = new List<Error>();

                if (emailResult is null)
                {
                    errors.Add(EmailErrors.NullResponse);
                }
                else if (emailResult.IsFailure)
                {
                    errors.AddRange(emailResult.Errors);
                }
                else
                {
                    _messages.EmailSendSuccessfulMessage();
                    _userInput.PressAnyKeyToContinue();
                    retrySend = false;
                }

                if (errors.Count > 0)
                {
                    _messages.ErrorMessage(errors);

                    retrySend = _userInput.GetRetrySendConfirmationFromUser();
                    if (retrySend) retryCount++;
                }
            }
        }
        else
        {
            _messages.EmailSendCancelledMessage();
            _userInput.PressAnyKeyToContinue();
        }
    }
}
