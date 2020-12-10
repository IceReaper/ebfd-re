namespace LibEmperor
{
	using System;
	using System.IO;

	public class XbfVertexAnimation
	{
		public XbfVertexAnimation(BinaryReader reader)
		{
			// TODO implement those!
			var unk1 = reader.ReadInt32();
			var unk2 = reader.ReadInt32();
			var unk3 = reader.ReadInt32();

			for (var i = 0; i < unk3; i++)
			{
				var unk4 = reader.ReadInt32();
			}

			if (unk2 >= 0)
				return;

			var unk5 = reader.ReadInt16();
			var flags = reader.ReadUInt16();
			var unk6 = reader.ReadInt32();

			if ((flags & 0b0111111111111111) != 0)
				throw new Exception("Unknown flags!");

			if (-unk2 != unk6)
				throw new Exception("-unk2 and unk6 differ!");

			var unk7 = reader.ReadBytes(unk6 * 8);

			if (((flags >> 12) & 0b1000) != 0)
			{
				var unk8 = reader.ReadBytes(unk1 * 4);
			}
		}
	}
}
