#nullable disable

namespace Diagram
{
    public interface IDropPlugin : IDiagramPlugin
    {
        bool DropAction(DiagramView diagramView, DragEventArgs e);
    }
}