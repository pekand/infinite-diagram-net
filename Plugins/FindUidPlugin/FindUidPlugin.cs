using Diagram;
using System.Text.RegularExpressions;

#nullable disable

namespace Plugin
{
    public partial class FindUidPlugin : INodeOpenPlugin, IOpenDiagramPlugin //UID0290845813
    {
        public string Name
        {
            get
            {
                return "Find uid in current diagram directory";
            }
        }

        public string Version
        {
            get
            {
                return "1.0";
            }
        }

        private string location = null;

        public void SetLocation(string location)
        {
            this.location = location;
        }

        private Log log = null;

        public void SetLog(Log log)
        {
            this.log = log;
        }

        [GeneratedRegex(@"^UID\d{10}$")]
        private static partial Regex UidMatch();

        [GeneratedRegex(@"^~[^ ]*$")]
        private static partial Regex StringMatch();

        public bool IsUid(string text)
        {
            Match matchUid = (UidMatch()).Match(text);

            if (matchUid.Success)
            {
                return true;
            }

            Match matchString = (StringMatch()).Match(text);

            if (matchString.Success)
            {
                return true;
            }

            return false;
        }

        [GeneratedRegex(@"^UID\d{10}$")]
        private static partial Regex UidMatch2();

        [GeneratedRegex(@"^~([^ ]*)$")]
        private static partial Regex WaveMatch();

        public string GetUid(string text)
        {
            text = text.Trim();

            Match matchUid = UidMatch2().Match(text); // match TEXT in  UIDTEXT for search

            if (matchUid.Success)
            {
                return text;
            }

            Match matchString = (WaveMatch()).Match(text); // match TEXT in ~TEXT for search

            if (matchString.Success)
            {
                return matchString.Groups[1].Value;
            }

            return null;
        }

        public void OpenFileOnPosition(string file, long pos = 0)
        {
            Os.OpenFileOnPosition(file, pos);
        }

        private class FileNameAndSizePair
        {
            public long size = 0;
            public string name = "";         
        }

        public bool ClickOnNodeAction(Diagram.Diagram diagram, DiagramView diagramview, Node node)
        {
            if (diagram.FileName !="" && this.IsUid(node.link)) {
                string uid = this.GetUid(node.link);
                if (Os.FileExists(diagram.FileName)) {
                    string diagramDirectory = Os.GetFileDirectory(diagram.FileName);

                    List<FileNameAndSizePair> files = [];

                    foreach (string file in Directory.EnumerateFiles(diagramDirectory, "*.*", SearchOption.AllDirectories))
                    {
                        FileNameAndSizePair pair = new()
                        {
                            name = file,
                            size = new System.IO.FileInfo(file).Length
                        };
                        files.Add(pair);                       
                    }

                    files.Sort(delegate (FileNameAndSizePair p1, FileNameAndSizePair p2) {
                        return p1.size < p2.size ? -1 : p1.size > p2.size ? 1 : 0; 
                    });

                    foreach (FileNameAndSizePair file in files)
                    {
                        try
                        {

                            // skip self
                            if (file.name == diagram.FileName)
                            {
                                continue;
                            }

                            long pos = 1;
                            foreach (string line in File.ReadAllLines(file.name))
                            {
                                if (line.Contains(uid))
                                {

                                    this.OpenFileOnPosition(file.name, pos);
                                    return true;
                                }
                                pos++;
                            }

                        }
                        catch (Exception ex)
                        {
                            Program.log.Write("FindUidPlugin: " + ex.Message);
                        }
                    }
                }
            }

            return false;
        }

        public void OpenDiagramAction(Diagram.Diagram diagram)
        {

        }


    }
}
