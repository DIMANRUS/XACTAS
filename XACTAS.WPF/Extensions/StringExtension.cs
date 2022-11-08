namespace XACTAS.WPF.Extensions;
public static class StringExtension
{
    public static bool ContainsAny(this string s, params string[] substrings)
    {
        if (string.IsNullOrEmpty(s) || substrings == null)
            return false;

        return substrings.Any(substring => s.Contains(substring));
    }
}