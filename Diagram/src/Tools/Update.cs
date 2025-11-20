using System.ComponentModel;

#nullable disable

namespace Diagram
{
    class Update
    {

        private const string updateFolderName = "InfiniteDiagramUpdate";
        private const string updateExecutableName = "infinite-diagram-install.exe";
        private const string homepage = "https://infinite-diagram.pekand.com/";
        private const string lastVersionFile = "lastversion.txt";
        private const string signatureFile = "signature.txt";
        private const string installationUrl = "https://github.com/pekand/infinite-diagram-core/releases/download/v{VERSION}/infinite-diagram-install.exe";

        public void CheckUpdates(bool showCurrentVersionStatus = false) {
            Job.DoJob(
               new DoWorkEventHandler(
                   delegate (object o, DoWorkEventArgs args)
                   {
                       try
                       {
                           string currentVersion = Os.GetThisAssemblyVersion();
                           Program.log.Write("CheckUpdates current version: " + currentVersion);

                           string lastVersion = Network.GetWebPage(homepage + lastVersionFile);
                           
                           Program.log.Write("CheckUpdates last version: " + lastVersion);

                           if (lastVersion == null) {
                               return;
                           }

                           lastVersion = lastVersion.TrimEnd('\r', '\n').Trim();


                           Version localVersion = new(currentVersion);
                           Version serverVersion = new(lastVersion);

                           if (serverVersion.CompareTo(localVersion) == 1)
                           {

                               string signature = Network.GetWebPage(homepage + signatureFile);
                               signature = signature.TrimEnd('\r', '\n').Trim();

                               if (signature == null || signature.Length < 64)
                               {
                                   return;
                               }

                               UpdateForm updateForm = new();
                               updateForm.ShowDialog();

                               if (updateForm.CanUpdate())
                               {
                                   string tempPath = Os.Combine(Os.GetTempPath(), updateFolderName);
                                   string executablePath = tempPath + Os.GetSeparator() + updateExecutableName;
                                   Os.RemoveDirectory(tempPath);
                                   Os.CreateDirectory(tempPath);

                                   string downloadFromUrl = installationUrl.Replace("{VERSION}", lastVersion);

                                   Program.log.Write("CheckUpdates downloading: " + downloadFromUrl);

                                   Network.DownloadFile(downloadFromUrl, executablePath);

                                   string downloadedFileChecksum = Hash.GetFileHash(executablePath);

                                   if (downloadedFileChecksum == signature)
                                   {
                                       Os.OpenFileInExplorer(executablePath);
                                   }
                                   else
                                   {
                                       Program.log.Write("CheckUpdates error: invalid signature");
                                   }
                               } else if (updateForm.SkipVersion()) { 

                               }
                           }
                           else {
                               if (showCurrentVersionStatus)
                               {
                                   MessageBox.Show("You have last version.");
                               }
                           }

                       } catch (Exception ex) {
                            Program.log.Write("CheckUpdates error: " + ex.Message);
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

    }
}
