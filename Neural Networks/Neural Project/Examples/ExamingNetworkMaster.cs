using System;
using NeuralProject.Networks;
using NeuralProject.LearningAlgorithms.SupervisedLearning;

namespace NeuralProject.Examples
{
	public class ExamingNetworkMaster
	{
		public Random R = new Random(Environment.TickCount);
		public ExamingNetworkMaster()
		{
			Console.WriteLine("Welcome to Examing Network Master");
			Console.WriteLine("Now you have to load existing network or create new");
			FeedForwardNetwork network = new FeedForwardNetwork();
			Console.WriteLine("Do you want to load existing network?");
			if (Console.ReadLine() == "Yes")
			{
				network = FeedForwardNetwork.Load(Console.ReadLine());
			}
			else
			{
				network = new FeedForwardNetwork(new BipolarSigmoidFunction(), Environment.TickCount, 1, 7, 1);
				Console.WriteLine("New neural network created successfully");
			}
			BackPropagationLearning teacher = new BackPropagationLearning(network);
			teacher.LearningRate = 0.03;
			Console.WriteLine("Let's teach it");
			Teaching(teacher);
			Console.WriteLine("Now it's time to exam this network(:");
			Examing(network);
			Console.WriteLine("Do you want to save this network?");
			if (Console.ReadLine() == "Yes")
			{
				Console.WriteLine("Enter filename: ");
				network.Save(Console.ReadLine());
			}
			Console.WriteLine("Press enter to exit...");
			Console.ReadKey();
		}
		public double[] ParseDoubles(string userString)
		{
			string[] Temp = userString.Split(' ');
			double[] Result = new double[Temp.Length];
			for (int i = 0; i < Temp.Length; i++) Result[i] = double.Parse(Temp[i]);
			return Result;
		}
		public void ManualTeaching(BackPropagationLearning _teacher)
		{
			Console.Write("[T] Inputs >> ");
			string userString = Console.ReadLine();
			while (userString != "stop")
			{
				double[] Inputs = ParseDoubles(userString);
				Console.Write("[T] Outputs >> ");
				userString = Console.ReadLine();
				double[] Outputs = ParseDoubles(userString);
				for (int i = 0; i < 5; i++) _teacher.Teach(Inputs, Outputs);
				Console.Write("[T] Inputs >> ");
				userString = Console.ReadLine();
			}
		}
		public void AutoTeaching(BackPropagationLearning _teacher)
		{
			for (int i = 0; i < 100000; i++)
			{
				double pacmanLength = R.Next(800);
				double AlphaSine = R.NextDouble();
				double ghostLength = R.Next(600);
				double BetaSine = R.NextDouble();
				double[] Inputs = { pacmanLength, AlphaSine, ghostLength, BetaSine };
				Console.Write("[T] Inputs >> ");
				for (int j = 0; j < Inputs.Length; j++) Console.Write(Inputs[j] + " ");
				Console.WriteLine();
				double[] Outputs = { BetaSine };
				Console.Write("[T] Outputs >> ");
				for (int j = 0; j < Outputs.Length; j++) Console.Write(Outputs[j] + " ");
				Console.WriteLine();
				for (int j = 0; j < 2; j++)
				{
					_teacher.Teach(Inputs, Outputs);
					double[] Test = _teacher.Launch(Inputs);
					Console.Write("[T] Out >> ");
					for (int k = 0; k < Test.Length; k++) Console.Write(Test[k] + " ");
					Console.WriteLine();
				}
			}
		}
		public void Teaching(BackPropagationLearning _teacher)
		{
			ManualTeaching(_teacher);
		}
		public void ManualExaming(FeedForwardNetwork _network)
		{
			Console.Write("[E] Inputs >> ");
			string userString = Console.ReadLine();
			while (userString != "stop")
			{
				double[] Inputs = ParseDoubles(userString);
				double[] Outputs = _network.Launch(Inputs);
				Console.Write("[E] Outputs >> ");
				for (int i = 0; i < Outputs.Length; i++) Console.Write(Outputs[i] + " ");
				Console.WriteLine();
				Console.Write("[E] Inputs >> ");
				userString = Console.ReadLine();
			}
		}
		public void AutoExaming(FeedForwardNetwork _network)
		{

		}
		public void Examing(FeedForwardNetwork _network)
		{
			ManualExaming(_network);
		}
	}
}
