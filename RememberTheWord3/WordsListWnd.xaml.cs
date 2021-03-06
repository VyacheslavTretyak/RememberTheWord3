﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RememberTheWord3
{
	public partial class WordListWnd : Window
	{
		public List<Word> wordsList;
		public MainWindow ParentWindow { get; set; }
		private Controller controller;
		public WordListWnd()
		{
			InitializeComponent();
			controller = Controller.GetInstance();
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
			TBOriginalSearch.TextChanged += Search_TextChanged;
			TBTranslateSearch.TextChanged += Search_TextChanged;

		}

		private void Search_TextChanged(object sender, TextChangedEventArgs e)
		{
			GetList();
		}

		private void ItemShow_Click(object sender, RoutedEventArgs e)
		{
			int id = GetSelectedId();
			Word nextWord = Task<Word>.Run(() => controller.Repository.Get(id)).Result;

			ParentWindow.WordShow(nextWord);
			GetList();
		}
		private int GetSelectedId()
		{
			var item = DataGridWords.SelectedItem;
			return int.Parse(item.GetType().GetProperty("Id").GetValue(item).ToString());
		}

		private void ItemEdit_Click(object sender, RoutedEventArgs e)
		{
			Word selectedWord = controller.Repository.Get(GetSelectedId());
			ParentWindow.TextBoxWord.Text = selectedWord.Origin;
			ParentWindow.TextBoxTranslate.Text = selectedWord.Translate;
			ParentWindow.IsEdit = true;
			ParentWindow.EditingWord = selectedWord;
			Close();
		}

		private void ItemReset_Click(object sender, RoutedEventArgs e)
		{
			controller.ResetCount(GetSelectedId());
			GetList();
		}

		private void ItemDelete_Click(object sender, RoutedEventArgs e)
		{
			Word selectedWord = controller.Repository.Get(GetSelectedId());
			Task.Run(() => controller.DeleteWord(selectedWord));
			GetList();
		}

		private void GetList()
		{
			DataGridWords.ItemsSource = null;
			Task.Run(() =>
			{
				IEnumerable<Word> list = null;
				string origin = "";
				string translate = "";
				Dispatcher.Invoke(() =>
				{
					origin = TBOriginalSearch.Text.ToLower();
					translate = TBTranslateSearch.Text.ToLower();
				});
				
				if (origin == "" && translate == "")
				{
					list = controller.Repository.Words;
				}
				else
				{
					list = controller.Repository.Words.Where(w => w.Origin.ToLower().Contains(origin)).Where(w => w.Translate.ToLower().Contains(translate));
					
				}
				Dispatcher.Invoke(() =>
				{
					DataGridWords.ItemsSource = list.Select(a => new { a.Origin, a.Translate, a.CountShow, a.TimeShow, a.TimeCreate, a.Id });
				});
			});			
		}
	}
}
