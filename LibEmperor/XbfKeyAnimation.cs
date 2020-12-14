namespace LibEmperor
{
	using System;
	using System.IO;

	public class XbfKeyAnimation
	{
		public readonly float[][] Animation;

		public XbfKeyAnimation(BinaryReader reader)
		{
			var lastFrame = reader.ReadInt32();
			var usedFrames = reader.ReadInt32();

			if (usedFrames == -1)
			{
				this.Animation = new float[lastFrame + 1][];

				for (var i = 0; i <= lastFrame; i++)
					this.Animation[i] = new[]
					{
						reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(),
						reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(),
						reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()
					};
			}
			else if (usedFrames == -2)
			{
				this.Animation = new float[lastFrame + 1][];

				for (var i = 0; i <= lastFrame; i++)
					this.Animation[i] = new[]
					{
						reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0, reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0,
						reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0, reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 1
					};
			}
			else if (usedFrames == -3)
			{
				// TODO it is possible, we need to use totalFrames here, and inject the ones defined by FrameId list.
				this.Animation = new float[reader.ReadInt32()][];

				for (var i = 0; i <= lastFrame; i++)
					reader.ReadInt16(); // FrameId

				for (var i = 0; i < this.Animation.Length; i++)
					this.Animation[i] = new[]
					{
						reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0, reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0,
						reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0, reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 1
					};
			}
			else
			{
				this.Animation = new float[usedFrames][];

				for (var i = 0; i < usedFrames; i++)
				{
					// TODO assemble matrix.
					var unk3 = reader.ReadInt16();
					var flags = reader.ReadInt16();

					// TODO Rotation?
					if (((flags >> 12) & 0b001) != 0)
					{
						var unk4a = reader.ReadSingle();
						var unk4b = reader.ReadSingle();
						var unk4c = reader.ReadSingle();
						var unk4d = reader.ReadSingle();
					}

					// TODO Scale?
					if (((flags >> 12) & 0b010) != 0)
					{
						var unk5a = reader.ReadSingle();
						var unk5b = reader.ReadSingle();
						var unk5c = reader.ReadSingle();
					}

					// TODO Position?
					if (((flags >> 12) & 0b100) != 0)
					{
						var unk6a = reader.ReadSingle();
						var unk6b = reader.ReadSingle();
						var unk6c = reader.ReadSingle();
					}

					if ((flags & 0b1000111111111111) != 0)
						throw new Exception("Unknown flags!");
				}
			}
		}
	}
}
