#nullable disable

namespace Diagram 
{
    public interface IKeyPressPlugin : IDiagramPlugin  
    {
        bool KeyPressAction(Diagram diagram, DiagramView diagramView, Keys keyData);
    }
}
