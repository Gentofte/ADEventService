using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Web.Http;
using System.Net;

using GK.AppCore.Queues;
using GK.AppCore.Utility;
using GK.AppCore.Http;
using GK.AppCore.Error;

using GK.AD;
using GK.AD.Events;
using GK.AD.MAP;

using ADEventService.Configuration;
using ADEventService.Models;

namespace ADEventService.Workers
{
    // ================================================================================
    public class SubscribtionEventQueueMessageHandler : IMessageHandler
    {
        readonly IADESConfig _config;
        readonly IHttpHelperFactory _httpClientFatory;
        readonly ISubscription _subscription;
        readonly IADEventMapper _adEventMapper;
        readonly ICircuitBreaker _circuitBreaker;

        string _serviceHost = "";

        // -----------------------------------------------------------------------------
        public SubscribtionEventQueueMessageHandler(IADESConfig config, ISubscription subscription, IADEventMapper adEventMapper, IHttpHelperFactory httpClientFatory, ICircuitBreakerFactory circuitBreakerFactory)
        {
            _config = GK.AppCore.Utility.Verify.ArgumentNotNull(config, "config");
            _subscription = GK.AppCore.Utility.Verify.ArgumentNotNull(subscription, "subscription");
            _adEventMapper = GK.AppCore.Utility.Verify.ArgumentNotNull(adEventMapper, "adEventMapper");

            _httpClientFatory = GK.AppCore.Utility.Verify.ArgumentNotNull(httpClientFatory, "httpClientFatory");

            var cbFactory = Verify.ArgumentNotNull(circuitBreakerFactory, "circuitBreakerFactory");
            _circuitBreaker = cbFactory.CreateCircuitBreaker(20);

            _serviceHost = string.Format("http://{0}", UriHelper.NormalizeHost(_subscription.Endpoint));
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
                // Get ADEvent from internal (GK.AD.Events.ADEvent) representation
                var adEvent = GK.AppCore.Utility.Serializer.DeSerializeFromByteArray<ADEvent>(messageBody);
                eventAtString = adEvent.ToString();

                // Convert event from internal rep. to DTO format ...
                var dtoEvent = _adEventMapper.CreateADEvent(adEvent);

                // ... pack DTO event into JSON  ...
                var dtoEventAsjson = GK.AD.DTO.Serializer.SerializeToJson(dtoEvent);

                _circuitBreaker.ExecuteAction(() =>
                {
                    using (HttpClient client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true }))
                    {
                        // Add an Accept header for JSON format.
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        IHttpClientHelper clientHelper = _httpClientFatory.CreateHttpClientHelper(client, _serviceHost, adEvent.CorrID);
                        response = clientHelper.PostAsJson(dtoEventAsjson);
                    }
                });

                Trace(adEvent, _serviceHost);
            }

            catch (HttpRetryException ex)
            {
                throw new RetryMessageException(string.Format("SUBSCRIBER UNAVAILABLE ATM. DELIVERY WILL BE RETRIED [{0} <== {1}] ", _serviceHost, eventAtString), ex);
            }

            catch (CircuitBreakerOpenException)
            {
                throw;
            }

            catch (Exception ex)
            {
                throw new FailMessageException(string.Format("SUBSCRIBER REJECTED EVENT. EVENT IS DEADLETTERED [{0} <== {1}] ", _serviceHost, eventAtString), ex);
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