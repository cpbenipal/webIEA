using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using System.Web.Mvc;


namespace Flexpage.Models
{
    public class MultiColumnModel : BlockModel
    {
        public int Index { get; set; } = 0;
        public List<ColumnModel> Columns { get; set; }
        /* public List<ColumnModel> OrderedColumns {
            get {
                return new List<ColumnModel>(Columns.OrderBy(e => e.Order));
            }
        } */

        public bool ShowHeaders { get; set; } = true;
        public bool ContentInRows { get; set; } = true;

        public LanguageSelectorModel LanguageSelector
        {
            get
            {
                return GetLanguageSelector(Columns.FirstOrDefault().Title.CurrentLangCode,
                    Columns.Select(t => t.Title).ToList(),
                    new List<LocalizedTextModel>() { },
                    "fp_multiColumn_SelectLanguage");
            }
        }

        public MultiColumnModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpageProcessor) :base(settings, flexpageProcessor)
        {
        }

        public MultiColumnModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpageProcessor, object source):this(settings, flexpageProcessor)
        {
            Assign(source);
        }

        public void SetPredefined(int id)
        {
            LanguageSelector.CurrentLangCode = "";
            Columns = new List<ColumnModel>();
            switch (id)
            {
                case 1:
                    AddColumn(1, 100, 1);
                    break;
                case 2:
                    AddColumn(0, 50, 1);
                    AddColumn(0, 50, 1);
                    break;
                case 3:
                    AddColumn(0, 33.33, 1);
                    AddColumn(0, 33.33, 1);
                    AddColumn(0, 33.33, 1);
                    break;
                case 4:
                    AddColumn(0, 25, 1);
                    AddColumn(0, 25, 1);
                    AddColumn(0, 25, 1);
                    AddColumn(0, 25, 1);
                    break;
                case 5:
                    AddColumn(0, 66.66, 1);
                    AddColumn(0, 33.33, 1);
                    break;
                case 6:
                    AddColumn(0, 33.33, 1);
                    AddColumn(0, 66.66, 1);
                    break;
                case 7:
                    AddColumn(0, 50, 1);
                    AddColumn(0, 25, 1);
                    AddColumn(0, 25, 1);
                    break;
                case 8:
                    AddColumn(0, 25, 1);
                    AddColumn(0, 25, 1);
                    AddColumn(0, 50, 1);
                    break;
                case 9:
                    AddColumn(0, 25, 1);
                    AddColumn(0, 50, 1);
                    AddColumn(0, 25, 1);
                    break;
            }
            Update();
        }

        public void Apply(IFlexpageRepository repository, MultiColumn target)
        {

            target.ContentInRows = this.ContentInRows;
            target.ShowHeaders = this.ShowHeaders;

            foreach (ColumnModel cm in Columns)
            {
                cm.Apply(repository, target);
            }

            List<Column> rl = new List<Column>();
            foreach (Column c in target.Column)
            {
                if (c.BlockListID != 0 && !Columns.Any(e => e.ID == c.BlockListID))
                    rl.Add(c);
            }
            foreach (Column c in rl)
                repository.RemoveColumn(c);
        }

        public override void Assign(object source, params object[] args)
        {
            var mc = source as MultiColumn;
            if (mc != null)
            {
                base.Assign(mc.Block, args);
                this.ShowHeaders = mc.ShowHeaders;
                this.ContentInRows = mc.ContentInRows;
                this.Columns = new List<ColumnModel>();
                foreach(var c in mc.Column)
                {
                    var cm = ViewModel.Create("ColumnModel", _settings, _flexpageProcessor) as ColumnModel;
                    cm.Assign(c);
                    // cm.LoadChildren()
                    this.Columns.Add(cm);
                }
                Reorder();
            }
        }

        /// <summary>
        /// Applies changes made to view model to repository
        /// </summary>
        /// <param name="repository">Repository</param>
        public override object Apply(IFlexpageRepository repository, params object[] args)
        {
            base.Apply(repository);
            var mc = repository.GetByBlockID<MultiColumn>(ID);
            if (mc == null)
            {
                if (Alias == repository.CreateNewAlias)
                {
                    Alias = null;
                }
                mc = repository.CreateNewMulticolumn(alias: Alias, columnCount : 0);
                repository.AddBlockToBlockList(mc.Block, BlocklistID, BlockAfter);
                // repository.AddC(mc.Block, BlocklistID);

            }
            Apply(repository, mc);

            return null;
        }

        private void AssignDefaultValues(IFlexpageRepository repository)
        {
            ColumnModel c = ColumnModel.Default(_settings, _flexpageProcessor);
            if (string.IsNullOrEmpty(this.Alias))
                this.Alias = repository.CreateNewAlias;
            Columns = new List<ColumnModel>();
            Columns.Add(c);
            // Columns = new Dictionary<int, ColumnModel>();
            // Columns.Add(Index++, c);
            //this.BlockType = "CmsText";
        }

        public override void Load(IFlexpageRepository repository, BlockCommandModel proto, string title = "", bool needToLoadContent = true)
        {
            base.Load(repository, proto,title, needToLoadContent);
            this.Alias = proto.BlockAlias;
            MultiColumn mc = null;
            if (Alias != null)
                mc = repository.GetByAlias<MultiColumn>(Alias);
            if (mc == null)
                mc = repository.GetByBlockID<MultiColumn>(proto.ID);
            if (mc == null)
                AssignDefaultValues(repository);
            else
                Assign(mc);
            this.IsStatic = proto.IsStatic;
            if (needToLoadContent)
            {
                foreach (ColumnModel c in Columns)
                    c.LoadContent(repository);
            }
            /* if (e == null)
                AssignDefaultValues(repository);
            else
                Assign(e); */
        }

        /// <summary>
        /// Creates new column
        /// </summary>
        /// <param name="widthType">0 - custom width, 1 - fill, 2 - minimum</param>
        /// <param name="width">custom width in units</param>
        /// <param name="widthUnit">0 - px, 1 - %</param>
        public void AddColumn(int widthType = 1, double width = 100, int widthUnit = 1)
        {
            ColumnModel c = ColumnModel.Default(_settings, _flexpageProcessor);
            int mo = 0;
            if (Columns.Count > 0)
                mo = Columns.Max(e => e.Order) + 1;
            c.Title = new LocalizedStringModel("{\"en\": \"" + String.Format("Column{0}", mo) + "\"}", _settings, _flexpageProcessor);
            c.Order = mo;
            c.WidthType = widthType;
            c.Width = width;
            c.CustomWidthUnit = widthUnit;
            c.MultiColumnID = this.ID;
            c.ID = -1;
            c.Alias = NewAlias;
            Columns.Add(c);
        }

        /// <summary>
        /// Removes column from columns list
        /// </summary>
        /// <param name="index">Zero-bazed column index</param>
        public void DeleteColumn(int index)
        {
            Columns.RemoveAt(index);
            Reorder();
        }

        public void Reorder()
        {
            Columns = new List<ColumnModel>(Columns.OrderBy(e => e.Order));
            UpdateOrder();
        }

        public void UpdateOrder()
        {
            int i = 0;
            foreach (var c in Columns)
            {
                c.Order = i;
                i++;
            }
        }

        public override void Delete(IFlexpageRepository repository)
        {
            base.Delete(repository);
        }

        public override void Update()
        {
            base.Update();
            foreach (var c in Columns)
                c.Update();
            // LanguageSelector.CurrentLangCode = Columns.FirstOrDefault().Title.CurrentLangCode;
        }

        public void SelectLanguage(string langCode)
        {
            foreach (var c in Columns)
                c.SelectLanguage(langCode);
        }
    }
}