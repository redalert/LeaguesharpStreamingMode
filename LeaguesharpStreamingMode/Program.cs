using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LeaguesharpStreamingMode
{
    class Program
    {
        static private bool timerz;
        static Assembly lib = Assembly.Load(LeaguesharpStreamingMode.Properties.Resources.LeaguesharpStreamingModelib); 
        static void Main(string[] args)
        {
            Enable();
            SetUpOffsets();
            LeagueSharp.Game.OnWndProc += OnWndProc;
        }

        static Int32 GetModuleAddress(String ModuleName)
        {
            Process P = Process.GetCurrentProcess();
            for (int i = 0; i < P.Modules.Count; i++)
                if (P.Modules[i].ModuleName == ModuleName)
                    return (Int32)(P.Modules[i].BaseAddress);
            return 0;
        }

        static byte[] ReadMemory(Int32 address, Int32 length)
        {
            MethodInfo _ReadMemory = lib.GetType("LeaguesharpStreamingModelib.MemoryModule").GetMethods()[2];
            return (byte[])_ReadMemory.Invoke(null, new object[] { address, length });  
        }

        static void WriteMemory(Int32 address, byte value)
        {
            MethodInfo _WriteMemory = lib.GetType("LeaguesharpStreamingModelib.MemoryModule").GetMethods()[4];
            _WriteMemory.Invoke(null, new object[] { address, value });
        }

        static void WriteMemory(Int32 address, byte[] array)
        {
            for (int i = 0; i < array.Length; i++)
                WriteMemory(address + i, array[i]);
        }

        static string version = LeagueSharp.Game.Version.Substring(0, 4);
        static Int32 LeaguesharpCore = GetModuleAddress("Leaguesharp.Core.dll");
        static Dictionary<string, Int32[]> offsets;

        enum functionOffset : int
        {
            drawEvent = 0,
            printChat = 1,
            loadingScreenWatermark = 2,
            watermarkChanging = 3
        }

        enum asm : byte
        {
            ret = 0xC3,
            push_ebp = 0x55,
            nop = 0x90
        }

        static void SetUpOffsets()
        {   
            offsets = new Dictionary<string, Int32[]>();
            offsets.Add("4.19", new Int32[] { 0x5F40, 0x9B60, 0x9B40 });
            offsets.Add("4.20", new Int32[] { 0x6040, 0x9C00, 0x9BE0 });
            offsets.Add("4.21", new Int32[] { 0x6420, 0xA230, 0xA1B5, 0xDA30 });
        }

        static void Enable()
        {
            WriteMemory(LeaguesharpCore + offsets[version][(int)functionOffset.printChat], (byte)asm.ret);
            WriteMemory(LeaguesharpCore + offsets[version][(int)functionOffset.loadingScreenWatermark], new byte[] { (byte)asm.nop, (byte)asm.nop, (byte)asm.nop, 
                                                                                                         (byte)asm.nop, (byte)asm.nop, (byte)asm.nop });
            WriteMemory(LeaguesharpCore + offsets[version][(int)functionOffset.watermarkChanging],      new byte[] { (byte)0x57, (byte)0x48, (byte)0x4B, 
                                                                                                         (byte)0x44, (byte)0x4C, (byte)0x6F, (byte)0x61, 
                                                                                                         (byte)0x64, (byte)0x65, (byte)0x72, (byte)0x20 });
        }

        static void OnWndProc(LeagueSharp.WndEventArgs args)
        {
            if (args.Msg == 0x100) //WM_KEYDOWN
            {
                if (hotkeys.Contains(args.WParam))
                {
                    if (IsEnabled())
                        Enable();
                    else
                        Enable();
                }
            }
        }
    }
}
