namespace XbfViewer.Graphics.Mesh
{
	using Cameras;
	using OpenTK.Graphics.OpenGL4;
	using OpenTK.Mathematics;
	using Shaders;

	public class Mesh
	{
		private readonly IShaderParameters? shaderParameters;
		private readonly Matrix4? transform;
		private readonly Mesh[]? children;
		private readonly int numIndices;
		private readonly int vertexBufferObject;
		private readonly int indexBufferObject;
		private readonly int vertexArrayObject;

		public Mesh(IShaderParameters? shaderParameters, Matrix4? transform, Mesh[]? children, float[]? vertices, int[]? indices)
		{
			this.transform = transform;
			this.children = children;

			if (shaderParameters == null || vertices == null || indices == null)
				return;

			this.shaderParameters = shaderParameters;
			this.numIndices = indices.Length;

			this.vertexBufferObject = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBufferObject);
			GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * 4, vertices, BufferUsageHint.StaticDraw);

			this.indexBufferObject = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.indexBufferObject);
			GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * 4, indices, BufferUsageHint.StaticDraw);

			this.vertexArrayObject = shaderParameters.CreateVertexArrayObject();
		}

		public void Draw(Camera camera, Matrix4 world)
		{
			var model = this.transform != null ? this.transform.Value * world : world;

			if (this.numIndices > 0 && this.shaderParameters != null)
			{
				this.shaderParameters.Bind(model, camera.View, camera.Projection);

				GL.BindVertexArray(this.vertexArrayObject);
				GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBufferObject);
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.indexBufferObject);

				GL.DrawElements(BeginMode.Triangles, this.numIndices, DrawElementsType.UnsignedInt, 0);
			}

			if (this.children == null)
				return;

			foreach (var child in this.children)
				child.Draw(camera, model);
		}

		public void Dispose()
		{
			GL.DeleteBuffer(this.vertexArrayObject);
			GL.DeleteBuffer(this.indexBufferObject);
			GL.DeleteBuffer(this.vertexBufferObject);

			this.shaderParameters?.Dispose();

			if (this.children == null)
				return;

			foreach (var child in this.children)
				child.Dispose();
		}
	}
}
