namespace EbfdExtractor
{
	using System;
	using System.IO;
	using System.IO.Compression;
	using System.Text;

	internal static class Program
	{
		private static void Main(string[] args)
		{
			if (args.Length == 0)
				return;

			Program.ProcessFolder(args[0]);
		}

		private static void ProcessFolder(string folder)
		{
			foreach (var f in Directory.GetDirectories(folder))
				Program.ProcessFolder(f);

			foreach (var f in Directory.GetFiles(folder))
			{
				if (f.ToUpper().EndsWith(".RFH"))
					Program.ExtractRfh(f);
				else if (f.ToUpper().EndsWith(".BAG"))
					Program.ExtractBag(f);
			}
		}

		private static void ExtractRfh(string path)
		{
			using var headerReader = new BinaryReader(File.OpenRead(path));
			using var dataReader = new BinaryReader(File.OpenRead(path.Substring(0, path.Length - 1) + (path.EndsWith("H") ? "D" : "d")));

			while (headerReader.BaseStream.Position < headerReader.BaseStream.Length)
			{
				var nameLength = headerReader.ReadInt32();
				headerReader.ReadInt32(); // dateTime
				var flags = headerReader.ReadInt32();
				var compressedSize = headerReader.ReadInt32();
				var uncompressedSize = headerReader.ReadInt32();
				var offset = headerReader.ReadInt32();
				var name = new string(headerReader.ReadChars(nameLength)).Split('\0')[0];

				if ((flags & 0b11111111111111111111111111111101) != 0)
					throw new Exception("Unknown flags!");

				var isCompressed = (flags & 0b10) != 0;

				dataReader.BaseStream.Position = offset + 6;
				byte[] bytes;

				if (isCompressed)
				{
					var deflateStream = new DeflateStream(dataReader.BaseStream, CompressionMode.Decompress);
					bytes = new byte[uncompressedSize];
					deflateStream.Read(bytes);
				}
				else
					bytes = dataReader.ReadBytes(compressedSize);

				var type = name.Substring(name.Length - 3).ToUpper();

				if (type == "TXT" || type == "INI")
					Program.Extract(path, name, bytes);
				else if (type == "TGA")
					Program.Extract(path, name, bytes);
				else if (type == "XBF")
					Program.Extract(path, name, XbfToFbx.Convert(new XbfFile(bytes)));
				else if (type == "TOK")
				{
					// TODO Mission
				}
				else if (type == "MAP")
				{
					// TODO Font
				}
				else if (type == "LIT")
				{
					// TODO Map.Lights
				}
				else if (type == "INF")
				{
					// TODO Map.Info NOT a text file
				}
				else if (type == "DAT")
				{
					// TODO Map.??? shows terrain in hex editor
				}
				else if (type == "CPT")
				{
					// TODO Map.??? list with a width of 16 bytes
				}
				else if (type == "CPF")
				{
					// TODO Map.??? shows terrain in hex editor
				}
				else if (type == "XAF")
				{
					// Developer leftover uncompiled XBF
				}
				else if (type == "BAK")
				{
					// Developer leftover file backup
				}
				else
					throw new Exception("Unknown type!");
			}
		}

		private static void ExtractBag(string path)
		{
			using var reader = new BinaryReader(File.OpenRead(path));
			var magic = new string(reader.ReadChars(4));
			var version = reader.ReadInt32();
			var numSounds = reader.ReadInt32();
			var stride = reader.ReadInt32();

			if (magic != "GABA")
				throw new Exception("Invalid magic!");

			if (version != 4)
				throw new Exception("Unknown version!");

			for (var i = 0; i < numSounds; i++)
			{
				var start = reader.BaseStream.Position;

				var name = new string(reader.ReadChars(32)).Split('\0')[0];
				var offset = reader.ReadInt32();
				var length = reader.ReadInt32();
				var sampleRate = reader.ReadInt32();
				var flags = reader.ReadInt32();
				var formatFlags = reader.ReadInt32();

				if ((flags & 0b11111111111111111111111111000000) != 0)
					throw new Exception("Unknown flags!");

				var isStereo = (flags & 0b000001) != 0;
				var isUncompressed = (flags & 0b000010) != 0;
				var is16Bit = (flags & 0b000100) != 0;
				var isCompressed = (flags & 0b001000) != 0;
				var isUnk = (flags & 0b010000) != 0;
				var isMp3 = (flags & 0b100000) != 0;

				if (isMp3)
				{
					// TODO formatFlags look like 2 shorts here.
					// The first one is either 4 on ingame tracks or 112 on menu and score. Possibly loop offset information?
					// The last one is always -13.

					reader.BaseStream.Position = offset;
					Program.Extract(path, $"{name}.mp3", reader.ReadBytes(length));
					reader.BaseStream.Position = start + stride;
				}
				else
				{
					using var stream = new MemoryStream();
					using var writer = new BinaryWriter(stream);

					writer.Write(Encoding.ASCII.GetBytes("RIFF"));
					writer.Write(length * (isCompressed ? 4 : 1) + 36);
					writer.Write(Encoding.ASCII.GetBytes("WAVE"));
					writer.Write(Encoding.ASCII.GetBytes("fmt "));
					writer.Write(16);
					writer.Write((short) 1);
					writer.Write((short) (isStereo ? 2 : 1));
					writer.Write(sampleRate);
					writer.Write(sampleRate * (is16Bit ? 2 : 1) * (isStereo ? 2 : 1));
					writer.Write((short) (is16Bit ? 2 : 1));
					writer.Write((short) (is16Bit ? 16 : 8));
					writer.Write(Encoding.ASCII.GetBytes("data"));
					writer.Write(length * (isCompressed ? 4 : 1));

					reader.BaseStream.Position = offset;

					if (isCompressed)
					{
						for (var j = 0; j < length; j++)
						{
							var compressed = reader.ReadByte();

							// TODO sound is properly decompressed but has continuous noise.
							writer.Write((short) (((compressed >> 4) & 0b1111) * formatFlags));
							writer.Write((short) (((compressed >> 0) & 0b1111) * formatFlags));
						}
					}
					else if (isUncompressed)
						writer.Write(reader.ReadBytes(length));
					else
						throw new Exception("Unknown flags combination!");

					reader.BaseStream.Position = start + stride;

					Program.Extract(path, $"{name}.wav", stream.GetBuffer());
				}
			}
		}

		private static void Extract(string path, string name, byte[] bytes)
		{
			var finalPath = Path.Combine(path.Substring(0, path.Length - 4), $"{name}");
			Directory.CreateDirectory(Path.GetDirectoryName(finalPath));
			using var writer = new BinaryWriter(File.OpenWrite(finalPath));
			writer.Write(bytes);
		}
	}
}
