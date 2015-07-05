#ifndef _Watches_
#define _Watches_

#ifndef _CTIME_
#include <ctime>
#endif

#ifndef _CSTDIO_
#include <cstdio>
#endif

#ifndef _CSTRING_
#include <cstring>
#endif

class Watch
{
	unsigned long int StartTime;
	unsigned long int StopTime;
	char* Name;
public:
	double Duration;
	Watch(const char* _Name = "Temp")
	{
		StartTime = clock();
		StopTime = 0;
		Duration = 0;
		Name = new char[strlen(_Name) + 1];
		strcpy(Name, _Name);
	}
	~Watch()
	{
		delete[] Name;
	}
	void Stop()
	{
		StopTime = clock();
		Duration = (double)(StopTime - StartTime) / 1000;
	}
	void Show()
	{
		if (StopTime == 0) Stop();
		printf("%s: %0.4f s\n", Name, Duration);
	}
};
#endif