namespace PhoneBook.Validation;

public class InputValidator
{
    public InputValidator() { }

    public static bool IsValidName(string input)
    {
        if(!string.IsNullOrWhiteSpace(input) && input.Length <=100)
        {
            return true;
        }
        return false;
    }

    public static bool IsValidPhoneNumber(string input)
    {
        if(!string.IsNullOrWhiteSpace(input) && System.Text.RegularExpressions.Regex.IsMatch(input, @"^\+?[0-9\-\s]{7,15}$"))
        {
            return true;
        }
        return false;
    }

    public static bool IsValidEmail(string input)
    {
        if(!string.IsNullOrWhiteSpace(input) && System.Text.RegularExpressions.Regex.IsMatch(input, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            return true;
        }
        return false;
    }
}
