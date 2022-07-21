using Flexpage.Abstract;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;

namespace Flexpage.Models
{
    public class FavoritesListModel : BlockModel
    {
        public FavoritesListModel(IFlexpageSettings settings, IFlexpage flexpage) : base(settings, flexpage)
        {
        }

        public override object Apply(IFlexpageRepository repository, params object[] args)
        {
            Block favoriteList = repository.CreateNewFavoritesList();
            if (BlocklistID > -1)
                repository.AddBlockToBlockList(favoriteList, BlocklistID, BlockAfter);
            repository.ApplyChanges();
            return null;
        }


        public override void Load(IFlexpageRepository repository, BlockCommandModel proto, string title = "", bool needToLoadContent = true)
        {
            base.Load(repository, proto, title, needToLoadContent);
            ID = proto.ID;
        }

    }
}