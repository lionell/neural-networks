#ifndef _ACTIVATIONFUNCTIONS_H
#define _ACTIVATIONFUNCTIONS_H

#include <cmath>

// Activation Neuron's Functions

typedef double(*ActivationFunction)(double Impulse);

//class ActivationFunction
//{
//protected:
//	double* Arguments;
//	AFunction Function;
//public:
//	ActivationFunction(AFunction _Function, double* Arguments)
//	{}
//};

// Require 1 argument
double Binary(double Impulse)
{
	return (Impulse > 0.0) ? (1) : (0);
}

// Require 1 argument
double Sigmoid(double Impulse)
{
	return (1 / (1 + exp(-Impulse)));
}

#endif