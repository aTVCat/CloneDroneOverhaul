using System.Collections;
using UnityEngine;

namespace CDOverhaul.HUD.Overlays
{
    public class OverhaulOverlayBase : MonoBehaviour
    {
        private void Awake()
        {
            base.gameObject.SetActive(false);
        }
    }
}