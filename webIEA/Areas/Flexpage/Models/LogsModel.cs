using DevExpress.Data;
using Flexpage.Domain.Abstract;
using Flexpage.Helpers;
using Flexpage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Utilities;
using System.Web.UI.WebControls;
using System.Web.UI;
using Flexpage.Abstract;

namespace Flexpage.Models
{
    public enum WebTrafficLogEntryActions
    {
        None = 0, Login, Logout
    };

    public class WebTrafficLogEntry: LogEntry
    {
        [LogEntryItemAttribute(1, ColumnSortOrder.Ascending)]
        public WebTrafficLogEntryActions ActionType { get; set; }

        [LogEntryItem(2, CellAlign = HorizontalAlign.Center, SortOrder = ColumnSortOrder.Ascending)]
        public string SessionID { get; set; }

        [LogEntryItem(3, CellAlign = HorizontalAlign.Center, SortOrder = ColumnSortOrder.Ascending)]
        public string Company { get; set; }

        [LogEntryItem(4, CellAlign = HorizontalAlign.Center, SortOrder = ColumnSortOrder.Ascending)]
        public string PWName { get; set; }

        public WebTrafficLogEntry()
        {
            this.ActionType = WebTrafficLogEntryActions.None;
            this.Company = "";
            this.SessionID = "";
        }

        public WebTrafficLogEntry(int ID, string entry, string fullType)
            : base(ID, entry, fullType)
        {
            System.Type type = this.GetType();
            if(!string.IsNullOrEmpty(fullType))
            {
                type = System.Type.GetType(fullType);
            }
            if(type != null)
            {
                try
                {
                    WebTrafficLogEntry tmp = (WebTrafficLogEntry)JsonConvert.DeserializeObject(entry, type, new JavaScriptDateTimeConverter());
                    if(tmp != null)
                    {
                        this.ActionType = tmp.ActionType;
                        this.SessionID = tmp.SessionID;
                        this.Company = tmp.Company;
                        this.PWName = tmp.PWName;
                    }
                    this.ID = ID;
                }
                catch
                {
                }
            }
        }

        public WebTrafficLogEntry(string ip, string userName, string url, string urlReferrer, DateTime? date,
             WebTrafficLogEntryActions actionType, string sessionID, string company, string pwName)
            : base(ip, userName, url, urlReferrer, date)
        {
            this.ActionType = actionType;
            this.SessionID = sessionID;
            this.Company = company;
            this.PWName = pwName;
        }

        public static WebTrafficLogEntry Create(WebTrafficLogEntryActions actionType, string sessionID, string company, string userName = "", string pwName = "")
        {
            string url = "";
            if(HttpContext.Current != null && HttpContext.Current.Request != null)
            {
                //url = HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.RawUrl;
                //if(url.IndexOf('?') > 0)
                //{
                //    url = url.Split('?')[0];
                //}

                //if((url.IndexOf("http://") == 0 || url.IndexOf("https://") == 0) == false)
                //{
                //    url = "http://" + url.TrimStart('/');
                //}
                url = HttpContext.Current.Request.RawUrl;
            }

            string referrerUrl = "";
            if(HttpContext.Current != null && HttpContext.Current.Request != null && HttpContext.Current.Request.UrlReferrer != null)
            {
                if(HttpContext.Current.Request.Url.Authority == HttpContext.Current.Request.UrlReferrer.Authority)
                {
                    referrerUrl = HttpContext.Current.Request.UrlReferrer.AbsolutePath;
                }
                else
                {
                    referrerUrl = HttpContext.Current.Request.UrlReferrer.AbsoluteUri;
                }
            }

            return new WebTrafficLogEntry(HttpContext.Current.Request.UserHostAddress,
                (HttpContext.Current == null || HttpContext.Current.User == null) ? userName : HttpContext.Current.User.Identity.Name,
                url,
               referrerUrl,
                DateTime.Now, actionType, sessionID, company, pwName);
        }
    }

    public enum TrackChangesLogEntryActions
    {
        None, Insert, Update, Delete,
        AddColumns, RemoveColumns, AddRemoveColumns,
        RemoveProtocols, AddProtocols, AddRemoveProtocols,
        RemoveParameters, AddParameters, AddRemoveParameters,
        RemovePrices, AddPrices, AddRemovePrices,
        AddAvailabilityMonths, RemoveAvailabilityMonths, AddRemoveAvailabilityMonths
    };

    public class TrackChangesLogEntry: LogEntry
    {
        [LogEntryItem(1, ColumnSortOrder.Ascending)]
        public TrackChangesLogEntryActions ActionType { get; set; }

        [LogEntryItem()]
        public string TableName { get; set; }

        [LogEntryItem()]
        public object PrimaryKey { get; set; }

        [LogEntryItem(IsDetail = true, IsListOfPairs = true)]
        public List<Pair> OldValues { get; set; }

        [LogEntryItem(IsDetail = true, IsListOfPairs = true)]
        public List<Pair> NewValues { get; set; }

        [LogEntryItem(IsDetail = true, IsListOfPairs = true)]
        public List<Pair> AddedColumns { get; set; }

        [LogEntryItem(IsDetail = true, IsListOfPairs = true)]
        public List<Pair> RemovedColumns { get; set; }

        [LogEntryItem(IsDetail = true, IsListOfPairs = true)]
        public List<Pair> AddedProtocols { get; set; }

        [LogEntryItem(IsDetail = true, IsListOfPairs = true)]
        public List<Pair> RemovedProtocols { get; set; }

        [LogEntryItem(IsDetail = true, IsListOfPairs = true)]
        public List<Pair> AddedParameters { get; set; }

        [LogEntryItem(IsDetail = true, IsListOfPairs = true)]
        public List<Pair> RemovedParameters { get; set; }

        [LogEntryItem(IsDetail = true, IsListOfPairs = true)]
        public List<Pair> AddedPrices { get; set; }

        [LogEntryItem(IsDetail = true, IsListOfPairs = true)]
        public List<Pair> RemovedPrices { get; set; }

        [LogEntryItem(IsDetail = true, IsListOfPairs = true)]
        public List<Pair> AddedAvailabilityMonths { get; set; }

        [LogEntryItem(IsDetail = true, IsListOfPairs = true)]
        public List<Pair> RemovedAvailabilityMonths { get; set; }

        public TrackChangesLogEntry()
        {
            this.ActionType = TrackChangesLogEntryActions.None;
            this.OldValues = new List<Pair>();
            this.NewValues = new List<Pair>();
            this.AddedColumns = new List<Pair>();
            this.RemovedColumns = new List<Pair>();
            this.AddedProtocols = new List<Pair>();
            this.RemovedProtocols = new List<Pair>();
            this.AddedParameters = new List<Pair>();
            this.RemovedParameters = new List<Pair>();
            this.AddedPrices = new List<Pair>();
            this.RemovedPrices = new List<Pair>();
            this.AddedAvailabilityMonths = new List<Pair>();
            this.RemovedAvailabilityMonths = new List<Pair>();
        }

        public TrackChangesLogEntry(int ID, string entry, string fullType)
            : base(ID, entry, fullType)
        {
            System.Type type = this.GetType();
            if(!string.IsNullOrEmpty(fullType))
            {
                type = System.Type.GetType(fullType);
            }
            if(type != null)
            {
                TrackChangesLogEntry tmp = (TrackChangesLogEntry)JsonConvert.DeserializeObject(entry, type, new JavaScriptDateTimeConverter());
                if(tmp != null)
                {
                    this.TableName = tmp.TableName;
                    this.ActionType = tmp.ActionType;
                    this.PrimaryKey = tmp.PrimaryKey;
                    this.OldValues = tmp.OldValues;
                    this.NewValues = tmp.NewValues;
                    this.AddedColumns = tmp.AddedColumns;
                    this.RemovedColumns = tmp.RemovedColumns;
                    this.AddedProtocols = tmp.AddedProtocols;
                    this.RemovedProtocols = tmp.RemovedProtocols;
                    this.AddedParameters = tmp.AddedParameters;
                    this.RemovedParameters = tmp.RemovedParameters;
                    this.AddedPrices = tmp.AddedPrices;
                    this.RemovedPrices = tmp.RemovedPrices;
                    this.AddedAvailabilityMonths = tmp.AddedAvailabilityMonths;
                    this.RemovedAvailabilityMonths = tmp.RemovedAvailabilityMonths;
                }
                this.ID = ID;
            }
        }

        public TrackChangesLogEntry(string ip, string userName, string url, string urlReferrer, DateTime? date,
            string tableName, TrackChangesLogEntryActions actionType, object primaryKey,
            List<Pair> oldValues, List<Pair> newValues,
            List<Pair> addedColumns, List<Pair> removedColumns,
            List<Pair> addedProtocols, List<Pair> removedProtocols,
            List<Pair> addedParameters, List<Pair> removedParameters,
            List<Pair> addedPrices, List<Pair> removedPrices,
            List<Pair> addedAvailabilityMonths, List<Pair> removedAvailabilityMonths)
            : base(ip, userName, url, urlReferrer, date)
        {
            this.ActionType = actionType;
            this.TableName = tableName;
            this.OldValues = oldValues.Count > 0 ? oldValues.OrderBy(c => c.First).ToList() : oldValues;
            this.NewValues = newValues.Count > 0 ? newValues.OrderBy(c => c.First).ToList() : newValues;
            this.AddedColumns = addedColumns.Count > 0 ? addedColumns.OrderBy(c => c.First).ToList() : addedColumns;
            this.RemovedColumns = removedColumns.Count > 0 ? removedColumns.OrderBy(c => c.First).ToList() : removedColumns;
            this.AddedProtocols = addedProtocols.Count > 0 ? addedProtocols.OrderBy(c => c.First).ToList() : addedProtocols;
            this.RemovedProtocols = removedProtocols.Count > 0 ? removedProtocols.OrderBy(c => c.First).ToList() : removedProtocols;
            this.AddedParameters = addedParameters.Count > 0 ? addedParameters.OrderBy(c => c.First).ToList() : addedParameters;
            this.RemovedParameters = removedParameters.Count > 0 ? removedParameters.OrderBy(c => c.First).ToList() : removedParameters;
            this.AddedPrices = addedPrices.Count > 0 ? addedPrices.OrderBy(c => c.First).ToList() : addedPrices;
            this.RemovedPrices = removedPrices.Count > 0 ? removedPrices.OrderBy(c => c.First).ToList() : removedPrices;
            this.AddedAvailabilityMonths = addedAvailabilityMonths.Count > 0 ?
                addedAvailabilityMonths.OrderBy(c => c.First).ToList() : addedAvailabilityMonths;
            this.RemovedAvailabilityMonths = removedAvailabilityMonths.Count > 0 ?
                removedAvailabilityMonths.OrderBy(c => c.First).ToList() : removedAvailabilityMonths;
            this.PrimaryKey = primaryKey;
        }

        public static TrackChangesLogEntry Create(string tableName, TrackChangesLogEntryActions actionType, object primaryKey,
            List<Pair> oldValues, List<Pair> newValues,
            List<Pair> addedColumns, List<Pair> removedColumns,
            List<Pair> addedProtocols, List<Pair> removedProtocols,
            List<Pair> addedParameters, List<Pair> removedParameters,
            List<Pair> addedPrices, List<Pair> removedPrices,
            List<Pair> addedAvailabilityMonths, List<Pair> removedAvailabilityMonths)
        {
            return new TrackChangesLogEntry(HttpContext.Current.Request.UserHostAddress,
                HttpContext.Current.User.Identity.Name,
                HttpContext.Current.Request.Url.AbsoluteUri,
                HttpContext.Current.Request.UrlReferrer != null ? HttpContext.Current.Request.UrlReferrer.AbsoluteUri : string.Empty,
                DateTime.Now, tableName, actionType, primaryKey,
                oldValues, newValues,
                addedColumns, removedColumns,
                addedProtocols, removedProtocols,
                addedParameters, removedParameters,
                addedPrices, removedPrices,
                addedAvailabilityMonths, removedAvailabilityMonths);
        }
    }

    public class LogEntryItemAttribute: Attribute
    {
        public bool IsDetail { get; set; }
        public bool IsHidden { get; set; }
        public int? Order { get; set; }
        public int? SortIndex { get; set; }
        public ColumnSortOrder SortOrder { get; set; }
        public bool IsListOfPairs { get; set; }
        public string DateStringFormat { get; set; }
        public bool IsLink { get; set; }
        public HorizontalAlign CellAlign { get; set; }

        public LogEntryItemAttribute()
        {
            this.IsDetail = false;
            this.IsHidden = false;
            this.Order = null;
            this.SortIndex = null;
            this.SortOrder = ColumnSortOrder.None;
            this.IsListOfPairs = false;
            this.DateStringFormat = string.Empty;
            this.IsLink = false;
            this.CellAlign = HorizontalAlign.Left;
        }


        public LogEntryItemAttribute(int sortIndex, ColumnSortOrder sortOrder)
        {
            this.IsDetail = false;
            this.IsHidden = false;
            this.Order = null;
            this.SortIndex = sortIndex;
            this.SortOrder = sortOrder;
            this.IsListOfPairs = false;
            this.DateStringFormat = string.Empty;
            this.IsLink = false;
            this.CellAlign = HorizontalAlign.Left;
        }

        public LogEntryItemAttribute(int order)
        {
            this.IsDetail = false;
            this.IsHidden = false;
            this.Order = order;
            this.SortIndex = null;
            this.SortOrder = ColumnSortOrder.None;
            this.IsListOfPairs = false;
            this.DateStringFormat = string.Empty;
            this.IsLink = false;
            this.CellAlign = HorizontalAlign.Left;
        }

        public LogEntryItemAttribute(int order, int sortIndex, ColumnSortOrder sortOrder)
        {
            this.IsDetail = false;
            this.IsHidden = false;
            this.Order = order;
            this.SortIndex = sortIndex;
            this.SortOrder = sortOrder;
            this.IsListOfPairs = false;
            this.DateStringFormat = string.Empty;
            this.IsLink = false;
            this.CellAlign = HorizontalAlign.Left;
        }

        public static LogEntryItemAttribute GetAttribute(PropertyInfo p)
        {
            var attrs = p.GetCustomAttributes(typeof(LogEntryItemAttribute), true);
            LogEntryItemAttribute attr = attrs.Length > 0 ? (LogEntryItemAttribute)attrs[0]
                : new LogEntryItemAttribute();
            return attr;
        }
    }
     
    public class LogEntry
    {
        [LogEntryItem(IsHidden = true)]
        public int ID { get; set; }

        [LogEntryItem(0, IsDetail = true)]
        public string Type { get; set; }

        [LogEntryItem(IsHidden = true)]
        public string FullType { get; set; }

        [LogEntryItem(1, CellAlign = HorizontalAlign.Center)]
        public string IP { get; set; }

        [LogEntryItem(2, CellAlign = HorizontalAlign.Center)]
        public string UserName { get; set; }

        [LogEntryItem(1, IsDetail = true, IsLink = true)]
        public string Url { get; set; }

        [LogEntryItem(2, IsDetail = true, IsLink = true)]
        public string UrlReferrer { get; set; }

        [LogEntryItem(0, 0, ColumnSortOrder.Descending, DateStringFormat = "dd/MM/yyyy HH:mm", CellAlign = HorizontalAlign.Center)]
        public DateTime Date { get; set; }

        [LogEntryItem(IsHidden = true)]
        public string Entry { get; set; }

        public LogEntry()
        {
        }

        public LogEntry(int ID, string entry, string fullType)
        {
            Type type = this.GetType();
            if(!string.IsNullOrEmpty(fullType))
            {
                type = System.Type.GetType(fullType);
            }
            if(type != null)
            {
                try
                {
                    LogEntry tmp = (LogEntry)JsonConvert.DeserializeObject(entry, type, new JavaScriptDateTimeConverter());
                    if(tmp != null)
                    {
                        this.Type = tmp.Type;
                        this.FullType = fullType;
                        this.IP = tmp.IP;
                        this.UserName = tmp.UserName;
                        this.Url = tmp.Url;
                        this.UrlReferrer = tmp.UrlReferrer;
                        this.Date = tmp.Date;
                        this.Entry = entry;
                    }
                    this.ID = ID;
                }
                catch
                {
                }
            }
        }

        public LogEntry(string ip, string userName, string url, string urlReferrer, DateTime? date)
        {
            this.Type = this.GetType().Name;
            this.FullType = this.GetType().AssemblyQualifiedName;
            this.IP = ip;
            this.UserName = userName;
            this.Url = url;
            this.UrlReferrer = urlReferrer;
            this.Date = date.HasValue ? date.Value : DateTime.Now;
        }
    }

    public class LogsModel: ViewModel
    {
        private List<Type> logTypes = new List<Type>();

        public string TargetName { get; set; }

        public Type TargetType
        {
            get;
            private set;
        }

        public List<GridColumnModel> Columns = new List<GridColumnModel>();

        public List<Type> AvailableLogTypes = new List<Type>();

        /// <summary>
        /// All classes should be derived from Flexpage.Models.LogEntry
        /// </summary>
        public List<Object> Items = new List<Object>();

        public LogsModel(Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
            AvailableLogTypes = GetAllDerivedEntities(typeof(LogEntry));
            TargetName = "";
            TargetType = null;
        }

        public LogsModel(Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage, object source, params object[] args) : this(settings, flexpage)
        {
            Assign(source, args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args">First argument is prefilter and other Target full type</param>
        public override void Assign(object source, params object[] args)
        {
            base.Assign(source, args);

            if(source is LogsModel)
            {
                LogsModel sourceModel = source as LogsModel;
                this.TargetName = sourceModel.TargetName;
                if(args != null)
                {
                    if(args.Length >= 1)
                    {
                        TargetName = args[0].ToString().Trim();
                        //TargetType = LogEntryHelper.GetType(args[0].ToString().Trim());
                    }
                }
                SetTargetType(TargetName);
                if(TargetType == null)
                {
                    TargetName = "";
                }
            }
        }

        public override void Load(IFlexpageRepository repository, BlockCommandModel proto,string title="", bool needToLoadContent = true)
        {
            base.Load(repository, proto,title, needToLoadContent);

            if(TargetType != null)
            {
                logTypes = LogEntryHelper.GetEntryLogTypes(repository, TargetType);
                if((logTypes != null) && (logTypes.Count > 0))
                {
                    Assign(this);
                    populateColumns();

                    Items = LogEntryHelper.GetEntryLogs(repository, TargetType);
                }
            }
        }

        private void populateColumns()
        {
            // get properties
            List<PropertyInfo> properties = LogEntryHelper.GetDefaultProperties(TargetType);
            foreach(Type t in logTypes)
            {
                // add only new properties (with different name)
                properties.AddRange(from p in t.GetProperties()
                                    where properties.Select(x => x.Name).Contains(p.Name) == false
                                    select p);
            }

            // get different fields
            List<PropertyInfo> addProperties = new List<PropertyInfo>();
            foreach(PropertyInfo p in properties)
            {
                LogEntryItemAttribute attr = LogEntryItemAttribute.GetAttribute(p);
                if(!attr.IsHidden)
                {
                    GridColumnModel tmpAddColumn = new GridColumnModel()
                    {
                        Name = p.Name,
                        FieldName = p.Name,
                        ColumnCaption = Resources.Strings.ResourceManager.GetString("LogEntry_" + p.Name) ?? p.Name,
                        Order = attr.Order ?? 9999,
                        SortOrder = attr.SortOrder == ColumnSortOrder.Ascending ? DevExpress.Data.ColumnSortOrder.Ascending :
                            DevExpress.Data.ColumnSortOrder.Descending,
                        ShowInDetail = attr.IsDetail,
                        ShowAsLink = attr.IsLink,
                        TextFormat = attr.DateStringFormat
                    }; ;
                    Columns.Add(tmpAddColumn);
                }
            }

            Columns = Columns.OrderBy(u => u.Order).ToList();
        }

        public void SetTargetType()
        {
            TargetType = AvailableLogTypes.FirstOrDefault(u => u.FullName == TargetName);
        }

        public void SetTargetType(string targetType)
        {
            TargetName = targetType;
            SetTargetType();
        }
    }
}