using System;
using System.IO;
using System.Security.Cryptography;

namespace Lab3
{
    static class Program
    {
        static void Main(string[] args)
        {
            string path = @"c:\Users\forss\Desktop\BlackP.png";

            if (!File.Exists(path))
            {
                Console.WriteLine("File not found.");
                return;
            }

            FileStream dataStream = new FileStream(path, FileMode.Open);
            BinaryReader interpreter = new BinaryReader(dataStream);
            short maybeBM = interpreter.ReadInt16();

            if (maybeBM == 19778)
            {
                dataStream.Position = 18;
                int width = interpreter.ReadInt32();
                int height = interpreter.ReadInt32();
                Console.WriteLine($"This is a .bmp image. Resolution: {width}x{height} pixels.");
                return;
            }

            dataStream.Position = 0;
            long maybePNG = interpreter.ReadInt64();

            if (maybePNG == 727905341920923785)
            {
                dataStream.Position = 16; //PNG image, consisting of a series of chunks beginning with an IHDR chunk.
                int width = interpreter.ReadInt32BigEndian();
                int height = interpreter.ReadInt32BigEndian();
                Console.WriteLine($"This is a .png image. Resolution: {width}x{height} pixels.");

                dataStream.Position = 8;
                while (dataStream.Position < dataStream.Length)
                {
                    PrintTypeNLengthPNG(dataStream, interpreter);
                }

                return;
            }

            Console.WriteLine("This is not a valid .bmp or .png file!");
        }

        static void PrintTypeNLengthPNG(FileStream dataStream, BinaryReader interpreter)
        {
            int ChunkDataLength = interpreter.ReadInt32BigEndian();
            string ChunkType = System.Text.Encoding.ASCII.GetString(interpreter.ReadBytes(4));

            Console.WriteLine($"Type: {ChunkType}    Length of chunk data: {ChunkDataLength}");

            dataStream.Position += ChunkDataLength + 4;
        }

        static int ReadInt32BigEndian(this BinaryReader interpreter)
        {
            Byte[] bytes = interpreter.ReadBytes(4);
            Byte[] reversed = new Byte[bytes.Length];
            int rIndex = 0;

            for (int i = bytes.Length - 1; i > -1; i--)
            {
                reversed[rIndex] = bytes[i];
                rIndex++;
            }

            return BitConverter.ToInt32(reversed);
        }
    }
}
