#include <cstdio>
#include "Neuron.h"

int main()
{
	printf("Hello world!\n");
	Neuron* A = new Neuron();
	A->SetCore(Sigmoid);

}