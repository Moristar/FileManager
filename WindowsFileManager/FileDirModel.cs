using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WindowsFileManager
{
	public class FileDirModel
	{
		public string Name { get; set; }
		public string Type { get; set; }
		public int Size { get; set; }
		public DateTime LastModificationDate { get; set; }
		
	}
}
