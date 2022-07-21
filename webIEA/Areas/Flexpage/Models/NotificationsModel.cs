
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using Pluritech.Contact.Abstract;
using Pluritech.Contact.Abstract.DTO;
using Pluritech.Shared.Abstract.DTO;

namespace Flexpage.Models
{
    public class NotificationsModel : EmailOverridingModel
    {
        public Notification? Notification { get; set; }
        public NotificationsModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
        }

        public void Load(IFlexpageRepository repository, IContactProvider _contactProvider, BlockCommandModel proto,
            int contactID, int contactShortcutID, eContactType contactType, int folderID,Notification notification, string title = "", bool needToLoadContent = true)
        {
            Notification = notification;
            base.Load(repository, _contactProvider, proto, contactID, contactShortcutID, contactType, folderID, title, needToLoadContent);
        }

    }
}