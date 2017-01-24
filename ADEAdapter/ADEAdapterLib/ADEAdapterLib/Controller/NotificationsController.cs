using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading;

using GK.AppCore.Utility;
using GK.AppCore.Error;

//using GK.AD.DTO;

using ADEAdapterLib.Configuration;
using ADEAdapterLib.Model;

namespace ADEAdapterLib.Controllers
{
    // ================================================================================
    [RoutePrefix(NotificationsController.RoutePrefix)]
    public class NotificationsController : ApiController
    {
        public const string RoutePrefix = "api/v1/notifications";

        readonly IADEAConfig _config;
        readonly IEngine _engine;
        readonly IHttpExceptionHelper _exceptionHelper;
        readonly IIncomingEventQueuePublisher _incomingPublisher;

        // -----------------------------------------------------------------------------
        public NotificationsController(IADEAConfig config, IEngine engine, IHttpExceptionHelper exceptionHelper, IIncomingEventQueuePublisher incomingPublisher)
        {
            _config = Verify.ArgumentNotNull(config, "config");
            _engine = Verify.ArgumentNotNull(engine, "engine");
            _exceptionHelper = Verify.ArgumentNotNull(exceptionHelper, "exceptionHelper");
            _incomingPublisher = Verify.ArgumentNotNull(incomingPublisher, "incomingPublisher");
        }

        // -----------------------------------------------------------------------------
        [Route("")]
        [HttpPost]
        //[Authorize]
        public HttpResponseMessage Notify([FromBody]string value)
        {
            try
            {
                // If engine IS OFF - return HTTP status 503
                if (_engine.IsEngineON == false)
                {
                    var s = "ADEAdapter is currently OFF. Retry message delivery later!";
                    throw _exceptionHelper.CreateHttpResponseException(HttpStatusCode.ServiceUnavailable, s);
                }

                return DoStuff(value);
            }

            catch (HttpResponseException)
            {
                throw;
            }

            catch (Exception ex)
            {
                throw _exceptionHelper.CreateHttpResponseException(ex);
            }
        }

        // -----------------------------------------------------------------------------
        [HttpGet]
        [Route("info")]
        //[Authorize]
        public HttpResponseMessage GetInfo()
        {
            var resp = Request.CreateResponse(HttpStatusCode.OK);
            resp.Content = new StringContent(_engine.GetEngineInfo());

            return resp;
        }

        // -----------------------------------------------------------------------------
        [HttpPost]
        [Route("on")]
        //[Authorize]
        public HttpResponseMessage Activate()
        {
            _engine.StartEngine();
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // -----------------------------------------------------------------------------
        [HttpPost]
        [Route("off")]
        //[Authorize]
        public HttpResponseMessage DeActivate()
        {
            _engine.StopEngine();
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // -----------------------------------------------------------------------------
        HttpResponseMessage DoStuff(string value)
        {
            GK.AD.DTO.ADEvent adevent = Serializer.DeserializeFromJson<GK.AD.DTO.ADEvent>(value);

            //ValidateRequest(adevent);

            // NB! It's the DTO object that is stored in incoming queue.
            byte[] message = Serializer.SerializeToByteArray(adevent);

            _incomingPublisher.PublishMessage(message);

            //Trace(adevent);

            return Request.CreateResponse(HttpStatusCode.Created);
        }

        // -----------------------------------------------------------------------------
        void ValidateRequest(GK.AD.DTO.ADEvent adevent)
        {
            string s = string.Format("EVENT RECEIVED IN CONTROLLER: [{0}]", adevent.ToString());

            // Simulate unrecoverable client side event error, a type of error the subscriber will not be able to recover from.
            // The event will NOT be resend by publisher, ie. on the ADEventService publisher side this client response will 
            // result in the event/message beiing move to the dead-letter bin.
            // In effekt the event is lost!
            if (adevent.Sender.What.Contains("EXT:ERROR:UNRECOVERABLE"))
            {
                string sErr = string.Format("EXT:ERROR:UNRECOVERABLE test response. Event {0} could NOT be processed due to bad things ... Event will be discarded serverside!", adevent.CorrID);
                if (_config.LogReceivedNotifications)
                {
                    _config.Logger.LogINFO(s);
                }

                // Translates to HTTP 500 or 404 error
                throw new ArgumentOutOfRangeException(sErr);
            }

            // Simulate client side event error, in witch the publisher should resend the event later.
            if (adevent.Sender.What.Contains("EXT:ERROR:RETRY"))
            {
                string sErr = string.Format("EXT:ERROR:RETRY test response.");
                if (_config.LogReceivedNotifications)
                {
                    _config.Logger.LogINFO(s);
                }

                // Translates to HTTP 503
                throw new RetryMessageException(sErr);
            }

            // Simulate client side time out. Expect publisher to resend event later (and in order).
            if (adevent.Sender.What.Contains("EXT:ERROR:TIMEOUT"))
            {
                string sErr = string.Format("EXT:ERROR:TIMEOUT test response. Event {0} processing TIMED OUT ... Event will be RESEND serverside!", adevent.CorrID);
                if (_config.LogReceivedNotifications)
                {
                    _config.Logger.LogINFO(s);
                }

                Thread.Sleep(60 * 1000);

                throw new TimeoutException(sErr);
            }

            // Simulate client WAKEUP. Semantics undefined atm
            if (adevent.Sender.What.Contains("EXT:WAKEUP"))
            {
                string sErr = string.Format("EXT:WAKEUP test response.");
                if (_config.LogReceivedNotifications)
                {
                    _config.Logger.LogINFO(s);
                }
            }

        }

        // -----------------------------------------------------------------------------
        void Trace(GK.AD.DTO.ADEvent adevent)
        {
            if (_config.LogReceivedNotifications)
            {
                _config.Logger.LogINFO(string.Format("EVENT RECEIVED ==> [{0}]", adevent.ToString()));
            }
        }

    }
}
