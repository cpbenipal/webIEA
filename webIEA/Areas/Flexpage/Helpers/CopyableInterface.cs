namespace Flexpage.Models
{
    public interface ICopyable
    {
        void PasteAfterCopy(int blockID, int sourceBlocklistID, int targetBlocklistID);
    }
}