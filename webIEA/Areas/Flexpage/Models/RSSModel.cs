using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using System.Collections.Specialized;

namespace Flexpage.Models
{
    public class RSSModel
    {
        public List<RSSModelFeed> Feeds { get; set; }

        public RSSModel(IFlexpageRepository repository)
        {
            this.Feeds = RssGrid(repository);
        }

        public static List<RSSModelFeed> RssGrid(IFlexpageRepository repository)
        {
            List<RssFeed> feeds = repository.GetEntityList<RssFeed>();
            List<RSSModelFeed> modelFeeds = new List<RSSModelFeed>();
            foreach(RssFeed feed in feeds)
            {
                RSSModelFeed mf = new RSSModelFeed();
                mf.ID = feed.ID;
                mf.BlockID = feed.BlockID;
                mf.IsArchived = feed.Archived;
                mf.PublicationDate = feed.PubDate;
                mf.ShortDescription = feed.ShortDescription;
                switch(feed.Block.BlockType.Name)
                {
                    case "CmsText":
                        {
                            var cms = repository.GetByBlockID<CmsText>(feed.BlockID);
                            if(cms != null)
                            {
                                var descr = cms.CmsTextLocalizations.SingleOrDefault(d => d.CmsTextID == cms.BlockID
                                    && d.Language.Code == Code.CMS.CmsSettings.GetCurrentOrDefaultLangCode());
                                if(descr != null)
                                {
                                    mf.Description = descr.FullText.ToString();
                                }
                            }
                            break;
                        }
                }
                modelFeeds.Add(mf);
            }
            return modelFeeds;
        }
    }

    public class RSSModelFeed
    {
        public int ID { get; set; }
        public DateTime PublicationDate { get; set; }
        public string ShortDescription { get; set; }
        public bool IsArchived { get; set; }
        public int BlockID { get; set; }
        public string NavigateUrl { get; set; }
        public string Description { get; set; }
    }
}