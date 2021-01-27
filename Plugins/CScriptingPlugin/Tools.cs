using System;
using System.Windows.Forms;
using System.IO;
using Diagram;
using System.Security.Cryptography;

namespace Plugin
{

    public class Tools //UID6975866488
    {
        private Diagram.Diagram diagram = null;
        private DiagramView diagramView = null;
        private Node node = null;
        private string clipboard = "";

        public Tools(Diagram.Diagram diagram, DiagramView diagramView, Node node, string clipboard = "")
        {
            this.diagram = diagram;
            this.diagramView = diagramView;
            this.node = node;
            this.clipboard = clipboard;

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
        // NODE

        // get node by script id
        public Node Get(string nodeScriptId)
        {
            return this.diagram.GetNodeByLink(nodeScriptId);
        }

        // get node by id
        public Node Id(long id)
        {
            return this.diagram.GetNodeByID(id);
        }

        // create node with position object
        public Node Create(Position p, string name = "", long layer = -1)
        {
            if (layer < 0)
            {
                layer = this.Layer().id;
            }

            return this.diagram.CreateNode(p, name, layer);
        }

        // create node with coordinates
        public Node Create(long x, long y, string name = "", long layer = -1)
        {
            return this.diagram.CreateNode(new Position(x,y), name, layer);
        }


        // remove node from diagram
        public void Remove(Node n)
        {
            this.diagram.DeleteNode(n);
        }

        // remove node from diagram
        public void Delete(Node n)
        {
            this.diagram.DeleteNode(n);
        }

        /*************************************************************************************************************************/
        // LINE

        // connect two nodes
        public Line Connect(Node a, Node b)
        {
            return this.diagram.Connect(a, b);
        }

        /*************************************************************************************************************************/
        // VIEW

        // get current view left corner position
        public Position Position()
        {
            return this.diagramView.shift;
        }

        // redraw view
        public void Refresh()
        {
            this.diagramView.Invalidate();
        }

        /*************************************************************************************************************************/
        // LAYER

        // get current layer
        public Layer Layer()
        {
            return this.diagramView.currentLayer;
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
            this.diagramView.GoToNode(n);
        }

        // go to position by coordinates
        public void Go(long x, long y, long layer = -1)
        {
            if (layer >= 0)
            {
                this.diagramView.GoToLayer(layer);
            }

            this.diagramView.GoToPosition(new Position(x, y));
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

        public string getRandomString(int length)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var bit_count = (length * 6);
                var byte_count = ((bit_count + 7) / 8); // rounded up
                var bytes = new byte[byte_count];
                rng.GetBytes(bytes);
                return Convert.ToBase64String(bytes);
            }
        }
        public string getRandomDigits(int length)
        {
            var random = new Random();
            string s = string.Empty;
            for (int i = 0; i < length; i++)
                s = String.Concat(s, random.Next(10).ToString());
            return s;
        }

        public Diagram.Diagram getCurrentDiagram() {
            return this.diagram;
        }

        public Diagram.DiagramView getCurrentDiagramView()
        {
            return this.diagramView;
        }

    }
}
