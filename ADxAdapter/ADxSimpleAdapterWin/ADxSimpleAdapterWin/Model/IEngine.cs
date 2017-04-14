using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADxSimpleAdapterWin.Model
{
    // ================================================================================
    public interface IEngine
    {
        // -----------------------------------------------------------------------------
        void InitEngine();

        // -----------------------------------------------------------------------------
        void StartEngine();

        // -----------------------------------------------------------------------------
        void StopEngine();

        // -----------------------------------------------------------------------------
        bool IsEngineON { get; }

        // -----------------------------------------------------------------------------
        string GetEngineInfo();
    }
}
