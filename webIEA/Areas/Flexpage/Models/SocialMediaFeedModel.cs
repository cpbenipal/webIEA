using System.Linq;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities.SocialMediaFeed;
using Flexpage.Models;
using FlexPage2.Areas.Flexpage.Models.Enums;
using SocialMediaType = FlexPage2.Areas.Flexpage.Models.Enums.SocialMediaType;
using SMFType = FlexPage2.Areas.Flexpage.Models.Enums.SMFType;


namespace Flexpage.Models
{
    public class SocialMediaFeedModel : BlockModel
    {
        public SocialMediaFeedModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage) { }
        public SocialMediaType Media { get; set; }

        public SMFType SMFType { get; set; }

        public bool IsShowPicture { get; set; } = true;

        public string TwitterUrl { get; set; }

        public string Account { get; set; }
        
        /// <summary>
        /// Gets or sets the uid.
        /// </summary>
        /// <value>
        /// The uid. Using for facebook
        /// </value>
        public long? FacebookUid { get; set; }

        /// <summary>
        /// Gets or sets the login.
        /// </summary>
        /// <value>
        /// The login. Using for Twitter.
        /// </value>
        public string TwitterLogin { get; set; }

        public string Header { get; set; }

        public int PostsNumber { get; set; }

        public override void Assign(object source, params object[] args)
        {
            if (source is SocialMediaFeed)
            {
                SocialMediaFeed socialMediaFeed = source as SocialMediaFeed;

                if (socialMediaFeed.Block != null)
                    base.Assign(socialMediaFeed.Block, args);

                assign(socialMediaFeed);
            }
        }

        protected void assign(SocialMediaFeed source)
        {
            base.Assign(source.Block);
            this.ID = source.BlockID;
            this.Alias = source.Block.Alias;
            this.Media = (SocialMediaType)source.Media;
            this.Account = source.Account;
            this.FacebookUid = source.FacebookUid;
            this.TwitterLogin = source.TwitterLogin;
            this.Header = source.Header;
            this.PostsNumber = source.PostsNumber;
            this.CssClass = "";
            this.SMFType = (SMFType)source.SMFType;
            this.TwitterUrl = source.TwitterUrl;
            this.IsShowPicture = source.IsShowPicture;
        }

        private void AssignDefaultValues(IFlexpageRepository repository, string predefinedAlias)
        {
            this.ID = -1;
            this.Alias = string.IsNullOrWhiteSpace(predefinedAlias) ? repository.CreateNewAlias : predefinedAlias;
        }

        public void Load(IFlexpageRepository repository, BlockCommandModel proto, SocialMediaType mediaType, bool needToLoadContent = true)
        {
            this.Media = mediaType;
            this.Load(repository, proto,"", needToLoadContent);
        }

        public override void Load(IFlexpageRepository repository, BlockCommandModel proto,string title="", bool needToLoadContent = true)
        {
            
            base.Load(repository, proto, title, needToLoadContent);
            this.Alias = proto.BlockAlias;
            ID = proto.ID;

            SocialMediaFeed item = null;

            if (proto.ID == BlockModel.NewStaticBlockID && !string.IsNullOrEmpty(proto.BlockAlias))
            {
                item = repository.GetByAlias<SocialMediaFeed>(Alias);
            }
            else
            {
                item = repository.GetByBlockID<SocialMediaFeed>(ID);
            }

            if (item == null)
            {
                AssignDefaultValues(repository, proto.BlockAlias);                
            }
            else
            {
                assign(item);
            }
        }

        /// <summary>
        /// Applies changes made to view model to repository
        /// </summary>
        /// <param name="repository">Repository</param>
        public override object Apply(IFlexpageRepository repository, params object[] args)
        {
            SocialMediaFeed socialMediaFeed = repository.GetByBlockID<SocialMediaFeed>(ID);
            if (socialMediaFeed == null || Alias == repository.CreateNewAlias) 
            {
                if (Alias == repository.CreateNewAlias)
                {
                    Alias = null;
                }
                socialMediaFeed = repository.CreateSocialMediaFeed(Visible, CssClass, Alias);
                apply(socialMediaFeed);
                repository.AddBlockToBlockList(socialMediaFeed.Block, BlocklistID, BlockAfter);
            }
            else
            {
                apply(socialMediaFeed);
            }
            repository.ApplyChanges();

            return socialMediaFeed;
        }

        protected void apply(SocialMediaFeed target)
        {
            target.Block.Alias = target.Block.Alias;
            //target.Block.Visible = Visible;
            target.Block.CssClass = CssClass;
            target.Media = (Domain.Entities.SocialMediaFeed.SocialMediaType)this.Media;
            target.Account = this.Account;
            target.Header = this.Header;
            target.FacebookUid = this.FacebookUid;
            target.TwitterLogin = this.TwitterLogin;
            target.PostsNumber = this.PostsNumber;
            target.SMFType = (Domain.Entities.SocialMediaFeed.SMFType)this.SMFType;
            target.TwitterUrl = this.TwitterUrl;
            target.IsShowPicture = this.IsShowPicture;
        }
    }
}