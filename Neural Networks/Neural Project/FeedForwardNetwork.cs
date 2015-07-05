using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

namespace NeuralProject.Networks
{
	// FeedForward
	[Serializable]
	public class Link
	{
		public ActivationNeuron From;
		public ActivationNeuron To;
		public double Weight { get; set; }
		public int FromIndex { get; set; }
		public int ToIndex { get; set; }
		public Link() { }
		public Link(ActivationNeuron _From, ActivationNeuron _To)
			: this()
		{
			From = _From;
			To = _To;
		}
		public void Transfer(double Impulse)
		{
			To.InputImpulse += (Weight * Impulse);
		}
	}

	[Serializable]
	public class ActivationNeuron
	{
		public List<Link> Inputs = new List<Link>();
		public List<Link> Outputs = new List<Link>();
		public IActivationFunction ActivationFunction;
		public double InputImpulse { get; set; }
		public double OutputImpulse { get; set; }
		public double Bias { get; set; }
		public ActivationNeuron() { }
		public ActivationNeuron(IActivationFunction _ActivationFunction)
			: this()
		{
			ActivationFunction = _ActivationFunction;
		}
		public void Attach(ActivationNeuron Neuron, double Weight)
		{
			Link Bridge = new Link(this, Neuron);
			Bridge.Weight = Weight;
			Outputs.Add(Bridge);
			Neuron.Inputs.Add(Bridge);
		}
		public void Launch()
		{
			if (ActivationFunction == null) throw new System.Exception("Activation function is missed!");
			InputImpulse += Bias;
			OutputImpulse = ActivationFunction.Function(InputImpulse);
			foreach (Link Bridge in Outputs) Bridge.Transfer(OutputImpulse);
		}
	}

	[Serializable]
	public class ActivationLayer
	{
		public List<ActivationNeuron> Neurons = new List<ActivationNeuron>();
		public ActivationLayer() { }
		public ActivationLayer(IActivationFunction _ActivationFunction, int Count)
			: this()
		{
			for (int i = 0; i < Count; i++) Neurons.Add(new ActivationNeuron(_ActivationFunction));
		}
		public void Activate()
		{
			foreach (ActivationNeuron Neuron in Neurons) Neuron.Launch();
		}
	}

	[Serializable]
	public class FeedForwardNetwork
	{
		public IActivationFunction ActivationFunction;
		public List<ActivationLayer> Layers = new List<ActivationLayer>();
		public FeedForwardNetwork() { }
		public FeedForwardNetwork(FeedForwardNetwork _Example)
		{
			Layers = new List<ActivationLayer>(_Example.Layers);
			ActivationFunction = _Example.ActivationFunction;
		}
		public FeedForwardNetwork(IActivationFunction _ActivationFunction, int _RandomSeed, params int[] Counts)
			: this()
		{
			if (Counts.Length == 0) throw new System.Exception("Minimum number of layers is 1");
			ActivationFunction = _ActivationFunction;
			Random R = new Random(_RandomSeed);
			Layers.Add(new ActivationLayer(new LinearFunction(), Counts[0])); // InputLayer
			for (int i = 1; i < Counts.Length; i++)
			{
				Layers.Add(new ActivationLayer(_ActivationFunction, Counts[i]));
				for (int j = 0; j < Layers[i - 1].Neurons.Count; j++)
				{
					for (int k = 0; k < Layers[i].Neurons.Count; k++)
					{
						Layers[i - 1].Neurons[j].Attach(Layers[i].Neurons[k], (2 * R.NextDouble() - 1));
						Layers[i - 1].Neurons[j].Outputs[Layers[i - 1].Neurons[j].Outputs.Count - 1].FromIndex = j;
						Layers[i - 1].Neurons[j].Outputs[Layers[i - 1].Neurons[j].Outputs.Count - 1].ToIndex = i;
					}
				}
				//foreach (ActivationNeuron From in Layers[i - 1].Neurons)
				//{
				//	foreach (ActivationNeuron To in Layers[i].Neurons)
				//	{
				//		From.Attach(To, (2 * R.NextDouble()) - 1);
				//	}
				//}
				for (int j = 0; j < Layers[i].Neurons.Count; j++) Layers[i].Neurons[j].Bias = 2 * R.NextDouble() - 1;
				//foreach (ActivationNeuron Neuron in Layers[i].Neurons) Neuron.Bias = (2 * R.NextDouble()) - 1;
			}
		}
		public double[] Launch(params double[] _Inputs)
		{
			foreach (ActivationLayer Layer in Layers)
			{
				foreach (ActivationNeuron Neuron in Layer.Neurons) Neuron.InputImpulse = 0.0; // Cleaning rubbish
			}
			if (Layers[0].Neurons.Count != _Inputs.Length) throw new System.Exception("Invalid number of inputs");
			for (int i = 0; i < Layers[0].Neurons.Count; i++) Layers[0].Neurons[i].InputImpulse = _Inputs[i];
			for (int i = 0; i < Layers.Count; i++) Layers[i].Activate();
			double[] Answer = new double[Layers[Layers.Count - 1].Neurons.Count];
			for (int i = 0; i < Layers[Layers.Count - 1].Neurons.Count; i++) Answer[i] = Layers[Layers.Count - 1].Neurons[i].OutputImpulse;
			return Answer;
		}
		public void Save(string _FileName)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream fileWriter = new FileStream(_FileName, FileMode.Create);
			binaryFormatter.Serialize(fileWriter, this);
			fileWriter.Close();
		}
		static public FeedForwardNetwork Load(string _FileName)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream fileReader = new FileStream(_FileName, FileMode.Open);
			FeedForwardNetwork Result = (FeedForwardNetwork)binaryFormatter.Deserialize(fileReader);
			fileReader.Close();
			return Result;
		}
	}
}