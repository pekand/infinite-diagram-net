#nullable disable

namespace Diagram 
{
    public interface INodeOpenPlugin : IDiagramPlugin  
    {
        bool ClickOnNodeAction(Diagram diagram, DiagramView diagramView, Node node);
    }
}
