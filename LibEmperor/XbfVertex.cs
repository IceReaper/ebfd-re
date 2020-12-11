namespace LibEmperor
{
	using System.IO;

	public class XbfVertex
	{
		public readonly float X;
		public readonly float Y;
		public readonly float Z;
		public readonly float NormalX;
		public readonly float NormalY;
		public readonly float NormalZ;

		public XbfVertex(BinaryReader reader)
		{
			this.X = reader.ReadSingle();
			this.Y = reader.ReadSingle();
			this.Z = reader.ReadSingle();
			this.NormalX = reader.ReadSingle();
			this.NormalY = reader.ReadSingle();
			this.NormalZ = reader.ReadSingle();
		}
	}
}
