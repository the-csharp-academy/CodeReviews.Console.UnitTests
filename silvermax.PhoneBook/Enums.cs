using System.Text.RegularExpressions;

namespace silvermax.PhoneBook;

internal class Enums
{
    internal static string ToDisplayName(Enum value) =>
        Regex.Replace(value.ToString(), "(?<!^)([A-Z])", " $1");

    internal enum Menu
    {
        AddContact,
        UpdateContact,
        ListContacts,
        DeleteContact,
        SendMail,
        SendSms,
        Exit
    }
}
