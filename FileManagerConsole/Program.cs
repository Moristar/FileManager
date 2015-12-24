using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace FileManagerConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			DirectoryInfo dirInfo = new DirectoryInfo("..\\..");
			int dirCount = 0;
			int fileCount = 0;
			long fileSize = 0;
			Console.WriteLine("Folder contents:{0} ", dirInfo.FullName);
			foreach (var dir in dirInfo.EnumerateDirectories())
			{
				Console.WriteLine("{0}\t<DIR>\t\t{1}", dir.LastWriteTime.ToString("dd.MM.yyyy HH:mm"), dir);
				dirCount++;
			}
			foreach (var file in dirInfo.EnumerateFiles())
			{
				Console.WriteLine("{2}\t{0:n0}\t\t{1}", file.Length, file, file.LastWriteTime.ToString("dd.MM.yyyy HH:mm"));
				fileCount++;
				fileSize += file.Length;
            }
			Console.WriteLine("\t\t{0} directories", dirCount);
			Console.WriteLine("\t\t{0} files\t{1} bytes", fileCount, fileSize);
		}
	}
}
