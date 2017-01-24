namespace ADEventService.Models
{
    // ================================================================================
    public class CreateSubscriptionRequest : ICreateSubscriptionRequest
    {
        // -----------------------------------------------------------------------------
        public string Name { get; set; }

        // -----------------------------------------------------------------------------
        public string Description { get; set; }

        // -----------------------------------------------------------------------------
        public string Endpoint { get; set; }

        // -----------------------------------------------------------------------------
        public string ContactEmail { get; set; }
    }
}