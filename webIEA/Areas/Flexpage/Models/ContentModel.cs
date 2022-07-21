using DevExpress.Data.Filtering;
using DevExpress.Web.Mvc;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Business;
using Flexpage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data;
using Flexpage.Code.CMS;
using System.Web;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using Flexpage.Code.Helpers;
using Flexpage.Domain.Enum;
using Pluritech.Shared.Abstract.DTO.Extensions;
using Pluritech.Shared.Abstract.DTO;
using Pluritech.Shared.Abstract;
using Flexpage.Abstract;

namespace Flexpage.Models
{
    public enum TypeContextMenu
    {
        Block = 0, Browser = 1, Contacts = 2, None = 3
    }
    public enum ContentType
    {
        [Display(Name = "File")]
        File = 0,
        [Display(Name = "Contact")]
        Contacts = 1
    }
    public enum ContentName
    {
        [Display(Name = "Folder Content")]
        FileContent = 0,
        [Display(Name = "Contacts Enumeration")]
        ContactsEnumeration = 1
    }
    public class ContentModel : BlockModel
    {
        public bool PWBrowser { get; set; }

        public enum eSourceType
        {
            TreeList = 0,
            FolderWithoutParam = 1,
            FolderWithParam = 2
        }
        //TODO make column info not static
        public List<Abstract.DTO.ColumnMapping> ColMappings { get; set; }

        public ContentType ContentType { get; set; }
        public ContentName ContentName { get; set; }
        public TypeContextMenu TypeContextMenu { get; set; }
        public int SourceType { get; set; }
        public bool WithParameter { get; set; }
        //number of files to show
        public int LastDocQnty { get; set; }
        public int PagingSize { get; set; }
        public PagingMode PagingMode { get; set; }
        

        //get from linked block
        public string SourceBlockAlias { get; set; }
        public string DateFormat { get; set; }
        public string OrderDirection { get; set; }
        public string OrderBy { get; set; }
        public string OrderFieldName { get; set; }
        public string Filter { get; set; }
        public string PWFolderName { get; set; }
        public string SelectFolderName { get; set; }
        public int SelectFolderId { get; set; }


        public bool GroupingSetCurrentLanguageFirst { get; set; }
        public bool ShowAllLanguagesInLine { get; set; }
        public bool ShowMoreLanguagesText { get; set; }
        public bool EnableRestrictLanguage { get; set; }
        public bool ShowFilterRow { get; set; }
        public bool ShowHeaderRow { get; set; }
        public bool ShowClearColumn { get; set; }
        public bool GroupByDescription { get; set; }
        public bool ShowDetailRow { get; set; }
        public bool AllowOnlyOneDetailRow { get; set; }
        public bool ShowDetailButtons { get; set; }
        public bool ExpandAllDetailsOnLoad { get; set; }
        public bool AllowExport { get; set; }
        public bool AllowSort { get; set; }
        public bool AllowUpload { get; set; }
        public int FileSource { get; set; }
        public string SourceCriteria { get; set; }
        public string FilterCustomProperties { get; set; }
        public string FilterExtension { get; set; }
        public bool AllowSelectByRowClick { get; set; }

        public string Search { get; set; }

        public bool ShowItemAsLink { get; set; }

        //columns
        public List<GridColumnModel> Columns { get; set; }
        public GridViewModel GridModel { get; set; }
        public CardViewModel CardModel { get; set; }

        //query string of original page
        protected NameValueCollection QueryString { get; set; }

        //dictionaries
        

        public string[] AllowedOrderBy { get; set; }
        public string[] AllowedOrderDirection { get; set; }

        public IFlexpageRepository Repository { get; set; }

        public bool ShowDescriptionAsName { get; set; } = true;

        public bool ShowSearchControl { get; set; }

        public bool ShowSelectionBoxes { get; set; }

        public ContentModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : this(settings, flexpage, null)
        {
        }

        public ContentModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage, NameValueCollection queryString) : base(settings, flexpage)
        {
            Columns = new List<GridColumnModel>();
            //Records = new List<Helpers.ContentObject>();

            GridModel = new GridViewModel();
            GridModel.KeyFieldName = "ID";

            CardModel = new CardViewModel();
            CardModel.KeyFieldName = "ID";

            
            AllowedOrderDirection = System.Enum.GetNames(typeof(ColumnSortOrder));

            QueryString = queryString;
        }

        public override void AssignDefaultValues(params object[] args)
        {
            SourceType = 0;
            LastDocQnty = 10;
            PagingSize = 15;

            DateFormat = "dd/MM/yyyy";
            PWFolderName = "\\";

            AllowOnlyOneDetailRow = true;
            AllowSort = true;
            ShowFilterRow = true;
            ShowHeaderRow = true;
            ShowDetailButtons = true;
            WithParameter = false;
            AllowSelectByRowClick = false;
        }
       
        public void AddColumn()
        {
            Columns.Sort((x, y) => x.Order.CompareTo(y.Order));
            Columns.Add(new GridColumnModel() { Name = "New column", Order = Columns.Count });
        }

        public void RemoveColumn(string idx)
        {
            // System columns dont show up in settings, so they should be put in the end of the array to not interfere with ordering.
            Columns.Sort((x, y) => x.IsSystem ? 1 : y.IsSystem ? -1 : x.Order.CompareTo(y.Order));
            try
            {
                var idInt = Convert.ToInt32(idx);
                while (Columns[idInt].IsSystem) 
                {
                    idInt++;
                }
                Columns.RemoveAt(idInt);
            }
            catch { }
        }

        public void ApplyColumns(IFlexpageRepository repository)
        {
            var oldColumns = repository.GetGridColumns(ID);
            List<GridColumn> newColumns = new List<GridColumn>();
            foreach (var column in Columns.Where(c => !c.IsSystem))
            {
                GridColumn target = repository.GetByID<GridColumn>(column.ID);
                if (target == null)
                {
                    target = repository.CreateGridColumn();
                }
                newColumns.Add(target);
                target.BlockID = ID;
                target.Name = column.Name;
                target.OnlyForAuthorized = column.OnlyForAuthorized;
                target.Order = column.Order;
                target.ShowAsLink = column.ShowAsLink;
                target.ShowInDetail = column.ShowInDetail;
                target.ShowInGrid = column.ShowInGrid;
                target.Alignment = column.Alignment;
            }
            foreach (var toDelete in oldColumns.Except(newColumns)){
                repository.DeleteEntity<GridColumn>(toDelete.ID);
            }
            repository.ApplyChanges();
        }

        public void AssignColumns(ICollection<GridColumn> columns)
        {
            foreach (var column in columns)
            {
                Abstract.DTO.ColumnMapping mapping = null;
                mapping = ColMappings.FirstOrDefault(m => m.ShortName == column.Name);
                //columns for editing
                var editColumn = new GridColumnModel()
                {
                    Name = column.Name,
                    Alignment = column.Alignment,
                    ShowInGrid = column.ShowInGrid,
                    ShowInDetail = column.ShowInDetail,
                    ShowAsLink = column.ShowAsLink,
                    OnlyForAuthorized = column.OnlyForAuthorized,
                    Order = column.Order,

                    FieldName = mapping != null ? mapping.FullName : column.Name,
                    ColumnCaption = mapping != null ? mapping.Caption : column.Name,
                    SortOrder = OrderBy == column.Name ? (OrderDirection == ColumnSortOrder.Ascending.ToString() ? ColumnSortOrder.Ascending : ColumnSortOrder.Descending) : ColumnSortOrder.None,
                };
                bool need_to_add = ColMappings.Any(c => c.ShortName == editColumn.Name)
                    && !Columns.Any(c => c.Name == editColumn.Name);
                if (need_to_add)
                    Columns.Add(editColumn);
                if (!need_to_add)
                {
                    var added = Columns.FirstOrDefault(c => c.Name == editColumn.Name);
                    if (added != null)
                    {
                        added.ShowInGrid |= editColumn.ShowInGrid;
                        added.ShowInDetail |= editColumn.ShowInDetail;
                        added.ShowAsLink |= editColumn.ShowAsLink;
                        added.OnlyForAuthorized |= editColumn.OnlyForAuthorized;
                    }
                }

                //columns for view
                if (need_to_add && (editColumn.ShowInGrid || OrderBy == column.Name))
                {
                    GridModel.Columns.Add(mapping != null ? mapping.FullName : column.Name);
                    CardModel.Columns.Add(mapping != null ? mapping.FullName : column.Name);

                    if (OrderBy == column.Name && AllowSort)
                    {
                        GridModel.Columns.Last().SortIndex = 0;
                        GridModel.Columns.Last().SortOrder = OrderDirection == ColumnSortOrder.Ascending.ToString() ? ColumnSortOrder.Ascending : ColumnSortOrder.Descending;
                        GridModel.SortBy(GridModel.Columns.Last(), true);

                        CardModel.Columns.Last().SortIndex = 0;
                        CardModel.Columns.Last().SortOrder = OrderDirection == ColumnSortOrder.Ascending.ToString() ? ColumnSortOrder.Ascending : ColumnSortOrder.Descending;
                        CardModel.SortBy(CardModel.Columns.Last(), true);
                    }
                }
            }
            Columns.Sort((x, y) => x.Order.CompareTo(y.Order));
        }

        //Working with data
        protected List<FilterDesciption> GetFilterDescrFromDevExOperands(FunctionOperator oper, string defTable)
        {
            string[] name = new string[0];
            string value = "";
            var result =new List<FilterDesciption>();
            foreach (var o in oper.Operands)
            {
                if(o is OperandProperty)
                    name = ((OperandProperty)o).PropertyName.TrimStart('[').TrimEnd(']').Split('.');
                if(o is OperandValue)
                    value = ((OperandValue)o).Value.ToString();
            }
            if(name.Length != 0 && !string.IsNullOrWhiteSpace(value))
            {
                value.Split(' ').ToList().ForEach(v =>
                {
                    result.Add(new FilterDesciption() {
                        Table = name.Length == 2 ? name[0] : defTable,
                        Field = name.Last(),
                        Value = v });
                });
            }
            return result;
        }

        protected List<FilterDesciption> GetFilterDescrFromDevExOperands(BinaryOperator oper, string defTable)
        {
            //left operand - field
            string[] name = ((OperandProperty)oper.LeftOperand).PropertyName.TrimStart('[').TrimEnd(']').Split('.');
            //right - value
            string value = ((OperandValue)oper.RightOperand).Value.ToString();
            var result = new List<FilterDesciption>();
            if (name.Length != 0 && !string.IsNullOrWhiteSpace(value))
            {
                value.Split(' ').ToList().ForEach(v =>
                {
                    result.Add(new FilterDesciption()
                    {
                        Table = name.Length == 2 ? name[0] : defTable,
                        Field = name.Last(),
                        Value = value,
                        FilterMode = oper.OperatorType == BinaryOperatorType.Equal ? eFilterMode.Equal : (oper.OperatorType == BinaryOperatorType.GreaterOrEqual ? eFilterMode.GreatEqual : eFilterMode.LessEqual)
                    });
                });
            }
            return result;
        }

        protected virtual List<FilterDesciption> GenerateFilterBy(string filterExpression, string defTable, string treeList_FolderName = null, List<FilterDesciption> filterBy=null)
        {
            string folderName = SourceType == 0 ? treeList_FolderName : (FileSource != 2|| SourceType == 3 ? PWFolderName : "\\");
            if (SourceType == 2 && !string.IsNullOrWhiteSpace(Filter))
            {
                string[] filter = Filter.Split('=');
                if (QueryString != null && QueryString[filter[0]] != null)
                {
                    string filterValue = QueryString[filter[0]].ToString();
                    folderName = String.Format(PWFolderName, filterValue);
                }
                else
                {
                    folderName = String.Format(PWFolderName, filter[1]);
                }
            }

            if (folderName == null)
            {
                folderName = PWFolderName;
            }
            if (filterBy == null)
            {
                filterBy = new List<FilterDesciption>();
            }
            if(ContentType == ContentType.Contacts && (FileSource == 2|| SourceType == 3))
            { }
            else
            {
                filterBy.Add(new FilterDesciption()
                {
                    Table = "Folder",
                    Field = "Name",
                    Value = folderName,
                    FilterMode = FileSource == 0 ? eFilterMode.Equal : eFilterMode.StartWith
                    //FilterMode = FileSource == 0 || SourceType == 0 ? eFilterMode.Equal : eFilterMode.StartWith
                });
            };
            if (!string.IsNullOrEmpty(Search))
            {
                Search.Split(' ').ToList().ForEach(s =>
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        filterBy.Add(new FilterDesciption()
                        {
                            Table = "Search",
                            Field = "Search",
                            Value = s,
                            FilterMode = eFilterMode.Contains
                        });
                    }
                });
               
            }
            if (!string.IsNullOrEmpty(FilterCustomProperties))
            {
                FilterCustomProperties.Split(',').ToList().ForEach(s =>
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        filterBy.Add(new FilterDesciption()
                        {
                            Table = "CustomProperty",
                            Field = s.Trim(),
                            FilterMode = eFilterMode.Contains
                        });
                    }
                });
            }
            if (!string.IsNullOrEmpty(FilterExtension))
            {
                FilterExtension.Split(',').ToList().ForEach(s =>
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        filterBy.Add(new FilterDesciption()
                        {
                            Table = "File",
                            Field = "Extension",
                            Value = s.Trim(),
                            FilterMode = eFilterMode.Contains
                        });
                    }
                });
            }
            //TODO: FILTER ARCHIVED FILES
            //if(!ShowArchivedFiles)
            //    filterBy.Add(new FilterDesciption() { Table = "CustomProperty", Field = "TimeToLeave", Value = DateTime.Now });

            if (EnableRestrictLanguage)
                filterBy.Add(new FilterDesciption() { Table = "CustomProperty", Field = "FieldsValues", Value = string.Format("3,{0}", _settings.GetCurrentOrDefaultLangCode().ToUpper()) });

            CriteriaOperator criteria = CriteriaOperator.Parse(filterExpression);
            if (criteria is FunctionOperator)
                filterBy.AddRange(GetFilterDescrFromDevExOperands(criteria as FunctionOperator, defTable));
            if (criteria is GroupOperator)
            {
                foreach (var o in ((GroupOperator)criteria).Operands)
                {
                    if (o is FunctionOperator)
                        filterBy.AddRange(GetFilterDescrFromDevExOperands(o as FunctionOperator, defTable));
                    if (o is BinaryOperator)
                        filterBy.AddRange(GetFilterDescrFromDevExOperands(o as BinaryOperator, defTable));
                }
            }
            filterBy.RemoveAll(f => f == null);
            return filterBy;
        }


        public void GetDataObjects(CardViewCustomBindingGetDataArgs e, string defTable, IObjectQueryBuilder query, string treeList_FolderName, List<FilterDesciption> filterBy)
        {
            // TODO: build filter and order from the block properties and calculate index of the first element to load
            //var filterBy = GenerateFilterBy(e.FilterExpression, defTable, treeList_FolderName);

            var orderBy = new List<OrderByDesciption>();
            if (e.State.SortedColumns != null && e.State.SortedColumns.Count > 0)
            {
                foreach (var sortCol in e.State.SortedColumns)
                {
                    string[] name = sortCol.FieldName.Split('.');
                    orderBy.Add(new OrderByDesciption() { Table = name.Length == 2 ? name[0] : defTable, Field = name.Last(), Ascending = sortCol.SortOrder == DevExpress.Data.ColumnSortOrder.Ascending });
                }
            }
            else
                orderBy.Add(new OrderByDesciption() { Table = defTable, Field = "ID" });

            orderBy = FileSourceOrderBy(orderBy);

            int startIndex = e.StartDataCardIndex + 1, numberOfObjects = e.DataCardCount;
            if (PagingMode == PagingMode.ShowAll)
            {
                numberOfObjects = query.FilterBy(filterBy).Count();
            }
            else
            {
                numberOfObjects = PagingSize;
            }

            var result = query;
            if (filterBy.Count > 0)
                result = (IObjectQueryBuilder)result.FilterBy(filterBy);
            if (orderBy.Count > 0)
                result = (IObjectQueryBuilder)result.OrderBy(orderBy);
            var list = result.ObjectsList(startIndex, numberOfObjects, true);
            if (ContentType == ContentType.File && (FileSource == 2 || SourceType == 3))
            {
                list.Take(LastDocQnty);
            }
            e.Data = list;
            // TODO: put objects into folder content model
            // show them in the the devexpress grid 
        }

        public virtual List<OrderByDesciption> FileSourceOrderBy(List<OrderByDesciption> orderBy)
        {
            if(ContentType == ContentType.File)
            {
                if(FileSource == 2|| SourceType == 3)
                {
                    if(SourceCriteria == "EntryDate")
                    {
                        orderBy.Add(new OrderByDesciption()
                        {
                            Table = "CustomProperty",
                            Field = "EntryDate",
                            Ascending = false
                        });
                    }
                    else
                    {
                        orderBy.Add(new OrderByDesciption()
                        {
                            Table = "File",
                            Field = "Date",
                            Ascending = false
                        });
                    }
                }
            }
            return orderBy;
        }

        public void GetDataObjects(GridViewCustomBindingGetDataArgs e, string defTable, IObjectQueryBuilder query, string treeList_FolderName,List<FilterDesciption> filterBy)
        {
            // TODO: build filter and order from the block properties and calculate index of the first element to load
            //var filterBy = GenerateFilterBy(e.FilterExpression, defTable, treeList_FolderName);

            var orderBy = new List<OrderByDesciption>();

            if (this.AllowSort && !string.IsNullOrEmpty(this.OrderBy))
            {
                orderBy.Add(new OrderByDesciption()
                {
                    Table = defTable,
                    Field = this.OrderBy,
                    Ascending = this.OrderDirection == DevExpress.Data.ColumnSortOrder.Ascending.ToString()
                });
            }

            if (this.AllowSort && !string.IsNullOrEmpty(this.OrderBy))
            {
                orderBy.Add(new OrderByDesciption()
                {
                    Table = defTable,
                    Field = this.OrderFieldName,
                    Ascending = this.OrderDirection == DevExpress.Data.ColumnSortOrder.Ascending.ToString()
                });
            }

            orderBy = FileSourceOrderBy(orderBy);
            if (e.State.SortedColumns != null && e.State.SortedColumns.Count > 0)
            {
                foreach (var sortCol in e.State.SortedColumns)
                {
                    string[] name = sortCol.FieldName.Split('.');
                    orderBy.Add(new OrderByDesciption() { Table = name.Length == 2 ? name[0] : defTable, Field = name.Last(), Ascending = sortCol.SortOrder == DevExpress.Data.ColumnSortOrder.Ascending });
                }
            }

            if (orderBy.Count == 0)
                orderBy.Add(new OrderByDesciption() { Table = defTable, Field = "ID" });
            
            int startIndex = e.StartDataRowIndex + 1, numberOfObjects = e.DataRowCount;
            if (PagingMode == PagingMode.ShowAll)
            {
                numberOfObjects = query.FilterBy(filterBy).Count();
            }
            else
            {
                numberOfObjects = PagingSize;
            }
            
            var result = query;

            if (filterBy.Count > 0)
                result = (IObjectQueryBuilder)result.FilterBy(filterBy);

            if (orderBy.Count > 0)
                result = (IObjectQueryBuilder)result.OrderBy(orderBy);
            var list = result.ObjectsList(startIndex, numberOfObjects, true);
            if(ContentType == ContentType.File && SourceType == 3)
            {
                list.Take(LastDocQnty);
            }
            e.Data = list;

            if (!string.IsNullOrEmpty(Search))
            {
                e.State.SearchPanel.Filter = Search;
            }
            // TODO: put objects into folder content model
            // show them in the the devexpress grid 
        }        

        public void GetDataObjectsCount(CardViewCustomBindingGetDataCardCountArgs e, string defTable, IObjectQueryBuilder query, string treeList_FolderName, List<FilterDesciption> filterBy)
        {
            //var filterBy = GenerateFilterBy(e.FilterExpression, defTable, treeList_FolderName);
            e.DataCardCount = query.FilterBy(filterBy).Count();
            
            if (ContentType == ContentType.File && (FileSource == 2 || SourceType == 3))// "Entire Site" for only case of Files
            {
                e.DataCardCount = e.DataCardCount < LastDocQnty? e.DataCardCount: LastDocQnty; 
            }
        }
        
        public void GetDataObjectsCount(GridViewCustomBindingGetDataRowCountArgs e,string defTable, IObjectQueryBuilder query, string treeList_FolderName, List<FilterDesciption> filterBy)
        {
            //var filterBy = GenerateFilterBy(e.FilterExpression, defTable, treeList_FolderName);
            e.DataRowCount = query.FilterBy(filterBy).Count();

            if (ContentType == ContentType.File && (FileSource == 2 || SourceType == 3))// "Entire Site" for only case of Files
            {
                e.DataRowCount = e.DataRowCount < LastDocQnty ? e.DataRowCount : LastDocQnty;
            }
        }

        public virtual object GetObjectPropertyValue(ObjectInfo data, string propName)
        {
            if(data != null)
            {
                object propValue = data.GetPropertyValue(propName);
                // GridColumnModel colModel = Columns.FirstOrDefault(c => c.FieldName == propName);
                if(propValue != null)
                {
                    object resValue = null;
                    switch(propName)
                    {
                        case "FileShortcut.Name":
                            resValue = GetFileInfoName(data);
                            break;
                        case "ShortText":
                            resValue = data.GetFieldValue(_settings.ShortTextFieldName);
                            break;
                        default:
                            resValue = propValue;
                            break;
                    }                       

                    if(propValue is DateTime)
                        return ((DateTime)propValue).ToString(DateFormat);
                                        
                    return resValue;
                }
            }
            return null;
        }
        public virtual void GetDataObjects(CardViewCustomBindingGetDataArgs e)
        {
            string defTable = "FileShortcut";
            var filterBy = GenerateFilterBy(e.FilterExpression, defTable, SelectFolderName);
            GetDataObjects(e, defTable, Repository.QueryFiles,SelectFolderName, filterBy);
        }
        public virtual void GetDataObjects(GridViewCustomBindingGetDataArgs e)
        {
            string defTable = "FileShortcut";
            var filterBy = GenerateFilterBy(e.FilterExpression, defTable, SelectFolderName);
            GetDataObjects(e, defTable, Repository.QueryFiles, SelectFolderName, filterBy);
        }
        public virtual void GetDataObjectsCount(CardViewCustomBindingGetDataCardCountArgs e)
        {
            string defTable = "FileShortcut";
            var filterBy = GenerateFilterBy(e.FilterExpression, defTable, SelectFolderName);
            GetDataObjectsCount(e, defTable,Repository.QueryFiles, SelectFolderName, filterBy);
        }
        public virtual void GetDataObjectsCount(GridViewCustomBindingGetDataRowCountArgs e)
        {
            string defTable = "FileShortcut";
            var filterBy = GenerateFilterBy(e.FilterExpression, defTable, SelectFolderName);
            GetDataObjectsCount(e, defTable, Repository.QueryFiles, SelectFolderName, filterBy);
        }

        public string GetFileInfoName(ObjectInfo data)
        {
            return (ShowDescriptionAsName 
                ? data.GetFileInfoDescriptionAsName(_settings.DescriptionFieldName)
                : data.GetFileInfoName(_settings.DescriptionFieldName));
        }
    }
}