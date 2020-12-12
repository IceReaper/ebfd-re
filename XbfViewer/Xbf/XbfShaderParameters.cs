namespace XbfViewer.Xbf
{
	using Graphics.Shaders;
	using OpenTK.Graphics.OpenGL4;
	using System;

	public class XbfShaderParameters : ShaderParameters<XbfShader>
	{
		public int Texture;

		public XbfShaderParameters(XbfShader shader)
			: base(shader)
		{
		}

		public override void Dispose()
		{
			GL.DeleteTexture(this.Texture);
			GC.SuppressFinalize(this);
		}
	}
}
