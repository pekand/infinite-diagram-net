using System.Text.RegularExpressions;

#nullable disable

namespace Diagram
{
    /// <summary>
    /// Regex patterns repository</summary>
    public partial class Patterns 
    {
        /*************************************************************************************************************************/
        // TYPE

        [GeneratedRegex("^(\\d+)$")]
        private static partial Regex IntMatch();

        /// <summary>
        /// check if string is integer</summary>
        public static bool IsNumber(string text)
        {
            Match matchNumber = IntMatch().Match(text);

            if (matchNumber.Success)
            {
                return true;
            }

            return false;
        }

        [GeneratedRegex("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$")]
        private static partial Regex ColorMatch();

        /// <summary>
        /// check if string is html color format starting with hash</summary>
        public static bool IsColor(string text)
        {
            Match matchNumber = ColorMatch().Match(text);

            if (matchNumber.Success)
            {
                return true;
            }

            return false;
        }

        /*************************************************************************************************************************/
        // COMMAND

        /// <summary>
        /// windows shell command with arguments spit for run command from node link</summary>
        public static string[] SplitCommand(string cmd)
        {
            // ^(?:\s*"?)((?<=")(?:\\.|[^"\\])*(?=")|[^ "]+)(?:"?\s*)(.*)
            string pattern = "";
            pattern += "^";
            pattern += "(?:\\s*\"?)";// skip start space and quote
            pattern += "(";
            pattern += "(?:(?<=\")(?:\\\\.|[^\"\\\\])*(?=\"))";// match command with quotas
            pattern += "|";
            pattern += "(?:[^ \"]+)"; // match command without quotas
            pattern += ")";
            pattern += "(?:\"?\\s*)"; //skip space between command and arguments
            pattern += "(.*)"; // arguments

            MatchCollection matches = Regex.Matches(cmd, pattern);

            string command = "";
            string arguments = "";

            foreach (Match match in matches)
            {
                command = match.Groups[1].Value;
                arguments = match.Groups[2].Value;
            }

            return [command, arguments];
        }

        [GeneratedRegex(@"^([^#]+)#(.*)$")]
        private static partial Regex HashtagMatch();

        /// <summary>
        /// check if file path ending on hash with string after hastag 
        /// Parse node link to file for open on position</summary>
        public static bool HasHastag(string link, ref string fileName, ref string searchString)
        {
            Match matchFileOpenOnPosition = HashtagMatch().Match(link.Trim());

            if (matchFileOpenOnPosition.Success)
            {
                fileName = matchFileOpenOnPosition.Groups[1].Value;
                searchString = matchFileOpenOnPosition.Groups[2].Value;
                return true;
            }

            return false;
        }

        [GeneratedRegex(@"^\s*#(\\d+)\s*$")]
        private static partial Regex GotoIdCommandMatch();

        /// <summary>
        /// check if string start with hash</summary>
        public static bool IsGotoIdCommand(string text)
        {
            Match matchNumber = GotoIdCommandMatch().Match(text);

            if (matchNumber.Success)
            {
                return true;
            }

            return false;
        }

        [GeneratedRegex(@"^\s*#(\\d+)\s*$", RegexOptions.IgnoreCase, "en-US")]
        private static partial Regex GetGotoIdCommandMatch();

        /// <summary>
        ///  parse integer (representing node id) from goto command string </summary>
        public static long GetGotoIdCommand(string text)
        {
            Match match = GetGotoIdCommandMatch().Match(text);

	        if (match.Success)
	        {
	            string key = match.Groups[1].Value;
                return Int64.Parse(key);
            }

            return -1;
        }

        [GeneratedRegex(@"^\s*#([\\w ]+)\s*$")]
        private static partial Regex GotoStringCommandMatch();

        /// <summary>
        ///  Check if string is in format of node link search command
        ///  Example: #search for string</summary>
        public static bool IsGotoStringCommand(string text)
        {
            Match matchNumber = GotoStringCommandMatch().Match(text);

            if (matchNumber.Success)
            {
                return true;
            }

            return false;
        }

        [GeneratedRegex(@"^\s*#([\\w ]+)\s*$", RegexOptions.IgnoreCase, "en-US")]
        private static partial Regex GetGotoStringCommandMatch();

        /// <summary>
        ///  Get argument from search command
        ///  Example: #search for string</summary>
        public static String GetGotoStringCommand(string text)
        {
            Match match = GetGotoStringCommandMatch().Match(text);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return null;
        }

        /*************************************************************************************************************************/
        // WEB

        [GeneratedRegex(@"^(http|https)://[^ ]*$")]
        private static partial Regex URLMatch();

        /// <summary>
        /// check if url start on http or https </summary>
        public static bool IsURL(String url)
        {
            return (URLMatch().IsMatch(url));
        }

        [GeneratedRegex(@"^(https)://[^ ]*$")]
        private static partial Regex HttpsURLMatch();

        /// <summary>
        /// check if url start on https </summary>
        public static bool IsHttpsURL(String url)
        {
            return HttpsURLMatch().IsMatch(url);
        }

        [GeneratedRegex(@"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$", RegexOptions.IgnoreCase, "en-US")]
        private static partial Regex EmailMatch();

        /// <summary>
        /// </summary>
        public static bool IsEmail(String email)
        {
            return EmailMatch().IsMatch(email);
        }

        [GeneratedRegex("<title>(.*?)</title>", RegexOptions.IgnoreCase | RegexOptions.Singleline, "en-US")]
        private static partial Regex TitleMatch();

        /// <summary>
        /// get page title from html</summary>
        public static string MatchWebPageTitle(String page)
        {
            return TitleMatch().Match(page).Groups[1].Value;
        }

        [GeneratedRegex("<meta.*?charset=['\"]?(?<Encoding>[^\"']+)['\"]?", RegexOptions.IgnoreCase, "en-US")]
        private static partial Regex EncodingMatch();

        /// <summary>
        /// get page encoding from html</summary>
        public static string MatchWebPageEncoding(String page)
        {
            return EncodingMatch().Match(page).Groups["Encoding"].Value;
        }

        [GeneratedRegex("<meta.*?http-equiv=\"refresh\".*?(CONTENT|content)=[\"']\\d;\\s?(URL|url)=(?<url>.*?)([\"']\\s*\\/?>)", RegexOptions.IgnoreCase, "en-US")]
        private static partial Regex WebPageRedirectUrlMatch();

        /// <summary>
        /// get page redirect url from html meta tag</summary>
        public static string MatchWebPageRedirectUrl(String page)
        {
            return WebPageRedirectUrlMatch().Match(page).Groups["url"].Value;
        }

        [GeneratedRegex(@"^\s*@(\w+){1}\s*$")]
        private static partial Regex ScriptIdMatch();

        /// <summary>
        /// </summary>
        public static bool IsScriptId(String link, string id)
        {

            Match match = ScriptIdMatch().Match(link);
            if (match.Success && match.Groups[1].Value == id)
                return true;

            return false;
        }

        [GeneratedRegex(@"open:(.*)", RegexOptions.IgnoreCase, "en-US")]
        private static partial Regex GetOpenCommandMatch();

        public static string GetOpenCommand(String Message)
        {
            Match match = GetOpenCommandMatch().Match(Message);
            if (match.Success)
            {
                string FileName = match.Groups[1].Value;
                return FileName;
            }

            return null;
        }

        [GeneratedRegex(@"^[0-9]{2}:[0-9]{2}:[0-9]{2}$", RegexOptions.IgnoreCase, "en-US")]
        private static partial Regex TimeMatch();

        public static bool IsTime(String text)
        {
            return TimeMatch().Match(text).Success;
        }
    }
}
