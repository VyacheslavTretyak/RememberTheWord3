using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RememberTheWord3
{
	class ConfigSaver
	{
		private string fileName = "config.ini";
		private Dictionary<string, string> config;
		public ConfigSaver()
		{
			config = new Dictionary<string, string>();
		}

		public void LoadConfig()
		{
			config = new Dictionary<string, string>();
			if (!File.Exists(fileName))
			{
				SetDefaultConfig();
				SaveConfig();
			}
			else
			{
				using (StreamReader sr = new StreamReader(fileName))
				{
					while (!sr.EndOfStream)
					{
						string line = sr.ReadLine();
						if (line.TrimStart()[0] == '#')
						{
							continue;
						}
						string[] keyVal = line.Split(":".ToCharArray());
						config[keyVal[0]] = keyVal[1];
					}
				}
			}
		}

		public Dictionary<string, string> GetConfig()
		{
			return config;
		}

		private void SetDefaultConfig()
		{
			config["hours"] = "12";
			config["days"] = "7";
			config["weeks"] = "4";
			config["ask"] = "2";
			config["autorun"] = "1";
		}

		public void SaveConfig()
		{
			using (StreamWriter sw = File.CreateText(fileName))
			{
				foreach (var pair in config)
				{
					if (pair.Key == "ask")
					{
						sw.WriteLine($"# 0 - Word, 1 - Translate, 2 - Both");
					}
					sw.WriteLine($"{pair.Key}:{pair.Value}");
				}
			}
		}
	}
}
