using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

			ViewData.Add(new FileDirModel { Name = "FolderTest", Type = "Dir", Size = 0, LastModificationDate = DateTime.Now });

			// Здесь мы инициализируем нашу команду нажатия кнопки, чтобы по нажати. оной что-то происходило. Если забыть это сделать, то будет эксепшен! Почем именно так - смотри ниже.

			// Два варианта использования универсального класса команды - один раз с Action, второй - с делегатом. Смотри ниже сам класс.
			OnReadDataCommand = new MyBasicCommand(new Action(GenerateModel));
			OnCleanDataCommand = new MyBasicCommand(new MyBasicCommand.MyCommandHandlerDelegate(CleanModel));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string name)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(name));
		}

		public void GenerateModel()
		{
			OnPropertyChanged("ViewData");
			ViewData.Add(new FileDirModel { Name = "Folder1", Type = "Dir", Size = 0, LastModificationDate = DateTime.Now });
			ViewData.Add(new FileDirModel { Name = "Folder2", Type = "Dir", Size = 21230, LastModificationDate = DateTime.Now });
			ViewData.Add(new FileDirModel { Name = "File1", Type = "txt", Size = 3000, LastModificationDate = DateTime.Now });
		}

		public void CleanModel()
		{
			ViewData = new ObservableCollection<FileDirModel>();
			OnPropertyChanged("ViewData");
		}
	}


	public class MyBasicCommand : ICommand
	{
		Action _CommandHandlerAction; // Action - это по сути указатель на функцию, которая ничего не принимает и ничего не возвращает. Ее сделали стандартным типом данных.

		public delegate void MyCommandHandlerDelegate(); // Delegate - это ключевое слово, которое позволяет описывать делегаты. Делегаты - это тоже указатели на функции, но их можно описывать как угодно
														 // Разницы между Action и нашим MyCommandHandlerDelegate нет никакой - оба указатели на функции, которые ничего не принимают и не возвращают
														 // Просто раньше были только делегаты, а потом позже добавили Action чисто для удобства. Но оба - указатели на функции
		MyCommandHandlerDelegate _CommandHandlerDelegate;   // Переменная нашего типа MyCommandHandlerDelegate - просто альтернативная имплементация переменной типа Action - чисто для сравнения.

		public event EventHandler CanExecuteChanged;


		// Далее идут два конструктора. По сути своей они одинаковы - получают указатель на метод и сохранют его. Сделано чтобы можно было попробовать оба варианта.
		// На практике сейчас аще всего используют Action, потому что проще и короче писать и не надо каждый раз писать свои делегаты.
		public MyBasicCommand(Action commandHandlerAction) 
		{
			_CommandHandlerAction = commandHandlerAction;
		}

		public MyBasicCommand(MyCommandHandlerDelegate commandHandlerDelegate)
		{
			_CommandHandlerDelegate = commandHandlerDelegate;
		}

		public bool CanExecute(object parameter) { return true; }


		public void Execute(object parameter)
		{
			// Здесь будем вызывать один или другой хэндлер, в зависимости от того какой конструктор мы вызвали. Для команды генерации данных воспользуемся одним, а для очистки - другим
			if (_CommandHandlerAction != null)
				_CommandHandlerAction();
			else if (_CommandHandlerDelegate != null)
				_CommandHandlerDelegate();
		}
	}

	/// <summary>
	/// Вот и наша реализация интерфейса ICommand.
	/// Ничего особого она не делает, просто говорит активна ли команда или нет ну и собственно вызывает метод, который выполняет непосредственно действия
	/// </summary>
	class MyReadCommand : ICommand
	{
		private FileDirViewModel _Model;

		public MyReadCommand(FileDirViewModel model)
		{
			// Конструктор в таком классе необязателен, но мы сейчас пишем очень конкретную реализацию интерфейса ICommand, которая будет очень конкретное действие
			// А именно - заполнять список данными. Для этого мы должны на время передать управление нашим кодом этому классу Команды. Мы делаем это - передав нашу Вью модель как параметр и сохранив ее.
			_Model = model;
		}

		/// <summary>
		/// Это нам не нужно сейчас, посколько мы не меняем возможность нажатия.
		/// </summary>
		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter)
		{
			// Допустим, что нашу команду можно выполнить всегда!
			// Попробуй поменять на false - кнопочка должна стать серой и перестать нажиматься :)
			return true;
		}

		public void Execute(object parameter)
		{
			// А вот и сам метод, который вызывается при нажатии кнопки. Конечно, в серьезной программе этот метод является всего-лишь проводником к реальному методу,
			// но пока что для простоты будем писать логику прямо сюда. Логика очень простая. Тот код который находится в конструкторе FileDirViewModel и создает три строчки с именами папок/файлов
			// должен быть вызван здесь. Для этого создание трех элементов нужно вынести в отдельный метод внутри класса FileDirViewModel, а потом вызвать этот метод посредством сохраненного 
			// экземпляра класса (в переменной _Model). Написать такой метод, вызвать его и понять как все происходит и есть твое ЗАДАНИЕ!

			// Для того, чтобы убедиться, что метод все же работает и вызывается, я вставил вывод простого Мессадж Бокса. Впоследствии его стоит стереть, потому что это плохая идея - смешивать слои
			// Мессадж Бокс относится к слою представления (вью), а у нас тут как бы логика ;)
			//System.Windows.MessageBox.Show("Удали меня!");
			_Model.GenerateModel();

		}
	}

	class MyCleanCommand : ICommand
	{
		private FileDirViewModel _Model;

		public MyCleanCommand(FileDirViewModel model)
		{
			_Model = model;
		}

		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			_Model.CleanModel();
		}
	}
}
