namespace TinybotInstaller
{
    public static class StringUtil
    {
        public static string DQuote(string text)
        {
            return "\"" + text + "\"";
        }

        public static string SQuote(string text)
        {
            return "'" + text + "'";
        }
    }
}
