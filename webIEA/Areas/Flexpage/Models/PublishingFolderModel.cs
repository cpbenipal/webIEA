using System;
using System.Collections.Generic;
using System.Linq;
using Flexpage.Domain.Abstract;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;

namespace Flexpage.Models
{
    public class PublishingFolderModel
    {
        public List<WebSiteModel> WebSiteModels { get; set; }

        public int ObjectPropertyID { get; set; }

        public int FolderID { get; set; }

        public string IsSelect { get; set; }

        public string IsDefault { get; set; }

        public string PropertyValue { get; set; }


        public PublishingFolderModel()
        {
            WebSiteModels = new List<WebSiteModel>();
        }

        public void Load(IFlexpageRepository repository, int objectPropertyID, int folderID)
        {
            ObjectPropertyID = objectPropertyID;
            FolderID = folderID;

            var entity = repository.GetObjectPropertiesValues().FirstOrDefault(m => m.ObjectPropertyID == objectPropertyID && m.Object.Folder.ID == FolderID);

            if (entity == null)
                return;

            repository.GetWebsites().ToList().ForEach(item => WebSiteModels.Add(new WebSiteModel() { Id = item.ID, Name = item.Name}));

            var tmpWebSite = new List<WebSiteModel>();

            try
            {
                tmpWebSite = JsonConvert.DeserializeObject<List<WebSiteModel>>(entity.Value);
            }
            catch (Exception)
            {
            }

            if (tmpWebSite==null||tmpWebSite.Count == 0)
                return;


            WebSiteModels.ForEach(w =>
            {
                var item = tmpWebSite.FirstOrDefault(p => p.Id == w.Id);
                if (item != null)
                {
                    w.Abbreviation = item.Abbreviation;
                    w.EmailNotification = item.EmailNotification;
                    w.IsSelect = item.IsSelect;
                    w.IsDefault = item.IsDefault;
                }
            });

            IsSelect = string.Join(",", WebSiteModels.Where(w => w.IsSelect).Select(s => s.Id).ToList());
            IsDefault = string.Join(",", WebSiteModels.Where(w => w.IsDefault).Select(s => s.Id).ToList());
            PropertyValue = JsonConvert.SerializeObject(WebSiteModels);
        }

        public void Save(IFlexpageRepository repository)
        {
            var entity = repository.GetObjectPropertiesValues().FirstOrDefault(m => m.ObjectPropertyID == ObjectPropertyID && m.Object.Folder.ID == FolderID);

            if (entity == null)
                return;

            var propertyValues = JsonConvert.DeserializeObject<List<WebSiteModel>>(this.PropertyValue);

            if(propertyValues.Count == 0)
                return;

            if (!string.IsNullOrEmpty(IsDefault) && int.TryParse(IsDefault.Split(',')[0], out var isDefault))
            {
                propertyValues.ForEach(item =>
                {
                    item.IsDefault = false;

                    if (item.Id == isDefault)
                        item.IsDefault = true;
                });
            }
            else
            {
                propertyValues.ForEach(item =>
                {
                    item.IsDefault = false;
                });
            }

            if (!string.IsNullOrEmpty(IsSelect))
            {
                var isSelectIds = IsSelect.Split(',');

                isSelectIds.ForEach(item =>
                {
                    if (int.TryParse(item, out var id))
                    {
                        propertyValues.ForEach(itemProp =>
                        {
                            if (itemProp.Id == id)
                                itemProp.IsSelect = true;
                        });
                    }
                });
            }
            else
            {
                propertyValues.ForEach(itemProp =>
                {
                    itemProp.IsSelect = false;
                });
            }


            var value = JsonConvert.SerializeObject(propertyValues);


            entity.Value = value;
            repository.ApplyChanges();
        }
    }

    public class WebSiteModel
    {
        public int Id { get; set; }

        [JsonIgnore]
        public string Name { get; set; }

        public bool IsSelect { get; set; }

        public bool IsDefault { get; set; }

        public Pluritech.Contact.Abstract.DTO.Notification EmailNotification { get; set; }

        public string Abbreviation { get; set; }
    }
}