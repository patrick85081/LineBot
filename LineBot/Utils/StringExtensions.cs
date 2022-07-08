namespace LineBot.Utils
{
    public static class StringExtensions
    {
        public static bool EqualsIgnoreCase(this string source, string other)
        {
            return string.Equals(source, other, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}