namespace LibEmperor
{
	using System;
	using System.IO;
	using System.Text;

	public class BagEntry
	{
		[Flags]
		private enum Flags
		{
			Stereo = 1,
			Uncompressed = 2,
			Is16Bit = 4,
			Compressed = 8,
			Unk = 16,
			Mp3 = 32
		}

		private readonly BinaryReader reader;

		public readonly string Path;

		private readonly int offset;
		private readonly int length;
		private readonly int sampleRate;
		private readonly Flags flags;
		private readonly int unk;

		public BagEntry(BinaryReader reader)
		{
			this.reader = reader;
			this.Path = new string(reader.ReadChars(32)).Split('\0')[0];
			this.offset = reader.ReadInt32();
			this.length = reader.ReadInt32();
			this.sampleRate = reader.ReadInt32();
			this.flags = (Flags) reader.ReadInt32();
			this.unk = reader.ReadInt32();

			if ((int) this.flags >> 6 != 0)
				throw new Exception("Unknown flags!");

			this.Path += (this.flags & Flags.Mp3) != 0 ? ".mp3" : ".wav";
		}

		public byte[] Read()
		{
			if ((this.flags & Flags.Mp3) != 0)
			{
				// TODO formatFlags look like 2 shorts here.
				// The first one is either 4 on ingame tracks or 112 on menu and score. Possibly loop offset information?
				// The last one is always -13.

				this.reader.BaseStream.Position = this.offset;

				return this.reader.ReadBytes(this.length);
			}

			var compressed = (this.flags & Flags.Compressed) != 0;
			var uncompressed = (this.flags & Flags.Uncompressed) != 0;

			if (compressed && uncompressed || !compressed && !uncompressed)
				throw new Exception("Unknown flags combination!");

			using var stream = new MemoryStream();
			using var writer = new BinaryWriter(stream);

			writer.Write(Encoding.ASCII.GetBytes("RIFF"));
			writer.Write(this.length * (compressed ? 4 : 1) + 36);
			writer.Write(Encoding.ASCII.GetBytes("WAVE"));
			writer.Write(Encoding.ASCII.GetBytes("fmt "));
			writer.Write(16);
			writer.Write((short) 1);
			writer.Write((short) ((this.flags & Flags.Stereo) != 0 ? 2 : 1));
			writer.Write(this.sampleRate);
			writer.Write(this.sampleRate * ((this.flags & Flags.Is16Bit) != 0 ? 2 : 1) * ((this.flags & Flags.Stereo) != 0 ? 2 : 1));
			writer.Write((short) ((this.flags & Flags.Is16Bit) != 0 ? 2 : 1));
			writer.Write((short) ((this.flags & Flags.Is16Bit) != 0 ? 16 : 8));
			writer.Write(Encoding.ASCII.GetBytes("data"));
			writer.Write(this.length * (compressed ? 4 : 1));

			this.reader.BaseStream.Position = this.offset;

			if (compressed)
			{
				for (var j = 0; j < this.length; j++)
				{
					var value = this.reader.ReadByte();

					// TODO sound is properly decompressed but has continuous noise.
					writer.Write((short) (((value >> 4) & 0b1111) * this.unk));
					writer.Write((short) (((value >> 0) & 0b1111) * this.unk));
				}
			}
			else
				writer.Write(this.reader.ReadBytes(this.length));

			return stream.GetBuffer();
		}
	}
}
