#pragma once

extern "C"
{
	__declspec(dllexport) void Pbgra32To1Bit(char* src, int srcStride, char* dest, int destStride, int leftPadding);
}
