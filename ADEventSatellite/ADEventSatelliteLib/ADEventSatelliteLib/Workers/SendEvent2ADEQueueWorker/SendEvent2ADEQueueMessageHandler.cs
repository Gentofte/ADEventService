using ADEventSatellite.Configuration;
using ADEventSatellite.DTO;
using GK.AD.Events;
using GK.AppCore.Error;
using GK.AppCore.Http;
using GK.AppCore.Queues;
using GK.AppCore.Utility;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ADEventSatellite.Workers
{
    // ================================================================================
    public class SendEvent2ADEQueueMessageHandler : IMessageHandler
    {
        readonly IADESLConfig _config;
        readonly IADEventFactory _adEventFactory;
        readonly IHttpHelperFactory _httpClientFatory;
        readonly ICircuitBreaker _circuitBreaker;

        string _serviceHost = "";

        // -----------------------------------------------------------------------------
        public SendEvent2ADEQueueMessageHandler(IADESLConfig config, IADEventFactory adEventFactory, IHttpHelperFactory httpClientFatory, ICircuitBreakerFactory circuitBreakerFactory)
        {
            _config = Verify.ArgumentNotNull(config, "config");
            _adEventFactory = Verify.ArgumentNotNull(adEventFactory, "adEventFactory");
            _httpClientFatory = Verify.ArgumentNotNull(httpClientFatory, "httpClientFatory");

            var cbFactory = Verify.ArgumentNotNull(circuitBreakerFactory, "circuitBreakerFactory");
            _circuitBreaker = cbFactory.CreateCircuitBreaker(20);

            _serviceHost = string.Format("http://{0}", UriHelper.NormalizeHost(_config.ADEventServiceURL));
        }

        // -----------------------------------------------------------------------------
        public event EventHandler<MessageDoneEventArgs> MessageDone
        {
            add { }
            remove { }
        }

        // -----------------------------------------------------------------------------
        public void Init()
        {
        }

        // -----------------------------------------------------------------------------
        public void CleanupOnExit()
        {
        }

        // -----------------------------------------------------------------------------
        public void HandleMessage(byte[] messageBody)
        {
            HttpResponseMessage response = null;
            var eventAtString = "";

            try
            {
                var adEvent = _adEventFactory.DeSerialize(messageBody);
                eventAtString = adEvent.ToString();

                if (_config.DropADEEvents)
                {
                    _config.Logger.LogINFO(string.Format("EVENT DROPPED: [{0}] <== DropADEEvents setting is ENABLED", eventAtString));
                    return;
                }

                _circuitBreaker.ExecuteAction(() =>
                {
                    using (HttpClient client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true }))
                    {
                        // Add an Accept header for JSON format.
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        IHttpClientHelper clientHelper = _httpClientFatory.CreateHttpClientHelper(client, _serviceHost, adEvent.CorrID);
                        response = clientHelper.PostAsJson(
                            new EventPackage() { SattelliteID = _config.ApplicationID, ADEventAsB64 = Convert.ToBase64String(messageBody), CallBackURI = _config.ServiceBaseURL }
                            );
                    }
                });

                Trace(adEvent, _serviceHost);
            }

            catch (HttpRetryException ex)
            {
                throw new RetryMessageException(string.Format("[{0} <== {1}] ", _serviceHost, eventAtString), ex);
            }

            catch (CircuitBreakerOpenException)
            {
                throw;
            }

            catch (Exception ex)
            {
                throw new FailMessageException(string.Format("[{0} <== {1}] ", _serviceHost, eventAtString), ex);
            }
        }

        // -----------------------------------------------------------------------------
        public void HandleMessage(object messageBody) { throw new NotImplementedException(); }

        // -----------------------------------------------------------------------------
        void Trace(ADEvent adEvent, string requestUri)
        {
            if (_config.LogEventsTransmitted)
            {
                _config.Logger.LogINFO(string.Format("EVENT DELIVERED: [{0}] ==> [{1}]", adEvent.ToString(), requestUri));
            }
        }
    }
}