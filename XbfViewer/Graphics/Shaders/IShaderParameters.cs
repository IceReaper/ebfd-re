namespace XbfViewer.Graphics.Shaders
{
	using OpenTK.Mathematics;
	using System;

	public interface IShaderParameters : IDisposable
	{
		public void Bind(Matrix4 model, Matrix4 view, Matrix4 projection);
		public int CreateVertexArrayObject();
	}
}
