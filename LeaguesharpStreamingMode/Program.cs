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
        static Assembly lib = Assembly.Load(LeaguesharpStreamingMode.Properties.Resources.LeaguesharpStreamingModelib); 
        static void Main(string[] args)
        {
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
            watermarkChanging = 3,
            changePrintChat = 4
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
            offsets.Add("4.21", new Int32[] { 0x6420, 0xA230, 0xA1B5, 0xDA30, 0xD8C8 });
        }

        static void Enable()
        {
            WriteMemory(LeaguesharpCore + offsets[version][(int)functionOffset.drawEvent], (byte)asm.ret);
            WriteMemory(LeaguesharpCore + offsets[version][(int)functionOffset.printChat], (byte)asm.ret);
            WriteMemory(LeaguesharpCore + offsets[version][(int)functionOffset.loadingScreenWatermark], new byte[] { (byte)asm.nop, (byte)asm.nop, (byte)asm.nop, 
                                                                                                         (byte)asm.nop, (byte)asm.nop, (byte)asm.nop });
            WriteMemory(LeaguesharpCore + offsets[version][(int)functionOffset.watermarkChanging],      new byte[] { (byte)0x57, (byte)0x48, (byte)0x4B, 
                                                                                                         (byte)0x44, (byte)0x4C, (byte)0x6F, (byte)0x61, 
                                                                                                         (byte)0x64, (byte)0x65, (byte)0x72, (byte)0x20 });
        
            WriteMemory(LeaguesharpCore + offsets[version][(int)functionOffset.changePrintChat],        new byte[] { (byte)0x3C,(byte)0x66 ,(byte)0x6F ,
                                                                                                         (byte)0x6E,(byte)0x74,(byte)0x20,(byte)0x63,
                                                                                                         (byte)0x6F,(byte)0x6C,(byte)0x6F,(byte)0x72,
                                                                                                         (byte)0x3D,(byte)0x22,(byte)0x23,(byte)0x34,
                                                                                                         (byte)0x30,(byte)0x63,(byte)0x31,(byte)0x66,
                                                                                                         (byte)0x66,(byte)0x22,(byte)0x3E,(byte)0x57,
                                                                                                         (byte)0x48,(byte)0x4B,(byte)0x44,(byte)0x4C,
                                                                                                         (byte)0x6F,(byte)0x61,(byte)0x64,(byte)0x65,
                                                                                                         (byte)0x72,(byte)0x20 ,(byte)0x2D ,(byte)0x2D ,
                                                                                                         (byte)0x20 ,(byte)0x76 ,(byte)0x65 ,(byte)0x72 ,
                                                                                                         (byte)0x73 ,(byte)0x2E ,(byte)0x3C ,(byte)0x2F ,
                                                                                                         (byte)0x66 ,(byte)0x6F ,(byte)0x6E ,(byte)0x74 ,
                                                                                                         (byte)0x3E ,(byte)0x20 ,(byte)0x3C ,(byte)0x66 ,
                                                                                                         (byte)0x6F ,(byte)0x6E ,(byte)0x74 ,(byte)0x20 ,
                                                                                                         (byte)0x63 ,(byte)0x6F ,(byte)0x6C ,(byte)0x6F ,
                                                                                                         (byte)0x72 ,(byte)0x3D ,(byte)0x22 ,(byte)0x23 ,
                                                                                                         (byte)0x33 ,(byte)0x39 ,(byte)0x46 ,(byte)0x46 ,
                                                                                                         (byte)0x31 ,(byte)0x34 ,(byte)0x22 ,(byte)0x3E ,
                                                                                                         (byte)0x25 ,(byte)0x73 ,(byte)0x20 ,(byte)0x25 ,
                                                                                                         (byte)0x73 ,(byte)0x3C ,(byte)0x2F ,(byte)0x66 ,
                                                                                                         (byte)0x6F ,(byte)0x6E ,(byte)0x74 ,(byte)0x3E ,
                                                                                                         (byte)0x00 ,(byte)0x00 ,(byte)0x00 ,(byte)0x00 ,
                                                                                                         (byte)0x00 ,(byte)0x62 ,(byte)0x79 ,(byte)0x20 ,
                                                                                                         (byte)0x57 ,(byte)0x48 ,(byte)0x4B ,(byte)0x44 ,
                                                                                                         (byte)0x20 ,(byte)0x2D ,(byte)0x20 ,(byte)0x73 ,
                                                                                                         (byte)0x6B ,(byte)0x79 ,(byte)0x70 ,(byte)0x65 ,
                                                                                                         (byte)0x3A ,(byte)0x72 ,(byte)0x6F ,(byte)0x73 ,
                                                                                                         (byte)0x65 ,(byte)0x6E ,(byte)0x62 ,(byte)0x65 ,
                                                                                                         (byte)0x72 ,(byte)0x67 ,(byte)0x63 ,(byte)0x69 ,
                                                                                                         (byte)0x6B ,(byte)0x20 ,(byte)0x2D ,(byte)0x20 ,
                                                                                                         (byte)0x45 ,(byte)0x6C ,(byte)0x6D ,(byte)0x6F ,
                                                                                                         (byte)0x20 ,(byte)0x4B ,(byte)0x65 ,(byte)0x6E ,
                                                                                                         (byte)0x6E ,(byte)0x65 ,(byte)0x64 ,(byte)0x79 ,
                                                                                                         (byte)0x20 ,(byte)0x6F ,(byte)0x6E ,(byte)0x20 ,
                                                                                                         (byte)0x46 ,(byte)0x61 ,(byte)0x63 ,(byte)0x65 ,
                                                                                                         (byte)0x62 ,(byte)0x6F ,(byte)0x6F ,(byte)0x6B ,
                                                                                                         (byte)0x20 ,(byte)0x2D ,(byte)0x20 ,(byte)0x73 ,
                                                                                                         (byte)0x61 ,(byte)0x64 ,(byte)0x62 ,(byte)0x6F ,
                                                                                                         (byte)0x79 ,(byte)0x73 ,(byte)0x20 ,(byte)0x32 ,
                                                                                                         (byte)0x30 ,(byte)0x30 ,(byte)0x35 });
            
        }

        static void OnWndProc(LeagueSharp.WndEventArgs args)
        {
            for (int i = 0; i < 2; i++)
                Enable();
        }
    }
}
