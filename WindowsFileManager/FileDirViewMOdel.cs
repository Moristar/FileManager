using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFileManager
{
	public class FileDirViewModel
	{
		public List<FileDirModel> ViewData { get; set; }
		
		public FileDirViewModel()
		{
			ViewData = new List<FileDirModel>();

			ViewData.Add(new FileDirModel { Name = "Folder1", Type = "Dir", Size = 0, LastModificationDate = DateTime.Now });
			ViewData.Add(new FileDirModel { Name = "Folder2", Type = "Dir", Size = 21230, LastModificationDate = DateTime.Now });
			ViewData.Add(new FileDirModel { Name = "File1", Type = "txt", Size = 3000, LastModificationDate = DateTime.Now });
		}
	}
}
