using NeuralProject.Networks;
using System.Collections.Generic;
namespace NeuralProject.LearningAlgorithms.SupervisedLearning
{
	public class BackPropagationLearning
	{
		private double[][] Errors;
		private FeedForwardNetwork Network;
		public double LearningRate { get; set; }
		public BackPropagationLearning(FeedForwardNetwork _Network)
		{
			Network = _Network;
		}
		public void Teach(double[] Inputs, double[] Outputs)
		{
			Errors = new double[Network.Layers.Count][];
			for (int i = 0; i < Network.Layers.Count; i++) Errors[i] = new double[Network.Layers[i].Neurons.Count];
			Network.Launch(Inputs);
			for (int i = 0; i < Network.Layers[Network.Layers.Count - 1].Neurons.Count; i++)
			{
				Errors[Network.Layers.Count - 1][i] = (Outputs[i] - Network.Layers[Network.Layers.Count - 1].Neurons[i].OutputImpulse) * (Network.ActivationFunction.Derivative(Network.Layers[Network.Layers.Count - 1].Neurons[i].InputImpulse));
				foreach (Link Bridge in Network.Layers[Network.Layers.Count - 1].Neurons[i].Inputs)
				{
					Errors[Network.Layers.Count - 2][Bridge.FromIndex] += (Bridge.Weight * Errors[Network.Layers.Count - 1][i]);
					Bridge.Weight += (LearningRate * Errors[Network.Layers.Count - 1][i] * Bridge.From.OutputImpulse);
				}
				Network.Layers[Network.Layers.Count - 1].Neurons[i].Bias += (LearningRate * Errors[Network.Layers.Count - 1][i]);
			}
			for (int i = (Network.Layers.Count - 2); i > 0; i--)
			{
				for (int j = 0; j < Network.Layers[i].Neurons.Count; j++)
				{
					Errors[i][j] *= Network.ActivationFunction.Derivative(Network.Layers[i].Neurons[j].InputImpulse);
					foreach (Link Bridge in Network.Layers[i].Neurons[j].Inputs)
					{
						Errors[i - 1][Bridge.FromIndex] += Errors[i][j];
						Bridge.Weight += (LearningRate * Errors[i][j] * Bridge.From.OutputImpulse);
					}
					Network.Layers[i].Neurons[j].Bias += (LearningRate * Errors[i][j]);
				}
			}
		}
		public double[] Launch(double[] Inputs)
		{
			return Network.Launch(Inputs);
		}
	}
}