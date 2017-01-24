using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading;

using GK.AppCore.Utility;
using GK.AppCore.Error;

using GK.AD.DTO;

using ADEventService.Configuration;
using ADEventService.Models;

namespace ADEventService.Controllers
{
    // ================================================================================
    /// <summary>
    /// Loopback controller. Receives ADEvent notifications the same way real adapters do. Used
    /// For internal heatbeat-like monitoring of ADEventService operations, and as skeleton 
    /// for error handling on adapter side.
    /// </summary>
    [RoutePrefix(NotificationsController.RoutePrefix)]
    public class NotificationsController : ApiController
    {
        public const string RoutePrefix = "api/v1/notifications";

        readonly IADESConfig _config;
        readonly IADESEngine _engine;
        readonly IHttpExceptionHelper _exceptionHelper;

        // -----------------------------------------------------------------------------
        public NotificationsController(IADESConfig config, IADESEngine engine, IHttpExceptionHelper exceptionHelper)
        {
            _config = Verify.ArgumentNotNull(config, "config");
            _engine = Verify.ArgumentNotNull(engine, "engine");
            _exceptionHelper = Verify.ArgumentNotNull(exceptionHelper, "exceptionHelper");
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
                    var s = "ADEventService LOOPBACK ADAPTER is currently OFF. Retry message delivery later!";
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
        HttpResponseMessage DoStuff(string value)
        {
            ADEvent dtoAde = GK.AD.DTO.Serializer.DeserializeFromJson<ADEvent>(value);

            string s = string.Format("AD event POSTED on LOOPBACK ADAPTER : [{0}]", dtoAde.ToString());

            // Simulate unrecoverable client side event error, a type of error the subscriber will not be able to recover from.
            // The event will NOT be resend by publisher, ie. on the ADEventService publisher side this client response will 
            // result in the event/message beiing move to the dead-letter bin.
            // In effekt the event is lost!
            if (dtoAde.Sender.What.Contains("EXT:ERROR:UNRECOVERABLE"))
            {
                string sErr = string.Format("EXT:ERROR:UNRECOVERABLE test response. Event {0} could NOT be processed due to bad things ... Event will be discarded serverside!", dtoAde.CorrID);
                if (_config.LogNULLNotifications)
                {
                    _config.Logger.LogINFO(s);
                }
                throw new ArgumentOutOfRangeException(sErr);
            }

            // Simulate client side event error, of a type in witch the subscriber should resend the event later.
            if (dtoAde.Sender.What.Contains("EXT:ERROR:RETRY"))
            {
                string sErr = string.Format("EXT:ERROR:RETRY test response.");
                if (_config.LogNULLNotifications)
                {
                    _config.Logger.LogINFO(s);
                }
                throw new RetryMessageException(sErr);
            }

            // Simulate client side time out. Expect publisher to resend event later (and in order).
            if (dtoAde.Sender.What.Contains("EXT:ERROR:TIMEOUT"))
            {
                string sErr = string.Format("EXT:ERROR:TIMEOUT test response. Event {0} processing TIMED OUT ... Event will be RESEND serverside!", dtoAde.CorrID);
                if (_config.LogNULLNotifications)
                {
                    _config.Logger.LogINFO(s);
                }

                Thread.Sleep(60 * 1000);

                throw new TimeoutException(sErr);
            }

            // Simulate client WAKEUP. Semantics undefined atm
            if (dtoAde.Sender.What.Contains("EXT:WAKEUP"))
            {
                string sErr = string.Format("EXT:WAKEUP test response.");
                if (_config.LogNULLNotifications)
                {
                    _config.Logger.LogINFO(s);
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }

            // Do something NORMAL with the AD object attached to the event ...
            // ...

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
