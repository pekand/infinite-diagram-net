using System.Xml.Linq;

namespace Diagram
{
    public interface ILoadPlugin : IDiagramPlugin
    {
        bool LoadAction(Diagram diagram, XElement root);
    }
}