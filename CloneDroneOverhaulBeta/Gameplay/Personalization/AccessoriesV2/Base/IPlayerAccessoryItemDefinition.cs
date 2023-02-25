using OverhaulAPI;

namespace CDOverhaul.Gameplay
{
    public interface IPlayerAccessoryItemDefinition : IOverhaulItemDefinition
    {
        void SetModel(IPlayerAccessoryModel model);
        IPlayerAccessoryModel GetModel();

        void SetFilter(ItemFilter filter);
        ItemFilter GetFilter();

        void SetBodypartType(MechBodyPartType weaponType);
        MechBodyPartType GetBodypartType();
    }
}
