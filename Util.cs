using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Interpark
{
    /// <summary>
    /// 
    /// </summary>
    internal static class Util
    {
        #region string

        /// <summary>
        /// 
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="startText"></param>
        /// <param name="endText"></param>
        /// <returns></returns>
        public static string Substring(this string origin, string startText, string endText)
        {
            return
                origin.Substring(origin.IndexOf(startText) + startText.Length,
                    origin.IndexOf(endText, origin.IndexOf(startText)) - origin.IndexOf(startText) - startText.Length)
                    .Trim();
        }

        public static string UrlDecode(this string origin)
        {
            return
                UrlDecode(origin, Encoding.GetEncoding("euc-kr"));
        }

        public static string UrlDecode(this string origin, Encoding encoding)
        {
            if (string.IsNullOrEmpty(origin)) return "";
            return
                System.Web.HttpUtility.UrlEncode(origin, encoding);
        }

        public static string UrlDecode(this IList<string> origins)
        {
            if (origins == null) return "";
            var result = "";
            for (int i = 0; i < origins.Count; i++)
            {
                //if (i == 0)
                {
                    result = System.Web.HttpUtility.UrlEncode(origins[i], Encoding.GetEncoding("euc-kr"));
                }
                //else if (i == origins.Count - 1)
                //{
                //    result += key + "=" + System.Web.HttpUtility.UrlEncode(origins[i]);    
                //}
                //else
                //{
                //    result += key + "=" + System.Web.HttpUtility.UrlEncode(origins[i]) + "&";    
                //}
            }
            

            return result;
        }

        #endregion

        #region logging

        public static void Logging(RichTextBox box, string log)
        {
            AppendText(box, Color.DarkRed, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            AppendText(box, Color.Black, string.Format(" - {0}\r\n", log));
        }

        public static void Logging(RichTextBox box, string log, Color color)
        {
            AppendText(box, Color.DarkRed, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            AppendText(box, color, string.Format(" - {0}\r\n", log));
        }

        public static void Logging(RichTextBox box, string log, string log1, Color color)
        {
            AppendText(box, Color.DarkRed, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            AppendText(box, Color.Black, string.Format(" - {0}", log));
            AppendText(box, color, string.Format(" - {0}\r\n", log1));
        }

        public static void Logging(RichTextBox box, string log, string error)
        {
            AppendText(box, Color.DarkRed, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            AppendText(box, Color.Black, string.Format(" - {0}", log));
            AppendText(box, Color.Red, string.Format(" - {0}\r\n", error));
        }

        private static void AppendText(RichTextBox box, Color color, string text)
        {
            int start = box.TextLength;
            box.AppendText(text);
            int end = box.TextLength;

            // Textbox may transform chars, so (end-start) != text.Length
            box.Select(start, end - start);
            {
                box.SelectionColor = color;
                // could set box.SelectionBackColor, box.SelectionFont too.
            }
            box.SelectionLength = 0; // clear

            Application.DoEvents();
        }

        #endregion

        #region cookie

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetGetCookieEx(string pchURL, string pchCookieName, StringBuilder pchCookieData,
            ref uint pcchCookieData, int dwFlags, IntPtr lpReserved);

        public const int INTERNET_COOKIE_HTTPONLY = 0x00002000;

        public static string GetGlobalCookies(string uri)
        {
            uint datasize = 1024;
            StringBuilder cookieData = new StringBuilder((int)datasize);
            if (InternetGetCookieEx(uri, null, cookieData, ref datasize, INTERNET_COOKIE_HTTPONLY, IntPtr.Zero)
                && cookieData.Length > 0)
            {
                return cookieData.ToString();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cookie"></param>
        /// <param name="httpWRequest"></param>
        /// <param name="domain"></param>
        public static void SetCookie(string cookie, HttpWebRequest httpWRequest, string domain = "")
        {
            var cookieSplit = cookie.Split(';');
            httpWRequest.CookieContainer = new CookieContainer();

            if (domain == "")
            {
                domain = httpWRequest.Host;
            }

            foreach (var s in cookieSplit)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    var key = s.Substring(0, s.IndexOf("="));
                    var value = s.Substring(s.IndexOf("=") + 1, s.Length - key.Length - 1).Trim();

                    httpWRequest.CookieContainer.Add(new Cookie(key.Trim(), value.Trim(), "/", domain));
                }
            }
        }

        #endregion

        internal static void ErrorLog(MethodBase methodBase, Exception ex, string p)
        {
            throw new NotImplementedException();
        }
    }
}
