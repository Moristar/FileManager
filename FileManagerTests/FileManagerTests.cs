using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileManagerConsole;

namespace FileManagerTests
{
	[TestClass]
	public class FileManagerTests
	{
		public class FileManagerTestable : FileManager
		{
			public string VirtualConsole { get; set; }

			public Func<ConsoleKeyInfo> GetPressedKeyInjection;
			public Func<string> GetReadLineInjection;

			public FileManagerTestable()
			{
				VirtualConsole = "";
			}

			protected override ConsoleKeyInfo GetPressedKey()
			{
				return GetPressedKeyInjection();
			}

			protected override string ReadLine()
			{
				return GetReadLineInjection();
			}

			protected override string WriteLine(string text, params object[] args)
			{
				var formattedText = base.WriteLine(text, args);
				VirtualConsole += formattedText + Environment.NewLine;
				return formattedText;
			}
		}

		[TestMethod]
		public void CanCreate()
		{
			FileManagerTestable manager = new FileManagerTestable();
		}

		[TestMethod]
		public void StartIteratingFolders_InputArrayIsNull_OutputsDefaultFolder()
		{
			// 1. Assign
			FileManagerTestable manager = new FileManagerTestable();
			manager.GetPressedKeyInjection = ReturnNonTabKey;

			int inputCount = 0;
			manager.GetReadLineInjection = () => 
			{
				if (inputCount++ == 0)
					return "";
				else
					return "exit";
			};
			manager.VirtualConsole = "";
			
			// 2. Act
			manager.StartIteratingFolders(null);

			// 3. Assert
			Assert.AreNotEqual(manager.VirtualConsole, "");
		}

		private ConsoleKeyInfo ReturnTabKey()
		{
			return new ConsoleKeyInfo('\t', ConsoleKey.Tab, false, false, false);
		}

		private ConsoleKeyInfo ReturnNonTabKey()
		{
			return new ConsoleKeyInfo(' ', ConsoleKey.Spacebar, false, false, false);
		}
	}
}
