﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

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

        [DllImport("shell32.dll", CharSet = CharSet.Ansi)]
        private static extern void
            SHAddToRecentDocs(ShellAddToRecentDocsFlags flag, string path);

    }
}
