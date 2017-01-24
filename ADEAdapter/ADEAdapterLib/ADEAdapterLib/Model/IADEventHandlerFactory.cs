using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADEAdapterLib.Model
{
    // ================================================================================
    public interface IADEventHandlerFactory
    {
        // -----------------------------------------------------------------------------
        IADEventHandler CreateEventHandler(GK.AD.DTO.ADEvent adEvent);
    }
}
