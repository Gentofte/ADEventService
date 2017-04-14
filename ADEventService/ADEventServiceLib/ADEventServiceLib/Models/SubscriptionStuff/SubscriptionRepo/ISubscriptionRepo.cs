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
        ICreateSubscriptionRequest CreateSubscriptionRequest(Guid id, string name, string description, string endpoint, string contactEmail);

        // -----------------------------------------------------------------------------
        ISubscription CreateSubscription(ICreateSubscriptionRequest createSubscriptionRequest);

        // -----------------------------------------------------------------------------
        ISubscription Get(Guid id);

        // -----------------------------------------------------------------------------
        ISubscription TryGet(Guid id);

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
