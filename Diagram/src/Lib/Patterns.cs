using System;
using System.Text.RegularExpressions;

namespace Diagram
{
    /// <summary>
    /// Regex patterns repository</summary>
    public class Patterns //UID4820527610
    {
        /*************************************************************************************************************************/
        // TYPE

        /// <summary>
        /// check if string is integer</summary>
        public static bool IsNumber(string text)
        {
            Match matchNumber = (new Regex("^(\\d+)$")).Match(text);

            if (matchNumber.Success)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// check if string is html color format starting with hash</summary>
        public static bool IsColor(string text)
        {
            Match matchNumber = (new Regex("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$")).Match(text);

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

            return new string[] { command, arguments };
        }

        /// <summary>
        /// check if file path ending on hash with string after hastag 
        /// Parse node link to file for open on position</summary>
        public static bool HasHastag(string link, ref string fileName, ref string searchString)
        {
            Match matchFileOpenOnPosition = (new Regex(@"^([^#]+)#(.*)$")).Match(link.Trim());

            if (matchFileOpenOnPosition.Success)
            {
                fileName = matchFileOpenOnPosition.Groups[1].Value;
                searchString = matchFileOpenOnPosition.Groups[2].Value;
                return true;
            }

            return false;
        }     

        /// <summary>
        /// check if string start with hash</summary>
        public static bool IsGotoIdCommand(string text)
        {
            Match matchNumber = (new Regex(@"^\s*#(\\d+)\s*$")).Match(text);

            if (matchNumber.Success)
            {
                return true;
            }

            return false;
        }        

        /// <summary>
        ///  parse integer (representing node id) from goto command string </summary>
        public static long GetGotoIdCommand(string text)
        {
            Match match = Regex.Match(text, @"^\s*#(\\d+)\s*$", RegexOptions.IgnoreCase);

	        if (match.Success)
	        {
	            string key = match.Groups[1].Value;
                return Int64.Parse(key);
            }

            return -1;
        }

        /// <summary>
        ///  Check if string is in format of node link search command
        ///  Example: #search for string</summary>
        public static bool IsGotoStringCommand(string text)
        {
            Match matchNumber = (new Regex(@"^\s*#([\\w ]+)\s*$")).Match(text);

            if (matchNumber.Success)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///  Get argument from search command
        ///  Example: #search for string</summary>
        public static String GetGotoStringCommand(string text)
        {
            Match match = Regex.Match(text, @"^\s*#([\\w ]+)\s*$", RegexOptions.IgnoreCase);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return null;
        }

        /*************************************************************************************************************************/
        // WEB

        /// <summary>
        /// check if url start on http or https </summary>
        public static bool IsURL(String url)
        {
            return (Regex.IsMatch(url, @"^(http|https)://[^ ]*$"));
        }

        /// <summary>
        /// check if url start on https </summary>
        public static bool IsHttpsURL(String url)
        {
            return (Regex.IsMatch(url, @"^(https)://[^ ]*$"));
        }
        
        /// <summary>
        /// </summary>
        public static bool IsEmail(String email)
        {
            string validEmailPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
            + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
            + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

            Regex ValidEmailRegex = new Regex(validEmailPattern, RegexOptions.IgnoreCase);

            return ValidEmailRegex.IsMatch(email);
        }

        /// <summary>
        /// get page title from html</summary>
        public static string MatchWebPageTitle(String page)
        {
            return Regex.Match(
                page,
                "<title>(.*?)</title>",
                RegexOptions.IgnoreCase | RegexOptions.Singleline
            ).Groups[1].Value;
        }

        /// <summary>
        /// get page encoding from html</summary>
        public static string MatchWebPageEncoding(String page)
        {
            return Regex.Match(
                page,
                "<meta.*?charset=['\"]?(?<Encoding>[^\"']+)['\"]?",
                RegexOptions.IgnoreCase
            ).Groups["Encoding"].Value;
        }

        /// <summary>
        /// get page redirect url from html meta tag</summary>
        public static string MatchWebPageRedirectUrl(String page)
        {
            return Regex.Match(
                page,
                "<meta.*?http-equiv=\"refresh\".*?(CONTENT|content)=[\"']\\d;\\s?(URL|url)=(?<url>.*?)([\"']\\s*\\/?>)",
                RegexOptions.IgnoreCase
            ).Groups["url"].Value;
        }

        /// <summary>
        /// </summary>
        public static bool IsScriptId(String link, string id)
        {
            Regex regex = new Regex(@"^\s*@(\w+){1}\s*$");
            Match match = regex.Match(link);
            if (match.Success && match.Groups[1].Value == id)
                return true;

            return false;
        }

    }
}
