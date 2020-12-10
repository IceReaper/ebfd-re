namespace LibEmperor
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	public class Xbf
	{
		public readonly IEnumerable<string> Textures;
		public readonly List<XbfObject> Objects = new();

		public Xbf(byte[] bytes)
		{
			using var reader = new BinaryReader(new MemoryStream(bytes));

			var version = reader.ReadInt32();

			if (version != 1)
				throw new Exception("Unknown version!");

			// TODO parse this! (but it seems the data is not required at all...?)
			var unk1Size = reader.ReadInt32();
			reader.BaseStream.Position += unk1Size;

			this.Textures = new string(reader.ReadChars(reader.ReadInt32())).Split('\0').Where(s => s != "").ToArray();

			while (true)
			{
				var test = reader.ReadInt32();

				if (test == -1)
					break;

				reader.BaseStream.Position -= 4;
				this.Objects.Add(new XbfObject(reader));
			}

			if (reader.BaseStream.Position != reader.BaseStream.Length)
				throw new Exception("Missing data!");
		}
	}
}
