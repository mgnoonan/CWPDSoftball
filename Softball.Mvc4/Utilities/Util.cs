using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Text;

namespace Softball.Mvc4.Utilities
{
    /// <summary>
    /// Summary description for Util.
    /// </summary>
    public class Util
    {

        private const int _timeout = 30000;

        public Util()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public static string GetUrl(string url)
        {
            WebRequest request = WebRequest.Create(url);
            
            //' Get the response.
            using (WebResponse response = request.GetResponse())
            {
                //' Get the stream containing content returned by the server.
                Stream dataStream = response.GetResponseStream();
                //' Open the stream using a StreamReader for easy access.
                using (var reader = new StreamReader(dataStream))
                {
                    //' Read the content.
                    return reader.ReadToEnd();
                }
            }
        }

        public static string DownloadFile(string url, string fileName, int retryCount)
        {
            bool success = false;

            File.Delete(fileName);

            while (!success && retryCount-- > 0)
            {
                try
                {
                    WebClient client = new WebClient();
                    client.DownloadFile(url, fileName);
                    success = true;
                }
                catch (Exception ex) { Console.WriteLine("{0}. Retrying...", ex.Message); }
            }

            return fileName;
        }

        public static string PostUrlWithRetries(string url, int retryCount)
        {
            string response = string.Empty;

            while (retryCount-- > 0)
            {
                try
                {
                    response = PostUrl(url);
                    if (response.Length > 0)
                        break;
                }
                catch { }
            }

            return response;
        }

        public static string PostUrl(string url)
        {
            return PostUrl(url, string.Empty);
        }

        public static string PostUrl(string url, string postData)
        {
            string response = string.Empty;

            try
            {
                response = HttpPost.Post(url, postData, _timeout);
            }
            catch
            {
                return response;
            }

            return response;
        }

        public static ArrayList PostUrlList(string url, string regex, string postData)
        {
            string response = PostUrl(url, postData);

            return GetMatches(regex, RegexOptions.IgnoreCase, response);
        }

        public static string GetMatch(string pattern, string responseData)
        {
            ArrayList list = GetMatches(pattern, RegexOptions.IgnoreCase, responseData);

            if (list.Count == 0)
                return string.Empty;
            else
                return list[0].ToString();
        }

        public static ArrayList GetMatches(string pattern, string responseData)
        {
            return GetMatches(pattern, RegexOptions.IgnoreCase, responseData);
        }

        public static ArrayList GetMatches(string pattern, RegexOptions options, string responseData)
        {
            ArrayList list = new ArrayList();
            MatchCollection matches;

            // Declare object variable of type Regex.
            Regex r = new Regex(pattern, options);
            matches = r.Matches(responseData);

            for (int i = 0; i < matches.Count; i++)
            {
                string match = matches[i].Value;
                list.Add(match);
            }

            return list;
        }

        public static ArrayList SplitTableRows(string table)
        {
            ArrayList list = new ArrayList();
            int start = table.IndexOf("<tr>");

            while (start > 0)
            {
                int end = table.IndexOf("<tr>", start + 1);
                if (end == -1)
                {
                    list.Add(table.Substring(start));
                    break;
                }

                list.Add(table.Substring(start, end - start));
                start = end;
            }

            return list;
        }

        public static string stripHtml(string sText)
        {
            String result;

            // Strip out HTML tags
            result = Regex.Replace(sText, "<[^>]*>", " ");

            // Replace HTML constructs
            result = result.Replace("&nbsp;", " ");
            result = result.Replace("&#38;", "&");
            result = result.Replace("&#63;", "?");

            // Replace whitespace
            result = result.Replace("\x09", " ");
            result = result.Replace("\x0A\x0A", "");
            result = result.Replace("\x0A", "\x0D\x0A");
            result = result.Replace("\x0D\x0D", "\x0D");
            result = result.Replace("  ", " ");

            return result;
        }

        public static string stripJavascriptAndCss(string text)
        {
            string commentPattern = @"(?'comment'<!--.*?--[ \n\r]*>)";
            string embeddedScriptComments = @"(\/\*.*?\*\/|\/\/.*?[\n\r])";
            string scriptPattern = string.Format(@"(?'script'<[ \n\r]*script[^>]*>(.*?{0}?)*<[ \n\r]*/script[^>]*>)", embeddedScriptComments);
            string cssPattern = string.Format(@"(?'style'<[ \n\r]*style[^>]*>(.*?{0}?)*<[ \n\r]*/style[^>]*>)", embeddedScriptComments);

            // the pattern includes the comment and script sub-patterns
            string pattern = string.Format(@"(?s)({0}|{1}|{2})", commentPattern, scriptPattern, cssPattern);
            Regex re = new Regex(pattern, RegexOptions.IgnoreCase);

            return re.Replace(text, string.Empty);
        }

        public static string stripSelectStatement(string text)
        {
            string optionPattern = @"(?'select'<[ \n\r]*select[^>]*>(.*?)*<[ \n\r]*/select[^>]*>)";

            // the pattern includes the comment and script sub-patterns
            string pattern = string.Format(@"(?s)({0})", optionPattern);
            Regex re = new Regex(pattern, RegexOptions.IgnoreCase);

            return re.Replace(text, string.Empty);
        }

        public static string stripCrLf(string text, string replacementString)
        {
            string pattern = @"[\n\r]";
            Regex re = new Regex(pattern, RegexOptions.IgnoreCase);

            return re.Replace(text, replacementString);
        }

        public static string stripPattern(string text, string pattern, string replacementString)
        {
            Regex re = new Regex(pattern, RegexOptions.IgnoreCase);

            return re.Replace(text, replacementString);
        }

        public static string GetNextNonBlankLine(StringReader reader)
        {
            string line = string.Empty;

            // Loop until we hit a non-blank line
            while (line == string.Empty)
            {
                if ((line = reader.ReadLine()) == null)
                {
                    line = string.Empty;
                    break;
                }

                line = line.Trim();
            }

            return line;
        }

        public static string GetNextTextBlock(StringReader reader, int maxLines)
        {
            string line = GetNextNonBlankLine(reader);
            StringBuilder sb = new StringBuilder(line + Environment.NewLine);
            int lineCount = 1;

            // Now loop until we hit a blank line or maxLines is reached
            line = reader.ReadLine().Trim();
            while (line != string.Empty && lineCount < maxLines)
            {
                lineCount++;
                sb.Append(line);
                sb.Append(Environment.NewLine);
                line = reader.ReadLine().Trim();
            }

            return sb.ToString();
        }
    }
}
