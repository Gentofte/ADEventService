using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GK.AD;

namespace ADEventService.Models
{
    // ================================================================================
    public interface IRawEventFilter
    {
        // -----------------------------------------------------------------------------
        void Init();

        // -----------------------------------------------------------------------------
        void CleanupOnExit();

        // -----------------------------------------------------------------------------
        IADObject FilterObject(IADObject adObj, DateTime changeTS);

        // -----------------------------------------------------------------------------
        void FlushCache();
    }
}
