using System;
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
		public Dictionary<string, string> settings;
		private Task task;
		private Thread thread;
		public bool IsEdit { get; set; } = false;
		private string oldWord;
		private string oldTranslate;		
		private bool isClosed = false;
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
			Configurator configurator = Configurator.GetInstance();
			configurator.GetConfig();
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

		private void ButtonRollback_Click(object sender, RoutedEventArgs e)
		{
			thread.Abort();
			dataManager.RollBack();
			task = Task.Run(() => NextWord());
		}

		private void NextWord()
		{
			thread = Thread.CurrentThread;
			while (true)
			{
				WordSet word = dataManager.NextWord(settings);
				if (word == null)
				{
					MessageBox.Show("No more words");
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
		public void WordShow(WordSet word)
		{
			WordShowing wordShowingWindow = new WordShowing();
			bool flag = false;
			if (settings["ask"] == AskWords.Word.ToString())
			{
				flag = true;
			}
			else if (settings["ask"] == AskWords.Both.ToString())
			{
				Random rnd = new Random();
				flag = rnd.Next(0, 2) == 1;
			}
			if (flag)
			{
				wordShowingWindow.TextBlockWord.Text = word.Word;
				wordShowingWindow.TextBlockTranslate.Text = word.Translate;
			}
			else
			{
				wordShowingWindow.TextBlockWord.Text = word.Translate;
				wordShowingWindow.TextBlockTranslate.Text = word.Word;
			}
			wordShowingWindow.Top = System.Windows.SystemParameters.WorkArea.Height - wordShowingWindow.Height;
			wordShowingWindow.Left = System.Windows.SystemParameters.WorkArea.Width - wordShowingWindow.Width;
			wordShowingWindow.ShowDialog();
			dataManager.UpdateWord(word);
		}

		private void ButtonList_Click(object sender, RoutedEventArgs e)
		{
			Hide();
			Words wordsWindow = new Words();
			wordsWindow.ParentWindow = this;
			wordsWindow.Top = System.Windows.SystemParameters.WorkArea.Height - wordsWindow.Height;
			wordsWindow.Left = System.Windows.SystemParameters.WorkArea.Width - wordsWindow.Width;
			wordsWindow.ShowDialog();
			thread.Abort();
			task = Task.Run(() => NextWord());
			if (!isClosed)
			{
				Show();
			}
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
			dataManager.SaveChanges();
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
			WordSet word = dataManager.NextWord(settings);
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
			WordSet word = new WordSet();
			if (IsEdit)
			{
				word = dataManager.Edit(TextBoxWord.Text, TextBoxTranslate.Text, oldWord, oldTranslate);
				word.WaitSeconds = 0;
			}
			else
			{
				word = dataManager.Add(TextBoxWord.Text, TextBoxTranslate.Text);
				word.WaitSeconds = 0;
			}
			WordShow(word);
			TextBoxWord.Text = "";
			TextBoxTranslate.Text = "";
			task = Task.Run(() => NextWord());
		}
	}
}
