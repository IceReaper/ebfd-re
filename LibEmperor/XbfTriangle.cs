namespace LibEmperor
{
	using System.IO;
	using System.Numerics;

	public class XbfTriangle
	{
		public readonly int[] Vertices;
		public readonly int Texture;
		public readonly Vector2[] UV;

		public XbfTriangle(BinaryReader reader)
		{
			this.Vertices = new[] {reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32()};
			this.Texture = reader.ReadInt32();

			// TODO implement this! Bitmask?
			reader.ReadInt32();

			this.UV = new[]
			{
				new Vector2(reader.ReadSingle(), reader.ReadSingle()), new Vector2(reader.ReadSingle(), reader.ReadSingle()),
				new Vector2(reader.ReadSingle(), reader.ReadSingle())
			};
		}
	}
}
