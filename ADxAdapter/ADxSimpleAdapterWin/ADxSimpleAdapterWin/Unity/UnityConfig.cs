using Microsoft.Practices.Unity;
using System;

namespace ADxSimpleAdapterWin.Unity
{
    // ================================================================================
    public class UnityConfig
    {
        static IUnityContainer _appCoreExternalContainer = null;

        #region Unity Container
        // -----------------------------------------------------------------------------
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        // -----------------------------------------------------------------------------
        public static IUnityContainer GetConfiguredContainer()
        {
            return (_appCoreExternalContainer == null) ? container.Value : _appCoreExternalContainer;
        }

        // -----------------------------------------------------------------------------
        public static void SetConfiguredContainer(IUnityContainer appCoreExternalContainer)
        {
            _appCoreExternalContainer = appCoreExternalContainer;
        }
        #endregion

        // -----------------------------------------------------------------------------
        public static void RegisterTypes(IUnityContainer container)
        {
        }
    }
}
