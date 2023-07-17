using CDOverhaul.Gameplay;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul.HUD
{
    public class MountsPersonalizationPanel : OverhaulPersonalizationPanel
    {
        public override PersonalizationCategory GetCategory() => PersonalizationCategory.Mounts;

        protected override void PopulateItems()
        {

        }

        protected override IEnumerator populateItemsCoroutine()
        {
            IsPopulatingItems = true;
            yield break;
        }
    }
}
