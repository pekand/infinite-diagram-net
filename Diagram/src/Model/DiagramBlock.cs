#nullable disable

namespace Diagram
{
    /// <summary>
    /// container for manipulation with part of diagram</summary> 
    public class DiagramBlock(Nodes nodes = null, Lines lines = null) //UID6305074892
    {
        public Nodes nodes = nodes;
        public Lines lines = lines;
    }
}
