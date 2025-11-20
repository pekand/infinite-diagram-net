#nullable disable

namespace Diagram
{
    /// <summary>
    /// container for manipulation with part of diagram</summary> 
    public class DiagramBlock //UID6305074892
    {
        public Nodes nodes = [];
        public Lines lines = [];

        public DiagramBlock(Nodes nodes = null, Lines lines = null)
        {
            this.nodes = nodes;
            this.lines = lines;
        }
    }
}
