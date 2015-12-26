using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManagerConsole
{
	public class DirFileInfo : IDirFileInfo
	{
		protected DirectoryInfo _DirectoryInfo;

		public string FullName { get { return _DirectoryInfo.FullName; } }
		public string Name { get { return _DirectoryInfo.Name; } }

		public DateTime LastModified { get { return _DirectoryInfo.LastWriteTime; } }
		public long Size { get; set; }


		public DirFileInfo(string path)
		{
			_DirectoryInfo = new DirectoryInfo(path);
		}

		public IEnumerable<IDirFileInfo> EnumerateFiles()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<IDirFileInfo> EnumerateDirectories()
		{
			throw new NotImplementedException();
			//return _DirectoryInfo.EnumerateDirectories();
		}
	}

	public interface IDirFileInfo
	{
		string Name { get; }
		string FullName { get; }
		DateTime LastModified { get; }
		long Size { get; }

		IEnumerable<IDirFileInfo> EnumerateFiles();
		IEnumerable<IDirFileInfo> EnumerateDirectories();
	}
}
