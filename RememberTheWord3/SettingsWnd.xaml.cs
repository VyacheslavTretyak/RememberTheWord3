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

namespace RememberTheWords3
{
	public partial class Settings : Window
	{
		public Settings()
		{
			InitializeComponent();
			ButtonCancel.Click += ButtonCancel_Click;
			ButtonOk.Click += ButtonOk_Click;
			foreach (var AskWords in (AskWords[])Enum.GetValues(typeof(AskWords)))
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
			TextBoxDays.Text = pairs["days"];
			TextBoxHours.Text = pairs["hours"];
			TextBoxWeeks.Text = pairs["weeks"];
			ComboBoxAsk.SelectedValue = pairs["ask"];
			ToggleButtonAutoRun.IsChecked = pairs["autoRun"].ToLower() == "true";
			bool res = ShowDialog() ?? false;
			if (res)
			{
				pairs["days"] = TextBoxDays.Text;
				pairs["hours"] = TextBoxHours.Text;
				pairs["weeks"] = TextBoxWeeks.Text;
				pairs["ask"] = ComboBoxAsk.SelectedValue.ToString();
				pairs["autoRun"] = ToggleButtonAutoRun.IsChecked.ToString();
				RegistryManager registryManager = new RegistryManager();
				registryManager.SaveSettings(pairs);
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
