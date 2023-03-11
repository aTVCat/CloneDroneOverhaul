using CDOverhaul.HUD.Overlays;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class OverhaulOverlays : OverhaulUI
    {
        public override void Initialize()
        {
            MyModdedObject.GetObject<Transform>(0).gameObject.AddComponent<WideScreenOverlay>();
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
        }
    }
}