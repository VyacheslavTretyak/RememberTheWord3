﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace RememberTheWord3
{
	public partial class MainWindow : Window
	{
		//TODO About
		private System.Windows.Forms.NotifyIcon notifyIcon;
		private ContextMenu contextMenu;		
		private Task task;
		private Thread thread;
		public bool IsEdit { get; set; } = false;
		public Word EditingWord { get; set; }
		private bool isClosed = false;

		private Controller controller;
		public MainWindow()
		{
			InitializeComponent();
			ButtonAddWord.IsEnabled = false;
			ButtonAddWord.Click += BtnAddWord_Click;
			TextBoxTranslate.TextChanged += TbTranslate_TextChanged;
			TextBoxWord.TextChanged += TbTranslate_TextChanged;
			ButtonShow.Click += ButtonShow_Click;
			ButtonList.Click += ButtonList_Click;
			ButtonSettings.Click += ButtonSettings_Click;
			ButtonRollback.Click += ButtonRollback_Click;
			Closing += MainWindow_Closing;
			Closed += MainWindow_Closed;
			ButtonExit.Click += ButtonExit_Click;

			Top = System.Windows.SystemParameters.WorkArea.Height - Height;
			Left = System.Windows.SystemParameters.WorkArea.Width - Width;
			Hide();


			//load settings
			controller = Controller.GetInstance();
			controller.Configurator.GetConfig();
			//notifyIcon
			notifyIcon = new System.Windows.Forms.NotifyIcon();
			notifyIcon.Visible = true;
			var icon = RememberTheWord3.Properties.Resources.icon1.GetHicon();
			notifyIcon.Icon = System.Drawing.Icon.FromHandle(icon);
			notifyIcon.MouseClick += NotifyIcon_MouseClick;
			//contextMenu
			contextMenu = new ContextMenu();
			MenuItem item = new MenuItem();
			item.Click += ButtonShow_Click;
			item.Header = "NEXT WORD";
			contextMenu.Items.Add(item);

			item = new MenuItem();
			item.Click += ButtonList_Click;
			item.Header = "LIST";
			contextMenu.Items.Add(item);

			item = new MenuItem();
			item.Click += ButtonSettings_Click;
			item.Header = "SETTINGS";
			contextMenu.Items.Add(item);

			item = new MenuItem();
			item.Click += ButtonExit_Click;
			item.Header = "EXIT";
			contextMenu.Items.Add(item);
			//Start
			task = Task.Run(() => NextWord());			
		}		

		private void NextWord()
		{
			thread = Thread.CurrentThread;
			while (true)
			{
				Word word = controller.NextWord();
				if (word == null)
				{
					MessageBox.Show("No more words");
					thread = null;
					return;
				}
				if (word.WaitSeconds != 0)
				{
					Thread.Sleep((int)word.WaitSeconds * 1000);
				}
				Dispatcher.Invoke(() =>
				{
					WordShow(word);
				});
			}

		}
		public void WordShow(Word word)
		{
			WordShowingWnd wordShowingWindow = new WordShowingWnd();
			bool showOrigin = true;
			if (controller.Configurator.AskWords == AskWordsType.Translate)
			{
				showOrigin = false;
			}
			else if (controller.Configurator.AskWords == AskWordsType.Both)
			{
				Random rnd = new Random();
				showOrigin = rnd.Next(0, 2) == 1;
			}
			if (showOrigin)
			{
				wordShowingWindow.TextBlockWord.Text = word.Origin;
				wordShowingWindow.TextBlockTranslate.Text = word.Translate;
			}
			else
			{
				wordShowingWindow.TextBlockWord.Text = word.Translate;
				wordShowingWindow.TextBlockTranslate.Text = word.Origin;
			}
			wordShowingWindow.Top = System.Windows.SystemParameters.WorkArea.Height - wordShowingWindow.Height;
			wordShowingWindow.Left = System.Windows.SystemParameters.WorkArea.Width - wordShowingWindow.Width;
			wordShowingWindow.ShowDialog();
			word.TimeShow = DateTime.Now;
			word.CountShow++;
			controller.Repository.Update(word);			
		}

		private void ButtonRollback_Click(object sender, RoutedEventArgs e)
		{
			thread.Abort();
			controller.RollBack();
			task = Task.Run(() => NextWord());
		}

		private void ButtonList_Click(object sender, RoutedEventArgs e)
		{
			Hide();
			WordListWnd wordsWindow = new WordListWnd();
			wordsWindow.ParentWindow = this;
			wordsWindow.Top = System.Windows.SystemParameters.WorkArea.Height - wordsWindow.Height;
			wordsWindow.Left = System.Windows.SystemParameters.WorkArea.Width - wordsWindow.Width;
			wordsWindow.ShowDialog();
			thread.Abort();
			task = Task.Run(() => NextWord());
			//if (!isClosed)
			//{
			//	Show();
			//}
		}

		private void ButtonSettings_Click(object sender, RoutedEventArgs e)
		{
			Hide();
			Settings settingsWindow = new Settings();
			settingsWindow.Top = System.Windows.SystemParameters.WorkArea.Height - settingsWindow.Height;
			settingsWindow.Left = System.Windows.SystemParameters.WorkArea.Width - settingsWindow.Width;
			settingsWindow.ShowDialog(ref settings);
			if (!isClosed)
			{
				Show();
			}
		}

		private void NotifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Right)
			{
				contextMenu.IsOpen = true;
			}
			else if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				if (IsVisible)
				{
					Activate();
				}
				else
				{
					if (!isClosed)
					{
						Show();
						WindowState = WindowState.Normal;
					}
				}
			}
		}
		private void ButtonExit_Click(object sender, RoutedEventArgs e)
		{
			App.Current.Shutdown();
		}

		private void MainWindow_Closed(object sender, EventArgs e)
		{
			isClosed = true;
			controller.SaveData();
			notifyIcon?.Dispose();
		}
		private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel = true;
			Hide();
		}

		private void ButtonShow_Click(object sender, RoutedEventArgs e)
		{
			thread.Abort();
			Word word = controller.NextWord();
			WordShow(word);
			task = Task.Run(() => NextWord());
		}

		private void TbTranslate_TextChanged(object sender, TextChangedEventArgs e)
		{
			ButtonAddWord.IsEnabled = !(TextBoxWord.Text.Length < 1 || TextBoxTranslate.Text.Length < 1);
		}

		private void BtnAddWord_Click(object sender, RoutedEventArgs e)
		{
			thread.Abort();
			Word newWord = new Word();
			newWord.Origin = TextBoxWord.Text;
			newWord.Translate = TextBoxTranslate.Text;
			if (IsEdit)
			{				
				newWord = controller.EditWord(EditingWord, newWord);				
				IsEdit = false;
			}
			else
			{						
				controller.Repository.Add(newWord);

			}
			WordShow(newWord);
			TextBoxWord.Text = "";
			TextBoxTranslate.Text = "";
			task = Task.Run(() => NextWord());
		}
	}
}
