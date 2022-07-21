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
using Flexpage.Code.Helpers;
using Flexpage.Domain.Context;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using FlexPage2.Areas.Flexpage.Infrastructure;
using System.ComponentModel;
using DevExpress.Utils.Extensions;
using DevExpress.Web;
using Pluritech.Properties.Abstract.DTO;
using Pluritech.Contact.Abstract.DTO;
using Pluritech.Shared.Abstract.DTO;
using Flexpage.Domain.Search;
using Flexpage.Abstract;

namespace Flexpage.Models
{
    public class ContactDetailsModel : BlockModel
    {
        public int ObjectID => Company?.ShortcutID ?? Person?.ShortcutID ?? -1;

        public int? ContactID { get; set; }
        public string SourceBlockAlias { get; set; }
        public int SelectTabIndex { get; set; }
        public string SelectTab { get; set; }
        public eContactType ContactType { get; set; }
        public bool ShowGeneralInfo { get; set; }
        public bool ShowAddresses { get; set; }
        public bool ShowTelecoms { get; set; }
        public bool ShowBankAccounts { get; set; }
        public bool ShowLinkedPersons { get; set; }
        public bool ShowLinkedCompanies { get; set; }
        public bool ShowLinkedFolders { get; set; }
        public bool ShowCustomProperties { get; set; }
        public bool ShowAdministration { get; set; }
        /// <summary>
        /// "Can Edit" property of the block setting
        /// </summary>
        public bool AllowEdit { get; set; }
        /// <summary>
        /// Determines whether current user can edit selected person's password
        /// </summary>
        public bool AllowPasswordEdit { get; set; }
        /// <summary>
        /// Determines whether the model is in edit mode and current user can edit it
        /// </summary>
        public bool Edit { get; set; }
        public PersonViewModel PersonView { get; set; }
        public CompanyViewModel CompanyView { get; set; }
        public AdministrationView Administration { get; set; }
        public Person Person { get; set; }
        public Company Company { get; set; }
        public List<AddressView> Address { get; set; }
        public List<AddressView> AllAddressViews { get; set; }
        public string AllAddressViewsJson { get; set; }
        public List<TelecomView> Telecoms { get; set; }
        public List<BankAccountView> BankAccounts { get; set; }
        public List<LinkedView> LinkedPersons { get; set; }
        public List<LinkedView> LinkedCompanies { get; set; }
        public List<LinkedFolderView> LinkedFolders { get; set; }
        public List<CustomPropertyValueView> CustomPropertyValues { get; set; }
        public List<SelectListItem> Languages { get; set; }
        public List<RefCountry> Countries { get; set; }
        public List<Flexpage.Domain.Entities.TelecomType> TelecomTypes { get; set; }
        public List<RefBank> Banks { get; set; }
        public List<LinkedViewName> Persons { get; set; }
        public List<LinkedViewName> Companies { get; set; }
        public List<LinkType> LinkTypesPersons { get; set; }
        public List<LinkType> LinkTypesCompanies { get; set; }
        public List<ObjectProperty> ObjectProperties { get; set; }
        public List<LinkedFolderView> Folders { get; set; }
        public IFlexpageRepository Repository { get; set; }
        public List<GridColumnModel> Columns { get; set; }
        public List<Abstract.DTO.ColumnMapping> ColMappings { get; set; }
        public IEnumerable<Pluritech.Properties.Abstract.DTO.Property> AllowedPropertiesObjectLink { get; set; }
        public string Language { get; set; }

        public CustomPropertyModel CustomProperties { get; set; }

        public ContactDetailsModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
            Columns = new List<GridColumnModel>();
            ColMappings = new List<Abstract.DTO.ColumnMapping>();

            var filesQueryBuilder = new FilesQueryBuilder();
            var objectTypes = filesQueryBuilder.GetObjectType(ObjectTypeEnum.ObjectLink);

            foreach (var objectType in objectTypes)
            {
                ColMappings.Add(new Abstract.DTO.ColumnMapping()
                {
                    ShortName = "CustomProperty." + objectType.Name,
                    FullName = "CustomProperty." + objectType.Name,
                    Caption = objectType.Caption
                });
            }
            ColMappings.Add(new Abstract.DTO.ColumnMapping()
            {
                ShortName = "Address.LinkedContact",
                FullName = "Address.Linked Contact",
                Caption = "Linked Contact"
            });

            Language = _settings.GetCurrentOrDefaultLangCode();
        }
        #region Load
        public override void Load(IFlexpageRepository repository, BlockCommandModel proto, string title="", bool needToLoadContent=true)
        {
            base.Load(repository, proto, title, needToLoadContent);
            Repository = repository;
            ContactDetails block = repository.GetByBlockID<ContactDetails>(proto.ID);


            if (block == null)
                AssignDefaultValues();
            else
            {
                Assign(block);
                AssignColumns(repository);
                if (needToLoadContent)
                {
                }
            }
        }

        public void Load(IFlexpageRepository repository, BlockCommandModel proto, bool needToLoadContent,
            int? contactID,
            eContactType contactType, bool edit = false, int selectTabIndex = 0, string selectTab = "GeneralInfo",
            bool canEditPasswords = false)
        {
            SelectTabIndex = selectTabIndex;
            ContactID = contactID;
            ContactType = contactType;
            SelectTab = selectTab;

            Load(repository, proto, "", needToLoadContent);
            Edit = AllowEdit && edit;
            AllowPasswordEdit = canEditPasswords;

            LoadGeneralInfo(repository);        

            if (SelectTab == "Addresses")
            {
                LoadAddresses(Repository);
            }
            else if (SelectTab == "Telecoms")
            {
                LoadTelecoms(Repository);
            }
            else if (SelectTab == "BankAccounts")
            {
                LoadBankAccounts(Repository);
            }
            else if (SelectTab == "LinkedPersons")
            {
                LoadPersons(Repository);
            }
            else if (SelectTab == "LinkedCompanies")
            {
                LoadCompanies(Repository);
            }
            else if (SelectTab == "LinkedFolders")
            {
                LoadFolders(Repository);
            }
            else if (SelectTab == "CustomProperties" || SelectTab == "Customproperties")
            {
                LoadCustomProperties(Repository);
            }
            else if (SelectTab == "Administration"|| SelectTab == "Credentials")
            {
                SelectTab= "Administration";
                LoadAdministration(Repository);
            }
        }

        public void AddColumn()
        {
            Columns.Sort((x, y) => x.Order.CompareTo(y.Order));
            Columns.Add(new GridColumnModel() { Name = "New column", Order = Columns.Count });
        }

        public void RemoveColumn(IFlexpageRepository repository, string idx)
        {
            Columns.Sort((x, y) => x.Order.CompareTo(y.Order));
            try
            {
                // Skip over system columns. They dont show up in settings, therefore their ordering is not accounted for when "delete" button is pressed.

                var idInt = Convert.ToInt32(idx);
                while (Columns[idInt].IsSystem)
                {
                    idInt++;
                }
                var column = Columns[idInt];
                if (column.ID > 0)
                {
                    repository.DeleteEntity<GridColumn>(column.ID);
                    repository.ApplyChanges();
                }
                Columns.RemoveAt(idInt);
            }
            catch { }
        }

        public void LoadGeneralInfo(IFlexpageRepository repository)
        {
            Languages = new List<SelectListItem>();
            repository.GetContactLanguages().ForEach(l => 
            {
                Languages.Add(new SelectListItem() { Text = l.Language.Name, Value = l.Language.Name });
            });
            if (ContactType == eContactType.Company)
            {
                if (ContactID != null)
                {
                    var company = repository.GetGeneralInfoCompany((int)ContactID);
                    Company = company;
                    CompanyView = new CompanyViewModel()
                    {
                        ID = company.ID,
                        ShortcutID = company.ShortcutID,
                        Language = company.ContactLanguage.Language.Name,
                        Name1 = company.Name1,
                        Name2 = company.Name2,
                        TradeNumber = company.TradeNumber,
                        VAT = company.VAT,
                        Notes=company.Notes
                    };
                }
                else
                {
                    CompanyView = new CompanyViewModel()
                    {
                    };
                }
            }
            else
            {
                if (ContactID != null)
                {
                    var person = repository.GetGeneralInfoPerson((int)ContactID);
                    Person = person;
                    PersonView = new PersonViewModel()
                    {
                        ID = person.ID,
                        ShortcutID = person.ShortcutID,
                        Language = person.ContactLanguage.Language.Name,
                        Title = person.Title,
                        Name1 = person.Name1,
                        Name2 = person.Name2,
                        TradeNumber = person.TradeNumber,
                        VAT = person.VAT,
                        Notes = person.Notes
                    };
                }
                else
                {
                    PersonView = new PersonViewModel()
                    {
                    };
                }
            }
        }
        public void LoadAdministration(IFlexpageRepository repository)
        {
            if (ContactType == eContactType.Person)
            {
                Administration = new AdministrationView();
                if (Person.WebLogin != null)
                {
                    Administration.Login = Person.WebLogin.Login;
                    Administration.Password = "";
                }
            }
        }
        #endregion

        public virtual void AssignDefaultValues(params object[] args)
        {
            ShowGeneralInfo = true;
            ShowAddresses = true;
            ShowTelecoms = true;
            ShowBankAccounts = true;
            ShowLinkedPersons = true;
            ShowLinkedCompanies = true;
            ShowLinkedFolders = true;
            ShowCustomProperties = true;
            ShowAdministration = true;
            AllowEdit = false;
        }
        #region Apply
        public override object Apply(IFlexpageRepository repository, params object[] args)
        {
            ContactDetails contactDetails = repository.GetByBlockID<ContactDetails>(ID);
            if (contactDetails == null || Alias == repository.CreateNewAlias)
            {
                if (Alias == repository.CreateNewAlias)
                    Alias = null;

                contactDetails = repository.CreateNewContactDetails(Visible, CssClass, Alias);
                repository.AddBlockToBlockList(contactDetails.Block, BlocklistID, BlockAfter);
            }

            contactDetails.SourceBlockAlias = SourceBlockAlias;
            contactDetails.ShowGeneralInfo = ShowGeneralInfo;
            contactDetails.ShowAddresses = ShowAddresses;
            contactDetails.ShowTelecoms = ShowTelecoms;
            contactDetails.ShowBankAccounts = ShowBankAccounts;
            contactDetails.ShowLinkedPersons = ShowLinkedPersons;
            contactDetails.ShowLinkedCompanies = ShowLinkedCompanies;
            contactDetails.ShowLinkedFolders = ShowLinkedFolders;
            contactDetails.ShowCustomProperties = ShowCustomProperties;
            contactDetails.ShowAdministration = ShowAdministration;
            contactDetails.AllowEdit = AllowEdit;
            repository.ApplyChanges();
            ApplyColumns(repository);
            repository.ApplyChanges();
            return contactDetails;
        }
        public void ApplyColumns (IFlexpageRepository repository)
        {
            foreach (var column in Columns)
            {
                GridColumn target = repository.GetByID<GridColumn>(column.ID);
                if (target == null)
                {
                    target = repository.CreateGridColumn();
                }
                target.BlockID = ID;
                target.Name = column.Name;
                target.OnlyForAuthorized = column.OnlyForAuthorized;
                target.Order = column.Order;
                target.ShowAsLink = column.ShowAsLink;
                target.ShowInDetail = column.ShowInDetail;
                target.ShowInGrid = column.ShowInGrid;
                target.Alignment = column.Alignment;
                repository.ApplyChanges();
            }
            repository.ApplyChanges();
        }
        public object ApplyGeneralInfo(IFlexpageRepository repository, params object[] args)
        {
            if (Person != null && PersonView != null)
            {
                Person.Title = PersonView.Title ?? "";
                Person.Name1 = PersonView.Name1 ?? "";
                Person.Name2 = PersonView.Name2 ?? "";
                Person.TradeNumber = PersonView.TradeNumber ?? "";
                var language = repository.Languages.FirstOrDefault(l => l.Name == PersonView.Language);
                if (language != null)
                {
                    Person.LanguageID = language.ID;
                }
                Person.VAT = PersonView.VAT ?? "";
                Person.Notes = PersonView.Notes ?? "";
            }
            if (Company != null && CompanyView != null)
            {
                Company.Name1 = CompanyView.Name1 ?? "";

                Company.Name2 = CompanyView.Name2 ?? "";

                Company.TradeNumber = CompanyView.TradeNumber ?? "";
                var language = repository.Languages.FirstOrDefault(l => l.Name == CompanyView.Language);
                if (language != null)
                {
                    Company.LanguageID = language.ID;
                }

                Company.VAT = CompanyView.VAT ?? "";
                Company.Notes = CompanyView.Notes ?? "";
            }
            repository.ApplyChanges();
            return this;
        }
        #endregion
        #region Assign
        public override void Assign(object source, params object[] args)
        {
            ContactDetails contactDetails = source as ContactDetails;
            base.Assign(contactDetails.Block);
            SourceBlockAlias = contactDetails.SourceBlockAlias;
            ShowGeneralInfo = contactDetails.ShowGeneralInfo;
            ShowAddresses = contactDetails.ShowAddresses;
            ShowTelecoms = contactDetails.ShowTelecoms;
            ShowBankAccounts = contactDetails.ShowBankAccounts;
            ShowLinkedPersons = contactDetails.ShowLinkedPersons;
            ShowLinkedCompanies = contactDetails.ShowLinkedCompanies;
            ShowLinkedFolders = contactDetails.ShowLinkedFolders;
            ShowCustomProperties = contactDetails.ShowCustomProperties;
            ShowAdministration = contactDetails.ShowAdministration;
            AllowEdit = contactDetails.AllowEdit;
        }

        public void AssignColumns(IFlexpageRepository repository)
        {
            var source = repository.GetGridColumns(ID);
            Columns = source.Select(c => new GridColumnModel()
            {
                ID = c.ID,
                Name = c.Name,
                Alignment = c.Alignment,
                ShowInGrid = c.ShowInGrid,
                ShowInDetail = c.ShowInDetail,
                ShowAsLink = c.ShowAsLink,
                OnlyForAuthorized = c.OnlyForAuthorized,
                Order = c.Order
            }).ToList();
        }
        public void AssignGeneralInfo(PersonViewModel personViewModel, params object[] args)
        {
            if (Person!=null&& personViewModel != null&& personViewModel.Type== eContactType.Person)
            {
                PersonView = personViewModel;
            }
        }
        public void AssignGeneralInfo(CompanyViewModel companyViewModel, params object[] args)
        {
            if (  Company != null&& companyViewModel != null&& companyViewModel.Type== eContactType.Company)
            {
                CompanyView = companyViewModel;
            }
        }
        public void AssignAdministration(AdministrationView administrationViewModel, params object[] args)
        {
            if (administrationViewModel != null)
            {
                Administration.Login = administrationViewModel.Login;
                Administration.Password = administrationViewModel.Password;
            }
        }
        #endregion
        #region Address
        public void LoadAddresses(IFlexpageRepository repository)
        {
            Countries = repository.GetCountries();
            Address = new List<AddressView>();
            AllAddressViews = new List<AddressView>();
            ObjectEntity contactObject = null;

            if (Company != null)
            {
                Company.CompanyAddress.Where(ca => ca.Address.Void == false).ToList().ForEach(address =>
                {
                    if (Address.Find(el => el.ID == address.AddressID) == null)
                        Address.Add(new AddressView()
                        {
                            ID = address.AddressID,
                            Street = address.Address.Street,
                            City = address.Address.City,
                            CountryID = address.Address.CountryID,
                            Zip = address.Address.Zip,
                            Description = address.Address.Description,
                            IsAddressLinked = address.SourceContactID.HasValue,
                            CustomProperties = DALHelper.Context.ObjectPropertyValues
                                .Where(opv => opv.ObjectID == address.AddressID)
                                .Select(opv => new CustomPropertyInfo()
                                {
                                    Name = opv.ObjectProperty.Name,
                                    Type = (Pluritech.Properties.Abstract.DTO.ObjectPropertyType)opv.ObjectProperty.TypeID,
                                    Value = opv.Value
                                }).ToList(),
                            SourceContactName = address.SourceContactShortcut?.CompanyShortcut == null ? 
                                address.SourceContactShortcut?.PersonShortcut.Person.Name1 + ' ' + address.SourceContactShortcut?.PersonShortcut.Person.Name2 :
                                address.SourceContactShortcut?.CompanyShortcut.Company.Name1 + ' ' + address.SourceContactShortcut?.CompanyShortcut.Company.Name2
                        });
                });
                contactObject = Company.CompanyObject.Object;
            }
            if (Person != null)
            {
                Person.PersonAddress.Where(ca => ca.Address.Void == false).ToList().ForEach(address =>
                {
                    if (Address.Find(el => el.ID == address.AddressID) == null)
                        Address.Add(new AddressView()
                        {
                            ID = address.AddressID,
                            Street = address.Address.Street,
                            City = address.Address.City,
                            CountryID = address.Address.CountryID,
                            Zip = address.Address.Zip,
                            Description = address.Address.Description,
                            IsAddressLinked = address.SourceContactID.HasValue,
                            CustomProperties = DALHelper.Context.ObjectPropertyValues
                                .Where(opv => opv.ObjectID == address.AddressID)
                                .Select(opv => new CustomPropertyInfo()
                                {
                                    Name = opv.ObjectProperty.Name,
                                    Type = (Pluritech.Properties.Abstract.DTO.ObjectPropertyType)opv.ObjectProperty.TypeID,
                                    Value = opv.Value
                                }).ToList(),
                            SourceContactName = address.SourceContactShortcut?.CompanyShortcut == null ?
                            address.SourceContactShortcut?.PersonShortcut.Person.Name1 + ' ' + address.SourceContactShortcut?.PersonShortcut.Person.Name2 :
                            address.SourceContactShortcut?.CompanyShortcut.Company.Name1 + ' ' + address.SourceContactShortcut?.CompanyShortcut.Company.Name2
                        });
                });
                contactObject = Person.PersonObject.Object;
            }
            contactObject?.ParentLink.ForEach(l =>
            {
                if (l.LinkType.Transition == eTransitionType.FromParentToTarget ||
                    l.LinkType.Transition == eTransitionType.BothSides)
                {
                    switch (l.LinkType.TargetType.ID)
                    {
                        case (int)ObjectTypeEnum.CompanyObject:
                            l.TargetObject.CompanyShortcut.Company.CompanyAddress.ForEach(address => AllAddressViews.Add(new AddressView()
                            {
                                ID = address.AddressID,
                                Street = address.Address.Street,
                                City = address.Address.City,
                                CountryID = address.Address.CountryID,
                                Zip = address.Address.Zip,
                                Description = address.Address.Description,
                                SourceContactShortcutID = l.TargetID
                            }));
                            break;
                        case (int)ObjectTypeEnum.PersonObject:
                            l.TargetObject.PersonShortcut.Person.PersonAddress.ForEach(address => AllAddressViews.Add(new AddressView()
                            {
                                ID = address.AddressID,
                                Street = address.Address.Street,
                                City = address.Address.City,
                                CountryID = address.Address.CountryID,
                                Zip = address.Address.Zip,
                                Description = address.Address.Description,
                                SourceContactShortcutID = l.TargetID
                            }));
                            break;
                    }
                }
            });
            contactObject?.TargetLink.ForEach(l =>
            {
                if (l.LinkType.Transition == eTransitionType.FromTargetToParent ||
                    l.LinkType.Transition == eTransitionType.BothSides)
                {
                    switch (l.LinkType.ParentTypeID)
                    {
                        case (int)ObjectTypeEnum.CompanyObject:
                            l.ParentObject.CompanyShortcut.Company.CompanyAddress.ForEach(address => AllAddressViews.Add(new AddressView()
                            {
                                ID = address.AddressID,
                                Street = address.Address.Street,
                                City = address.Address.City,
                                CountryID = address.Address.CountryID,
                                Zip = address.Address.Zip,
                                Description = address.Address.Description,
                                SourceContactShortcutID = l.ParentID
                            }));
                            break;
                        case (int)ObjectTypeEnum.PersonObject:
                            l.ParentObject.PersonShortcut.Person.PersonAddress.ForEach(address => AllAddressViews.Add(new AddressView()
                            {
                                ID = address.AddressID,
                                Street = address.Address.Street,
                                City = address.Address.City,
                                CountryID = address.Address.CountryID,
                                Zip = address.Address.Zip,
                                Description = address.Address.Description,
                                SourceContactShortcutID = l.ParentID
                            }));
                            break;
                    }
                }
            });

            AllAddressViewsJson = ToJSON(AllAddressViews);
            Address = Address.OrderBy(a => a.ID).ToList();
        }
        #endregion

        #region Telecom
        public void LoadTelecoms(IFlexpageRepository repository)
        {
            TelecomTypes = repository.GetTelecomTypes();
            Telecoms = new List<TelecomView>();

            if (Company != null)
            {
                Company.CompanyTelecom.Where(ct=>ct.Telecom.Void==false).ToList().ForEach(telecom =>
                {
                    if (Telecoms.Find(el => el.ID == telecom.TelecomID) == null)
                        Telecoms.Add(new TelecomView() { ID = telecom.TelecomID, IsDefault = telecom.IsDefault, TypeID = telecom.Telecom.TypeID, Value = telecom.Telecom.Value, Description = telecom.Telecom.Description });
                });
            }
            if (Person != null)
            {
                Person.PersonTelecom.Where(ct => ct.Telecom.Void == false).ToList().ForEach(telecom =>
                {
                    if (Telecoms.Find(el => el.ID == telecom.TelecomID) == null)
                        Telecoms.Add(new TelecomView() { ID = telecom.TelecomID, IsDefault = telecom.IsDefault, TypeID = telecom.Telecom.TypeID, Value = telecom.Telecom.Value, Description = telecom.Telecom.Description });
                });
            }
            Telecoms= Telecoms.OrderBy(t => t.ID).OrderByDescending(a => a.IsDefault).ToList();
        }
        #endregion

        #region BankAccount
        public void LoadBankAccounts(IFlexpageRepository repository)
        {
            Banks = repository.GetBanks();
            BankAccounts = new List<BankAccountView>();

            if (Company != null)
            {
                Company.CompanyBankAccount.ToList().ForEach(bankAccount =>
                {
                    BankAccounts.Add(new BankAccountView() { ID = bankAccount.BankAccount.ID, BankID = bankAccount.BankAccount.BankID, Account = bankAccount.BankAccount.Account });
                });
            }
            if (Person != null)
            {
                Person.PersonBankAccount.ToList().ForEach(bankAccount =>
                {
                    BankAccounts.Add(new BankAccountView() { ID = bankAccount.BankAccount.ID, BankID = bankAccount.BankAccount.BankID, Account = bankAccount.BankAccount.Account });
                });
            }
        }
        public void AddBankAccount(BankAccountView view)
        {
            CheckBankAccount(view);
            var bankAccount = Repository.AddBankAccount(new Domain.Entities.BankAccount()
            {
                BankID = view.BankID,
                Account = view.Account
            }, Company?.ID, Person?.ID);
            
            Repository.ApplyChanges();
            view.ID = bankAccount.ID;
            BankAccounts.Add(view);
            BankAccounts.OrderBy(ba => ba.ID);
        }
        public void CheckBankAccount(BankAccountView view)
        {
            var fields = new List<string>();
            if (string.IsNullOrEmpty(view.Account))
            {
                fields.Add("Account");
            }
            if (view.BankID == 0)
            {
                fields.Add("Bank");
            }
            if (fields.Count() > 0)
            {
                throw new Exception("Fill in the required fields: " + string.Join(", ", fields));
            }
        }
        public void EditBankAccount(BankAccountView view)
        {
            CheckBankAccount(view);
            var bankAccount = Repository.GetBankAccount(view.ID);
            if (bankAccount == null)
            {
                throw new Exception("BankAccount not found ");
            }
            bankAccount.BankID = view.BankID;
            bankAccount.Account = view.Account ?? "";
            
            Repository.ApplyChanges();
            var old = BankAccounts.FirstOrDefault(a => a.ID == view.ID);
            if (old != null)
            {
                old.BankID = view.BankID;
                old.Account = view.Account;
            }
        }
        public void DeleteBankAccount(int ID)
        {
            Repository.DeleteBankAccount(ID);
            Repository.ApplyChanges();
            var bankAccount = BankAccounts.Find(a => a.ID == ID);
            BankAccounts.Remove(bankAccount);
        }
        #endregion

        #region LinkedPerson
        public void LoadPersons(IFlexpageRepository repository)
        {

            Persons = new List<LinkedViewName>();
            /*List<int> search = SearchPropertyQuery.SearchProperties(SearchPropertyTypeType.Person,
                 new FilterDesciption() { Field = "Search", Value = "", FilterMode = eFilterMode.Contains }).Select(s => s.ObjectID).ToList();
            var contacts = search.ToList();
            */
            var filterBy = new List<FilterDesciption>();            
            filterBy.Add(new FilterDesciption()
            {
                Field = eContactType.Person.ToString(),
                Table = "ShowContacts",
                Value = 1
            });
            repository.QueryContacts.FilterBy(filterBy).ToList().ForEach(c =>
            {
                var id = c.Properties.FirstOrDefault(l => l.Name == "PersonCompany.ID")?.Value;
                var firstName = c.Properties.FirstOrDefault(l => l.Name == "PersonCompany.FirstName")?.Value;
                var lastName = c.Properties.FirstOrDefault(l => l.Name == "PersonCompany.LastName")?.Value;
                var voidF = c.Properties.FirstOrDefault(l => l.Name == "PersonCompany.Void")?.Value;
                if ((ContactID != (int)id || Company != null) && (bool)voidF == false)
                {
                    Persons.Add(new LinkedViewName()
                    {
                        LinkID = (int)id,
                        LinkShortcutID = (int)c.ID,
                        LinkName = firstName + " " + lastName
                    });
                }
            });
            LinkedPersons = new List<LinkedView>();

            if (Person != null)
            {
                LinkTypesPersons = Repository.GetLinkTypes().Where(lt => lt.ParentTypeID == (int)ObjectTypeEnum.PersonObject && lt.TargetTypeID == (int)ObjectTypeEnum.PersonObject).ToList();
                //LinkedPersons.AddRange(repository.GetLinkedPersons(Person.ID));
            }
            if (Company != null)
            {
                LinkTypesPersons = Repository.GetLinkTypes().Where(lt => lt.ParentTypeID == (int)ObjectTypeEnum.PersonObject && lt.TargetTypeID == (int)ObjectTypeEnum.CompanyObject
                || lt.TargetTypeID == (int)ObjectTypeEnum.PersonObject && lt.ParentTypeID == (int)ObjectTypeEnum.CompanyObject).ToList();
                //LinkedPersons.AddRange(repository.GetLinkedPersons(null, Company.ID));
            }
        }
        public void CheckLinked(LinkedView view)
        {
            var fields = new List<string>();
            /*if (view.CanEdit == 0)
            {
                throw new Exception("no editing allowed");
            }*/
            if (view.LinkedContactID == 0)
            {
                fields.Add("Name");
            }
            if (view.LinkTypeID == 0)
            {
                fields.Add("LinkType");
            }
            if (fields.Count() > 0)
            {
                throw new Exception("Fill in the required fields: " + string.Join(", ", fields));
            }
            // this should be checked at business layer
            /*
            if (LinkedPersons!=null && LinkedPersons.Find(lp => lp.ContactID == view.ContactID && lp.LinkTypeID == view.LinkTypeID) != null)
            {
                throw new Exception("Person already added");
            }
            if (LinkedCompanies != null && LinkedCompanies.Find(lp => lp.ContactID == view.ContactID && lp.LinkTypeID == view.LinkTypeID) != null)
            {
                throw new Exception("Companies already added");
            }*/
        }
        public void AddLinkedPerson(LinkedView view)
        {
            view.CanEdit = 1;
            LinkedPersons.Add(view);
            LinkedPersons.OrderBy(p => p.ID);
        }
        public void EditLinkedPerson(int oldID,LinkedView view)
        {
            view.CanEdit = 1;
            LinkedPersons.RemoveAll(t => t.ID == oldID);
            LinkedPersons.Add(view);
            LinkedPersons.OrderBy(c => c.ID);
        }
        public void DeleteLinkedPerson(int ID)
        {
            LinkedPersons.RemoveAll(f => f.ID == ID);
        }

        #endregion

        #region LinkedCompany
        public void LoadCompanies(IFlexpageRepository repository)
        {
            
            LinkedCompanies = new List<LinkedView>();
            Companies = new List<LinkedViewName>();
            /*
            List<int> search = SearchPropertyQuery.SearchProperties(SearchPropertyTypeType.Company,
                new FilterDesciption() { Field = "Search", Value = "", FilterMode = eFilterMode.Contains }).Select(s => s.ObjectID).ToList();            
            var contacts = search.ToList();
            */
            var filterBy = new List<FilterDesciption>();
            filterBy.Add(new FilterDesciption()
            {
                Field = eContactType.Company.ToString(),
                Table = "ShowContacts",
                Value = 2
            });
            repository.QueryContacts.FilterBy(filterBy).ToList().ForEach(c =>
            {
                var id = c.Properties.FirstOrDefault(l => l.Name == "PersonCompany.ID")?.Value;
                var firstName = c.Properties.FirstOrDefault(l => l.Name == "PersonCompany.FirstName")?.Value;
                var lastName = c.Properties.FirstOrDefault(l => l.Name == "PersonCompany.LastName")?.Value;
                var voidF = c.Properties.FirstOrDefault(l => l.Name == "PersonCompany.Void")?.Value;
                if ((ContactID != (int)id || Person != null) && (bool)voidF == false)
                {
                    Companies.Add(new LinkedViewName()
                    {
                        LinkID = (int)id,
                        LinkShortcutID = (int)c.ID,
                        LinkName = firstName + " " + lastName
                    });
                }
            });

            Persons = new List<LinkedViewName>();
            repository.GetContactsQueryPersons(false, false).ToList().ForEach(c =>
            {
                Persons.Add(new LinkedViewName()
                {
                    LinkShortcutID = c.ShortcutID,
                    LinkID = (int)c.ID,
                    LinkName = c.Name1 + ", " + c.Name2,
                });
            });
           
            if (Person != null)
            {
                LinkTypesCompanies = Repository.GetLinkTypes().Where(lt => lt.ParentTypeID == (int)ObjectTypeEnum.CompanyObject&& lt.TargetTypeID== (int)ObjectTypeEnum.PersonObject 
                || lt.TargetTypeID == (int)ObjectTypeEnum.CompanyObject && lt.ParentTypeID== (int)ObjectTypeEnum.PersonObject).ToList();
                LinkedCompanies.AddRange(repository.GetLinkedCompanies(Person.ID));
            }
            if (Company != null)
            {
                LinkTypesCompanies = Repository.GetLinkTypes().Where(lt => lt.ParentTypeID == (int)ObjectTypeEnum.CompanyObject && lt.TargetTypeID == (int)ObjectTypeEnum.CompanyObject).ToList();
                LinkedCompanies.AddRange(repository.GetLinkedCompanies(null,Company.ID));
            }
            
        }

        public void GetLinkedObjectShortcut(LinkedView view, eContactType type) {
            if(type == eContactType.Company)
                view.LinkedContactShortcutID = Companies.Find(lc => lc.LinkID == view.LinkedContactID).LinkShortcutID;
            if(type == eContactType.Person)
                view.LinkedContactShortcutID = Persons.Find(lc => lc.LinkID == view.LinkedContactID).LinkShortcutID;
        }

        public void AddLinkedCompany(LinkedView view)
        {
            view.CanEdit = 1;
            LinkedCompanies.Add(view);
            LinkedCompanies.OrderBy(c => c.ID);
        }
        
        public void EditLinkedCompany(int oldID, LinkedView view)
        {
            view.CanEdit = 1;
            LinkedCompanies.RemoveAll(t => t.ID == oldID);
            LinkedCompanies.Add(view);
            LinkedCompanies.OrderBy(c => c.ID);
        }
        public void DeleteLinkedCompany(int ID)
        {
            LinkedCompanies.RemoveAll(f => f.ID == ID);
        }
        #endregion

        #region CustomProperty
        public void LoadCustomProperties(IFlexpageRepository repository)
        {
            
            CustomPropertyValues = new List<CustomPropertyValueView>();
            ObjectProperties = Repository.GetObjectProperties().Where(op => op.Type.ID != 1).ToList();
            int? id = (Company != null) ? Company.ShortcutID : (Person?.ShortcutID);
            if (id != null)
            {
                repository.GetObjectPropertiesValues().Where(v => v.ObjectID == id).ToList().ForEach(val =>
                {
                    if (CustomPropertyValues.Find(el => el.ID == val.ID) == null)
                        CustomPropertyValues.Add(new CustomPropertyValueView()
                        {
                            ID = val.ID,
                            PropertyID = val.ObjectPropertyID,
                            Value = val.Value
                        });
                });
            }
        }
        public void AddCustomProperty(CustomPropertyValueView view)
        {
            if (Person != null)
            {
                var objectPropertyValue = Repository.AddCustomProperty(new Domain.Entities.ObjectPropertyValue()
                {
                    Value = view.Value,
                    ObjectPropertyID = view.PropertyID,
                    ObjectID = (int)Person.ShortcutID,
                });
                Repository.ApplyChanges();
                view.ID = objectPropertyValue.ID;
                CustomPropertyValues.Add(view);
            }
            CustomPropertyValues.OrderBy(p => p.ID);
        }
        public void EditCustomProperty(CustomPropertyValueView view)
        {
            var objectPropertyValue = Repository.GetCustomProperty(view.ID);

            objectPropertyValue.Value = view.Value ?? "";
            objectPropertyValue.ObjectPropertyID = view.PropertyID;

            Repository.ApplyChanges();
            var old = CustomPropertyValues.FirstOrDefault(a => a.ID == view.ID);
            if (old != null)
            {
                old.Value = view.Value ?? "";
                old.PropertyID = view.PropertyID;
            }
        }
        #endregion

        #region LinkedFolders
        public void LoadFolders(IFlexpageRepository repository)
        {
            Folders = repository.GetLinkedFolders().ToList();
            if (Person != null)
            {
                LinkedFolders= repository.GetLinkedFoldersPerson(Person.ID).ToList();
            }
            if (Company != null)
            {
                LinkedFolders = repository.GetLinkedFoldersCompany(Company.ID).ToList();
            }
        }

        public void CheckLinkedFolder(LinkedFolderView view)
        {
            var fields = new List<string>();
            //we choose folder from drop down list... we don't have name... only ID
            /*if (string.IsNullOrEmpty( view.Name))
            {
                fields.Add("Name");
            }*/
            if(view.ID == 0)
            {
                fields.Add("Name");
            }
            if (fields.Count() > 0)
            {
                throw new Exception("Fill in the required fields: " + string.Join(", ", fields));
            }
        }

        public void AddLinkedFolders(LinkedFolderView view)
        {
            CheckLinkedFolder(view);
            if (LinkedFolders.Find(lf => lf.ID == view.ID) != null)
            {
                throw new Exception("Folder exist!");
            }
            var Folder = Repository.GetFolders().ToList().Find(f => f.ID == view.ID);
            if (Company != null)
            {
                //Repository.AddObjectFolderCompany((int)Company.ID, view.ID, DALHelper.CurrentUserID);
                //var cs = Repository.GetCompanyShortcut(Folder.Object.ID);
                view.Name = Folder.Name;
                view.Type = LinkedFolderView.GetTypeFolder(Repository, Folder);
                view.TypeImg = LinkedFolderView.GetTypeFolderImg(view.Type);
                //Company.CompanyShortcut.Add(cs);
            }
            else if (Person != null)
            {
                //view.ID = 
                //Repository.AddObjectFolderPerson((int)Person.ID, view.ID, DALHelper.CurrentUserID);
                //var ps = Repository.GetPersonShortcut(Folder.Object.ID);
                view.Name = Folder.Name;
                view.Type = LinkedFolderView.GetTypeFolder(Repository, Folder);
                view.TypeImg = LinkedFolderView.GetTypeFolderImg(view.Type);
                //Person.PersonShortcut.Add(ps);
            }
            LinkedFolders.Add(view);
            LinkedFolders.OrderBy(f=>f.Name);
        }
        public void DeleteLinkedFolders(int folderID)
        {            
           
            /*if (Company != null)
            {
                Repository.DeleteObjectFolder(Company.ID,null, folderID);
                Repository.ApplyChanges();
            }
            else if (Person != null)
            {
                Repository.DeleteObjectFolder(null,Person.ID, folderID);
                Repository.ApplyChanges();
            }*/
               
            
            LinkedFolders.RemoveAll(f => f.ID == folderID);
        }
        #endregion
    }
}