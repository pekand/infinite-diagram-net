using System;
using System.Collections.Generic;
using System.Linq;

namespace Diagram
{
    /// <summary>
    /// collection of layers</summary>
    public class Layers //UID6548243626
    {
        private long maxid = 0;                    // last used node id

        private readonly Dictionary<long, Node> allNodes = new Dictionary<long, Node>();

        private readonly List<Layer> layers = new List<Layer>();

        /*************************************************************************************************************************/
        // CONSTRUCTORS

        public Layers()
        {
            this.GetOrCreateLayer();
        }

        public Layer GetOrCreateLayer(Node parent = null)
        {
            Layer layer = GetLayer((parent == null) ? 0 : parent.id);

            // create new layer if not exist
            if (layer == null)
            {
                Layer parentLayer = null;

                if (parent != null)
                {
                    parentLayer = this.GetLayer(parent.layer);
                }

                layer = new Layer(parent, parentLayer);
                this.layers.Add(layer);
            }

            return layer;
        }

        /// <summary>
        /// Add referencies to layers to parents for fast parents search
        /// Is called after load diagram from file or clipboard
        /// </summary>
        public void SetLayersParentsReferences()
        {
            foreach (Layer l in this.layers)
            {
                if (l.id != 0)
                {
                    foreach (Layer p in this.layers)
                    {
                        if (l.parentNode.layer == p.id)
                        {
                            l.parentLayer = p;
                            break;
                        }
                    }
                }
            }
        }

        /*************************************************************************************************************************/
        // SELECT ITEM

        public Node GetNode(long id)
        {
            if (this.allNodes.ContainsKey(id)) {
                return this.allNodes[id];
            }

            return null;
        }

        public Line GetLine(Node start, Node end)
        {
            foreach (Layer l in this.layers)
            {
                foreach (Line line in l.lines)
                {
                    if ((line.start == start.id && line.end == end.id) || (line.start == end.id && line.end == start.id))
                        return line;
                }
            }

            return null;
        }

        public Line GetLine(long startId, long endId)
        {
            Node startNode = GetNode(startId);

            if (startNode == null)
            {
                return null;
            }

            Node endNode = GetNode(endId);

            if (endNode == null)
            {
                return null;
            }

            return GetLine(startNode, endNode);
        }

        public bool HasLayer(long id = 0)
        {
            foreach (Layer l in this.layers)
            {
                if (l.id == id)
                {
                    return true;
                }
            }

            return false;
        }

        public Layer GetLayer(long id = 0)
        {
            foreach (Layer l in this.layers)
            {
                if (l.id == id)
                {
                    return l;
                }
            }

            return null;
        }

        public Layer GetLayer(Node node)
        {
            if (node.layer == 0) {
                return GetLayer(0);
            }

            foreach (Layer l in this.layers)
            {

                if (l.parentNode != null && l.parentNode.id == node.layer)
                {
                    return l;
                }

            }

            return null;
        }

        public Nodes GetAllNodes()
        {
            Nodes nodes = new Nodes();

            foreach (Node n in this.allNodes.Values)
            {
                nodes.Add(n);
            }

            return nodes;
        }

        public Nodes GetAllNodes(Node node)
        {
            Nodes nodes = new Nodes();

            if (node.haslayer)
            {
                Layer layer = this.GetLayer(node.id);

                foreach (Node subNode in layer.nodes)
                {
                    nodes.Add(subNode);

                    if (subNode.haslayer)
                    {
                        Nodes subNodes = this.GetAllNodes(subNode);

                        foreach (Node subNode2 in subNodes)
                        {
                            nodes.Add(subNode2);
                        }
                    }
                }
            }

            return nodes;
        }

        public Lines GetAllLines()
        {
            Lines lines = new Lines();

            foreach (Layer l in this.layers)
            {
                lines.AddRange(l.lines);
            }

            return lines;
        }      

        // all nodes contain nodes and all sublayer nodes, allLines contain all node lines and all sublayer lines UID4508113260
        public void GetAllNodesAndLines(Nodes nodes, ref Nodes allNodes, ref Lines allLines)
        {
            foreach (Node node in nodes)
            {
                // add node itself to output
                allNodes.Add(node);

                if (node.haslayer)
                {
                    Layer layer = this.GetLayer(node.id);
                    GetAllNodesAndLines(layer.nodes, ref allNodes, ref allLines);
                }

                Lines lines = GetAllLinesFromNode(node);
                foreach (Line line in lines)
                {
                    bool found = false;

                    foreach (Line subline in allLines)
                    {
                        if (line.start == subline.start && line.end == subline.end)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        allLines.Add(line);
                    }
                }
            }
        }

        // get all lines for all children nodes
        public Lines GetAllSubNodeLines(Node node)
        {
            Lines lines = new Lines();

            if (node.haslayer)
            {
                Layer layer = this.GetLayer(node.id);

                foreach (Line line in layer.lines)
                {
                    lines.Add(line);
                }

                foreach (Node subNode in layer.nodes)
                {
                    if (node.haslayer)
                    {
                        Lines sublines = this.GetAllSubNodeLines(subNode);

                        foreach (Line line in sublines)
                        {
                            lines.Add(line);
                        }
                    }
                }
            }

            return lines;
        }

        public Lines GetAllLinesFromNode(Node node) //UID1353555007
        {
            Lines lines = new Lines();

            Layer layer = this.GetLayer(node);

            if (layer != null)
            {
                foreach (Line line in layer.lines)
                {
                    if (line.start == node.id || line.end == node.id)
                    {
                        lines.Add(line);
                    }
                }
            }

            return lines;
        }

        public Lines GetAllLinesFromNodes(Nodes nodes)
        {
            Lines lines = new Lines();

            foreach (Line li in this.GetAllLines())
            {
                foreach (Node recstart in nodes)
                {
                    if (li.start == recstart.id)
                    {
                        foreach (Node recend in nodes)
                        {
                            if (li.end == recend.id)
                            {
                                lines.Add(li);
                            }
                        }
                    }
                }
            }

            return lines;
        }

        /// <summary>
        /// Search for string in all nodes </summary>        
        public Nodes SearchInAllNodes(string searchFor)
        {
            Nodes nodes = new Nodes();

            foreach (Layer l in this.layers)
            {
                foreach (Node node in l.nodes)
                {
                    if (node.Contain(searchFor))
                    {
                        nodes.Add(node);
                    }
                }
            }

            return nodes;
        }

        /*************************************************************************************************************************/
        // ADD ITEM

        public Node CreateNode(Node node)
        {

            DateTime dt = DateTime.Now;
            node.timecreate = String.Format("{0:yyyy-M-d HH:mm:ss}", dt);
            node.timemodify = node.timecreate;

            Layer layer = this.AddNode(node);

            if (layer != null)
            {
                return node;
            }

            return null;
        }

        public Layer AddNode(Node node)
        {
            // prevent duplicate id
            if (node.id == 0)
            {
                node.id = ++maxid; // new node
            }
            else
            {

                Node nodeById = this.GetNode(node.id); 

                if (nodeById != null)
                {
                    node.id = ++maxid; // already exist node with this id
                }
                else if (maxid < node.id)
                {
                    maxid = node.id; // node not exist but has id bigger then current max id
                }
            }

            Layer layer = this.GetLayer(node.layer);

            if (layer == null)
            {
                Node parentNode = this.GetNode(node.layer);
                layer = this.GetOrCreateLayer(parentNode);
                parentNode.haslayer = true;
            }

            this.allNodes.Add(node.id, node);
            layer.nodes.Add(node);

            return layer;
        }

        public Layer AddLine(Line line)
        {
            Layer layer = this.GetLayer(line.layer);

            if (layer == null)
            {
                layer = this.GetOrCreateLayer(this.GetNode(line.layer));
            }

            layer.lines.Add(line);

            return layer;
        }

        /*************************************************************************************************************************/
        // REMOVE ITEM

        public void RemoveNode(long id)
        {
            Node node = this.GetNode(id);
            if (node != null)
            {
                this.RemoveNode(node);
            }
        }

        public void RemoveNode(Node node) //UID6631907739
        {
            this.allNodes.Remove(node.id);

            Layer layer = GetLayer(node.layer);

            foreach (Node n in layer.nodes)
            {
                if (n.shortcut == node.id) // remove shortcut to node
                {
                    n.shortcut = 0;
                }
            }

            foreach (Line l in layer.lines.Reverse<Line>()) // remove lines to node
            {
                if (l.start == node.id || l.end == node.id)
                {
                    this.RemoveLine(l);
                }
            }

            if (node.haslayer) // remove nodes in node layer
            {
                RemoveLayer(node.id);
            }

            if (layer != null) // remove node from node layer
            {
                layer.nodes.Remove(node);
            }

        }

        public Layer RemoveLine(Line line)
        {
            Layer layer = this.GetLayer(line.layer);

            layer.lines.Remove(line);

            return layer;
        }

        public Layer RemoveLine(long startId, long endId)
        {
            Line line = GetLine(startId, endId);

            if (line != null)
            {
                return this.RemoveLine(line);
            }

            return null;
        }

        // LAYER remove layer and all sub layers
        public void RemoveLayer(long layerId)
        {
            Layer layer = this.GetLayer(layerId);

            if (layer != null)
            {
                foreach (Node n in layer.nodes)
                {
                    if (n.haslayer)
                    {
                        RemoveLayer(n.id);
                    }

                    layers.Remove(layer);
                }
            }
        }

        public void Clear()
        {
            this.maxid = 0;

            foreach (Layer l in this.layers)
            {
                l.lines.Clear();
                l.nodes.Clear();
            }

            this.allNodes.Clear();
            this.layers.Clear();
            this.GetOrCreateLayer();
        }

        /*************************************************************************************************************************/
        // MOVE ITEM

        // MOVE move node from layer to other layer
        public void MoveNode(Node node, long layer)
        {
            Layer outLayer = GetLayer(layer);
            Layer inLayer = GetLayer(node.layer);

            if (outLayer != null && inLayer != null)
            {
                outLayer.nodes.Add(node);
                inLayer.nodes.Remove(node);

                node.layer = layer;
            }
        }

        // NODE move nodes to foreground
        public void MoveToForeground(Node node)
        {
            Layer layer = GetLayer(node);
            if (layer != null)
            {
                var item = node;
                layer.nodes.Remove(item);
                layer.nodes.Insert(layer.nodes.Count(), item);
            }
        }

        // NODE move nodes to background
        public void MoveToBackground(Node node)
        {
            Layer layer = GetLayer(node);
            if (layer != null)
            {
                var item = node;
                layer.nodes.Remove(node);
                layer.nodes.Insert(0, item);
            }
        }
    }
}
