using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DevExpress.Export.Xl;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using Pluritech.Pluriworks.Service.SelectColumn;
using Pluritech.Properties.Abstract.DTO;
using Pluritech.Contact.Abstract;
using Pluritech.Contact.Abstract.DTO;
using Enum = System.Enum;
using ObjectPropertyType = Pluritech.Properties.Abstract.DTO.ObjectPropertyType;
using TelecomType = Pluritech.Contact.Abstract.DTO.TelecomType;

namespace FlexPage2.Areas.Flexpage.Helpers
{
    public class ExportContactsHelper
    {
        private readonly IFlexpageRepository _repository;
        private readonly IContactProvider _contactProvider;
        private XlCellFormatting _cellFormatting;
        private XlCellFormatting _headerFormatting;
        private XlCellFormatting _rowFormatting;
        private readonly BaseColumn _baseColumn;

        public ExportContactsHelper(IFlexpageRepository repository, IContactProvider contactProvider)
        {
            _repository = repository;
            _contactProvider = contactProvider;
            
            _baseColumn = new BaseColumn();

            _cellFormatting = new XlCellFormatting();
            _cellFormatting.Font = new XlFont();
            _cellFormatting.Font.Name = "Century Gothic";
            _cellFormatting.Font.SchemeStyle = XlFontSchemeStyles.None;

            _headerFormatting = new XlCellFormatting();
            _headerFormatting.CopyFrom(_cellFormatting);
            _headerFormatting.Font.Bold = true;
            _headerFormatting.Border = XlBorder.OutlineBorders(XlColor.FromArgb(0x0, 0x0, 0x0), XlBorderLineStyle.Thick);

            _rowFormatting = new XlCellFormatting();
            _rowFormatting.CopyFrom(_cellFormatting);
            _rowFormatting.Font.Bold = false;
            _rowFormatting.Border = XlBorder.OutlineBorders(XlColor.FromArgb(0x0, 0x0, 0x0), XlBorderLineStyle.Medium);
        }

        public MemoryStream Export(List<SelectColumnTreeList> selectedFields, List<ObjectEntity> dataExportPerson, List<ObjectEntity> dataExportCompany)
        {
            IXlExporter exporter = XlExport.CreateExporter(XlDocumentFormat.Xlsx);

            var stream = new MemoryStream(1024000);

            // Create a new document and begin to write it to the specified stream.  
            using (IXlDocument document = exporter.CreateDocument(stream))
            {
                // Add a new worksheet to the document.  
                using (IXlSheet sheet = document.CreateSheet())
                {
                    // Specify the worksheet name.  
                    sheet.Name = "Persons";
                    //add your code here to generate columns and cells  

                    var setColumns = new List<string>();
                    var inColumn = new Dictionary<string, int>();
                    var inRow = new List<string>();
                    var inColumnLinked = new Dictionary<string, int>();
                    var inRowLinked = new List<string>();

                    foreach (var field in selectedFields.Where(f => f.ParentType == ParentTypes.Person).ToList())
                    {
                        if (field.Name == _baseColumn.TelecomsChildColumns[0])
                        {
                            var fieldToModify = selectedFields.First(f => f.Id == field.Id);
                            var parent = selectedFields.First(s => s.Id == field.ParentId);
                            fieldToModify.IsSelect = true;
                            fieldToModify.Name = "Default " + parent.Name;
                            continue;
                        }
                        if (field.Name == _baseColumn.TelecomsChildColumns[1])
                        {
                            var parent = selectedFields.First(s => s.Id == field.ParentId);
                            if (parent.ParentId == 1)
                                inColumn[parent.Name] = 0;
                            else
                                inColumnLinked[parent.Name] = 0;
                            selectedFields.Remove(field);
                            continue;
                        }
                        if (field.Name == _baseColumn.TelecomsChildColumns[2])
                        {
                            var parent = selectedFields.First(s => s.Id == field.ParentId);
                            if (parent.ParentId == 1)
                                inRow.Add(parent.Name);
                            else 
                                inRowLinked.Add(parent.Name);
                            selectedFields.Remove(field);
                            continue;
                        }

                        if (_baseColumn.AddressColumns.Contains(field.Name))
                        {
                            var parent = selectedFields.First(s => s.Id == field.ParentId);
                            field.IsSelect = true;
                            field.Name = parent.Name + " " + field.Name;
                            parent.IsSelect = false;
                            continue;
                        }
                    }
                    
                    var itemsLinked = selectedFields.Where(w => w.ParentId == 2 || selectedFields.Any(s => s.ParentId == 1 && s.Id == w.ParentId)).ToList();

                    List<LinkedView> linkedPersons;
                    var dataExportPersonsLinked = new List<PersonShortcut>();
                    if (itemsLinked.Any())
                    {
                        linkedPersons = dataExportPerson.Select(p =>
                            _contactProvider.GetLinkedPersons(p.PersonShortcut.Person.ID)).SelectMany(x => x).ToList();
                        if (linkedPersons.Any())
                        {

                            var persons = _repository.GetPersons().ToList();
                            dataExportPersonsLinked =
                                persons.Where(w => linkedPersons.Any(l => l.LinkedContactShortcutID == w.ID)).ToList();
                        }
                    }

                    foreach (var column in inColumn.Keys.ToList())
                    {
                        var count = dataExportPerson.Select(p => p.PersonShortcut.Person.PersonTelecom
                                .Count(pt => pt.Telecom.TypeID == (int)Enum.Parse(typeof(TelecomType), column) && !pt.IsDefault)).DefaultIfEmpty().Max();
                            inColumn[column] = count;
                    }

                    foreach (var column in inColumnLinked.Keys.ToList())
                    {
                        var count = dataExportPersonsLinked.Select(p => p.Person.PersonTelecom
                            .Count(pt => pt.Telecom.TypeID == (int)Enum.Parse(typeof(TelecomType), column) && !pt.IsDefault)).DefaultIfEmpty().Max();
                        inColumn[column] = Math.Max(count, inColumn[column]);
                    }

                    foreach (var column in inColumn)
                    {
                        for (var i = 0; i < column.Value; i++)
                        {
                            selectedFields.Add(new SelectColumnTreeList()
                            {
                                IsSelect = true,
                                Name = column.Key + " " + i,
                                ParentType = ParentTypes.Person,
                                ParentId = 1
                            });
                        }
                    }

                    inRow.ForEach(r => selectedFields.Add(new SelectColumnTreeList()
                    {
                        IsSelect = true,
                        Name = r,
                        ParentType = ParentTypes.Person,
                        ParentId = 1
                    }));


                    inRowLinked.ForEach(r => selectedFields.Add(new SelectColumnTreeList()
                    {
                        IsSelect = true,
                        Name = r,
                        ParentType = ParentTypes.Person,
                        ParentId = 2
                    }));

                    var items = selectedFields.Where(w => w.ParentId == 1 || selectedFields.Any(s => s.ParentId == 1 && s.Id == w.ParentId)).ToList();
                    itemsLinked = selectedFields.Where(w => w.ParentId == 2 || selectedFields.Any(s => s.ParentId == 2 && s.Id == w.ParentId)).ToList();

                    foreach (var field in selectedFields.Where(f => f.ParentType == ParentTypes.Person))
                    {
                        if (setColumns.Contains(field.Name) || !field.IsSelect)
                            continue;

                        setColumns.Add(field.Name);

                        using (IXlColumn column = sheet.CreateColumn())
                        {
                            column.WidthInPixels = 250;
                        }
                    }
                    using (IXlRow row = sheet.CreateRow())
                    {
                        foreach (var field in setColumns)
                        {
                            using (IXlCell cell = row.CreateCell())
                            {
                                cell.Value = field;
                                cell.ApplyFormatting(_headerFormatting);
                            }
                        }
                    }


                    GeneratePersonFile(sheet, setColumns, items, itemsLinked, dataExportPerson, dataExportPersonsLinked, inRow, inRowLinked);
                }

                using (IXlSheet sheet = document.CreateSheet())
                {
                    // Specify the worksheet name.  
                    sheet.Name = "Companies";
                    //add your code here to generate columns and cells  
                    var setColumns = new List<string>();
                    var inColumn = new Dictionary<string, int>();
                    var inRow = new List<string>();
                    var inColumnLinked = new Dictionary<string, int>();
                    var inRowLinked = new List<string>();

                    foreach (var field in selectedFields.Where(f => f.ParentType == ParentTypes.Company).ToList())
                    {
                        if (field.Name == _baseColumn.TelecomsChildColumns[0])
                        {
                            var fieldToModify = selectedFields.First(f => f.Id == field.Id);
                            var parent = selectedFields.First(s => s.Id == field.ParentId);
                            fieldToModify.IsSelect = true;
                            fieldToModify.Name = "Default " + parent.Name;
                            continue;
                        }
                        if (field.Name == _baseColumn.TelecomsChildColumns[1])
                        {
                            var parent = selectedFields.First(s => s.Id == field.ParentId);
                            if (parent.ParentId == 1)
                                inColumn[parent.Name] = 0;
                            else
                                inColumnLinked[parent.Name] = 0;
                            selectedFields.Remove(field);
                            continue;
                        }
                        if (field.Name == _baseColumn.TelecomsChildColumns[2])
                        {
                            var parent = selectedFields.First(s => s.Id == field.ParentId);
                            if (parent.ParentId == 1)
                                inRow.Add(parent.Name);
                            else
                                inRowLinked.Add(parent.Name);
                            selectedFields.Remove(field);
                        }

                    }
                    var itemsLinked = selectedFields.Where(w => w.ParentId == 4 || selectedFields.Any(s => s.ParentId == 4 && s.Id == w.ParentId)).ToList();

                    List<LinkedView> linkedCompanies;
                    var dataExportCompaniesLinked = new List<CompanyShortcut>();
                    if (itemsLinked.Any())
                    {
                        linkedCompanies = dataExportCompany.Select(p =>
                            _contactProvider.GetLinkedPersons(null, p.CompanyShortcut.CompanyID)).SelectMany(x => x).ToList();
                        if (linkedCompanies.Any())
                        {

                            var companies = _repository.GetCompanies().ToList();
                            dataExportCompaniesLinked =
                                companies.Where(w => linkedCompanies.Any(l => l.LinkedContactShortcutID == w.ID)).ToList();
                        }
                    }

                    foreach (var column in inColumn.Keys.ToList())
                    {
                        var count = dataExportCompany.Select(p => p.CompanyShortcut.Company.CompanyTelecom
                                .Count(pt => pt.Telecom.TypeID == (int)Enum.Parse(typeof(TelecomType), column) && !pt.IsDefault)).DefaultIfEmpty().Max();
                        inColumn[column] = count;
                    }

                    foreach (var column in inColumnLinked.Keys.ToList())
                    {
                        var count = dataExportCompaniesLinked.Select(p => p.Company.CompanyTelecom
                            .Count(pt => pt.Telecom.TypeID == (int)Enum.Parse(typeof(TelecomType), column) && !pt.IsDefault)).DefaultIfEmpty().Max();
                        inColumn[column] = Math.Max(count, inColumn[column]);
                    }

                    foreach (var column in inColumn)
                    {
                        for (var i = 0; i < column.Value; i++)
                        {
                            selectedFields.Add(new SelectColumnTreeList()
                            {
                                IsSelect = true,
                                Name = column.Key + " " + i,
                                ParentType = ParentTypes.Company,
                                ParentId = 3
                            });
                        }
                    }

                    inRow.ForEach(r => selectedFields.Add(new SelectColumnTreeList()
                    {
                        IsSelect = true,
                        Name = r,
                        ParentType = ParentTypes.Company,
                        ParentId = 3
                    }));


                    inRowLinked.ForEach(r => selectedFields.Add(new SelectColumnTreeList()
                    {
                        IsSelect = true,
                        Name = r,
                        ParentType = ParentTypes.Company,
                        ParentId = 4
                    }));

                    var items = selectedFields.Where(w => w.ParentId == 3 || selectedFields.Any(s => s.ParentId == 3 && s.Id == w.ParentId)).ToList();
                    itemsLinked = selectedFields.Where(w => w.ParentId == 4 || selectedFields.Any(s => s.ParentId == 4 && s.Id == w.ParentId)).ToList();

                    foreach (var field in selectedFields.Where(f => f.ParentType == ParentTypes.Company))
                    {
                        if (setColumns.Contains(field.Name) || !field.IsSelect)
                            continue;

                        setColumns.Add(field.Name);

                        using (IXlColumn column = sheet.CreateColumn())
                        {
                            column.WidthInPixels = 250;
                        }
                    }
                    using (IXlRow row = sheet.CreateRow())
                    {
                        foreach (var field in setColumns)
                        {
                            using (IXlCell cell = row.CreateCell())
                            {
                                cell.Value = field;
                                cell.ApplyFormatting(_headerFormatting);
                            }
                        }
                    }


                    GenerateCompanyFile(sheet, setColumns, items, itemsLinked, dataExportCompany, dataExportCompaniesLinked, inRow, inRowLinked);
                }


            }

            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        private void GenerateCompanyFile(IXlSheet sheet,
            List<string> setColumns,
            List<SelectColumnTreeList> items,
            List<SelectColumnTreeList> itemsLink,
            List<ObjectEntity> dataExport, List<CompanyShortcut> dataExportCompaniesLinked, List<string> inRow,
            List<string> inRowLinked)
        {
            foreach (var objectEntity in dataExport)
            {
                if (inRow.Any())
                {
                    int rowIndex = 1;
                    inRow.ForEach(r =>
                    {
                        var e = (int) Enum.Parse(typeof(TelecomType), r);
                        rowIndex = Math.Max(rowIndex,
                            _repository.GetTelecomsCompany(objectEntity.CompanyShortcut.CompanyID).Count(tp => tp.Telecom.TypeID == e && !tp.IsDefault));
                    });
                    for (int i = 0; i < rowIndex; i++)
                    {
                        using (IXlRow row = sheet.CreateRow())
                        {
                            foreach (var column in setColumns)
                            {
                                var foundItem = items.FirstOrDefault(f => f.Name == column);

                                if (foundItem != null)
                                    SetCompanyValue(row, objectEntity.CompanyShortcut.Company, foundItem.Name, i);
                                else
                                    SetCellValue(row, string.Empty);

                            }
                        }
                    }
                }
                else
                {
                    using (IXlRow row = sheet.CreateRow())
                    {
                        foreach (var column in setColumns)
                        {
                            var foundItem = items.FirstOrDefault(f => f.Name == column);

                            if (foundItem != null)
                                SetCompanyValue(row, objectEntity.CompanyShortcut.Company, foundItem.Name);
                            else
                                SetCellValue(row, string.Empty);

                        }
                    }
                }
            }

            foreach (var linkedCompany in dataExportCompaniesLinked)
            {
                if (inRowLinked.Any())
                {
                    int rowIndex = 1;
                    inRowLinked.ForEach(r =>
                    {
                        var e = (int) Enum.Parse(typeof(TelecomType), r);
                        rowIndex = Math.Max(rowIndex, 
                            _repository.GetTelecomsPerson(linkedCompany.CompanyID).Count(tp => tp.Telecom.TypeID == e && !tp.IsDefault));
                    });
                    for (int i = 0; i < rowIndex; i++)
                    {
                        using (IXlRow row = sheet.CreateRow())
                        {
                            foreach (var column in setColumns)
                            {
                                var foundItem = items.FirstOrDefault(f => f.Name == column);

                                if (foundItem != null)
                                    SetCompanyValue(row, linkedCompany.Company, foundItem.Name, i);
                                else
                                    SetCellValue(row, string.Empty);

                            }
                        }
                    }
                }
                else
                {
                    using (IXlRow row = sheet.CreateRow())
                    {
                        foreach (var column in setColumns)
                        {
                            var foundItem = items.FirstOrDefault(f => f.Name == column);

                            if (foundItem != null)
                                SetCompanyValue(row, linkedCompany.Company, foundItem.Name);
                            else
                                SetCellValue(row, string.Empty);

                        }
                    }
                }
            }
        }

        private void GeneratePersonFile(IXlSheet sheet,
            List<string> setColumns, List<SelectColumnTreeList> items,
            List<SelectColumnTreeList> itemsLink,
            List<ObjectEntity> dataExport, List<PersonShortcut> dataExportPersonsLinked, List<string> inRow,
            List<string> inRowLinked)
        {
            foreach (var objectEntity in dataExport)
            {
                if (inRow.Any())
                {
                    int rowIndex = 1;
                    inRow.ForEach(r =>
                        {
                            var e = (int)Enum.Parse(typeof(TelecomType), r);
                            rowIndex = Math.Max(rowIndex, 
                                _repository.GetTelecomsPerson(objectEntity.PersonShortcut.PersonID).Count(tp => tp.Telecom.TypeID == e && !tp.IsDefault));
                        });
                    for (int i = 0; i < rowIndex; i++)
                    {
                        using (IXlRow row = sheet.CreateRow())
                        {
                            foreach (var column in setColumns)
                            {
                                var foundItem = items.FirstOrDefault(f => f.Name == column);

                                if (foundItem != null)
                                    SetPersonValue(row, objectEntity.PersonShortcut.Person, foundItem.Name, i);
                                else
                                    SetCellValue(row, string.Empty);

                            }
                        }
                    }
                }
                else
                {
                    using (IXlRow row = sheet.CreateRow())
                    {
                        foreach (var column in setColumns)
                        {
                            var foundItem = items.FirstOrDefault(f => f.Name == column);

                            if (foundItem != null)
                                SetPersonValue(row, objectEntity.PersonShortcut.Person, foundItem.Name);
                            else
                                SetCellValue(row, string.Empty);

                        }
                    }
                }
            }

            foreach (var linkedPerson in dataExportPersonsLinked)
            {
                if (inRowLinked.Any())
                {
                    int rowIndex = 1;
                    inRowLinked.ForEach(r =>
                    {
                        var e = (int)Enum.Parse(typeof(TelecomType), r);
                        rowIndex = Math.Max(rowIndex,
                            _repository.GetTelecomsPerson(linkedPerson.PersonID).Count(tp => tp.Telecom.TypeID == e && !tp.IsDefault));
                    });
                    for (int i = 0; i < rowIndex; i++)
                    {
                        using (IXlRow row = sheet.CreateRow())
                        {
                            foreach (var column in setColumns)
                            {
                                var foundItem = items.FirstOrDefault(f => f.Name == column);

                                if (foundItem != null)
                                    SetPersonValue(row, linkedPerson.Person, foundItem.Name, i);
                                else
                                    SetCellValue(row, string.Empty);

                            }
                        }
                    }
                }
                else
                {
                    using (IXlRow row = sheet.CreateRow())
                    {
                        foreach (var column in setColumns)
                        {
                            var foundItem = items.FirstOrDefault(f => f.Name == column);

                            if (foundItem != null)
                                SetPersonValue(row, linkedPerson.Person, foundItem.Name);
                            else
                                SetCellValue(row, string.Empty);

                        }
                    }
                }
            }
        }


        private void SetPersonValue(IXlRow row, Person person, string columnName, int? rowIndex = null)
        {
            if (columnName.Equals("ID"))
                SetCellValue(row, person.ID);
            else if (columnName.Equals("Type"))
                SetCellValue(row, "----");
            else if (columnName.Equals("Title"))
                SetCellValue(row, person.Title);
            else if (columnName.Equals("Name"))
                SetCellValue(row, person.Name1);
            else if (columnName.Equals("Surname"))
                SetCellValue(row, person.Name2);
            else if (columnName.Equals("Language"))
                SetCellValue(row, person.ContactLanguage.Language.Name);
            else if (columnName.Equals("Login"))
                SetCellValue(row, person.WebLogin?.Login ?? string.Empty);
            else if (columnName.StartsWith("Default"))
                SetCellValue(row,
                    person.PersonTelecom?.FirstOrDefault(w =>
                        w.Telecom.TypeID == (int)Enum.Parse(typeof(TelecomType), columnName.Substring(8)) &&
                        w.IsDefault)?.Telecom.Value ?? string.Empty);
            else if (columnName.StartsWith("Primary Address"))
            {
                SetAddressValue(row, person, columnName, true);
            }
            else if (columnName.StartsWith("Secondary Address"))
            {
                SetAddressValue(row, person, columnName, false);
            }
            else if (_baseColumn.TelecomsNodes.Any(str =>
                columnName.StartsWith(str) && columnName.Length > str.Length))
            {
                var number = int.Parse(columnName.Substring(columnName.Length - 1));
                SetCellValue(row,
                    person.PersonTelecom?
                        .Where(w => !w.IsDefault && w.Telecom.TypeID == (int)Enum.Parse(typeof(TelecomType), columnName.Substring(0, columnName.Length - 2)) && !w.IsDefault)
                        .ToList()
                        .ElementAtOrDefault(number)?
                        .Telecom.Value ?? string.Empty);
            }
            else if (_baseColumn.TelecomsNodes.Any(str => str.Equals(columnName)) && rowIndex.HasValue)
            {
                SetCellValue(row,
                    person.PersonTelecom
                        ?.Where(w =>
                            !w.IsDefault && w.Telecom.TypeID == (int)Enum.Parse(typeof(TelecomType), columnName))
                        .ElementAtOrDefault(rowIndex.Value)?.Telecom.Value ?? string.Empty);
            }
            else if (columnName.StartsWith("Other."))
            {
                var propertyName = columnName.Substring(6);
                var value = person.PersonObject
                    .Object.ObjectPropertyValues.FirstOrDefault(opv => opv.ObjectProperty.Name == propertyName);
                string text = "";
                DateTime date;
                if (value != null && value.ObjectProperty.TypeID == (int)ObjectPropertyType.DateTime && DateTime.TryParse(value.Value, out date))
                    text = date.ToString("d");
                SetCellValue(row, text);
            }
            else
                SetCellValue(row, string.Empty);
        }

        private Address GetAddress(Person person, bool isPrimary, out string linkedFrom)
        {
            Address result = null;
            linkedFrom = "";
            if(isPrimary)
            {
                if (person.MainAddress != null)
                    result = person.MainAddress;
                else if(person.PersonAddress.Count > 0)
                {
                    var pa = person.PersonAddress.OrderBy(p => p.AddressID).FirstOrDefault();
                    if (pa != null)
                    {
                        result =  pa.Address;
                        if (pa.SourceContactShortcut?.CompanyShortcut != null)
                            linkedFrom = pa.SourceContactShortcut?.CompanyShortcut.Company.Name1 + ' ' +
                                         pa.SourceContactShortcut?.CompanyShortcut.Company.Name2;
                        if (pa.SourceContactShortcut?.PersonShortcut != null)
                            linkedFrom = pa.SourceContactShortcut?.PersonShortcut.Person.Name1 + ' ' +
                                         pa.SourceContactShortcut?.PersonShortcut.Person.Name2;
                    }
                }
            }
            else
            {
                if (person.SecondaryAddress != null)
                    result = person.SecondaryAddress;
                else if (person.PersonAddress.Count > 1)
                {
                    var pa = person.PersonAddress.OrderBy(p => p.AddressID).ElementAtOrDefault(1);
                    if (pa != null)
                    {
                        result = pa.Address;
                        if (pa.SourceContactShortcut?.CompanyShortcut != null)
                            linkedFrom = pa.SourceContactShortcut?.CompanyShortcut.Company.Name1 + ' ' +
                                         pa.SourceContactShortcut?.CompanyShortcut.Company.Name2;
                        if (pa.SourceContactShortcut?.PersonShortcut != null)
                            linkedFrom = pa.SourceContactShortcut?.PersonShortcut.Person.Name1 + ' ' +
                                         pa.SourceContactShortcut?.PersonShortcut.Person.Name2;
                    }
                }
            }
            return result;
        }

        private Address GetAddress(Company company, bool isPrimary, out string linkedFrom)
        {
            Address result = null;
            if (isPrimary)
            {
                if (company.MainAddress != null)
                    result = company.MainAddress;
                if (company.CompanyAddress.Count > 0)
                {
                    var ca = company.CompanyAddress.OrderBy(c => c.AddressID).FirstOrDefault();
                    if (ca != null)
                    {
                        result = ca.Address;
                        if (ca.SourceContactShortcut?.CompanyShortcut != null)
                            linkedFrom = ca.SourceContactShortcut?.CompanyShortcut.Company.Name1 + ' ' +
                                         ca.SourceContactShortcut?.CompanyShortcut.Company.Name2;
                        if (ca.SourceContactShortcut?.PersonShortcut != null)
                            linkedFrom = ca.SourceContactShortcut?.PersonShortcut.Person.Name1 + ' ' +
                                         ca.SourceContactShortcut?.PersonShortcut.Person.Name2;
                    }
                }
            }
            else
            {
                if (company.SecondaryAddress != null)
                    result = company.SecondaryAddress;
                if (company.CompanyAddress.Count > 1)
                {
                    var ca = company.CompanyAddress.OrderBy(c => c.AddressID).ElementAtOrDefault(1);
                    if (ca != null)
                    {
                        result = ca.Address;
                        if (ca.SourceContactShortcut?.CompanyShortcut != null)
                            linkedFrom = ca.SourceContactShortcut?.CompanyShortcut.Company.Name1 + ' ' +
                                         ca.SourceContactShortcut?.CompanyShortcut.Company.Name2;
                        if (ca.SourceContactShortcut?.PersonShortcut != null)
                            linkedFrom = ca.SourceContactShortcut?.PersonShortcut.Person.Name1 + ' ' +
                                         ca.SourceContactShortcut?.PersonShortcut.Person.Name2;
                    }
                }
            }

            if (result != null)
            {
                linkedFrom = result.CompanyAddress.FirstOrDefault(ca => ca.Company == company)?.SourceContactShortcut
                                 ?.CompanyShortcut?.Company.Name1
                             + ' ' + result.CompanyAddress.FirstOrDefault(ca => ca.Company == company)
                                 ?.SourceContactShortcut?.CompanyShortcut?.Company.Name2;
            }
            else
                linkedFrom = "";

            return result;
        }

        private void SetAddressValue(IXlRow row, Person person, string columnName, bool isPrimary)
        {
            string linkedFrom;
            var address = GetAddress(person, isPrimary, out linkedFrom);
            SetAddressValue(row, address, linkedFrom, columnName, isPrimary);
        }

        private void SetAddressValue(IXlRow row, Company company, string columnName, bool isPrimary)
        {
            string linkedFrom;
            var address = GetAddress(company, isPrimary, out linkedFrom);
            SetAddressValue(row, address, linkedFrom, columnName, isPrimary);
        }

        private void SetAddressValue(IXlRow row, Address address, string linkedFrom, string columnName, bool isPrimary)
        {
            if (address == null)
            {
                SetCellValue(row, string.Empty);
                return;
            }

            string fieldName = columnName.Substring(isPrimary ? _baseColumn.MainAddressNodes[0].Length + 1 : _baseColumn.MainAddressNodes[1].Length + 1);
            //"Street", "Zip", "City", "Country", "AddressDescription"
            if (fieldName == _baseColumn.AddressColumns[0])
            {
                SetCellValue(row, address.Street ?? "");
            }
            else if (fieldName == _baseColumn.AddressColumns[1])
            {
                SetCellValue(row, address.Zip ?? "");
            }
            else if (fieldName == _baseColumn.AddressColumns[2])
            {
                SetCellValue(row, address.City ?? "");
            }
            else if (fieldName == _baseColumn.AddressColumns[3])
            {
                SetCellValue(row, address.Country?.Name ?? "");
            }
            else if (fieldName == _baseColumn.AddressColumns[4])
            {
                SetCellValue(row, address.Description ?? "");
            }
            else if (fieldName == _baseColumn.AddressColumns[5])
            {
                SetCellValue(row, linkedFrom);
            }
            else
                SetCellValue(row, string.Empty);
        }

        private void SetCompanyValue(IXlRow row, Company company, string columnName, int? rowIndex = null)
        {
            if (columnName.Equals("ID"))
                SetCellValue(row, company.ID);
            else if (columnName.Equals("Type"))
                SetCellValue(row, "----");
            else if (columnName.Equals("FileAs"))
                SetCellValue(row, company.FileAs);
            else if (columnName.Equals("Name"))
                SetCellValue(row, company.Name1);
            else if (columnName.Equals("Surname"))
                SetCellValue(row, company.Name2);
            else if (columnName.Equals("Language"))
                SetCellValue(row, company.ContactLanguage.Language.Name);
            else if (columnName.StartsWith("Default"))
                SetCellValue(row, company.CompanyTelecom?.FirstOrDefault(w => w.Telecom.TypeID == (int)Enum.Parse(typeof(TelecomType), columnName.Substring(8)) && w.IsDefault)?.Telecom.Value ?? string.Empty);
            else if (columnName.StartsWith("Primary Address"))
            {
                SetAddressValue(row, company, columnName, true);
            }
            else if (columnName.StartsWith("Secondary Address"))
            {
                SetAddressValue(row, company, columnName, false);
            }
            else if (_baseColumn.TelecomsNodes.Any(str => columnName.StartsWith(str) && columnName.Length > str.Length))
            {
                var number = int.Parse(columnName.Substring(columnName.Length - 1));
                SetCellValue(row, company.CompanyTelecom?.Where(w => !w.IsDefault && w.Telecom.TypeID == (int)Enum.Parse(typeof(TelecomType), columnName.Substring(0, columnName.Length - 2)) && !w.IsDefault).ToList().ElementAtOrDefault(number)?.Telecom.Value ?? string.Empty);
            }
            else if (_baseColumn.TelecomsNodes.Any(str => str.Equals(columnName)) && rowIndex.HasValue)
            {
                SetCellValue(row, company.CompanyTelecom?.Where(w => !w.IsDefault && w.Telecom.TypeID == (int)Enum.Parse(typeof(TelecomType), columnName)).ElementAtOrDefault(rowIndex.Value)?.Telecom.Value ?? string.Empty);
            }
            else if (columnName.StartsWith("Other."))
            {
                var propertyName = columnName.Substring(6);
                SetCellValue(row, company.CompanyObject
                    .Object.ObjectPropertyValues.FirstOrDefault(opv => opv.ObjectProperty.Name == propertyName)
                    ?.Value ?? string.Empty);
            }
            else
                SetCellValue(row, string.Empty);
        }
        
        private void SetCellValue(IXlRow row, XlVariantValue value)
        {
            using (IXlCell cell = row.CreateCell())
            {
                cell.Value = value;
                cell.ApplyFormatting(_rowFormatting);
            }
        }
    }
}