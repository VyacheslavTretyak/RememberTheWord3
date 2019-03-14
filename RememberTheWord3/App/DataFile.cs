using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RememberTheWord3
{
	class DataFile
	{
		private string directoryName = "data";
		private string fileName = "words";
		private string fileExtension = "dat";
		private string formatInFile = "yyyy_MM_dd_HH_mm_ss";

		public List<Word> LoadLastFile()
		{			
			FileInfo fi = new FileInfo(directoryName);
			if (!fi.Exists)
			{
				Directory.CreateDirectory(fi.FullName);
			}
			DirectoryInfo info = new DirectoryInfo(fi.FullName);
			FileInfo[] files = info.GetFiles();
			if (files.Length == 0)
			{
				throw new Exception("File not found!");
			}
			//Looking for newest file
			FileInfo newestFile = files[0];
			int index = newestFile.Name.IndexOf("__");
			string strTime = newestFile.Name.Substring(index + 2, 19);
			DateTime newest = DateTime.ParseExact(strTime, formatInFile, CultureInfo.InvariantCulture);
			foreach (FileInfo file in files)
			{
				index = file.Name.IndexOf("__");
				strTime = file.Name.Substring(index + 2, 19);
				DateTime dateTime = DateTime.ParseExact(strTime, formatInFile,
					CultureInfo.InvariantCulture);
				if (dateTime > newest)
				{
					newestFile = file;
					newest = dateTime;
				}
			}
			//Read file
			string spliter = Word.spliter;
			List<Word> words = new List<Word>();
			using (StreamReader sr = new StreamReader(newestFile.FullName))
			{
				while (!sr.EndOfStream)
				{
					string[] line = sr.ReadLine().Split(spliter.ToCharArray());
					Word word = new Word()
					{
						Origin = line[0],
						Translate = line[1],
						CountShow = int.Parse(line[2]),
						TimeShow = DateTime.ParseExact(line[3], Word.formatInWord, System.Globalization.CultureInfo.InvariantCulture),
						TimeCreate = DateTime.ParseExact(line[4], Word.formatInWord, System.Globalization.CultureInfo.InvariantCulture)
					};
					words.Add(word);
				}
			}
			return words;
		}
	}
}
