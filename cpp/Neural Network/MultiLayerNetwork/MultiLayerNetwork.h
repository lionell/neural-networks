#pragma once
#ifndef _MULTILAYERNETWORK_H
#define _MULTILAYERNETWORK_H

#define DEBUG

#include "List.h"
#include "ActivationFunctions.h"

class Link;

// Neuron Interface
class INeuron
{
public:
	List<Link> In;
	List<Link> Out;
	INeuron() {};
	virtual ~INeuron();
	virtual void AccumulateImpulse(Link*, double);
	virtual void Wake();
};

class ActivationNeuron : public INeuron
{
private:
	ActivationFunction Core;
	double AccumulatedImpulse;
public:
	ActivationNeuron();
	~ActivationNeuron();
	void SetCore(ActivationFunction);
	void AccumulateImpulse(Link*, double);
	void Wake();
};

class Link
{
protected:
	double Weight;
public:
	INeuron* From;
	INeuron* To;
	Link();
	~Link();
	void Transfer(double);
};

// Neuron Implementaion

ActivationNeuron::ActivationNeuron()
{
	Core = Binary;
}

ActivationNeuron::~ActivationNeuron() {}

void ActivationNeuron::SetCore(ActivationFunction _Core)
{
	Core = _Core;
}

void ActivationNeuron::AccumulateImpulse(Link* _From, double _Impulse = 0.0)
{
	AccumulatedImpulse += _Impulse;
}

void ActivationNeuron::Wake()
{
	std::list<int> a;
	for each(int i in a)
	{

	}
	Core(AccumulatedImpulse);
}

// Link Implementation

Link::Link() {}

Link::~Link() 
{
	From->Out.Remove(this);
	To->In.Remove(this);
}

void Link::Transfer(double _Impulse)
{
	To->AccumulateImpulse(_Impulse);
}

#endif