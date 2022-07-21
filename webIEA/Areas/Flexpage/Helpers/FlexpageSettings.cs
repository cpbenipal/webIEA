using Flexpage.Abstract;
using Pluritech.Services;
using System;
using System.Web.Mvc;

namespace Flexpage.Helpers
{
    public class FlexpageSettings
    {
        private FlexpageSettings() { }
        private static IFlexpageSettings _instance;
        [Obsolete("The static instance of the settings should be removed. Use IFlexpageSettings reference instead.")]
        public static IFlexpageSettings Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new Flexpage.Service.FlexpageSettings(new Pluritech.AppSettingsProvider.AppSettings(),
                        DependencyResolver.Current.GetService<Business.Abstract.IRolesProcessor>(),
                        DependencyResolver.Current.GetService<IPageAccessProvider>());
                }
                return _instance;
            }
        }
    }
}