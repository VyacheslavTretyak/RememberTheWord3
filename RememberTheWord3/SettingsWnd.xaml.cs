using System;
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
	public partial class SettingsWnd : Window
	{
		public SettingsWnd()
		{
			InitializeComponent();
			ButtonCancel.Click += ButtonCancel_Click;
			ButtonOk.Click += ButtonOk_Click;
			foreach (var AskWords in (AskWordsType[])Enum.GetValues(typeof(AskWordsType)))
			{
				ComboBoxAsk.Items.Add(AskWords.ToString());
			}
		}

		private void ButtonOk_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		public bool ShowDialog(ref Dictionary<string, string> pairs)
		{
			Controller controller = Controller.GetInstance();
			var config = controller.Configurator;
			TextBoxDays.Text = config.Days.ToString();
			TextBoxHours.Text = config.Hours.ToString();
			TextBoxWeeks.Text = config.Weeks.ToString();
			ComboBoxAsk.SelectedValue = config.AskWords.ToString();
			ToggleButtonAutoRun.IsChecked = config.AutoRun;
			bool res = ShowDialog() ?? false;
			if (res)
			{
				config.Days = int.Parse(TextBoxDays.Text);
				config.Hours = int.Parse(TextBoxHours.Text);
				config.Weeks = int.Parse(TextBoxWeeks.Text);
				config.AskWords = (AskWordsType)ComboBoxAsk.SelectedValue;
				config.AutoRun = ToggleButtonAutoRun.IsChecked??false;
				config.SaveConfig();
			}
			return res;

		}

		private void ButtonCancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}

		private void textbox_OnlyNumeric(object sender, TextCompositionEventArgs e)
		{
			var textBox = sender as TextBox;
			e.Handled = Regex.IsMatch(e.Text, "[^0-9.]");
		}
	}
}
