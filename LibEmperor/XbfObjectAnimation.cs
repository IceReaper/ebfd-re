namespace LibEmperor
{
	using System;
	using System.Collections.Generic;
	using System.IO;

	public class XbfObjectAnimation
	{
		public readonly int Length;
		public readonly Dictionary<int, float[]> Frames = new();

		public XbfObjectAnimation(BinaryReader reader)
		{
			this.Length = reader.ReadInt32() + 1;
			var usedFrames = reader.ReadInt32();

			if (usedFrames == -1)
				for (var i = 0; i < this.Length; i++)
					this.Frames.Add(
						i,
						new[]
						{
							reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(),
							reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(),
							reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()
						}
					);
			else if (usedFrames == -2)
				for (var i = 0; i < this.Length; i++)
					this.Frames.Add(
						i,
						new[]
						{
							reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0, reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(),
							0, reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0, reader.ReadSingle(), reader.ReadSingle(),
							reader.ReadSingle(), 1
						}
					);
			else if (usedFrames == -3)
			{
				var matrices = new float[reader.ReadInt32()][];
				var frames = new short[this.Length];

				for (var i = 0; i < frames.Length; i++)
					frames[i] = reader.ReadInt16();

				for (var i = 0; i < matrices.Length; i++)
					matrices[i] = new[]
					{
						reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0, reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0,
						reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0, reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 1
					};

				for (var i = 0; i < this.Length; i++)
					this.Frames.Add(i, matrices[frames[i]]);
			}
			else
			{
				for (var i = 0; i < usedFrames; i++)
				{
					var frameId = reader.ReadInt16();
					var flags = reader.ReadInt16();

					if ((flags & 0b1000111111111111) != 0)
						throw new Exception("Unknown flags!");

					var frame = new float[] {1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1};

					if (((flags >> 12) & 0b001) != 0)
					{
						var x = reader.ReadSingle();
						var y = reader.ReadSingle();
						var z = reader.ReadSingle();
						var w = reader.ReadSingle();

						var m = 2 / (x * x + y * y + z * z + w * w);
						frame[0] *= 1 - m * (y * y + z * z);
						frame[1] *= m * (x * y + z * w);
						frame[2] *= m * (x * z - y * w);
						frame[4] *= m * (x * y - z * w);
						frame[5] *= 1 - m * (x * x + z * z);
						frame[6] *= m * (y * z + x * w);
						frame[8] *= m * (x * z + y * w);
						frame[9] *= m * (y * z - x * w);
						frame[10] *= 1 - m * (x * x + y * y);
					}

					if (((flags >> 12) & 0b010) != 0)
					{
						frame[0] *= reader.ReadSingle();
						frame[5] *= reader.ReadSingle();
						frame[10] *= reader.ReadSingle();
					}

					if (((flags >> 12) & 0b100) != 0)
					{
						frame[12] *= reader.ReadSingle();
						frame[13] *= reader.ReadSingle();
						frame[14] *= reader.ReadSingle();
					}

					this.Frames.Add(frameId, frame);
				}
			}
		}
	}
}
