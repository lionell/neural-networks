#ifndef _NEURON_
#define _NEURON_

#include <cstdio>
#include "CoreFunction.h"

#define NULL 0

void Error(const char* s)
{
	printf(s);
}

#pragma region Classes Predefinition

class InputLink;
class OutputLink;
class Neuron;

#pragma endregion

// Synapse Interface
class Synapse
{
private:
	double Weight;
public:
	OutputLink* From;
	InputLink* To;
	Synapse();
	~Synapse();
	void Destroy();
	void SetWeight(double _Weight);
	void Transfer(double _Impulse);
};

// InputLink Interface
class InputLink
{
private:
	double Impulse;
	bool Status; // 0 - free, 1 - busy
public:
	Neuron* Owner;
	Synapse* From;
	InputLink* Next;
	InputLink(Synapse* _From, Neuron* _Owner);
	~InputLink();
	void SetImpulse(double _Impulse);
	double GetImpulse();
	void Clean();
	bool IsBusy();
	bool IsFree();
};

// InputLinkList Interface
class InputLinkList
{
private:
	InputLink* Head;
	InputLink* Tail;
public:
	Neuron* Owner;
	InputLinkList(Neuron* _Owner);
	~InputLinkList();
	InputLink* Add(Synapse* _From);
	void Remove(Synapse* _From);
	bool IsAllBusy();
	double Summarize();
	void CleanAll();
};

// OutputLink Interface
class OutputLink
{
public:
	Neuron* Owner;
	Synapse* Target;
	OutputLink* Next;
	OutputLink(Synapse* _Target, Neuron* _Owner);
	~OutputLink();
	void SendImpulse(double _Impulse);
};

// OutputLinkList Interface
class OutputLinkList
{
private:
	OutputLink* Head;
	OutputLink* Tail;
public:
	Neuron* Owner;
	OutputLinkList(Neuron* _Owner);
	~OutputLinkList();
	OutputLink* Search(Neuron* _Example);
	OutputLink* Add(Synapse* _To);
	void Remove(Synapse* _To);
	void SendAll(double _Impulse);
};

// Neuron Interface
class Neuron
{
private:
	CoreFunction Core;
	InputLinkList* InLinkList;
	OutputLinkList* OutLinkList;
	double Impulse;
public:
	Neuron();
	~Neuron();
	void SetCore(CoreFunction _Core);
	InputLink* AddInLink(Synapse* _From);
	void RemoveInLink(Synapse* _From);
	OutputLink* AddOutLink(Synapse* _To);
	void RemoveOutLink(Synapse* _To);
	Synapse* Attach(Neuron* _What, double _Weight = 0);
	void Detach(Neuron* _What);
	Synapse* GetSynapse(Neuron* _What);
	void Activate();
	void GenerateImpulse(double _Impulse);
	double GetImpulse();
};

#pragma region Synapse Implementation

// Synapse Implementation
Synapse::Synapse()
{
	From = NULL; 
	To = NULL;
	Weight = 0.0;
}

Synapse::~Synapse() 
{
	To->Owner->RemoveInLink(this);
	From->Owner->RemoveOutLink(this);
}

void Synapse::Destroy()
{
	delete this;
}

void Synapse::SetWeight(double _Weight = 0.0)
{
	Weight = _Weight;
}

void Synapse::Transfer(double _Impulse)
{
	To->SetImpulse(_Impulse * Weight);
}

#pragma endregion

#pragma region InputLink Implementation

// InputLink Implementation
InputLink::InputLink(Synapse* _From, Neuron* _Owner)
{
	Impulse = 0.0;
	Status = 0;
	Owner = _Owner;
	From = _From;
	Next = NULL;
}

InputLink::~InputLink() {}

void InputLink::SetImpulse(double _Impulse)
{
	Impulse = _Impulse;
	Status = 1;
}

double InputLink::GetImpulse()
{
	return Impulse;
}

void InputLink::Clean()
{
	Impulse = 0.0;
	Status = 0;
}

bool InputLink::IsBusy()
{
	return (Status);
}

bool InputLink::IsFree()
{
	return (!Status);
}

#pragma endregion

#pragma region InputLinkList Implementation

// InputLinkList Implementation
InputLinkList::InputLinkList(Neuron* _Owner)
{
	Head = NULL;
	Tail = NULL;
	Owner = _Owner;
}

InputLinkList::~InputLinkList()
{
	InputLink* _Old = Head;
	InputLink* _Current = Head;
	while (_Current != NULL)
	{
		_Old = _Current;
		_Current = _Current->Next;
		delete _Old;
	}
}

InputLink* InputLinkList::Add(Synapse* _From)
{
	InputLink* Element = new InputLink(_From, Owner);
	if (Head != NULL)
	{
		Tail->Next = Element;
		Tail = Element;
	}
	else
	{
		Head = Element;
		Tail = Element;
	}
	return Element;
}

void InputLinkList::Remove(Synapse* _From)
{
	if (Head->From != _From)
	{
		InputLink* _Current = Head;
		while ((_Current->Next != NULL) && (_Current->Next->From != _From)) _Current = _Current->Next;
		if (_Current->Next != NULL)
		{
			InputLink* _Temp = _Current;
			_Temp = _Temp->Next;
			_Current->Next = _Temp->Next;
			delete _Temp;
		}
		else
		{
			Error("InputLink not found");
		}
	}
	else
	{
		InputLink* _Temp = Head;
		Head = Head->Next;
		delete _Temp;
	}
}

bool InputLinkList::IsAllBusy()
{
	bool Result = 1;
	InputLink* _Current = Head;
	while ((Result == 1) && (_Current != NULL))
	{
		if (!(_Current->IsBusy())) Result = 0;
		_Current = _Current->Next;
	}
	return Result;
}

double InputLinkList::Summarize()
{
	InputLink* _Current = Head;
	double Result = 0.0;
	while (_Current != NULL)
	{
		Result += _Current->GetImpulse();
		_Current = _Current->Next;
	}
	return Result;
}

void InputLinkList::CleanAll()
{
	InputLink* _Current = Head;
	while (_Current != NULL)
	{
		_Current->Clean();
		_Current = _Current->Next;
	}
}

#pragma endregion

#pragma region OutputLink Implementation

// OutputLink Implementation
OutputLink::OutputLink(Synapse* _Target, Neuron* _Owner)
{
	Owner = _Owner;
	Target = _Target;
	Next = NULL;
}

OutputLink::~OutputLink() {};

void OutputLink::SendImpulse(double _Impulse)
{
	Target->Transfer(_Impulse);
}

#pragma endregion

#pragma region OutputLinkList Implementation

// OutputLinkList Implementation
OutputLinkList::OutputLinkList(Neuron* _Owner)
{
	Owner = _Owner;
	Head = NULL;
	Tail = NULL;
}

OutputLinkList::~OutputLinkList()
{
	OutputLink* _Old = Head;
	OutputLink* _Current = Head;
	while (_Current != NULL)
	{
		_Old = _Current;
		_Current = _Current->Next;
		delete _Old;
	}
}

OutputLink* OutputLinkList::Search(Neuron* _Example)
{
	OutputLink* _Current = Head;
	while ((_Current->Target->To->Owner != _Example) && (_Current != NULL)) _Current = _Current->Next;
	return _Current;
}

OutputLink* OutputLinkList::Add(Synapse* _To)
{
	OutputLink* Element = new OutputLink(_To, Owner);
	if (Head != NULL)
	{
		Tail->Next = Element;
		Tail = Element;
	}
	else
	{
		Head = Element;
		Tail = Element;
	}
	return Element;
}

void OutputLinkList::Remove(Synapse* _To)
{
	if (Head->Target != _To)
	{
		OutputLink* _Current = Head;
		while ((_Current->Next != NULL) && (_Current->Next->Target != _To)) _Current = _Current->Next;
		if (_Current->Next != NULL)
		{
			OutputLink* _Temp = _Current;
			_Temp = _Temp->Next;
			_Current->Next = _Temp->Next;
			delete _Temp;
		}
		else
		{
			Error("No OutputLink found");
		}
	}
	else
	{
		OutputLink* _Temp = Head;
		Head = Head->Next;
		delete _Temp;
	}
}

void OutputLinkList::SendAll(double _Impulse)
{
	OutputLink* _Current = Head;
	while (_Current != NULL)
	{
		_Current->SendImpulse(_Impulse);
		_Current = _Current->Next;
	}
}

#pragma endregion

#pragma region Neuron Implementation

// Neuron Implementation
Neuron::Neuron()
{
	InLinkList = new InputLinkList(this);
	OutLinkList = new OutputLinkList(this);
	Core = NULL;
	Impulse = 0.0;
}

Neuron::~Neuron()
{
	delete InLinkList;
	delete OutLinkList;
}

void Neuron::SetCore(CoreFunction _Core)
{
	Core = _Core;
}

InputLink* Neuron::AddInLink(Synapse* _From)
{
	return InLinkList->Add(_From);
}

void Neuron::RemoveInLink(Synapse* _From)
{
	InLinkList->Remove(_From);
}

OutputLink* Neuron::AddOutLink(Synapse* _To)
{
	return OutLinkList->Add(_To);
}

void Neuron::RemoveOutLink(Synapse* _To)
{
	OutLinkList->Remove(_To);
}

Synapse* Neuron::Attach(Neuron* _What, double _Weight)
{
	Synapse* _Bridge = new Synapse();
	OutputLink* _OutLink = OutLinkList->Add(_Bridge);
	InputLink* _InLink = _What->AddInLink(_Bridge);
	_Bridge->From = _OutLink;
	_Bridge->To = _InLink;
	_Bridge->SetWeight(_Weight);
	return _Bridge;
}

void Neuron::Detach(Neuron* _What)
{
	OutputLink* _OutLink = OutLinkList->Search(_What);
	_OutLink->Target->Destroy();
}

Synapse* Neuron::GetSynapse(Neuron* _What)
{
	return (OutLinkList->Search(_What)->Target);
}

void Neuron::Activate()
{
	if (InLinkList->IsAllBusy())
	{
		double Argument = InLinkList->Summarize();
		Impulse = Core(Argument);
		OutLinkList->SendAll(Impulse);
	}
	else Error("Not ready yet!");
}

void Neuron::GenerateImpulse(double _Impulse = 0.0)
{
	OutLinkList->SendAll(_Impulse);
	Impulse = _Impulse;
}

double Neuron::GetImpulse()
{
	return Impulse;
}

#pragma endregion

class Layer
{

};

#endif