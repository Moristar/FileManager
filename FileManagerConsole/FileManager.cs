using System;
using System.Collections.Generic;
using System.Linq;

namespace FileManagerConsole
{
	public abstract class FileManager
	{
		public IEnumerable<DirFileInfo> listOfDirectories;
		public IEnumerable<DirFileInfo> listOfFiles;
		public List<String> ListOfNamesInDirectory = new List<string>();

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
			string inputBuffer = "";
			bool isTabLast = false;
			int lastSubStringPosition = 0;

			while (true)
			{
				if (showDir)
					path = ListDirectoryContents(path);

				var pressedKey = GetPressedKey();

				while (pressedKey.Key != ConsoleKey.Enter)
				{
					if (pressedKey.Key == ConsoleKey.Tab)
					{
						position = AutocompletePath(position, out tabCurrentString, inputBuffer, ref lastSubStringPosition);
						isTabLast = true;
					}
					else if (pressedKey.Key == ConsoleKey.Backspace)
					{
						if (isTabLast)
							inputBuffer = tabCurrentString;
						inputBuffer = EraseLastInputSymbol(inputBuffer);
						isTabLast = false;
						lastSubStringPosition = -1;
					}
					else
					{
						if (isTabLast)
							inputBuffer = tabCurrentString;
						inputBuffer += pressedKey.KeyChar;
						tabCurrentString = inputBuffer;
						isTabLast = false;
						lastSubStringPosition = -1;
					}

					pressedKey = GetPressedKey();
				}

				position = 0;

				inputBuffer = "";
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
					else if (newPath.StartsWith("delete "))
					{
						DeleteFile(path, newPath);
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

		private void DeleteFile(string path, string newPath)
		{
			if (!CheckFilePath(ref path, ref newPath, "delete"))
				return;

			IOHelper.DeleteFile(path);

		}

		private string EraseLastInputSymbol(string tabCurrentString)
		{
			if (tabCurrentString.Length > 0)
			{
				tabCurrentString = tabCurrentString.Remove(tabCurrentString.Length - 1);
				var cursorLeft = Console.CursorLeft;
				Write(" ");
				Console.SetCursorPosition(cursorLeft, Console.CursorTop);
			}

			return tabCurrentString;
		}

		private int AutocompletePath(int position, out string tabCurrentString, string inputBuffer, ref int lastSubStringPosition)
		{
			tabCurrentString = TabPressed(ref position, inputBuffer, ref lastSubStringPosition);
			if (position < ListOfNamesInDirectory.Count() - 1)
				position++;
			else
				position = 0;
			return position;
		}

		protected abstract string ReadLine();

		protected abstract ConsoleKeyInfo GetPressedKey();

		private string TabPressed(ref int position, string inputBuffer, ref int lastSubStringPosition)
		{
			string putString = inputBuffer.ToLowerInvariant();
			string printSpaces = new String(' ', MaxLengthListOfNamesInDirectory());

			if (String.IsNullOrEmpty(putString))
			{
				putString = ListOfNamesInDirectory[position];
			}
			else
			{
				bool stringFound = false;

				for (int i = lastSubStringPosition + 1; i < ListOfNamesInDirectory.Count(); i++)
				{
					if (ListOfNamesInDirectory[i].StartsWith(putString))
					{
						putString = ListOfNamesInDirectory[i];
						lastSubStringPosition = i;
						stringFound = true;
						break;
					}
				}
				if (!stringFound)
					for (int i = 0; i <= lastSubStringPosition; i++)
					{
						if (ListOfNamesInDirectory[i].StartsWith(putString))
						{
							putString = ListOfNamesInDirectory[i];
							lastSubStringPosition = i;
							break;
						}
					}
			}
			Console.SetCursorPosition(0, Console.CursorTop);
			Write(printSpaces);
			Console.SetCursorPosition(0, Console.CursorTop);
			Write(putString);

			return putString;
		}

		private int MaxLengthListOfNamesInDirectory()
		{
			int length = 0;
			for (int i = 0; i < ListOfNamesInDirectory.Count(); i++)
			{
				if (ListOfNamesInDirectory[i].Length > length)
					length = ListOfNamesInDirectory[i].Length;
			}

			return length;
		}

		private void PrintFile(string path, string newPath)
		{
			if (!CheckFilePath(ref path, ref newPath, "read"))
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

		private bool CheckFilePath(ref string path, ref string newPath, string commandName)
		{
			//  "read read me.txt"
			newPath = newPath.Substring(commandName.Length+1);
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

			ListOfNamesInDirectory.Clear();

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
				ListOfNamesInDirectory.Add(file.Name.ToLowerInvariant());
			}
		}

		public void ListFoldersInDirectory(DirectoryManager dirInfo, ref int dirCount)
		{
			listOfDirectories = dirInfo.EnumerateDirectories();
			foreach (var dir in listOfDirectories)
			{
				WriteLine("{0}\t<DIR>\t\t{1}", dir.LastModified.ToString("dd.MM.yyyy HH:mm"), dir);
				dirCount++;
				ListOfNamesInDirectory.Add(dir.Name.ToLowerInvariant());
			}
		}

		protected virtual string WriteLine(string text, params object[] args)
		{
			if (args == null || args.Length == 0)
				return text;
			else
				return string.Format(text, args);
		}

		protected virtual string Write(string text, params object[] args)
		{
			return string.Format(text, args);
		}


	}
}
