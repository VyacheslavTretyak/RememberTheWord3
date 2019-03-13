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
using System.Windows.Shapes;

namespace RememberTheWords3
{
	public partial class Words : Window
	{
		public List<WordSet> wordsList;
		public MainWindow ParentWindow { get; set; }
		private DataManager dataManager;
		public Words()
		{
			InitializeComponent();
			dataManager = DataManager.GetInstance();
			GetList();
			ContextMenu contextMenu = new ContextMenu();
			MenuItem item = new MenuItem();
			item.Click += ItemShow_Click; ;
			item.Header = "SHOW";
			contextMenu.Items.Add(item);

			item = new MenuItem();
			item.Click += ItemReset_Click;
			item.Header = "COUNT RESET";
			contextMenu.Items.Add(item);

			item = new MenuItem();
			item.Click += ItemEdit_Click; ;
			item.Header = "EDIT";
			contextMenu.Items.Add(item);

			item = new MenuItem();
			item.Click += ItemDelete_Click;
			item.Header = "DELETE";
			contextMenu.Items.Add(item);

			DataGridWords.ContextMenu = contextMenu;
		}

		private void ItemShow_Click(object sender, RoutedEventArgs e)
		{
			var item = DataGridWords.SelectedItem;
			string word = item.GetType().GetProperty("Word").GetValue(item).ToString();
			string translate = item.GetType().GetProperty("Translate").GetValue(item).ToString();

			WordSet nextWord = Task<WordSet>.Run(() => dataManager.GetWord(word, translate)).Result;
			ParentWindow.WordShow(nextWord);
			GetList();
		}

		private void ItemEdit_Click(object sender, RoutedEventArgs e)
		{
			var item = DataGridWords.SelectedItem;
			string word = item.GetType().GetProperty("Word").GetValue(item).ToString();
			string translate = item.GetType().GetProperty("Translate").GetValue(item).ToString();
			ParentWindow.TextBoxWord.Text = word;
			ParentWindow.TextBoxTranslate.Text = translate;
			ParentWindow.IsEdit = true;
			ParentWindow.OldWord = word;
			ParentWindow.OldTranslate = translate;
			Close();
		}

		private void ItemReset_Click(object sender, RoutedEventArgs e)
		{
			var item = DataGridWords.SelectedItem;
			string word = item.GetType().GetProperty("Word").GetValue(item).ToString();
			string translate = item.GetType().GetProperty("Translate").GetValue(item).ToString();
			Task.Run(() => dataManager.CountReset(word, translate));
			GetList();
		}

		private void ItemDelete_Click(object sender, RoutedEventArgs e)
		{
			var item = DataGridWords.SelectedItem;
			string word = item.GetType().GetProperty("Word").GetValue(item).ToString();
			string translate = item.GetType().GetProperty("Translate").GetValue(item).ToString();
			Task.Run(() => dataManager.DeleteRow(word, translate));
			GetList();
		}

		private void GetList()
		{
			DataGridWords.ItemsSource = null;
			Task.Run(() =>
			{
				var list = dataManager.GetWordsList();
				Dispatcher.Invoke(() =>
				{
					DataGridWords.ItemsSource = list.Select(a => new { a.Word, a.Translate, a.CountShow, a.TimeShow, a.TimeCreate });
				});
			});
		}
	}
}
