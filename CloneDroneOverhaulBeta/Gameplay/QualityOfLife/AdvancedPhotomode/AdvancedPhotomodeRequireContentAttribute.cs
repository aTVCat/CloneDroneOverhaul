using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
