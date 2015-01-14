namespace BowlingApp
{
    internal static class StringExtensions
    {
        public static string Highlight(this string s)
        {
            return string.Format("{0}{1}{0}", HighlightingConsole.HighlightMarker, s);
        }
    }
}
