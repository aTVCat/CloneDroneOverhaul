namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationControllerInfo
    {
        public PersonalizationController Reference;

        public PersonalizationItemInfo ItemInfo;

        public PersonalizationControllerInfo()
        {

        }

        public PersonalizationControllerInfo(PersonalizationController reference, PersonalizationItemInfo itemInfo)
        {
            Reference = reference;
            ItemInfo = itemInfo;
        }
    }
}
