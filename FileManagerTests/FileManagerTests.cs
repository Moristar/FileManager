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
			public Func<ConsoleKeyInfo> GetPressedKeyInjection;

			protected override ConsoleKeyInfo GetPressedKey()
			{
				return GetPressedKeyInjection();
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
			FileManagerTestable manager = new FileManagerTestable();
			manager.GetPressedKeyInjection = ReturnNonTabKey;

			manager.StartIteratingFolders(null);

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
