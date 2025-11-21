#nullable disable

namespace Diagram
{
    /// <summary>
    /// Node in diagram</summary>
    public class Node //UID6202169477
    {
        public long id = 0; // node unique id

        /*************************************************************************************************************************/
        // SIZE AND POSITION

        public Position position = new(); // node position in canvas
        public decimal width = 0; // node size counted from current font
        public decimal height = 0;
        public decimal scale = 0;

        /*************************************************************************************************************************/
        // FLAGS

        public bool selected = false; // node is selected by mouse
        public bool visible = true;

        /*************************************************************************************************************************/
        // STYLES

        public ColorType color = new("#FFFFB8"); // node color
        public Font font = null; // node name font
        public ColorType fontColor = new(); // node name ext color
        public bool transparent = false; // node is transparent, color is turn off

        /*************************************************************************************************************************/
        // TEXT

        public string name = ""; // node name
        public string note = ""; // node note
        public string link = ""; // node link to external source

        /*************************************************************************************************************************/
        // LAYER

        public long layer = 0; // layer id or parent node id
        public bool hasLayer = false; // nose has one or more children
        public Position layerShift = new(); // last position in layer
        public decimal layerScale = 0;

        /*************************************************************************************************************************/
        // SHORTCUT

        public long shortcut = 0; // node id which is linked with this node
        public bool mark = false; // mark node position for navigation history

        /*************************************************************************************************************************/
        // IMAGE

        public bool isImage = false; // show node as image instead of text
        public bool embeddedImage = false; // image is imported to xml file as string
        public string imagePath = ""; // path to node image
        public ImageEntry image = null; // loaded image
        public long iWidth = 0; //image size
        public long iHeight = 0;

        /*************************************************************************************************************************/
        // IMAGE TRANSFORMATION
        public bool isImageTransformed = false;
        public int transformationRotateX = 0;
        public int transformationRotateY = 0;
        public bool transformationFlipX = false;
        public bool transformationFlipY = false;

        /*************************************************************************************************************************/
        // ATTACHMENT

        public string attachment = ""; // compressed file attachment

        /*************************************************************************************************************************/
        // TIME

        public string timeCreate = ""; // node creation time
        public string timeModify = ""; // node modification time

        /*************************************************************************************************************************/
        // SCRIPT

        public string scriptId = ""; // node text id for in script search

        /*************************************************************************************************************************/
        // PADDING

        public const int NodePadding = 10;             // node padding around node name text
        public const int EmptyNodePadding = 20;        // node padding for empty node circle
        public const string protectedName = "*****";   // name for protected node
        
        /*************************************************************************************************************************/
        // SECURITY

        public bool protect = false; // protect sensitive data like password in node name (show asterisk instead of name)

        /*************************************************************************************************************************/
        // PLUGINS

        public DataStorage dataStorage = new(); // extra storage for plugins

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
            this.fontColor.Set(node.fontColor);
            this.transparent = node.transparent;

            this.name = node.name;
            this.note = node.note;
            this.link = node.link;

            this.position.Set(node.position);
            this.width = node.width;
            this.height = node.height;
            this.scale = node.scale;

            this.layer = node.layer;
            this.hasLayer = node.hasLayer;
            this.layerShift.Set(node.layerShift);

            this.shortcut = node.shortcut;
            this.mark = node.mark;

            this.isImage = node.isImage;
            this.embeddedImage = node.embeddedImage;
            this.imagePath = node.imagePath;
            this.image = node.image;

            this.isImageTransformed = node.isImageTransformed;
            this.transformationRotateX = node.transformationRotateX;
            this.transformationRotateY = node.transformationRotateY;
            this.transformationFlipX = node.transformationFlipX;
            this.transformationFlipY = node.transformationFlipY;

            this.iWidth = node.iWidth;
            this.iHeight = node.iHeight;

            this.attachment = node.attachment;

            this.timeCreate = node.timeCreate;
            this.timeModify = node.timeModify;

            this.scriptId = node.scriptId;

            this.protect = node.protect;
        }

        /// <summary>
        /// node copy from another node to current node</summary>
        public void CopyNode(Node node, bool skipPosition = false, bool skipSize = false) 
        {
            this.color.Set(node.color);
            this.font = node.font;
            this.fontColor.Set(node.fontColor);
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

            this.isImage = node.isImage;
            this.embeddedImage = node.embeddedImage;
            this.imagePath = node.imagePath;
            this.image = node.image;

            this.isImageTransformed = node.isImageTransformed;
            this.transformationRotateX = node.transformationRotateX;
            this.transformationRotateY = node.transformationRotateY;
            this.transformationFlipX = node.transformationFlipX;
            this.transformationFlipY = node.transformationFlipY;

            this.iWidth = node.iWidth;
            this.iHeight = node.iHeight;

            this.timeCreate = node.timeCreate;
            this.timeModify = node.timeModify;

            this.scriptId = node.scriptId;

            this.protect = node.protect;
        }

        /// <summary>
        /// node copy style from another node to current node</summary>
        public void CopyNodeStyle(Node node)
        {
            this.color.Set(node.color);
            this.font = node.font;
            this.fontColor.Set(node.fontColor);
            this.transparent = node.transparent;

            this.isImage = node.isImage;
            this.embeddedImage = node.embeddedImage;
            this.imagePath = node.imagePath;
            this.image = node.image;

            this.isImageTransformed = node.isImageTransformed;
            this.transformationRotateX = node.transformationRotateX;
            this.transformationRotateY = node.transformationRotateY;
            this.transformationFlipX = node.transformationFlipX;
            this.transformationFlipY = node.transformationFlipY;

            this.iWidth = node.iWidth;
            this.iHeight = node.iHeight;

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
            if (!this.isImage)
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
