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
						Program.Extract(outPath, rfhFile.Path, rfhFile.Read());
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
