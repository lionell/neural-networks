#ifndef _BIGINTEGER_
#define _BIGINTEGER_

#ifndef _CSTRING_
#include <cstring>
#endif

#ifndef _CSTDIO_
#include <cstdio>
#endif

#define MaxLength 1000
int TArray[MaxLength];
const int Base = 10;

// Declaration

class BigInteger
{
private:
	bool Sign;
	int Length;
	int* Array;

public:
	BigInteger()
	{
		Sign = 1;
		Length = 1;
		Array = new int[1];
		Array[0] = 0;
	}
	BigInteger(long long int Example)
	{
		if (Example != 0)
		{
			long long int _Example = (Example > 0) ? (Example) : (-Example);
			Sign = (Example > 0) ? 1 : 0;
			int _Length = 0;
			while (_Example > 0)
			{
				_Example /= Base;
				_Length++;
			}
			Length = _Length;
			Array = new int[_Length];
			_Example = (Example > 0) ? (Example) : (-Example);
			_Length = 0;
			for (int i = 0; i < Length; i++)
			{
				Array[_Length] = (int)(_Example % Base);
				_Example /= Base;
				_Length++;
			}
		}
		else
		{
			Sign = 1;
			Length = 1;
			Array = new int[1];
			Array[0] = 0;
		}
	}
	BigInteger(const BigInteger& Example)
	{
		Sign = Example.Sign;
		Length = Example.Length;
		Array = new int[Length];
		memcpy(Array, Example.Array, sizeof(int)* Example.Length);
	}
	~BigInteger()
	{
		delete[] Array;
	}
	friend void print(const BigInteger& Number);

	// Operators

	BigInteger& operator=(const BigInteger& Right)
	{
		if (this != &Right)
		{
			delete[] Array;
			Sign = Right.Sign;
			Length = Right.Length;
			Array = new int[Length];
			memcpy(Array, Right.Array, sizeof(int)* Right.Length);
		}
		return *this;
	}
	BigInteger& operator=(const long long int& Right)
	{
		delete[] Array;
		if (Right != 0)
		{
			long long int _Right = (Right >= 0) ? (Right) : (-Right);
			Sign = (Right >= 0) ? 1 : 0;
			int _Length = 0;
			while (_Right > 0)
			{
				_Right /= Base;
				_Length++;
			}
			Length = _Length;
			Array = new int[_Length];
			_Right = (Right >= 0) ? (Right) : (-Right);
			_Length = 0;
			for (int i = 0; i < Length; i++)
			{
				Array[_Length] = (int)(_Right % Base);
				_Right /= Base;
				_Length++;
			}
		}
		else
		{
			Sign = 1;
			Length = 1;
			Array = new int[1];
			Array[0] = 0;
		}
		return *this;
	}

	// Bool operators

	friend bool operator<(const BigInteger&, const BigInteger&); // BigInteger < BigInteger
	friend bool operator>(const BigInteger&, const BigInteger&); // BigInteger > BigInteger
	friend bool operator==(const BigInteger&, const BigInteger&); // BigInteger == BigInteger

	// Unary operators

	friend const BigInteger& operator-(const BigInteger&); // -BigInteger

	// Binary operators

	friend const BigInteger operator+(const BigInteger&, const BigInteger&); // BigInteger + BigInteger
	friend const BigInteger operator-(const BigInteger&, const BigInteger&); // BigInteger - BigInteger
	friend const BigInteger operator*(const BigInteger&, const BigInteger&); // BigInteger * BigInteger
	friend const BigInteger operator/(const BigInteger&, const long long int&); // BigInteger / long long int
	friend const long long int operator%(const BigInteger&, const long long int&);
};

// Implementation

// Less then
bool operator<(const BigInteger& Left, const BigInteger& Right)
{
	bool Result = 1;
	if ((Left.Sign == 1) && (Right.Sign == 1))
	{
		if (Left.Length == Right.Length)
		{
			int i = Left.Length - 1;
			while ((i >= 0) && (Left.Array[i] == Right.Array[i])) i--;
			if ((i < 0) || (Left.Array[i] > Right.Array[i])) Result = 0;
		}
		else Result = (Left.Length < Right.Length) ? 1 : 0;
	}
	else
	{
		if ((Left.Sign == 1) && (Right.Sign == 0)) Result = 0;
		if ((Left.Sign == 0) && (Right.Sign == 1)) Result = 1;
		if ((Left.Sign == 0) && (Right.Sign == 0)) Result = ((-Left) >(-Right));
	}
	return Result;
}

// More then
bool operator>(const BigInteger& Left, const BigInteger& Right)
{
	bool Result = 1;
	if ((Left.Sign == 1) && (Right.Sign == 1))
	{
		if (Left.Length == Right.Length)
		{
			int i = Left.Length - 1;
			while ((i >= 0) && (Left.Array[i] == Right.Array[i])) i--;
			if ((i < 0) || (Left.Array[i] < Right.Array[i])) Result = 0;
		}
		else Result = (Left.Length > Right.Length) ? 1 : 0;
	}
	else
	{
		if ((Left.Sign == 1) && (Right.Sign == 0)) Result = 1;
		if ((Left.Sign == 0) && (Right.Sign == 1)) Result = 0;
		if ((Left.Sign == 0) && (Right.Sign == 0)) Result = ((-Left) < (-Right));
	}
	return Result;
}

// Equals to
bool operator==(const BigInteger& Left, const BigInteger& Right)
{
	bool Result = 1;
	if (Left.Sign == Right.Sign)
	{
		if (Left.Length == Right.Length)
		{
			int i = Left.Length - 1;
			while ((i >= 0) && (Left.Array[i] == Right.Array[i])) i--;
			if (i >= 0) Result = 0;
		}
		else Result = 0;
	}
	else Result = 0;
	return Result;
}

// Unary minus
const BigInteger& operator-(const BigInteger& Left)
{
	BigInteger* Result = new BigInteger(Left);
	Result->Sign = !Result->Sign;
	return *Result;
}

// Binary plus
const BigInteger operator+(const BigInteger& Left, const BigInteger& Right)
{
	if (((Left.Sign == 0) && (Right.Sign == 0)) || ((Left.Sign == 1) && (Right.Sign == 1)))
	{
		int MaximalLength = (Left.Length > Right.Length) ? Left.Length : Right.Length;
		int MinimalLength = (Left.Length < Right.Length) ? Left.Length : Right.Length;
		memset(TArray, 0, sizeof(int)* MaximalLength);
		for (int i = 0; i < MinimalLength; i++)
		{
			TArray[i] += (Left.Array[i] + Right.Array[i]);
			TArray[i + 1] = TArray[i] / Base;
			TArray[i] %= Base;
		}
		if (Left.Length > Right.Length)
		{
			for (int i = MinimalLength; i < MaximalLength; i++)
			{
				TArray[i] += Left.Array[i];
				TArray[i + 1] = TArray[i] / Base;
				TArray[i] %= Base;
			}
		}
		if (Right.Length > Left.Length)
		{
			for (int i = MinimalLength; i < MaximalLength; i++)
			{
				TArray[i] += Right.Array[i];
				TArray[i + 1] = TArray[i] / Base;
				TArray[i] %= Base;
			}
		}
		BigInteger Result;
		Result.Length = (TArray[MaximalLength] != 0) ? (MaximalLength + 1) : (MaximalLength);
		Result.Sign = ((Left.Sign == 1) && (Right.Sign == 1)) ? 1 : 0;
		Result.Array = new int[Result.Length];
		memcpy(Result.Array, TArray, sizeof(int)* Result.Length);
		return Result;
	}
	else
	{
		if ((Left.Sign == 1) && (Right.Sign == 0)) return (Left - (-Right));
		if ((Left.Sign == 0) && (Right.Sign == 1)) return (Right - (-Left));
	}
	return Left; // returning trash
}

// Binary minus
const BigInteger operator-(const BigInteger& Left, const BigInteger& Right)
{
	if ((Left.Sign == 1) && (Right.Sign == 1))
	{
		if (Left > Right)
		{
			memset(TArray, 0, sizeof(int)* Left.Length);
			bool T = 0;
			for (int i = 0; i < Right.Length; i++)
			{
				if (T == 1)
				{
					if ((Left.Array[i] - 1) < Right.Array[i])
					{
						TArray[i] = (Left.Array[i] - 1 + Base) - Right.Array[i];
						T = 1;
					}
					else
					{
						TArray[i] = (Left.Array[i] - 1) - Right.Array[i];
						T = 0;
					}
				}
				else
				{
					if (Left.Array[i] < Right.Array[i])
					{
						TArray[i] = (Left.Array[i] + Base) - Right.Array[i];
						T = 1;
					}
					else
					{
						TArray[i] = Left.Array[i] - Right.Array[i];
						T = 0;
					}
				}
			}
			for (int i = Right.Length; i < Left.Length; i++)
			{
				if (T == 0)
				{
					TArray[i] = Left.Array[i];
				}
				else
				{
					if (Left.Array[i] > 0)
					{
						TArray[i] = Left.Array[i] - 1;
						T = 0;
					}
					else TArray[i] = Base - 1;
				}
			}
			int _Length = Left.Length - 1;
			while ((_Length >= 0) && (TArray[_Length] == 0)) _Length--;
			BigInteger Result;
			Result.Sign = 1;
			Result.Length = _Length + 1;
			Result.Array = new int[Result.Length];
			memcpy(Result.Array, TArray, sizeof(int)* Result.Length);
			return Result;
		}
		else
		{
			memset(TArray, 0, sizeof(int)* Right.Length);
			bool T = 0;
			for (int i = 0; i < Left.Length; i++)
			{
				if (T == 1)
				{
					if ((Right.Array[i] - 1) < Left.Array[i])
					{
						TArray[i] = (Right.Array[i] - 1 + Base) - Left.Array[i];
						T = 1;
					}
					else
					{
						TArray[i] = (Right.Array[i] - 1) - Left.Array[i];
						T = 0;
					}
				}
				else
				{
					if (Right.Array[i] < Left.Array[i])
					{
						TArray[i] = (Right.Array[i] + Base) - Left.Array[i];
						T = 1;
					}
					else
					{
						TArray[i] = Right.Array[i] - Left.Array[i];
						T = 0;
					}
				}
			}
			for (int i = Left.Length; i < Right.Length; i++)
			{
				if (T == 0)
				{
					TArray[i] = Right.Array[i];
				}
				else
				{
					if (Right.Array[i] > 0)
					{
						TArray[i] = Right.Array[i] - 1;
						T = 0;
					}
					else TArray[i] = Base - 1;
				}
			}
			int _Length = Right.Length - 1;
			while ((_Length >= 0) && (TArray[_Length] == 0)) _Length--;
			BigInteger Result;
			Result.Sign = 0;
			Result.Length = _Length + 1;
			Result.Array = new int[Result.Length];
			memcpy(Result.Array, TArray, sizeof(int)* Result.Length);
			return Result;
		}
	}
	else
	{
		if ((Left.Sign == 0) && (Right.Sign == 0)) return ((-Right) - (-Left));
		if ((Left.Sign == 1) && (Right.Sign == 0)) return (Left + (-Right));
		if ((Left.Sign == 0) && (Right.Sign == 1)) return -((-Left) + Right);
	}
	return Right; // returning trash
}

// Binary multiply
const BigInteger operator*(const BigInteger& Left, const BigInteger& Right)
{
	int ResultLength = Left.Length + Right.Length + 1;
	memset(TArray, 0, sizeof(int)* ResultLength);
	for (int i = 0; i < Left.Length; i++)
	{
		for (int j = 0; j < Right.Length; j++)
		{
			TArray[i + j] += (Left.Array[i] * Right.Array[j]);
		}
	}
	for (int i = 0; i < ResultLength; i++)
	{
		TArray[i + 1] += (TArray[i] / Base);
		TArray[i] %= Base;
	}
	ResultLength--;
	while ((ResultLength > 0) && (TArray[ResultLength] == 0)) ResultLength--;
	BigInteger Result;
	Result.Sign = (Left.Sign == Right.Sign) ? 1 : 0;
	Result.Length = ResultLength + 1;
	Result.Array = new int[Result.Length];
	memcpy(Result.Array, TArray, sizeof(int)* Result.Length);
	return Result;
}

// Binary division(long long int)
const BigInteger operator/(const BigInteger& Left, const long long int& Right)
{
	if (Right != 0)
	{
		int ResultLength = Left.Length;
		memset(TArray, 0, sizeof(int)* ResultLength);
		int Modulo = 0;
		for (int i = (Left.Length - 1); i >= 0; i--)
		{
			TArray[i] = Left.Array[i] + (Modulo * Base);
			Modulo = TArray[i] % Right;
			TArray[i] = (int)(TArray[i] / Right);
		}
		ResultLength--;
		while ((ResultLength > 0) && (TArray[ResultLength] == 0)) ResultLength--;
		BigInteger Result;
		if (((Right >= 0) && (Left.Sign == 1)) || ((Right < 0) && (Left.Sign == 0))) Result.Sign = 1;
		else Result.Sign = 0;
		Result.Length = ResultLength + 1;
		Result.Array = new int[Result.Length];
		memcpy(Result.Array, TArray, sizeof(int)* Result.Length);
		return Result;
	}
	else return Left; // ERROR: Division by zero!
}

// Binary modulo(long long int)
const long long int operator%(const BigInteger& Left, const long long int& Right)
{
	if (Right != 0)
	{
		memset(TArray, 0, sizeof(int)* Left.Length);
		int Modulo = 0;
		for (int i = (Left.Length - 1); i >= 0; i--)
		{
			TArray[i] = Left.Array[i] + (Modulo * Base);
			Modulo = TArray[i] % Right;
			TArray[i] = (int)(TArray[i] / Right);
		}
		return Modulo;
	}
	else return Right; // ERROR: Division by zero!
}

// Printing BigInteger
void print(const BigInteger& Number)
{
	if (Number.Sign == 0) printf("-");
	for (int i = (Number.Length - 1); i >= 0; i--) printf("%d", Number.Array[i]);
}

#endif