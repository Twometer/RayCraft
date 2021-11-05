using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayCraft.Renderer
{
    public unsafe class DisplayBuffer
    {
        private const PixelFormat PixFmt = PixelFormat.Format32bppRgb;

        private Bitmap bitmap;
        private BitmapData bitmapData;
        private int bpp;
        private byte* scan0;

        public DisplayBuffer(int width, int height)
        {
            bitmap = new Bitmap(width, height, PixFmt);
        }

        public void Begin()
        {
            bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, PixFmt);
            bpp = Image.GetPixelFormatSize(PixFmt);
            scan0 = (byte*)bitmapData.Scan0.ToPointer();
        }

        public void SetPixel(int x, int y, int rgb)
        {
            byte* colorPointer = scan0 + y * bitmapData.Stride + x * bpp / 8;
            *(int*)colorPointer = rgb;
        }

        public void Finish()
        {
            bitmap.UnlockBits(bitmapData);
        }

        public Bitmap GetBitmap()
        {
            return bitmap;
        }
    }
}
