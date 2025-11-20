using Diagram;
using System.ComponentModel;
using System.Text.RegularExpressions;

#nullable disable

namespace Plugin
{
    public partial class ScriptingPlugin : INodeOpenPlugin, IKeyPressPlugin //UID0290845814
    {

        public string Name
        {
            get
            {
                return "Scripting Plugin";
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

        public bool ClickOnNodeAction(Diagram.Diagram diagram, DiagramView diagramView, Node node)
        {
            if (node.link.Trim() == "#ironpython")  // OPEN SCRIPT node with link "script" is executed as script
            {
                this.Evaluate(diagram, diagramView, node, "");
                return true;
            }

            return false;
        }

        public bool KeyPressAction(Diagram.Diagram diagram, DiagramView diagramView, Keys keyData)
        {

            if (KeyMap.ParseKey("F9", keyData)) // [KEY] [F9] evaluate python script for selected nodes by stamp in link
            {
                this.Evaluate(diagram, diagramView); 
                return true;
            }

            return false;
        }
        [GeneratedRegex(@"^\s*(eval(uate)|!){1}(#\w+){0,1}\s*$")]
        private static partial Regex EvaluateMatch();

        // SCRIPT evaluate python script in nodes signed with stamp in node link [F9]
        private void Evaluate(Diagram.Diagram diagram, DiagramView diagramView)
        {
            Nodes nodes = null;

            if (diagramView.selectedNodes.Count > 0)
            {
                nodes = [.. diagramView.selectedNodes];
            }
            else
            {
                nodes = [.. diagram.GetAllNodes()];
            }
            // remove nodes whit link other then [ ! | eval | evaluate | !#num_order | eval#num_order |  evaluate#num_order]
            // higest number is executed first
            Regex regex = EvaluateMatch();
            nodes.RemoveAll(n => !regex.Match(n.link).Success);

            nodes.OrderByLink();
            nodes.Reverse();

            String clipboard = Os.GetTextFormClipboard();

#if !DEBUG
            Job.DoJob(
                new DoWorkEventHandler(
                    delegate (object o, DoWorkEventArgs args)
                    {
#endif
                        this.Evaluate(diagram, diagramView, nodes, clipboard);
#if !DEBUG
                    }
                )
            );
#endif
        }

        public string GetStandardLibraryPath()
        {
            string standardLibraryPath = "";
            
            if (Os.FileExists(this.location))
            {
                string pluginDirectory = Os.GetDirectoryName(this.location);
                standardLibraryPath = Os.Combine(pluginDirectory, "IronPython.zip");

                if (!Os.FileExists(standardLibraryPath))
                {
                    Program.log.Write("ScriptPlugim: standard python librar IronPython.zip not exist");
                    return null;
                }

            }

            return standardLibraryPath;
        }

        Script script = null;

        private void InitScriptEngine()
        {
            script ??= new Script(GetStandardLibraryPath());

        }

        // SCRIPT evaluate python script in node name or in node note in nodes
        private void Evaluate(Diagram.Diagram diagram, DiagramView diagramView, Nodes nodes, string clipboard = "")
        {
            Program.log.Write("diagram: openlink: run macro");

            InitScriptEngine();
            script.SetDiagram(diagram);
            script.SetDiagramView(diagramView);
            script.SetClipboard(clipboard);

            foreach (Node node in nodes)
            {
                string body = node.note.Trim() != "" ? node.note : node.name;
                script.RunScript(body);
            }
        }

        // SCRIPT evaluate python script in node name or in node note in node
        private void Evaluate(Diagram.Diagram diagram, DiagramView diagramView, Node node, string clipboard = "")
        {
            // run macro
            Program.log.Write("diagram: openlink: run macro");
            InitScriptEngine();
            script.SetDiagram(diagram);
            script.SetDiagramView(diagramView);
            script.SetClipboard(clipboard);
            string body = node.note.Trim() != "" ? node.note : node.name;
            script.RunScript(body);
        }


    }
}
