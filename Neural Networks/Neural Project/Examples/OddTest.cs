using System;
using NeuralProject.Networks;
using NeuralProject.LearningAlgorithms.SupervisedLearning;

namespace NeuralProject.Examples
{
	public class OddTest
	{
		public OddTest()
		{
			Console.WriteLine("Welcome to Neural Odd Test Example");
			FeedForwardNetwork network = new FeedForwardNetwork(new SigmoidFunction(), 1, 10, 1);
			BackPropagationLearning teacher = new BackPropagationLearning(network);
			teacher.LearningRate = 0.1;
			Random R = new Random(Environment.TickCount);
			Console.WriteLine("Training...");
			for (int i = 0; i < 1000; i++)
			{
				Console.Write("[T] Input >> ");
				int Input = R.Next(1024);
				Console.WriteLine(Input);
				teacher.Teach(Binary(Input), (Input % 2 == 0) ? (new double[] { 0 }) : (new double[] { 1 }));
				Console.Write("[T] Output >> ");
				Console.WriteLine(network.Launch(Binary(Input))[0].ToString());
			}
			Console.WriteLine("Examing...");
			Console.Write("[E] Input >> ");
			string userString = Console.ReadLine();
			while (userString != "stop")
			{
				int Number = int.Parse(userString.Split(' ')[0]);
				Console.Write("[E] Output >> ");
				Print(network.Launch(Binary(Number)));
				Console.Write("[E] Input >> ");
				userString = Console.ReadLine();
			}
			Console.WriteLine();
		}
		public void Print(double[] Array)
		{
			foreach (double i in Array) Console.Write(i + " ");
			Console.WriteLine();
		}
		public double[] Binary(int a)
		{
			double[] Answer = new double[10];
			int i = 9;
			while (a > 0)
			{
				Answer[i] = a % 2;
				a /= 2;
				i--;
			}
			return Answer;
		}
	}
}