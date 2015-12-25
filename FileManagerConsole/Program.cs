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
			var manager = new FileManagerConsole();
			manager.StartIteratingFolders(args);
		}
	}
}