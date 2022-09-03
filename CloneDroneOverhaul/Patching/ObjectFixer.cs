using UnityEngine;

namespace CloneDroneOverhaul.Patching.VisualFixes
{
    public class ObjectFixer
    {
        public static void FixObject(Transform transform, string id, object instanceScript = null)
        {
            if (id == "FixArmorPiece")
            {
                ArmorPiece ap = instanceScript as ArmorPiece;
                foreach (MeshRenderer renderer in transform.GetComponentsInChildren<MeshRenderer>())
                {
                    renderer.material.shader = Shader.Find("Standard");
                    renderer.material.renderQueue = 3001;
                    renderer.material.color = new Color(9f, 2f, 0.75f, 0.44f);
                    renderer.gameObject.AddComponent<ArmorAnimation>();
                }
                return;
            }
            if (id == "FixSelectableUI")
            {
                SelectableUI ui = instanceScript as SelectableUI;
                ui.GameThemeData.ButtonBackground[0].Color = new Color(0.19f, 0.37f, 0.88f, 1);
                ui.GameThemeData.ButtonBackground[1].Color = new Color(0.3f, 0.5f, 1, 1f);
            }
            if (id == "FixPerformanceStats")
            {
                (transform as RectTransform).anchoredPosition = new Vector2(0, 15);
            }
        }

        private class ArmorAnimation : MonoBehaviour
        {
            private readonly Color endColor = new Color(9f, 2f, 0.75f, 0.44f);
            private Renderer myRenderer;

            private void Awake()
            {
                myRenderer = GetComponent<Renderer>();
                myRenderer.material.color = Color.clear;
            }

            private void FixedUpdate()
            {
                Color color = myRenderer.material.color;
                color.r += (endColor.r - color.r) * 0.02f;
                color.g += (endColor.g - color.g) * 0.02f;
                color.b += (endColor.b - color.b) * 0.02f;
                color.a += (endColor.a - color.a) * 0.02f;
                myRenderer.material.color = color;
            }
        }
    }

}
