using CDOverhaul.HUD.Overlays;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class OverhaulOverlays : OverhaulUI
    {
        public override void Initialize()
        {
            if (!OverhaulVersion.TechDemo2Enabled)
            {
                return;
            }

            MyModdedObject.GetObject<Transform>(0).gameObject.AddComponent<WideScreenOverlay>();

            base.transform.SetParent(GameUIRoot.Instance.transform);
            base.transform.SetAsFirstSibling();
            base.transform.localScale = Vector3.one;
            base.transform.localPosition = Vector3.zero;
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
        }
    }
}