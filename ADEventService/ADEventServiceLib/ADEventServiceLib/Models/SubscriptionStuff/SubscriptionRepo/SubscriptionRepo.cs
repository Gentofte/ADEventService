using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Xml;
using System.Xml.Serialization;

using GK.AppCore.Utility;

using ADEventService.Configuration;

namespace ADEventService.Models
{
    // ================================================================================
    public class SubscriptionRepo : ISubscriptionRepo
    {
        const string _subscriptionCollectionStoreKey = @"Collection-032D2138-D9A6-4220-8942-54D96BB4F951";

        object _lock = new object();

        readonly IADESConfig _config;
        readonly INullSubscription _nullSubscription;

        Dictionary<Guid, ISubscription> _subscriptions = new Dictionary<Guid, ISubscription>();
        SubscriptionCollection _subColl = new SubscriptionCollection();

        // -----------------------------------------------------------------------------
        public SubscriptionRepo(IADESConfig config, INullSubscription nullSubscription)
        {
            _config = Verify.ArgumentNotNull(config, "config");
            _nullSubscription = Verify.ArgumentNotNull(nullSubscription, "nullSubscription");

            Init();
        }

        // -----------------------------------------------------------------------------
        public ISubscription CreateSubscription(ICreateSubscriptionRequest createSubscriptionRequest)
        {
            var key = Guid.NewGuid();

            var sub = new Subscription()
            {
                ID = key,
                Name = createSubscriptionRequest.Name,
                Description = createSubscriptionRequest.Description,
                Endpoint = createSubscriptionRequest.Endpoint,
                ContactEmail = createSubscriptionRequest.ContactEmail,

                Approved = false,
                Enabled = false,
                PublishON = false,

                Version = Guid.NewGuid()
            };

            Add(sub);

            return sub;
        }

        // -----------------------------------------------------------------------------
        public ISubscription Get(Guid id)
        {
            return _subscriptions[id];
        }

        // -----------------------------------------------------------------------------
        public void Update(ISubscription clientSub)
        {
            var sub = Get(clientSub.ID);

            sub.Name = clientSub.Name;
            sub.Description = clientSub.Description;
            sub.Endpoint = clientSub.Endpoint;
            sub.ContactEmail = clientSub.ContactEmail;

            // Other subcription values is left unmodifed

            VerifyAndUpdate(sub, sub.Version);
        }

        // -----------------------------------------------------------------------------
        public void Approve(ISubscription clientSub)
        {
            var sub = Get(clientSub.ID);

            sub.Approved = true;

            VerifyAndUpdate(sub, sub.Version);
        }

        // -----------------------------------------------------------------------------
        public void Enable(ISubscription clientSub, bool enabled)
        {
            var sub = Get(clientSub.ID);

            sub.Enabled = enabled;

            VerifyAndUpdate(sub, sub.Version);
        }

        // -----------------------------------------------------------------------------
        public void RemoveSubscription(ISubscription clientSub)
        {
            var sub = Get(clientSub.ID);
            VerifyAndUpdate(sub, sub.Version);

            _subscriptions.Remove(clientSub.ID);

            Checkpoint();
        }

        // -----------------------------------------------------------------------------
        public IEnumerable<ISubscription> GetAll()
        {
            return _subscriptions.Values;
        }

        // -----------------------------------------------------------------------------
        void VerifyAndUpdate(ISubscription updatedSub, Guid prevVersion)
        {
            if (updatedSub.Version != prevVersion)
            {
                throw new Exception("Concurrency failure, trying to update subscription");
            }

            updatedSub.Version = Guid.NewGuid();
            _subscriptions[updatedSub.ID] = updatedSub;

            Checkpoint();
        }

        // -----------------------------------------------------------------------------
        void Init()
        {
            try
            {
                Load(_config.SubscriptionsConfigFilename);
            }
            catch (FileNotFoundException)
            {
                _config.Logger.LogWRITE(
                    string.Format("Configuration file [{0}] was NOT found! A new one will be created!", _config.SubscriptionsConfigFilename)
                    );

                var initLP = CreateLoopbackSubscription();
                initLP.Name = initLP.Name;
                initLP.Description = initLP.Description;
                Add(initLP);

                //var initLP2 = CreateLoopbackSubscription();
                //initLP2.ID = Guid.NewGuid();
                //initLP2.Name = initLP.Name + "02";
                //initLP2.Description = initLP.Description + "02";
                //initLP2.Version = Guid.NewGuid();
                //Add(initLP2);

                //var initLP3 = CreateLoopbackSubscription();
                //initLP3.ID = Guid.NewGuid();
                //initLP3.Name = initLP.Name + "03";
                //initLP3.Description = initLP.Description + "03";
                //initLP3.Version = Guid.NewGuid();
                //Add(initLP3);

                Load(_config.SubscriptionsConfigFilename);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // -----------------------------------------------------------------------------
        //void LoadSubscriptions()
        //{
        //    if (_config.ConfigStore.Exists(_subscriptionCollectionStoreKey) == false)
        //    {
        //        Checkpoint();
        //    }

        //    _subscriptions = GK.AppCore.Utility.Serializer.DeSerializeFromByteArray<Dictionary<Guid, ISubscription>>(_config.ConfigStore.GetBlob(_subscriptionCollectionStoreKey));
        //}

        // -----------------------------------------------------------------------------
        void Add(ISubscription sub)
        {
            _subscriptions.Add(sub.ID, sub);
            Checkpoint();
        }

        // -----------------------------------------------------------------------------
        void Checkpoint()
        {
            byte[] ba = GK.AppCore.Utility.Serializer.SerializeToByteArray(_subscriptions);
            _config.ConfigStore.SetBlob(_subscriptionCollectionStoreKey, ba);

            Save();
        }

        // -----------------------------------------------------------------------------
        public void Save()
        {
            _subColl.Subscriptions.Clear();
            foreach (var item in _subscriptions.Values)
            {
                _subColl.Subscriptions.Add(item as Subscription);
            }

            lock (_lock)
            {
                var serializer = new XmlSerializer(typeof(SubscriptionCollection));
                using (var stream = new FileStream(_config.SubscriptionsConfigFilename, FileMode.Create))
                {
                    serializer.Serialize(stream, _subColl);
                    stream.Close();
                }
            }
        }

        // -----------------------------------------------------------------------------
        void Load(string path)
        {
            lock (_lock)
            {
                var serializer = new XmlSerializer(typeof(SubscriptionCollection));
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    _subColl = serializer.Deserialize(stream) as SubscriptionCollection;
                    stream.Close();
                }
            }

            _subscriptions.Clear();
            foreach (var item in _subColl.Subscriptions)
            {
                _subscriptions.Add(item.ID, item);
            }

            Checkpoint();
        }

        // -----------------------------------------------------------------------------
        ISubscription CreateLoopbackSubscription()
        {
            Subscription loopbackSubscription = new Subscription()
            {
                ID = _nullSubscription.ID,
                Name = _nullSubscription.Name,
                Description = _nullSubscription.Description,
                Endpoint = _nullSubscription.Endpoint,
                ContactEmail = _nullSubscription.ContactEmail,
                Approved = _nullSubscription.Approved,
                Enabled = _nullSubscription.Enabled,
                PublishON = _nullSubscription.Enabled,
                Version = _nullSubscription.Version
            };

            return loopbackSubscription;
        }
    }

    // ================================================================================
    [Serializable]
    public class SubscriptionCollection
    {
        public List<Subscription> Subscriptions = new List<Subscription>();
    }
}
