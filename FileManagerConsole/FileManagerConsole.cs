using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManagerConsole
{
	public class FileManagerConsole : FileManager
	{
		protected override string WriteLine(string text, params object[] args)
		{
			string msg = base.WriteLine(text, args);
			Console.WriteLine(msg);
			return msg;
		}

		protected override string Write(string text, params object[] args)
		{
			string msg = base.WriteLine(text, args);
			Console.Write(msg);
			return msg;
		}

		protected override ConsoleKeyInfo GetPressedKey()
		{
			return Console.ReadKey();
		}
	}
}
