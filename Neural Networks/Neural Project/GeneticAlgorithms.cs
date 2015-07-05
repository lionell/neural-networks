using System;
using NeuralProject.Networks;

namespace NeuralProject.LearningAlgorithms.UnsupervisedLearning.GeneticAlgorithms
{
	public static class Nature
	{
		public static int Mutate(int _Seed, FeedForwardNetwork _Example)
		{
			Random R = new Random(_Seed);
			int ChromosomesMutated = 0;
			for (int i = 1; i < _Example.Layers.Count; i++)
			{
				for (int j = 0; j < _Example.Layers[i].Neurons.Count; j++)
				{
					for (int k = 0; k < _Example.Layers[i].Neurons[j].Inputs.Count; k++)
					{
						if (R.Next(100) == 23)
						{
							_Example.Layers[i].Neurons[j].Inputs[k].Weight += (2 * R.NextDouble() - 1);
							ChromosomesMutated++;
						}
					}
				}
			}
			return ChromosomesMutated;
		}
		public static FeedForwardNetwork Cross(int _Seed, FeedForwardNetwork _A, FeedForwardNetwork _B)
		{
			Random R = new Random(_Seed);
			FeedForwardNetwork Answer = new FeedForwardNetwork(_A);
			int ChromosomesCount = 0;
			for (int i = 1; i < _A.Layers.Count; i++)
			{
				for (int j = 0; j < _A.Layers[i].Neurons.Count; j++)
				{
					ChromosomesCount += _A.Layers[i].Neurons[j].Inputs.Count;
				}
			}
			int Range = R.Next(ChromosomesCount);
			for (int i = 1; i < _A.Layers.Count; i++)
			{
				for (int j = 0; j < _A.Layers[i].Neurons.Count; j++)
				{
					for (int k = 0; k < _A.Layers[i].Neurons[j].Inputs.Count; k++)
					{

						if (i + j + k >= Range) Answer.Layers[i].Neurons[j].Inputs[k].Weight = _B.Layers[i].Neurons[j].Inputs[k].Weight;
						else Answer.Layers[i].Neurons[j].Inputs[k].Weight = _A.Layers[i].Neurons[j].Inputs[k].Weight;
					}
				}
			}
			return Answer;
		}
	}
}