using System;
using System.Linq;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using Flexpage.Code.Helpers;
using Flexpage.Domain.Enum;

namespace Flexpage.Models
{
    public class VideoPlaylistModel : MediaPlaylistModel
    {
        public VideoPlaylistModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) :base(settings, flexpage)
        {
            MediaType = new MediaType() { Name = MediaTypeName.VideoPlaylist.GetDisplay() };
            AddItem();
        }
        
        public override object Apply(IFlexpageRepository repository, params object[] args)
        {
            var block = repository.GetByBlockID<MediaPlaylist>(ID);
            if (block == null)
            {
                if (Alias == repository.CreateNewAlias)
                {
                    Alias = null;
                }
                block = repository.CreateNewMediaPlaylist(MediaTypeName.VideoPlaylist.GetDisplay(), alias: Alias);
                repository.AddBlockToBlockList(block.Block, BlocklistID, BlockAfter);
            }

            base.Apply(repository, block.Block);

            ApplyPlaylist(block, repository);

            repository.ApplyChanges();

            return null;
        }


        public override void ApplyPlaylist(MediaPlaylist playlist, IFlexpageRepository repository)
        {
            base.ApplyPlaylist(playlist, repository);

            foreach (var i in Items)
            {
                var v = playlist.Media.FirstOrDefault(e => e.BlockID == i.ID);
                if (v == null)
                {
                    v = repository.CreateNewMedia(MediaTypeName.Video.GetDisplay(), playlist);
                }

                if (v != null)
                {
                    i.Apply(v, repository);
                }
            }

            // playlist.Info = repository.GetByBlockID<CmsText>(InfoID);

            if (playlist.Info == null)
            {
                playlist.Info = repository.CreateNewCmsText(Visible, CssClass);
                Info.Apply(repository, playlist.Info);
                this.InfoID = playlist.InfoID == null ? -1 : playlist.InfoID.Value;
            }
            else
            {
                Info.Apply(repository, playlist.Info);
            }
        }
        public override void AddItem()
        {
            CurrentItem = new VideoModel(_settings, _flexpageProcessor)
            {
                Index = Items.Count,
                ID = -1,
                Title = new LocalizedStringModel(_settings, _flexpageProcessor, String.Format("Video {0}", Items.Count)),
                Description = new LocalizedStringModel(_settings, _flexpageProcessor, "Video description"),
            };
            Items.Add(CurrentItem);
        }
    }
}