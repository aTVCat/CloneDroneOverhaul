namespace OverhaulMod.Combat
{
    public class ModGameModifiersManager : Singleton<ModGameModifiersManager>
    {
        private bool m_forceEnableGreatSwords;
        public bool forceEnableGreatSwords
        {
            get
            {
                return m_forceEnableGreatSwords;
            }
            set
            {
                m_forceEnableGreatSwords = value;
            }
        }
    }
}
