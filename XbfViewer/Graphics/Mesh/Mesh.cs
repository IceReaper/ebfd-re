namespace XbfViewer.Graphics.Mesh
{
	using Cameras;
	using OpenTK.Graphics.OpenGL4;
	using OpenTK.Mathematics;
	using Shaders;
	using System;

	public class Mesh : IDisposable
	{
		public string Name { get; protected set; }

		protected IShaderParameters? ShaderParameters;
		protected Matrix4? Transform;
		protected Mesh[]? Children;
		protected int NumIndices;
		protected int VertexBufferObject;
		protected int IndexBufferObject;
		protected int VertexArrayObject;

		protected Mesh()
		{
		}

		public Mesh(string name, IShaderParameters? shaderParameters, Matrix4? transform, Mesh[]? children, float[]? vertices, int[]? indices)
		{
			this.Name = name;
			this.Transform = transform;
			this.Children = children;

			if (shaderParameters == null || vertices == null || indices == null)
				return;

			this.ShaderParameters = shaderParameters;
			this.NumIndices = indices.Length;

			this.VertexBufferObject = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, this.VertexBufferObject);
			GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * 4, vertices, BufferUsageHint.StaticDraw);

			this.IndexBufferObject = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.IndexBufferObject);
			GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * 4, indices, BufferUsageHint.StaticDraw);

			this.VertexArrayObject = shaderParameters.CreateVertexArrayObject();
		}

		public void Draw(Camera camera, Matrix4 world)
		{
			var model = this.Transform != null ? this.Transform.Value * world : world;

			if (this.NumIndices > 0 && this.ShaderParameters != null)
			{
				this.ShaderParameters.Bind(model, camera.View, camera.Projection);

				GL.BindVertexArray(this.VertexArrayObject);
				GL.BindBuffer(BufferTarget.ArrayBuffer, this.VertexBufferObject);
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.IndexBufferObject);

				GL.DrawElements(BeginMode.Triangles, this.NumIndices, DrawElementsType.UnsignedInt, 0);
			}

			if (this.Children == null)
				return;

			foreach (var child in this.Children)
				child.Draw(camera, model);
		}

		public virtual void Dispose()
		{
			GL.DeleteBuffer(this.VertexArrayObject);
			GL.DeleteBuffer(this.IndexBufferObject);
			GL.DeleteBuffer(this.VertexBufferObject);

			if (this.Children != null)
				foreach (var child in this.Children)
					child.Dispose();

			GC.SuppressFinalize(this);
		}
	}
}
