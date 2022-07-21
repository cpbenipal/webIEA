using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using Flexpage.Code.Helpers;
using Flexpage.Domain.Enum;

namespace Flexpage.Models
{

    public class AudioModel : MediaModel
    {
        public AudioModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpageProcessor) : base(settings, flexpageProcessor)
        {
            MediaType = new MediaType() { Name = MediaTypeName.Audio.GetDisplay() };
        }

        public AudioModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpageProcessor, object source, params object[] args) 
            : base(settings, flexpageProcessor, source, args)
        {
            MediaType = new MediaType() { Name = MediaTypeName.Audio.GetDisplay() };
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
                    
                    block = repository.CreateNewMedia(MediaTypeName.Audio.GetDisplay(), alias: Alias);
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