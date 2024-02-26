using OverhaulMod.UI;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.Patches.Addons
{
    internal class MinorAddon : GameAddon
    {
        public override void Patch()
        {
            GameUIRoot gameUIRoot = ModCache.gameUIRoot;
            if (gameUIRoot)
            {
                CrosshairsUI crosshairsUI = gameUIRoot.CrosshairsUI;
                if (crosshairsUI && crosshairsUI.Child)
                {
                    CrosshairOffsetController crosshairOffsetController = crosshairsUI.Child.GetComponent<CrosshairOffsetController>();
                    if (!crosshairOffsetController)
                    {
                        _ = crosshairsUI.Child.AddComponent<CrosshairOffsetController>();
                    }
                }

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

            PhotoManager photoManager = PhotoManager.Instance;
            if (photoManager)
            {
                FlyingCameraController flyingCameraController = photoManager.CameraControllerPrefab;
                if (flyingCameraController)
                {
                    flyingCameraController.FieldOfViewMultiplier = -1000f;
                }
            }

            DirectionalLightManager directionalLightManager = DirectionalLightManager.Instance;
            if (directionalLightManager)
            {
                Light light = directionalLightManager.DirectionalLight;
                if (light)
                {
                    light.shadowNormalBias = 1.1f;
                    light.shadowBias = 1f;
                }
            }
        }
    }
}
