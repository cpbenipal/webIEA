using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using Flexpage.Code.Helpers;
using Flexpage.Domain.Enum;

namespace Flexpage.Models
{

    public class VideoModel : MediaModel
    {
        public VideoModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) 
            : base(settings, flexpage)
        {
            MediaType = new MediaType() { Name = MediaTypeName.Video.GetDisplay() };
        }
        public VideoModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage, object source, params object[] args) 
            : base(settings, flexpage, source,  args)
        {
            MediaType = new MediaType() { Name = MediaTypeName.Video.GetDisplay() };
        }
        
        public override object Apply(IFlexpageRepository repository, params object[] args)
        {
            Media block = repository.GetByBlockID<Media>(ID);
            if (block == null)
            {
                block = repository.GetByAlias<Media>(Alias);

                if (block == null)
                {
                    if (Alias == repository.CreateNewAlias)
                    {
                        Alias = null;
                    }
                    block = repository.CreateNewMedia(MediaTypeName.Video.GetDisplay(), alias: Alias);
                    repository.AddBlockToBlockList(block.Block, BlocklistID, BlockAfter);
                }
            }

            base.Apply(repository, block.Block);

            Apply(block, repository);

            repository.ApplyChanges();

            return null;
        }
    }
}