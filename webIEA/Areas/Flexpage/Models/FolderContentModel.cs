using Flexpage.Domain.Abstract;
using Flexpage.Domain.Business;
using Flexpage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data;
using System.Collections.Specialized;
using Pluritech.Shared.Abstract.DTO.Extensions;
using Pluritech.Shared.Abstract.DTO;
using Flexpage.Abstract;

namespace Flexpage.Models
{
   
    public class FolderContentModel : ContentModel
    {
        public bool ManualTileGeneration { get; set; }
        public bool ShowTileName { get; set; }
        public bool ShowTiles { get; set; }
        public bool ShowPreview { get; set; }
        public bool ShowArchivedFiles { get; set; }
        public bool AllowZipDownload { get; set; }
        public bool ShowDateInName { get; set; }
        public string NameWithDateFormat { get; set; }        
        public string FolderPermissions { get; set; }

        public FolderContentModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : this(settings, flexpage, null)
        {

        }

        public FolderContentModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage, NameValueCollection queryString)
            :base(settings, flexpage, queryString)
        {
            ContentType = ContentType.File;
            ContentName = ContentName.FileContent;
            ColMappings = new List<Abstract.DTO.ColumnMapping>();

            ColMappings.Add(new Abstract.DTO.ColumnMapping() { ShortName = "Name", FullName = "FileShortcut.Name", Caption = "Name" });
            ColMappings.Add(new Abstract.DTO.ColumnMapping() { ShortName = "Extension", FullName = "File.Extension", Caption = "Extension" });
            ColMappings.Add(new Abstract.DTO.ColumnMapping() { ShortName = "Date", FullName = "File.Date", Caption = "Creation Date" });
            ColMappings.Add(new Abstract.DTO.ColumnMapping() { ShortName = "ModificationDate", FullName = "FileVersion.DateModification", Caption = "Modification Date" });
            ColMappings.Add(new Abstract.DTO.ColumnMapping() { ShortName = "UploadDate", FullName = "FileRevision.Date", Caption = "Upload Date" });

            var filesQueryBuilder = new FilesQueryBuilder();
            var objectTypes = filesQueryBuilder.GetObjectType(ObjectTypeEnum.FileShortcut);

            foreach (var objectType in objectTypes)
            {
                ColMappings.Add(new Abstract.DTO.ColumnMapping()
                {
                    ShortName = objectType.Name,
                    FullName = "CustomProperty." + objectType.Name,
                    Caption = objectType.Caption
                });
            }

            filesQueryBuilder.GetObjectType(ObjectTypeEnum.Folder);

            foreach (var objectType in objectTypes)
            {
                var name = (ColMappings.FirstOrDefault(w => w.FullName == objectType.Name) != null ? objectType.Name : "Folder" + objectType.Name);

                ColMappings.Add(new Abstract.DTO.ColumnMapping()
                {
                    ShortName = name,
                    FullName = "Folder." + objectType.Name,
                    Caption = objectType.Title
                });
            }
           
            ColMappings.Add(new Abstract.DTO.ColumnMapping() { ShortName = "FolderName", FullName = "Folder.Name", Caption = "Name" });
            AllowedOrderBy = System.Enum.GetNames(typeof(FolderContentOrderBy));
        }

        public override void Load(IFlexpageRepository repository, BlockCommandModel proto, string title = "", bool needToLoadContent=true)
        {
            Load(repository, proto, needToLoadContent, null);
        }

        public void Load(IFlexpageRepository repository, BlockCommandModel proto, bool needToLoadContent, string SelectFolderName, bool AllowSelectByRowClick=false)
        {
            base.Load(repository, proto,"", needToLoadContent);
            Repository = repository;
            FolderContent block = repository.GetByBlockID<FolderContent>(proto.ID);
            this.SelectFolderName = SelectFolderName;
            this.AllowSelectByRowClick = AllowSelectByRowClick;
            UpdateFolderPermissions();
            if (block == null)
                AssignDefaultValues();
            else
            {
                
                Repository = repository;
                Assign(block);
                if (needToLoadContent)
                {
                    //GetDataObjects(repository);
                }
            }
        }
        public override void AssignDefaultValues(params object[] args)
        {
            SourceType = 0;
            LastDocQnty = 10;
            PagingSize = 15;

            DateFormat = "dd/MM/yyyy";
            NameWithDateFormat = "{0}, {1}";
            PWFolderName = "\\";

            AllowOnlyOneDetailRow = true;
            AllowSort = true;
            ShowFilterRow = true;
            ShowHeaderRow = true;
            ShowDetailButtons = true;
            ShowDescriptionAsName = true;
            ShowSelectionBoxes = false;
        }

        public override object Apply(IFlexpageRepository repository, params object[] args)
        {
            FolderContent folderContent = repository.GetByBlockID<FolderContent>(ID);
            if (folderContent == null || Alias == repository.CreateNewAlias) // ???
            {
                if (Alias == repository.CreateNewAlias)
                    Alias = null;

                folderContent = repository.CreateNewFolderContent( Visible, CssClass,Alias);
                repository.AddBlockToBlockList(folderContent.Block, BlocklistID, BlockAfter);
            }

            //apply props
            //folderContent.ShowSize = ShowSize;
            folderContent.ShowArchivedFiles = ShowArchivedFiles;
            folderContent.OrderBy = OrderBy;
            folderContent.OrderDirection = OrderDirection;
            folderContent.OrderFieldName = OrderFieldName;
            folderContent.GroupingSetCurrentLanguageFirst = GroupingSetCurrentLanguageFirst;
            folderContent.AllowZipDownloads = AllowZipDownload;
            folderContent.GroupByDescription = GroupByDescription;
            //folderContent.ShowItemAsLink = ShowItemAsLink;
            folderContent.ShowAllLanguagesInLine = ShowAllLanguagesInLine;
            folderContent.ShowMoreLanguagesText = ShowMoreLanguagesText;
            folderContent.EnableRestrictLanguage = EnableRestrictLanguage;
            if (PagingSize != 0)
            {
                folderContent.PagingSize = PagingSize;
            }
            if (folderContent.PagingSize == 0)
            {
                folderContent.PagingSize = 15;
            }
            folderContent.PagingMode = PagingMode;
            folderContent.ShowDetailRow = ShowDetailRow;
            folderContent.AllowOnlyOneDetailRow = AllowOnlyOneDetailRow;
            folderContent.ExpandAllDetailsOnLoad = ExpandAllDetailsOnLoad;
            folderContent.AllowSort = AllowSort;
            folderContent.ShowFilterRow = ShowFilterRow;
            folderContent.ShowHeaderRow = ShowHeaderRow;
            //folderContent.CenteredDate = CenteredDate;
            //folderContent.ShowDateAsLink = ShowDateAslink;
            //folderContent.ShowDateInName = ShowDateInName;

            //TODO
            folderContent.NameWithDateFormat = "{0}, {1}";// NameWithDateFormat;

            folderContent.DateFormat = DateFormat;
            folderContent.ShowDetailButtons = ShowDetailButtons;
            folderContent.ShowClearColumn = ShowClearColumn;
            //folderContent.UseFilter = UseFilter;
            folderContent.Filter = Filter;
            folderContent.LastDocQnty = LastDocQnty;
            folderContent.FileSource = FileSource;
            folderContent.SourceCriteria = SourceCriteria;

            folderContent.SourceType = SourceType == 1 && WithParameter ? 2 : SourceType;
            folderContent.FolderName = PWFolderName;
            folderContent.SourceBlockAlias = (SourceType == 0 && !string.IsNullOrEmpty( SourceBlockAlias)) ? SourceBlockAlias : (SourceType == 0? folderContent.SourceBlockAlias:"");

            folderContent.ShowTiles = ShowTiles;
            folderContent.ShowPreview = ShowPreview;
            folderContent.ManualTileGeneration = ManualTileGeneration;
            folderContent.ShowTileName = ShowTileName;
            folderContent.AllowUpload = AllowUpload;
            folderContent.ShowDescriptionAsName = ShowDescriptionAsName;
            folderContent.ShowSelectionBoxes = ShowSelectionBoxes;
            folderContent.AllowSelectByRowClick = AllowSelectByRowClick;
            repository.ApplyChanges();
            ID = folderContent.BlockID;
            ApplyColumns(repository);

            
            return folderContent;
        }
        public override void Assign(object source, params object[] args)
        {
            FolderContent folderContent = source as FolderContent;
            base.Assign(folderContent.Block);

            if (folderContent.ShowArchivedFiles != null) ShowArchivedFiles = (bool) folderContent.ShowArchivedFiles;
            PWFolderName = folderContent.FolderName;
            OrderBy = folderContent.OrderBy;
            OrderDirection = folderContent.OrderDirection;
            OrderFieldName = folderContent.OrderFieldName;
            GroupingSetCurrentLanguageFirst = folderContent.GroupingSetCurrentLanguageFirst ?? false;
            AllowZipDownload = folderContent.AllowZipDownloads ?? false;
            GroupByDescription = folderContent.GroupByDescription ?? false;
            ShowAllLanguagesInLine = folderContent.ShowAllLanguagesInLine ?? false;
            ShowMoreLanguagesText = folderContent.ShowMoreLanguagesText ?? false;
            EnableRestrictLanguage = folderContent.EnableRestrictLanguage ?? false;
            PagingSize = folderContent.PagingSize;
            if (PagingSize == 0)
            {
                PagingSize = 15;
            }
            
            PagingMode = folderContent.PagingMode;
            ShowDetailRow = folderContent.ShowDetailRow;
            AllowOnlyOneDetailRow = folderContent.AllowOnlyOneDetailRow;
            ExpandAllDetailsOnLoad = folderContent.ExpandAllDetailsOnLoad;
            AllowSort = folderContent.AllowSort;
            ShowFilterRow = folderContent.ShowFilterRow;
            ShowHeaderRow = folderContent.ShowHeaderRow;
            ShowDateInName = folderContent.ShowDateInName;
            NameWithDateFormat = folderContent.NameWithDateFormat;
            DateFormat = folderContent.DateFormat;
            ShowDetailButtons = folderContent.ShowDetailButtons;
            ShowClearColumn = folderContent.ShowClearColumn;
            Filter = folderContent.Filter;
            SourceType = folderContent.SourceType;
            LastDocQnty = folderContent.LastDocQnty ?? 10;
            FileSource = folderContent.FileSource ?? 0;
            SourceCriteria = folderContent.SourceCriteria;
            SourceType = folderContent.SourceType;
            PWFolderName = folderContent.FolderName;
            SourceBlockAlias = SourceType == 0 ? folderContent.SourceBlockAlias : "";
            WithParameter = SourceType == 2;
            ShowItemAsLink = folderContent.ShowItemAsLink ?? false;
            ShowTiles = folderContent.ShowTiles;
            ShowPreview = folderContent.ShowPreview;
            ManualTileGeneration = folderContent.ManualTileGeneration;
            ShowTileName = folderContent.ShowTileName;
            AllowUpload = folderContent.AllowUpload;
            ShowDescriptionAsName = folderContent.ShowDescriptionAsName;
            ShowSelectionBoxes = folderContent.ShowSelectionBoxes;
            AllowSelectByRowClick = folderContent.AllowSelectByRowClick;
            if (folderContent.Block.GridColumns.Any())
            {
                AssignColumns(folderContent.Block.GridColumns);
                Columns.Add(new GridColumnModel()
                {
                    Name = "FileShortcut.Permissions",
                    FieldName = "FileShortcut.Permissions",
                    ShowInGrid = false,
                    SortOrder = ColumnSortOrder.Ascending,
                    IsSystem = true
                });
                Columns.Sort((x, y) => x.Order.CompareTo(y.Order));
            }

            GridModel.Pager.PageSize = PagingSize;
            CardModel.Pager.SettingsTableLayout.ColumnCount = PagingSize;

            //TODO: Set from forms controls
            CardModel.Pager.SettingsTableLayout.RowsPerPage = 1;

        }

        public override object GetObjectPropertyValue(ObjectInfo data, string propName)
        {
            if (data != null)
            {
                object propValue = data.GetPropertyValue(propName);
                // GridColumnModel colModel = Columns.FirstOrDefault(c => c.FieldName == propName);
                if (propValue != null)
                {
                    object resValue = null;
                    switch (propName)
                    {
                        case "FileShortcut.Name":
                            resValue = this.GetFileInfoName(data);
                            if (ShowDateInName && !string.IsNullOrWhiteSpace(NameWithDateFormat))
                            {
                                var mapping = ColMappings.FirstOrDefault(m => m.ShortName == "Date");
                                if (mapping != null)
                                {
                                    propValue = data.GetPropertyValue(mapping.FullName);
                                    if (propValue != null)
                                        resValue = string.Format(NameWithDateFormat, resValue, ((DateTime)propValue).ToString(DateFormat));
                                }
                            }
                            break;
                        case "ShortText":
                            resValue = data.GetFieldValue(_settings.ShortTextFieldName);
                            break;
                        default:
                            resValue = propValue;
                            break;
                    }

                    if (propValue is DateTime)
                        return ((DateTime)propValue).ToString(DateFormat);

                    return resValue;
                }
            }
            return null;
        }

        public static string GetFolderName(IFlexpageRepository repository, int folderContentID, string selectedFolderName)
        {
            FolderContent block = repository.GetByBlockID<FolderContent>(folderContentID);
            string folderName = "";
            switch(block.SourceType)
            {
                case (int)ContentModel.eSourceType.TreeList:
                    {
                        folderName = selectedFolderName;
                        break;
                    }
                case (int)ContentModel.eSourceType.FolderWithoutParam:
                    {
                        folderName = block.FolderName;
                        break;
                    }
                case (int)ContentModel.eSourceType.FolderWithParam:
                    {
                        // TODO: take param from query string ?
                        throw new NotImplementedException();
                    }
            }
            return folderName;
        }

        public void UpdateFolderPermissions()
        {
            if (!String.IsNullOrEmpty(SelectFolderName))
            {
                var result = Repository.QueryFolders;
                var filterBy = new List<FilterDesciption>() {
                    new FilterDesciption()
                    {
                        Table = "Folder",
                        Field = "Name",
                        Value = SelectFolderName,
                        FilterMode = eFilterMode.Equal
                    },
                };
                result = result.FilterBy(filterBy);
                ObjectInfo folder = result.ObjectsList(PWBrowser).FirstOrDefault();
                if (folder != null) // TODO: null sometime happens for unclear reason
                {
                    FolderPermissions = folder.GetPropertyValue("Folder.Permissions").ToString();
                }
            }
        }
    }
}