﻿using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Craft.Net.Util
{
    public static class ZlibUtils
    {
        /// <summary>
        /// Compress a byte array into another bytes array using Zlib compression
        /// </summary>
        /// <param name="to_compress">Data to compress</param>
        /// <returns>Compressed data as a byte array</returns>

        public static byte[] Compress(byte[] to_compress)
        {
            ZlibStream stream = new ZlibStream(new System.IO.MemoryStream(to_compress, false), CompressionMode.Compress);
            List<byte> temp_compression_list = new List<byte>();
            byte[] b = new byte[1];
            while (true)
            {
                int read = stream.Read(b, 0, 1);
                if (read > 0) { temp_compression_list.Add(b[0]); }
                else break;
            }
            stream.Close();
            return temp_compression_list.ToArray();
        }

        /// <summary>
        /// Decompress a byte array into another byte array of the specified size
        /// </summary>
        /// <param name="to_decompress">Data to decompress</param>
        /// <param name="size_uncompressed">Size of the data once decompressed</param>
        /// <returns>Decompressed data as a byte array</returns>

        public static byte[] Decompress(byte[] to_decompress, int size_uncompressed)
        {
            ZlibStream stream = new ZlibStream(new System.IO.MemoryStream(to_decompress, false), CompressionMode.Decompress);
            byte[] packetData_decompressed = new byte[size_uncompressed];
            stream.Read(packetData_decompressed, 0, size_uncompressed);
            stream.Close();
            return packetData_decompressed;
        }
    }
}
