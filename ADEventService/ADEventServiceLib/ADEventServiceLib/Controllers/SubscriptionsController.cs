using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Http;

using System.Web.Http;

using GK.AppCore.Utility;

using ADEventService.Configuration;
using ADEventService.MAP;
using ADEventService.Models;

namespace ADEventService.Controllers
{
    // ================================================================================
    [RoutePrefix(SubscriptionsController.RoutePrefix)]
    public class SubscriptionsController : ApiController
    {
        public const string RoutePrefix = "api/v1/subscriptions";

        readonly IADESConfig _config;
        readonly ISubscriptionRepo _subscriptionRepo;
        readonly ISubscriptionMapper _subscriptionMapper;

        // -----------------------------------------------------------------------------
        public SubscriptionsController(IADESConfig config, ISubscriptionRepo subscriptionRepo, ISubscriptionMapper subscriptionMapper)
        {
            _config = Verify.ArgumentNotNull(config, "config");
            _subscriptionRepo = Verify.ArgumentNotNull(subscriptionRepo, "subscriptionRepo");
            _subscriptionMapper = Verify.ArgumentNotNull(subscriptionMapper, "subscriptionMapper");
        }

        // -----------------------------------------------------------------------------
        [Route("")]
        [HttpGet]
        //[Authorize]
        public IEnumerable<ADEventService.DTO.Subscription> GetAll([FromUri] string filter = null)
        {
            var mdlSubColl = _subscriptionRepo.GetAll();
            var dtoSubColl = _subscriptionMapper.CreateDTOFromModel(mdlSubColl);

            return dtoSubColl;
        }

        // -----------------------------------------------------------------------------
        [Route("")]
        [HttpPost]
        //[Authorize]
        public ADEventService.DTO.Subscription Create([FromBody]ADEventService.DTO.CreateSubscriptionRequest dtoSubscriptionRequest)
        {
            try
            {
                var mdlSubscriptionRequest = _subscriptionMapper.CreateFromDTO(dtoSubscriptionRequest);
                var mdlNewSub = _subscriptionRepo.CreateSubscription(mdlSubscriptionRequest);
                var dtoNewSub = _subscriptionMapper.CreateDTOFromModel(mdlNewSub);

                return dtoNewSub;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
