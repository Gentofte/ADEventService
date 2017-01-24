using System;
using GK.AppCore.Common;

namespace ADEventService.Models
{
    // ================================================================================
    public interface ISubscription
    {
        Guid ID { get; set; }

        string Name { get; set; }
        string Description { get; set; }
        string Endpoint { get; set; }
        string ContactEmail { get; set; }

        bool Approved { get; set; }
        bool Enabled { get; set; }
        bool PublishON { get; set; }

        Guid Version { get; set; }

        string ToString(bool fullRecord = false);
    }
}