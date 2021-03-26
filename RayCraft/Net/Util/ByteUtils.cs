using System;
using System.Collections.Generic;
using System.Text;

namespace Craft.Net.Util
{
    public class ByteUtils
    {
        public static byte[] ToVarInt(long paramInt)
        {
            List<byte> bytes = new List<byte>();
            while ((paramInt & -128) != 0)
            {
                bytes.Add((byte)(paramInt & 127 | 128));
                paramInt = (int)(((uint)paramInt) >> 7);
            }
            bytes.Add((byte)paramInt);
            return bytes.ToArray();
        }

        public static byte[] ToLengthPrefixedString(string theString)
        {
            byte[] str = Encoding.UTF8.GetBytes(theString);
            byte[] len = ToVarInt(str.Length);
            byte[] packet = Concat(len, str);
            return packet;
        }

        public static byte[] Concat(params byte[][] bts)
        {
            List<Byte> bytes = new List<Byte>();
            foreach (byte[] item in bts)
            {
                foreach (byte b in item)
                {
                    bytes.Add(b);
                }
            }
            return bytes.ToArray();
        }
    }
}