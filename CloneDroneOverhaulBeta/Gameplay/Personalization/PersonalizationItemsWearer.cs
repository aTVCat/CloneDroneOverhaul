using CDOverhaul.Gameplay.Multiplayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul.Gameplay
{
    public abstract class PersonalizationItemsWearer : OverhaulCharacterExpansion
    {
        public OverhaulPlayerInfo PlayerInformation
        {
            get => OverhaulPlayerInfo.GetOverhaulPlayerInfo(Owner);
        }

        public abstract void RefreshItems();
    }
}
