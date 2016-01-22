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
	// 22.01.2016
	// Новый интерфейс, который я упоминал раньше. Декларирует вью модель для нашего вью
	public interface IFileDirView : INotifyPropertyChanged
	{
		ICommand OnListViewDoubleClick { get; set; }
	}

	public class FileDirViewModel : IFileDirView
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

		// 22.01.2016
		// Новая команда
		public ICommand OnListViewDoubleClick { get; set; }

		public FileDirViewModel()
		{
			ViewData = new ObservableCollection<FileDirModel>();

			ViewData.Add(new FileDirModel { Name = "FolderTest", Type = "Dir", Size = 0, LastModificationDate = DateTime.Now });

			// Здесь мы инициализируем нашу команду нажатия кнопки, чтобы по нажати. оной что-то происходило. Если забыть это сделать, то будет эксепшен! Почем именно так - смотри ниже.

			// Два варианта использования универсального класса команды - один раз с Action, второй - с делегатом. Смотри ниже сам класс.

			//OnReadDataCommand = new MyBasicCommand(new Action(GenerateModel));
			OnReadDataCommand = new MyBasicCommand(ReadDir);
			OnCleanDataCommand = new MyBasicCommand(() => ViewData.Clear());

			// 22.01.2016
			// Поскольку команда с параметром, мне нужно было написать новый класс-обработчик команд с параметром. В принципе то же самое, однако теперь он принимает указатели на 
			// функции вида void func(object), т.е. с параметром типа обджект.
			OnListViewDoubleClick = new MyBasicParameterCommand(OnDoubleClickItem);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string name)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(name));
		}

		// 22.01.2016
		// Реализация метода чисто тестовая - увидеть, что параметр передается. Твое ЗАДАНИЕ заключается в том, чтобы очищать список папок и файлов и показывать содержимое той папки, на которую
		// щелкнули. Файлы игнорируй пока.
		public void OnDoubleClickItem(object item)
		{
			// 22.01.2016
			// Собственно айтем - это то, что мы передали как параметр в нашу команду. Как ты видишь - это все тот же объект FileDirModel, который ты создаешь в методе ReadDir. 
			// Что мы туда отправляем - то и получаем обратно.
			FileDirModel fdItem = item as FileDirModel;
			System.Windows.MessageBox.Show(fdItem.Name);
		}

		public void ReadDir()
		{
			string path = "..\\..";
			var dirFileInfo = new DirectoryInfo(path);

			DirectoryInfo[] dirInfo = dirFileInfo.GetDirectories();

			FileInfo[] fileInfo = dirFileInfo.GetFiles("*.*");
			
			OnPropertyChanged("ViewData");
			foreach (DirectoryInfo d in dirInfo)
			{
				ViewData.Add(new FileDirModel { Name = d.Name, Type = d.Extension.ToString(), LastModificationDate = d.LastWriteTime });
			}
			foreach (FileInfo f in fileInfo)
			{
				ViewData.Add(new FileDirModel { Name = Path.GetFileNameWithoutExtension(f.Name), Type = f.Extension.ToString(), Size = f.Length, LastModificationDate = f.LastWriteTime });
			}

		}
	}

	// 22.01.2016
	// Новый класс для обработки команд с параметром. Почти такой же как и обычный с парой изменнеий
	public class MyBasicParameterCommand : ICommand
	{
		// 22.01.2016
		// Теперь у нас не просто экшен, а экшен с параметром обджект
		Action<object> _CommandHandlerAction;

		public event EventHandler CanExecuteChanged;

		public MyBasicParameterCommand(Action<object> commandHandlerAction)
		{
			_CommandHandlerAction = commandHandlerAction;
		}

		// 22.01.2016
		// А здесь мы проверяем, что параметр не нулл, т.е. мы кликнули на чем-то существующем. Во вью мы вызывем этот метод, прежде чем вызывать сам Экзекьют
		// Теперь все - возвращайся во вью модель и пиши хождение по папкам. Удачи!
		public bool CanExecute(object parameter)
		{
			return parameter != null;
		}

		public void Execute(object parameter)
		{
			if (_CommandHandlerAction != null)
				_CommandHandlerAction(parameter);
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
