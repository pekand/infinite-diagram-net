namespace Diagram
{
    public class ImageEntry : IDisposable
    {
        private bool disposed = false;

        public Bitmap? Image { get; set; }
        public string? Path { get; set; }
        public string? Hash { get; set; }

        public void Dispose()
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

        ~ImageEntry()
        {
            Dispose();
        }
    }
}
