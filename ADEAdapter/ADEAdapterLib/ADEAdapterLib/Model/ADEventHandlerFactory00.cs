using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GK.AppCore.Utility;

using ADEAdapterLib.Configuration;

namespace ADEAdapterLib.Model
{
    // ================================================================================
    /// <summary>
    /// Default / sample event handler factory class.
    /// This class should be implementeted in the assembly that contains the adapter logic
    /// at the receiving end of solution for whitch this provision integration is set up.
    /// Register this class in IoC container like ...
    /// ... container.RegisterType<IADEventHandlerFactory, ADEventHandlerFactory00>(new ContainerControlledLifetimeManager());
    /// ... and it will hereafter be used for building events as they are received from ADEventService
    /// </summary>
    public class ADEventHandlerFactory00 : IADEventHandlerFactory
    {
        readonly IADEAConfig _config;

        // -----------------------------------------------------------------------------
        public ADEventHandlerFactory00(IADEAConfig config)
        {
            _config = Verify.ArgumentNotNull(config, "config");
        }

        // -----------------------------------------------------------------------------
        /// <summary>
        /// Creates a new event handler for the current AD event in question.
        /// This method takes care of dropping irrelevant events, building different types of eventhandler, etc.
        /// </summary>
        /// <param name="adEvent"></param>
        /// <returns></returns>
        public IADEventHandler CreateEventHandler(GK.AD.DTO.ADEvent adEvent)
        {
            switch (adEvent.ADObject.objectClass)
            {
                case GK.AD.DTO.ObjectClass.user: return new ADEventHandler00(_config, adEvent);

                case GK.AD.DTO.ObjectClass.organizationalUnit: return new ADEventHandler00(_config, adEvent);

                case GK.AD.DTO.ObjectClass.group: break;
            }

            return null;
        }
    }
}
