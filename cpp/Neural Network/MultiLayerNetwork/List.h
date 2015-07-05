#pragma once
#ifndef _LIST_H
#define _LIST_H
#include <list>

template <class T> class List
{
protected:
	std::list<T>* Array;
	int Length;
public:
	List()
	{
		Array = new std::list<T>();
		Length = 0;
	}
	~List()
	{
		delete Array;
	}
	void Add(T* _Element)
	{
		Array->push_back(_Element);
		Length++;
	}
	void Remove(T* _Element)
	{
		Array->remove(_Element);
		Length--;
	}
	auto Begin()
	{
		return Array->begin();
	}
	auto End()
	{
		return Array->end();
	}
};

#endif