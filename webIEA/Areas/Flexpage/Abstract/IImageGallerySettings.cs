
namespace Flexpage.Abstract
{
    public interface IImageGallerySettings 
    {
        bool IsSlider { get; }
        bool AddWatermark { get; }
        string WatermarkText { get; }
        string WatermarkedImagesPath { get; }
        bool RenderThumbnailActualSize { get; }
        short ThumbnailWidth { get; }
        short ThumbnailHeight { get; }
        bool EnableClickToEnlarge { get; set; }
    }
}
