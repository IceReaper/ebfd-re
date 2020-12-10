namespace LibEmperor
{
	using System.Collections.Generic;
	using System.IO;

	public class Rfh
	{
		public readonly List<RfhEntry> Files = new();

		public Rfh(Stream header, Stream data)
		{
			var headerReader = new BinaryReader(header);
			var dataReader = new BinaryReader(data);

			while (headerReader.BaseStream.Position < headerReader.BaseStream.Length)
				this.Files.Add(new RfhEntry(headerReader, dataReader));
		}
	}
}
