using System;
namespace NeuralProject.Networks
{
	public interface IActivationFunction
	{
		double Function(double Argument);
		double Derivative(double Argument);
	}

	[Serializable]
	public class SigmoidFunction : IActivationFunction
	{
		public SigmoidFunction() { }
		public double Function(double Argument)
		{
			return (1.0 / (1.0 + System.Math.Exp(-Argument)));
		}
		public double Derivative(double Argument)
		{
			return (Function(Argument) * (1.0 - Function(Argument)));
		}
	}

	[Serializable]
	public class BipolarSigmoidFunction : IActivationFunction
	{
		public BipolarSigmoidFunction() { }
		public double Function(double Argument)
		{
			return ((2.0 / (1.0 + System.Math.Exp(-Argument))) - 1.0);
		}
		public double Derivative(double Argument)
		{
			return ((1.0 + Function(Argument) * (1.0 - Function(Argument))) / 2.0);
		}
	}

	[Serializable]
	public class HyperbolicTangent : IActivationFunction
	{
		public HyperbolicTangent() { }
		public double Function(double Argument)
		{
			return System.Math.Tanh(Argument);
		}
		public double Derivative(double Argument)
		{
			return (1.0 / (System.Math.Cosh(Argument) * System.Math.Cosh(Argument)));
		}
	}

	[Serializable]
	public class LinearFunction : IActivationFunction
	{
		public LinearFunction() { }
		public double Function(double Argument)
		{
			return Argument;
		}
		public double Derivative(double Argument)
		{
			return (1.0);
		}
	}
}