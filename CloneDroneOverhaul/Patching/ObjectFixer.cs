using UnityEngine;

namespace CloneDroneOverhaul.Patching.VisualFixes
{
    public class ObjectFixer
    {
        public static GameUIThemeData GameUIThemeData { get; private set; }

        public static void FixObject(Transform transform, string id, Object instanceScript = null)
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
                if (GameUIThemeData == null) GameUIThemeData = ui.GameThemeData;
                ui.GameThemeData.ButtonBackground[0].Color = new Color(0.19f, 0.37f, 0.88f, 1);
                ui.GameThemeData.ButtonBackground[1].Color = new Color(0.3f, 0.5f, 1, 1f);
                if(ui.GetComponent<UnityEngine.UI.Image>() != null && ui.GetComponent<UnityEngine.UI.Image>().sprite != null && ui.GetComponent<UnityEngine.UI.Image>().sprite.name == "UISprite")
                {
                    ui.GetComponent<UnityEngine.UI.Image>().sprite = ModLibrary.AssetLoader.GetObjectFromFile<Sprite>("cdo_rw_stuff", "CanvasRoundedUnityDarkEdge");
                }
            }
            if (id == "FixPerformanceStats")
            {
                (transform as RectTransform).anchoredPosition = new Vector2(0, 15);
            }
            if(id == "FixEscMenu")
            {
                TransformUtils.HideAllChildren(transform);
                transform.GetComponent<UnityEngine.UI.Image>().enabled = false;
            }
            if(id == "Planet_Earth")
            {
                PlanetCollider __instance = instanceScript as PlanetCollider;
                __instance.gameObject.GetComponent<Rotator>().enabled = true;
                __instance.gameObject.GetComponent<Rotator>().SetRotationSpeed(new Vector3(0f, 0.25f, 0f));
                for (int i = 0; i < __instance.gameObject.transform.childCount; i++)
                {
                    __instance.gameObject.transform.GetChild(i).gameObject.SetActive(true);
                    __instance.gameObject.transform.GetChild(i).localPosition = new Vector3(0f, 0f, 0f);
                }
                for (int j = 0; j < __instance.gameObject.transform.childCount; j++)
                {
                    bool flag = __instance.gameObject.transform.GetChild(j).name == "Clouds_Pixel_Original" || __instance.gameObject.transform.GetChild(j).name == "Atmosphere (1)" || __instance.gameObject.transform.GetChild(j).name == "Clouds_Pixel (1)";
                    if (flag)
                    {
                        __instance.gameObject.transform.GetChild(j).gameObject.SetActive(false);
                    }
                    bool flag2 = __instance.gameObject.transform.GetChild(j).name == "Atmosphere_original";
                    if (flag2)
                    {
                        __instance.gameObject.transform.GetChild(j).localPosition = new Vector3(0f, 0.01f, 0f);
                    }
                    bool flag3 = __instance.gameObject.transform.GetChild(j).name == "OceanSphere";
                    if (flag3)
                    {
                        __instance.gameObject.transform.GetChild(j).localScale = new Vector3(29.5f, 29.5f, 29.5f);
                    }
                    bool flag4 = __instance.gameObject.transform.GetChild(j).name == "Atmosphere_Clouds";
                    if (flag4)
                    {
                        __instance.gameObject.transform.GetChild(j).localScale = new Vector3(32.2f, 32.2f, 32.2f);
                    }
                    bool flag5 = __instance.gameObject.transform.GetChild(j).name == "SphereClouds";
                    if (flag5)
                    {
                        __instance.gameObject.transform.GetChild(j).localScale = new Vector3(0.045f, 0.045f, 0.21f);
                        __instance.gameObject.transform.GetChild(j).localPosition = new Vector3(-0.005f, 0.005f, -0.005f);
                    }
                    bool flag6 = __instance.gameObject.transform.GetChild(j).name == "Continents";
                    if (flag6)
                    {
                        __instance.gameObject.transform.GetChild(j).localScale = new Vector3(0.95f, 0.99f, 0.95f);
                    }
                }
            }
            if (id == "PS4Cube")
            {
                MeshRenderer component = transform.GetComponent<MeshRenderer>();
                bool flag = component == null;
                if (!flag)
                {
                    Material material = component.material;
                    bool flag2 = material == null;
                    if (!flag2)
                    {
                        bool flag3 = material.name == "MindSpaceMaterial_EmperorCrown (Instance)";
                        if (flag3)
                        {
                            material.color = new Color(2f, 1.5f, 0.35f, 0.98f);
                        }
                        else
                        {
                            bool flag4 = material.name == "MindSpaceMaterial_EmperorFace (Instance)";
                            if (flag4)
                            {
                                material.color = new Color(0.5f, 0.76f, 1.8f, 0.89f);
                            }
                        }
                    }
                }
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
