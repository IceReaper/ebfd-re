namespace XbfViewer.Assets
{
	using FileSystem;
	using Graphics;
	using Graphics.Mesh;
	using Graphics.Vertices;
	using LibEmperor;
	using OpenTK.Mathematics;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	public class XbfMesh : Mesh
	{
		private readonly AssetManager assetManager;

		public XbfMesh(AssetManager assetManager, IReadableFileSystem fileSystem, string path)
		{
			this.assetManager = assetManager;

			var xbf = new Xbf(fileSystem.Read(path)!);

			this.Name = Path.GetFileNameWithoutExtension(path);

			// TODO: Some textures are using additional flags by prefixing their filenames:
			// TODO: = => apply player color to 0000?? pixels
			// TODO: ! => render additive

			// TODO: % might be ""
			// TODO: @ might be ""

			this.Children = xbf.Objects.Select(
					xbfObject => XbfMesh.LoadXbfObject(
						xbfObject,
						this.assetManager.Load<XbfShader>(this),
						xbf.Textures.Select(
								name =>
								{
									try
									{
										return this.assetManager.Load<Texture>(this, $"Textures/{name}").Id;
									}
									catch (Exception)
									{
										return this.assetManager.Load<Texture>(this, "Textures/X_32.tga").Id;
									}
								}
							)
							.ToArray()
					)
				)
				.ToArray();
		}

		private static Mesh LoadXbfObject(XbfObject xbfObject, XbfShader shader, IReadOnlyList<int> textures)
		{
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

			return new Mesh(null, null, null)
			{
				Name = xbfObject.Name,
				Transform = XbfMesh.BuildMatrix(xbfObject.Transform),
				TransformAnimation =
					xbfObject.ObjectAnimation != null && xbfObject.ObjectAnimation.Frames.Length > 0
						? xbfObject.ObjectAnimation?.Frames.Select(XbfMesh.BuildMatrix).ToArray()
						: null,
				Children = allVertices.Keys
					.Select(
						texture => new Mesh(
							new XbfShader.XbfShaderParameters(shader) {Texture = textures[texture]},
							allVertices[texture].Select(vertex => vertex.Pack()).SelectMany(v => v).ToArray(),
							allIndices[texture].Select(index => new[] {index.I1, index.I2, index.I3}).SelectMany(i => i).ToArray()
						)
					)
					.Concat(xbfObject.Children.Select(childXbfObject => XbfMesh.LoadXbfObject(childXbfObject, shader, textures)))
					.ToArray(),
			};
		}

		private static Matrix4 BuildMatrix(double[] values)
		{
			return new(
				(float) values[0],
				(float) values[1],
				(float) values[2],
				(float) values[3],
				(float) values[4],
				(float) values[5],
				(float) values[6],
				(float) values[7],
				(float) values[8],
				(float) values[9],
				(float) values[10],
				(float) values[11],
				(float) values[12],
				(float) values[13],
				(float) values[14],
				(float) values[15]
			);
		}

		private static Matrix4 BuildMatrix(float[] values)
		{
			return new(
				values[0],
				values[1],
				values[2],
				values[3],
				values[4],
				values[5],
				values[6],
				values[7],
				values[8],
				values[9],
				values[10],
				values[11],
				values[12],
				values[13],
				values[14],
				values[15]
			);
		}

		public override void Dispose()
		{
			base.Dispose();
			this.assetManager.Unload(this);
			GC.SuppressFinalize(this);
		}
	}
}
