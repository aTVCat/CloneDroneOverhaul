using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverhaulMod.Content
{
    public class ContentDownloadInfo
    {
        public string DisplayName;
        public long Size;

        public List<string> Files;

        public void FixValues()
        {
            if (Files == null)
                Files = new List<string>();
        }
    }
}
