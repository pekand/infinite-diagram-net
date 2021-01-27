using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using Shell32;

namespace Diagram
{
    /// <summary>
    /// OS and path related functions repository</summary>
    public class Os //UID8599434163
    {

        /*************************************************************************************************************************/
        // ASSEMBLY

        /// <summary>
        /// get current app version</summary>
        public static string GetThisAssemblyVersion()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.FileVersion;
        }

        /// get current app executable path</summary>
        public static string GetThisAssemblyLocation()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().Location;
        }

        /*************************************************************************************************************************/
        // FILE EXTENSION

        /// <summary>
        /// get file extension</summary>
        public static string GetExtension(string file)
        {
            string ext = "";
            if (file != "" && Os.FileExists(file))
            {
                ext = Path.GetExtension(file).ToLower();
            }

            return ext;
        }

        /// <summary>
        /// check if diagramPath file path has good extension  </summary>
        public static bool IsDiagram(string diagramPath)
        {
            diagramPath = NormalizePath(diagramPath);
            if (Os.FileExists(diagramPath) && Path.GetExtension(diagramPath).ToLower() == ".diagram")
            {
                return true;
            }

            return false;
        }

        /*************************************************************************************************************************/
        // FILE OPERATIONS

        /// <summary>
        /// check if path is file</summary>
        public static bool IsFile(string path)
        {
            return Os.FileExists(path);
        }

        /// <summary>
        /// check if file exist independent on os </summary>
        public static bool FileExists(string path)
        {
            return File.Exists(NormalizePath(path));
        }

        /// <summary>
        /// check if directory or file exist independent on os </summary>
        public static bool Exists(string path)
        {
            return FileExists(path) || DirectoryExists(path);
        }

        /// <summary>
        /// get file name or directory name from path</summary>
        public static string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        /// <summary>
        /// get file name or directory name from path</summary>
        public static string GetFileNameWithoutExtension(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        /// <summary>
        /// get parent directory of FileName path </summary>
        public static string GetFileDirectory(string FileName)
        {
            if (FileName.Trim().Length > 0 && Os.FileExists(FileName))
            {
                return new FileInfo(FileName).Directory.FullName;
            }
            return null;
        }

        /*************************************************************************************************************************/
        // DIRECTORY OPERATIONS

        /// <summary>
        /// check if path is directory</summary>
        public static bool IsDirectory(string path)
        {
            return Os.DirectoryExists(path);
        }

        /// <summary>
        /// check if directory exist independent on os </summary>
        public static bool DirectoryExists(string path)
        {
            return Directory.Exists(NormalizePath(path));
        }

        /// <summary>
        /// get file name or directory name from path</summary>
        public static string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path);
        }

        /// <summary>
        /// set current directory</summary>
        public static void SetCurrentDirectory(string path)
        {
            Directory.SetCurrentDirectory(path);
        }

        /// <summary>
        /// create directory</summary>
        public static bool CreateDirectory(string path)
        {
            try
            {
                Directory.CreateDirectory(path);

                return true;
            }
            catch (Exception e)
            {
                Program.log.Write("os.createDirectory fail: " + path + ": " + e.ToString());
            }
            return false;
        }

        /// <summary>
        /// get current running application executable directory
        /// Example: c:\Program Files\Infinite Diagram\
        /// </summary> 
        public static String GetCurrentApplicationDirectory()
        {
            return Os.GetDirectoryName(Application.ExecutablePath);
        }

        /// <summary>
        /// get config file directory
        /// Example: C:\Users\user_name\AppData\Roaming\
        /// </summary> 
        public static string GetApplicationsDirectory()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }

        /// <summary>
        ///remove directory and content
        /// Example: C:\Users\user_name\AppData\Roaming\
        /// </summary> 
        public static void RemoveDirectory(string path)
        {
            if (Directory.Exists(path)) {
                Directory.Delete(path, true);
            }
        }

        /*************************************************************************************************************************/
        // PATH OPERATIONS

        /// <summary>
        /// get full path</summary>
        public static string GetFullPath(string path)
        {
            return Path.GetFullPath(path);
        }

        /// <summary>
        /// concat path and subdir</summary>
        public static string Combine(string path, string subdir)
        {
            return Path.Combine(path, subdir);
        }

        /// <summary>
        /// convert slash dependent on current os </summary>
        public static string NormalizePath(string path)
        {
            return path.Replace("/", "\\");
        }

        /// <summary>
        /// normalize path and get full path from relative path </summary>
        public static string NormalizedFullPath(string path)
        {
            return Path.GetFullPath(NormalizePath(path));
        }

        /// <summary>
        /// meke filePath relative to currentPath. 
        /// If is set inCurrentDir path is converted to relative only 
        /// if currentPath is parent of filePath</summary>
        public static string MakeRelative(string filePath, string currentPath, bool inCurrentDir = true)
        {
            filePath = filePath.Trim();
            currentPath = currentPath.Trim();

            if (currentPath == "")
            {
                return filePath;
            }

            if (!Os.FileExists(filePath) && !Os.DirectoryExists(filePath))
            {
                return filePath;
            }

            filePath = Os.GetFullPath(filePath);

            if (Os.FileExists(currentPath))
            {
                currentPath = Os.GetDirectoryName(currentPath);
            }

            if (!Os.DirectoryExists(currentPath))
            {
                return filePath;
            }

            currentPath = Os.GetFullPath(currentPath);

            Uri pathUri = new Uri(filePath);
            // Folders must end in a slash
            if (!currentPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                currentPath += Path.DirectorySeparatorChar;
            }

            int pos = filePath.ToLower().IndexOf(currentPath.ToLower());
            if (inCurrentDir && pos != 0) // skip files outside of currentPath
            {
                return filePath;
            }

            Uri folderUri = new Uri(currentPath);
            return Uri.UnescapeDataString(
                folderUri.MakeRelativeUri(pathUri)
                .ToString()
                .Replace('/', Path.DirectorySeparatorChar)
            );
        }

        public static long FileSize(string path)
        {
            if (!FileExists(path))
            {
                return 0;
            }

            return new System.IO.FileInfo(path).Length;
        }

        public static long DirectorySize(string path, int level = 100)
        {
            if (level == 0)
            {
                return 0;
            }

            if (!DirectoryExists(path))
            {
                return 0;
            }

            DirectoryInfo d = new DirectoryInfo(path);

            long size = 0;
            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                size += fi.Length;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                size += DirectorySize(di.FullName, level--);
            }
            return size;
        }

        /*************************************************************************************************************************/
        // SHORTCUTS

        /// <summary>
        /// get path from lnk file in windows  </summary>
        public static string[] GetShortcutTargetFile(string shortcutFilename)
        {
            try
            {
                string pathOnly = Os.GetDirectoryName(shortcutFilename);
                string filenameOnly = Os.GetFileName(shortcutFilename);

                Shell shell = new Shell();
                Folder folder = shell.NameSpace(pathOnly);
                FolderItem folderItem = folder.ParseName(filenameOnly);
                if (folderItem != null)
                {
                    Shell32.ShellLinkObject link = (Shell32.ShellLinkObject)folderItem.GetLink;

                    if (link.Arguments != "") {
                        return new string[] { link.Path, link.Arguments };
                    }

                    return new string[] { link.Path, "" };
                }
            }
            catch (Exception e)
            {
                Program.log.Write("GetShortcutTargetFile error: " + e.Message);
            }

            return new string[] { "", "" };
        }

        /// <summary>
        ///get icon from lnk file in windows  </summary>
        public static string GetShortcutIcon(String shortcutFilename)
        {
            String pathOnly = Os.GetDirectoryName(shortcutFilename);
            String filenameOnly = Os.GetFileName(shortcutFilename);

            Shell shell = new Shell();
            Folder folder = shell.NameSpace(pathOnly);
            FolderItem folderItem = folder.ParseName(filenameOnly);
            if (folderItem != null)
            {
                Shell32.ShellLinkObject link = (Shell32.ShellLinkObject)folderItem.GetLink;
                link.GetIconLocation(out String iconlocation);
                return iconlocation;
            }

            return String.Empty;
        }

        /*************************************************************************************************************************/
        // OPEN

        /// <summary>
        /// run application in current os </summary>
        public static void RunProcess(string path)
        {
            path = NormalizePath(path);            
            System.Diagnostics.Process.Start("explorer.exe", path);
        }

        /// <summary>
        /// open path in explorer in current os </summary>
        public static void OpenDirectory(string path)
        {
            path = NormalizePath(path);
            System.Diagnostics.Process.Start("explorer.exe", path);
        }

        /// <summary>
        /// open path in system if exist  </summary>
        public static void OpenPathInSystem(string path)
        {
            if (Os.FileExists(path))       // OPEN FILE
            {
                try
                {
                    string parent_diectory = Os.GetFileDirectory(path);
                    System.Diagnostics.Process.Start(parent_diectory);
                }
                catch (Exception ex) { Program.log.Write("openPathInSystem open file: error:" + ex.Message); }
            }
            else if (Os.DirectoryExists(path))  // OPEN DIRECTORY
            {
                try
                {
                    System.Diagnostics.Process.Start(path);
                }
                catch (Exception ex) { Program.log.Write("openPathInSystem open directory: error:"+ex.Message); }
            }
        }

        /// <summary>
        /// open diagram file in current runing application with system call command </summary>
        public static void OpenDiagram(string diagramPath)
        {
            try
            {

                string currentApp = System.Reflection.Assembly.GetExecutingAssembly().Location;
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = currentApp,
                    Arguments = "\"" + Escape(diagramPath) + "\""
                };
                Program.log.Write("diagram: openlink: open diagram: " + currentApp + "\"" + Escape(diagramPath) + "\"");
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                Program.log.Write(ex.Message);
            }
        }

        /// <summary>
        /// open directory in system</summary>
        public static void ShowDirectoryInExternalApplication(string path)
        {
            try
            {
                Process.Start("explorer.exe", path);
            }
            catch (Exception ex)
            {
                Program.log.Write("open directory: " + path + ": error: " + ex.Message);
            }
        }

        /// <summary>
        /// run command in system and wait for output </summary>
        public static void RunCommand(string cmd, string workdir = null) //b39d265706
        {
            try
            {

                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                string[] parts = Patterns.SplitCommand(cmd);
                startInfo.FileName = parts[0];
                startInfo.Arguments = parts[1];

                startInfo.WorkingDirectory = workdir;
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                process.StartInfo = startInfo;
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();
                Program.log.Write("output: " + output);
                Program.log.Write("error: " + error);
            }
            catch (Exception ex)
            {
                Program.log.Write("exception: " + ex.Message);
            }
        }

        /// <summary>
        /// run command in system and discard output </summary>
        public static void RunCommandAndExit(string cmd, string parameters = "")
        {

            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
            };

            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C " + "\"" + cmd + ((parameters!="")?" "+ parameters:"") + "\"";

            process.StartInfo = startInfo;
            process.Start();
        }

        /// <summary>
        /// open file on position </summary>
        public static void OpenFileOnPosition(string fileName, long pos = 0)
        {

            String editFileCmd = "subl \"%FILENAME%\":%LINE%"; ;
            editFileCmd = editFileCmd.Replace("%FILENAME%", Os.NormalizedFullPath(fileName));
            editFileCmd = editFileCmd.Replace("%LINE%", pos.ToString());

            Program.log.Write("diagram: openlink: open file on position " + editFileCmd);
            Os.RunCommand(editFileCmd);
        }
        
        /// <summary>
        /// open email </summary>
        public static void OpenEmail(string email)
        {
            Program.log.Write("open email: " + email);
            
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = "mailto:"+email;
            proc.Start();
        }


        public delegate void CopyProgressDelegate(long count);

        public static void CopyByBlock(string inputPath, string outputPath, CopyProgressDelegate callback = null)
        {
            using (FileStream input = File.Open(inputPath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using (FileStream output = File.Open(outputPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
                {
                    byte[] buffer = new byte[1024 * 1024];
                    int bytesRead;
                    while ((bytesRead = input.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        output.Write(buffer, 0, bytesRead);
                        callback?.Invoke(bytesRead);
                    }
                }
            }
        }

        /// <summary>
        /// copy file or directory </summary>
        public static bool Copy(string SourcePath, string DestinationPath , CopyProgressDelegate callback = null)
        {
            try
            {
                if (Directory.Exists(SourcePath))
                {
                    foreach (string dirPath in Directory.GetDirectories(SourcePath, "*", SearchOption.AllDirectories))
                        Os.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));

                    foreach (string newPath in Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories))
                        CopyByBlock(newPath, newPath.Replace(SourcePath, DestinationPath), callback);
                }
                else if (File.Exists(SourcePath))
                {
                    CopyByBlock(SourcePath, Os.Combine(DestinationPath, Os.GetFileName(SourcePath)), callback);
                }
                return true;
            }
            catch (Exception ex)
            {
                Program.log.Write(ex.Message);
                return false;
            }
        }
        /*************************************************************************************************************************/
        // TOOLS

        /// <summary>
        /// add slash before slash and quote </summary>
        public static string Escape(string text)
        {
            return text.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        /// <summary>
        /// convert win path slash to linux type slash </summary>
        public static string ToBackslash(string text)
        {
            return text.Replace("\\", "/");
        }

        /// <summary>
        /// get path separator dependent on os </summary>
        public static string GetSeparator()
        {
            return Path.DirectorySeparatorChar.ToString();
        }

        /*************************************************************************************************************************/
        // SEARCH IN DIRECTORY

        /// <summary>
        /// Scans a folder and all of its subfolders recursively, and updates the List of files
        /// </summary>
        /// <param name="path">Full path for scened directory</param>
        /// <param name="files">out - file list</param>
        /// <param name="directories">out - directories list</param>
        public static void Search(string path, List<string> files, List<string> directories)
        {
            try
            {
                foreach (string f in Directory.GetFiles(path))
                {
                    files.Add(f);
                }

                foreach (string d in Directory.GetDirectories(path))
                {
                    directories.Add(d);
                    Search(d, files, directories);
                }

            }
            catch (System.Exception ex)
            {
                Program.log.Write(ex.Message);
            }
        }

        /*************************************************************************************************************************/
        // SEARCH IN FILE

        /// <summary>
        /// find line number with first search string occurrence </summary>
        public static int FndLineNumber(string fileName, string search)
        {
            int pos = 0;
            string line;
            using (StreamReader file = new StreamReader(fileName))
            {
                while ((line = file.ReadLine()) != null)
                {
                    pos++;
                    if (line.Contains(search))
                    {
                        return pos;
                    }
                }
            }

            return pos;
        }

        /*************************************************************************************************************************/
        // TEMP

        /// <summary>
        /// get temporary directory</summary>
        public static string GetTempPath()
        {
            return Path.GetTempPath();
        }

        /*************************************************************************************************************************/
        // WRITE AND READ FILE OPERATIONS

        /// <summary>
        /// create empty file</summary>
        public static void CreateEmptyFile(string path)
        {
            File.Create(path).Dispose();
        }

        /// <summary>
        /// write string content to file</summary>
        public static void WriteAllText(string path, string data)
        {
            File.WriteAllText(path, data);
        }

        /// <summary>
        /// write string content to file</summary>
        public static string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        /// <summary>
        /// get file content as string</summary>
        public static string GetFileContent(string file)
        {
            try
            {
                using (StreamReader streamReader = new StreamReader(file, Encoding.UTF8))
                {
                    return streamReader.ReadToEnd();
                }
            }
            catch (System.IO.IOException ex)
            {
                Program.log.Write(ex.Message);
            }

            return null;
        }

        /// <summary>
        /// write string content to file</summary>
        public static void WriteAllBytes(string path, byte[] data)
        {
            File.WriteAllBytes(path, data);
        }

        /// <summary>
        /// write string content to file</summary>
        public static byte[] ReadAllBytes(string path)
        {
            return File.ReadAllBytes(path);
        }

        /*************************************************************************************************************************/
        // CLIPBOARD

        /// <summary>
        /// get string from clipboard </summary>
        public static string GetTextFormClipboard()
        {
            DataObject retrievedData = (DataObject)Clipboard.GetDataObject();
            string clipboard = "";
            if (retrievedData != null && retrievedData.GetDataPresent(DataFormats.Text))  // [PASTE] [TEXT] insert text
            {
                clipboard = retrievedData.GetData(DataFormats.Text) as string;
            }

            return clipboard;
        }

    }
}
