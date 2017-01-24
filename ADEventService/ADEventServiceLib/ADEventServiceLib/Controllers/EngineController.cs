using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Http;

using System.Web.Http;

using GK.AppCore.Utility;

using ADEventService.Configuration;
using ADEventService.Models;

namespace ADEventService.Controllers
{
    // ================================================================================
    [RoutePrefix(EngineController.RoutePrefix)]
    public class EngineController : ApiController
    {
        public const string RoutePrefix = "api/v1/engine";

        readonly IADESConfig _config;
        readonly IADESEngine _engine;

        // -----------------------------------------------------------------------------
        public EngineController(IADESConfig config, IADESEngine engine)
        {
            _config = Verify.ArgumentNotNull(config, "config");
            _engine = Verify.ArgumentNotNull(engine, "engine");
        }

        // -----------------------------------------------------------------------------
        [HttpGet]
        [Route("info")]
        public HttpResponseMessage GetState()
        {
            var resp = Request.CreateResponse(HttpStatusCode.OK);
            resp.Content = new StringContent("OK");
            return resp;
        }

        // -----------------------------------------------------------------------------
        [HttpPost]
        [Route("on")]
        public HttpResponseMessage Activate()
        {
            _engine.StartEngine();
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // -----------------------------------------------------------------------------
        [HttpPost]
        [Route("off")]
        public HttpResponseMessage DeActivate()
        {
            _engine.StopEngine();
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
