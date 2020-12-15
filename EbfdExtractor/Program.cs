namespace EbfdExtractor
{
	using LibEmperor;
	using System;
	using System.IO;

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
			foreach (var file in Directory.GetDirectories(folder))
				Program.ProcessFolder(file);

			foreach (var file in Directory.GetFiles(folder))
			{
				var outPath = file.Substring(0, file.Length - 4);

				if (file.EndsWith(".RFH", StringComparison.OrdinalIgnoreCase))
				{
					var rfh = new Rfh(File.OpenRead(file), File.OpenRead(file.Substring(0, file.Length - 1) + (file.EndsWith("H") ? "D" : "d")));

					foreach (var rfhFile in rfh.Files)
					{
						var type = rfhFile.Path.Substring(rfhFile.Path.Length - 3).ToUpper();

						if (type == "TXT" || type == "INI")
							Program.Extract(outPath, rfhFile.Path, rfhFile.Read());
						else if (type == "TGA")
							Program.Extract(outPath, rfhFile.Path, rfhFile.Read());
						else if (type == "XBF")
							Program.Extract(outPath, rfhFile.Path, rfhFile.Read());
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
						else if (type == "LOG")
						{
							// Developer leftover error logfile
						}
						else
							throw new Exception($"Unknown type {type} !");
					}
				}
				else if (file.EndsWith(".BAG", StringComparison.OrdinalIgnoreCase))
				{
					var bag = new Bag(File.OpenRead(file));

					foreach (var bagFile in bag.Files)
						Program.Extract(outPath, bagFile.Path, bagFile.Read());
				}
			}
		}

		private static void Extract(string path, string name, byte[] bytes)
		{
			var finalPath = Path.Combine(path, $"{name}");
			Directory.CreateDirectory(Path.GetDirectoryName(finalPath)!);
			using var writer = new BinaryWriter(File.OpenWrite(finalPath));
			writer.Write(bytes);
		}
	}
}
