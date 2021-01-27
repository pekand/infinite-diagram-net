using System;
using System.Windows.Forms;

namespace Diagram
{
    /// <summary>
    /// key shortcuts table </summary>
    public class KeyMap //UID6078031921
    {
        public static string selectAllElements = "CTRL+A";
        public static string alignToLine = "CTRL+L";
        public static string alignToColumn = "CTRL+H";
        public static string alignToGroup = "CTRL+K";
        public static string alignToLineGroup = "CTRL+SHIFT+K";
        public static string copy = "CTRL+C";
        public static string copyLinks = "CTRL+SHIFT+C";
        public static string copyNotes = "CTRL+ALT+SHIFT+C";
        public static string paste = "CTRL+V";
        public static string cut = "CTRL+X";
        public static string pasteToNote = "CTRL+SHIFT+V";
        public static string pasteToLink = "CTRL+SHIFT+INS";
        public static string undo = "CTRL+Z";
        public static string redo = "CTRL+SHIFT+Z";
        public static string newDiagram = "CTRL+N";
        public static string newDiagramView = "F7";
        public static string switchViews = "F8";
        public static string save = "CTRL+S";
        public static string open = "CTRL+O";
        public static string search = "CTRL+F";
        public static string searchNext = "F3";
        public static string date = "CTRL+D";
        public static string promote = "CTRL+SHIFT+P";
        public static string refresh = "CTRL+R";
        public static string hideBackground = "F6";
        public static string reverseSearch = "SHIFT+F3";
        public static string home = "HOME";
        public static string openViewHome = "CTRL+HOME";
        public static string end = "END";
        public static string openViewEnd = "CTRL+END";
        public static string setHome = "SHIFT+HOME";
        public static string setEnd = "SHIFT+END";
        public static string openDrectory = "F5";
        public static string console = "F12";
        public static string moveNodeUp = "CTRL+PAGEUP";
        public static string moveNodeDown = "CTRL+PAGEDOWN";
        public static string pageUp = "PAGEUP";
        public static string pageDown = "PAGEDOWN";
        public static string editNodeName =  "F2";
        public static string editNodeLink = "F4";
        public static string fullScreean = "F11";
        public static string openEditForm = "CTRL+E";
        public static string editOrLayerIn = "ENTER";
        public static string layerIn = "ADD";
        public static string layerOut = "SUBTRACT";
        public static string layerOut2 = "BACK";
        public static string minimalize = "ESCAPE";
        public static string delete = "DELETE";
        public static string moveLeft = "LEFT";
        public static string moveLeftFast = "SHIFT+LEFT";
        public static string moveRight = "RIGHT";
        public static string moveRightFast = "SHIFT+RIGHT";
        public static string moveUp = "UP";
        public static string moveUpFast = "SHIFT+UP";
        public static string moveDown = "DOWN";
        public static string moveDownFast = "SHIFT+DOWN";
        public static string alignLeft = "TAB";
        public static string alignRight = "SHIFT+TAB";
        public static string editCancel = "ESCAPE";
        public static string resetZoom = "CTRL+0";
        public static string markNodes = "CTRL+M";
        public static string nextMarkNode = "ALT+RIGHT";
        public static string prevMarkNode = "ALT+LEFT";
        public static string switchSecurityLock = "CTRL+ALT+L";
        public static string createPolygon = "CTRL+P";

        /// <summary>
        /// check if key string match keyData representing key pressed code</summary>
        public static bool ParseKey(string key, Keys keyData)
        {

            string[] parts = key.Split('+');
            Keys keyCode = 0;
            Keys code = 0;

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

                if (Enum.TryParse(Fonts.FirstCharToUpper(part), out code))
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
        /// conver pressed key to string</summary>
        public static string ParseKey(Keys keyData)
        {

            KeysConverter kc = new KeysConverter();
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
