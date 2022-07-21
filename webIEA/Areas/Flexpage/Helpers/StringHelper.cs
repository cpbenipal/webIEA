using System.Text.RegularExpressions;

namespace Flexpage.Helpers
{
    public static class StringHelper
    {
        public static string RemoveTags(string htmlText, string replacement=" ")
        {
            return Regex.Replace(htmlText, "(<[^>]*>|<[^>]*$)", replacement);
        }
        public static string RemoveTagsAndEscapeChars(string htmlText, string replacement = "")
        {
            return Regex.Replace(htmlText, "<[^>]*>|&[^;]*;", replacement);
        }
        public static string RemoveIllegalCharacters(string input)
        {
            // cast the input to a string  
            string data = input.ToString();

            // replace illegal characters in XML documents with their entity references  
            data = data.Replace("&", "&amp;");
            data = data.Replace("\"", "&quot;");
            data = data.Replace("'", "&apos;");
            data = data.Replace("<", "&lt;");
            data = data.Replace(">", "&gt;");

            return data;
        }
    }
}