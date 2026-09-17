namespace Phonebook.Services;

/// <summary>
/// The "read until valid" loop used by the console prompts.
/// Returns the trimmed input once it passes validation, or <see cref="string.Empty"/>
/// when the user enters nothing (which the caller treats as "cancel this flow").
/// The reader/validator/reporter callbacks keep the loop free of any UI dependency,
/// so the behaviour is unit-testable without a console.
/// </summary>
public static class InputCollector
{
    public static string ReadValid(Func<string?> read, Func<string, bool> isValid, Action<string> onInvalid)
    {
        while (true)
        {
            var value = (read() ?? string.Empty).Trim();
            if (value.Length == 0)
                return string.Empty;

            if (isValid(value))
                return value;

            onInvalid(value);
        }
    }
}
