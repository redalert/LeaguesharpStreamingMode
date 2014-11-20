using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace LeaguesharpStreamingMode
{
    class Program
    {
        static Assembly lib = Assembly.Load(LeaguesharpStreamingMode.Properties.Resources.LeaguesharpStreamingModelib);
        static void Main(string[] args)
        {
            lib.GetType("LeaguesharpStreamingModelib.StreamingMode").GetMethod("Enable").Invoke(null, null);

            AppDomain.CurrentDomain.DomainUnload += delegate
            {
                lib.GetType("LeaguesharpStreamingModelib.StreamingMode").GetMethod("Disable").Invoke(null, null);
            };

        }
    }
}
