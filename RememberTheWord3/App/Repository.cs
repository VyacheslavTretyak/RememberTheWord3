using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RememberTheWord3
{	
	class Repository
	{				
		public List<Word> Words { get; set; }
		public int LastId()
		{			
			return Words.Max(w => w.Id);
		}

		public Word Get(int id)
		{
			return Words.FirstOrDefault(w => w.Id == id);
		}

		public void Add(Word word)
		{
			Words.Add(word);
		}

		internal void Update(Word word)
		{
			var found = Words.FirstOrDefault(w => w.Id == word.Id);
			if(found == null)
			{
				MessageBox.Show($"Word ID {word.Id} not found!");
				return;
			}
			//TODO перевірити оновлення даних
			found = word;
		}
	}
}
