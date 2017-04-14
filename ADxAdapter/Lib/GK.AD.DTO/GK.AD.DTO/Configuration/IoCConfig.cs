
using Microsoft.Practices.Unity;

namespace GK.AD.DTO.Configuration
{
    // ================================================================================
    public class IoCConfig
    {
        // -----------------------------------------------------------------------------
        public void ConfigureIoCStuff(IUnityContainer container)
        {
            container.RegisterType<IWhoWhatWhen, WhoWhatWhen>();

            container.RegisterType<IUser, User>();
            container.RegisterType<IGroup, Group>();
            container.RegisterType<IOU, OU>();

            container.RegisterType<IADEvent, ADEvent>();
        }
    }
}
