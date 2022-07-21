using System.ComponentModel.DataAnnotations;

namespace FlexPage2.Areas.Flexpage.Models.Enums
{
    /// <summary>
    /// AdvertisementSize Type. Type showing block.
    /// </summary>
    public enum AdvertisementSizeType
    {
        Auto = 1,
        Full = 2,
        Custom = 3
    }
    /// <summary>
    /// AdvertisementSizeUnit Type. Type showing block.
    /// </summary>
    public enum AdvertisementSizeUnitType
    {
        [Display(Name = "px")]
        Pixel = 1,
        [Display(Name = "%")]
        Percentage = 2,
        [Display(Name = "em")]
        EM = 3,
        [Display(Name = "rem")]
        REM = 4
    }
}