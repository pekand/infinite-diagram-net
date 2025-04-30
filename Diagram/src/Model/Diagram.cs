using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using System.Security;

#nullable disable

namespace Diagram
{
    // map node structure for copy paste operation
    public struct MappedNode
    {
        public long oldId;
        public Node newNode;
    }

    public class Diagram //UID2487098516
    {
        private readonly Main main = null;                 // reference to main form

        /*************************************************************************************************************************/
        // COOLECTIONS

        public Layers layers = new Layers();
        public List<DiagramView> DiagramViews = new List<DiagramView>(); // all views forms to diagram
        public List<TextForm> TextWindows = new List<TextForm>();   // opened text textforms for this diagram

        /*************************************************************************************************************************/
        // ATTRIBUTES OPTIONS

        public Options options = new Options();  // diagram options saved to xml file        

        /*************************************************************************************************************************/
        // FILE

        public bool NewFile = true;              // flag for new unsaved file without name
        public bool SavedFile = true;            // flag for saved diagram with name
        public string FileName = "";             // path to diagram file        

        /*************************************************************************************************************************/
        // ATTRIBUTES ENCRYPTION

        private bool encrypted = false;           // flag for encrypted file
        private bool locked = false;              // flag for encrypted file
        private SecureString password = null;     // password for encrypted file
        private string passwordHash = null;
        private byte[] salt = null;               // salt

        /*************************************************************************************************************************/
        // ATTRIBUTES SIGNATURE
        private bool signed = true;              // check if diagram is writen by current user 

        /*************************************************************************************************************************/
        // UNDO

        public UndoOperations undoOperations = null;  // undo operations repository        

        /*************************************************************************************************************************/
        // RESOURCES

        public Font FontDefault = null; // default font

        /*************************************************************************************************************************/
        // PLUGINS

        public DataStorage dataStorage = new DataStorage();

        /*************************************************************************************************************************/
        // CONSTRUCTORS

        public Diagram(Main main = null)
        {
            this.main = main;
            this.FontDefault = new Font("Open Sans", 10);
            this.undoOperations = new UndoOperations(this);
        }

        /*************************************************************************************************************************/
        // LOAD DIAGRAM

        // open file. If file is invalid return false UID4610064109
        public bool OpenFile(string FileName)
        {
            if (Os.FileExists(FileName))
            {
                Os.SetCurrentDirectory(Os.GetFileDirectory(FileName));

                this.ResetDefaultValuesForNewFile();
                this.FileName = FileName;
                this.NewFile = false;
                this.SavedFile = true;
                this.signed = false;

                string xml = Os.GetFileContent(FileName);

                if (xml == null) {
                    this.ResetDefaultValuesForNewFile();
                    return false;
                }

                bool opened;
                if (xml.Trim() == "" || xml.Trim() == "<diagram></diagram>")
                {
                    this.signed = true; // empty file is by default signed
                    opened = true; // count empty file as valid new diagram
                }
                else
                {
                    opened = this.LoadXML(xml);
                }

                this.SetTitle();

                return opened;

            }

            return false;
        }

        // XML LOAD If file is invalid return false UID9397528693
        public bool LoadXML(string xml)
        {

            string version = null;
            string salt = null;
            string encrypted = null;
            string signature = null;
            string signatureIV = this.main.programOptions.signatureIV;
            XElement diagram = null;

            XElement option = null;
            XElement rectangles = null;
            XElement lines = null;

            try
            {

                XElement root = this.StringToXml(xml);

                foreach (XElement element in root.Elements())
                {
                    if (element.Name.ToString() == "version") // version 1
                    {
                        version = element.Value;
                    }

                    if (element.Name.ToString() == "salt") // version 1
                    {
                        salt = element.Value;
                    }

                    if (element.Name.ToString() == "encrypted") // version 1
                    {
                        encrypted = element.Value;
                    }

                    if (element.Name.ToString() == "option") // version 1
                    {
                        option = element;
                    }

                    if (element.Name.ToString() == "rectangles") // version 1
                    {
                        rectangles = element;
                    }

                    if (element.Name.ToString() == "lines") // version 1
                    {
                        lines = element;
                    }

                    if (element.Name.ToString() == "signatureIV") // version 2
                    {
                        signatureIV = element.Value;
                    }

                    if (element.Name.ToString() == "signature") // version 2
                    {
                        signature = element.Value;
                    }

                    if (element.Name.ToString() == "diagram")  // version 2
                    {
                        diagram = element;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Translations.fileHasWrongFormat);
                Program.log.Write("load xml error: " + ex.Message);
                return false;
            }

            if (version == null || version == "1")
            {
                this.signed = false;

                if (salt != null && encrypted != null)
                {
                    bool error = false;
                    do
                    {
                        error = false;

                        string password = this.main.GetPassword(Os.GetFileNameWithoutExtension(this.FileName));
                        if (password != null)
                        {
                            try
                            {
                                this.salt = Encrypt.SetSalt(salt);
                                string decryptedXml = Encrypt.DecryptStringAES(encrypted, password, this.salt, "2");
                                diagram = this.StringToXml(decryptedXml);
                                this.encrypted = true;
                                this.SetPassword(password);
                                this.LoadInnerXML(diagram);
                            }
                            catch (Exception e)
                            {
                                // probably invalid password
                                Program.log.Write("LoadXML: Password or file is invalid: " + e.Message);
                                error = true;
                            }
                        }
                        else
                        {
                            // password dialog is cancled
                            this.ResetDefaultValuesForNewFile();
                            return false;
                        }

                    } while (error);
                } else {
                    diagram = new XElement("diagram");

                    if (option != null) {
                        diagram.Add(option);
                    }

                    if (rectangles != null) {
                        diagram.Add(rectangles);
                    }

                    if (lines != null) {
                        diagram.Add(lines);
                    }

                    this.LoadInnerXML(diagram);
                }

                return true;
            }

            if (version == "2" || version == "3")
            {

                if (salt != null && encrypted != null) {
                    bool error = false;
                    do
                    {
                        error = false;

                        string password = this.main.GetPassword(Os.GetFileNameWithoutExtension(this.FileName));
                        if (password != null)
                        {
                            try
                            {
                                this.salt = Encrypt.SetSalt(salt);
                                string decryptedXml = Encrypt.DecryptStringAES(encrypted, password, this.salt, version);
                                diagram = this.StringToXml(decryptedXml);
                                this.encrypted = true;
                                this.SetPassword(password);

                            }
                            catch (Exception e)
                            {
                                // probably invalid password
                                Program.log.Write("LoadXML: Password or file is invalid: " + e.Message);
                                error = true;
                            }
                        }
                        else
                        {
                            // password dialog is cancled
                            this.ResetDefaultValuesForNewFile();
                            return false;
                        }

                    } while (error);
                } 
                
                if(diagram != null) {
                    this.LoadInnerXML(diagram);
                    
                    string diagraxml = this.XmlToString(diagram);
                    this.signed = Signature.CheckSignature(this.main.programOptions.signatureSecret, signature, diagraxml, signatureIV);

                    Program.log.Write("Document is: " + (this.signed ? "SIGNED" : "UNSIGNED"));

                    this.main.plugins.LoadAction(this, diagram);
                }

                return true;
            }

            return true;
        }

        // XML LOAD inner part of diagram file. If file is invalid return false UID3586094034
        public bool LoadInnerXML(XElement root)
        {
            string FontDefaultString = Fonts.FontToString(this.FontDefault);

            Nodes nodes = new Nodes();
            Lines lines = new Lines();

            try
            {
                foreach (XElement diagram in root.Elements())
                {
                    if (diagram.HasElements)
                    {

                        if (diagram.Name.ToString() == "option") // [options] [config]
                        {
                            this.LoadInnerXmlOptions(diagram, FontDefaultString);
                        }

                        if (diagram.Name.ToString() == "rectangles")
                        {
                            this.LoadInnerXmlRectangles(nodes, diagram, FontDefaultString);
                        }

                        if (diagram.Name.ToString() == "lines")
                        {
                            this.LoadInnerXmlLines(lines, nodes, diagram);
                        }

                        if (diagram.Name.ToString() == "data")
                        {
                            this.dataStorage.fromXml(diagram);
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(Translations.fileHasWrongFormat);
                Program.log.Write("load xml error: " + ex.Message);
                this.ResetDefaultValuesForNewFile();
                return false;
            }

            decimal newWidth = 0;
            decimal newHeight = 0;

            Nodes nodesReordered = new Nodes(); // order nodes parent first (layer must exist when sub node is created)
            this.NodesReorderNodes(0, null, nodes, nodesReordered);

            foreach (Node rec in nodesReordered)
            {
                if (!rec.isimage)
                {
                    SizeF s = rec.Measure();
                    newWidth = (decimal)s.Width;
                    newHeight = (decimal)s.Height;

                    rec.Resize();

                }

                this.layers.AddNode(rec);
            }

            this.layers.SetLayersParentsReferences();

            foreach (Line line in lines)
            {
                this.Connect(
                    this.layers.GetNode(line.start),
                    this.layers.GetNode(line.end),
                    line.arrow,
                    line.color,
                    line.width
                );
            }

            return true;
        }

        public void LoadInnerXmlOptions(XElement diagram, string FontDefaultString)
        {
            foreach (XElement el in diagram.Descendants())
            {
                try
                {
                    if (el.Name.ToString() == "shiftx")
                    {
                        this.options.homePosition.x = Tools.StringToDecimal(el.Value);
                    }

                    if (el.Name.ToString() == "shifty")
                    {
                        this.options.homePosition.y = Tools.StringToDecimal(el.Value);
                    }

                    if (el.Name.ToString() == "scale")
                    {
                        this.options.homeScale = Tools.StringToDecimal(el.Value);
                    }

                    if (el.Name.ToString() == "homelayer")
                    {
                        options.homeLayer = Int64.Parse(el.Value);
                    }

                    if (el.Name.ToString() == "endlayer")
                    {
                        options.endLayer = Int64.Parse(el.Value);
                    }

                    if (el.Name.ToString() == "endPositionx")
                    {
                        this.options.endPosition.x = Tools.StringToDecimal(el.Value);
                    }

                    if (el.Name.ToString() == "endPositiony")
                    {
                        this.options.endPosition.y = Tools.StringToDecimal(el.Value);
                    }

                    if (el.Name.ToString() == "endScale")
                    {
                        this.options.homeScale = Tools.StringToDecimal(el.Value);
                    }

                    if (el.Name.ToString() == "startShiftX")
                    {
                        options.homePosition.x = Tools.StringToDecimal(el.Value);
                    }

                    if (el.Name.ToString() == "startShiftY")
                    {
                        options.homePosition.y = Tools.StringToDecimal(el.Value);
                    }

                    if (el.Name.ToString() == "diagramreadonly")
                    {
                        this.options.readOnly = bool.Parse(el.Value);
                    }

                    if (el.Name.ToString() == "grid")
                    {
                        this.options.grid = bool.Parse(el.Value);
                    }

                    if (el.Name.ToString() == "borders")
                    {
                        this.options.borders = bool.Parse(el.Value);
                    }

                    if (el.Name.ToString() == "defaultfont")
                    {
                        if (el.Attribute("type").Value == "font")
                        {
                            this.FontDefault = Fonts.XmlToFont(el);
                        }
                        else
                        {
                            if (FontDefaultString != el.Value)
                            {
                                this.FontDefault = Fonts.StringToFont(el.Value);
                            }
                        }
                    }

                    if (el.Name.ToString() == "coordinates")
                    {
                        this.options.coordinates = bool.Parse(el.Value);
                    }

                    if (el.Name.ToString() == "firstLayereShift.x")
                    {
                        this.options.firstLayereShift.x = Tools.StringToDecimal(el.Value);
                    }

                    if (el.Name.ToString() == "firstLayereShift.y")
                    {
                        this.options.firstLayereShift.y = Tools.StringToDecimal(el.Value);
                    }

                    if (el.Name.ToString() == "firstLayereScale")
                    {
                        this.options.firstLayereScale = Tools.StringToDecimal(el.Value);
                    }

                    if (el.Name.ToString() == "openLayerInNewView")
                    {
                        this.options.openLayerInNewView = bool.Parse(el.Value);
                    }

                    if (el.Name.ToString() == "window.position.restore")
                    {
                        this.options.restoreWindow = bool.Parse(el.Value);
                    }

                    if (el.Name.ToString() == "window.position.x")
                    {
                        this.options.Left = Int64.Parse(el.Value);
                    }

                    if (el.Name.ToString() == "window.position.y")
                    {
                        this.options.Top = Int64.Parse(el.Value);
                    }

                    if (el.Name.ToString() == "window.position.width")
                    {
                        this.options.Width = Int64.Parse(el.Value);
                    }

                    if (el.Name.ToString() == "window.position.height")
                    {
                        this.options.Height = Int64.Parse(el.Value);
                    }

                    if (el.Name.ToString() == "window.state")
                    {
                        this.options.WindowState = Int64.Parse(el.Value);
                    }

                    if (el.Name.ToString() == "icon")
                    {
                        this.options.icon = el.Value;
                    }

                    if (el.Name.ToString() == "lineColor")
                    {
                        this.options.lineColor.Set(el.Value.ToString());
                    }

                    if (el.Name.ToString() == "nodeColor")
                    {
                        this.options.nodeColor.Set(el.Value.ToString());
                    }

                    if (el.Name.ToString() == "selectedNodeColor")
                    {
                        this.options.selectedNodeColor.Set(el.Value.ToString());
                    }

                    if (el.Name.ToString() == "backgroundColor")
                    {
                        this.options.backgroundColor.Set(el.Value.ToString());
                    }

                    if (el.Name.ToString() == "gridColor")
                    {
                        this.options.gridColor.Set(el.Value.ToString());
                    }

                    if (el.Name.ToString() == "scrollbarColor")
                    {
                        this.options.scrollbarColor.Set(el.Value.ToString());
                    }

                    if (el.Name.ToString() == "selectionColor")
                    {
                        this.options.selectionColor.Set(el.Value.ToString());
                    }

                    if (el.Name.ToString() == "backgroundImage")
                    {
                        this.options.backgroundImage = Media.StringToImage(el.Value);
                    }

                }
                catch (Exception ex)
                {
                    Program.log.Write("load xml diagram options: " + ex.Message);
                }
            }
        }

        public void LoadInnerXmlRectangles(Nodes nodes, XElement diagram, string FontDefaultString)
        {
            foreach (XElement block in diagram.Descendants())
            {

                if (block.Name.ToString() == "rectangle")
                {
                    Node R = new Node
                    {
                        font = this.FontDefault
                    };

                    foreach (XElement el in block.Descendants())
                    {
                        try
                        {
                            if (el.Name.ToString() == "id")
                            {
                                R.id = Int64.Parse(el.Value);
                            }

                            if (el.Name.ToString() == "font")
                            {
                                if (el.Attribute("type").Value == "font")
                                {
                                    R.font = Fonts.XmlToFont(el);
                                }
                                else
                                {
                                    if (FontDefaultString != el.Value)
                                    {
                                        R.font = Fonts.StringToFont(el.Value);
                                    }
                                }
                            }

                            if (el.Name.ToString() == "fontcolor")
                            {
                                R.fontcolor.Set(el.Value.ToString());
                            }

                            if (el.Name.ToString() == "text")
                            {
                                R.name = el.Value;
                            }


                            if (el.Name.ToString() == "note")
                            {
                                R.note = el.Value;
                            }


                            if (el.Name.ToString() == "link")
                            {
                                R.link = el.Value;
                            }

                            if (el.Name.ToString() == "scriptid")
                            {
                                R.scriptid = el.Value;
                            }

                            if (el.Name.ToString() == "shortcut")
                            {
                                R.shortcut = Int64.Parse(el.Value);
                            }

                            if (el.Name.ToString() == "mark")
                            {
                                R.mark = bool.Parse(el.Value);
                            }

                            if (el.Name.ToString() == "attachment")
                            {
                                R.attachment = el.Value;
                            }

                            if (el.Name.ToString() == "layer")
                            {
                                R.layer = Int64.Parse(el.Value);
                            }

                            if (el.Name.ToString() == "haslayer")
                            {
                                R.haslayer = bool.Parse(el.Value);
                            }

                            if (el.Name.ToString() == "layershiftx")
                            {
                                R.layerShift.x = Tools.StringToDecimal(el.Value);
                            }

                            if (el.Name.ToString() == "layershifty")
                            {
                                R.layerShift.y = Tools.StringToDecimal(el.Value);
                            }

                            if (el.Name.ToString() == "layerscale")
                            {
                                R.layerScale = Tools.StringToDecimal(el.Value);
                            }

                            if (el.Name.ToString() == "x")
                            {
                                R.position.x = Tools.StringToDecimal(el.Value);
                            }

                            if (el.Name.ToString() == "y")
                            {
                                R.position.y = Tools.StringToDecimal(el.Value);
                            }

                            if (el.Name.ToString() == "width")
                            {
                                R.width = Tools.StringToDecimal(el.Value);
                            }

                            if (el.Name.ToString() == "height")
                            {
                                R.height = Tools.StringToDecimal(el.Value);
                            }

                            if (el.Name.ToString() == "scale")
                            {
                                R.scale = Tools.StringToDecimal(el.Value);
                            }

                            if (el.Name.ToString() == "color")
                            {
                                R.color.Set(el.Value.ToString());
                            }

                            if (el.Name.ToString() == "transparent")
                            {
                                R.transparent = bool.Parse(el.Value);
                            }

                            if (el.Name.ToString() == "embeddedimage")
                            {
                                R.embeddedimage = bool.Parse(el.Value);
                            }

                            if (el.Name.ToString() == "imagedata")
                            {
                                R.image = Media.StringToBitmap(el.Value);
                                R.height = R.image.Height;
                                R.width = R.image.Width;
                                R.isimage = true;
                            }

                            if (el.Name.ToString() == "image")
                            {
                                R.imagepath = el.Value.ToString();
                                R.LoadImage();
                            }


                            if (el.Name.ToString() == "timecreate")
                            {
                                R.timecreate = el.Value;
                            }


                            if (el.Name.ToString() == "timemodify")
                            {
                                R.timemodify = el.Value;
                            }

                            if (el.Name.ToString() == "protect")
                            {
                                R.protect = bool.Parse(el.Value);
                            }

                            if (el.Name.ToString() == "data")
                            {
                                R.dataStorage.fromXml(el);
                            }

                        }
                        catch (Exception ex)
                        {
                            Program.log.Write("load xml nodes error: " + ex.Message);
                        }
                    }
                    nodes.Add(R);
                }
            }
        }

        public void LoadInnerXmlLines(Lines lines, Nodes nodes, XElement diagram)
        {
            foreach (XElement block in diagram.Descendants())
            {
                if (block.Name.ToString() == "line")
                {
                    Line L = new Line
                    {
                        layer = -1 // for identification unset layers
                    };

                    foreach (XElement el in block.Descendants())
                    {
                        try
                        {
                            if (el.Name.ToString() == "start")
                            {
                                L.start = Int64.Parse(el.Value);
                            }

                            if (el.Name.ToString() == "end")
                            {
                                L.end = Int64.Parse(el.Value);
                            }

                            if (el.Name.ToString() == "scale")
                            {
                                L.scale = Tools.StringToDecimal(el.Value);
                            }

                            if (el.Name.ToString() == "arrow")
                            {
                                L.arrow = el.Value == "1" ? true : false;
                            }

                            if (el.Name.ToString() == "color")
                            {
                                L.color.Set(el.Value.ToString());
                            }

                            if (el.Name.ToString() == "width")
                            {
                                L.width = Int64.Parse(el.Value);
                            }

                            if (el.Name.ToString() == "layer")
                            {
                                L.layer = Int64.Parse(el.Value);
                            }

                            if (el.Name.ToString() == "data")
                            {
                                L.dataStorage.fromXml(el);
                            }

                        }
                        catch (Exception ex)
                        {
                            Program.log.Write("load xml lines error: " + ex.Message);
                        }
                    }

                    if (L.start <= 0)
                    {
                        continue;
                    }


                    Node startNode = nodes.Find(L.start);
                    if (startNode == null)
                    {
                        continue;
                    }

                    L.startNode = startNode;

                    if (L.end <= 0)
                    {
                        continue;
                    }


                    Node endNode = nodes.Find(L.end);
                    if (endNode == null)
                    {
                        continue;
                    }


                    L.endNode = endNode;

                    lines.Add(L);
                }
            }
        }

        /*************************************************************************************************************************/
        // SAVE DIAGRAM

        // save diagram
        public bool Save() //UID8354947577
        {
            if (this.IsReadOnly())
            {
                return false;
            }

            if (this.IsLocked() || this.FileName == "" || !Os.FileExists(this.FileName))
            {
                return false;
            }

            this.SaveXMLFile(this.FileName);
            this.NewFile = false;
            this.SavedFile = true;
            this.undoOperations.RememberSave();
            this.SetTitle();
            return true;
        }

        // save diagram as
        public void Saveas(String FileName) //UID9358805584
        {
            if (this.IsReadOnly())
            {
                return;
            }

            if (this.IsLocked())
            {
                return;
            }

            this.SaveXMLFile(FileName);
            this.FileName = FileName;
            this.SavedFile = true;
            this.NewFile = false;

            this.SetTitle();
        }

        // XML SAVE file or encrypted file
        public void SaveXMLFile(string FileName) //UID6023051509
        {
            XElement root = new XElement("diagram");
            root.Add(new XElement("version", "3"));

            XElement diagram = this.SaveInnerXMLFile();
            string diagraxml = this.XmlToString(diagram);

            // encrypt data if password is set
            try
            {

                if (this.password != null)
                {
                    // encrypted file is saved allways as different string
                    this.salt = Encrypt.CreateSalt(14);

                    root.Add(new XElement("encrypted", Encrypt.EncryptStringAES(diagraxml, this.password, this.salt)));
                    root.Add(new XElement("salt", Encrypt.GetSalt(this.salt)));
                }
                else
                {
                    root.Add(diagram);
                }

            }
            catch (Exception ex)
            {
                Program.log.Write("save file error: " + ex.Message);
            }

            if (this.signed) {
                string signatureIV = this.main.programOptions.signatureIV;

                if (this.password != null) {
                    signatureIV = Signature.GenerateIV();
                    root.Add(new XElement("signatureIV", signatureIV));
                }

                string signature = Signature.SignText(this.main.programOptions.signatureSecret, diagraxml, signatureIV);
                root.Add(new XElement("signature", signature));
            }

            // save data to file
            try
            {
                if (diagraxml != "") { // prevent acidentaly loos data
                    System.IO.StreamWriter file = new System.IO.StreamWriter(FileName);
                    file.Write(this.XmlToString(root));
                    file.Close();
                }

            }
            catch (System.IO.IOException ex)
            {
                Program.log.Write("save file io error: " + ex.Message);
                MessageBox.Show(Translations.fileIsLocked);
            }
            catch (Exception ex)
            {
                Program.log.Write("save file error: " + ex.Message);
            }
        }

        // XML SAVE create xml from current diagram file state
        public XElement SaveInnerXMLFile() //UID8716692347
        {
            XElement diagram = new XElement("diagram");
            diagram.Add(new XElement("version", "1"));

            try
            {
                this.main.plugins.SaveAction(this, diagram);

                XElement options = this.SaveInnerXmlOptions();
                XElement rectangles = this.SaveInnerXmlNodes(this.GetAllNodes());
                XElement lines = this.SaveInnerXmlLines(this.GetAllLines());
                XElement dataStorage = this.dataStorage.toXml(new XElement("data"));

                diagram.Add(options);
                diagram.Add(rectangles);
                diagram.Add(lines);
                diagram.Add(dataStorage);
            }
            catch (Exception ex)
            {
                Program.log.Write("save xml error: " + ex.Message);
            }

            return diagram;
        }

        public XElement SaveInnerXmlOptions() //UID8029528026
        {
            // [options] [config]
            XElement option = new XElement("option");
            option.Add(new XElement("shiftx", this.options.homePosition.x));
            option.Add(new XElement("shifty", this.options.homePosition.y));
            option.Add(new XElement("scale", this.options.homeScale));
            option.Add(new XElement("endPositionx", this.options.endPosition.x));
            option.Add(new XElement("endPositiony", this.options.endPosition.y));
            option.Add(new XElement("endScale", this.options.endScale));
            option.Add(new XElement("firstLayereShift.x", this.options.firstLayereShift.x));
            option.Add(new XElement("firstLayereShift.y", this.options.firstLayereShift.y));
            option.Add(new XElement("firstLayereScale", this.options.firstLayereScale));
            if (this.options.openLayerInNewView) option.Add(new XElement("openLayerInNewView", this.options.openLayerInNewView));
            option.Add(new XElement("homelayer", this.options.homeLayer));
            option.Add(new XElement("endlayer", this.options.endLayer));
            option.Add(new XElement("diagramreadonly", this.options.readOnly));
            option.Add(new XElement("grid", this.options.grid));
            option.Add(new XElement("borders", this.options.borders));
            option.Add(Fonts.FontToXml(this.FontDefault, "defaultfont"));
            option.Add(new XElement("coordinates", this.options.coordinates));
            option.Add(new XElement("window.position.restore", this.options.restoreWindow));
            option.Add(new XElement("window.position.x", this.options.Left));
            option.Add(new XElement("window.position.y", this.options.Top));
            option.Add(new XElement("window.position.width", this.options.Width));
            option.Add(new XElement("window.position.height", this.options.Height));
            option.Add(new XElement("window.state", this.options.WindowState));
            option.Add(new XElement("lineColor", this.options.lineColor.ToString()));
            option.Add(new XElement("nodeColor", this.options.nodeColor.ToString()));
            option.Add(new XElement("selectedNodeColor", this.options.selectedNodeColor.ToString()));
            option.Add(new XElement("backgroundColor", this.options.backgroundColor.ToString()));
            option.Add(new XElement("gridColor", this.options.gridColor.ToString()));
            option.Add(new XElement("scrollbarColor", this.options.scrollbarColor.ToString()));
            option.Add(new XElement("selectionColor", this.options.selectionColor.ToString()));

            if (this.options.icon != "")
            {
                option.Add(new XElement("icon", this.options.icon));
            }

            if (this.options.backgroundImage != null)
            {
                option.Add(new XElement(
                    "backgroundImage", 
                    Media.ImageToString(this.options.backgroundImage)
                    )
                );
            }
            
            return option;
        }
        
        public XElement SaveInnerXmlNodes(Nodes nodes) //UID0137352615
        {
            XElement rectangles = new XElement("rectangles");
            foreach (Node rec in nodes)
            {
                XElement rectangle = new XElement("rectangle");
                rectangle.Add(new XElement("id", rec.id));
                if (!Fonts.Compare(this.FontDefault, rec.font))
                {
                    rectangle.Add(Fonts.FontToXml(rec.font));
                }
                rectangle.Add(new XElement("fontcolor", rec.fontcolor));
                if (rec.name != "") rectangle.Add(new XElement("text", rec.name));
                if (rec.note != "") rectangle.Add(new XElement("note", rec.note));
                if (rec.link != "") rectangle.Add(new XElement("link", rec.link));
                if (rec.scriptid != "") rectangle.Add(new XElement("scriptid", rec.scriptid));
                if (rec.shortcut != 0) rectangle.Add(new XElement("shortcut", rec.shortcut));
                if (rec.mark) rectangle.Add(new XElement("mark", rec.mark));
                if (rec.attachment != "") rectangle.Add(new XElement("attachment", rec.attachment));

                rectangle.Add(new XElement("layer", rec.layer));

                if (rec.haslayer)
                {
                    rectangle.Add(new XElement("haslayer", rec.haslayer));
                    rectangle.Add(new XElement("layershiftx", rec.layerShift.x));
                    rectangle.Add(new XElement("layershifty", rec.layerShift.y));
                    rectangle.Add(new XElement("layerscale", rec.layerScale));
                }

                rectangle.Add(new XElement("x", rec.position.x));
                rectangle.Add(new XElement("y", rec.position.y));
                rectangle.Add(new XElement("width", rec.width));
                rectangle.Add(new XElement("height", rec.height));
                rectangle.Add(new XElement("scale", rec.scale));
                rectangle.Add(new XElement("color", rec.color));
                if (rec.transparent) rectangle.Add(new XElement("transparent", rec.transparent));
                if (rec.embeddedimage) rectangle.Add(new XElement("embeddedimage", rec.embeddedimage));

                if (rec.embeddedimage && rec.image != null) // image is inserted directly to file
                {
                    rectangle.Add(new XElement("imagedata", Media.BitmapToString(rec.image)));
                }
                else if (rec.imagepath != "")
                {
                    rectangle.Add(new XElement("image", rec.imagepath));
                }

                if (rec.protect) rectangle.Add(new XElement("protect", rec.protect));

                rectangle.Add(new XElement("timecreate", rec.timecreate));
                rectangle.Add(new XElement("timemodify", rec.timemodify));

                rectangles.Add(rectangle);

                rectangle.Add(rec.dataStorage.toXml(new XElement("data")));
            }
            
            return rectangles;
        }
        
        public XElement SaveInnerXmlLines(Lines lines) //UID2182227651
        {
            XElement xlines = new XElement("lines");
            foreach (Line lin in lines)
            {
                XElement line = new XElement("line");
                line.Add(new XElement("start", lin.start));
                line.Add(new XElement("end", lin.end));
                line.Add(new XElement("scale", lin.scale));
                line.Add(new XElement("arrow", (lin.arrow) ? "1" : "0"));
                line.Add(new XElement("color", lin.color));
                if (lin.width != 1) line.Add(new XElement("width", lin.width));
                line.Add(new XElement("layer", lin.layer));
                line.Add(lin.dataStorage.toXml(new XElement("data")));
                xlines.Add(line);
            }

            return xlines;
        }

        public string XmlToString(XElement root) //UID8716692347
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                XmlWriterSettings xws = new XmlWriterSettings
                {
                    OmitXmlDeclaration = true,
                    CheckCharacters = false
                };

                using (XmlWriter xw = XmlWriter.Create(sb, xws))
                {
                    root.WriteTo(xw);
                }

                return sb.ToString();

            }
            catch (Exception ex)
            {
                Program.log.Write("write xml to file error: " + ex.Message);
            }

            return "";
        }

        public XElement StringToXml(string xml) //UID8716692347
        {
            try
            {
                XmlReaderSettings xws = new XmlReaderSettings
                {
                    CheckCharacters = false
                };

                using (XmlReader xr = XmlReader.Create(new StringReader(xml), xws))
                {
                    return XElement.Load(xr);
                }
            }
            catch (Exception ex)
            {
                Program.log.Write("StringToXml error: " + ex.Message);
            }

            return null;
        }


        /*************************************************************************************************************************/
        // CLOSE DIAGRAM

        // set default options for file like new file 
        public void ResetDefaultValuesForNewFile() //UID3849853197
        {
            // Prednadstavenie atributov
            this.NewFile = true;
            this.SavedFile = true;
            this.FileName = "";

            // clear nodes and lines lists
            this.layers.Clear();

            this.options.readOnly = false;
            this.options.grid = true;
            this.options.coordinates = false;

            this.TextWindows.Clear();
        }

        /*************************************************************************************************************************/
        // STATES

        // check if file is empty
        public bool IsNew()
        {
            return (this.FileName == "" && this.NewFile && this.SavedFile);
        }

        // check if file is empty
        public bool IsReadOnly()
        {
            return this.options.readOnly || this.locked;
        }

        /*************************************************************************************************************************/
        // UNSAVE

        public void Unsave(string type, Node node, Position position = null, decimal scale = 0, long layer = 0)
        {
            Nodes nodes = new Nodes
            {
                node
            };
            this.Unsave(type, nodes, null, position, scale, layer);
        }

        public void Unsave(string type, Line line, Position position = null, decimal scale = 0, long layer = 0)
        {
            Lines lines = new Lines
            {
                line
            };
            this.Unsave(type, null, lines, position, scale, layer);
        }

        public void Unsave(string type, Node node, Line line, Position position = null, decimal scale = 0, long layer = 0)
        {
            Nodes nodes = new Nodes
            {
                node
            };
            Lines lines = new Lines
            {
                line
            };
            this.Unsave(type, nodes, lines, position, scale, layer);
        }

        public void Unsave(string type, Nodes nodes = null, Lines lines = null, Position position = null, decimal scale = 0, long layer = 0)
        {
            this.undoOperations.RememberSave();
            this.undoOperations.Add(type, nodes, lines, position, scale, layer);
            this.Unsave();
        }

        public void Unsave()
        {
            this.SavedFile = false;
            this.SetTitle();

            this.InvalidateDiagram();
        }

        public void Restoresave()
        {
            this.SavedFile = true;
            this.SetTitle();

            this.InvalidateDiagram();
        }

        /*************************************************************************************************************************/
        // UNDO

        // undo
        public void DoUndo(DiagramView view = null)
        {
            this.undoOperations.DoUndo(view);
        }

        // redo
        public void DoRedo(DiagramView view = null)
        {
            this.undoOperations.DoRedo(view);
        }

        /*************************************************************************************************************************/
        // NODES SELECT

        // NODE find node by id
        public Node GetNodeByID(long id)
        {
            return this.layers.GetNode(id);
        }

        // NODE find first node by link
        public Node GetNodeByLink(string text)
        {
            foreach (Node rec in this.GetAllNodes()) // Loop through List with foreach
            {
                if (rec.link.Trim() == text)
                {
                    return rec;
                }
            }

            return null;
        }

        public Nodes GetAllNodes()
        {
            return this.layers.GetAllNodes();
        }

        public Lines GetAllLines()
        {
            return this.layers.GetAllLines();
        }     

        // NODE Najdenie nody podla pozicie myši
        public Node FindNodeInPosition(Position position, long layer, Node skipNode = null)
        {
            decimal scale;

            foreach (Node node in this.layers.GetLayer(layer).nodes.Reverse<Node>()) // Loop through List with foreach
            {
                if (layer == node.layer || layer == node.id)
                {
                    scale = Tools.GetScale(node.scale);
                    if
                    (                       
                        node.position.x <= position.x && position.x <= node.position.x + (node.width * scale) &&
                        node.position.y <= position.y && position.y <= node.position.y + (node.height * scale) &&
                        (skipNode == null || skipNode.id != node.id)
                    )
                    {
                        return node;
                    }
                }
            }

            return null;
        }


        /*************************************************************************************************************************/
        // NODES DELETE

        // NODE delete all nodes which is not in layer history UID0424081301
        public bool CanDeleteNode(Node node)
        {
            // sub node is viewed
            foreach (DiagramView view in this.DiagramViews)
            {
                if (view.IsNodeInLayerHistory(node))
                {
                    return false;
                }
            }

            return true;
        }

        // NODE delete node
        public void DeleteNode(Node rec) //UID6856126776
        {
            if (rec != null && !this.IsReadOnly())
            {
                foreach (DiagramView diagramView in this.DiagramViews) //remove node from selected nodes in views
                {
                    diagramView.RemoveNodeFromSelection(rec);
                }

                if (this.TextWindows.Count() > 0) // close text edit to node
                {
                    for (int i = this.TextWindows.Count() - 1; i >= 0; i--)
                    {
                        if (this.TextWindows[i].node == rec)
                        {
                            this.TextWindows[i].Close();
                            break;
                        }
                    }
                }

                this.layers.RemoveNode(rec);
            }
        }

        // NODE delete multiple nodes and set undo operation UID3218416151
        public void DeleteNodes(Nodes nodes, Position position = null, long layer = 0)
        {
            bool canDelete = false;

            Nodes toDeleteNodes = new Nodes();
            Lines toDeleteLines = new Lines();

            this.layers.GetAllNodesAndLines(nodes, ref toDeleteNodes, ref toDeleteLines);

            foreach (Node node in nodes)
            {
                if (this.CanDeleteNode(node))
                {
                    canDelete = true;
                }
            }

            if (canDelete)
            {
                this.undoOperations.Add("delete", toDeleteNodes, toDeleteLines, position, layer);

                foreach (Node node in toDeleteNodes.Reverse<Node>()) // remove lines to node
                {
                    this.DeleteNode(node);
                }

                this.InvalidateDiagram();
                this.Unsave();
            }
        }

        /*************************************************************************************************************************/
        // NODES EDIT

        // NODE Editovanie vlastnosti nody
        public TextForm EditNode(Node rec)
        {
            bool found = false;
            for (int i = TextWindows.Count() - 1; i >= 0; i--) // Loop through List with foreach
            {
                if (TextWindows[i].node == rec)
                {
                    Media.BringToFront(TextWindows[i]);
                    found = true;
                    return TextWindows[i];
                }
            }

            if (!found) {
                TextForm textf = new TextForm(this.main);
                textf.SetDiagram(this);
                textf.node = rec;
                string[] lines = rec.name.Split(Environment.NewLine.ToCharArray()).ToArray();
                if(lines.Count()>0)
                    textf.Text = lines[0];

                this.TextWindows.Add(textf);
                this.main.AddTextWindow(textf);
                textf.Show();
                Media.BringToFront(textf);
                return textf;
            }
            return null;
        }

        // NODE Editovanie vlastnosti nody
        public void EditNodeClose(Node rec)
        {
            for (int i = TextWindows.Count() - 1; i >= 0; i--) // Loop through List with foreach
            {
                if (TextWindows[i].node == rec)
                {
                    TextWindows.RemoveAt(i);
                }
            }
        }

        /*************************************************************************************************************************/
        // NODES CREATE

        // NODE Create Rectangle on point
        public Node CreateNode(
            Position position,
            string name = "",
            long layer = 0,
            ColorType color = null,
            Font font = null
        ) {
            var rec = new Node();
            if (font == null)
            {
                rec.font = this.FontDefault;
            }
            else
            {
                rec.font = font;
            }

            rec.layer = layer;

            rec.SetName(name);
            rec.note = "";
            rec.link = "";

            rec.position.Set(position);

            if (color != null)
            {
                rec.color.Set(color);
            }
            else
            {
                rec.color.Set(this.options.nodeColor.Get());
            }
            
            rec.scale = 1;

            return this.layers.CreateNode(rec);
        }

        // NODE add node to diagram (create new id and layer if not exist) 
        public Node CreateNode(Node node)
        {
            return this.layers.CreateNode(node);
        }

        /*************************************************************************************************************************/
        // LINES SELECT

        // LINE HASLINE check if line exist between two nodes
        public bool HasConnection(Node a, Node b)
        {
            Line line = this.layers.GetLine(a, b);
            return line != null;
        }

        public Line GetLine(Node a, Node b)
        {
            return this.layers.GetLine(a, b);
        }

        public Line GetLine(long a, long b)
        {
            return this.layers.GetLine(a, b);
        }

        // LINE CONNECT connect two nodes
        public Line Connect(Node a, Node b)
        {
            if (a != null && b != null)
            {
                Line line = this.layers.GetLine(a, b);

                if (line == null)
                {

                    // calculate line layer from node layers
                    long layer;
                    if (a.layer == b.layer) // nodes are in same layer
                    {
                        layer = a.layer;
                    }
                    else
                    if (a.layer == b.id) // b is perent of a
                    {
                        layer = a.layer;
                    }
                    else
                    if (b.layer == a.id) // a is perent of b
                    {
                        layer = b.layer;
                    }
                    else
                    {
                        return null; // invalid connection (nodes are not related or in same layer)
                    }

                    line = new Line {
                        start = a.id,
                        end = b.id,
                        scale = a.scale,
                        startNode = this.GetNodeByID(a.id),
                        endNode = this.GetNodeByID(b.id),
                        layer = layer,
                        color = new ColorType(this.options.lineColor)
                    };

                    this.layers.AddLine(line);

                    return line;
                }
            }

            return null;
        }

        // LINE DISCONNECT remove connection between two nodes
        public void Disconnect(Node a, Node b)
        {
            if (a != null && b != null)
            {
                Line line = this.layers.GetLine(a, b);

                if (line != null)
                {
                    this.layers.RemoveLine(line);
                }
            }
        }

        // LINE CONNECT connect two nodes and add arrow or set color
        public Line Connect(Node a, Node b, bool arrow = false, ColorType color = null, long width = 1)
        {
            Line line = this.Connect(a, b);

            if (line != null)
            {
                line.arrow = arrow;
                if (color != null)
                {
                    line.color.Set(color);
                }
                
                line.width = width;
            }

            return line;
        }

        /*************************************************************************************************************************/
        // ALIGN

        // align to line
        public void AlignToLine(Nodes Nodes)
        {
            if (Nodes.Count() > 0)
            {
                decimal minx = Nodes[0].position.x;
                decimal topy = Nodes[0].position.y;
                foreach (Node rec in Nodes)
                {
                    if (rec.position.x <= minx)
                    {
                        minx = rec.position.x;
                        topy = rec.position.y + rec.height / 2;
                    }
                }

                foreach (Node rec in Nodes)
                {
                    rec.position.y = topy - rec.height / 2;
                }
            }
        }

        // align node to top element and create constant space between nodes
        public void AlignCompact(Nodes nodes)
        {
            if (nodes.Count() > 0)
            {
                decimal minx = nodes[0].position.x;
                decimal miny = nodes[0].position.y;
                foreach (Node rec in nodes)
                {
                    if (rec.position.y <= miny) // find most top element
                    {
                        miny = rec.position.y;
                        minx = rec.position.x;
                    }
                }

                foreach (Node rec in nodes) // align to left
                {
                    rec.position.x = minx;
                }

                // sort elements by y coordinate
                nodes.OrderByPositionY();

                decimal posy = miny;
                foreach (Node rec in nodes) // change space between nodes
                {
                    rec.position.y = posy;
                    decimal s = Tools.GetScale(rec.scale);
                    posy = posy + rec.height * s + 10 * s;
                }
            }
        }

        // NODES ALIGN to column
        public void AlignToColumn(Nodes Nodes)
        {
            if (Nodes.Count() > 0)
            {
                decimal miny = Nodes[0].position.y;
                decimal topx = Nodes[0].position.x;
                foreach (Node rec in Nodes)
                {
                    if (rec.position.y <= miny)
                    {
                        decimal s = Tools.GetScale(rec.scale);
                        miny = rec.position.y;
                        topx = rec.position.x + rec.width * s / 2; ;
                    }
                }

                foreach (Node rec in Nodes)
                {
                    decimal s = Tools.GetScale(rec.scale);
                    rec.position.x = topx - rec.width * s / 2;
                }
            }
        }

        // align node to most left node and create constant space between nodes
        public void AlignCompactLine(Nodes nodes)
        {
            if (nodes.Count() > 0)
            {
                decimal minx = nodes[0].position.x;
                decimal miny = nodes[0].position.y;
                foreach (Node rec in nodes)
                {
                    if (rec.position.x <= minx) // find top left element
                    {
                        minx = rec.position.x;
                        miny = rec.position.y;
                    }
                }

                foreach (Node rec in nodes) // align to top
                {
                    rec.position.y = miny;
                }

                // sort elements by y coordinate
                nodes.OrderByPositionX();

                decimal posx = minx;
                foreach (Node rec in nodes) // zmensit medzeru medzi objektami
                {
                    decimal s = Tools.GetScale(rec.scale);
                    rec.position.x = posx;
                    posx = posx + rec.width * s + 10 * s;
                }
            }
        }

        // align left
        public void AlignRight(Nodes Nodes)
        {
            if (Nodes.Count() > 0)
            {
                decimal maxx = Nodes[0].position.x + Nodes[0].width;
                foreach (Node rec in Nodes)
                {
                    if (rec.position.x + rec.width >= maxx)
                    {
                        decimal s = Tools.GetScale(rec.scale);
                        maxx = rec.position.x + rec.width * s;
                    }
                }

                foreach (Node rec in Nodes)
                {
                    decimal s = Tools.GetScale(rec.scale);
                    rec.position.x = maxx - rec.width * s;
                }
            }
        }

        // align left
        public void AlignLeft(Nodes Nodes)
        {
            if (Nodes.Count() > 0)
            {
                decimal minx = Nodes[0].position.x;
                foreach (Node rec in Nodes)
                {
                    if (rec.position.x <= minx)
                    {
                        minx = rec.position.x;
                    }
                }

                foreach (Node rec in Nodes)
                {
                    rec.position.x = minx;
                }
            }
        }

        // align node to top element and create constant space between nodes and sort items
        public void SortNodes(Nodes nodes)
        {
            if (nodes.Count() > 0)
            {
                decimal minx = nodes[0].position.x;
                decimal miny = nodes[0].position.y;
                foreach (Node rec in nodes)
                {
                    if (rec.position.y <= miny) // find most top element
                    {
                        minx = rec.position.x;
                        miny = rec.position.y;
                    }
                }

                foreach (Node rec in nodes) // align to left
                {
                    rec.position.x = minx;
                }

                nodes.OrderByPositionY();

                // check if nodes are ordered by name already
                bool alreadyAsc = true;
                for (int i = 0; i < nodes.Count - 1; i++)
                {
                    if (String.Compare(nodes[i + 1].name, nodes[i].name) < 0)
                    {
                        alreadyAsc = false;
                        break;
                    }
                }

                if (alreadyAsc)
                {
                    nodes.OrderByNameDesc();
                }
                else
                {
                    nodes.OrderByNameAsc();
                }

                decimal posy = miny;
                foreach (Node rec in nodes) // change space between nodes
                {
                    rec.position.y = posy;
                    posy = posy + rec.height + 10;
                }
            }
        }

        // split node to lines
        public Nodes SplitNode(Nodes nodes)
        {
            if (nodes.Count() > 0)
            {
                Nodes newNodes = new Nodes();

                decimal minx = nodes[0].position.x;
                decimal miny = nodes[0].position.y;
                foreach (Node rec in nodes)
                {
                    if (rec.position.y <= miny) // find most top element
                    {
                        minx = rec.position.x;
                        miny = rec.position.y;
                    }
                }

                foreach (Node node in nodes) // create new nodes
                {
                    string[] lines = node.name.Split(new string[] { "\n" }, StringSplitOptions.None);

                    decimal posy = node.position.y + node.height + 10;

                    foreach (String line in lines)
                    {
                        if (line.Trim() != "")
                        {
                            Node newNode = this.CreateNode(new Node(node)); // duplicate content of old node
                            newNode.SetName(line);
                            newNode.position.y = posy;
                            posy = posy + newNode.height + 10;
                            newNodes.Add(newNode);
                        }
                    }
                }

                return newNodes;
            }

            return null;
        }

        /*************************************************************************************************************************/
        // SHORTCUTS

        // remove shortcut
        public void RemoveShortcut(Node node)
        {
            if (node.shortcut > 0) node.shortcut = 0;
        }

        // remove mark
        public void RemoveMark(Node node)
        {
            if (node.mark) node.mark = false;
        }

        /*************************************************************************************************************************/
        // OPTIONS
        public void SetLineColor()
        {

            if (this.IsReadOnly()) {
                return;
            }

            ColorDialog lineColorDialog = new ColorDialog();

            if (lineColorDialog.ShowDialog() == DialogResult.OK)
            {
                this.options.lineColor.Set(lineColorDialog.Color);
            }
        }

        public void SetNodeColor()
        {

            if (this.IsReadOnly())
            {
                return;
            }

            ColorDialog NodeColorDialog = new ColorDialog();

            if (NodeColorDialog.ShowDialog() == DialogResult.OK)
            {
                this.options.nodeColor.Set(NodeColorDialog.Color);
            }
        }

        public void SetSelectedNodeColor()
        {

            if (this.IsReadOnly())
            {
                return;
            }

            ColorDialog SelectedNodeColorDialog = new ColorDialog();

            if (SelectedNodeColorDialog.ShowDialog() == DialogResult.OK)
            {
                this.options.selectedNodeColor.Set(SelectedNodeColorDialog.Color);
            }
        }

        public void SetBackgroundColor()
        {

            if (this.IsReadOnly())
            {
                return;
            }

            ColorDialog backgroundColorDialog = new ColorDialog();

            if (backgroundColorDialog.ShowDialog() == DialogResult.OK)
            {
                this.options.backgroundColor.Set(backgroundColorDialog.Color);

                foreach (DiagramView diagramView in this.DiagramViews)
                {
                    diagramView.BackColor = backgroundColorDialog.Color;
                    diagramView.Invalidate();
                }
            }
        }

        public void SetGridColor()
        {

            if (this.IsReadOnly())
            {
                return;
            }

            ColorDialog gridColorDialog = new ColorDialog();

            if (gridColorDialog.ShowDialog() == DialogResult.OK)
            {
                this.options.gridColor.Set(gridColorDialog.Color);

                foreach (DiagramView diagramView in this.DiagramViews)
                {
                    diagramView.Invalidate();
                }
            }
        }

        public void SetScrollbarColor()
        {

            if (this.IsReadOnly())
            {
                return;
            }

            ColorDialog scrollbarColorDialog = new ColorDialog();

            if (scrollbarColorDialog.ShowDialog() == DialogResult.OK)
            {
                this.options.scrollbarColor.Set(scrollbarColorDialog.Color);

                foreach (DiagramView diagramView in this.DiagramViews)
                {
                    diagramView.SetScrollbarColor(this.options.scrollbarColor);
                    diagramView.Invalidate();
                }
            }
        }

        public void SetSelectionColor()
        {

            if (this.IsReadOnly())
            {
                return;
            }

            ColorDialog selectionColorDialog = new ColorDialog();

            if (selectionColorDialog.ShowDialog() == DialogResult.OK)
            {
                this.options.selectionColor.Set(selectionColorDialog.Color);
            }
        }

        /*************************************************************************************************************************/
        // TOOLS

        // FILE Open diagram directory if diagram is already saved
        public void OpenDiagramDirectory()
        {
            if (!this.NewFile && Os.FileExists(this.FileName))
            {
                Os.ShowDirectoryInExternalApplication(Os.GetDirectoryName(this.FileName));
            }
        }

        /*************************************************************************************************************************/
        // NODE FONTS

        // NODE Reset font to default font for group of nodes
        public void ResetFont(Nodes nodes, Position position = null, long layer = 0)
        {
            if (nodes.Count>0) {
                this.undoOperations.Add("edit", nodes, null, position, layer);
                foreach (Node rec in nodes) // Loop through List with foreach
                {
                    rec.font = this.FontDefault;
                    rec.Resize();
                }
                this.Unsave();
                this.InvalidateDiagram();
            }
        }

        /*************************************************************************************************************************/
        // IMAGE

        // set image
        public void SetImage(Node rec, string file)
        {
            try
            {
                rec.isimage = true;
                rec.image = Media.GetImage(file);
                rec.imagepath = Os.MakeRelative(file, this.FileName);
                string ext = Os.GetExtension(file);
                if (ext != ".ico") rec.image.MakeTransparent(Color.White);
                rec.height = rec.image.Height;
                rec.width = rec.image.Width;
            }
            catch(Exception e)
            {
                Program.log.Write("setImage: " + e.Message);
            }
        }

        // remove image
        public void RemoveImage(Node rec)
        {
            rec.isimage = false;
            rec.imagepath = "";
            rec.image = null;
            rec.embeddedimage = false;
            rec.Resize();
        }

        // set image embedded
        public void SetImageEmbedded(Node rec)
        {
            if (rec.isimage)
            {
                rec.embeddedimage = true;
            }
        }
        
        /*************************************************************************************************************************/
        // LAYERS

        // LAYER MOVE posunie rekurzivne layer a jeho nody OBSOLATE
        public void MoveLayer(Node rec, Position vector)
        {
            if (rec != null)
            {
                Nodes nodes = this.layers.GetLayer(rec.id).nodes;
                foreach (Node node in nodes) // Loop through List with foreach
                {
                    if (node.layer == rec.id)
                    {
                        node.position.Add(vector);

                        if (node.haslayer)
                        {
                            MoveLayer(node, vector);
                        }
                    }
                }
            }
        }

        /*************************************************************************************************************************/
        // VIEW

        // open new view on diagram
        public DiagramView OpenDiagramView(DiagramView parent = null, Layer layer = null) //UID8210770134
        {
            DiagramView diagramview = new DiagramView(this.main, this, parent);
            diagramview.SetDiagram(this);
            this.DiagramViews.Add(diagramview);
            this.main.AddDiagramView(diagramview);
			this.SetTitle();
            diagramview.Show();
            if (layer != null)
            {
                diagramview.GoToLayer(layer.id);
            }
            return diagramview;
        }

        // invalidate all opened views
        public void InvalidateDiagram()
        {
            foreach (DiagramView DiagramView in this.DiagramViews)
            {
                if (DiagramView.Visible == true)
                {
                    DiagramView.Invalidate();
                }
            }
        }

        // close view UID2584689730
        public void CloseView(DiagramView view)
        {
            this.DiagramViews.Remove(view);
            this.main.RemoveDiagramView(view);

            foreach (DiagramView diagramView in this.DiagramViews) {
                if (diagramView.parentView == view) {
                    diagramView.parentView = null;
                }
            }

            this.CloseDiagram();
        }

        private bool isEmptyDiagram()
        {
            Nodes nodes = this.GetAllNodes();

            return nodes.Count == 0;
        }

        // UID3110763859
        public bool CloseDiagramViewWithDialog(DiagramView diagramView)
        {

            // multiple views already open, save is not needed now
            if (this.DiagramViews.Count() > 1) {
                return true;
            }

            // new empty file
            if (this.NewFile && this.isEmptyDiagram())
            {
                return true;
            }

            // file is already saved
            if (this.SavedFile && this.FileName != "" && Os.FileExists(this.FileName)) 
            {
                return true;
            }

            var res = MessageBox.Show(Translations.saveBeforeExit, Translations.confirmExit, MessageBoxButtons.YesNoCancel);

            if (res == DialogResult.Yes)
            {

                // save as new file with save dialog
                if ((this.FileName == "" || !Os.FileExists(this.FileName)))
                {

                    // set last open 
                    if (this.FileName != "") {
                        diagramView.DSave.FileName = this.FileName;
                    }

                    if (diagramView.DSave.ShowDialog() == DialogResult.OK)
                    {
                        this.SaveXMLFile(diagramView.DSave.FileName);
                        this.SetTitle();
                        return true;
                    }
                }

                //save to current file without dialog
                if (this.FileName != "" && Os.FileExists(this.FileName))
                {
                    this.SaveXMLFile(this.FileName);
                    return true;
                }
            }

            if (res == DialogResult.No)
            {
                return true;
            }


            if (res == DialogResult.Cancel)
            {
                return  false;
            }

            return false;
        }

        // close diagram UID4103426891
        public void CloseDiagram()
        {
            bool canclose = true;

            if (this.DiagramViews.Count > 0 || TextWindows.Count > 0)
            {
                canclose = false;
            }

            if (canclose)
            {
                this.ResetDefaultValuesForNewFile();
                this.main.RemoveDiagram(this);
                this.main.CloseEmptyApplication();
            }
        }

        // change title
        public void SetTitle()
        {
            foreach (DiagramView DiagramView in this.DiagramViews)
            {
                DiagramView.SetTitle();
            }
        }

        // refresh - refresh items depends on external resources like images
        public void RefreshAll()
        {
            foreach (Node node in this.layers.GetAllNodes())
            {
                
                if (node.isimage && !node.embeddedimage)
                {
                    node.LoadImage();
                }
                else
                {
                    node.Resize();
                }
            }

            this.InvalidateDiagram();
        }

        // refresh nodes- refresh items depends on external resources like images or hyperlinks names
        public void RefreshNodes(Nodes nodes)
        {
            foreach (Node node in nodes)
            {
                if (node.isimage && !node.embeddedimage)
                {
                    node.LoadImage();
                }
                else
                {
                    node.Resize();
                }
            }

            this.InvalidateDiagram();
        }

        // refresh background image after background image change
        public void RefreshBackgroundImages()
        {
            foreach (DiagramView DiagramView in this.DiagramViews)
            {
                DiagramView.RefreshBackgroundImage();
            }
        }

        // check if view to diagram exist and show diagram view
        public bool FocusToView()
        {
            Program.log.Write("window get focus");
            Program.log.Write("OpenDiagram: diagramView: setFocus");

            if (this.DiagramViews.Count() != 0)
            {
                this.DiagramViews[0].Show();
                this.DiagramViews[0].RestoreFormWindowState(); //UID4510272262
                return true;
            }

            Program.log.Write("bring focus");
            

            return false;
        }

        /*************************************************************************************************************************/

        // TextForm
        public void RemoveTextForm(TextForm textForm)
        {
           this.TextWindows.Remove(textForm);
        }


        /*************************************************************************************************************************/
        // CLIPBOARD

        // paste part of diagram from clipboard UID4178168001
        public DiagramBlock AddDiagramPart(string DiagramXml, Position position, long layer, decimal scale = 0)
        {
            string FontDefaultString = Fonts.FontToString(this.FontDefault);

            Nodes NewNodes = new Nodes();
            Lines NewLines = new Lines();

            XmlReaderSettings xws = new XmlReaderSettings
            {
                CheckCharacters = false
            };

            // parse xml file
            string xml = DiagramXml;
            try
            {
                using (XmlReader xr = XmlReader.Create(new StringReader(xml), xws))
                {
                    XElement root = XElement.Load(xr);
                    foreach (XElement diagram in root.Elements())
                    {
                        if (diagram.HasElements)
                        {
                            if (diagram.Name.ToString() == "rectangles")
                            {
                                this.LoadInnerXmlRectangles(NewNodes, diagram, FontDefaultString);
                            }

                            if (diagram.Name.ToString() == "lines")
                            {
                                this.LoadInnerXmlLines(NewLines, NewNodes, diagram);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Program.log.Write("Data has wrong structure. : error: " + ex.Message);
            }
            
            // order nodes
            Nodes NewReorderedNodes = new Nodes(); // order nodes parent first (layer must exist when sub node is created)
            this.NodesReorderNodes(0, null, NewNodes, NewReorderedNodes);

            // set first item for searching max scale and left corner
            decimal maxScale = 0;
            if (NewReorderedNodes.Count > 0) {
                maxScale = NewReorderedNodes[0].scale;              
            }

            List<MappedNode> maps = new List<MappedNode>();
            long layerParent = 0;
            long oldId = 0;
            Node newNode = null;
            Nodes createdNodes = new Nodes();
            foreach (Node rec in NewReorderedNodes)
            {
                // remap layer ids
                layerParent = 0;
                if (rec.layer == 0)
                {
                    layerParent = layer;
                }
                else
                {
                    foreach (MappedNode mapednode in maps)
                    {
                        if (rec.layer == mapednode.oldId)
                        {
                            layerParent = mapednode.newNode.id;
                            break;
                        }
                    }
                }

                rec.layer = layerParent;
                
                //rec.position.Add(position);
                //rec.resize();

                oldId = rec.id;
                newNode = this.CreateNode(rec);
                MappedNode mappedNode;
                if (newNode != null) {
                    // remember id in old diagram and current diagram
                    mappedNode = new MappedNode {
                        oldId = oldId,
                        newNode = newNode
                    };
                    createdNodes.Add(newNode);
                    maps.Add(mappedNode);
                }
                
                // search for max sxale in selection
                if (rec.scale > maxScale) {
                    maxScale = rec.scale;
                }
            }

            // fix layers and shortcuts
            foreach (Node rec in NewNodes)
            {
                if (rec.shortcut != 0)
                {
                    bool foundMappedNode = false;
                    foreach (MappedNode mapednode in maps)
                    {
                        if (rec.shortcut == mapednode.oldId)
                        {
                            rec.shortcut = mapednode.newNode.id;
                            foundMappedNode = true;
                            break;
                        }
                    }
                    
                    if(!foundMappedNode) 
                    {
                        rec.shortcut = 0;
                    }
                }
            }

            // move nodes in top layer relative to paste position
            // nodes inside layers are still on own position
            decimal deltaScale = maxScale - scale;
            foreach (Node rec in NewNodes)
            {
                if (rec.layer == layer) {
                    rec.scale -= deltaScale;
                    rec.position.Split(Tools.GetScale(deltaScale)).Add(position);
                }
            }

            Line newLine = null;
            Lines createdLines = new Lines();
            foreach (Line line in NewLines)
            {
                foreach (MappedNode mapbegin in maps)
                {
                    if (line.start == mapbegin.oldId)
                    {
                        foreach (MappedNode mapend in maps)
                        {
                            if (line.end == mapend.oldId)
                            {
                                newLine = this.Connect(
                                    mapbegin.newNode,
                                    mapend.newNode,
                                    line.arrow,
                                    line.color,
                                    line.width
                                );

                                if (newLine != null)
                                {
                                    createdLines.Add(newLine);
                                }
                            }
                        }
                    }
                }
            }
            

            return new DiagramBlock(NewNodes, createdLines);
        }

        // 
        private void NodesReorderNodes(long layer, Node parent, Nodes nodesIn, Nodes nodesOut)
        {
            foreach (Node node in nodesIn)
            {
                if (node.layer == layer)
                {
                    if (parent != null) {
                        parent.haslayer = true;
                    }

                    nodesOut.Add(node);

                    NodesReorderNodes(node.id, node, nodesIn, nodesOut);
                }
            }
        }

        // Get all layers nodes
        public void GetLayerNodes(Node node, Nodes nodes)
        {
            if (node.haslayer) {
                Layer layer = this.layers.GetLayer(node.id);
                if (layer != null && layer.nodes != null) {
                    foreach (Node subnode in this.layers.GetLayer(node.id).nodes) {
                        nodes.Add(subnode);
                        if (subnode.haslayer) {
                            GetLayerNodes(subnode, nodes);
                        }
                    }
                }
            }
        }

        // copy part of diagram to text xml string UID4762897496
        public string GetDiagramPart(Nodes nodes)
        {
            if (nodes.Count() == 0)
            {
                return "";
            }
            
            Nodes copy = new Nodes();
            copy.Copy(nodes);

            XElement root = new XElement("diagram");

            decimal minx = copy[0].position.x;
            decimal miny = copy[0].position.y;

            foreach (Node node in copy)
            {
                if (node.position.x < minx) minx = node.position.x;
                if (node.position.y < miny) miny = node.position.y;
            }

            foreach (Node rec in copy)
            {
                rec.position.x -= minx;
                rec.position.y -= miny;
            }
            
            Nodes subnodes = new Nodes();

            foreach (Node node in nodes)
            {
                GetLayerNodes(node, subnodes);
            }

            foreach (Node node in subnodes)
            {
                copy.Add(node.Clone());
            }            
           
            Lines lines = new Lines();
            lines.Copy(this.layers.GetAllLinesFromNodes(copy));

            long lastId = 0;
            Dictionary<long, long> ids = new Dictionary<long, long>();

            foreach (Node node in copy)
            {
                if (!ids.ContainsKey(node.id))
                {
                    lastId++;
                    ids.Add(node.id, lastId);
                    node.id = lastId;
                }
                else {
                    node.id = ids[node.id];
                }

            }

            foreach (Node node in copy)
            {
                if (!ids.ContainsKey(node.layer))
                {
                    node.layer = 0;
                }
                else
                {
                    node.layer = ids[node.layer];
                }
            }


            foreach (Node node in copy)
            {
                if (!ids.ContainsKey(node.shortcut))
                {
                    node.shortcut = 0;
                }
                else
                {
                    node.shortcut = ids[node.shortcut];
                }
            }

            foreach (Line li in lines)
            {
                li.start = ids[li.start];
                li.end = ids[li.end];
                li.layer = !ids.ContainsKey(li.layer) ? 0 : ids[li.layer];
            }

            XElement xrectangles = this.SaveInnerXmlNodes(copy);
            XElement xlines = this.SaveInnerXmlLines(lines);
                
            root.Add(xrectangles);
            root.Add(xlines);
            string copyxml = root.ToString();

            return copyxml;
        }

        public DiagramBlock GetPartOfDiagram(Nodes nodes)
        {
            Nodes allNodes = new Nodes();
            Lines lines = new Lines();

            foreach (Node node in nodes)
            {
                allNodes.Add(node);
            }

            if (allNodes.Count() > 0)
            {
                Nodes subnodes = new Nodes();

                foreach (Node node in allNodes)
                {
                    GetLayerNodes(node, subnodes);
                }

                foreach (Node node in subnodes)
                {
                    allNodes.Add(node);
                }

                foreach (Line li in this.GetAllLines())
                {
                    foreach (Node recstart in allNodes)
                    {
                        if (li.start == recstart.id)
                        {
                            foreach (Node recend in allNodes)
                            {
                                if (li.end == recend.id)
                                {
                                    lines.Add(li);
                                }
                            }
                        }
                    }
                }
            }

            return new DiagramBlock(allNodes, lines);
        }

        public DiagramBlock DuplicatePartOfDiagram(Nodes nodes, long layer = 0)
        {
            // get part of diagram for duplicate
            DiagramBlock diagramPart = this.GetPartOfDiagram(nodes);

            List<MappedNode> maps = new List<MappedNode>();

            Nodes duplicatedNodes = new Nodes();
            foreach (Node node in diagramPart.nodes)
            {
                duplicatedNodes.Add(node.Clone());
            }

            // order nodes parent first (layer must exist when sub node is created)
            Nodes NewReorderedNodes = new Nodes(); 
            this.NodesReorderNodes(layer, null, duplicatedNodes, NewReorderedNodes);

            long layerParent = 0;

            MappedNode mappedNode;
            Nodes createdNodes = new Nodes();
            Node newNode = null;
            long oldId = 0;
            foreach (Node rec in NewReorderedNodes)
            {
                layerParent = layer;
                
                // find layer id for sub layer
                foreach (MappedNode mapednode in maps)
                {
                    if (rec.layer == mapednode.oldId)
                    {
                        layerParent = mapednode.newNode.id;
                        break;
                    }
                }


                rec.layer = layerParent;
                rec.Resize();

                oldId = rec.id;
                newNode = this.CreateNode(rec);

                if (newNode != null)
                {
                    mappedNode = new MappedNode
                    {
                        oldId = oldId,
                        newNode = newNode
                    };

                    createdNodes.Add(newNode);
                    maps.Add(mappedNode);
                }
            }

            // fix layers and shortcuts
            foreach (Node rec in duplicatedNodes)
            {
                if (rec.shortcut != 0)
                {
                    foreach (MappedNode mapednode in maps)
                    {
                        if (rec.shortcut == mapednode.oldId)
                        {
                            rec.shortcut = mapednode.newNode.id;
                            break;
                        }
                    }
                }
            }

            Lines createdLines = new Lines();
            Line newLine = null;
            foreach (Line line in diagramPart.lines)
            {
                foreach (MappedNode mapbegin in maps)
                {
                    if (line.start == mapbegin.oldId)
                    {
                        foreach (MappedNode mapend in maps)
                        {
                            if (line.end == mapend.oldId)
                            {
                                // create new line by connecting new nodes
                                newLine = this.Connect(
                                    mapbegin.newNode,
                                    mapend.newNode,
                                    line.arrow,
                                    line.color,
                                    line.width
                                );

                                if (newLine != null) // skip invalid lines (perent not exist)
                                {
                                    createdLines.Add(newLine);
                                }
                            }
                        }
                    }
                }
            }

            return new DiagramBlock(createdNodes, createdLines);
        }

        /*************************************************************************************************************************/
        // SECURITY

        // encrypt diagram 
        public bool SetPassword(string password = null)
        {
            string newPassword = null;

            if (password == null)
            {
                newPassword = this.main.GetNewPassword();
            }
            else
            {
                newPassword = password;
            }

            if (newPassword != null)
            {

                if (newPassword == "")
                {
                    this.encrypted = false;
                    this.password = null;
                    this.passwordHash = null;
                    return true;
                }

                if (newPassword != "" && this.password == null)
                {
                    this.encrypted = true;
                    this.password = Encrypt.ConvertToSecureString(newPassword);
                    this.passwordHash = Encrypt.CalculateSHA512Hash(newPassword);
                    return true;
                }

                if (newPassword != "" && this.password != null && !Encrypt.CompareSecureString(this.password, newPassword))
                {
                    this.encrypted = true;
                    this.password = Encrypt.ConvertToSecureString(newPassword);
                    this.passwordHash = Encrypt.CalculateSHA512Hash(newPassword);
                    return true;
                }
            }

            return false;
        }

        // change password
        public bool ChangePassword()
        {
            string newPassword = this.main.ChangePassword(this.password);
            if (newPassword != null)
            {
                return this.SetPassword(newPassword);
            }

            return false;
        }

        // check if password is set
        public bool IsEncrypted()
        {
            return this.encrypted;
        }

        // check if diagram is locked
        public bool IsLocked()
        {
            return this.locked;
        }

        // lock diagram - forgot password
        public void LockDiagram() //UID9013092575
        {
            if (this.encrypted && !this.locked)
            {
                foreach(DiagramView view in DiagramViews) {
                    view.LockView();
                }

                this.locked = true;
                this.password = null;
                this.InvalidateDiagram();
            }
        }

        // unlock diagram - prompt for new password
        public bool UnlockDiagram()
        {
            if (this.encrypted && this.locked)
            {
                while (true) // while password is not correct or cancel is pressed
                {
                    string password = this.main.GetPassword();

                    if (password != null && this.passwordHash == Encrypt.CalculateSHA512Hash(password))
                    {
                        this.SetPassword(password);
                        this.locked = false;
                        this.InvalidateDiagram();
                        return true;
                    }
                    else if (password == null)
                    {
                        this.locked = true;
                        this.CloseDiagram();
                        return false;
                    }
                }
            }

            return false;
        }

        public void TakeOwnership(bool showDialog = false)
        {

            if (showDialog) {
                ConfirmTakeOwnership confirmTakeOwnership = new ConfirmTakeOwnership();
                confirmTakeOwnership.ShowDialog();

                if (confirmTakeOwnership.isConfirmed()) {
                    this.signed = true;
                }
            } 
        }

        public bool isSigned()
        {
            return this.signed;
        }
    }
}
