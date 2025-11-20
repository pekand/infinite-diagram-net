using Shell32;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

#nullable disable

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
            if (Os.FileExists(diagramPath) && Path.GetExtension(diagramPath).Equals(".diagram", StringComparison.CurrentCultureIgnoreCase))
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

        /// <summary>
        /// compare files by file content </summary>
        public static bool AreFilesSame(string file1, string file2)
        {
            var f1 = new FileInfo(file1);
            var f2 = new FileInfo(file2);

            if (!f1.Exists || !f2.Exists)
                return false;

            if (f1.Length != f2.Length)
                return false;

            using var sha = SHA256.Create();

            using var s1 = File.OpenRead(file1);
            using var s2 = File.OpenRead(file2);

            byte[] h1 = sha.ComputeHash(s1);
            byte[] h2 = sha.ComputeHash(s2);

            // 3) Compare hashes
            return h1.SequenceEqual(h2);
        }

        /// <summary>
        /// get next available file name in destination </summary>
        public static string GetNextAvailableFilePath(string desiredPath)
        {
            if (!File.Exists(desiredPath)) return desiredPath;

            var dir = Path.GetDirectoryName(desiredPath) ?? "";
            var ext = Path.GetExtension(desiredPath);
            var name = Path.GetFileNameWithoutExtension(desiredPath);

            int i = 1;
            string candidate;
            do
            {
                candidate = Path.Combine(dir, $"{name}.{i}{ext}");
                i++;
            } while (File.Exists(candidate));

            return candidate;
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
        public static string Combine(string path, string subpath)
        {
            return Path.Combine(path, subpath ?? "");
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

            Uri pathUri = new(filePath);
            // Folders must end in a slash
            if (!currentPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                currentPath += Path.DirectorySeparatorChar;
            }

            int pos = filePath.IndexOf(currentPath, StringComparison.CurrentCultureIgnoreCase);
            if (inCurrentDir && pos != 0) // skip files outside of currentPath
            {
                return filePath;
            }

            Uri folderUri = new(currentPath);
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

            DirectoryInfo d = new(path);

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

                Shell shell = new();
                Folder folder = shell.NameSpace(pathOnly);
                FolderItem folderItem = folder.ParseName(filenameOnly);
                if (folderItem != null)
                {
                    Shell32.ShellLinkObject link = (Shell32.ShellLinkObject)folderItem.GetLink;

                    if (link.Arguments != "") {
                        return [link.Path, link.Arguments];
                    }

                    return [link.Path, ""];
                }
            }
            catch (Exception e)
            {
                Program.log.Write("GetShortcutTargetFile error: " + e.Message);
            }

            return ["", ""];
        }

        /// <summary>
        ///get icon from lnk file in windows  </summary>
        public static string GetShortcutIcon(String shortcutFilename)
        {
            String pathOnly = Os.GetDirectoryName(shortcutFilename);
            String filenameOnly = Os.GetFileName(shortcutFilename);

            Shell shell = new();
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
        public static void OpenFileInExplorer(string path)
        {
            try
            {
                path = NormalizePath(path);

                if (!Os.FileExists(path))
                {
                    return;

                }
                string workingDirectory = Path.GetDirectoryName(path);
                string fileName = Path.GetFileName(path);

                ProcessStartInfo startInfo = new()
                {
                    FileName = ("explorer.exe"),
                    Arguments = fileName,
                    WorkingDirectory = workingDirectory
                };
                Process process = new()
                {
                    StartInfo = startInfo
                };
                process.Start();
            }
            catch (Exception ex)
            {
                Program.log.Write("RunProcess open directory: error:" + ex.Message);
            }
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
                ProcessStartInfo startInfo = new()
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

        static string GetCommandPath(string command)
        {
            string commandPath = null;

            string pathVariable = Environment.GetEnvironmentVariable("PATH");
            if (pathVariable != null)
            {
                string[] paths = pathVariable.Split(';'); // Use ':' on Unix-based systems
                foreach (string path in paths)
                {
                    if (path != null && path.Trim() != ""){
                        string combinedParh = Path.Combine(Path.GetFullPath(path), command);
                        if (Os.FileExists(combinedParh)) {
                            commandPath = combinedParh;
                            break;
                        }
                    }
                }
            }

            return commandPath;
        }

        /// <summary>
        /// run command in system and wait for output </summary>
        public static void RunCommand(string cmd, string workdir = null) //b39d265706
        {
            Job.DoJob(
                new DoWorkEventHandler(
                    delegate (object o, DoWorkEventArgs args)
                    {
                        try
                        {

                            Process process = new();
                            ProcessStartInfo startInfo = new()
                            {
                                WindowStyle = ProcessWindowStyle.Hidden,
                                FileName = "cmd.exe",
                                Arguments = "/C" + cmd,

                                WorkingDirectory = workdir,
                                UseShellExecute = true,
                                RedirectStandardOutput = false,
                                RedirectStandardError = false,
                                CreateNoWindow = false
                            };
                            process.StartInfo = startInfo;
                            process.Start();
                            //string output = process.StandardOutput.ReadToEnd();
                            //string error = process.StandardError.ReadToEnd();
                            //process.WaitForExit();
                            //Program.log.Write("output: " + output);
                            //Program.log.Write("error: " + error);
                        }
                        catch (Exception ex)
                        {
                            Program.log.Write("exception: " + ex.Message);
                        }

                    }
                ),
                new RunWorkerCompletedEventHandler(
                    delegate (object o, RunWorkerCompletedEventArgs args)
                    {
                        
                    }
                )
            );     
        }

        /// <summary>
        /// run command in system and wait for output </summary>
        public static void RunSilentCommand(string cmd, string workdir = null) //b39d265706
        {
            Job.DoJob(
                new DoWorkEventHandler(
                    delegate (object o, DoWorkEventArgs args)
                    {
                        try
                        {

                            Process process = new();
                            ProcessStartInfo startInfo = new()
                            {
                                WindowStyle = ProcessWindowStyle.Hidden,
                                FileName = "cmd.exe",
                                Arguments = "/C" + cmd,

                                WorkingDirectory = workdir,
                                UseShellExecute = true,
                                RedirectStandardOutput = false,
                                RedirectStandardError = false,
                                CreateNoWindow = false
                            };
                            process.StartInfo = startInfo;
                            process.Start();
                            /*string output = process.StandardOutput.ReadToEnd();
                            string error = process.StandardError.ReadToEnd();
                            process.WaitForExit();
                            Program.log.Write("output: " + output);
                            Program.log.Write("error: " + error);*/
                        }
                        catch (Exception ex)
                        {
                            Program.log.Write("exception: " + ex.Message);
                        }

                    }
                ),
                new RunWorkerCompletedEventHandler(
                    delegate (object o, RunWorkerCompletedEventArgs args)
                    {

                    }
                )
            );
        }

        /// <summary>
        /// run command in system and discard output </summary>
        public static void RunCommandAndExit(string cmd, string parameters = "")
        {

            Process process = new();
            ProcessStartInfo startInfo = new()
            {
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                Arguments = "/C " + "\"" + cmd + ((parameters != "") ? " " + parameters : "") + "\""
            };

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
            Os.RunSilentCommand(editFileCmd);
        }
        
        /// <summary>
        /// open email </summary>
        public static void OpenEmail(string email)
        {
            Program.log.Write("open email: " + email);

            Process proc = new();
            proc.StartInfo.FileName = "mailto:"+email;
            proc.Start();
        }


        public delegate void CopyProgressDelegate(long count);

        public static void CopyByBlock(string inputPath, string outputPath, CopyProgressDelegate callback = null)
        {
            using FileStream input = File.Open(inputPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using FileStream output = File.Open(outputPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);

            byte[] buffer = new byte[1024 * 1024];
            int bytesRead;
            while ((bytesRead = input.Read(buffer, 0, buffer.Length)) != 0)
            {
                output.Write(buffer, 0, bytesRead);
                callback?.Invoke(bytesRead);
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
                else if (Os.FileExists(SourcePath))
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

        /// <summary>
        /// copy file to other file in destination, not overwrite file in destination</summary>
        public static void CopyFile(string sourcePath, string destPath)
        {
            var destDir = Path.GetDirectoryName(destPath);
            if (!Os.DirectoryExists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            File.Copy(sourcePath, destPath, false);
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
            using (StreamReader file = new(fileName))
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
                using StreamReader streamReader = new(file, Encoding.UTF8);

                return streamReader.ReadToEnd();
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

        public static async Task RunCommandWithTimeout(string cmd, string workdir = null)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));

            try
            {
                await RunProcessWithTimeoutAsync(cmd, workdir, cts.Token);
            }
            catch (OperationCanceledException)
            {
                Program.log.Write("Process was canceled due to timeout.");
            }
            catch (Exception ex)
            {
                Program.log.Write($"Process failed: {ex.Message}");
            }
        }

        public static async Task RunProcessWithTimeoutAsync(string cmd, string workdir, CancellationToken token)
        {
            var tcs = new TaskCompletionSource<bool>();

            Process process = new()
            {
                EnableRaisingEvents = true
            };
            process.Exited += (sender, e) => tcs.TrySetResult(true);
            ProcessStartInfo startInfo = new()
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                Arguments = "/C" + cmd,

                WorkingDirectory = workdir,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            process.StartInfo = startInfo;

            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {

                    Program.context.Post(_ => Program.log.Write(e.Data), null);
                }
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Program.context.Post(_ => Program.log.Write(e.Data), null);
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(Timeout.Infinite, token));

            if (completedTask == tcs.Task)
            {
                // Process finished within timeout
                await tcs.Task;
            }
            else
            {
                // Timeout or cancellation occurred
                process.Kill();
                throw new OperationCanceledException("Process timed out.");
            }
        }

    }
}
