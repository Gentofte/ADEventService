using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GK.AppCore.Threads;

namespace ADEventService.Models
{
    // ================================================================================
    public interface IADESEngine
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
    }
}
