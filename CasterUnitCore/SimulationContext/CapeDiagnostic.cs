using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CAPEOPEN;

namespace CasterUnitCore
{
    public static class CapeDiagnostic
    {
        static ICapeDiagnostic _diagnostic;

        public static void SetSimulationContext(ICapeDiagnostic diagnostic)
        {
            //if diagnostic is null, just ignore it
            _diagnostic = diagnostic;
        }

        public static void PopUpMessage(string msg, params object[] args)
        {
            _diagnostic.PopUpMessage(string.Format(msg, args));
        }

        public static void LogMessage(string msg, params object[] args)
        {
            _diagnostic.LogMessage(string.Format(msg, args));
        }
    }
}
