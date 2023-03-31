using UnityEngine;

namespace CDOverhaul.HUD.Overlays
{
    public class OverhaulOverlays : OverhaulUI
    {
        public override void Initialize()
        {
            if (!OverhaulVersion.Upd3TechDemo2)
            {
                return;
            }

            _ = MyModdedObject.GetObject<Transform>(0).gameObject.AddComponent<WideScreenOverlay>();
            MyModdedObject.GetObject<Transform>(0).gameObject.SetActive(true);

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