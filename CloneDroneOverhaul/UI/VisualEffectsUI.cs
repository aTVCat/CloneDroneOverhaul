using UnityEngine;
using UnityEngine.UI;

namespace CloneDroneOverhaul.UI
{
    public class VisualEffectsUI : ModGUIBase
    {
        // Token: 0x06000100 RID: 256 RVA: 0x00008204 File Offset: 0x00006404
        public override void OnInstanceStart()
        {
            base.MyModdedObject = base.GetComponent<ModdedObject>();
            RectTransform objectFromList = base.MyModdedObject.GetObjectFromList<RectTransform>(0);
            objectFromList.SetParent(Singleton<GameUIRoot>.Instance.transform);
            objectFromList.SetAsFirstSibling();
            objectFromList.sizeDelta = Vector2.zero;
            objectFromList.localScale = Vector2.one;
            objectFromList.localPosition = Vector2.zero;
            this._vignette = objectFromList.GetComponent<Image>();
        }

        // Token: 0x06000101 RID: 257 RVA: 0x00008277 File Offset: 0x00006477
        public override void OnSettingRefreshed(string ID, object value, bool isRefreshedOnStart = false)
        {
            if (ID == "Graphics.Additions.Vignette")
            {
                this._vignette.gameObject.SetActive((bool)value);
            }
        }

        // Token: 0x040000AD RID: 173
        private Image _vignette;
    }
}
