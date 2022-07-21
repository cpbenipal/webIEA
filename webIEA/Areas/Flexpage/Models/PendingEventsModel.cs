using Flexpage.Abstract;
using Flexpage.Code.StructureManagement;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using Flexpage.Domain.Enum;
using Flexpage.Helpers;
using Flexpage.Helpers.StructureManagement;
using Pluritech.Contact.Abstract.DTO;
using Pluritech.Pluriworks.Service.DTO;
using Pluritech.Shared.Abstract.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Flexpage.Models
{
   
    public class PendingEventsModel : ViewModel
    {
        public List< PublishingQueueFile> Files { get; set; }
        public List<PublishingQueueEmail> Emails { get; set; }
        public int Delay { get; set; }
        public bool Start { get; set; }
        public PendingEventsModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
            
        }
        
        public void Load(IFlexpageRepository repository, string alias,List<PublishingQueueFile> files, List<PublishingQueueEmail> emails,
            bool start, int delay, bool needToLoadContent = true)
        {
            Files = files;
            Emails = emails;
            Start = start;
            Delay = delay;
        }

        public override void Update()
        {
           
        }
        public void Assign(Maintenance source)
        {
           
        }
    }
}