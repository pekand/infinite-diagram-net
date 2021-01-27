using System;
using System.Drawing;

namespace Diagram
{
    /// <summary>
    /// Node in diagram</summary>
    public class Node //UID6202169477
    {
        public long id = 0; // node unique id

        /*************************************************************************************************************************/
        // SIZE AND POSITION

        public Position position = new Position(); // node position in canvas
        public decimal width = 0; // node size counted from current font
        public decimal height = 0;
        public decimal scale = 0;

        /*************************************************************************************************************************/
        // FLAGS

        public bool selected = false; // node is selected by mouse
        public bool visible = true;

        /*************************************************************************************************************************/
        // STYLES

        public ColorType color = new ColorType("#FFFFB8"); // node color
        public Font font = null; // node name font
        public ColorType fontcolor = new ColorType(); // node name ext color
        public bool transparent = false; // node is transparent, color is turn off

        /*************************************************************************************************************************/
        // TEXT

        public string name = ""; // node name
        public string note = ""; // node note
        public string link = ""; // node link to external source

        /*************************************************************************************************************************/
        // LAYER

        public long layer = 0; // layer id or parent node id
        public bool haslayer = false; // nose has one or more childrens
        public Position layerShift = new Position(); // last position in layer
        public decimal layerScale = 0;

        /*************************************************************************************************************************/
        // SHORTCUT

        public long shortcut = 0; // node id whitch is linked with this node
        public bool mark = false; // mark node position for navigation history

        /*************************************************************************************************************************/
        // IMAGE

        public bool isimage = false; // show node as image instead of text
        public bool embeddedimage = false; // image is imported to xml file as string
        public string imagepath = ""; // path to node image
        public Bitmap image = null; // loaded image
        public long iwidth = 0; //image size
        public long iheight = 0;

        /*************************************************************************************************************************/
        // ATTACHMENT

        public string attachment = ""; // compressed file attachment

        /*************************************************************************************************************************/
        // TIME

        public string timecreate = ""; // node creation time
        public string timemodify = ""; // node modification time

        /*************************************************************************************************************************/
        // SCRIPT

        public string scriptid = ""; // node text id for in script search

        /*************************************************************************************************************************/
        // PADDING

        public const int NodePadding = 10;             // node padding around node name text
        public const int EmptyNodePadding = 20;        // node padding for empty node circle
        public const string protectedName = "*****";   // name for protected node
        
        /*************************************************************************************************************************/
        // SECURITY

        public bool protect = false; // protect sensitive data like pasword in node name (show asterisk instead of name)

        /*************************************************************************************************************************/
        // CONSTRUCTORS

        public Node()
        {
        }

        public Node(Node node)
        {
            this.Set(node);
        }

        /*************************************************************************************************************************/
        // SETTERS AND GETTERS

        public void Set(Node node)
        {
            this.id = node.id;

            this.color.Set(node.color);
            this.font = node.font;
            this.fontcolor.Set(node.fontcolor);
            this.transparent = node.transparent;

            this.name = node.name;
            this.note = node.note;
            this.link = node.link;

            this.position.Set(node.position);
            this.width = node.width;
            this.height = node.height;
            this.scale = node.scale;

            this.layer = node.layer;
            this.haslayer = node.haslayer;
            this.layerShift.Set(node.layerShift);

            this.shortcut = node.shortcut;
            this.mark = node.mark;

            this.isimage = node.isimage;
            this.embeddedimage = node.embeddedimage;
            this.imagepath = node.imagepath;
            this.image = node.image;

            this.iwidth = node.iwidth;
            this.iheight = node.iheight;

            this.attachment = node.attachment;

            this.timecreate = node.timecreate;
            this.timemodify = node.timemodify;

            this.scriptid = node.scriptid;

            this.protect = node.protect;
        }

        /// <summary>
        /// node copy from another node to current node</summary>
        public void CopyNode(Node node, bool skipPosition = false, bool skipSize = false) 
        {
            this.color.Set(node.color);
            this.font = node.font;
            this.fontcolor.Set(node.fontcolor);
            this.transparent = node.transparent;

            this.name = node.name;
            this.note = node.note;
            this.link = node.link;

            this.shortcut = node.shortcut;
            this.mark = node.mark;

            if (!skipPosition)
            {
                this.position.Set(node.position);
            }

            if (!skipSize)
            {
                this.width = node.width;
                this.height = node.height;
                this.scale = node.scale;
            }

            this.isimage = node.isimage;
            this.embeddedimage = node.embeddedimage;
            this.imagepath = node.imagepath;
            this.image = node.image;

            this.iwidth = node.iwidth;
            this.iheight = node.iheight;

            this.timecreate = node.timecreate;
            this.timemodify = node.timemodify;

            this.scriptid = node.scriptid;

            this.protect = node.protect;
        }

        /// <summary>
        /// node copy style from another node to current node</summary>
        public void CopyNodeStyle(Node node)
        {
            this.color.Set(node.color);
            this.font = node.font;
            this.fontcolor.Set(node.fontcolor);
            this.transparent = node.transparent;

            this.isimage = node.isimage;
            this.embeddedimage = node.embeddedimage;
            this.imagepath = node.imagepath;
            this.image = node.image;

            this.iwidth = node.iwidth;
            this.iheight = node.iheight;

            this.protect = node.protect;

            this.Resize();
        }

        /// <summary>
        /// clone node to new node</summary>
        public Node Clone()
        {
            return new Node(this);
        }

        /*************************************************************************************************************************/
        // RESIZE

        public void SetName(string name)
        {
            this.name = name;

            if (this.protect)
            {
                this.ResizeProtect();
            }
            else
            {
                this.Resize();
            }
        }

        public SizeF Measure()
        {
            SizeF s;

            if (name != "")
            {
                s = Fonts.MeasureString((this.protect) ? Node.protectedName : this.name, this.font);
                s.Height += 2 * Node.NodePadding;
                s.Width += 2 * Node.NodePadding;
            }
            else
            {
                s = new SizeF(Node.EmptyNodePadding, Node.EmptyNodePadding);
            }

            return s;
        }

        public void Resize()
        {
            if (!this.isimage)
            {
                if (!this.protect)
                {
                    SizeF s = Measure();

                    this.width = (int)s.Width;
                    this.height = (int)s.Height;
                }
                else
                {
                    this.ResizeProtect();
                }
            }
        }

        public void ResizeProtect()
        {
            SizeF s = Fonts.MeasureString(Node.protectedName, this.font);
            s.Height += 2 * Node.NodePadding;
            s.Width += 2 * Node.NodePadding;

            this.width = (int)s.Width;
            this.height = (int)s.Height;
        }

        public void SetProtect(bool protect)
        {
            this.protect = protect;

            if (this.protect)
            {
                this.ResizeProtect();
            }
            else
            {
                this.Resize();
            }
        }

        /*************************************************************************************************************************/
        // IMAGE

        public void LoadImage()
        {
            if (this.imagepath != "" && Os.FileExists(this.imagepath))
            {
                try
                {
                    string ext = "";
                    ext = Os.GetExtension(this.imagepath).ToLower();

                    if (ext == ".jpg" || ext == ".png" || ext == ".ico" || ext == ".bmp")
                    {
                        this.image = Media.GetImage(this.imagepath);
                        if (ext != ".ico") this.image.MakeTransparent(Color.White);
                        this.height = this.image.Height;
                        this.width = this.image.Width;
                        this.isimage = true;
                    }
                }
                catch (Exception ex)
                {
                    Program.log.Write("load image from xml error: " + ex.Message);
                }
            }
            else
            {
                this.imagepath = "";
            }
        }

        /*************************************************************************************************************************/
        // SEARCH

        /// <summary>
        /// Check if node contain string in some attribute (for search nodes by string)</summary>
        public bool Contain(string searchFor)
        {
            if (name.Contains(searchFor))
            {
                return true;
            }

            if (note.Contains(searchFor))
            {
                return true;
            }

            if (link.Contains(searchFor))
            {
                return true;
            }

            return false;
        }
    }
}
