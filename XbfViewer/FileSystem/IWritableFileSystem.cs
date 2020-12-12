namespace XbfViewer.FileSystem
{
	using System.IO;

	public interface IWritableFileSystem : IReadableFileSystem
	{
		public void Delete(string path);
		public Stream? Write(string path);
	}
}
