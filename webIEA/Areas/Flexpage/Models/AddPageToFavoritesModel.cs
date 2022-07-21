using Flexpage.Abstract;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using System.Web;

namespace Flexpage.Models
{
    public class AddPageToFavoritesModel : BlockModel
    {
        public AddPageToFavoritesModel(IFlexpageSettings settings, IFlexpage flexpage) : base(settings, flexpage)
        {
        }

        public override object Apply(IFlexpageRepository repository, params object[] args)
        {
            Block addToFavorite = repository.CreateNewAddToFavorites();
            if (BlocklistID > -1)
                repository.AddBlockToBlockList(addToFavorite, BlocklistID, BlockAfter);
            repository.ApplyChanges();
            return null;
        }


        public override void Load(IFlexpageRepository repository, BlockCommandModel proto, string title = "", bool needToLoadContent = true)
        {
            base.Load(repository, proto, title, needToLoadContent);
            ID = proto.ID;
            
            Visible = repository.GetBlockVisibility(proto.ID) && HttpContext.Current != null && HttpContext.Current.User.Identity.IsAuthenticated;
        }
    }
}