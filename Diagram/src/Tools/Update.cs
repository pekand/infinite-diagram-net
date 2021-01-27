using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

namespace Diagram
{
    class Update
    {

        private string updateFolderName = "InfiniteDiagramUpdate";
        private string updateExecutableName = "infinite-diagram-install.exe";
        private string homepage = "https://infinite-diagram.pekand.com/";
        private string lastversionFile = "lastversion.txt";
        private string signatureFile = "signature.txt";
        private string installationUrl = "https://github.com/pekand/infinite-diagram-core/releases/download/v{VERSION}/infinite-diagram-install.exe";

        public void CheckUpdates(bool showCurrentVersionStatus = false) {
            Job.DoJob(
               new DoWorkEventHandler(
                   delegate (object o, DoWorkEventArgs args)
                   {
                       try
                       {
                           string currentVersion = Os.GetThisAssemblyVersion();
                           Program.log.Write("CheckUpdates current version: " + currentVersion);

                           string lastVersion = Network.GetWebPage(this.homepage + this.lastversionFile);
                           
                           Program.log.Write("CheckUpdates last version: " + lastVersion);

                           if (lastVersion == null) {
                               return;
                           }

                           lastVersion = lastVersion.TrimEnd('\r', '\n').Trim();


                           Version localVersion = new Version(currentVersion);
                           Version serverVersion = new Version(lastVersion);

                           if (serverVersion.CompareTo(localVersion) == 1)
                           {

                               string signature = Network.GetWebPage(this.homepage + this.signatureFile);
                               signature = signature.TrimEnd('\r', '\n').Trim();

                               if (signature == null || signature.Length < 64)
                               {
                                   return;
                               }

                               UpdateForm updateForm = new UpdateForm();
                               updateForm.ShowDialog();

                               if (updateForm.CanUpdate())
                               {
                                   string tempPath = Os.Combine(Os.GetTempPath(), this.updateFolderName);
                                   string executablePath = tempPath + Os.GetSeparator() + this.updateExecutableName;
                                   Os.RemoveDirectory(tempPath);
                                   Os.CreateDirectory(tempPath);

                                   string downloadFromUrl = installationUrl.Replace("{VERSION}", lastVersion);

                                   Program.log.Write("CheckUpdates downloading: " + downloadFromUrl);

                                   Network.DownloadFile(downloadFromUrl, executablePath);

                                   string downloadedFileChecksum = Hash.GetFileHash(executablePath);

                                   if (downloadedFileChecksum == signature)
                                   {
                                       Os.RunProcess(executablePath);
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
