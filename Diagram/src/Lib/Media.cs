using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Diagram
{

    /// <summary>
    /// repository for screen and images related functions</summary>
    public class Media //UID0928056661
    {
        /*************************************************************************************************************************/
        // SCREEN

        /// <summary>
        /// get active form screen width </summary>
        public static int ScreenWidth(Form form)
        {
            return Screen.FromControl(form).Bounds.Size.Width;
        }

        /// <summary>
        /// get active form screenn height </summary>
        public static int ScreenHeight(Form form)
        {
            return Screen.FromControl(form).Bounds.Size.Height;
        }

        /// <summary>
        /// get active form screen width </summary>
        public static int ScreenWorkingAreaWidth(Form form)
        {
            return Screen.FromControl(form).WorkingArea.Width;
        }

        /// <summary>
        /// get active form screenn height </summary>
        public static int ScreenWorkingAreaHeight(Form form)
        {
            return Screen.FromControl(form).WorkingArea.Height;
        }

        /*************************************************************************************************************************/
        // COLOR

        /// <summary>
        /// convert hexidecimal html color to Color object </summary>
        public static Color GetColor(string color)
        {
            return System.Drawing.ColorTranslator.FromHtml(color);
        }

        /*************************************************************************************************************************/
        // BITMAPS

        /// <summary>
        /// load image from file </summary>
        public static Bitmap GetImage(string file)
        {
            try
            {
                return  (Bitmap)Image.FromFile(file, true);
            }
            catch (Exception e)
            {
                Program.log.Write("getImage: " + e.Message);
            }

            return null;
        }

        /// <summary>
        /// load icon from file </summary>
        public static Icon GetIcon(string file)
        {
            try
            {
                return new Icon(file);
            }
            catch (Exception e)
            {
                Program.log.Write("getIcon: " + e.Message);
            }

            return null;
        }

        /// <summary>
        /// convert Bitmap to base64 string </summary>
        public static string ImageToString(Bitmap image)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    return Compress.ZipStream(ms);
                }
            }
            catch (Exception e)
            {
                Program.log.Write("BitmapToString error: " + e.Message);
                return "";
            }
        }

        /// <summary>
        /// convert base64 string to Icon</summary>
        public static Bitmap StringToImage(string bitmap)
        {
            try
            {
                MemoryStream ms = new MemoryStream(); // input stream for gzip
                Compress.UnzipStream(bitmap, ms);
                return new Bitmap(ms);
            }
            catch (Exception e)
            {
                Program.log.Write("StringToBitmap error: " + e.Message);
                return null;
            }
        }

        /// <summary>
        /// convert Bitmap to base64 string </summary>
        public static string BitmapToString(Bitmap image)
        {
            try
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(Bitmap));
                return Convert.ToBase64String(
                    (byte[])converter.ConvertTo(image, typeof(byte[]))
                );
            }
            catch (Exception e)
            {
                Program.log.Write("StringToBitmap error: " + e.Message);
                return null;
            }
        }

        /// <summary>
        /// convert Bitmap to base64 string </summary>
        public static Bitmap StringToBitmap(String str)
        {
            return new Bitmap(
                new MemoryStream(
                    Convert.FromBase64String(str)
                )
            );
        }

        /*************************************************************************************************************************/
        // FOCUS

        /// <summary>
        /// bring form to foreground </summary>
        public static void BringToFront(Form form)   // [focus] UID0703915427
        {
            Program.log.Write("bringToFront");
            Tick.Timer(1000, (t, args) =>
            {
                if (t is Timer)
                {
                    Program.log.Write("bringToFront: timer tick");

                    Timer timer = t as Timer;                    
					timer.Enabled = false;

                    if(form.WindowState == FormWindowState.Minimized) {
                        form.WindowState = FormWindowState.Normal;
                    }
                    form.TopMost = true;                    
                    form.TopMost = false;
                    form.BringToFront();
                    form.Focus();
                }
            });
        }

        /*************************************************************************************************************************/
        // ICONS

        /// <summary>
        /// extract icon from executable</summary>
        public static Bitmap ExtractSystemIcon(string path)
        {

            try
            {
                Icon ico = Icon.ExtractAssociatedIcon(path);
                return ico.ToBitmap();
            }
            catch (Exception e)
            {
                Program.log.Write("get exe icon error: " + e.Message);
            }

            return null;

        }

        /// <summary>
        /// extract icon from link file</summary>
        public static Bitmap ExtractLnkIcon(string path)
        {

            try
            {
                var shl = new Shell32.Shell();
                string lnkPath = System.IO.Path.GetFullPath(path);
                var dir = shl.NameSpace(System.IO.Path.GetDirectoryName(lnkPath));
                var itm = dir.Items().Item(System.IO.Path.GetFileName(lnkPath));
                var lnk = (Shell32.ShellLinkObject)itm.GetLink;

                lnk.GetIconLocation(out String strIcon);
                Icon awIcon = Icon.ExtractAssociatedIcon(strIcon);

                return awIcon.ToBitmap();
            }
            catch (Exception e)
            {
                Program.log.Write("get exe icon error: " + e.Message);
            }

            return null;
        }

        /// <summary>
        /// convert Icon to base64 string </summary>
        public static string IconToString(Icon icon)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    icon.Save(ms);
                    byte[] bytes = ms.ToArray();
                    return Convert.ToBase64String(bytes);
                }
            }
            catch (Exception e)
            {
                Program.log.Write("IconToString error: " + e.Message);
                return "";
            }
        }

        /// <summary>
        /// convert base64 string to Icon</summary>
        public static Icon StringToIcon(string icon)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(icon);
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    return new Icon(ms);
                }
            }
            catch (Exception e)
            {
                Program.log.Write("StringToIcon error: " + e.Message);
                return null;
            }
        }
        
    }
}
