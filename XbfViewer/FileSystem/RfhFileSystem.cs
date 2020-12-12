namespace XbfViewer.FileSystem
{
	using LibEmperor;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	public class RfhFileSystem : IReadableFileSystem
	{
		private readonly Rfh rfh;

		public RfhFileSystem(Rfh rfh)
		{
			this.rfh = rfh;
		}

		public bool Exists(string path)
		{
			var rfhPath = path.Replace('/', '\\');

			return this.rfh.Files.Any(rfhEntry => rfhEntry.Path == rfhPath);
		}

		public Stream? Read(string path)
		{
			var rfhPath = path.Replace('/', '\\');
			var entry = this.rfh.Files.FirstOrDefault(rfhEntry => rfhEntry.Path == rfhPath);

			return entry != null ? new MemoryStream(entry.Read()) : null;
		}

		public IEnumerable<string> GetFiles(string path = "")
		{
			var rfhPath = path.Replace('/', '\\');

			return this.rfh.Files.Where(rfhEntry => rfhEntry.Path.StartsWith(rfhPath)).Select(rfhEntry => rfhEntry.Path.Replace('\\', '/').Trim('/'));
		}

		public void Dispose()
		{
			this.rfh.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
