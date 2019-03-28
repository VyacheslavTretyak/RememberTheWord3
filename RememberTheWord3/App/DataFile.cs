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
		//TODO винести властивості в config
		private string directoryName = "data";
		private string fileName = "words";
		private string fileExtension = "dat";
		private string formatInFile = "yyyy_MM_dd_HH_mm_ss";
		private int maxCountFiles = 20;

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
			return ReadData(newestFile.FullName);			
		}

		private List<Word> ReadData(string fileName)
		{
			//Read file			
			List<Word> words = new List<Word>();
			using (StreamReader sr = new StreamReader(fileName))
			{
				while (!sr.EndOfStream)
				{
					string[] line = sr.ReadLine().Split(Word.spliter.ToCharArray());
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

		public List<Word> RollBack()
		{
			System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
			DirectoryInfo info = new DirectoryInfo(directoryName);
			fileDialog.InitialDirectory = info.FullName;
			if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				return ReadData(fileDialog.FileName);				
			}
			return new List<Word>();
		}		

		public void Save(List<Word> words)
		{
			string time = DateTime.Now.ToString(formatInFile);
			string fullpath = $"{directoryName}\\{fileName}__{time}.wrd";
			using (StreamWriter streamWriter = new StreamWriter(fullpath))
			{
				foreach (var row in words)
				{
					streamWriter.WriteLine(row.ToLine());
				}
			}
			FileInfo fi = new FileInfo(directoryName);
			DirectoryInfo info = new DirectoryInfo(fi.FullName);
			FileInfo[] files = info.GetFiles();
			while (files.Length > maxCountFiles)
			{
				FileInfo latestFile = files[0];
				int index = latestFile.Name.IndexOf("__");
				string strTime = latestFile.Name.Substring(index + 2, 19);
				DateTime latest = DateTime.ParseExact(strTime, formatInFile, CultureInfo.InvariantCulture);

				foreach (FileInfo file in files)
				{
					index = file.Name.IndexOf("__");
					strTime = file.Name.Substring(index + 2, 19);
					DateTime dateTime = DateTime.ParseExact(strTime, formatInFile, CultureInfo.InvariantCulture);
					if (dateTime < latest)
					{
						latestFile = file;
						latest = dateTime;
					}
				}
				if (File.Exists(latestFile.FullName))
				{
					File.Delete(latestFile.FullName);
				}
				files = info.GetFiles();
			}
		}
	}
}
