using System.IO.Compression;
using System.Text;
using System.Xml;
using System.Xml.Linq;

#nullable disable

namespace Diagram
{
    /// <summary>
    /// directory structure for zip file in directory to string</summary>
    public class EDirectory
    {
        public string name = "";
    }

    /// <summary>
    /// file structure for zip file in directory to string</summary>
    public class EFile
    {
        public string name = "";
        public string data = "";
    }

    /// <summary>
    /// repository for compression related functions</summary>
    public class Compress //UID4089400971
    {
        /*************************************************************************************************************************/
        // ZIP STRING

        /// <summary>
        /// use string as stringstream</summary>
        public static void StringToStream(string s, MemoryStream stream)
        {
            StreamWriter writer = new(stream);
            writer.Write(s);
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);
        }

        /// <summary>
        /// use stringstream to string</summary>
        public static string StreamToString(MemoryStream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);

            using StreamReader reader = new(stream);

            return reader.ReadToEnd();
        }

        /// <summary>
        /// gZip utf8 string to base64</summary>
        public static string Zip(string str)
        {
            MemoryStream stream = new();
            StringToStream(str, stream);
            return ZipStream(stream);            
        }

        /// <summary>
        /// gUnzip base64 strng to utf8</summary>
        public static string Unzip(string str)
        {
            MemoryStream stream = new(); ;
            UnzipStream(str, stream);
            return StreamToString(stream);
        }

        /// <summary>
        /// zip stream to base64 string</summary>
        public static string ZipStream(MemoryStream input)
        {
            input.Seek(0, SeekOrigin.Begin);

            using var gzipOutput = new MemoryStream();

            using (var gzip = new GZipStream(gzipOutput, CompressionMode.Compress))
            {
                input.CopyTo(gzip);
            }


            return Convert.ToBase64String(gzipOutput.ToArray());
        }

        /// <summary>
        /// base64 string unzip</summary>
        public static void UnzipStream(string str, MemoryStream output)
        {
            byte[] bytes = Convert.FromBase64String(str);

            using MemoryStream gzipInput = new(bytes);
            using GZipStream gzip = new(gzipInput, CompressionMode.Decompress);

            gzip.CopyTo(output);
        }

        /*************************************************************************************************************************/
        // COMPRESS DIRECTORY

        /// <summary>
        /// compress directory with files to string</summary>
        public static string CompressPath(string path)
        {
            if (!Os.Exists(path)) {
                return "";
            }

            path = Os.NormalizedFullPath(path);

            List<EDirectory> directories = [];
            List<EFile> files = [];

            if (Os.IsFile(path)) {
                EFile eFile = new()
                {
                    name = Os.GetFileName(path),
                    data = Convert.ToBase64String(
                    Os.ReadAllBytes(path)
                )
                };
                files.Add(eFile);
            }

            if (Os.IsDirectory(path))
            {
                List<string> filePaths = [];
                List<string> directoryPaths = [];

                Os.Search(path, filePaths, directoryPaths);

                int pathLength = path.Length + 1;

                foreach (string dirPath in directoryPaths)
                {
                    EDirectory eDirectory = new()
                    {
                        name = dirPath[pathLength..]
                    };
                    directories.Add(eDirectory);
                }

                foreach (string filePath in filePaths)
                {
                    EFile eFile = new()
                    {
                        name = filePath[pathLength..],
                        data = Convert.ToBase64String(
                        File.ReadAllBytes(filePath)
                    )
                    };
                    files.Add(eFile);
                }
            }

            XElement xRoot = new("archive");

            XElement xDirectories = new("directories");

            foreach (EDirectory directory in directories)
            {
                XElement xDirectory = new("directory");

                xDirectory.Add(
                    new XElement(
                        "name",
                        directory.name
                    )
                );

                xDirectories.Add(xDirectory);
            }

            xRoot.Add(xDirectories);

            XElement xFiles = new("files");

            foreach (EFile file in files) {
                XElement xFile = new("file");

                xFile.Add(
                    new XElement(
                        "name",
                        file.name
                    )
                );

                xFile.Add(
                    new XElement(
                        "data",
                        file.data
                    )
                );
                xFiles.Add(xFile);
            }

            xRoot.Add(xFiles);

            StringBuilder sb = new();
            XmlWriterSettings xws = new()
            {
                OmitXmlDeclaration = true,
                CheckCharacters = false,
                Indent = true
            };

            using (XmlWriter xw = XmlWriter.Create(sb, xws))
            {
                xRoot.WriteTo(xw);
            }

            return Zip(sb.ToString());
        }

        /// <summary>
        /// decompress string with directory structure to path</summary>
        public static void DecompressPath(string compressedData, string destinationPath)
        {
            if (!Os.DirectoryExists(destinationPath))
            {
                return;
            }

            destinationPath = Os.NormalizedFullPath(destinationPath);

            string xml = Unzip(compressedData);

            XmlReaderSettings xws = new()
            {
                CheckCharacters = false
            };

            List<EDirectory> directories = [];
            List<EFile> files = [];

            try
            {
                using XmlReader xr = XmlReader.Create(new StringReader(xml), xws);

                XElement xRoot = XElement.Load(xr);
                if (xRoot.Name.ToString() == "archive")
                {
                    foreach (XElement xEl in xRoot.Elements())
                    {
                        if (xEl.Name.ToString() == "directories")
                        {
                            foreach (XElement xDirectory in xEl.Descendants())
                            {
                                if (xDirectory.Name.ToString() == "directory")
                                {

                                    string name = "";

                                    foreach (XElement xData in xDirectory.Descendants())
                                    {
                                        if (xData.Name.ToString() == "name")
                                        {
                                            name = xData.Value;
                                        }
                                    }

                                    if (name.Trim() != "")
                                    {
                                        EDirectory eDirectory = new()
                                        {
                                            name = name
                                        };
                                        directories.Add(eDirectory);
                                    }
                                }
                            }
                        }

                        if (xEl.Name.ToString() == "files")
                        {
                            foreach (XElement xFile in xEl.Descendants())
                            {
                                if (xFile.Name.ToString() == "file")
                                {
                                    string name = "";
                                    string data = "";

                                    foreach (XElement xData in xFile.Descendants())
                                    {
                                        if (xData.Name.ToString() == "name")
                                        {
                                            name = xData.Value;
                                        }

                                        if (xData.Name.ToString() == "data")
                                        {
                                            data = xData.Value;
                                        }
                                    }

                                    if (name.Trim() != "" && data.Trim() != "")
                                    {
                                        EFile eFile = new()
                                        {
                                            name = name,
                                            data = data
                                        };
                                        files.Add(eFile);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Program.log.Write("decompress file xml error: " + ex.Message);
            }

            foreach (EDirectory directory in directories)
            {
                string newDirPath = Os.Combine(destinationPath, directory.name);
                if (!Os.Exists(newDirPath))
                {
                    Os.CreateDirectory(newDirPath);
                }
            }

            foreach (EFile file in files)
            {
                string newFilePath = Os.Combine(destinationPath, file.name);
                if (!Os.Exists(newFilePath)) {
                    Os.WriteAllBytes(
                        newFilePath,
                        Convert.FromBase64String(
                            file.data
                        )
                    );
                }
            }

            // process dirrectories create to path

            // process files create to path 
        }
    }
}
