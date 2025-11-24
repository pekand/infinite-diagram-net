#nullable disable

namespace Diagram
{
    /// <summary>
    /// key shortcuts table </summary>
    public class KeyMap 
    {
        public const string selectAllElements = "CTRL+A";
        public const string alignToLine = "CTRL+L";
        public const string alignToColumn = "CTRL+H";
        public const string alignToGroup = "CTRL+K";
        public const string alignToLineGroup = "CTRL+SHIFT+K";
        public const string copy = "CTRL+C";
        public const string copyLinks = "CTRL+SHIFT+C";
        public const string copyNotes = "CTRL+ALT+SHIFT+C";
        public const string paste = "CTRL+V";
        public const string cut = "CTRL+X";
        public const string pasteToNote = "CTRL+SHIFT+V";
        public const string pasteToLink = "CTRL+SHIFT+INS";
        public const string undo = "CTRL+Z";
        public const string redo = "CTRL+SHIFT+Z";
        public const string newDiagram = "CTRL+N";
        public const string newDiagramView = "F7";
        public const string switchViews = "F8";
        public const string save = "CTRL+S";
        public const string open = "CTRL+O";
        public const string search = "CTRL+F";
        public const string searchNext = "F3";
        public const string date = "CTRL+D";
        public const string promote = "CTRL+SHIFT+P";
        public const string refresh = "CTRL+R";
        public const string hideBackground = "F6";
        public const string reverseSearch = "SHIFT+F3";
        public const string home = "HOME";
        public const string openViewHome = "CTRL+HOME";
        public const string end = "END";
        public const string openViewEnd = "CTRL+END";
        public const string setHome = "SHIFT+HOME";
        public const string setEnd = "SHIFT+END";
        public const string openDirectory = "F5";
        public const string console = "F12";
        public const string moveNodeUp = "CTRL+PAGEUP";
        public const string moveNodeDown = "CTRL+PAGEDOWN";
        public const string pageUp = "PAGEUP";
        public const string pageDown = "PAGEDOWN";
        public const string editNodeName = "F2";
        public const string editNodeLink = "F4";
        public const string fullScreen = "F11";
        public const string openEditForm = "CTRL+E";
        public const string editOrLayerIn = "ENTER";
        public const string layerIn = "ENTER";
        public const string layerOut = "BACK";
        public const string minimalize = "ESCAPE";
        public const string delete = "DELETE";
        public const string moveLeft = "LEFT";
        public const string moveLeftFast = "SHIFT+LEFT";
        public const string moveRight = "RIGHT";
        public const string moveRightFast = "SHIFT+RIGHT";
        public const string moveUp = "UP";
        public const string moveUpFast = "SHIFT+UP";
        public const string moveDown = "DOWN";
        public const string moveDownFast = "SHIFT+DOWN";
        public const string alignLeft = "TAB";
        public const string alignRight = "SHIFT+TAB";
        public const string editCancel = "ESCAPE";
        public const string resetZoom = "CTRL+0";
        public const string markNodes = "CTRL+M";
        public const string nextMarkNode = "ALT+RIGHT";
        public const string prevMarkNode = "ALT+LEFT";
        public const string switchSecurityLock = "CTRL+ALT+L";
        public const string createPolygon = "CTRL+P";

        /// <summary>
        /// check if key string match keyData representing key pressed code</summary>
        public static bool ParseKey(string key, Keys keyData)
        {

            string[] parts = key.Split('+');
            Keys keyCode = 0;

            foreach (string part in parts)
            {
                if (part == "CTRL")
                {
                    keyCode = Keys.Control | keyCode;
                    continue;
                }

                if (part == "ALT")
                {
                    keyCode = Keys.Alt | keyCode;
                    continue;
                }

                if (part == "SHIFT")
                {
                    keyCode = Keys.Shift | keyCode;
                    continue;
                }

                if (part == "PAGEUP")
                {
                    keyCode = Keys.PageUp | keyCode;
                    continue;
                }

                if (part == "PAGEDOWN")
                {
                    keyCode = Keys.PageDown | keyCode;
                    continue;
                }

                if (part == "INS")
                {
                    keyCode = Keys.Insert | keyCode;
                    continue;
                }

                if (part == "DEL")
                {
                    keyCode = Keys.Delete | keyCode;
                    continue;
                }

                if (part == "LEFT")
                {
                    keyCode = Keys.Left | keyCode;
                    continue;
                }

                if (part == "RIGHT")
                {
                    keyCode = Keys.Right | keyCode;
                    continue;
                }

                if (part == "UP")
                {
                    keyCode = Keys.Up | keyCode;
                    continue;
                }

                if (part == "DOWN")
                {
                    keyCode = Keys.Down | keyCode;
                    continue;
                }

                if (part == "0")
                {
                    keyCode = Keys.D0 | keyCode;
                    continue;
                }

                if (part == "1")
                {
                    keyCode = Keys.D1 | keyCode;
                    continue;
                }

                if (part == "2")
                {
                    keyCode = Keys.D2 | keyCode;
                    continue;
                }

                if (part == "3")
                {
                    keyCode = Keys.D3 | keyCode;
                    continue;
                }

                if (part == "4")
                {
                    keyCode = Keys.D4 | keyCode;
                    continue;
                }

                if (part == "5")
                {
                    keyCode = Keys.D5 | keyCode;
                    continue;
                }

                if (part == "6")
                {
                    keyCode = Keys.D6 | keyCode;
                    continue;
                }

                if (part == "7")
                {
                    keyCode = Keys.D7 | keyCode;
                    continue;
                }

                if (part == "8")
                {
                    keyCode = Keys.D8 | keyCode;
                    continue;
                }

                if (part == "9")
                {
                    keyCode = Keys.D9 | keyCode;
                    continue;
                }

                if (Enum.TryParse(Fonts.FirstCharToUpper(part), out Keys code))
                {
                    keyCode = code | keyCode;
                }
            }

            if (keyCode == keyData)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// convert pressed key to string</summary>
        public static string ParseKey(Keys keyData)
        {

            KeysConverter kc = new();
            string keyChar = kc.ConvertToString(keyData);

            keyChar = keyChar.Replace("Control", "CTRL");
            keyChar = keyChar.Replace("Alt", "ALT");
            keyChar = keyChar.Replace("Shift", "SHIFT");
            keyChar = keyChar.Replace("Left", "LEFT");
            keyChar = keyChar.Replace("Right", "RIGHT");
            keyChar = keyChar.Replace("Up", "UP");
            keyChar = keyChar.Replace("Down", "DOWN");

            keyChar = keyChar.Replace("Del", "UP");
            keyChar = keyChar.Replace("Ins", "INS");
            keyChar = keyChar.Replace("End", "END");
            keyChar = keyChar.Replace("Home", "HOME");
            keyChar = keyChar.Replace("PgDn", "PAGEDOWN");
            keyChar = keyChar.Replace("PgUP", "PAGEUP");

            return keyChar;
        }
    }
}
