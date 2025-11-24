#nullable disable

namespace Diagram 
{
    public interface IOpenDiagramPlugin : IDiagramPlugin  
    {
        void OpenDiagramAction(Diagram diagram);
    }
}
