namespace XbfViewer.Xbf
{
	using Graphics.Shaders;

	public class XbfShaderParameters : ShaderParameters<XbfShader>
	{
		public int Texture;

		public XbfShaderParameters(XbfShader shader)
			: base(shader)
		{
		}
	}
}
