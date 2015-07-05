// NOT A STANDARD NEURAL NETWORK
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace NeuralProject.Networks
{
	[Serializable]
	class MemorableNeuron
	{
		private List<double> Memory;
		public double TrainingSpeed = 0.3; // Training multiplier(0..1)
		public MemorableNeuron() { }
		public MemorableNeuron(List<double> _Memory)
		{
			Memory = _Memory;
		}
		public MemorableNeuron(string _FileName)
		{
			LoadMemoryFromFile(_FileName);
		}
		public void LoadMemoryFromFile(string _FileName)
		{
			FileInfo InputFile = new FileInfo(_FileName);
			if (InputFile.Exists == true)
			{
				StreamReader InputReader = new StreamReader(_FileName);
				Memory = new List<string>(InputReader.ReadToEnd().Split(' ')).ConvertAll(x => double.Parse(x));
				InputReader.Close();
			}
			else throw new System.Exception("Input file not exists!");
		}
		public void SaveMemoryToFile(string _FileName)
		{
			StreamWriter OutputWriter = new StreamWriter(_FileName);
			for (int i = 0; i < Memory.Count; i++) OutputWriter.Write(Memory[i] + " ");
			OutputWriter.Close();
		}
		public double Exam(List<double> _Example)
		{
			if (_Example.Count != Memory.Count) throw new System.Exception("Size mismatch");
			double Answer = 0.0;
			for (int i = 0; i < Memory.Count; i++) Answer += System.Math.Abs(Memory[i] - _Example[i]);
			return Answer;
		}
		public void Study(List<double> _Example)
		{
			if (_Example.Count != Memory.Count) throw new System.Exception("Train size is not equal memory size");
			for (int i = 0; i < Memory.Count; i++) Memory[i] = (Memory[i] + TrainingSpeed * (_Example[i] - Memory[i]));
		}
	}
	[Serializable]
	class MemorableNetwork
	{
		public Dictionary<string, MemorableNeuron> NeuralDictionary = new Dictionary<string, MemorableNeuron>();
		public MemorableNetwork() { }
		public void Add(string _Name, MemorableNeuron _Neuron)
		{
			if (NeuralDictionary.ContainsKey(_Name)) throw new System.Exception("Name duplication!");
			NeuralDictionary.Add(_Name, _Neuron);
		}
		public bool Remove(string _Name)
		{
			if (!NeuralDictionary.ContainsKey(_Name)) throw new System.Exception("Network don't contains this neuron!");
			return NeuralDictionary.Remove(_Name);
		}
		public string Exam(List<double> _Example)
		{
			double MinResult = double.PositiveInfinity; // Infinity
			double Result = 0.0;
			string Answer = "ERROR";
			foreach (string Name in NeuralDictionary.Keys)
			{
				Result = NeuralDictionary[Name].Exam(_Example);
				if (Result < MinResult)
				{
					Answer = Name;
					MinResult = Result;
				}
			}
			return Answer;
		}
		public void Train(List<double> _Example, string _Answer)
		{
			if (!NeuralDictionary.ContainsKey(_Answer)) throw new System.Exception("Error! Unknown neuron " + _Answer);
			while (Exam(_Example) != _Answer) NeuralDictionary[_Answer].Study(_Example);
		}
		//public void SaveNeuronMemory(string _Name, string _FileName)
		//{
		//	if (!NeuralDictionary.ContainsKey(_Name)) throw new System.Exception("Unknown neuron " + _Name);
		//	NeuralDictionary[_Name].SaveMemoryToFile(_FileName);
		//}
		//public void SaveNetwork(string _FolderName)
		//{
		//	if (new DirectoryInfo(_FolderName).Exists) throw new System.Exception("Folder already exists");
		//	Directory.CreateDirectory(_FolderName);
		//	StreamWriter NetworkWriter = new StreamWriter(_FolderName + "\\network.set");
		//	foreach (string Name in NeuralDictionary.Keys) NetworkWriter.WriteLine(Name);
		//	NetworkWriter.Close();
		//	foreach (string Name in NeuralDictionary.Keys) NeuralDictionary[Name].SaveMemoryToFile(_FolderName + "\\" + Name + ".neu");
		//}
		public void Save(string _FileName)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream fileWriter = new FileStream(_FileName, FileMode.Create);
			binaryFormatter.Serialize(fileWriter, this);
			fileWriter.Close();
		}
		static public MemorableNetwork Load(string _FileName)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream fileReader = new FileStream(_FileName, FileMode.Open);
			MemorableNetwork Result = (MemorableNetwork)binaryFormatter.Deserialize(fileReader);
			fileReader.Close();
			return Result;
		}
	}
}