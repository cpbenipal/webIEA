using System;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using Pluritech.Permissions.Abstract.DTO;
using Pluritech.Properties.Abstract.DTO;
using System.Collections.Generic;
using System.Linq;
using FlexPage2.Areas.Flexpage.Models.CustomPropertyAddModel;
using Newtonsoft.Json;
using Pluritech.Contact.Abstract.DTO;

namespace Flexpage.Models
{
    public enum ObjectPropertiesTypeEnum
    {
        Unknown,
        Folder,
        FileShortcut,
        CompanyObject,
        PersonObject,
        CompanyShortcut,
        PersonShortcut,
        ContactsShortcut,
        ObjectLink
    }
    public class ObjectPropertiesModel : ViewModel
    {
        
        public ObjectPermissionsModel Permissions { get; set; }
        public CustomPropertyModel Properties { get; set; }
        public List<PublicationModel> Publication { get; set; }
        public string Title { get; set; }
        public List<Folder> Folders { get; set; }
        public ContactsEnumerationModel ContactsEnumerationModel { get; set; }
        public bool IsSupportVersion { get; set; }
        public bool ShowAllAvailableProperties { get; set; } = false;
        public int SupportVersion { get; set; }
        public bool IsRecursive { get; set; }
        public ObjectPropertiesTypeEnum Type { get; set; }
        public string EditProperties { get; set; }
        private ObjectPropertiesRequest _propertiesRequest { get; set; }
        public string LanguageName { get; set; }
        public Folder FolderParent { get; set; }
        public string ObjectName { get; set; }
        public bool AllowCustomPropertiesEdit { get; set; }

        public Dictionary<string, string> LinkedShortcuts { get; set; } = new Dictionary<string, string>();

        public ObjectPropertiesModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
            Folders = new List<Folder>();
            ContactsEnumerationModel = new ContactsEnumerationModel(settings, flexpage);
            Publication = new List<PublicationModel>();
        }

        public override void Load(IFlexpageRepository repository, BlockCommandModel proto,string title, bool needToLoadContent = true)
        {
            ID = proto.ID;
            // proto.ID is the object ID
            Title = title;
            Folder everyone = new Folder();
            everyone.Name = "Everyone";
            Folders.Add(everyone);
            Folders.AddRange(repository.GetFoldersSecurityGroup().OrderBy(f => f.Name));
            switch (proto.Parameters)
            {
                case "file":
                    Type = ObjectPropertiesTypeEnum.FileShortcut;

                    var FileShortcut = repository.GetFileShortcuts().FirstOrDefault(p => p.ID == ID);
                    if (FileShortcut != null)
                    {
                        SupportVersion = FileShortcut.File.MaxVersions;
                        IsSupportVersion = FileShortcut.File.IsVersioned;
                        if (String.IsNullOrEmpty(Title)) {
                            Title = "Properties: " + FileShortcut.Name + FileShortcut.File.Extension;
                        }
                        LanguageName =
                            repository.Languages.FirstOrDefault(w => w.ID == FileShortcut.File.LanguageID)?.Name ??
                            string.Empty;

                        foreach (var item in FileShortcut.File.FileShortcuts)
                        {
                            foreach (var itemFolder in item.Object.Folders)
                            {
                                LinkedShortcuts[item.Name] = itemFolder.Name;
                            }
                        }
                        FolderParent = FileShortcut.Object.Folders.FirstOrDefault();
                        Console.WriteLine();
                    }
                    break;
                case "contact":
                    Type = ObjectPropertiesTypeEnum.ContactsShortcut;
                    break;
                default:
                    Type = ObjectPropertiesTypeEnum.Folder;

                    var folder = repository.GetFolders().FirstOrDefault(p => p.ID == ID);
                    if (folder != null)
                    {
                        SupportVersion = folder.MaxVersions;
                        IsSupportVersion = folder.IsVersioned;
                    }
                    break;
            }

            var alias = "_ObjectPropertiesVer2_";
            var blockContactsEnumeration = repository.GetByAlias<ContactEnumeration>("_ContactsEnumeration" + alias);

            if (blockContactsEnumeration == null)
            {
                ContactsEnumerationModel.AssignDefaultValues();
                ContactsEnumerationModel.Visible = true;
                ContactsEnumerationModel.Alias = "_ContactsEnumeration" + alias;
                ContactsEnumerationModel.ShowContacts = Pluritech.Contact.Abstract.DTO.eContactType.Person;
                blockContactsEnumeration = ContactsEnumerationModel.Apply(repository) as ContactEnumeration;
                repository.ApplyChanges();
                var columns = new List<GridColumn>();
                columns.Add(new GridColumn()
                {
                    Name = "Name",
                    Alignment = "Centered",
                    ShowInGrid = true,
                    ShowInDetail = false,
                    ShowAsLink = true,
                    OnlyForAuthorized = false,
                    Order = 0
                });
                ContactsEnumerationModel.ID = blockContactsEnumeration.BlockID;
                ContactsEnumerationModel.AssignColumns(columns);
                ContactsEnumerationModel.ApplyColumns(repository);
            }
            ContactsEnumerationModel.Load(repository,
                new BlockCommandModel(_settings) { BlockAlias = alias, ID = blockContactsEnumeration.BlockID },"",
                needToLoadContent);
            ContactsEnumerationModel.IsStatic = true;
        }

        public ObjectPropertiesRequest GetEditProperties()
        {
            if(string.IsNullOrEmpty(EditProperties))
                _propertiesRequest = new ObjectPropertiesRequest();
            else if (_propertiesRequest == null)
                _propertiesRequest = JsonConvert.DeserializeObject<ObjectPropertiesRequest>(EditProperties);

            return _propertiesRequest;
        }
    }
}