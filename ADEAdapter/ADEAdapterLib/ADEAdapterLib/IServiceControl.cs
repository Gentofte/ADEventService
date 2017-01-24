using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GK.AppCore.Threads;

namespace ADEAdapterLib
{
    // ================================================================================
    public interface IServiceControl : IRunner
    {
        // Inherit from IRunner in order to register in IoC
    }
}
