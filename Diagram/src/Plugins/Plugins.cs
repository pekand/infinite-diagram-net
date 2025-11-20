using System.Reflection;
using System.Runtime.Loader;
using System.Xml.Linq;

#nullable disable

namespace Diagram
{
    /// <summary>
    /// load plugins</summary>
    public class Plugins //UID8736657869
    {
        public List<IDiagramPlugin> plugins = [];
        public List<INodeOpenPlugin> nodeOpenPlugins = [];
        public List<IKeyPressPlugin> keyPressPlugins = [];
        public List<IOpenDiagramPlugin> openDiagramPlugins = [];
        public List<IPopupPlugin> popupPlugins = [];
        public List<IDropPlugin> dropPlugins = [];
        public List<ISavePlugin> savePlugins = [];
        public List<ILoadPlugin> loadPlugins = [];

        public AssemblyLoadContext loadContext = new("PluginsContext", isCollectible: true);

        /// <summary>
        /// load plugins from path</summary>
        public void LoadPlugins(string path)
        {
            try
            {
                Program.log.Write("Loading plugins from:" + path);

                IEnumerable<string> dllFileNames = null;
                if (Directory.Exists(path))
                {
                    dllFileNames = Directory.EnumerateFiles(path, "*.dll", SearchOption.AllDirectories);
                }

                List<Assembly> assemblies = new(dllFileNames.Count());
                foreach (string dllFile in dllFileNames)
                {
                    try
                    {
                        Program.log.Write("Loading plugin from: " + dllFile);
                        Assembly assembly = loadContext.LoadFromAssemblyPath(dllFile);

                        if (assembly != null)
                        {
                            assemblies.Add(assembly);
                        }
                    }
                    catch (FileNotFoundException fnfEx)
                    {
                        Program.log.Write($"File not found: {fnfEx.Message}");
                        if (fnfEx.InnerException != null)
                        {
                            Program.log.Write($"Inner exception: {fnfEx.InnerException.Message}");
                        }
                    }
                    catch (BadImageFormatException bifEx)
                    {
                        Program.log.Write($"Bad image format: {bifEx.Message}");
                    }
                    catch (FileLoadException fleEx)
                    {
                        Program.log.Write($"File load exception: {fleEx.Message}");
                        if (fleEx.InnerException != null)
                        {
                            Program.log.Write($"Inner exception: {fleEx.InnerException.Message}");
                        }
                    }
                    catch (Exception e)
                    {
                        Program.log.Write("Load plugin error: " + dllFile + "  : " + e.Message);
                    }
                }

                // proces all assemblies in folder
                foreach (Assembly assembly in assemblies)
                {
                    Type[] types;

                    try
                    {
                        types = assembly.GetTypes();
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        Program.log.Write("Load types from " + assembly.FullName + ": location:"+ assembly.Location + "plugin error  : " + ex.Message);

                        foreach (var item in ex.LoaderExceptions)
                        {                            
                            Program.log.Write("Loaderexceptoon: " + item.Message);
                        }

                        Program.log.Write("Skipping plugin due to errors");
                        continue;
                    }

                    // proces all elements like classes in assemblie
                    foreach (Type type in types)
                    {
                        if (type.IsInterface || type.IsAbstract)
                        {
                            continue;
                        }

                        // get all libraries with IDiagramPlugin interface
                        if (type.GetInterface(typeof(IDiagramPlugin).FullName) == null)
                        {
                            continue;
                        }

                        // create plugin instance
                        if (Activator.CreateInstance(type) is not IDiagramPlugin plugin)
                        {
                            continue;
                        }

                        // original assembly location for mapping resources
                        plugin.SetLocation(assembly.Location);

                        // add log object to plugin and allow debug messages from plugin
                        plugin.SetLog(Program.log);

                        // assign plugin to collection of all plugins
                        plugins.Add(plugin);

                        Program.log.Write("Loading plugin: " + plugin.Name);

                        // add plugin to category

                        if (type.GetInterface(typeof(INodeOpenPlugin).FullName) != null)
                        {
                            nodeOpenPlugins.Add(plugin as INodeOpenPlugin);
                        }

                        if (type.GetInterface(typeof(IKeyPressPlugin).FullName) != null)
                        {
                            keyPressPlugins.Add(plugin as IKeyPressPlugin);
                        }

                        if (type.GetInterface(typeof(IOpenDiagramPlugin).FullName) != null)
                        {
                            openDiagramPlugins.Add(plugin as IOpenDiagramPlugin);
                        }

                        if (type.GetInterface(typeof(IPopupPlugin).FullName) != null)
                        {
                            popupPlugins.Add(plugin as IPopupPlugin);
                        }

                        if (type.GetInterface(typeof(IDropPlugin).FullName) != null)
                        {
                            dropPlugins.Add(plugin as IDropPlugin);
                        }

                        if (type.GetInterface(typeof(ISavePlugin).FullName) != null)
                        {
                            savePlugins.Add(plugin as ISavePlugin);
                        }

                        if (type.GetInterface(typeof(ILoadPlugin).FullName) != null)
                        {
                           loadPlugins.Add(plugin as ILoadPlugin);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Program.log.Write("Load plugin error : " + e.Message);
            }
        }

        /// <summary>
        /// run event for all registred plugins in NodeOpenPlugins </summary>
        public bool ClickOnNodeAction(Diagram diagram, DiagramView diagramView, Node node) 
        {
            if (!diagram.IsSigned())
            {
                return false;
            }

            bool stopNextAction = false;
                           
            if (nodeOpenPlugins.Count > 0)
            {
                foreach (INodeOpenPlugin plugin in nodeOpenPlugins)
                {
                    try
                    {
                        stopNextAction = plugin.ClickOnNodeAction(diagram, diagramView, node); //UID6935831875
                        if (stopNextAction)
                        {
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        Program.log.Write("Exception in plugin: " + plugin.Name + " : " + e.Message);
                    }
                }
            }
            
            return stopNextAction;
        }

        /// <summary>
        /// run event for all registred plugins in KeyPressPlugins </summary>
        public bool KeyPressAction(Diagram diagram, DiagramView diagramView, Keys keyData)
        {
            if (!diagram.IsSigned())
            {
                return false;
            }

            bool stopNextAction = false;

            if (keyPressPlugins.Count > 0)
            {
                foreach (IKeyPressPlugin plugin in keyPressPlugins)
                {
                    try
                    {
                        stopNextAction = plugin.KeyPressAction(diagram, diagramView, keyData);
                        if (stopNextAction)
                        {
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        Program.log.Write("Exception in plugin: " + plugin.Name + " : " + e.Message);
                    }
                }
            }

            return stopNextAction;
        }

        /// <summary>
        /// run event for all registred plugins in KeyPressPlugins </summary>
        public void OpenDiagramAction(Diagram diagram)
        {
            if (!diagram.IsSigned())
            {
                return;
            }

            if (openDiagramPlugins.Count > 0)
            {
                foreach (IOpenDiagramPlugin plugin in openDiagramPlugins)
                {
                    try
                    {
                        plugin.OpenDiagramAction(diagram);

                    }
                    catch (Exception e)
                    {
                        Program.log.Write("Exception in plugin: " + plugin.Name + " : " + e.Message);
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        public void PopupAddItemsAction(DiagramView diagramView, Popup popup)
        {
            if (!diagramView.diagram.IsSigned())
            {
                return;
            }

            if (popupPlugins.Count > 0)
            {
                foreach (IPopupPlugin plugin in popupPlugins)
                {
                    try
                    {
                        plugin.PopupAddItemsAction(diagramView, popup.GetPluginsItem());

                    }
                    catch (Exception e)
                    {
                        Program.log.Write("Exception in plugin: " + plugin.Name + " : " + e.Message);
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        public void PopupOpenAction(DiagramView diagramView, Popup popup)
        {
            if (!diagramView.diagram.IsSigned())
            {
                return;
            }

            if (popupPlugins.Count > 0)
            {
                foreach (IPopupPlugin plugin in popupPlugins)
                {
                    try
                    {
                        plugin.PopupOpenAction(diagramView, popup.GetPluginsItem());

                    }
                    catch (Exception e)
                    {
                        Program.log.Write("Exception in plugin: " + plugin.Name + " : " + e.Message);
                    }
                }
            }
        }


        /// <summary>
        /// </summary>
        public bool DropAction(DiagramView diagramView, DragEventArgs ev)
        {
            if (!diagramView.diagram.IsSigned())
            {
                return false;
            }

            if (dropPlugins.Count > 0)
            {
                bool acceptAction = false;

                foreach (IDropPlugin plugin in dropPlugins)
                {
                    try
                    {
                        acceptAction = plugin.DropAction(diagramView, ev);

                        if (acceptAction) {
                            return true;
                        }

                    }
                    catch (Exception e)
                    {
                        Program.log.Write("Exception in plugin: " + plugin.Name + " : " + e.Message);
                    }
                }

                return acceptAction;
            }

            return false;
        }

        /// <summary>
        /// </summary>
        public bool SaveAction(Diagram diagram, XElement root)
        {
            if (!diagram.IsSigned())
            {
                return false;
            }

            if (dropPlugins.Count > 0)
            {
                bool acceptAction = false;

                foreach (ISavePlugin plugin in savePlugins)
                {
                    try
                    {
                        acceptAction = plugin.SaveAction(diagram, root);

                        if (acceptAction)
                        {
                            return true;
                        }

                    }
                    catch (Exception e)
                    {
                        Program.log.Write("Exception in plugin: " + plugin.Name + " : " + e.Message);
                    }
                }

                return acceptAction;
            }

            return false;
        }

        /// <summary>
        /// </summary>
        public bool LoadAction(Diagram diagram, XElement root)
        {
            if (!diagram.IsSigned()) {
                return false;
            }

            if (dropPlugins.Count > 0)
            {
                bool acceptAction = false;

                foreach (ILoadPlugin plugin in loadPlugins)
                {
                    try
                    {
                        acceptAction = plugin.LoadAction(diagram, root);

                        if (acceptAction)
                        {
                            return true;
                        }

                    }
                    catch (Exception e)
                    {
                        Program.log.Write("Exception in plugin: " + plugin.Name + " : " + e.Message);
                    }
                }

                return acceptAction;
            }

            return false;
        }
    }
}
