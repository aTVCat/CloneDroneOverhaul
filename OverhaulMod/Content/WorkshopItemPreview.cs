namespace OverhaulMod.Content
{
    public class WorkshopItemPreview
    {
        public string URL;

        public Steamworks.EItemPreviewType PreviewType;

        public WorkshopItemPreview(string url, Steamworks.EItemPreviewType previewType)
        {
            URL = url;
            PreviewType = previewType;
        }

        ~WorkshopItemPreview()
        {
            URL = null;
            PreviewType = default;
        }
    }
}
