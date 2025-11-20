using System.Runtime.InteropServices;

#nullable disable

namespace Diagram
{
    internal static partial class WinProcess
    {

        /*[DllImport("shell32.dll", SetLastError = true)]
        static extern void SetCurrentProcessExplicitAppUserModelID([MarshalAs(UnmanagedType.LPWStr)] string AppID);*/

        [LibraryImport("shell32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        internal static partial void SetCurrentProcessExplicitAppUserModelID(string appId);

        /// <summary>
        /// Prevent taskbar to group diagrams to one
        /// </summary>
        public static void SetId() {
            SetCurrentProcessExplicitAppUserModelID(Randomizer.GetRandomString());
        }

    }
}
