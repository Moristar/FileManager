using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WindowsFileManager
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			this.DataContext = new FileDirViewModel();
		}

		// 22.01.2016
		// Вот это и есть обработчик события двойного щелчка на элементе
		private void ListViewItem_MouseDoubleClick(object sender, RoutedEventArgs args)
		{
			// 22.01.2016
			// Поскольку мы не можем держать логику в форме (а этот класс и есть форма), то нам придется как-то достучаться до нашей вью модели
			// мы знаем, что вью модель лежит в свойстве DataContext, так как мы сами ее туда положили в конструкторе, а значит можем обратиться к ней
			// тем не менее, мы не будем усугублять и укреплять связи между вью (формой) и вью моделью, как мы уже сделали в конструкторе, а пойдем по пути ООП
			// для этого мы введем интерфейс IFileDirView, который будет декларировать, какой именно должна быть Вью модель для нашего Вью
			// ну и первым реально необхходимым свойством интерфейса-декларации станет команда обработчик двойного щелчка.
 			// Да, для кнопок такой херней страдать не требуется, а вот для двойного щелчка по списку приходится. Есть и другие способы, но этот на сейчас самы простой
			var doubleClickHandler = (this.DataContext as IFileDirView).OnListViewDoubleClick;

			// 22.01.2016
			// В данную команду мы передадим еще и параметр, а именно тот элемент, который был выбран во время щелчка
			// Также, я хочу убедиться, что команда может быть выполнена, прежде чем вызывать ее, а значит мы проверим метод CanExecute
			// Идем во вью модель! Начинай читать сверху
			if (doubleClickHandler != null && doubleClickHandler.CanExecute(listView.SelectedItem))
				doubleClickHandler.Execute(listView.SelectedItem);
		}
	}
}
