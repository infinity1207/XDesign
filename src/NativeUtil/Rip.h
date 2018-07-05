#pragma once

extern "C" __declspec(dllexport) void Foo();
extern "C" __declspec(dllexport) void Pbgra32To1Bit(char* src, DWORD width, DWORD height, DWORD srcStride, char* dest, DWORD destStride, DWORD leftPadding);
