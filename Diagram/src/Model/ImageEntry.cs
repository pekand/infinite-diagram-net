namespace Diagram
{
    public class ImageEntry : IDisposable
    {
        private bool disposed = false;

        public Bitmap? Image { get; set; }
        public string? Path { get; set; }
        public string? Hash { get; set; }

        public bool InvalidImage = false;

        ~ImageEntry()
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


            if (Image != null) {
                Image.Dispose();
                Image = null;
                Path = null;
                Hash = null;
            }          

            disposed = true;
        }
    }
}
