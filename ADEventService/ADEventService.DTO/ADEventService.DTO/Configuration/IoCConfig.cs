using Microsoft.Practices.Unity;

namespace ADEventService.DTO.Configuration
{
    // ================================================================================
    public class IoCConfig
    {
        // -----------------------------------------------------------------------------
        public void ConfigureIoCStuff(IUnityContainer container)
        {
            container.RegisterType<ISubscription, Subscription>();
            container.RegisterType<ICreateSubscriptionRequest, CreateSubscriptionRequest>();
        }
    }
}
