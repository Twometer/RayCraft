﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayCraft.Renderer
{
    public static class BlockRegistry
    {
        public static ColorCollection[] BlockColors = new ColorCollection[256];

        static BlockRegistry()
        {
            for (int i = 0; i < 256; i++)
                BlockColors[i] = new ColorCollection(0, 0, 0);

            Add(0, 0xAFC6E9);
            Add(1, 0x606060);
            Add(2, 0x75B049);
            Add(3, 0xB9855C);
            Add(4, 0x404040);
            Add(5, 0x9F844D);
            Add(8, 0x1F3A8E);
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
            BlockColors[id] = ColorCollection.FromRgb(rgb);
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
