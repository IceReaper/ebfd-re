namespace LibEmperor
{
	using System;
	using System.IO;

	public class Tga
	{
		public readonly ushort Width;
		public readonly ushort Height;
		public readonly byte[] Pixels;

		public Tga(Stream stream)
		{
			using var reader = new BinaryReader(stream);

			if (reader.ReadByte() != 0x00)
				throw new Exception("Unsupported IDLength");

			if (reader.ReadByte() != 0x00)
				throw new Exception("Unsupported ColorMapType");

			var imageType = reader.ReadByte();

			// TODO fix those!
			if (imageType == 0x00)
			{
				this.Width = 1;
				this.Height = 1;
				this.Pixels = new byte[] {0xff, 0x00, 0x00, 0xff};

				return;
			}

			if (imageType != 0x02)
				throw new Exception("Unsupported ImageType");

			if (reader.ReadUInt16() != 0x0000)
				throw new Exception("Unsupported FirstIndexEntry");

			if (reader.ReadUInt16() != 0x0000)
				throw new Exception("Unsupported ColorMapLength");

			if (reader.ReadByte() != 0x00)
				throw new Exception("Unsupported ColorMapEntrySize");

			if (reader.ReadUInt16() != 0x0000)
				throw new Exception("Unsupported XOrigin");

			if (reader.ReadUInt16() != 0x0000)
				throw new Exception("Unsupported YOrigin");

			this.Width = reader.ReadUInt16();
			this.Height = reader.ReadUInt16();

			var pixelDepth = reader.ReadByte();

			if (pixelDepth != 16 && pixelDepth != 24 && pixelDepth != 32)
				throw new Exception("Unsupported PixelDepth");

			this.Pixels = new byte[this.Width * this.Height * 4];

			for (var i = 0; i < this.Pixels.Length; i += 4)
			{
				if (pixelDepth == 32)
				{
					this.Pixels[i + 3] = reader.ReadByte();
					this.Pixels[i + 2] = reader.ReadByte();
					this.Pixels[i + 1] = reader.ReadByte();
					this.Pixels[i + 0] = reader.ReadByte();
				}

				if (pixelDepth == 24)
				{
					this.Pixels[i + 3] = 0xff;
					this.Pixels[i + 0] = reader.ReadByte();
					this.Pixels[i + 2] = reader.ReadByte();
					this.Pixels[i + 1] = reader.ReadByte();
				}
				else if (pixelDepth == 16)
				{
					var color16 = (reader.ReadByte() << 8) | reader.ReadByte();

					this.Pixels[i + 3] = (byte) (((color16 >> 15) & 0x01) * 0xff);
					this.Pixels[i + 2] = (byte) (((color16 >> 0) & 0x1f) * 0xff / 0x1f);
					this.Pixels[i + 1] = (byte) (((color16 >> 5) & 0x1f) * 0xff / 0x1f);
					this.Pixels[i + 0] = (byte) (((color16 >> 10) & 0x1f) * 0xff / 0x1f);
				}
			}
		}
	}
}
