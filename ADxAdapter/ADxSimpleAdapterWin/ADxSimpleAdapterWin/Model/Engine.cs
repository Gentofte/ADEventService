using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADxSimpleAdapterWin.Model
{
    // ================================================================================
    public class Engine : IEngine
    {
        public bool IsEngineON { get { return true; } }

        public string GetEngineInfo()
        {
            return "Status OK";
        }

        public void InitEngine()
        {
        }

        public void StartEngine()
        {
        }

        public void StopEngine()
        {
        }
    }
}
