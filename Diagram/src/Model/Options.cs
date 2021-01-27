using System.Drawing;

namespace Diagram
{

    /// <summary>
    /// opened diagram options</summary>
    public class Options //UID6652785641
    {
        /*************************************************************************************************************************/
        // POSITION

        public Position shift = new Position();            // startup position in diagram
        public Position firstLayereShift = new Position(); // position in layer
        public decimal firstLayereScale = 0;

        /*************************************************************************************************************************/
        // HOME AND END POSITIONS

        public Position homePosition = new Position();     // diagram start and home key position
        public long homeLayer = 0;                          // startup layer after diagram open
        public decimal homeScale = 0;                          // startup scale after diagram open
        public Position endPosition = new Position();      // diagram end key position seet by end key
        public long endLayer = 0;                           // startup layer after diagram open
        public decimal endScale = 0;                           // startup scale after diagram open

        /*************************************************************************************************************************/
        // STATES

        public bool readOnly = false;                      // diagram is read only
        public bool grid = true;                           // show grid
        public bool borders = false;                       // show node borders
        public bool coordinates = false;                   // show coordinates for debuging purpose        
        public bool openLayerInNewView = false;            // show grid

        /*************************************************************************************************************************/
        // WINDOW STATES

        public bool restoreWindow = false;                 // restore last window position and state 
        public long Left = 0;                               // diagram view position 
        public long Top = 0;                                // diagram view position 
        public long Width = 100;                            // diagram view position 
        public long Height = 100;                           // diagram view position 
        public long WindowState = 0;                        // 0 unset; 1 maximalized; 2 normal; 3 minimalized

        /*************************************************************************************************************************/
        // MOVE SPEED

        public long keyArrowSlowMoveNodeSpeed = 1;          // node moving speed
        public long keyArrowFastMoveNodeSpeed = 100;        // node moving speed
        public long keyArrowSlowSpeed = 50;                 // node moving speed

        /*************************************************************************************************************************/
        // COLORS

        public string colorDirectory = "#AF92FF";          // color for node linked with directory
        public string colorFile = "#D9CCFF";               // color for node linked with file
        public string colorAttachment = "#C495DB";         // color for node linked with url

        public ColorType lineColor = new ColorType("#000000");
        public ColorType nodeColor = new ColorType("#FFFFB8");
        public ColorType selectedNodeColor = new ColorType("#000000");
        public ColorType backgroundColor = new ColorType("#F2F2F2");
        public ColorType gridColor = new ColorType("#AAAAAA");
        public ColorType scrollbarColor = new ColorType("#000000");
        public ColorType selectionColor = new ColorType(Color.FromArgb(100, 10, 200, 200));


        /*************************************************************************************************************************/
        // ICON

        public string icon = "";                             // diagram custom icon
        
        public Bitmap backgroundImage = null;                // diagram custom icon
    }
}
