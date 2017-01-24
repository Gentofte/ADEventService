using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ADEventService.Models;

namespace ADEventService.MAP
{
    // ================================================================================
    public interface ISubscriptionMapper
    {
        //// -----------------------------------------------------------------------------
        //ISubscription CreateModelFromDTO(ADEventService.DTO.ISubscription subscription);

        //// -----------------------------------------------------------------------------
        //ADEventService.DTO.ADEventSubscription CreateDTOFromModel(ISubscription subscription);

        //// -----------------------------------------------------------------------------
        //ADEventService.DTO.ADEventSubscription[] CreateDTOArrayFromModel(ISubscription[] subscriptions);

        //// -----------------------------------------------------------------------------
        //List<ADEventService.DTO.ADEventSubscription> CreateDTOListFromModel(List<ISubscription> subscriptions);

        //// -----------------------------------------------------------------------------
        //List<ADEventService.DTO.ADEventSubscription> CreateDTOListFromModel(ISubscription[] subscriptions);

        // -----------------------------------------------------------------------------
        ICreateSubscriptionRequest CreateFromDTO(ADEventService.DTO.CreateSubscriptionRequest createRequest);

        // -----------------------------------------------------------------------------
        ADEventService.DTO.Subscription CreateDTOFromModel(ISubscription mdlSubscription);

        // -----------------------------------------------------------------------------
        IEnumerable<ADEventService.DTO.Subscription> CreateDTOFromModel(IEnumerable<ISubscription> mdlSubscription);
    }
}
