using System;
using System.Collections.Generic;
using System.Linq;

namespace FileManagerConsole
{
	public abstract class FileManager
	{
		public IEnumerable<DirFileInfo> listOfDirectories;
		public IEnumerable<DirFileInfo> listOfFiles;
		protected IOHelper IOHelper;

		public FileManager()
		{
			IOHelper = new IOHelper();
		}

		public void StartIteratingFolders(string[] args)
		{
			string path = "";
			string newPath = "";
			int position = 0;

			if (args != null && args.Length > 0)
				path = args[0];

			bool showDir = true;
			string tabCurrentString = "";

			while (true)
			{
				if (showDir)
					path = ListDirectoryContents(path);

				var pressedKey = GetPressedKey();

				if (pressedKey.Key == ConsoleKey.Tab)
				{
					tabCurrentString = TabPressed(position);
					showDir = false;
					if (position < listOfDirectories.Count() + listOfFiles.Count() - 1)
						position++;
					else
						position = 0;
				}
				else
				{
					while (pressedKey.Key != ConsoleKey.Enter)
					{
						if (pressedKey.Key != ConsoleKey.Backspace)
							tabCurrentString += pressedKey.KeyChar;
						else if (tabCurrentString.Length > 0)
						{
							tabCurrentString = tabCurrentString.Remove(tabCurrentString.Length - 1);
							var cursorLeft = Console.CursorLeft;
							Write(" ");
							Console.SetCursorPosition(cursorLeft, Console.CursorTop);
						}

						pressedKey = GetPressedKey();
					}

					newPath = tabCurrentString;
					tabCurrentString = "";

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
							string potentialNewPath = IOHelper.Combine(path, newPath);

							if (IOHelper.DirectoryExists(potentialNewPath))
								path = potentialNewPath;
							else
								path = newPath;
							showDir = true;
						}
					}
				}
			}
		}

		protected abstract string ReadLine();

		protected abstract ConsoleKeyInfo GetPressedKey();

		private string TabPressed(int position)
		{
			string putString = "";

			if (position < listOfDirectories.Count())
				putString = listOfDirectories.ElementAt(position).Name;
			else if (position < listOfDirectories.Count() + listOfFiles.Count())
				putString = listOfFiles.ElementAt(position - listOfDirectories.Count()).Name;
			Console.SetCursorPosition(0, Console.CursorTop);
			Write("                                                              ");
			Console.SetCursorPosition(0, Console.CursorTop);
			Write(putString);

			return putString;
		}

		private void PrintFile(string path, string newPath)
		{
			if (!CheckFilePath(ref path, ref newPath))
				return;

			IEnumerable<string> file = IOHelper.ReadFileLines(path);
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
			string potentialNewPath = IOHelper.Combine(path, newPath);

			if (IOHelper.FileExists(potentialNewPath))
				path = potentialNewPath;
			else if (IOHelper.FileExists(newPath))
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
				if (!IOHelper.DirectoryExists(args))
				{
					WriteLine("Path {0} doesn't exist", args);
					return path;
				}
				else
					path = args;
			}

			var dirMgr = new DirectoryManager(path);
			int dirCount = 0;
			int fileCount = 0;
			long fileSize = 0;

			WriteLine("Folder contents:{0} ", dirMgr.FullName);

			ListFoldersInDirectory(dirMgr, ref dirCount);
			ListFilesInDiretory(dirMgr, ref fileCount, ref fileSize);

			WriteLine("\t\t{0} directories", dirCount);
			WriteLine("\t\t{0} files\t{1:n0} bytes", fileCount, fileSize);

			WriteLine("Please put new path or type 'Exit'  ");
			return path;
		}

		private void ListFilesInDiretory(DirectoryManager dirInfo, ref int fileCount, ref long fileSize)
		{
			listOfFiles = dirInfo.EnumerateFiles();
			foreach (var file in listOfFiles)
			{
				WriteLine("{2}\t{0:n0}\t\t{1}", file.Size, file, file.LastModified.ToString("dd.MM.yyyy HH:mm"));
				fileCount++;
				fileSize += file.Size;
			}
		}

		public void ListFoldersInDirectory(DirectoryManager dirInfo, ref int dirCount)
		{
			listOfDirectories = dirInfo.EnumerateDirectories();
			foreach (var dir in listOfDirectories)
			{
				WriteLine("{0}\t<DIR>\t\t{1}", dir.LastModified.ToString("dd.MM.yyyy HH:mm"), dir);
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
