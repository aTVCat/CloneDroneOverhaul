using CDOverhaul.Gameplay.Multiplayer;

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
