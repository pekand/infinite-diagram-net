using System;
using System.Collections.Generic;

namespace Diagram
{
    /// <summary>
    /// global program parmeters for all instances </summary>
    public class ProgramOptions //UID0014460148
    {
        /*************************************************************************************************************************/

        // NOT SYNCHRONIZED PARAMETERS

        /// <summary>
        /// license</summary>
        public String license = "GPLv3";

        /// <summary>
        /// author</summary>
        public String author = "Andrej Pekar";

        /// <summary>
        /// contact email</summary>
        public String email = "pekand@gmail.com";

        /// <summary>
        /// home page url</summary>
        public String home_page = "https://infinite-diagram.pekand.com";

        /// <summary>
        /// local server ip address fo messaging beetwen runing instances</summary>
        public String server_default_ip = "127.0.0.1";

        /*************************************************************************************************************************/

        // SYNCHRONIZED PARAMETERS

        /// <summary>
        /// proxy uri</summary>
        public String proxy_uri = "";

        /// <summary>
        /// proxy auth username</summary>
        public String proxy_username = "";

        /// <summary>
        /// proxy auth password</summary>
        public String proxy_password = "";

#if DEBUG
        /// <summary>
        /// debug local messaging server port</summary>
        public long server_default_port = 13001;
#else
        /// <summary>
        /// release local messaging server port</summary>
        public long server_default_port = 13000;
#endif


        /// <summary>
        /// command for open editor on line position</summary>
        public String texteditor = "subl \"%FILENAME%\":%LINE%";

        /// <summary>
        /// recently opened files</summary>
        public List<String> recentFiles = new List<String>();

        /// <summary>
        /// when application start as empty and this option is set then open last file</summary>
        public bool openLastFile = false;

        /// <summary>
        /// when application start as empty and this option is set then open default diagram if exist instead of empty file</summary>
        public string defaultDiagram = "";

        /// <summary>
        /// user signature (randomly generated token uniqe for user)</summary>
        public string signatureSecret = "";

        /// <summary>
        /// user signature iv - random vector inicializer</summary>
        public string signatureIV = null;

        /// <summary>
        /// skip version for update. Show update dialog when newer version is released.</summary>
        public string skipVersion = null;

        /*************************************************************************************************************************/
        // Recent files

        /// <summary>
        /// add path to recent files</summary>
        public void AddRecentFile(String path)
        {
            if (Os.FileExists(path))
            {
                this.recentFiles.Remove(path);
                this.recentFiles.Insert(0, path);
            }
        }

        /// <summary>
        /// remove old not existing diagrams from recent files</summary>
        public void RemoveOldRecentFiles()
        {
            List<String> newRecentFiles = new List<String>();

            foreach (String path in this.recentFiles)
            {
                if(Os.FileExists(path))
                {
                    newRecentFiles.Add(path);
                }
            }
            this.recentFiles = newRecentFiles;
        }
    }
}
