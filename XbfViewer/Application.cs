namespace XbfViewer
{
	using Cameras;
	using Graphics;
	using Graphics.Mesh;
	using Graphics.Vertices;
	using LibEmperor;
	using OpenTK.Graphics.OpenGL4;
	using OpenTK.Mathematics;
	using OpenTK.Windowing.Common;
	using OpenTK.Windowing.Desktop;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Xbf;

	public class Application : GameWindow
	{
		private XbfShader shader;
		private Mesh mesh;
		private MeshInstance meshInstance;
		private Camera camera;

		public Application()
			: base(GameWindowSettings.Default, NativeWindowSettings.Default)
		{
		}

		protected override void OnLoad()
		{
			GL.ClearColor(0, 0, 0, 1);
			GL.Enable(EnableCap.DepthTest);

			this.shader = new XbfShader();

			// TODO implement filesystem!
			this.mesh = this.LoadXbf("3DDATA0001", "Buildings/OR_Palace_H0");
			this.meshInstance = new MeshInstance(this.mesh);

			this.camera = new PerspectiveCamera
			{
				Size = new Vector2(this.Size.X, this.Size.Y), Direction = new Vector3(-1, -1, 1).Normalized(), Position = new Vector3(1, 1, -1) * 128
			};
		}

		private Mesh LoadXbf(string dataFolder, string file)
		{
			var xbf = new LibEmperor.Xbf(File.ReadAllBytes(Path.Combine(dataFolder, file + ".xbf")));

			return new Mesh(
				null,
				Matrix4.Identity,
				xbf.Objects.Select(
						xbfObject => this.LoadXbfObject(
							xbfObject,
							xbf.Textures.Select(
									name =>
									{
										var texture = GL.GenTexture();
										GL.BindTexture(TextureTarget.Texture2D, texture);
										GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (uint) TextureWrapMode.Repeat);
										GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (uint) TextureWrapMode.Repeat);
										GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (uint) TextureMinFilter.Linear);
										GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (uint) TextureMagFilter.Linear);
										GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

										var tga = new Tga(File.ReadAllBytes(Path.Combine(dataFolder, $"Textures/{name}")));

										GL.TexImage2D(
											TextureTarget.Texture2D,
											0,
											PixelInternalFormat.Rgba8,
											tga.Width,
											tga.Height,
											0,
											PixelFormat.Rgba,
											PixelType.UnsignedByte,
											tga.Pixels
										);

										return texture;
									}
								)
								.ToArray()
						)
					)
					.ToArray(),
				null,
				null
			);
		}

		private Mesh LoadXbfObject(XbfObject xbfObject, IReadOnlyList<int> textures)
		{
			var transform = new Matrix4(
				(float) xbfObject.Transform[0],
				(float) xbfObject.Transform[1],
				(float) xbfObject.Transform[2],
				(float) xbfObject.Transform[3],
				(float) xbfObject.Transform[4],
				(float) xbfObject.Transform[5],
				(float) xbfObject.Transform[6],
				(float) xbfObject.Transform[7],
				(float) xbfObject.Transform[8],
				(float) xbfObject.Transform[9],
				(float) xbfObject.Transform[10],
				(float) xbfObject.Transform[11],
				(float) xbfObject.Transform[12],
				(float) xbfObject.Transform[13],
				(float) xbfObject.Transform[14],
				(float) xbfObject.Transform[15]
			);

			var allVertices = new Dictionary<int, List<VertexPositionNormalUv>>();
			var allIndices = new Dictionary<int, List<ShaderIndex>>();

			foreach (var triangle in xbfObject.Triangles)
			{
				if (triangle.Texture == -1)
					continue;

				if (!allVertices.ContainsKey(triangle.Texture))
				{
					allVertices.Add(triangle.Texture, new List<VertexPositionNormalUv>());
					allIndices.Add(triangle.Texture, new List<ShaderIndex>());
				}

				var vertices = allVertices[triangle.Texture];
				var indices = allIndices[triangle.Texture];

				vertices.Add(
					new VertexPositionNormalUv(
						new Vector3(xbfObject.Vertices[triangle.Vertex1].X, xbfObject.Vertices[triangle.Vertex1].Y, xbfObject.Vertices[triangle.Vertex1].Z),
						new Vector3(
							xbfObject.Vertices[triangle.Vertex1].NormalX,
							xbfObject.Vertices[triangle.Vertex1].NormalY,
							xbfObject.Vertices[triangle.Vertex1].NormalZ
						),
						new Vector2(triangle.U1, 1 - triangle.V1)
					)
				);

				vertices.Add(
					new VertexPositionNormalUv(
						new Vector3(xbfObject.Vertices[triangle.Vertex2].X, xbfObject.Vertices[triangle.Vertex2].Y, xbfObject.Vertices[triangle.Vertex2].Z),
						new Vector3(
							xbfObject.Vertices[triangle.Vertex2].NormalX,
							xbfObject.Vertices[triangle.Vertex2].NormalY,
							xbfObject.Vertices[triangle.Vertex2].NormalZ
						),
						new Vector2(triangle.U2, 1 - triangle.V2)
					)
				);

				vertices.Add(
					new VertexPositionNormalUv(
						new Vector3(xbfObject.Vertices[triangle.Vertex3].X, xbfObject.Vertices[triangle.Vertex3].Y, xbfObject.Vertices[triangle.Vertex3].Z),
						new Vector3(
							xbfObject.Vertices[triangle.Vertex3].NormalX,
							xbfObject.Vertices[triangle.Vertex3].NormalY,
							xbfObject.Vertices[triangle.Vertex3].NormalZ
						),
						new Vector2(triangle.U3, 1 - triangle.V3)
					)
				);

				indices.Add(new ShaderIndex(vertices.Count - 3, vertices.Count - 2, vertices.Count - 1));
			}

			return new Mesh(
				new XbfShaderParameters(this.shader),
				transform,
				allVertices.Keys.Select(
						texture =>
						{
							return new Mesh(
								new XbfShaderParameters(this.shader) {Texture = textures[texture]},
								Matrix4.Identity,
								null,
								allVertices[texture].Select(vertex => vertex.Pack()).SelectMany(v => v).ToArray(),
								allIndices[texture].Select(index => new[] {index.I1, index.I2, index.I3}).SelectMany(i => i).ToArray()
							);
						}
					)
					.Concat(xbfObject.Children.Select(childXbfObject => this.LoadXbfObject(childXbfObject, textures)))
					.ToArray(),
				null,
				null
			);
		}

		protected override void OnResize(ResizeEventArgs args)
		{
			GL.Viewport(0, 0, args.Width, args.Height);
			this.camera.Size = new Vector2(args.Width, args.Height);
		}

		protected override void OnUpdateFrame(FrameEventArgs args)
		{
			this.meshInstance.World *= Matrix4.CreateRotationY((float) args.Time);
		}

		protected override void OnRenderFrame(FrameEventArgs args)
		{
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			this.camera.Update();

			this.meshInstance.Draw(this.camera);

			this.Context.SwapBuffers();
		}
	}
}
