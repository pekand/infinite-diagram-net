using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Diagram
{
    public class WinProcess
    {

        [DllImport("shell32.dll", SetLastError = true)]
        static extern void SetCurrentProcessExplicitAppUserModelID([MarshalAs(UnmanagedType.LPWStr)] string AppID);

        public static void setId() {
            SetCurrentProcessExplicitAppUserModelID(Encrypt.GetRandomString());
        }

    }
}
