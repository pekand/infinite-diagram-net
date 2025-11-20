using System.Runtime.InteropServices;

#nullable disable

namespace Diagram
{
    internal static partial class RecentFiles
    {
        public static void AddToRecentlyUsedDocs(string path)
        {
            SHAddToRecentDocs(ShellAddToRecentDocsFlags.Path, path);
        }


        internal enum ShellAddToRecentDocsFlags
        {
            Pidl = 0x001,
            Path = 0x002,
        }

        [LibraryImport("shell32.dll", StringMarshalling = StringMarshalling.Utf16)]
        static partial void SHAddToRecentDocs(ShellAddToRecentDocsFlags flag, string path);

    }
}
