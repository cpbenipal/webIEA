using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Flexpage.Helpers
{
    public static class HtmlScrambler
    {
        private static Regex charsToReplace = new Regex(@"[@\.-]", RegexOptions.Compiled);

        public static string ScrambleEmails(string html)
        {
            if(html == null)
            {
                return null;
            }
            Regex re = new Regex(
                @"<a\s+[^>]*href=[""']?mailto:[^(</a>)][^""'>\s]*?.*?>(.*?)</a>",
                RegexOptions.IgnoreCase);

            return re.Replace(html, matchEvaluator);
        }

        private static string scramble(string text)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in text)
            {
                sb.Append(((byte)c).ToString("X2"));
            }

            return sb.ToString();
        }

        private static string matchEvaluator(Match match)
        {
            string email = match.Value;
            StringBuilder scrambledEmail = new StringBuilder();
            string id = Guid.NewGuid().ToString("n");
            scrambledEmail.AppendFormat("<span id=\"{0}\" title=\"{1}\" fpscrambledmail>{2}</span>",
                id, scramble(match.Value), charsToReplace.Replace(match.Groups[1].Value, ""));

            return scrambledEmail.ToString();
        }

    }
}