using System;

namespace ADEventService.Models
{
    // ================================================================================
    [Serializable]
    public class Subscription : ISubscription
    {
        public Guid ID { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Endpoint { get; set; }
        public string ContactEmail { get; set; }

        public bool Approved { get; set; }
        public bool Enabled { get; set; }
        public bool PublishON { get; set; }

        public Guid Version { get; set; }

        // -----------------------------------------------------------------------------
        public string ToString(bool fullRecord = false)
        {
            var activeString = Active() ? true.ToString().ToUpperInvariant() : false.ToString().ToLowerInvariant();

            if (fullRecord)
                return string.Format("{0}/{4} ({1}), contact=[{5}], approved=[{2}], enabled=[{6}], Publish-ON=[{7}], endpoint=[{3}]",
                    Name,
                    ID,
                    Approved,
                    Endpoint,
                    Description,
                    ContactEmail,
                    Enabled,
                    PublishON
                    );

            return string.Format("{0} ({1}): Active=[{2}], Endpoint=[{3}]",
                Name,
                ID,
                activeString,
                Endpoint
                );
        }

        // -----------------------------------------------------------------------------
        bool Active()
        {
            return (Approved && Enabled);
        }
    }
}
