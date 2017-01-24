using GK.AppCore.Threads;

namespace ADEventSatellite
{
    // ================================================================================
    public interface IServiceControl : IRunner
    {
        // Inherit from IRunner in order to register in IoC

        void Reset();
    }
}