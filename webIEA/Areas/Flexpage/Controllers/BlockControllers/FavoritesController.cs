using Flexpage.Abstract;
using System.Web.Mvc;
using Flexpage.Abstract.DTO;

namespace Flexpage.Controllers.BlockControllers
{
    public class FavoritesController : Controller
    {
        private readonly IFavoritesBlockProvider _favoritesBlockProvider;

        public FavoritesController(IFavoritesBlockProvider favoritesBlockProvider)
        {
            _favoritesBlockProvider = favoritesBlockProvider;
        }
        
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult AddToFavorites(string url, string title)
        {
            title = string.IsNullOrEmpty(title) ? url : title;
            _favoritesBlockProvider.Add(new FavoriteItemViewModel() { Url = url, Name = title, Index = 1 });

            return Json("{\"success\": true}");
            
        }

        public ActionResult FavoritesList()
        {
            var model = _favoritesBlockProvider.Load();
            return PartialView("~/Areas/Flexpage/Views/Shared/EditorTemplates/Favorites/_FavoritesList.cshtml", model);
        }
    }
}