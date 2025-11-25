using Diagram.src.Lib;
using System.Security.Cryptography;

namespace Diagram
{
    public class ImageManager : IDisposable
    {
        private bool disposed = false;

        private readonly Dictionary<string, ImageEntry> images = [];

        public static string ComputeHash(byte[] data)
        {
            byte[] hashBytes = SHA256.HashData(data);
            return Convert.ToHexStringLower(hashBytes);
        }

        private Bitmap? LoadImageFromBytes(byte[] data)
        {
            try
            {
                using var ms = new MemoryStream(data);

                return (Bitmap)Bitmap.FromStream(ms);
            }
            catch (Exception)
            {

                return null;
            }
            
        }

        public ImageEntry? AddImage(string filePath)
        {

            if (!Os.FileExists(filePath)) { 
                return null;
            }

            string fileExtension = Os.GetExtension(filePath);

            byte[] data = File.ReadAllBytes(filePath);
            string hash = ComputeHash(data);

            if (images.TryGetValue(hash, out ImageEntry? value))
                return value;

            try
            {
                Bitmap? img = LoadImageFromBytes(data);
                if (img != null) {
                    images[hash] = new ImageEntry
                    {
                        Image = img,
                        Path = filePath,
                        Hash = hash
                    };

                    if (!Images.CanDraw(img)) {
                        images[hash].InvalidImage = true;
                    }

                    return images[hash];
                }
            }
            
            catch (Exception)
            {
               
            }

            return null;
        }

        public static string GetBitmapHash(Bitmap image)
        {
            using var ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            byte[] data = ms.ToArray();
            string hash = ImageManager.ComputeHash(data);
            return hash;
        }

        public static Bitmap CloneBitmap(Bitmap image)
        {
            using var ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            byte[] data = ms.ToArray();
            using var msClone = new MemoryStream(data);
            image = (Bitmap)Bitmap.FromStream(msClone);
            return image;
        }

        public ImageEntry? AddImage(Bitmap image, bool cloneImage = false)
        {
            string hash = GetBitmapHash(image);

            if (images.TryGetValue(hash, out ImageEntry? value))
                return value;

            if (cloneImage)
            {
                image = CloneBitmap(image);
            }

            images[hash] = new ImageEntry
            {
                Image = image,
                Path = null,
                Hash = hash
            };

            return images[hash];
        }

        public bool RemoveImageByPath(string filePath)
        {
            byte[] data = File.ReadAllBytes(filePath);
            string hash = ComputeHash(data);

            return RemoveImageByHash(hash);
        }

        public bool RemoveImageByHash(string hash)
        {
            if (images.TryGetValue(hash, out var entry))
            {
                if (entry != null && entry.Image != null) {
                    entry.Image.Dispose();
                }
                
                images.Remove(hash);
                return true;
            }
            return false;
        }

        public bool RemoveImageByInstance(Bitmap image)
        {
            string hash = GetBitmapHash(image);
            return images.Remove(hash); ;
        }

        public bool RemoveImageEntry(ImageEntry imageEntry)
        {
            if (imageEntry != null && imageEntry.Hash != null && images.ContainsKey(imageEntry.Hash))
            {
                imageEntry.Image?.Dispose();
                
                images.Remove(imageEntry.Hash);
                return true;
            }

            return false;
        }

        public List<ImageEntry> GetAllEntries()
        {
            return [.. images.Values];
        }


        ~ImageManager()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            foreach (var entry in images.Values)
            {
                entry.Image?.Dispose();
            }

            images.Clear();
            disposed = true;
        }
    }

}
