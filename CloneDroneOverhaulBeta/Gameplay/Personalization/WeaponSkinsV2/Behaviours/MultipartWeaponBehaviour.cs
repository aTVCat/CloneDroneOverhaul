using UnityEngine;

namespace CDOverhaul.Gameplay
{
    /// <summary>
    /// By Zerkie#5342
    /// </summary>
    public class MultipartWeaponBehaviour : WeaponSkinBehaviour
    {
        public override void OnBeginDraw()
        {
        }

        public override void OnDeath()
        {
        }

        public override void OnEndDraw()
        {
        }
        public override void OnSetColor(Color color)
        {
            base.OnSetColor(color);
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>(true))
            {
                Material mat = renderer.material;
                if (mat != null && mat.HasProperty("_EmissionColor"))
                {
                    mat.SetColor("_EmissionColor", color);
                }
            }
        }
    }
}
