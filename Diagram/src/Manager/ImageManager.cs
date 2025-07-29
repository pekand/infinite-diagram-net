using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Diagram
{
    public class ImageManager : IDisposable
    {
        private bool disposed = false;

        private Dictionary<string, ImageEntry> images = new Dictionary<string, ImageEntry>();

        private string ComputeHash(byte[] data)
        {
            using (var sha = SHA256.Create())
            {
                byte[] hashBytes = sha.ComputeHash(data);
                return Convert.ToBase64String(hashBytes);
            }
        }

        private Bitmap? LoadImageFromBytes(byte[] data)
        {
            try
            {
                using (var ms = new MemoryStream(data))
                {
                    return (Bitmap)Bitmap.FromStream(ms);
                }
            }
            catch (Exception)
            {

                return null;
            }
            
        }

        public ImageEntry? AddImage(string filePath)
        {

            if (!File.Exists(filePath)) { 
                return null;
            }

            byte[] data = File.ReadAllBytes(filePath);
            string hash = ComputeHash(data);

            if (images.ContainsKey(hash))
                return images[hash];

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

                    return images[hash];
                }
            }
            
            catch (Exception)
            {
               
            }

            return null;
        }

        public ImageEntry? AddImage(Bitmap image, bool cloneImage = false)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                byte[] data = ms.ToArray();
                string hash = ComputeHash(data);

                if (images.ContainsKey(hash))
                    return images[hash];

                if (cloneImage)
                {
                    using (var msClone = new MemoryStream(data))
                    {
                        image = (Bitmap)Bitmap.FromStream(msClone);
                    }
                }

                images[hash] = new ImageEntry
                {
                    Image = image,
                    Path = null,
                    Hash = hash
                };

                return images[hash];
            }
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
            foreach (var kvp in images)
            {
                if (object.ReferenceEquals(kvp.Value.Image, image))
                {
                    kvp.Value.Image.Dispose();
                    images.Remove(kvp.Key);
                    return true;
                }
            }
            return false;
        }

        public bool RemoveImageEntry(ImageEntry imageEntry)
        {
            if (imageEntry != null && imageEntry.Hash != null && images.ContainsKey(imageEntry.Hash))
            {
                if (imageEntry.Image != null)
                {
                    imageEntry.Image.Dispose();
                }
                
                images.Remove(imageEntry.Hash);
                return true;
            }

            return false;
        }

        public List<ImageEntry> GetAllEntries()
        {
            return new List<ImageEntry>(images.Values);
        }

        public void Dispose()
        {
            if (disposed) return;

            foreach (var entry in images.Values)
            {
                if (entry.Image != null)
                {
                    entry.Image.Dispose();
                }
            }

            images.Clear();
            disposed = true;
        }

        ~ImageManager()
        {
            Dispose();
        }
    }

}
