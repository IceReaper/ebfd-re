namespace XbfViewer.Graphics.Mesh
{
	using Cameras;
	using OpenTK.Mathematics;
	using System;
	using System.Linq;

	public class MeshInstance
	{
		private readonly Mesh mesh;
		private readonly double totalFrames;

		public Matrix4 World = Matrix4.Identity;
		public double Speed = 1;

		private double frame;

		public MeshInstance(Mesh mesh)
		{
			this.mesh = mesh;
			this.totalFrames = this.GetLongestAnimation(this.mesh);
		}

		private int GetLongestAnimation(Mesh mesh)
		{
			var frames = mesh.TransformAnimation?.Length ?? 1;

			if (mesh.Children != null && mesh.Children.Any())
				frames = Math.Max(frames, mesh.Children.Max(this.GetLongestAnimation));

			return frames;
		}

		public void Update(double argsTime)
		{
			this.frame += argsTime * this.Speed;
		}

		public void Draw(Camera camera)
		{
			this.mesh.Draw(camera, this.World, this.frame % this.totalFrames);
		}
	}
}
