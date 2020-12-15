namespace XbfViewer.Assets
{
	using FileSystem;
	using Graphics.Shaders;
	using OpenTK.Graphics.OpenGL4;
	using OpenTK.Mathematics;

	public class XbfShader : Shader<XbfShader.XbfShaderParameters>
	{
		public class XbfShaderParameters : ShaderParameters<XbfShader>
		{
			public int Texture;

			public XbfShaderParameters(XbfShader shader)
				: base(shader)
			{
			}
		}

		// language=GLSL
		private const string VertexShader = @"
			#version 300 es
			precision highp float;

			uniform mat4 uModel;
			uniform mat4 uView;
			uniform mat4 uProjection;

			in vec3 aPosition;
			in vec3 aNormal;
			in vec2 aUv;

			out vec3 vNormal;
			out vec2 vUv;

			void main()
			{
				vNormal = aNormal;
				vUv = aUv;
				gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
			}
		";

		// language=GLSL
		private const string FragmentShader = @"
			#version 300 es
			precision highp float;

			uniform sampler2D uTexture;

			in vec3 vNormal;
			in vec2 vUv;

			out vec4 fColor;

			void main()
			{
				fColor = texture(uTexture, vUv);
				
				if (fColor.w == 0.0)
					discard;
			}
		";

		private readonly int vertexPosition;
		private readonly int vertexNormal;
		private readonly int vertexUv;

		public XbfShader(AssetManager assetManager, IReadableFileSystem fileSystem, string path)
			: base(XbfShader.VertexShader, XbfShader.FragmentShader)
		{
			this.vertexPosition = GL.GetAttribLocation(this.Program, "aPosition");
			this.vertexNormal = GL.GetAttribLocation(this.Program, "aNormal");
			this.vertexUv = GL.GetAttribLocation(this.Program, "aUv");
		}

		protected override void Bind(Matrix4 model, Matrix4 view, Matrix4 projection, XbfShaderParameters parameters)
		{
			base.Bind(model, view, projection, parameters);

			GL.BindTexture(TextureTarget.Texture2D, parameters.Texture);
		}

		public override int CreateVertexArrayObject()
		{
			var vertexArrayObject = base.CreateVertexArrayObject();

			GL.VertexAttribPointer(this.vertexPosition, 3, VertexAttribPointerType.Float, false, 8 * 4, 0 * 4);
			GL.EnableVertexAttribArray(this.vertexPosition);

			GL.VertexAttribPointer(this.vertexNormal, 3, VertexAttribPointerType.Float, false, 8 * 4, 3 * 4);
			GL.EnableVertexAttribArray(this.vertexNormal);

			GL.VertexAttribPointer(this.vertexUv, 2, VertexAttribPointerType.Float, false, 8 * 4, 6 * 4);
			GL.EnableVertexAttribArray(this.vertexUv);

			return vertexArrayObject;
		}
	}
}
