using System;

namespace CDOverhaul.Gameplay.QualityOfLife
{
    public class AdvancedPhotomodeRequireContentAttribute : Attribute
    {
        public string ContentID;

        public bool HasLoadedContent() => AdditionalContentLoader.HasLoadedContent(ContentID);

        public AdvancedPhotomodeRequireContentAttribute(string contentUniqueId)
        {
            ContentID = contentUniqueId;
        }
    }
}
