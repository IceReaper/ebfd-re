namespace LibEmperor
{
	using System;
	using System.IO;

	public class XbfObject
	{
		[Flags]
		private enum Flags
		{
			Unk1 = 1,
			Unk2 = 2,
			VertexAnimation = 4,
			KeyAnimation = 8
		}

		public readonly XbfVertex[] Vertices;
		public readonly XbfTriangle[] Triangles;
		public readonly XbfObject[] Children;
		public readonly double[] Transform = new double[4 * 4];
		public readonly string Name;
		public readonly XbfVertexAnimation VertexAnimation;
		public readonly XbfKeyAnimation KeyAnimation;

		public XbfObject(BinaryReader reader)
		{
			this.Vertices = new XbfVertex[reader.ReadInt32()];

			var flags = (Flags) reader.ReadInt32();

			if ((int) flags >> 4 != 0)
				throw new Exception("Unknown flags!");

			this.Triangles = new XbfTriangle[reader.ReadInt32()];
			this.Children = new XbfObject[reader.ReadInt32()];

			for (var i = 0; i < this.Transform.Length; i++)
				this.Transform[i] = reader.ReadDouble();

			this.Name = new string(reader.ReadChars(reader.ReadInt32())).Split('\0')[0];

			for (var i = 0; i < this.Children.Length; i++)
				this.Children[i] = new XbfObject(reader);

			for (var i = 0; i < this.Vertices.Length; i++)
				this.Vertices[i] = new XbfVertex(reader);

			for (var i = 0; i < this.Triangles.Length; i++)
				this.Triangles[i] = new XbfTriangle(reader);

			// TODO identify this
			if ((flags & Flags.Unk1) != 0)
				for (var i = 0; i < this.Vertices.Length; i++)
					reader.ReadBytes(3);

			// TODO identify this
			if ((flags & Flags.Unk2) != 0)
				for (var i = 0; i < this.Triangles.Length; i++)
					reader.ReadInt32();

			if ((flags & Flags.VertexAnimation) != 0)
				this.VertexAnimation = new XbfVertexAnimation(reader);

			if ((flags & Flags.KeyAnimation) != 0)
				this.KeyAnimation = new XbfKeyAnimation(reader);
		}
	}
}