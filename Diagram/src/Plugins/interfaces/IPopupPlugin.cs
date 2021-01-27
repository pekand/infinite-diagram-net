namespace Diagram 
{
    public interface IPopupPlugin : IDiagramPlugin
    {
        void PopupAddItemsAction(DiagramView diagramView, System.Windows.Forms.ToolStripMenuItem pluginsItem);
        void PopupOpenAction(DiagramView diagramView, System.Windows.Forms.ToolStripMenuItem pluginsItem);
        
    }
}
