namespace Diagram
{
    /// <summary>
    /// Line between two nodes in diagram</summary>
    public class Line //UID5674116969
    {
        /*************************************************************************************************************************/
        // POSITION

        public long start = 0; // node id 
        public long end = 0; // node id 
        public decimal scale = 0;

        public Node startNode = null; // linked start node for quick access
        public Node endNode = null; // linked end node for quick access

        /*************************************************************************************************************************/
        // STYLES

        public bool arrow = false; // node is rendered as arrow
        public ColorType color = new ColorType(); // line color
        public long width = 1; // line width

        /*************************************************************************************************************************/
        // LAYER

        public long layer = 0; // layer parent node id

        /*************************************************************************************************************************/
        // CONSTRUCTORS

        public Line()
        {
        }

        public Line(Line line)
        {
            this.Set(line);
        }

        /*************************************************************************************************************************/
        // SETTERS AND GETTERS

        public void Set(Line line)
        {
            this.start = line.start;
            this.end = line.end;
            this.startNode = line.startNode;
            this.endNode = line.endNode;
            this.arrow = line.arrow;
            this.color.Set(line.color);
            this.width = line.width;
            this.layer = line.layer;
        }

        /// <summary>
        /// clone line to new line</summary>
        public Line Clone()
        {
            return new Line(this);
        }
    }
}
