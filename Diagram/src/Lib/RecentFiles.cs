using System.Runtime.InteropServices;

#nullable disable

namespace Diagram
{
    public static class RecentFiles
    {
        public static void AddToRecentlyUsedDocs(string path)
        {
            SHAddToRecentDocs(ShellAddToRecentDocsFlags.Path, path);
        }


        private enum ShellAddToRecentDocsFlags
        {
            Pidl = 0x001,
            Path = 0x002,
        }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern void
            SHAddToRecentDocs(ShellAddToRecentDocsFlags flag, string path);

    }
}
