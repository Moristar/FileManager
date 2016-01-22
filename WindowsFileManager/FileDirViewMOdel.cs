using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WindowsFileManager
{
	public class FileDirViewModel : INotifyPropertyChanged

	{
		public ObservableCollection<FileDirModel> ViewData { get; set; }

		/// <summary>
		/// Команда с таким же именем как и то, что мы забиндили во вью (см. MainWindow.xaml).
		/// Эта команда должна соответствовать стандартному интерфейсу ICommand (нажми F12 на названии интерфейса в коде, чтобы посмотреть какие методы и свойства определены им).
		/// К сожалению, во фреймворке нет стандартной реализации этого интерфейса, так что будем писать сами (это несложно), смотри класс MyCommand чуть ниже.
		/// Важно заметить, что все что ты биндишь должно быть public, иначе вьюха не сможет достучаться до команды и будет молчать как партизан при нажатии кнопки (или выводе данных)
		/// </summary>
		public ICommand OnReadDataCommand { get; set; }
		public ICommand OnCleanDataCommand { get; set; }

		public FileDirViewModel()
		{
			ViewData = new ObservableCollection<FileDirModel>();

			ReadDir();

			// Здесь мы инициализируем нашу команду нажатия кнопки, чтобы по нажати. оной что-то происходило. Если забыть это сделать, то будет эксепшен! Почем именно так - смотри ниже.

			// Два варианта использования универсального класса команды - один раз с Action, второй - с делегатом. Смотри ниже сам класс.

			//OnReadDataCommand = new MyBasicCommand(new Action(GenerateModel));
			OnReadDataCommand = new MyBasicCommand(ReadDir);
			OnCleanDataCommand = new MyBasicCommand(() => ViewData.Clear());
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string name)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(name));
		}


		public void ReadDir()
		{
			string path = "..\\..";
			var dirFileInfo = new DirectoryInfo(path);

			DirectoryInfo[] dirInfo = dirFileInfo.GetDirectories();

			FileInfo[] fileInfo = dirFileInfo.GetFiles("*.*");

			ViewData.Add(new FileDirModel { Name = "...", Type = null, LastModificationDate = null, Size = null });

			OnPropertyChanged("ViewData");
			foreach (DirectoryInfo d in dirInfo)
			{
				ViewData.Add(new FileDirModel { Name = d.Name, Type = "<DIR>", Size = null, LastModificationDate = d.LastWriteTime });
			}
			foreach (FileInfo f in fileInfo)
			{
				ViewData.Add(new FileDirModel { Name = Path.GetFileNameWithoutExtension(f.Name), Type = f.Extension.ToString(), Size = f.Length, LastModificationDate = f.LastWriteTime });
			}

		}
	}


	public class MyBasicCommand : ICommand
	{
		Action _CommandHandlerAction; // Action - это по сути указатель на функцию, которая ничего не принимает и ничего не возвращает. Ее сделали стандартным типом данных.

		public event EventHandler CanExecuteChanged;


		// Далее идут два конструктора. По сути своей они одинаковы - получают указатель на метод и сохранют его. Сделано чтобы можно было попробовать оба варианта.
		// На практике сейчас аще всего используют Action, потому что проще и короче писать и не надо каждый раз писать свои делегаты.
		public MyBasicCommand(Action commandHandlerAction) 
		{
			_CommandHandlerAction = commandHandlerAction;
		}

		public bool CanExecute(object parameter) { return true; }


		public void Execute(object parameter)
		{
			// Здесь будем вызывать один или другой хэндлер, в зависимости от того какой конструктор мы вызвали. Для команды генерации данных воспользуемся одним, а для очистки - другим
			if (_CommandHandlerAction != null)
				_CommandHandlerAction();
		}
	}

}
