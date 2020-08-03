namespace dunetest
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	public class XbfFile
	{
		public byte[] Temp;
		public XbfHeader Header;
		public IEnumerable<string> Textures;
		public List<XbfObject> Objects = new List<XbfObject>();

		public XbfFile(byte[] bytes)
		{
			this.Temp = bytes;

			using var reader = new BinaryReader(new MemoryStream(bytes));

			this.Header = new XbfHeader(reader);
			this.Textures = new string(reader.ReadChars(reader.ReadInt32())).Split('\0').Where(s => s != "").ToArray();

			while (true)
			{
				var test = reader.ReadInt32();

				if (test == -1)
					break;

				reader.BaseStream.Position -= 4;

				this.Objects.Add(new XbfObject(reader));
			}
		}
	}

	public class XbfHeader
	{
		public int Version;

		public XbfHeader(BinaryReader reader)
		{
			this.Version = reader.ReadInt32();

			// TODO parse the header! (but it seems the data is not required at all...?)
			var unk1Size = reader.ReadInt32();
			reader.BaseStream.Position += unk1Size;
		}
	}

	public class XbfObject
	{
		public XbfVertex[] Vertices;
		public XbfFace[] Faces;
		public XbfObject[] Children;
		public double[] Transform = new double[4 * 4];
		public string Name;
		public XbfVertexAnimation VertexAnimation;
		public XbfKeyAnimation KeyAnimation;

		public XbfObject(BinaryReader reader)
		{
			this.Vertices = new XbfVertex[reader.ReadInt32()];
			var flags = reader.ReadInt32();
			this.Faces = new XbfFace[reader.ReadInt32()];
			this.Children = new XbfObject[reader.ReadInt32()];

			for (var i = 0; i < this.Transform.Length; i++)
				this.Transform[i] = reader.ReadDouble();

			this.Name = new string(reader.ReadChars(reader.ReadInt32())).Split('\0')[0];

			for (var i = 0; i < this.Children.Length; i++)
				this.Children[i] = new XbfObject(reader);

			for (var i = 0; i < this.Vertices.Length; i++)
				this.Vertices[i] = new XbfVertex(reader);

			for (var i = 0; i < this.Faces.Length; i++)
				this.Faces[i] = new XbfFace(reader);

			// TODO identify this
			if ((flags & 0b0001) != 0)
				for (var i = 0; i < this.Vertices.Length; i++)
					reader.ReadBytes(3);

			// TODO identify this
			if ((flags & 0b0010) != 0)
				for (var i = 0; i < this.Faces.Length; i++)
					reader.ReadInt32();

			if ((flags & 0b0100) != 0)
				this.VertexAnimation = new XbfVertexAnimation(reader);

			if ((flags & 0b1000) != 0)
				this.KeyAnimation = new XbfKeyAnimation(reader);
		}
	}

	public class XbfVertex
	{
		public double X;
		public double Y;
		public double Z;

		public XbfVertex(BinaryReader reader)
		{
			this.X = reader.ReadDouble();
			this.Y = reader.ReadDouble();
			this.Z = reader.ReadDouble();
		}
	}

	public class XbfFace
	{
		public int Vertex1;
		public int Vertex2;
		public int Vertex3;
		public float U1;
		public float V1;
		public float U2;
		public float V2;
		public float U3;
		public float V4;

		public XbfFace(BinaryReader reader)
		{
			this.Vertex1 = reader.ReadInt32();
			this.Vertex2 = reader.ReadInt32();
			this.Vertex3 = reader.ReadInt32();

			// TODO implement those!
			var unk1 = reader.ReadInt32(); // texture?
			var unk2 = reader.ReadInt32(); // smoothing?

			this.U1 = reader.ReadSingle();
			this.V1 = reader.ReadSingle();
			this.U2 = reader.ReadSingle();
			this.V2 = reader.ReadSingle();
			this.U3 = reader.ReadSingle();
			this.V4 = reader.ReadSingle();
		}
	}

	public class XbfVertexAnimation
	{
		public XbfVertexAnimation(BinaryReader reader)
		{
			// TODO fix this!
			// TODO implement those!
			var unk1 = reader.ReadInt32();
			var unk2 = reader.ReadInt32();
			var unk3 = reader.ReadInt32();

			for (var i = 0; i < unk3; i++)
			{
				var unk4 = reader.ReadInt32();
			}
			
			//TODO there is more here...
		}
	}

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
			else if (unk2 > 0)
			{
				for (var i = 0; i < unk2; i++)
				{
					var unk3 = reader.ReadInt16();
					var unk4 = reader.ReadInt16() >> 12;

					if ((unk4 & 0b001) != 0)
					{
						var unk5 = reader.ReadBytes(16);
					}

					if ((unk4 & 0b010) != 0)
					{
						var unk5 = reader.ReadBytes(12);
					}

					if ((unk4 & 0b100) != 0)
					{
						var unk5 = reader.ReadBytes(12);
					}
				}
			}
		}
	}
}
