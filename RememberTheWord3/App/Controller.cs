using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RememberTheWord3
{
	class Controller
	{
		private static Controller instance = null;
		private DataFile dataFile;
		private List<Word> words;

		public static Controller GetInstance()
		{
			if(instance == null)
			{
				instance = new Controller();
			}
			return instance;
		}
		private Controller()
		{
			dataFile = new DataFile();
			LoadData();
		}

		private void LoadData()
		{
			try
			{
				words = dataFile.LoadLastFile();
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

		}
	}
}
