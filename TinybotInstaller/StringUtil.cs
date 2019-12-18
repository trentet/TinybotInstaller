using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
