namespace XbfViewer.Graphics.Mesh
{
	using Cameras;
	using OpenTK.Mathematics;

	public class MeshInstance
	{
		private readonly Mesh mesh;

		public Matrix4 World = Matrix4.Identity;

		public MeshInstance(Mesh mesh)
		{
			this.mesh = mesh;
		}

		public void Draw(Camera camera)
		{
			this.mesh.Draw(camera, this.World);
		}
	}
}
