using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Redips.Services
{
    class Compression
    {
        public static bool Compress(string text, out byte[] bytes)
        {
            try
            {
                var unicodeEncoding = new UnicodeEncoding();
                var byteArray = unicodeEncoding.GetBytes(text);

                int byteArrayLength = byteArray.Length, textLen = text.Length;

                var ms = new MemoryStream(byteArray);
                var ds = new DeflateStream(ms, CompressionLevel.Optimal);

                ds.CopyTo(ms);
                long length = ms.Length;
                var data = new Byte[length];
                ms.Read(data, 0, (int)length);
                bytes = data;
                return true;
            }
            catch (Exception)
            {
                bytes = null;
                return false;
            }
        }

        public static bool Decompress(byte[] bytes, out string text)
        {
            try
            {
                var ms = new MemoryStream(bytes);
                var gzs = new GZipStream(ms, CompressionLevel.Optimal);

                //while (gzs.() != -1)
                //{
                //    length++;
                //    br.ReadByte();
                //}
                //var data = new Byte[length];
                //br.Read(data, 0, length);
                //bytes = data;
                text = null;
                return true;
            }
            catch (Exception)
            {
                text = null;
                return false;
            }
        }
    }
}
