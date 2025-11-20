using System.Runtime.InteropServices;

#nullable disable

namespace Diagram
{
    public class WinProcess
    {

        [DllImport("shell32.dll", SetLastError = true)]
        static extern void SetCurrentProcessExplicitAppUserModelID([MarshalAs(UnmanagedType.LPWStr)] string AppID);

        /// <summary>
        /// Prevent taskbar to group diagrams to one
        /// </summary>
        public static void SetId() {
            SetCurrentProcessExplicitAppUserModelID(Randomizer.GetRandomString());
        }

    }
}
