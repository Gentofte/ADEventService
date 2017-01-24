using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GK.AppCore.Threads;

namespace ADEAdapterLib.Model
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
        IWorkerCollection GetWorkerCollection();

        // -----------------------------------------------------------------------------
        string GetEngineInfo();
    }
}
