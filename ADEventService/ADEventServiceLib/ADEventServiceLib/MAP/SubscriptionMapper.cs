using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ADEventService.Models;

namespace ADEventService.MAP
{
    // ================================================================================
    public class SubscriptionMapper : ISubscriptionMapper
    {
        //// -----------------------------------------------------------------------------
        //public ISubscription CreateModelFromDTO(ADEventService.DTO.ISubscription subscription)
        //{
        //    ISubscription subModel = new Subscription();

        //    subModel.Id = subscription.Id;
        //    subModel.Name = subscription.Name;
        //    subModel.Description = subscription.Description;
        //    subModel.Endpoint = subscription.Endpoint;
        //    subModel.Mail = subscription.Mail;
        //    subModel.Enabled = subscription.Enabled;

        //    return subModel;
        //}

        //// -----------------------------------------------------------------------------
        //public ADEventService.DTO.ADEventSubscription CreateDTOFromModel(ISubscription subscription)
        //{
        //    ADEventService.DTO.ADEventSubscription subDTO = new DTO.ADEventSubscription();

        //    subDTO.Id = subscription.Id;
        //    subDTO.Name = subscription.Name;
        //    subDTO.Description = subscription.Description;
        //    subDTO.Endpoint = subscription.Endpoint;
        //    subDTO.Mail = subscription.Mail;
        //    subDTO.Enabled = subscription.Enabled;

        //    return subDTO;
        //}

        //// -----------------------------------------------------------------------------
        //public ADEventService.DTO.ADEventSubscription[] CreateDTOArrayFromModel(ISubscription[] subscriptions)
        //{
        //    return null;
        //}

        //// -----------------------------------------------------------------------------
        //public List<ADEventService.DTO.ADEventSubscription> CreateDTOListFromModel(List<ISubscription> subscriptions)
        //{
        //    List<ADEventService.DTO.ADEventSubscription> subs = new List<ADEventService.DTO.ADEventSubscription>();
        //    foreach (var item in subscriptions)
        //    {
        //        subs.Add(CreateDTOFromModel(item) as ADEventService.DTO.ADEventSubscription);
        //    }

        //    return subs;
        //}

        //// -----------------------------------------------------------------------------
        //public List<ADEventService.DTO.ADEventSubscription> CreateDTOListFromModel(ISubscription[] subscriptions)
        //{
        //    List<ADEventService.DTO.ADEventSubscription> subs = new List<ADEventService.DTO.ADEventSubscription>();
        //    foreach (var item in subscriptions)
        //    {
        //        subs.Add(CreateDTOFromModel(item) as ADEventService.DTO.ADEventSubscription);
        //    }

        //    return subs;
        //}

        // -----------------------------------------------------------------------------
        public ICreateSubscriptionRequest CreateFromDTO(ADEventService.DTO.CreateSubscriptionRequest createRequest)
        {
            var requestModel = new CreateSubscriptionRequest()
            {
                Name = createRequest.Name,
                Description = createRequest.Description,
                Endpoint = createRequest.Endpoint,
                ContactEmail = createRequest.ContactEmail
            };
            return requestModel;
        }

        // -----------------------------------------------------------------------------
        public DTO.Subscription CreateDTOFromModel(ISubscription mdlSubscription)
        {
            var dtoSubscription = new DTO.Subscription()
            {
                ID = mdlSubscription.ID,
                Name = mdlSubscription.Name,
                Description = mdlSubscription.Description,
                Endpoint = mdlSubscription.Endpoint,
                ContactEmail = mdlSubscription.ContactEmail,
                Enabled = mdlSubscription.Enabled
            };

            dtoSubscription.Enabled = true;

            return dtoSubscription;
        }


        // -----------------------------------------------------------------------------
        public IEnumerable<ADEventService.DTO.Subscription> CreateDTOFromModel(IEnumerable<ISubscription> mdlSubscriptions)
        {
            var result = new List<ADEventService.DTO.Subscription>();

            foreach (var item in mdlSubscriptions)
            {
                result.Add(CreateDTOFromModel(item));
            }

            return result;
        }
    }
}
