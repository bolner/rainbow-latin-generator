#pragma warning disable CA1050

public static class StringExtensions
{
    public static string FirstCharToUpper(this string input) => input switch
    {
        null => throw new ArgumentNullException(nameof(input)),
        "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
        _ => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1))
    };

    public static string Left(this string str, int maxLength)
    {
        if (string.IsNullOrEmpty(str)) {
            return str;
        }

        return str.Substring(0, Math.Min(str.Length, maxLength));
    }
}
