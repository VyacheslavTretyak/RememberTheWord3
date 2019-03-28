using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RememberTheWord3
{
	internal class Controller
	{
		private static Controller instance = null;
		private DataFile dataFile;		
		public Repository Repository { get; set; }
		public Configurator Configurator { get; set; }
		

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
			Repository = new Repository();
			Configurator = new Configurator();
			LoadData();
		}

		private void LoadData()
		{
			try
			{
				Repository.Words = dataFile.LoadLastFile();					
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

		}

		public void ResetCount(int id)
		{
			Word word = Repository.Get(id);
			word.CountShow = 0;
			Repository.Update(word);
		}

		public Word EditWord(Word editingWord, Word newWord)
		{
			editingWord.Origin = newWord.Origin;
			editingWord.Translate = newWord.Translate;
			Repository.Update(editingWord);
			return editingWord;
		}

		public Word NextWord()
		{			
			DateTime now = DateTime.Now;
			int to = Configurator.Hours;			
			int from = 0;
			var words = Repository.Words.Where(a => a.CountShow <= to);
			Word found = null;
			foreach (var word in words)
			{
				if ((now - word.TimeShow).TotalMinutes > 60)
				{
					found = word;
					break;
				}
			}	
			if (found != null)
			{
				found.WaitSeconds = 0;				
				return found;
			}
			from = Configurator.Hours;
			to = Configurator.Days + from;
			words = words.Where(w => w.CountShow > from && w.CountShow <= to);
			found = null;
			foreach (var word in words)
			{
				if ((now - word.TimeShow).TotalHours > 24)
				{
					found = word;
					break;
				}
			}			
			if (found != null)
			{				
				found.WaitSeconds = 0;
				return found;
			}
			from = to;
			to = Configurator.Weeks + from;
			words = words.Where(w => w.CountShow > from && w.CountShow <= to);
			found = null;
			foreach (var word in words)
			{
				if ((now - word.TimeShow).TotalDays > 7)
				{
					found = word;
					break;
				}
			}			
			if (found != null)
			{			
				found.WaitSeconds = 0;
				return found;
			}
			from = to;
			words = words.Where(a => a.CountShow > from);
			found = null;
			foreach (var word in words)
			{
				if ((now - word.TimeShow).TotalDays > 30)
				{
					found = word;
					break;
				}
			}			
			if (found != null)
			{				
				found.WaitSeconds = 0;
				return found;
			}

			//Якщо не має що показати зараз, 
			//шукаємо, що першим показати через певний час
			to = Configurator.Hours;
			found = words.Where(w => w.CountShow <= to).OrderBy(w => w.TimeShow).FirstOrDefault();
			if (found != null)
			{				
				found.WaitSeconds = 60 * 60 - (now - found.TimeShow).TotalSeconds;
				return found;
			}
			from = to;
			to = Configurator.Days + from;
			found = words.Where(w => w.CountShow > from && w.CountShow <= to).OrderBy(w => w.TimeShow).FirstOrDefault();
			if (found != null)
			{				
				found.WaitSeconds = 60 * 60 * 24 - (now - found.TimeShow).TotalSeconds;
				return found;
			}
			from = to;
			to = Configurator.Weeks + from;
			found = words.Where(w => w.CountShow > from && w.CountShow <= to).OrderBy(w => w.TimeShow).FirstOrDefault();
			if (found != null)
			{				
				found.WaitSeconds = 60 * 60 * 24 * 7 - (now - found.TimeShow).TotalSeconds;
				return found;
			}
			from = to;
			found = words.Where(a => a.CountShow > from).OrderBy(a => a.TimeShow).FirstOrDefault();
			if (found != null)
			{				
				found.WaitSeconds = 60 * 60 * 24 * 30 - (now - found.TimeShow).TotalSeconds;
				return found;
			}
			return null;
		}

		public void RollBack()
		{
			Repository.Words = dataFile.RollBack();
		}

		public void SaveData()
		{
			dataFile.Save(Repository.Words);
		}
	}
}
