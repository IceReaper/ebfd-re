namespace LibEmperor
{
	using System.IO;

	public class XbfVertex
	{
		public readonly double X;
		public readonly double Y;
		public readonly double Z;

		public XbfVertex(BinaryReader reader)
		{
			this.X = reader.ReadDouble();
			this.Y = reader.ReadDouble();
			this.Z = reader.ReadDouble();
		}
	}
}
