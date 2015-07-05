#ifndef _BITMAP_
#define _BITMAP_

#ifndef _CSTDIO_
#include <cstdio>
#endif

#pragma pack(2)
struct BITMAPFILEHEADER
{
	unsigned short int bfType;
	unsigned int bfSize;
	unsigned short int bfReserved1;
	unsigned short int bfReserved2;
	unsigned int bfOffBits;
};

struct BITMAPINFOHEADER
{
	unsigned int biSize;
	int biWidth;
	int biHeight;
	unsigned short int biPlanes;
	unsigned short int biBitCount;
	unsigned int biCompression;
	unsigned int biSizeImage;
	int XPelsPerMeter;
	int YPelsPerMeter;
	unsigned int biClrUsed;
	unsigned int biClrImportant;
};

class RGBColor
{
public:
	unsigned char R;
	unsigned char G;
	unsigned char B;
	RGBColor() { }
	RGBColor(unsigned char _R, unsigned char _G, unsigned char _B)
	{
		R = _R;
		G = _G;
		B = _B;
	}
	RGBColor& operator=(const RGBColor& Right)
	{
		if (this != &Right)
		{
			R = Right.R;
			G = Right.G;
			B = Right.B;
		}
		return *this;
	}
};

class Bitmap
{
private:
	int Height;
	int Width;
	RGBColor** Data;
public:
	Bitmap()
	{
		Height = 0;
		Width = 0;
		Data = NULL;
	}
	Bitmap(int _Height, int _Width)
	{
		Height = _Height;
		Width = _Width;
		Data = new RGBColor*[_Height];
		for (int i = 0; i < _Height; i++)
		{
			Data[i] = new RGBColor[_Width];
			for (int j = 0; j < Width; j++)
			{
				Data[i][j].R = 255;
				Data[i][j].G = 255;
				Data[i][j].B = 255;
			}
		}
	}
	Bitmap(const char* _FileName)
	{
		FILE* Input = fopen(_FileName, "rb");
		if (Input != NULL)
		{
			BITMAPFILEHEADER BitmapFileHeader;
			BITMAPINFOHEADER BitmapInfoHeader;
			fread(&BitmapFileHeader, sizeof(BitmapFileHeader), 1, Input);
			fread(&BitmapInfoHeader, sizeof(BitmapInfoHeader), 1, Input);
			unsigned char* _Data = new unsigned char[BitmapInfoHeader.biSizeImage];
			fseek(Input, BitmapFileHeader.bfOffBits, SEEK_SET);
			fread(_Data, BitmapInfoHeader.biSizeImage, 1, Input);
			Height = BitmapInfoHeader.biHeight;
			Width = BitmapInfoHeader.biWidth;
			Data = new RGBColor*[BitmapInfoHeader.biHeight];
			for (int i = 0; i < BitmapInfoHeader.biHeight; i++) Data[i] = new RGBColor[BitmapInfoHeader.biWidth];
			unsigned short int Padding = ((4 - ((3 * BitmapInfoHeader.biWidth) % 4)) % 4); // Bytes in row should % 4 == 0
			int _i = 0; // Position in _Data
			for (int i = BitmapInfoHeader.biHeight - 1; i >= 0; i--) // Bitmap rows are turned over
			{
				for (int j = 0; j < BitmapInfoHeader.biWidth; j++)
				{
					// Colors are storaged in BGR format
					Data[i][j].R = _Data[_i + 2];
					Data[i][j].G = _Data[_i + 1];
					Data[i][j].B = _Data[_i];
					_i += 3;
				}
				_i += Padding;
			}
			delete[] _Data;
			fclose(Input);
		}
		else {/*ERROR OPENING FILE*/ }
	}
	~Bitmap()
	{
		delete[] Data;
	}
	RGBColor*& operator[](int i)
	{
		return Data[i];
	}
	void LoadFromFile(const char* _FileName)
	{
		delete[] Data;
		FILE* Input = fopen(_FileName, "rb");
		if (Input != NULL)
		{
			BITMAPFILEHEADER BitmapFileHeader;
			BITMAPINFOHEADER BitmapInfoHeader;
			fread(&BitmapFileHeader, sizeof(BitmapFileHeader), 1, Input);
			fread(&BitmapInfoHeader, sizeof(BitmapInfoHeader), 1, Input);
			unsigned char* _Data = new unsigned char[BitmapInfoHeader.biSizeImage];
			fseek(Input, BitmapFileHeader.bfOffBits, SEEK_SET);
			fread(_Data, BitmapInfoHeader.biSizeImage, 1, Input);
			Height = BitmapInfoHeader.biHeight;
			Width = BitmapInfoHeader.biWidth;
			Data = new RGBColor*[BitmapInfoHeader.biHeight];
			for (int i = 0; i < BitmapInfoHeader.biHeight; i++) Data[i] = new RGBColor[BitmapInfoHeader.biWidth];
			unsigned short int Padding = ((4 - ((3 * BitmapInfoHeader.biWidth) % 4)) % 4); // Bytes in row should % 4 == 0
			int _i = 0; // Position in _Data
			for (int i = BitmapInfoHeader.biHeight - 1; i >= 0; i--) // Bitmap rows are turned over
			{
				for (int j = 0; j < BitmapInfoHeader.biWidth; j++)
				{
					// Colors are storaged in BGR format
					Data[i][j].R = _Data[_i + 2];
					Data[i][j].G = _Data[_i + 1];
					Data[i][j].B = _Data[_i];
					_i += 3;
				}
				_i += Padding;
			}
			delete[] _Data;
			fclose(Input);
		}
		else {/*ERROR OPENING FILE*/ }
	}
	void SaveToFile(const char* _FileName)
	{
		FILE* Output = fopen(_FileName, "wb");
		if (Output != NULL)
		{
			BITMAPINFOHEADER BitmapInfoHeader;
			BitmapInfoHeader.biSize = sizeof(BITMAPINFOHEADER);
			BitmapInfoHeader.biWidth = Width;
			BitmapInfoHeader.biHeight = Height;
			BitmapInfoHeader.biPlanes = 1;
			BitmapInfoHeader.biBitCount = 24;
			BitmapInfoHeader.biCompression = 0l;
			BitmapInfoHeader.biSizeImage = 3 * Height * Width;
			BITMAPFILEHEADER BitmapFileHeader;
			BitmapFileHeader.bfType = 'B' + ('M' << 8);
			BitmapFileHeader.bfSize = BitmapInfoHeader.biSizeImage + sizeof(BITMAPFILEHEADER)+sizeof(BITMAPINFOHEADER);
			BitmapFileHeader.bfReserved1 = 0;
			BitmapFileHeader.bfReserved2 = 0;
			BitmapFileHeader.bfOffBits = sizeof(BITMAPFILEHEADER)+sizeof(BITMAPINFOHEADER);
			fwrite(&BitmapFileHeader, 1, sizeof(BITMAPFILEHEADER), Output);
			fwrite(&BitmapInfoHeader, 1, sizeof(BITMAPINFOHEADER), Output);
			unsigned short int Padding = ((4 - ((3 * Width) % 4)) % 4);
			unsigned char* ZeroArray = new unsigned char[Padding];
			for (int i = 0; i < Padding; i++) ZeroArray[i] = 0;
			for (int i = Height - 1; i >= 0; i--)
			{
				for (int j = 0; j < Width; j++)
				{
					fwrite(&Data[i][j].B, 1, 1, Output);
					fwrite(&Data[i][j].G, 1, 1, Output);
					fwrite(&Data[i][j].R, 1, 1, Output);
				}
				fwrite(ZeroArray, 1, Padding, Output);
			}
		}
		else {/*ERROR OPENING FILE*/ }
	}
	void Print()
	{
		for (int i = 0; i < Height; i++)
		{
			for (int j = 0; j < Width; j++) printf("(R: %d, G: %d, B: %d) ", Data[i][j].R, Data[i][j].G, Data[i][j].B);
			printf("\n");
		}
	}
};
#endif