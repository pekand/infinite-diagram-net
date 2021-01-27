using System.Windows.Forms;

namespace Diagram
{
    public interface IDropPlugin : IDiagramPlugin
    {
        bool DropAction(DiagramView diagramview, DragEventArgs e);
    }
}