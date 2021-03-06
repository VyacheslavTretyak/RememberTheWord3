﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RememberTheWord3
{
	public class Word
	{
		public static string spliter = ";";
		public static string formatInWord = "dd.MM.yyyy HH:mm:ss";
		public int Id { get; set; } = 0;
		public string Origin{ get; set; }
		public string Translate { get; set; }
		public DateTime TimeShow { get; set; }
		public DateTime TimeCreate { get; set; }
		public int CountShow { get; set; } = 0;
		public double WaitSeconds { get; set; } = 0;

		public Word()
		{
			TimeCreate = DateTime.Now;
			TimeShow = DateTime.Now;
		}

		
		public string ToLine()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Id);
			stringBuilder.Append(spliter);
			stringBuilder.Append(Origin);
			stringBuilder.Append(spliter);
			stringBuilder.Append(Translate);
			stringBuilder.Append(spliter);
			stringBuilder.Append(CountShow.ToString());
			stringBuilder.Append(spliter);
			stringBuilder.Append(TimeShow.ToString(formatInWord));
			stringBuilder.Append(spliter);
			stringBuilder.Append(TimeCreate.ToString(formatInWord));
			return stringBuilder.ToString();
		}


	}
}
