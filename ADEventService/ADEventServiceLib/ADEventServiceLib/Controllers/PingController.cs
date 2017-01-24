using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.Threading;

using Newtonsoft.Json;

using GK.AppCore.Utility;

using ADEventService.Configuration;
using ADEventService.Models;

namespace ADEventService.Controller
{
    // ================================================================================
    [RoutePrefix(PingController.RoutePrefix)]
    public class PingController : ApiController
    {
        public const string RoutePrefix = "api/v1/ping";

        readonly IADESConfig _config;
        readonly IADESEngine _engine;

        // -----------------------------------------------------------------------------
        public PingController(IADESConfig config, IADESEngine engine)
        {
            _config = Verify.ArgumentNotNull(config, "config");
            _engine = Verify.ArgumentNotNull(engine, "engine");
        }

        // -----------------------------------------------------------------------------
        //[Authorize]
        [Route("")]
        [HttpGet]
        public HttpResponseMessage Get()
        {
            try
            {
                if (_engine.IsEngineON == false)
                {
                    var resp = Request.CreateResponse(HttpStatusCode.ServiceUnavailable);
                    resp.Headers.Add("Retry-After", "300");
                    return resp;
                }

                string user = "";
                user = string.IsNullOrWhiteSpace(user) ? "UNKNOWN" : user.ToUpperInvariant();

                PingResponse pingResponse = new PingResponse() { Referrer = Request.Headers.Referrer.AbsoluteUri, User = user, When=DateTime.Now };

                string s = JsonConvert.SerializeObject(pingResponse);

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(s, Encoding.UTF8, "application/json");

                _config.Logger.LogINFO(
                    string.Format("Inside PingController.Get() : [{0}]",
                    response.Content.ReadAsStringAsync().Result)
                    );

                return response;
            }
            catch (System.Exception ex)
            {
                string s = ex.Message;

                var response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ReasonPhrase = ex.Message,
                    Content = new StringContent(ex.Message)
                };
                throw new HttpResponseException(response);
            }
        }
    }

    // ================================================================================
    public class PingResponse
    {
        public string Referrer { get; set; }
        public string User { get; set; }
        public DateTime When { get; set; }
    }
}
