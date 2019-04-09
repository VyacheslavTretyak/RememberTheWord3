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
		private System.Windows.Forms.NotifyIcon notifyIcon;
		private ContextMenu contextMenu;		
		private Task task;
		private Thread thread;
		public bool IsEdit { get; set; } = false;
		public Word EditingWord { get; set; }
		public SettingsWnd SettingsWnd { get; set; }
		public WordShowingWnd WordShowingWnd { get; set; }
		public WordListWnd WordListWnd { get; set; }

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
			controller.LoadData();			
			
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
					var miliseconds = (double)word.WaitSeconds * 1000;
					if(miliseconds > Int32.MaxValue)
					{
						miliseconds = Int32.MaxValue;
					}					
					Thread.Sleep((int)miliseconds);
				}
				Dispatcher.Invoke(() =>
				{
					WordShow(word);
				});
			}

		}
		public void WordShow(Word word)
		{			
			WordShowingWnd = new WordShowingWnd();
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
				WordShowingWnd.TextBlockWord.Text = word.Origin;
				WordShowingWnd.TextBlockTranslate.Text = word.Translate;
			}
			else
			{
				WordShowingWnd.TextBlockWord.Text = word.Translate;
				WordShowingWnd.TextBlockTranslate.Text = word.Origin;
			}
			WordShowingWnd.Top = System.Windows.SystemParameters.WorkArea.Height - WordShowingWnd.Height;
			WordShowingWnd.Left = System.Windows.SystemParameters.WorkArea.Width - WordShowingWnd.Width;
			WordShowingWnd.ShowDialog();
			WordShowingWnd = null;
			word.TimeShow = DateTime.Now;
			word.CountShow++;
			word.WaitSeconds = 0;
			controller.Repository.Update(word);			
		}

		private void ButtonRollback_Click(object sender, RoutedEventArgs e)
		{
			thread?.Abort();
			controller.RollBack();
			task = Task.Run(() => NextWord());
		}

		private void ButtonList_Click(object sender, RoutedEventArgs e)
		{
			Hide();
			WordListWnd = new WordListWnd();
			WordListWnd.ParentWindow = this;
			WordListWnd.Top = System.Windows.SystemParameters.WorkArea.Height - WordListWnd.Height;
			WordListWnd.Left = System.Windows.SystemParameters.WorkArea.Width - WordListWnd.Width;
			WordListWnd.ShowDialog();
			WordListWnd = null;
			thread?.Abort();
			task = Task.Run(() => NextWord());
		}

		private void ButtonSettings_Click(object sender, RoutedEventArgs e)
		{
			Hide();
			SettingsWnd = new SettingsWnd();
			SettingsWnd.Top = System.Windows.SystemParameters.WorkArea.Height - SettingsWnd.Height;
			SettingsWnd.Left = System.Windows.SystemParameters.WorkArea.Width - SettingsWnd.Width;
			SettingsWnd.ShowDialog();
			SettingsWnd = null;
		}

		public bool HasWndOpen()
		{
			if(SettingsWnd != null)
			{
				return true;
			}
			else if (WordShowingWnd != null)
			{
				return true;
			}
			else if(WordListWnd != null)
			{
				return true;
			}
			return false;
		}

		private void NotifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (HasWndOpen())
			{
				return;
			}
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
					Show();
					WindowState = WindowState.Normal;					
				}
			}
		}
		private void ButtonExit_Click(object sender, RoutedEventArgs e)
		{
			App.Current.Shutdown();
		}

		private void MainWindow_Closed(object sender, EventArgs e)
		{			
			controller.SaveData();
			notifyIcon?.Dispose();
		}
		private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{			
			thread?.Abort();
			e.Cancel = true;
			Hide();
		}

		private void ButtonShow_Click(object sender, RoutedEventArgs e)
		{
			thread?.Abort();
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
			thread?.Abort();
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
