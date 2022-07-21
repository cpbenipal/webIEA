using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Linq;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Web.Mvc;
using Pluritech.FileSystem.Abstract;
using Pluritech.FileSystem.Abstract.DTO;

namespace Flexpage.Models
{
    public class RecycleBinModel
    {
        public GridViewModel GridModel { get; set; }

        private readonly IFileContentProcessor _fileService;

        public RecycleBinModel(IFileContentProcessor fileService)
        {
            _fileService = fileService;
            GridModel = new GridViewModel();
            GridModel.KeyFieldName = "Id";
            GridModel.Pager.PageSize = 25;
        }

        public void GetDataObjectsCount(GridViewCustomBindingGetDataRowCountArgs e)
        {
            var sortOrder = string.Join(", ", e.State.SortedColumns
                .Select(c => c.FieldName + " " + (c.SortOrder == ColumnSortOrder.Descending ? "DESC" : "ASC"))
                .ToArray());

            var criteria = CriteriaOperator.Parse(e.State.FilterExpression);
            var where = CriteriaToWhereClauseHelper.GetDataSetWhere(criteria);

            e.DataRowCount = _fileService.GetRecycles(0, 1000000000, sortOrder, where).Count();
        }

        public void GetDataObjects(GridViewCustomBindingGetDataArgs e)
        {
            var sortOrder = string.Join(", ",e.State.SortedColumns
                .Select(c => c.FieldName + " " + (c.SortOrder == ColumnSortOrder.Descending ? "DESC" : "ASC"))
                .ToArray());

            var criteria = CriteriaOperator.Parse(e.State.FilterExpression);
            var where = CriteriaToWhereClauseHelper.GetDataSetWhere(criteria);

            e.Data = _fileService.GetRecycles(e.StartDataRowIndex, e.DataRowCount, sortOrder, where);
        }


    }
}