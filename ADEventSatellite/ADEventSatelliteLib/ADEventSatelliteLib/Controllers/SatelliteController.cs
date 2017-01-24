using ADEventSatellite.Configuration;
using ADEventSatellite.Model;
using GK.AppCore.Utility;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ADEventSatellite.Controllers
{
    // ================================================================================
    [RoutePrefix("api/v1/satellite")]
    public class SatelliteController : ApiController
    {
        readonly IADESLConfig _config;
        readonly IEngine _engine;

        // -----------------------------------------------------------------------------
        public SatelliteController(IADESLConfig config, IEngine engine)
        {
            _config = Verify.ArgumentNotNull(config, "config");
            _engine = Verify.ArgumentNotNull(engine, "engine");
        }

        // -----------------------------------------------------------------------------
        [HttpGet]
        [Route("info")]
        public HttpResponseMessage GetInfo()
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

        // -----------------------------------------------------------------------------
        [HttpPost]
        [Route("reset")]
        public HttpResponseMessage Reset()
        {
            _engine.Reset();
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        //// -----------------------------------------------------------------------------
        //[HttpPost]
        //[Route("initalize")]
        //public HttpResponseMessage Initalize()
        //{
        //    return Request.CreateResponse(HttpStatusCode.OK);
        //}
    }
}