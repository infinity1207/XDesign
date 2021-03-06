#pragma once

extern "C" __declspec(dllexport) void Foo();

extern "C" __declspec(dllexport) void Pbgra32To1Bit(
    char* src,
    DWORD width,
    DWORD height,
    DWORD srcStride,
    char* dest,
    DWORD destStride,
    DWORD leftPadding);

extern "C" __declspec(dllexport) void Argb32To1Bit(
    char* src,
    DWORD width,
    DWORD height,
    DWORD srcStride,
    char* dest,
    DWORD destStride,
    DWORD leftPadding);


extern "C" __declspec(dllexport) void CopyPixels(
    char* src,
    DWORD srcStride,
    DWORD srcWidth,
    DWORD srcHeight,
    char* dest,
    DWORD destStride,
    DWORD destWidth,
    DWORD destHeight,
    DWORD x,
    DWORD y);
