namespace LibEmperor
{
	using System;
	using System.IO;

	public class XbfTriangle
	{
		public readonly int Vertex1;
		public readonly int Vertex2;
		public readonly int Vertex3;
		public readonly int Texture;
		public readonly float U1;
		public readonly float V1;
		public readonly float U2;
		public readonly float V2;
		public readonly float U3;
		public readonly float V3;

		public XbfTriangle(BinaryReader reader)
		{
			this.Vertex1 = reader.ReadInt32();
			this.Vertex2 = reader.ReadInt32();
			this.Vertex3 = reader.ReadInt32();

			this.Texture = reader.ReadInt32();

			// TODO implement this!
			var unk2 = reader.ReadInt32(); // smoothing? // bitmask?

			this.U1 = reader.ReadSingle();
			this.V1 = reader.ReadSingle();
			this.U2 = reader.ReadSingle();
			this.V2 = reader.ReadSingle();
			this.U3 = reader.ReadSingle();
			this.V3 = reader.ReadSingle();
		}
	}
}
