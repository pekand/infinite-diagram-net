namespace Diagram 
{
    public interface INodeOpenPlugin : IDiagramPlugin  //UID0290945800
    {
        bool ClickOnNodeAction(Diagram diagram, DiagramView diagramview, Node node);
    }
}
