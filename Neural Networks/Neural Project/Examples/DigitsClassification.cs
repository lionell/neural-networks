using System;
using System.Collections.Generic;
using NeuralProject.Networks;
using System.Drawing;

namespace NeuralProject.Examples
{
	public class DigitsClassification
	{
		public string Folder = "D:\\Projects\\C#\\Neural Networks\\Neural Project\\Examples\\Digits\\";
		public DigitsClassification()
		{
			Console.WriteLine("Welcome to Memorable Network Digits Classification Example");
			Console.WriteLine("Do you want to load network?");
			MemorableNetwork network;
			if (Console.ReadLine() == "Yes")
			{
				Console.Write("Enter filename: ");
				network = MemorableNetwork.Load(Folder + Console.ReadLine());
			}
			else
			{
				network = new MemorableNetwork();
				Bitmap Symbol;
				for (int k = 0; k < 10; k++)
				{
					Symbol = new Bitmap(Folder + Convert.ToString(k) + ".bmp");
					List<double> SymbolList = new List<double>();
					for (int i = 0; i < Symbol.Height; i++)
					{
						for (int j = 0; j < Symbol.Width; j++) SymbolList.Add(Symbol.GetPixel(j, i).R);
					}
					network.Add(Convert.ToString(k), new MemorableNeuron(SymbolList));
				}
			}
			Console.Write("[E] Input >> ");
			string UserString = Console.ReadLine();
			while (UserString != "stop")
			{
				Bitmap ExampleBMP = new Bitmap(Folder + UserString);
				List<double> ExampleList = new List<double>();
				for (int i = 0; i < ExampleBMP.Height; i++)
				{
					for (int j = 0; j < ExampleBMP.Width; j++) ExampleList.Add(ExampleBMP.GetPixel(j, i).R);
				}
				string Answer = network.Exam(ExampleList);
				Console.Write("[E] Output >> ");
				Console.WriteLine(Answer);
				Console.Write("[E] Right output >> ");
				UserString = Console.ReadLine();
				if (UserString != Answer) network.Train(ExampleList, UserString);
				Console.Write("[E] Input >> ");
				UserString = Console.ReadLine();
			}
			Console.WriteLine("Do you want to save this network?");
			if (Console.ReadLine() == "Yes")
			{
				Console.Write("Enter filename: ");
				network.Save(Folder + Console.ReadLine());
			}
		}
	}
}
