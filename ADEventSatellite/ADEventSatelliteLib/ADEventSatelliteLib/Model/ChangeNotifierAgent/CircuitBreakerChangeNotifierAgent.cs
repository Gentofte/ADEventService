using GK.AD;
using GK.AppCore.Error;
using GK.AppCore.Utility;
using System;

namespace ADEventSatellite.Model
{
    // ================================================================================
    public class CircuitBreakerChangeNotifierAgent : IChangeNotifierAgent
    {
        readonly IChangeNotifierAgent _innerAgent;
        readonly ICircuitBreaker2 _breaker;

        // -----------------------------------------------------------------------------
        public CircuitBreakerChangeNotifierAgent(IChangeNotifierAgent agent, ICircuitBreaker2 breaker)
        {
            _innerAgent = Verify.ArgumentNotNull(agent, "agent");
            _breaker = Verify.ArgumentNotNull(breaker, "breaker");
        }

        // -----------------------------------------------------------------------------
        public ChangeNotifier NewChangeNotifier(EventHandler<ADObjectChangedEventArgs> handler)
        {
            _breaker.Guard();
            try
            {
                ChangeNotifier cn = _innerAgent.NewChangeNotifier(handler);
                _breaker.Succeed();
                return cn;
            }
            catch (Exception ex)
            {
                _breaker.Trip(ex);
                throw;
            }
        }

        // -----------------------------------------------------------------------------
        public void DisposeChangeNotifier(ref ChangeNotifier cn)
        {
            _innerAgent.DisposeChangeNotifier(ref cn);
        }
    }
}
