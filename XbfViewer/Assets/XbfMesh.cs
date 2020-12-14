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
						xbf.Textures.Select(name => this.assetManager.Load<Texture>(this, $"Textures/{name}").Id).ToArray()
					)
				)
				.ToArray();
		}

		private static Mesh LoadXbfObject(XbfObject xbfObject, XbfShader shader, IReadOnlyList<int> textures)
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
				xbfObject.Name,
				new XbfShader.XbfShaderParameters(shader),
				transform,
				allVertices.Keys.Select(
						texture =>
						{
							return new Mesh(
								"",
								new XbfShader.XbfShaderParameters(shader) {Texture = textures[texture]},
								Matrix4.Identity,
								null,
								allVertices[texture].Select(vertex => vertex.Pack()).SelectMany(v => v).ToArray(),
								allIndices[texture].Select(index => new[] {index.I1, index.I2, index.I3}).SelectMany(i => i).ToArray()
							);
						}
					)
					.Concat(xbfObject.Children.Select(childXbfObject => XbfMesh.LoadXbfObject(childXbfObject, shader, textures)))
					.ToArray(),
				null,
				null
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
