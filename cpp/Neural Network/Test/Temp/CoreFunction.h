#ifndef _CORE_
#define _CORE_
#include <cmath> // for exp()
// Standard Core Functions for Neuron's
typedef double(*CoreFunction)(double S);

double Binary(double S)
{
	return (S > 0) ? 1.0 : 0.0;
}

double Sigmoid(double S)
{
	return (1.0 / (1.0 + exp(-S)));
}
#endif