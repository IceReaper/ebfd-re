namespace XbfViewer.Graphics.Vertices
{
	using OpenTK.Mathematics;

	public readonly struct VertexPositionNormalUv
	{
		public readonly Vector3 Position;
		public readonly Vector3 Normal;
		public readonly Vector2 Uv;

		public VertexPositionNormalUv(Vector3 position, Vector3 normal, Vector2 uv)
		{
			this.Position = position;
			this.Normal = normal;
			this.Uv = uv;
		}

		public float[] Pack()
		{
			return new[] {this.Position.X, this.Position.Y, this.Position.Z, this.Normal.X, this.Normal.Y, this.Normal.Z, this.Uv.X, this.Uv.Y};
		}
	}
}
