//#include "MultiLayerNetwork.h"
#include "List.h"
#include <cstdio>

class Temp
{
private:
	int a;
public:
	Temp(int _a)
	{
		a = _a;
	}
	void Print()
	{
		printf("%d", a);
	}
};

int main()
{
	List<Temp> A;
	A.Add(new Temp(1));
	A.Add(new Temp(2));
	std::list<int> a;
	a.push_back(2);
	std::list<int>::iterator i;
	for (auto i = A.Begin(); i != A.End(); i++)
	{
		i->Print();
	}
	//List<Temp> A;
}