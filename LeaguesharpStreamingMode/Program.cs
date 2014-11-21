using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;

namespace LeaguesharpStreamingMode
{
    class Program
    {
        static Assembly lib = Assembly.Load(LeaguesharpStreamingMode.Properties.Resources.LeaguesharpStreamingModelib);
        static void Main(string[] args)
        {
            lib.GetType("LeaguesharpStreamingModelib.StreamingMode").GetMethod("Enable").Invoke(null, null);

            LeagueSharp.Game.OnWndProc += OnWndProc;
            AppDomain.CurrentDomain.DomainUnload += delegate
            {
                lib.GetType("LeaguesharpStreamingModelib.StreamingMode").GetMethod("Disable").Invoke(null, null);
            };
        }

        static Int32 GetModuleAddress(String ModuleName)
        {
            Process P = Process.GetCurrentProcess();
            for (int i = 0; i < P.Modules.Count; i++)
                if (P.Modules[i].ModuleName == ModuleName)
                    return (Int32)(P.Modules[i].BaseAddress);
            return 0;
        }

        static IntPtr LeaguesharpCore_PrintChat = new IntPtr(GetModuleAddress("Leaguesharp.Core.dll") + 0x9B80);
        static bool IsEnabled() { return (System.Runtime.InteropServices.Marshal.ReadByte(LeaguesharpCore_PrintChat) == 0xC3); }
        static uint HotKey = 0x24;  //home key
        static void OnWndProc(LeagueSharp.WndEventArgs args)
        {
            if (args.Msg == 0x100) //WM_KEYDOWN
            {
                if (args.WParam == HotKey)
                {
                    if (IsEnabled())
                        lib.GetType("LeaguesharpStreamingModelib.StreamingMode").GetMethod("Disable").Invoke(null, null);
                    else
                        lib.GetType("LeaguesharpStreamingModelib.StreamingMode").GetMethod("Enable").Invoke(null, null);
                }
            }
        }
    }
}
