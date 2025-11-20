using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;

#nullable disable

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
                

                HttpClientHandler handler = new()
                {
                    AllowAutoRedirect = false,             
                    UseDefaultCredentials = true

                };

                if (proxy_uri != "" ||
                    proxy_password != "" ||
                    proxy_username != ""
                    )
                {
                    // set proxy credentials
                    WebProxy myProxy = new()
                    {
                        BypassProxyOnLocal = true,
                        UseDefaultCredentials = true
                    };

                    if (proxy_uri != "")
                    {
                        Uri newUri = new(proxy_uri);
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
                    handler.Proxy = myProxy;
                }
                else
                {
                    handler.Proxy = WebRequest.GetSystemWebProxy();
                }

                cookieContainer ??= new CookieContainer();

                if (cookieContainer != null)
                {
                    handler.CookieContainer = cookieContainer;
                }

                using HttpClient client = new(handler);

                client.Timeout = TimeSpan.FromMilliseconds(2000);

                using Stream resStream = client.GetStreamAsync(url).GetAwaiter().GetResult();

                MemoryStream memoryStream = new();
                resStream.CopyTo(memoryStream);

                // read stream with utf8
                memoryStream.Seek(0, SeekOrigin.Begin);
                using (StreamReader reader = new(memoryStream))
                {
                    page = reader.ReadToEnd();
                }

                string encoding = Patterns.MatchWebPageEncoding(page);

                // use different encoding
                if (encoding.Trim() != "" && encoding.ToLower() != "utf-8")
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    using StreamReader reader2 = new(memoryStream, System.Text.Encoding.GetEncoding(encoding));

                    page = reader2.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Program.log.Write("GetWebPage url:" + url + "error: " + ex.Message);
            }

            return page;
        }

        /// <summary>
        /// download file from url to file </summary>
        public static bool DownloadFile(string sourceUrl, string pathToSave)
        {
            try
            {
                using HttpClient client = new();

                HttpResponseMessage response = client.GetAsync(sourceUrl).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();
                byte[] fileBytes = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();

                File.WriteAllBytes(pathToSave, fileBytes);

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
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
        }
		
    }
}
