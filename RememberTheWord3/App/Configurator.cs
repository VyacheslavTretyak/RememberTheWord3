using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RememberTheWord3
{
	class Configurator
	{
		public AskWordsType AskWords { get; set; }
		public int Hours { get; set; }
		public int Days { get; set; }
		public int Weeks { get; set; }
		public bool AutoRun { get; set; }
		public string DirectoryName { get; set; }
		public string FileName { get; set; }
		public string FileExtension { get; set; } 
		public string FormatInFile { get; set; } 
		public int MaxCountFiles { get; set; } 
		private string appName = "RememberTheWord3";
		private ConfigSaver saver;
		public Configurator()
		{
			saver = new ConfigSaver();
			saver.LoadConfig();			
		}

		public void SaveConfig()
		{
			saver.SaveConfig();
		}

		public void GetConfig()
		{
			Dictionary<string, string> config = saver.GetConfig();
			Hours = int.Parse(config["hours"]);
			Days= int.Parse(config["days"]);
			Weeks = int.Parse(config["weeks"]);
			AutoRun = config["autorun"] == "1";
			DirectoryName = config["directoryName"];
			FileName = config["fileName"];
			FileExtension = config["fileExtension"];
			FormatInFile = config["formatInFile"];
			MaxCountFiles = int.Parse(config["maxCountFiles"]);
			AskWords = (AskWordsType)int.Parse(config["ask"]);
			if (AutoRun)
			{
				AutoRunSet();
			}
			else
			{
				AutoRunUnset();
			}
		}	

		public void AutoRunSet()
		{
			RegistryKey curUserkey = Registry.CurrentUser;
			RegistryKey autoRunKey = curUserkey.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
			var location = System.Reflection.Assembly.GetEntryAssembly().Location;
			autoRunKey.SetValue(appName, location);
			autoRunKey.Close();
			curUserkey.Close();
		}
		public void AutoRunUnset()
		{
			RegistryKey curUserkey = Registry.CurrentUser;
			RegistryKey autoRunKey = curUserkey.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
			autoRunKey.DeleteValue(appName);
			autoRunKey.Close();
			curUserkey.Close();
		}
	}
}
