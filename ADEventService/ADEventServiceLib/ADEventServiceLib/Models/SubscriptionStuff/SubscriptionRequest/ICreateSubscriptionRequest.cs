using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADEventService.Models
{
    // ================================================================================
    public interface ICreateSubscriptionRequest
    {
        // -----------------------------------------------------------------------------
        string Name { get; set; }
        // -----------------------------------------------------------------------------
        string Description { get; set; }
        // -----------------------------------------------------------------------------
        string Endpoint { get; set; }
        // -----------------------------------------------------------------------------
        string ContactEmail { get; set; }
    }
}
