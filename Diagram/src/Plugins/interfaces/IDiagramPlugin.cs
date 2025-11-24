#nullable disable

namespace Diagram
{
    public interface IDiagramPlugin 
    {
        // name for identification plugin
        string Name { get; }

        // plugin version
        string Version { get; }

        // plugin location for resource mapping
        void SetLocation(string location);

        // connection to program debug console
        void SetLog(Log log);

    }
}
