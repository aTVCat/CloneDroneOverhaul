using UnityEngine;
using UnityEngine.UI;

namespace CloneDroneOverhaul.UI
{
    public class VisualEffectsUI : ModGUIBase
    {
        Image _vignette;

        public override void OnInstanceStart()
        {
            base.MyModdedObject = base.GetComponent<ModdedObject>();

            RectTransform vignette = MyModdedObject.GetObjectFromList<RectTransform>(0);
            vignette.SetParent(GameUIRoot.Instance.transform);
            vignette.SetAsFirstSibling();
            vignette.sizeDelta = Vector2.zero;
            vignette.localScale = Vector2.one;
            vignette.localPosition = Vector2.zero;
            _vignette = vignette.GetComponent<Image>();
        }

        public override void OnSettingRefreshed(string ID, object value, bool isRefreshedOnStart = false)
        {
            if (ID == "Graphics.Additions.Vignette")
            {
                _vignette.gameObject.SetActive((bool)value);
            }
        }
    }
}
