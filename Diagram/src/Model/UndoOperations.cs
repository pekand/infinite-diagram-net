using System.Collections.Generic;
using System.Linq;

namespace Diagram
{

    /// <summary>
    /// one undo operation</summary> 
    public class UndoOperation //UID6178817122
    {
        public string type = ""; // type of undo operation (delete, create, edit, move, changeLineColor, changeLineWidth, changeNodeColor)

        public long group = 0; // undo operations group. Undo operations in some group are undo as one operation
        
        public Nodes nodes = new Nodes(); // affected nodes
        public Lines lines = new Lines(); // affected lines
        public decimal scale = 0;

        public Position position = new Position(); // position in diagram when change occurred
        public long layer = 0; // layer in diagram when change occurred

        /*************************************************************************************************************************/
        // CONSTRUCTORS

        public UndoOperation(
            string type, 
            Nodes nodes, 
            Lines lines,
            long group, 
            Position position,
            decimal scale,
            long layer
        ) {
            this.type = type;
            this.group = group;
            this.position.Set(position);
            this.scale = scale;
            this.layer = layer;

            if (nodes != null)
            {
                foreach (Node node in nodes)
                {
                    this.nodes.Add(new Node(node));
                }
            }

            if (lines != null)
            {
                foreach (Line line in lines)
                {
                    this.lines.Add(new Line(line));
                }
            }
        }
    }

    /// <summary>
    /// container for UndoOperations and undo manipulation</summary> 
    public class UndoOperations
    {
        public long group = 0; // if two operations is in same group then undo restore both operations

        public long saved = 0; // if is 0 then indicate saved
        public bool saveLost = false; // if save is in redo and redo is cleared then save position is lost
        public bool grouping = false; // if grouping is true and new undo is added then new undo is same group as previous undo

        public Diagram diagram = null;                // diagram assigned to current undo
        
        public Stack<UndoOperation> operations = new Stack<UndoOperation>(); // saved undo operations
        public Stack<UndoOperation> reverseOperations = new Stack<UndoOperation>(); // saved redo operations

        /*************************************************************************************************************************/
        // CONSTRUCTORS

        public UndoOperations(Diagram diagram)
        {
            this.diagram = diagram;
        }

        /*************************************************************************************************************************/
        // ADD UNDO OPERATIONS

        public void Add(string type, Node node, Position position = null, decimal scale = 0, long layer = 0)
        {
            Nodes nodes = new Nodes();
            if (node != null)
            {
                nodes.Add(new Node(node));
            }
            this.Add(type, nodes, null, position, scale, layer);
        }

        public void Add(string type, Line line, Position position = null, decimal scale = 0, long layer = 0)
        {
            Lines lines = new Lines();
            if (line != null)
            {
                lines.Add(new Line(line));
            }
            this.Add(type, null, lines, position, scale, layer);
        }

        public void Add(string type, Node node, Line line, Position position = null, decimal scale = 0, long layer = 0)
        {
            Nodes nodes = new Nodes();
            if (node != null)
            {
                nodes.Add(new Node(node));
            }

            Lines lines = new Lines();
            if (line != null)
            {
                lines.Add(new Line(line));
            }
            this.Add(type, nodes, lines, position, scale, layer);
        }

        public void Add(string type, Nodes nodes = null, Lines lines = null, Position position = null, decimal scale = 0, long layer = 0)
        {
            operations.Push(
                new UndoOperation(
                    type, 
                    (nodes != null) ? new Nodes(nodes) : null, 
                    (lines != null) ? new Lines(lines) : null,
                    (grouping) ? group : 0, // add multiple operations into one undo group
                    position.Clone(),
                    scale,
                    layer
                )
            );

            this.saved++;
            
            // forgot undo operation
            if (reverseOperations.Count() > 0)
            {
                if (this.saved > 0) // save is in redo but if redo is cleared theh save is lost
                {
                    this.saveLost = true;
                }
                reverseOperations.Clear();
            }
        }

        /*************************************************************************************************************************/
        // UNDO OPERATIONS

        public bool CanUndo()
        {
            return this.operations.Count() > 0;
        }

        public bool CanRedo()
        {
            return this.reverseOperations.Count() > 0;
        }

        public bool DoUndo(DiagramView view = null)
        {

            if (operations.Count() == 0)
            {
                return false;
            }

            long group = 0;

            bool result = false;

            do
            {
                UndoOperation operation = operations.First();

                // first restore position where change occurred
                if (view != null && !view.IsOnPosition(operation.position, operation.scale, operation.layer))
                {
                    view.GoToPosition(operation.position, operation.scale, operation.layer);
                    view.Invalidate();
                    return false;
                }

                // process all operations in same group
                if (group != 0 && operation.group != group)
                {
                    group = 0;
                    break;
                }

                group = operation.group;

                if (operation.type == "delete")
                {
                    this.DoUndoDelete(operation);
                    reverseOperations.Push(operation);
                }

                if (operation.type == "create")
                {
                    this.DoUndoCreate(operation);
                    reverseOperations.Push(operation);
                }

                if (operation.type == "edit" ||
                    operation.type == "move" ||
                    operation.type == "changeLineColor" ||
                    operation.type == "changeLineWidth" ||
                    operation.type == "changeNodeColor"
                )
                {
                    Nodes nodes = new Nodes();
                    foreach (Node node in operation.nodes)
                    {
                        nodes.Add(this.diagram.GetNodeByID(node.id));
                    }

                    Lines lines = new Lines();
                    foreach (Line line in operation.lines)
                    {
                        lines.Add(this.diagram.GetLine(line.start, line.end));
                    }

                    UndoOperation roperation = new UndoOperation(
                        operation.type,
                        nodes,
                        lines,
                        operation.group,
                        operation.position,
                        operation.scale,
                        operation.layer
                    );
                    reverseOperations.Push(roperation);
                    this.DoUndoEdit(operation);
                }

                operations.Pop();
                result = true;
            } while (group != 0 && operations.Count() > 0);

            if (result)
            {
                this.saved--;
                if (!this.saveLost && this.saved == 0)
                {
                    this.diagram.Restoresave();
                }
                else
                {
                    this.diagram.Unsave();
                }
            }

            return result;
        }

        public bool DoRedo(DiagramView view = null)
        {
            if (reverseOperations.Count() == 0)
            {
                return false;
            }

            long group = 0;
            bool result = false;

            do
            {
                UndoOperation operation = reverseOperations.First();

                // first restore position where change occurred
                if (view != null && !view.IsOnPosition(operation.position, operation.scale, operation.layer))
                {
                    view.GoToPosition(operation.position, operation.scale, operation.layer);
                    view.Invalidate();
                    return false;
                }

                // process all operations in same group
                if (group != 0 && operation.group != group)
                {
                    group = 0;
                    break;
                }

                group = operation.group;

                if (operation.type == "delete")
                {
                    this.DoUndoCreate(operation);
                    operations.Push(operation);
                }

                if (operation.type == "create")
                {
                    this.DoUndoDelete(operation);
                    operations.Push(operation);
                }

                if (operation.type == "edit" ||
                    operation.type == "move" ||
                    operation.type == "changeLineColor" ||
                    operation.type == "changeLineWidth" ||
                    operation.type == "changeNodeColor"
                )
                {
                    Nodes nodes = new Nodes();
                    foreach (Node node in operation.nodes)
                    {
                        nodes.Add(this.diagram.GetNodeByID(node.id));
                    }

                    Lines lines = new Lines();
                    foreach (Line line in operation.lines)
                    {
                        lines.Add(this.diagram.GetLine(line.start, line.end));
                    }


                    UndoOperation roperation = new UndoOperation(
                        operation.type,
                        nodes,
                        lines,
                        operation.group,
                        operation.position,
                        operation.scale,
                        operation.layer
                    );

                    operations.Push(roperation);
                    this.DoUndoEdit(operation);
                }

                reverseOperations.Pop();
                result = true;

            } while (group != 0 && reverseOperations.Count() > 0);

            if (result)
            {
                this.saved++;
                if (!this.saveLost && this.saved == 0)
                {
                    this.diagram.Restoresave();
                }
                else
                {
                    this.diagram.Unsave();
                }
            }

            return result;
        }


        /*************************************************************************************************************************/
        // EVERSE OPERATION BY TYPE

        private void DoUndoDelete(UndoOperation operation)
        {
            if (operation.nodes != null)
            {
                foreach (Node node in operation.nodes)
                {
                    this.diagram.layers.AddNode(node);
                }
            }

            if (operation.lines != null)
            {
                foreach (Line line in operation.lines)
                {
                    line.startNode = this.diagram.GetNodeByID(line.start);
                    line.endNode = this.diagram.GetNodeByID(line.end);
                    this.diagram.layers.AddLine(line);
                }
            }
        }

        private void DoUndoCreate(UndoOperation operation)
        {
            if (operation.lines != null)
            {
                foreach (Line line in operation.lines)
                {
                    this.diagram.layers.RemoveLine(line.start, line.end);
                }
            }

            if (operation.nodes != null)
            {
                foreach (Node node in operation.nodes)
                {
                    this.diagram.layers.RemoveNode(node.id);
                }
            }
        }

        private void DoUndoEdit(UndoOperation operation)
        {
            if (operation.lines != null)
            {
                foreach (Line lineOld in operation.lines)
                {
                    lineOld.startNode = this.diagram.GetNodeByID(lineOld.start);
                    lineOld.endNode = this.diagram.GetNodeByID(lineOld.end);
                    Line line = this.diagram.layers.GetLine(lineOld.startNode, lineOld.endNode);

                    if (line != null)
                    {
                        line.Set(lineOld);
                    }
                }
            }

            if (operation.nodes != null)
            {
                foreach (Node nodeOld in operation.nodes)
                {
                    Node node = this.diagram.layers.GetNode(nodeOld.id);

                    if (node != null)
                    {
                        node.Set(nodeOld);
                    }
                }
            }
        }

        /*************************************************************************************************************************/
        // GROUPING OF UNDO OPERATIONS

        public long StartGroup()
        {
            grouping = true;
            return ++this.group;
        }

        public long EndGroup()
        {
            grouping = false;
            return ++this.group;
        }

        /// <summary>
        /// Check if operation is same as previous operation 
        /// If previous operation is move node (by arrow forexample) is better group operation like one big move instead of many small moves.
        /// </summary>
        public bool IsSame(string type, Nodes nodes, Lines lines)
        {
            
            if (operations.Count()>0)
            {

                UndoOperation operation = operations.First();

                if (operation.type == type)
                {

                    if (operation.nodes.Count() == 0 && nodes == null)
                    {
                    }
                    else if ((operation.nodes == null && nodes != null) || (operation.nodes != null && nodes == null))
                    {
                        return false;
                    }
                    else if (operation.nodes.Count() == nodes.Count())
                    {
                        for (int i = 0; i < nodes.Count(); i++)
                        {
                            if (operation.nodes[i].id != nodes[i].id)
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }

                    if (operation.lines.Count() == 0 && lines == null)
                    {
                    }
                    else if ((operation.lines == null && lines != null) || (operation.lines != null && lines == null))
                    {
                        return false;
                    }
                    else if (operation.lines.Count() == lines.Count())
                    {
                        for (int i = 0; i < lines.Count(); i++)
                        {
                            if (operation.lines[i].start != lines[i].start || operation.lines[i].end != lines[i].end)
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }

                    return true;
                }
            }

            return false;
        }

        /*************************************************************************************************************************/
        // SAVE STATE

        /// <summary>
        /// Save current state as state where file is saved and save is not needed.
        /// When undo or redo is executed and after it file is already savet in curent state asterisk from title can be removed.
        /// </summary>
        public void RememberSave()
        {
            saveLost = false;
            saved = 0;
        }

    }
}
