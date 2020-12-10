namespace LibEmperor
{
	using System;
	using System.IO;

	public class XbfKeyAnimation
	{
		public XbfKeyAnimation(BinaryReader reader)
		{
			// TODO implement those!
			var unk1 = reader.ReadInt32();
			var unk2 = reader.ReadInt32();

			if (unk2 == -1)
			{
				var unk3 = reader.ReadBytes((unk1 + 1) * 64);
			}
			else if (unk2 == -2)
			{
				var unk3 = reader.ReadBytes((unk1 + 1) * 48);
			}
			else if (unk2 == -3)
			{
				var unk3 = reader.ReadInt32();
				var unk4 = reader.ReadBytes((unk1 + 1) * 2);

				for (var i = 0; i < unk3; i++)
				{
					var unk5 = reader.ReadBytes(48);
				}
			}
			else
			{
				for (var i = 0; i < unk2; i++)
				{
					var unk3 = reader.ReadInt16();
					var flags = reader.ReadInt16();

					if (((flags >> 12) & 0b001) != 0)
					{
						var unk4 = reader.ReadBytes(16);
					}

					if (((flags >> 12) & 0b010) != 0)
					{
						var unk5 = reader.ReadBytes(12);
					}

					if (((flags >> 12) & 0b100) != 0)
					{
						var unk6 = reader.ReadBytes(12);
					}

					if ((flags & 0b1000111111111111) != 0)
						throw new Exception("Unknown flags!");
				}
			}
		}
	}
}
