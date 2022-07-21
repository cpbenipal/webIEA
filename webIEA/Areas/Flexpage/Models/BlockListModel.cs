using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;

using Flexpage.Abstract.DTO;

namespace Flexpage.Models
{
    public class BlockListModel : BlockModel
    {
        public IList<IBlockModel> Blocks { get; set; }
        private int websiteID { get; set; }

        public BlockListModel(Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpageProcessor) : base(settings, flexpageProcessor)
        {
            Blocks = new List<IBlockModel>();
            websiteID = settings.WebsiteID;
        }

        public override void Load(IFlexpageRepository repository, BlockCommandModel proto, string title="", bool needToLoadContent = true)
        {
            if(proto.BlockAlias != null)
            {
                string alias = proto.BlockAlias.ToLower();
                Page page = repository.GetPageByName(alias, websiteID);

                if (page == null)
                {
                    page = repository.CreateNewPage(alias, websiteID);
                    repository.ApplyChanges();
                }

                ID = BlocklistID = page.Block.BlockListContainer.FirstOrDefault().BlockListID;
                needToLoadContent &= (page.IsPublished || this.AdminMode);
            }
            if(proto.BlocklistID != 0)
                ID = BlocklistID = proto.BlocklistID;

            if (needToLoadContent && BlocklistID != 0)
            {
                fillBlocks(repository, BlocklistID);

            }
        }

        public virtual void LoadContent(IFlexpageRepository repository)
        {
            fillBlocks(repository, ID, false );
        }

        protected void fillBlocks(IFlexpageRepository repository, int id, bool IsEditor = false)
        {
            var list = repository.GetBlocksForBlockList(id);
            foreach(var b in list)
            {
                var block = FlexpageProcessor.LoadBlockModel(ID, b.ID, b.BlockType.Name, b.Alias, HttpContext.Current.Request.QueryString);

                if (block != null)
                {
                    block.Css = b.Css;
                    Blocks.Add(block);
                }
                else
                {
                    ViewModel m = Create(b.BlockType.Name + "Model", _settings, _flexpageProcessor);
                    if (m != null)
                    {
                        try
                        {
                            m.Load(repository, new BlockCommandModel(_settings)
                            {
                                BlockAlias = b.Alias,
                                ID = b.ID,
                                BlocklistID = id,
                                IsEditor = IsEditor
                            });
                        }
                        catch (Exception ex)
                        {
                            if (m is IBlockModel)
                            {
                                var bm = m as IBlockModel;
                                bm.HasErrors = true;
                                if (this.AdminMode)
                                    bm.ErrorsLog += String.Format("<b>Error in block {0}(ID {1}).</b> Please contact your administrator. <br><b>Message:</b> {2} <br><b>Stacktrace:</b> {3} <br><br>", bm.GetType().Name, bm.ID, ex.Message, ex.StackTrace);
                                else
                                    bm.ErrorsLog += String.Format("<b>Error in block {0}(ID {1}).</b> Please contact your administrator. <br><br>", bm.GetType().Name, bm.ID);
                            }
                        }
                        Blocks.Add(m as BlockModel);
                    }
                }

            }


        }

        public void Assign(BlockListModel source)
        {
            this.Blocks = new List<IBlockModel>(source.Blocks);
        }

        public override void Assign(object source, params object[] args)
        {
            base.Assign(source, args);
            var bl = source as BlockList;
            if (bl != null)
            {
                // this.BlocklistID = bl.ID;
                this.ID = bl.ID;
            }
        }

        public virtual object AddBlock(BlockModel block)
        {
            Blocks.Add(block);
            return block;
        }
    }

}