using System;
using System.IO;
using System.Text;

namespace Lab3
{
    static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Beep! Jag tar exakt ett argument.");
                return;
            }

            string path = args[0];

            if (!File.Exists(path))
            {
                Console.WriteLine("File not found.");
                return;
            }

            PrintImageMetadata(path);
        }

        static void PrintImageMetadata(string path)
        {
            FileStream dataStream = new FileStream(path, FileMode.Open);
            BinaryReader reader = new BinaryReader(dataStream);
            short maybeBM = reader.ReadInt16();

            if (maybeBM == 19778)
            {
                dataStream.Position = 18;
                int width = reader.ReadInt32();
                int height = reader.ReadInt32();
                Console.WriteLine($"This is a .bmp image. Resolution: {width}x{height} pixels.");
                return;
            }

            dataStream.Position = 0;
            long maybePNG = reader.ReadInt64();

            if (maybePNG == 727905341920923785)
            {
                dataStream.Position = 16;
                int width = reader.ReadInt32BigEndian();
                int height = reader.ReadInt32BigEndian();
                Console.WriteLine($"This is a .png image. Resolution: {width}x{height} pixels.");
                Console.WriteLine();

                dataStream.Position = 8;
                Console.WriteLine("---- Chunk list ----");
                while (dataStream.Position < dataStream.Length)
                {
                    PrintTypeNLengthPNG(dataStream, reader);
                }

                return;
            }

            Console.WriteLine("This is not a valid .bmp or .png file!");
        }

        static void PrintTypeNLengthPNG(FileStream dataStream, BinaryReader reader)
        {
            int chunkDataLength = reader.ReadInt32BigEndian();
            string chunkType = Encoding.ASCII.GetString(reader.ReadBytes(4));

            Console.WriteLine($"Type: {chunkType}      Length of chunk: {chunkDataLength + 12} bytes");

            dataStream.Position += chunkDataLength + 4;
        }

        static int ReadInt32BigEndian(this BinaryReader reader)
        {
            byte[] bytes = reader.ReadBytes(4);
            byte[] reversed = new byte[bytes.Length];
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