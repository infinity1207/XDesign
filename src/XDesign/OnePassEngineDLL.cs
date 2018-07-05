using System;
using System.Runtime.InteropServices;

namespace Xyz.Pcs.DataType.DLLWrap
{
    //.net
    [StructLayoutAttribute(LayoutKind.Sequential, Pack = 1)]
    public struct SJobEngineParam
    {
        [MarshalAsAttribute(UnmanagedType.I4)]
        public Int32 pageCount;
        [MarshalAsAttribute(UnmanagedType.I4)]
        public Int32 jobResolution_X;
        [MarshalAsAttribute(UnmanagedType.I4)]
        public Int32 jobResolution_Y;
        [MarshalAsAttribute(UnmanagedType.I4)]
        public Int32 colorCount;
        [MarshalAsAttribute(UnmanagedType.I4)]
        public Int32 bitsPerPixel;
    };
    //.net
    public enum EJobType : Int32
    {
        None,
        Engine,
        Pattern_Line,
    };

    //.net
    [StructLayoutAttribute(LayoutKind.Sequential, Pack = 1)]
    public struct SPageColorInfo
    {
        [MarshalAsAttribute(UnmanagedType.I4)]
        public Int32 width;
        [MarshalAsAttribute(UnmanagedType.I4)]
        public Int32 height;
        [MarshalAsAttribute(UnmanagedType.I4)]
        public Int32 bitsPerPixel;
        [MarshalAsAttribute(UnmanagedType.I4)]
        public Int32 bytesPerLine;
        public IntPtr buffer;
        [MarshalAsAttribute(UnmanagedType.I4)]
        public Int32 bufferSize;
    };


    public class OnePassEngineDLL
    {
        //#region log
        //private static log4net.ILog _log = null;
        //private static log4net.ILog log
        //{
        //    get
        //    {
        //        if (_log == null)
        //        {
        //            string loggerName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString();
        //            _log = log4net.LogManager.GetLogger(loggerName);
        //        }
        //        return _log;
        //    }
        //}
        //#endregion

        //Initialize
        [DllImportAttribute("OnePassEngine.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAsAttribute(UnmanagedType.I1)]
        private static extern bool OPE_Initialize();
        public static bool Initialize()
        {
            return OPE_Initialize();
        }

        [DllImportAttribute("OnePassEngine.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAsAttribute(UnmanagedType.I1)]
        private static extern bool OPE_UnInitialize();
        public static bool UnInitialize()
        {
            return OPE_UnInitialize();
        }

        [DllImportAttribute("OnePassEngine.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAsAttribute(UnmanagedType.I1)]
        private static extern bool OPE_Usage_1_EmulatePrint(Int32 pageCount);
        public static bool Usage_1_EmulatePrint(Int32 pageCount)
        {
            return OPE_Usage_1_EmulatePrint(pageCount);
        }

        //[DllImportAttribute("OnePassEngine.dll", CallingConvention = CallingConvention.Cdecl)]
        //[return: MarshalAsAttribute(UnmanagedType.I4)]
        //private static extern EPrinterStatus OPE_PrinterStatus_Get();
        //public static EPrinterStatus PrinterStatus_Get()
        //{
        //    return OPE_PrinterStatus_Get();
        //}

        [DllImportAttribute("OnePassEngine.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAsAttribute(UnmanagedType.I1)]
        private static extern bool OPE_Wait_Ready(UInt32 time_ms);
        public static bool Wait_Ready(UInt32 time_ms)
        {
            return OPE_Wait_Ready(time_ms);
        }

        [DllImportAttribute("OnePassEngine.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAsAttribute(UnmanagedType.I4)]
        private static extern Int32 OPE_ColorCount_Get();
        public static Int32 ColorCount_Get()
        {
            return OPE_ColorCount_Get();
        }
        [DllImportAttribute("OnePassEngine.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAsAttribute(UnmanagedType.I4)]
        private static extern Int32 OPE_ColorWidth_Get(Int32 colorIndex);
        public static Int32 ColorWidth_Get(Int32 colorIndex)
        {
            return OPE_ColorWidth_Get(colorIndex);
        }

        [DllImportAttribute("OnePassEngine.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr OPE_Print_Begin(ref SJobEngineParam engineParam, EJobType jobType);
        public static IntPtr Print_Begin(ref SJobEngineParam engineParam, EJobType jobType)
        {
            return OPE_Print_Begin(ref engineParam, jobType);
        }

        [DllImportAttribute("OnePassEngine.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAsAttribute(UnmanagedType.I1)]
        private static extern bool OPE_Print_ColorData_ExistBuffer(IntPtr printHandle, UInt32 time_ms);
        public static bool Print_ColorData_ExistBuffer(IntPtr printHandle, UInt32 time_ms)
        {
            return OPE_Print_ColorData_ExistBuffer(printHandle, time_ms);
        }

        [DllImportAttribute("OnePassEngine.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAsAttribute(UnmanagedType.I1)]
        private static extern bool OPE_Print_ColorData_Add(IntPtr printHandle, IntPtr pageColorInfo, Int32 colorCount);
        public static bool Print_ColorData_Add(IntPtr printHandle, SPageColorInfo[] pageColorInfoArray)
        {
            return OPE_Print_ColorData_Add(
                printHandle,
                Marshal.UnsafeAddrOfPinnedArrayElement(pageColorInfoArray, 0),
                pageColorInfoArray.Length);
        }

        [DllImportAttribute("OnePassEngine.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAsAttribute(UnmanagedType.I1)]
        private static extern bool OPE_Print_ColorData_End(IntPtr printHandle);
        public static bool Print_ColorData_End(IntPtr printHandle)
        {
            return OPE_Print_ColorData_End(printHandle);
        }

        [DllImportAttribute("OnePassEngine.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAsAttribute(UnmanagedType.I1)]
        private static extern bool OPE_Print_IsAbort(IntPtr printHandle);
        public static bool Print_IsAbort(IntPtr printHandle)
        {
            return OPE_Print_IsAbort(printHandle);
        }

        [DllImportAttribute("OnePassEngine.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAsAttribute(UnmanagedType.I1)]
        private static extern bool OPE_Print_WaitExit(IntPtr printHandle, UInt32 time_ms);
        public static bool Print_WaitExit(IntPtr printHandle, UInt32 time_ms)
        {
            return OPE_Print_WaitExit(printHandle, time_ms);
        }

        [DllImportAttribute("OnePassEngine.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAsAttribute(UnmanagedType.I1)]
        private static extern bool OPE_Print_End(IntPtr printHandle);
        public static bool Print_End(IntPtr printHandle)
        {
            return OPE_Print_End(printHandle);
        }
    }
}
