using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Security;
using Microsoft.Win32;


namespace Diagram
{
    /// <summary>    
    /// Processing global options files
    /// Start server
    /// Processing command line arguments.
    /// Create mainform
    /// </summary>
    public class Main //UID7118462915
    {

        /*************************************************************************************************************************/
        // PROGRAM OPTONS

        /// <summary>
        /// Global program options</summary>
        public ProgramOptions programOptions = null;

        /// <summary>
        /// managing file with global program options</summary>
        private ProgramOptionsFile programOptionsFile = null;

        /// <summary>
        /// keyboard shorcut mapping</summary>
        public KeyMap keyMap = null;

        /*************************************************************************************************************************/
        // Plugins

        /// <summary>
        /// load plugins</summary>
        public string pluginsDirectoryName = "plugins";
        public Plugins plugins = null;

        /*************************************************************************************************************************/
        // SERVER

        /// <summary>
        /// local messsaging server for communication between running program instances</summary>
        private Server server = null;

        /*************************************************************************************************************************/
        // DIAGRAMS

        /// <summary>
        /// all opened diagrams models</summary>
        private readonly List<Diagram> Diagrams = new List<Diagram>();


        /*************************************************************************************************************************/
        // VIEWS

        /// <summary>
        /// all opened form views to diagrams models</summary>
        private readonly List<DiagramView> DiagramViews = new List<DiagramView>();

        /*************************************************************************************************************************/
        // DIAGRAM EDIT FORMS

        /// <summary>
        /// all opened node edit forms for all diagrams models</summary>
        private readonly List<TextForm> TextWindows = new List<TextForm>();

        /*************************************************************************************************************************/
        // PASSWORD FORMS

        /// <summary>
        ///input form for password</summary>
        private PasswordForm passwordForm = null;

        /// <summary>
        /// input form for new password</summary>
        private NewPasswordForm newPasswordForm = null;

        /// <summary>
        /// input form for change old password</summary>
        private ChangePasswordForm changePasswordForm = null;


        /*************************************************************************************************************************/
        // ABOUT FORM        

        /// <summary>
        /// about form for display basic informations about application</summary>
        private AboutForm aboutForm = null;

        /*************************************************************************************************************************/
        // CONSOLE

        /// <summary>
        /// form for display logged messages</summary>
        private Console console = null;

        /*************************************************************************************************************************/
        // MAIN APPLICATION

        // command line arguments
        private string[] args = null;

        /// <summary>
        /// form for catching messages from local server</summary>
        public MainForm mainform = null;

        /*************************************************************************************************************************/
        // CONSTRUCTOR

        /// <summary>
        /// parse command line arguments and open forms</summary>
        public Main() //UID8239288102
        {
            Program.log.Write("Program: Main");

            this.LoadProgramOptionFiles();

            this.LoadPugins();

            this.LoadServer();

            this.ProcessCommandLineArguments();

            this.RegisterPowerChangeEvent();

            // check if this program instance created server (is main application)
            // or if running debug console from command line parameter
            if ((server.mainProcess && this.Diagrams.Count > 0) || this.console != null)
            {
                this.mainform = new MainForm(this);

                Update update = new Update();
                update.CheckUpdates();
            }
        }

        /*************************************************************************************************************************/
        // PROGRAM CONFIGURATION

        /// <summary>
        /// load options from global configuration files</summary>
        public void LoadProgramOptionFiles() //UID3013916734
        {
            this.programOptions = new ProgramOptions();
            programOptionsFile = new ProgramOptionsFile(this.programOptions);
        }

        /// <summary>
        /// open directori with global configuration</summary>
        public void OpenConfigDir()
        {
            Os.ShowDirectoryInExternalApplication(Os.GetDirectoryName(this.programOptionsFile.optionsFilePath));
        }

        /*************************************************************************************************************************/
        // PLUGINS

        /// <summary>
        /// load plugins from assebmblies</summary>
        public void LoadPugins()
        {
            // load external plugins UID9841812564
            plugins = new Plugins();

            // load plugins from current application directory (portable mode)
            string pluginsLocalDirectory = Os.Combine(Os.GetCurrentApplicationDirectory(), this.pluginsDirectoryName);
            if (Os.DirectoryExists(pluginsLocalDirectory))
            {
                plugins.LoadPlugins(pluginsLocalDirectory);
            }

#if !DEBUG
            // load plugins from global plugins directory
            string pluginsGlobalDirectory = Os.Combine(this.programOptionsFile.GetGlobalConfigDirectory(), this.pluginsDirectoryName);
            if (Os.DirectoryExists(pluginsGlobalDirectory))
            {
                plugins.LoadPlugins(pluginsGlobalDirectory);
            }
#endif

        }

        /*************************************************************************************************************************/
        // SERVER

        /// <summary>
        /// load server</summary>
        public void LoadServer()
        {
            // create local server for comunication between local instances UID2964640610
            server = new Server(this);

            server.StartServer();
        }

        /*************************************************************************************************************************/
        // COMMAND LINE

        /// <summary>
        /// process command line arguments</summary>
        public void ProcessCommandLineArguments()
        {
            // get command line arguments
            this.args = Environment.GetCommandLineArgs();

            // process comand line arguments
            this.ParseCommandLineArguments(this.args);
        }

        /// <summary>
        /// process comand line arguments</summary>
        public void ParseCommandLineArguments(string[] args) // [PARSE] [COMMAND LINE] UID5172911205
        {
            // options - create new file with given name if not exist
            bool CommandLineCreateIfNotExistFile = false;

            bool ShowCommandLineHelp = false;
            bool ShowDebugConsole = false;

            // list of diagram files names for open
            List<String> CommandLineOpen = new List<String>();

            String arg;
            for (int i = 0; i < args.Length; i++)
            {

                //skip application name
                if (i == 0)
                {
                    continue;
                }

                // current processing argument
                arg = args[i];

                // [COMAND LINE] [CREATE]  oprions create new file with given name if not exist
                if (arg == "-h" || arg == "--help" || arg == "/?")
                {
                    ShowCommandLineHelp = true;
                    break;

                }
                if (arg == "-c" || arg == "--console")
                {
                    ShowDebugConsole = true;
                    break;
                }

                if (arg == "-e")
                {
                    CommandLineCreateIfNotExistFile = true;
                    break;
                }

                // [COMAND LINE] [OPEN] check if argument is diagram file
                if (Os.GetExtension(arg).ToLower() == ".diagram")
                {
                    CommandLineOpen.Add(arg);
                    break;
                }

                Program.log.Write("bed commmand line argument: " + arg);
            }

            if (ShowDebugConsole)
            {
                this.ShowConsole();
            }

            // open diagram given as arguments
            if (ShowCommandLineHelp)
            {
                String help =
                "diagram -h --help /?  >> show this help\n" +
                "diagram -c --console /?  >> show debug console\n" +
                "diagram -e {filename} >> create file if not exist\n" +
                "diagram {filepath} {filepath} >> open existing file\n";
                MessageBox.Show(help, "Command line parameters");
                return;
            }

            if (CommandLineOpen.Count == 0)
            {
                if (this.programOptions.defaultDiagram != "" && Os.FileExists(this.programOptions.defaultDiagram))
                {
                    this.OpenDiagram(this.programOptions.defaultDiagram); // open default diagram if default diagram is set
                }
                else if (this.programOptions.openLastFile && this.programOptions.recentFiles.Count > 0 && Os.FileExists(this.programOptions.recentFiles[0]))
                {
                    this.OpenDiagram(this.programOptions.recentFiles[0]); // open last file if user option is enabled UID2130542088
                }
                else
                {
                    this.OpenDiagram(); //open empty diagram UID5981683893
                }

                return;
            }

            for (int i = 0; i < CommandLineOpen.Count; i++)
            {
                string file = CommandLineOpen[i];

                // tray create diagram file if command line option is set
                if (CommandLineCreateIfNotExistFile && !Os.FileExists(file))
                {
                    try
                    {
                        Os.CreateEmptyFile(file);
                    }
                    catch (Exception ex)
                    {
                        Program.log.Write("create empty diagram file error: " + ex.Message);
                    }
                }

                if (Os.FileExists(file))
                {
                    this.OpenDiagram(file); //UID2130542088
                }
            }

            // cose application if is not diagram model opened
            this.CloseEmptyApplication();
        }

        /*************************************************************************************************************************/
        // LOCK DIAGRAM AND POWER CHANGE

        /// <summary>
        /// register power save event </summary>
        public void RegisterPowerChangeEvent()
        {
            // sleep or hibernate event UID7641650028
            SystemEvents.PowerModeChanged += OnPowerChange;
        }

        /// <summary>
        /// lock encrypted diagrams if computer go to sleep or hibernation</summary>
        private void OnPowerChange(object s, PowerModeChangedEventArgs e) // UID1864495676
        {
            switch (e.Mode)
            {
                case PowerModes.Resume:
                    break;
                case PowerModes.Suspend:
                    this.LockDiagrams();
                    break;
            }
        }

        /// <summary>
        /// forgot password if diagram is encrypted</summary>
        public void LockDiagrams() //UID6105963009
        {
            foreach (Diagram diagram in Diagrams)
            {
                diagram.LockDiagram();
            }
        }

        /// <summary>
        /// prompt for password if diagram is encrypted</summary>
        public void UnlockDiagrams()
        {
            foreach (Diagram diagram in Diagrams)
            {
                diagram.UnlockDiagram();
            }
        }


        /*************************************************************************************************************************/
        // DIAGRAMS

        /// <summary>
        /// add diagram to list of all diagrams</summary>
        public void AddDiagram(Diagram diagram)
        {
            this.Diagrams.Add(diagram);
        }

        /// <summary>
        /// remove diagram from list of all diagrams</summary>
        public void RemoveDiagram(Diagram diagram) //UID1434097522
        {
            this.Diagrams.Remove(diagram);
        }

        /// <summary>
        /// open existing diagram or create new empty diagram
        /// Create diagram model and then open diagram view on this model</summary>
        public bool OpenDiagram(String FilePath = "") //UID1771511767
        {
            Program.log.Write("Program : OpenDiagram: " + FilePath);

            if (passwordForm != null) // prevent open diagram if another diagram triing open 
            {
                return false;
            }
            
            // open new empty diagram in main process
            if (FilePath == "" && !server.mainProcess)
            {
                // if server already exist in system, send him message whitch open empty diagram
                server.SendMessage("open:");
                return false;
            }
            
            // open diagram in current program instance
            if (FilePath == "" && server.mainProcess)
            {
                // create new model
                Diagram emptyDiagram = new Diagram(this);
                Diagrams.Add(emptyDiagram);
                // open diagram view on diagram model
                emptyDiagram.OpenDiagramView();
                return false;
            }
              
            // open existing diagram file
            
            if (!Os.FileExists(FilePath))
            {
                return false;
            }
            
            FilePath = Os.NormalizedFullPath(FilePath);
            
            // if server already exist in system, send him message whitch open diagram file
            if (!server.mainProcess)
            {
                FilePath = Os.GetFullPath(FilePath);
                server.SendMessage("open:" + FilePath); //UID1105610325
                return false;
            }  

            // open diagram in current program instance

            // check if file is already opened in current instance
            bool alreadyOpen = false;
            foreach (Diagram openedDiagram in Diagrams)
            {
                if (openedDiagram.FileName != FilePath)
                {
                    continue;
                }

                openedDiagram.FocusToView();

                alreadyOpen = true;
                break;
            }

            if (alreadyOpen)
            {
                return false;
            }
            
            Diagram diagram = new Diagram(this); //UID8780020416
            lock (diagram)
            {
                // create new model
                if (!diagram.OpenFile(FilePath))
                {
                    return false;
                }
                
                this.programOptions.AddRecentFile(FilePath);

#if !MONO
                RecentFiles.AddToRecentlyUsedDocs(FilePath); // add to system recent files
#endif

                Diagrams.Add(diagram);
                // open diagram view on diagram model
                DiagramView newDiagram = diagram.OpenDiagramView(); //UID3015837184

                this.plugins.OpenDiagramAction(diagram); //UID0290845816

                Program.log.Write("bring focus");
                Media.BringToFront(newDiagram); //UID4510272263
            }

            return true;
        }

        /*************************************************************************************************************************/
        // VIEWS

        /// <summary>
        /// add diagram view to list of all views</summary>
        public void AddDiagramView(DiagramView view)
        {
            this.DiagramViews.Add(view);
        }

        /// <summary>
        /// remove diagram view from list of all diagram views</summary>
        public void RemoveDiagramView(DiagramView view)
        {
            this.DiagramViews.Remove(view);
        }

        /// <summary>
        /// hide diagram views except diagramView</summary>
        public void SwitchViews(DiagramView diagramView = null) //UID9164062779
        {
            bool someIsHidden = false;
            foreach (DiagramView view in DiagramViews)
            {
                if (!view.Visible)
                {
                    someIsHidden = true;
                    break;
                }
            }

            if (someIsHidden)
            {
                ShowAllViews();
            }
            else
            {
                HideViews(diagramView);
            }
        }

        /// <summary>
        /// show views if last visible view is closed</summary>
        public void ShowFirstHiddenView(DiagramView diagramView = null) //UID3969703093
        {
            foreach (DiagramView view in DiagramViews)
            {
                if (!view.Visible && diagramView != view)
                {
                    view.Show();
                    break;
                }
            }
        }

        /// <summary>
        /// show diagram views</summary>
        public void ShowAllViews() //UID5230996149
        {
            foreach (DiagramView view in DiagramViews)
            {
                if (!view.Visible)
                {
                    view.Show();
                }
            }
        }

        /// <summary>
        /// hide diagram views</summary>
        public void HideViews(DiagramView diagramView = null) //UID3131107610
        {
            foreach (DiagramView view in DiagramViews)
            {
                if (view != diagramView) {
                    view.Hide();
                }
            }
        }

        /*************************************************************************************************************************/
        // DIAGRAM EDIT FORMS

        /// <summary>
        /// add text form to list of all text forms</summary>
        public void AddTextWindow(TextForm textWindows)
        {
            this.TextWindows.Add(textWindows);
        }

        /// <summary>
        /// remove text form from list of all text forms</summary>
        public void RemoveTextWindow(TextForm textWindows)
        {
            this.TextWindows.Remove(textWindows);
        }

        /*************************************************************************************************************************/
        // PASSWORD FORMS

        /// <summary>
        /// show dialog for password for diagram unlock</summary>
        public string GetPassword(string subtitle = "")
        {
            string password = null;

            if (this.passwordForm == null)
            {
                this.passwordForm = new PasswordForm(this);
            }

            this.passwordForm.Text = Translations.password + " - " + subtitle;
            this.passwordForm.Clear();
            this.passwordForm.ShowDialog();
            if (!this.passwordForm.cancled)
            {
                password = this.passwordForm.GetPassword();
                this.passwordForm.Clear();
            }
            else 
            {
                return null;
            }

            this.passwordForm = null;

            return password;
        }

        /// <summary>
        /// show dialog for new password for diagram</summary>
        public string GetNewPassword()
        {
            string password = null;

            if (this.newPasswordForm == null)
            {
                this.newPasswordForm = new NewPasswordForm(this);
            }

            this.newPasswordForm.Clear();
            this.newPasswordForm.ShowDialog();
            if (!this.newPasswordForm.cancled)
            {
                password = this.newPasswordForm.GetPassword();
                this.newPasswordForm.Clear();
            }

            this.newPasswordForm = null;

            return password;
        }

        /// <summary>
        /// show dialog for change password for diagram</summary>
        public string ChangePassword(SecureString currentPassword)
        {
            string password = null;

            if (this.changePasswordForm == null)
            {
                this.changePasswordForm = new ChangePasswordForm(this);
            }

            this.changePasswordForm.Clear();
            this.changePasswordForm.oldpassword = currentPassword;
            this.changePasswordForm.ShowDialog();
            if (!this.changePasswordForm.cancled)
            {
                password = this.changePasswordForm.GetPassword();
                this.changePasswordForm.Clear();
            }

            this.changePasswordForm = null;

            return password;
        }

        /*************************************************************************************************************************/
        // ABOUT FORM        

        /// <summary>
        /// show about</summary>
        public void ShowAbout()
        {
            if (this.aboutForm == null)
            {
                this.aboutForm = new AboutForm(this);
            }

            this.aboutForm.ShowDialog();

            this.aboutForm = null;
        }

        /*************************************************************************************************************************/
        // CONSOLE

        /// <summary>
        /// show error console</summary>
        public void ShowConsole()
        {
            if (this.console == null)
            {
                this.console = new Console(this);
                this.console.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CloseConsole);
            }

            this.console.Show();
        }

        /// <summary>
        /// clean after error console close</summary>
        private void CloseConsole(object sender, FormClosedEventArgs e)
        {
            this.console = null;
            this.CloseEmptyApplication();
        }

        /*************************************************************************************************************************/
        // CLOSE APPLICATION

        /// <summary>
        /// close application if not diagram view or node edit form is open </summary>
        public void CloseEmptyApplication() //UID0787891060
        {
            Program.log.Write("Program : CloseApplication");

            bool canclose = true;

            if (Diagrams.Count > 0 || DiagramViews.Count > 0 || TextWindows.Count > 0)
            {
                canclose = false;
            }

            if (console != null)
            {
                // prevent close application if debug console is open
                // console must by closed mannualy by user
                Program.log.Write("Program : Console is still open...");
                canclose = false;
            }

            if (canclose)
            {
                ExitApplication();
            }
        }

        /// <summary>
        /// force close application</summary>
        public void ExitApplication() //UID0090378181
        {
            Program.log.Write("Program : ExitApplication");

            if (passwordForm != null)
            {
                passwordForm.Close();
            }

            if (newPasswordForm != null)
            {
                newPasswordForm.Close();
            }

            if (changePasswordForm != null)
            {
                changePasswordForm.Close();
            }

            if (mainform != null)
            {
                mainform.Close();
            }
            
            if (console != null)
            {
                console.Close();
            }

            if (this.server != null && this.server.mainProcess)
            {
                this.programOptionsFile.SaveConfigFile();                
                server.RequestStop();
            }
        }

        public void TerminateApplication() //UID4067187261
        {
            try
            {                
                Application.Exit();
                Application.ExitThread();
                Environment.Exit(0);
            }
            catch (Exception e)
            {
                Program.log.Write("Terminate aplication exception: " + e.Message);
                Program.log.Write("Terminate aplication trace: " + e.StackTrace);
            }
        }

    }
}
