using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADEventService.Models
{
    // ================================================================================
    public interface ISubscriptionRepo
    {
        // -----------------------------------------------------------------------------
        ISubscription CreateSubscription(ICreateSubscriptionRequest createSubscriptionRequest);

        // -----------------------------------------------------------------------------
        ISubscription Get(Guid id);

        // -----------------------------------------------------------------------------
        void Update(ISubscription subscription);

        // -----------------------------------------------------------------------------
        void Approve(ISubscription subscription);

        // -----------------------------------------------------------------------------
        void Enable(ISubscription subscription, bool enabled);

        // -----------------------------------------------------------------------------
        void RemoveSubscription(ISubscription subscription);

        // -----------------------------------------------------------------------------
        IEnumerable<ISubscription> GetAll();
    }
}
