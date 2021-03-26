using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayCraft.Renderer
{
    public static class BlockRegistry
    {
        public static Dictionary<byte, ColorCollection> BlockColors { get; } = new Dictionary<byte, ColorCollection>();

        static BlockRegistry()
        {
            BlockColors.Add(0, new ColorCollection(60, 114, 240));
            BlockColors.Add(1, new ColorCollection(65, 65, 65));
            BlockColors.Add(2, ColorCollection.FromRgb(0x75B049));
            BlockColors.Add(3, ColorCollection.FromRgb(0xB9855C));
            BlockColors.Add(4, ColorCollection.FromRgb(0x404040));
            BlockColors.Add(5, ColorCollection.FromRgb(0x9F844D));
            BlockColors.Add(8, ColorCollection.FromRgb(0x1F3A8E));
            Add(9, 0x1A2C64);
            Add(10, 0xC04406);
            Add(11, 0xd48b26);
            Add(12, 0xE2DAA4);
            Add(13, 0x837A78);
            Add(17, 0x463823);
            Add(18, 0x3C9614);
        }

        private static void Add(byte id, int rgb)
        {
            BlockColors.Add(id, ColorCollection.FromRgb(rgb));
        }

    }

    public class ColorCollection
    {
        public int Lvl0;
        public int Lvl1;
        public int Lvl2;
        public int Lvl3;

        public static ColorCollection FromRgb(int rgb)
        {
            Color color = Color.FromArgb(rgb);
            return new ColorCollection(color.R, color.G, color.B);
        }

        public ColorCollection(int r, int g, int b)
        {
            Lvl0 = Color.FromArgb(r, g, b).ToArgb();
            Lvl1 = Color.FromArgb((int)(r * 0.7), (int)(g * 0.7), (int)(b * 0.7)).ToArgb();
            Lvl2 = Color.FromArgb((int)(r * 0.49), (int)(g * 0.49), (int)(b * 0.49)).ToArgb();
            Lvl3 = Color.FromArgb((int)(r * 0.34), (int)(g * 0.34), (int)(b * 0.34)).ToArgb();
        }
    }
}
