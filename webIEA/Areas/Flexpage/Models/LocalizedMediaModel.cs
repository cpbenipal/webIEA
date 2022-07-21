using System;
using System.ComponentModel.DataAnnotations;

namespace Flexpage.Models
{
    public class LocalizedMediaModel : ViewModel
    {
        public LocalizedMediaModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage) { }
        public int MediaID { get; set; }
        public int LanguageID { get; set; }
        public string Language { get; set; }

        [StringLength(1000)]
        public string MediaUrl { get; set; }
        [StringLength(1000)]
        public string ThumbUrl { get; set; }

        public string GetThumbUrl()
        {
            if (String.IsNullOrEmpty(ThumbUrl))
                return "/Areas/Flexpage/Content/Images/images/video.png";
            else
                return ThumbUrl;
        }

        public void Assign(LocalizedMediaModel source)
        {
            this.Language = source.Language;
            this.LanguageID = source.LanguageID;
            this.MediaID = source.MediaID;
            this.MediaUrl = source.MediaUrl;
            this.ThumbUrl = source.ThumbUrl;
        }
    }

}