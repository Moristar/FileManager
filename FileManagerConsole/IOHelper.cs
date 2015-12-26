using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManagerConsole
{
	public class IOHelper
	{
		public bool DirectoryExists(string path)
		{
			return Directory.Exists(path);
		}

	}
}
