using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;

namespace Diagram
{
    /// <summary>
    /// repository of network and web related functions</summary>
    public class Network //UID6787833981
    {
        /*************************************************************************************************************************/
        // GET CONTENT

        /// <summary>
        /// download https page and parse title from it </summary>
        public static string GetWebPageTitle(
            string url, 
            string proxy_uri = "",
            string proxy_password = "",
            string proxy_username = "",
            int level = 0
            )
        {
            string page = Network.GetWebPage(
                url,
                proxy_uri,
                proxy_password,
                proxy_username,
                level = 0,
                null,
                false
            );

            string title = "";
            try {
                title = Patterns.MatchWebPageTitle(page);

            }
            catch (Exception ex)
            {
                Program.log.Write("get link name error: " + ex.Message);
            }

            return (title.Trim() == "")? url : title.Trim();
        }


        /// <summary>
        /// download https page and parse title from it </summary>
        public static string GetWebPage(
            string url,
            string proxy_uri = "",
            string proxy_password = "",
            string proxy_username = "",
            int level = 0,
            CookieContainer cookieContainer = null,
            bool skiphttps = false
            )
        {

            Program.log.Write("get title from: " + url);

            if (skiphttps)
            {
                url = url.Replace("https:", "http:");
            }

            string page = "";

            try
            {
                
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AllowAutoRedirect = false;
                request.UseDefaultCredentials = true;
                request.Timeout = 2000;

                if (proxy_uri != "" ||
                    proxy_password != "" ||
                    proxy_username != ""
                    )
                {
                    // set proxy credentials
                    WebProxy myProxy = new WebProxy();
                    if (proxy_uri != "")
                    {
                        Uri newUri = new Uri(proxy_uri);
                        myProxy.Address = newUri;
                    }

                    if (proxy_password != "" ||
                        proxy_username != ""
                        )
                    {
                        myProxy.Credentials = new NetworkCredential(
                            proxy_username,
                            proxy_password
                        );
                    }
                    request.Proxy = myProxy;
                }
                else
                {
                    request.Proxy = WebRequest.GetSystemWebProxy();
                }

                if (cookieContainer == null)
                {
                    cookieContainer = new CookieContainer();
                }

                if (cookieContainer != null)
                {
                    request.CookieContainer = cookieContainer;
                }

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if ((int)response.StatusCode >= 300 && (int)response.StatusCode <= 399)
                {
                    string uriString = response.Headers["Location"];

                    if (level < 10)
                    {
                        return Network.GetWebPage(
                                uriString,
                                proxy_uri,
                                proxy_password,
                                proxy_username,
                                level + 1,
                                cookieContainer,
                                skiphttps
                            );
                    }
                }

                Stream resStream = response.GetResponseStream();

                MemoryStream memoryStream = new MemoryStream();
                resStream.CopyTo(memoryStream);

                // read stream with utf8
                memoryStream.Seek(0, SeekOrigin.Begin);
                using (StreamReader reader = new StreamReader(memoryStream)) {
                    page = reader.ReadToEnd();
                }

                string encoding = Patterns.MatchWebPageEncoding(page);

                // try redirect 
                if (level < 10)
                {
                    string redirect = Patterns.MatchWebPageRedirectUrl(page);

                    if (redirect.Trim() != "")
                    {
                        Uri.TryCreate(new Uri(url), redirect, out Uri result);
                        return Network.GetWebPage(
                            result.ToString(),
                            proxy_uri,
                            proxy_password,
                            proxy_username,
                            level + 1,
                            cookieContainer,
                            skiphttps
                        );
                    }
                }

                // use different encoding
                if (encoding.Trim() != "" && encoding.ToLower() != "utf-8")
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    using (StreamReader reader2 = new StreamReader(memoryStream, System.Text.Encoding.GetEncoding(encoding)))
                    {
                        page = reader2.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                Program.log.Write("get link name error: " + ex.Message);
            }

            return page;
        }

        public static bool DownloadFile(string sourceUrl, string pathToSave)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile(sourceUrl, pathToSave);
                }

                return true;

            } catch (Exception ex)
            {
                while (ex != null)
                {
                    Program.log.Write("download file error: " + ex.Message);
                    ex = ex.InnerException;
                }
            }

            return false;
        }
        /*************************************************************************************************************************/
        // OPEN

        /// <summary>
        /// open url in os default browser </summary>
        public static void OpenUrl(String url)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                System.Diagnostics.Process.Start(url);
            }
        }
		
    }
}
