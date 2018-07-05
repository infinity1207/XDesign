#include "stdafx.h"
#include "Rip.h"

void Foo()
{

}

void Pbgra32To1Bit(char* src, DWORD width, DWORD  height, DWORD srcStride, char* dest, DWORD  destStride, DWORD leftPadding)
{
    for (int j = 0; j < height; j++)
    {
        auto pSrc = (DWORD*)(src + srcStride * j);
        auto pDest = (BYTE*)(dest + destStride * j);

        BYTE v = 0;
        BYTE mask = 0x80;
        int k = 7;
        int s = 0;
        for (int i = 0; i < width; i++)
        {
            if (k < 0)
            {
                *(pDest + s) = v;

                v = 0;
                mask = 0x80;
                k = 7;
                s++;
            }

            if (*(pSrc + i) == 0xff000000)
            {
                v |= mask;
            }
            
            mask = mask >> 1;
            k--;
        }
    }
}
