using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Http;

using System.Web.Http;

using GK.AppCore.Error;
using GK.AppCore.Utility;
using GK.AppCore.Http;

using GK.AD.Events;

using ADEventSatellite.DTO;

using ADEventService.Configuration;
using ADEventService.Models;

namespace ADEventService.Controllers
{
    // ================================================================================
    [RoutePrefix(RawEventController.RoutePrefix)]
    public class RawEventController : ApiController
    {
        public const string RoutePrefix = "api/v1/rawevent";

        readonly IADESConfig _config;
        readonly IADESEngine _engine;
        readonly IRawEventQueuePublisher _raweventQueue;
        readonly IHttpExceptionHelper _exceptionHelper;

        // -----------------------------------------------------------------------------
        public RawEventController(IADESConfig config, IADESEngine engine, IRawEventQueuePublisher raweventQueue, IHttpExceptionHelper exceptionHelper)
        {
            _config = Verify.ArgumentNotNull(config, "config");
            _engine = Verify.ArgumentNotNull(engine, "engine");
            _raweventQueue = Verify.ArgumentNotNull(raweventQueue, "raweventQueue");

            _exceptionHelper = Verify.ArgumentNotNull(exceptionHelper, "httpHelperFactory");
        }

        // -----------------------------------------------------------------------------
        [HttpPost]
        [Route("")]
        public HttpResponseMessage PostEventPackage([FromBody] EventPackage eventPackage)
        {
            try
            {
                // TODO Verify satellite ID. 20160102/SDE
                //if (1 == 0)
                //{
                //    var s = "Specified satellite (ID) is UNKNOWN!";
                //    throw _exceptionHelper.CreateHttpResponseException(HttpStatusCode.Forbidden, s);
                //}

                // If engine IS OFF - return HTTP status 503
                if (_engine.IsEngineON == false)
                {
                    var s = "ADEventService is currently OFF. Retry RAW message delivery (POST) later!";
                    throw _exceptionHelper.CreateHttpResponseException(HttpStatusCode.ServiceUnavailable, s);
                }

                byte[] message = GK.AppCore.Utility.Serializer.SerializeToByteArray(eventPackage);

                // Persist raw event ...
                _raweventQueue.PublishMessage(message);

                Trace(eventPackage);

                return Request.CreateResponse(HttpStatusCode.Created);
            }

            catch (HttpResponseException ex)
            {
                throw ex;
            }

            catch (Exception ex)
            {
                throw _exceptionHelper.CreateHttpResponseException(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        // -----------------------------------------------------------------------------
        void Trace(EventPackage eventPackage)
        {
            if (_config.LogRawEventsReceived)
            {
                var rawEvent = Convert.FromBase64String(eventPackage.ADEventAsB64);

                var adEvent = Serializer.DeSerializeFromByteArray<ADEvent>(rawEvent);
                var s = string.Format("RAW EVENT RECEIVED: [{0}] from [{1}]", adEvent.ToString(), eventPackage.CallBackURI);

                _config.Logger.LogINFO(s);
            }
        }

    }
}
