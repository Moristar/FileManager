using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManagerConsole
{
	public interface IDirectoryManager
	{
		string FullName { get; }

		IEnumerable<DirFileInfo> EnumerateFiles();
		IEnumerable<DirFileInfo> EnumerateDirectories();
	}

	public class DirectoryManager : IDirectoryManager
	{
		protected DirectoryInfo _DirectoryInfo;

		public DirectoryManager(string path)
		{
			_DirectoryInfo = new DirectoryInfo(path);
		}

		public string FullName { get { return _DirectoryInfo.FullName; } }

		public IEnumerable<DirFileInfo> EnumerateDirectories()
		{
			return _DirectoryInfo.EnumerateDirectories().Select(dir => new DirFileInfo { Name = dir.Name, LastModified = dir.LastWriteTime });
		}

		public IEnumerable<DirFileInfo> EnumerateFiles()
		{
			return _DirectoryInfo.EnumerateFiles().Select(file => new DirFileInfo { Name = file.Name, LastModified = file.LastWriteTime, Size = file.Length });
		}
	}

	public class IOHelper
	{
		public bool DirectoryExists(string path)
		{
			return Directory.Exists(path);
		}

		public string Combine(string path, string newPath)
		{
			return Path.Combine(path, newPath);
		}

		public IEnumerable<string> ReadFileLines(string path)
		{
			return File.ReadLines(path);
		}

		public bool FileExists(string filePath)
		{
			return File.Exists(filePath);
		}

		public void DeleteFile(string filePath)
		{
			File.Delete(filePath);
		}
	}
}
