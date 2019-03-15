using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RememberTheWord3
{	
	class Repository
	{

		private List<Word> words;
		public List<Word> Words
		{
			get { return words; }
			set { words = value; }
		}

		public void Add(Word word)
		{
			words.Add(word);
		}

		

	}
}
