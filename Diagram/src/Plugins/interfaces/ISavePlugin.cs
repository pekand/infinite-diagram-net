using System.Xml.Linq;

namespace Diagram
{
    public interface ISavePlugin : IDiagramPlugin
    {
        bool SaveAction(Diagram diagram, XElement root);
    }
}