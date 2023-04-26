using UnityEngine.UI;

namespace CDOverhaul.NetworkAssets.AdditionalContent
{
    public class OverhaulAdditionalContentUIEntry : OverhaulBehaviour
    {
        private ModdedObject m_ModdedObject;
        private Image m_ProgressBar;

        public OverhaulAdditionalContentPackInfo MyPack;
        public bool PackIsntNull => MyPack != null;

        public bool ShouldShowProgressBar()
        {
            return PackIsntNull && MyPack.PackType == OverhaulAdditionalContentPackInfo.ContentPackType.Network;
        }

        public void Initialize(OverhaulAdditionalContentPackInfo pack)
        {
            MyPack = pack;
            m_ModdedObject = base.GetComponent<ModdedObject>();
            m_ProgressBar = m_ModdedObject.GetObject<Image>(3);

            RefreshDisplays();
        }

        protected override void OnDisposed()
        {
            m_ModdedObject = null;
            m_ProgressBar = null;

            MyPack = null;
        }

        public void RefreshDisplays()
        {
            m_ProgressBar.gameObject.SetActive(ShouldShowProgressBar());
        }
    }
}