using ADEventService.Configuration;
using GK.AppCore.Utility;
using System;

using ADEventService.Controllers;

namespace ADEventService.Models
{
    // ================================================================================
    [Serializable]
    public class NullSubscription : Subscription, INullSubscription
    {
        readonly IADESConfig _config;

        // -----------------------------------------------------------------------------
        public NullSubscription(IADESConfig config)
        {
            _config = Verify.ArgumentNotNull(config, "config");

            ID = Guid.NewGuid();

            Name = string.Format("{0}-NULL", _config.ApplicationPrefix.ToUpperInvariant());
            Description = "ADEventService NULL subscription. Used internally by ADEventService";

            Endpoint = string.Format("{0}/{1}", _config.ServiceBaseURL, NotificationsController.RoutePrefix);
            ContactEmail = "";

            Approved = true;
            Enabled = true;
            PublishON = true;

            Version = Guid.NewGuid();
        }
    }
}