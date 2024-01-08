using OverhaulMod.Utils;
using OverhaulMod.Visuals;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.Patches.Addons
{
    internal class MinorUIChangesAddon : GameAddon
    {
        public override void Patch()
        {
            var gameUIRoot = ModCache.gameUIRoot;
            if (gameUIRoot)
            {
                GameObject emoteSelectionUIObject = gameUIRoot.EmoteSelectionUI?.gameObject;
                if (emoteSelectionUIObject)
                {
                    Image image = emoteSelectionUIObject.GetComponent<Image>();
                    image.enabled = false;
                }

                GameObject controlMapperObject = gameUIRoot.ControlMapper?.gameObject;
                if (controlMapperObject)
                {
                    Transform canvasTransform = TransformUtils.FindChildRecursive(controlMapperObject.transform, "Canvas");
                    if (canvasTransform)
                    {
                        GameObject gameObject = new GameObject("Shading");
                        RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
                        rectTransform.anchorMax = Vector2.one;
                        rectTransform.anchorMin = Vector2.zero;
                        rectTransform.anchoredPosition = Vector2.zero;
                        rectTransform.SetParent(canvasTransform);
                        rectTransform.SetAsFirstSibling();
                        rectTransform.localScale = Vector3.one;
                        Image image = gameObject.AddComponent<Image>();
                        image.color = new Color(0f, 0f, 0f, 0.4f);
                    }
                }
            }
        }
    }
}
