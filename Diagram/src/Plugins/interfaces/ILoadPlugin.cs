using System.Xml.Linq;

#nullable disable

namespace Diagram
{
    public interface ILoadPlugin : IDiagramPlugin
    {
        bool LoadAction(Diagram diagram, XElement root);
    }
}