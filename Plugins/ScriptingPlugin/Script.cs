using System;
using System.Windows.Forms;

using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System.IO;
using System.Drawing;
using Diagram;


/*
    ! | eval | evaluate
    script executed by F9
    evaluate selected nodes or all nodes globaly

    !#1 | eval#1 | evaluate#1
    script evaluated by priority

    $ | script | macro
    evaluate node only by double click

    @id
    get node by diagram.getNoteBySciptId
*/

/*
    #Tools

    F.log('text') # write to console F12
    F.show(string message) # show message dialog
    F.setClipboard() # set clipboard content to value after script finish
    F.getClipboard() # get clipboard content before script run
    F.get('scriptId') # get node by scriptId
    F.id(1) # get node by id
    F.layer() #current layer id
    F.create(100, 100, "name", [layerId]) #create node id
    F.connect(nodeA, nodeB)
    F.remove(node)
    F.delete(node)
    F.go(node) #go to node position
    F.go(x, y, [layerId])
    F.position() # return current position in diagram
    F.refresh() # refres diagram views
    F.val('123') # convert str to int
*/

/*
# example of python script:
#
# create circle from nodes in current layer

import clr
import math
clr.AddReference('Diagram')
from Diagram import Position

a = (2 * math.pi) / 100
l = DiagramView.currentLayer.id
prev = None
for i in range(100):
    x = int(500 * math.cos(a*i))
    y = int(500 * math.sin(a*i))
    rec = Diagram.CreateNode(Position(x, y), "", l)
    rec.transparent = True
    if prev != None:
        Diagram.Connect(rec, prev)
    prev = rec
DiagramView.Invalidate();

*/

/*
# example of python script:
#
# create circle from nodes in current layer

import clr
import math

a = (2 * math.pi) / 100
l = F.layer().id
prev = None
for i in range(100):
    x = int(500 * math.cos(a*i))
    y = int(500 * math.sin(a*i))
    rec = F.create(x, y, "", l)
    rec.transparent = True
    if prev != None:
        F.connect(rec, prev)
    prev = rec
F.refresh();

*/

/*
    # short example of python script using Tools:
    #
    # create circle from nodes in current layer

    import math

    a = (2 * math.pi) / 100
    l = F.layer()
    prev = None
    for i in range(100):
        x = int(500 * math.cos(a*i))
        y = int(500 * math.sin(a*i))
        rec = F.create(x, y, "", l)
        rec.transparent = True
        if prev != None:
            F.connect(rec, prev)
        prev = rec
    F.refresh()
*/

namespace Plugin
{

    /// <summary>
    ///  Tools advalible from script
    /// </summary>
    /// <example>
    /// Tools.log('message')
    /// Tools.ShowMessage('message')
    /// Tools.setClipboard('text')
    /// a = Tools.getClipboard()
    /// </example>
    public class Tools //UID6975866488
    {
        private Script script = null;

        public Tools(Script script)
        {
            this.script = script; // for get scope of script inside script (instance of whole parent script class)
        }

        /*************************************************************************************************************************/
        // MESSAGES

        // add mesage to console window
        public void Log(String text)
        {
            Program.log.Write(text);
        }

        // show alert message
        public void Show(string message)
        {
            MessageBox.Show(message);
        }

        /*************************************************************************************************************************/
        // CLIPBOARD

        public string clipboard = "";

        // set text content of clipboard (engine cant acess to cliboard)
        public void SetClipboard(String clipboard)
        {
            this.clipboard = clipboard;
        }

        // get text message from clipboard (text before engine start running)
        public String GetClipboard()
        {
            return this.clipboard;
        }

        /*************************************************************************************************************************/
        // NODE

        // get node by script id
        public Node Get(string nodeScriptId)
        {
            return this.script.diagram.GetNodeByLink(nodeScriptId);
        }

        // get node by id
        public Node Id(long id)
        {
            return this.script.diagram.GetNodeByID(id);
        }

        // create node with position object
        public Node Create(Position p, string name = "", long layer = -1)
        {
            if (layer < 0)
            {
                layer = this.Layer().id;
            }

            return this.script.diagram.CreateNode(p, name, layer);
        }

        // create node with coordinates
        public Node Create(long x, long y, string name = "", long layer = -1)
        {
            return this.script.diagram.CreateNode(new Position(x,y), name, layer);
        }


        // remove node from diagram
        public void Remove(Node n)
        {
            this.script.diagram.DeleteNode(n);
        }

        // remove node from diagram
        public void Delete(Node n)
        {
            this.script.diagram.DeleteNode(n);
        }

        /*************************************************************************************************************************/
        // LINE

        // connect two nodes
        public Line Connect(Node a, Node b)
        {
            return this.script.diagram.Connect(a, b);
        }

        /*************************************************************************************************************************/
        // VIEW

        // get current view left corner position
        public Position Position()
        {
            return this.script.diagramView.shift;
        }

        // redraw view
        public void Refresh()
        {
            this.script.diagramView.Invalidate();
        }

        /*************************************************************************************************************************/
        // LAYER

        // get current layer
        public Layer Layer()
        {
            return this.script.diagramView.currentLayer;
        }

        /*************************************************************************************************************************/
        // POSITION

        // create position object
        public Position Position(long x, long y)
        {
            return new Position(x, y);
        }

        // go to node with animation
        public void Go(Node n)
        {
            this.script.diagramView.GoToNode(n);
        }

        // go to position by coordinates
        public void Go(long x, long y, long layer = -1)
        {
            if (layer >= 0)
            {
                this.script.diagramView.GoToLayer(layer);
            }

            this.script.diagramView.GoToPosition(new Position(x, y));
        }

        /*************************************************************************************************************************/
        // CONVERT

        // convert string to int
        public long Val(string s)
        {			
			return Converter.ToInt(s);
        }

        // convert int to string
        public string Val(long v)
        {
            return v.ToString();
        }


    }

    /// <example>
    /// try
    /// {
    ///
    ///     Script macro = new Script();
    ///     result = macro.runScript(this.SelectedNodes[0].text);
    /// }
    /// catch(Exception ex)
    /// {
    ///     Program.log.write("evaluation error: " + ex.Message);
    /// }
    /// </example>
    public class Script //UID6870742772
    {
        private ScriptEngine pyEngine = null;
        private dynamic pyScope = null;
        private string StandardLibraryPath;

        // ATTRIBUTES Diagram
        public Diagram.Diagram diagram = null;       // diagram ktory je previazany z pohladom
        public DiagramView diagramView = null;       // diagram ktory je previazany z pohladom
        public Tools tools = null;

        public string script = "";

        /*************************************************************************************************************************/
        // ENGINE

        public Script(string StandardLibraryPath = "")
        {
            this.StandardLibraryPath = StandardLibraryPath;
            this.tools = new Tools(this);
        }

        /// <summary>
        /// Run python code in curent scope
        /// </summary>
        /// <param name="code">python code</param>
        /// <returns></returns>
        private dynamic CompileSourceAndExecute(String code)
        {
            ScriptSource source = pyEngine.CreateScriptSourceFromString(code, SourceCodeKind.Statements);
            CompiledCode compiled = source.Compile(); // Executes in the scope of Python
            return compiled.Execute(pyScope);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="script">Script with python code</param>
        /// <example>
        /// import clr
        /// clr.AddReference('Diagram')
        /// from Diagram import Position
        /// DiagramView.CreateNode(Position(0,0))
        /// </example>
        /// <example>
        /// Tools.log("test")
        /// Tools.ShowMessage("test")
        /// Tools.setClipboard("test")
        /// clp = Tools.getClipboard()
        /// <returns>Return script string result</returns>
        public string RunScript(String script)
        {
            if (pyEngine == null)
            {
                pyEngine = Python.CreateEngine();
                var paths = pyEngine.GetSearchPaths();
                if (Os.FileExists(this.StandardLibraryPath))
                {
                    paths.Add(this.StandardLibraryPath);
                }
                pyEngine.SetSearchPaths(paths);
                pyScope = pyEngine.CreateScope();

                /// add items to scope
                pyScope.Diagram = this.diagram;
                pyScope.Tools = this.tools;
                pyScope.F = this.tools;
                pyScope.DiagramView = this.diagramView;
            }

            string output = null;

            try
            {
                /// set streams
                MemoryStream ms = new MemoryStream();
                StreamWriter outputWr = new StreamWriter(ms);
                pyEngine.Runtime.IO.SetOutput(ms, outputWr);
                pyEngine.Runtime.IO.SetErrorOutput(ms, outputWr);

                /// execute script
                this.CompileSourceAndExecute(script);

                /// read script output
                ms.Position = 0;
                StreamReader sr = new StreamReader(ms);
                output = sr.ReadToEnd();

                Program.log.Write("Script: output:\n" + output);
            }
            catch (Exception ex)
            {
                Program.log.Write("Script: error: " + ex.ToString());
            }

            return output;
        }

        /*************************************************************************************************************************/
        // REFERENCIES

        /// <summary>
        /// Set current diagram for context in script
        /// </summary>
        /// <param name="diagram"></param>
        public void SetDiagram(Diagram.Diagram diagram)
        {
            this.diagram = diagram;
        }

        /// <summary>
        /// Get diagram in current script context
        /// </summary>
        /// <returns>Diagram</returns>
        public Diagram.Diagram GetDiagram()
        {
            return this.diagram;
        }

        /// <summary>
        /// Set current diagramView for context in script
        /// </summary>
        /// <param name="diagramView"></param>
        public void SetDiagramView(DiagramView diagramView)
        {
            this.diagramView = diagramView;
        }

        /// <summary>
        /// Get diagramView in current script context
        /// </summary>
        /// <returns></returns>
        public DiagramView GetDiagramView()
        {
            return this.diagramView;
        }

        /// <summary>
        /// Set text to clipboard
        /// </summary>
        /// <param name="clipboard"></param>
        public void SetClipboard(String clipboard)
        {
            this.tools.SetClipboard(clipboard);
        }

        /// <summary>
        /// Get text from clipboard
        /// </summary>
        /// <returns></returns>
        public String GetClipboard()
        {
            return this.tools.GetClipboard();
        }

    }
}
