using Pluritech.Settings.Abstract;
using System.Web.Mvc;

namespace Flexpage.Controllers
{
    public class SettingsController : Controller
    {
        private readonly ISettings _settings;
        public SettingsController(ISettings settings)
        {
            _settings = settings;
        }
        public string GetScriptPath()
        {
            if (_settings.GetBoolean("Web:Debug") == true)
            {
                return _settings.GetString("Web:bundleEventManagerPathDedug");
            }
            return _settings.GetString("Web:bundleEventManagerPath");
        }
    }
}