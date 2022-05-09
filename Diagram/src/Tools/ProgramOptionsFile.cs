using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Diagram
{
    /// <summary>    
    /// </summary>
    public class ProgramOptionsFile //UID8285897740
    {
        /*************************************************************************************************************************/
        public String configFileDirectory = "InfiniteDiagram"; // name of directory for save global configuration file

#if DEBUG
        // global configuration file name in debug mode
        public String configFileName = "diagram.debug.xml";
#else
        // global configuration file name
        public String configFileName = "diagram.xml";
#endif

        // Example: C:\Users\user_name\AppData\Roaming\Diagram\diagram.json
        public String optionsFilePath = ""; // full path to global options json file

        public ProgramOptions programOptions = null;

        /*************************************************************************************************************************/

        /// <summary>
        /// load global config file from portable file configuration or global file configuration
        /// </summary>
        /// <param name="parameters">reference to parameter object</param>
        public ProgramOptionsFile(ProgramOptions programOptions)
        {
            this.programOptions = programOptions;

            // use local config file
            this.optionsFilePath = Os.Combine(Os.GetCurrentApplicationDirectory(), this.configFileName);

            // use global config file if local version not exist
            if (!Os.FileExists(this.optionsFilePath))
            {
                this.optionsFilePath = Os.Combine(
                    this.GetGlobalConfigDirectory(),
                    this.configFileName
                );
            }

            // open config file if exist
            if (Os.FileExists(this.optionsFilePath))
            {
                this.LoadConfigFile();
            }
            else
            {
                string globalConfigDirectory = Os.Combine(
                    Os.GetApplicationsDirectory(),
                    this.configFileDirectory
                );

                // create global config directory if not exist
                if (!Os.DirectoryExists(globalConfigDirectory))
                {
                    Os.CreateDirectory(globalConfigDirectory);
                }

                // if config file dosn't exist create one with default values
                this.SaveConfigFile();
            }
        }

        /// <summary>
        /// load global config file from json file</summary>
        private void LoadConfigFile()
        {
            try
            {
                Program.log.Write("loadConfigFile: path:" + this.optionsFilePath);

                if (Os.FileExists(this.optionsFilePath))
                {

                    string xml = Os.GetFileContent(this.optionsFilePath);

                    XmlReaderSettings xws = new XmlReaderSettings
                    {
                        CheckCharacters = false
                    };


                    using (XmlReader xr = XmlReader.Create(new StringReader(xml), xws))
                    {
                        XElement root = XElement.Load(xr);

                        this.LoadParams(root);
                    }

                }

            }
            catch (Exception ex)
            {
                Program.log.Write("loadConfigFile: " + ex.Message);
            }

        }

        /// <summary>
        /// load configuration</summary>
        public void LoadParams(XElement root)
        {
            
            foreach (XElement option in root.Elements())
            {
                if (option.Name.ToString() == "proxy_uri")
                {
                    this.programOptions.proxy_uri = option.Value;
                }

                if (option.Name.ToString() == "proxy_username")
                {
                    this.programOptions.proxy_username = option.Value;
                }

                if (option.Name.ToString() == "proxy_password")
                {
                    this.programOptions.proxy_password = option.Value;
                }

                if (option.Name.ToString() == "server_default_port")
                {
                    this.programOptions.server_default_port = Converter.ToInt(option.Value);
                }

                if (option.Name.ToString() == "texteditor")
                {
                    this.programOptions.texteditor = option.Value;
                }

                if (option.Name.ToString() == "openLastFile")
                {
                    this.programOptions.openLastFile = (option.Value == "1");
                }

                if (option.Name.ToString() == "defaultDiagram")
                {
                    this.programOptions.defaultDiagram = option.Value;
                }

                if (option.Name.ToString() == "signatureSecret")
                {
                    this.programOptions.signatureSecret = option.Value;
                }

                if (option.Name.ToString() == "signatureIV")
                {
                    this.programOptions.signatureIV = option.Value;
                }

                if (option.Name.ToString() == "recentFiles" && option.HasElements)
                {
                    this.programOptions.recentFiles.Clear();

                    foreach (XElement recentFileOption in option.Elements())
                    {
                        if (recentFileOption.Name.ToString() == "item")
                        {
                            this.programOptions.recentFiles.Add(recentFileOption.Value);
                        }
                    }
                }

                if (option.Name.ToString() == "skipVersion")
                {
                    this.programOptions.skipVersion = option.Value;
                }

                if (option.Name.ToString() == "data")
                {
                    this.programOptions.dataStorage.fromXml(option);
                }
            }

            if (this.programOptions.signatureSecret == null || this.programOptions.signatureSecret.Trim() == "") {
                this.programOptions.signatureSecret = Signature.GenerateSignatureSecret();
            }

            if (this.programOptions.signatureIV == null || this.programOptions.signatureIV.Trim() == "")
            {
                this.programOptions.signatureIV = Signature.GenerateIV();
            }

            this.programOptions.RemoveOldRecentFiles();
        }

        /// <summary>
        /// save global config file as json</summary>
        public void SaveConfigFile()
        {
            try
            {
                XElement root = this.SaveParams();
                
                System.IO.StreamWriter file = new System.IO.StreamWriter(this.optionsFilePath);

                string xml = "";

                StringBuilder sb = new StringBuilder();
                XmlWriterSettings xws = new XmlWriterSettings
                {
                    OmitXmlDeclaration = true,
                    CheckCharacters = false,
                    Indent = true
                };

                using (XmlWriter xw = XmlWriter.Create(sb, xws))
                {
                    root.WriteTo(xw);
                }

                xml = sb.ToString();

                file.Write(xml);
                file.Close();


            }
            catch (Exception ex)
            {
                Program.log.Write("saveConfigFile: " + ex.Message);
            }
        }

        /// <summary>
        /// save configuration </summary>
        public XElement SaveParams()
        {
            XElement root = new XElement("configuration");

            root.Add(new XElement("proxy_uri", this.programOptions.proxy_uri));
            root.Add(new XElement("proxy_username", this.programOptions.proxy_username));
            root.Add(new XElement("proxy_password", this.programOptions.proxy_password));
            root.Add(new XElement("server_default_port", this.programOptions.server_default_port.ToString()));
            root.Add(new XElement("texteditor", this.programOptions.texteditor));
            root.Add(new XElement("openLastFile", this.programOptions.openLastFile ? "1" : "0"));
            root.Add(new XElement("defaultDiagram", this.programOptions.defaultDiagram));
            root.Add(new XElement("signatureSecret", this.programOptions.signatureSecret));
            root.Add(new XElement("signatureIV", this.programOptions.signatureIV));
            root.Add(new XElement("skipVersion", this.programOptions.skipVersion));

            programOptions.RemoveOldRecentFiles();

            XElement recentFilesNode = new XElement("recentFiles");

            foreach (String path in this.programOptions.recentFiles)
            {
                recentFilesNode.Add(new XElement("item", path));
            }

            root.Add(recentFilesNode);

            XElement dataStorageElement = new XElement("data");

            this.programOptions.dataStorage.toXml(dataStorageElement);

            root.Add(dataStorageElement);

            return root;
        }

        /*************************************************************************************************************************/

        /// <summary>
        /// open directory with configuration file</summary> 
        public string  GetGlobalConfigDirectory()
        {
            return Os.Combine(
                Os.GetApplicationsDirectory(),
                this.configFileDirectory
            );
        }
    }
}
