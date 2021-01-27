using System;
using Diagram;
using System.Windows.Forms;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Linq;
using System.Security.Cryptography;
using System.Reflection;
using Microsoft.CodeAnalysis.Scripting.Hosting;

namespace Plugin
{

    public class Globals
    {
        public Tools Script { get; set; }
    }

    public class CScriptingPlugin : INodeOpenPlugin, IKeyPressPlugin //UID0290845814
    {
        public string Name
        {
            get
            {
                return "Csharp Scripting Plugin";
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
            if (node.link.Trim() == "#csharp")  // OPEN SCRIPT node with link "script" is executed as script
            {

                String clipboard = Os.GetTextFormClipboard();

                try
                {
                    this.EvaluateAsync(diagram, diagramView, node, clipboard).Wait();
                }
                catch (System.AggregateException ae)
                {
                    ae.Handle(ex =>
                    {
                        Program.log.Write("Exception in embed csharp script: "+ ex.Message);
                        return true;
                    });
                    
                }


                return true;
            }

            return false;
        }

        public bool KeyPressAction(Diagram.Diagram diagram, DiagramView diagramView, Keys keyData)
        {
            return false;
        }

        private InteractiveAssemblyLoader interactiveLoader = null;
        private ScriptOptions scriptOptions = null;

        // SCRIPT evaluate python script in node name or in node note in node
        private async System.Threading.Tasks.Task EvaluateAsync(Diagram.Diagram diagram, DiagramView diagramView, Node node, string clipboard = "")
        {
            string body = node.note;

            if (this.interactiveLoader == null) {
                this.interactiveLoader = new InteractiveAssemblyLoader();
            }

            if (this.scriptOptions == null)
            {
                this.scriptOptions = ScriptOptions.Default;

                var asms = AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly asm in asms)
                {

                    if (asm.ManifestModule.Name == "Diagram.dll") {
                        this.scriptOptions = this.scriptOptions.AddReferences(asm);
                    }
                }

                this.scriptOptions = this.scriptOptions.AddImports("Diagram");
            }

            System.Threading.Tasks.Task<ScriptState<object>> task = CSharpScript.Create(
                options: this.scriptOptions,
                code: body,
                globalsType: typeof(Globals),
                assemblyLoader: this.interactiveLoader
            )               
            .RunAsync(new Globals { 
                Script = new Tools(diagram, diagramView, node, clipboard)
            }); ;
        }
    }
}
 