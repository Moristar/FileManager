using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManagerConsole
{
	public abstract class FileManager
	{
		public IEnumerable<DirectoryInfo> listOfDirectories;
		public IEnumerable<FileInfo> listOfFiles;


		public void StartIteratingFolders(string[] args)
		{
			string path = "";
			string newPath = "";
			int position = 0;

			if (args != null && args.Length > 0)
				path = args[0];

			bool showDir = true;


			while (true)
			{
				if (showDir)
					path = ListDirectoryContents(path);

				var pressedKey = GetPressedKey();
				if (pressedKey.Key == ConsoleKey.Tab)
				{
					TabPressed(path, position);
					showDir = false;
					if (position < listOfDirectories.Count() + listOfFiles.Count() - 1)
						position++;
					else
						position = 0;
				}
				else
				{
					WriteLine("Please put new path or type 'Exit'  ");
					newPath = Console.ReadLine().ToLower();

					if (newPath == "exit")
						return;
					else
					{
						if (newPath.StartsWith("read "))
						{
							PrintFile(path, newPath);
							showDir = false;
						}
						else
						{
							string potentialNewPath = Path.Combine(path, newPath);

							if (Directory.Exists(potentialNewPath))
								path = potentialNewPath;
							else
								path = newPath;
							showDir = true;
						}
					}
				}
			}
		}

		protected abstract ConsoleKeyInfo GetPressedKey();

		private void TabPressed(string path, int position)
		{
			var dirInfo = new DirectoryInfo(path);
			string putString = "";

			if (position < listOfDirectories.Count())
				putString = listOfDirectories.ElementAt(position).Name;
			else if (position < listOfDirectories.Count() + listOfFiles.Count())
				putString = listOfFiles.ElementAt(position - listOfDirectories.Count()).Name;
			Console.SetCursorPosition(0, Console.CursorTop);
			Write("                                                              ");
			Console.SetCursorPosition(0, Console.CursorTop);
			Write(putString);
		}

		private void PrintFile(string path, string newPath)
		{
			if (!CheckFilePath(ref path, ref newPath))
				return;

			IEnumerable<string> file = File.ReadLines(path);
			int countOfReadedLines = 0;

			while (file.Count() > countOfReadedLines)
			{
				string printString = String.Join("\n", file.Skip(countOfReadedLines).Take(10));

				WriteLine(printString);
				Write("Would you like to read next 10 lines? (Y=yes)  ");

				string readNext = Console.ReadLine().ToLower();

				if (readNext != "y")
					break;
				else countOfReadedLines += 10;
			}
		}

		private bool CheckFilePath(ref string path, ref string newPath)
		{
			//  "read read me.txt"
			newPath = newPath.Substring(5);
			string potentialNewPath = Path.Combine(path, newPath);

			if (File.Exists(potentialNewPath))
				path = potentialNewPath;
			else if (File.Exists(newPath))
				path = newPath;
			else
			{
				WriteLine("File {0} doesn't exist", newPath);
				return false;
			}
			return true;
		}

		private string ListDirectoryContents(string args)
		{
			string path = "..\\..";

			if (!String.IsNullOrEmpty(args))
			{
				if (!Directory.Exists(args))
				{
					WriteLine("Path {0} doesn't exist", args);
					return path;
				}
				else
					path = args;
			}

			var dirInfo = new DirectoryInfo(path);
			int dirCount = 0;
			int fileCount = 0;
			long fileSize = 0;

			WriteLine("Folder contents:{0} ", dirInfo.FullName);

			ListFoldersInDirectory(dirInfo, ref dirCount);
			ListFilesInDiretory(dirInfo, ref fileCount, ref fileSize);

			WriteLine("\t\t{0} directories", dirCount);
			WriteLine("\t\t{0} files\t{1:n0} bytes", fileCount, fileSize);

			WriteLine("Please put new path or type 'Exit'  ");
			return path;
		}

		private void ListFilesInDiretory(DirectoryInfo dirInfo, ref int fileCount, ref long fileSize)
		{
			listOfFiles = dirInfo.EnumerateFiles();
			foreach (var file in listOfFiles)
			{
				WriteLine("{2}\t{0:n0}\t\t{1}", file.Length, file, file.LastWriteTime.ToString("dd.MM.yyyy HH:mm"));
				fileCount++;
				fileSize += file.Length;
			}
		}

		public void ListFoldersInDirectory(DirectoryInfo dirInfo, ref int dirCount)
		{
			listOfDirectories = dirInfo.EnumerateDirectories();
			foreach (var dir in listOfDirectories)
			{
				WriteLine("{0}\t<DIR>\t\t{1}", dir.LastWriteTime.ToString("dd.MM.yyyy HH:mm"), dir);
				dirCount++;
			}
		}

		protected virtual string WriteLine(string text, params object[] args)
		{
			return string.Format(text, args);
		}

		protected virtual string Write(string text, params object[] args)
		{
			return string.Format(text, args);
		}
	}
}
