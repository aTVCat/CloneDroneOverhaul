using CloneDroneOverhaul.V3Tests.Optimisation;
using System.Collections.Generic;
using UnityEngine;

namespace CloneDroneOverhaul.Patching.VisualFixes
{
    public class ObjectFixer
    {
        public static GameUIThemeData GameUIThemeData { get; private set; }
        public static readonly List<string> LowPriorityObjects = new List<string>
        {
            "HumanEnvoyShip",
            "Spidertron6000_TransportTube"
        };

        public static void FixObject(Transform transform, string id, Object instanceScript = null)
        {
            if (id == "objectPlacedInLevel")
            {
                if (GameModeManager.IsInLevelEditor())
                {
                    return;
                }

                ObjectPlacedInLevel objectPlacedInLevel = instanceScript as ObjectPlacedInLevel;
                string dispName = objectPlacedInLevel.GetDisplayName();
                // LODs
                if (OverhaulCacheManager.HasCached(objectPlacedInLevel.GetDisplayName() + "_LOD0"))
                {
                    if (!ObjectsLODController.LoDEnabled)
                    {
                        goto IL_0000;
                    }
                    bool lowPriority = LowPriorityObjects.Contains(dispName);
                    Material origMat = transform.GetComponent<Renderer>().material;
                    string lodBaseString = dispName + "_LOD";
                    GameObject[] gObjs = new GameObject[3];
                    for (int i = 0; i < 3; i++)
                    {
                        GameObject g = UnityEngine.Object.Instantiate(OverhaulCacheManager.GetCached<GameObject>(lodBaseString + i.ToString()));
                        g.GetComponent<Renderer>().material = origMat;
                        gObjs[i] = g;
                    }

                    if (dispName == "ibeam")
                    {
                        ObjectsLODController.AddLODGroup(transform.gameObject, gObjs, 15f);
                        goto IL_0000;
                    }
                    else if (dispName == "White_Pillar" || dispName == "ThinWhitePillar")
                    {
                        ObjectsLODController.AddLODGroup(transform.gameObject, gObjs, 75f);
                        goto IL_0000;
                    }

                    ObjectsLODController.AddLODGroup(transform.gameObject, gObjs, lowPriority ? 80f : 45f);
                }
            IL_0000:
                // Normal maps
                ReplaceMaterial component = transform.GetComponent<ReplaceMaterial>();
                if (objectPlacedInLevel.LevelObjectEntry.PathUnderResources == "Prefabs/LevelObjects/Primitives/Tile")
                {
                    foreach (Material material in component.MaterialOptions)
                    {
                        string name = material.name;
                        if (OverhaulCacheManager.HasCached("MatHeightMap_" + name))
                        {
                            material.mainTexture.filterMode = FilterMode.Point;
                            material.SetTexture("_OcclusionMap", OverhaulCacheManager.GetCached<Texture>("MatHeightMap_" + name));
                        }
                    }
                }
                else if (objectPlacedInLevel.LevelObjectEntry.PathUnderResources == "Prefabs/LevelObjects/Primitives/Ramp")
                {
                    foreach (Material material2 in component.MaterialOptions)
                    {
                        string name2 = material2.name;
                        if (OverhaulCacheManager.HasCached("MatHeightMap_" + name2))
                        {
                            material2.mainTexture.filterMode = FilterMode.Point;
                            material2.SetTexture("_OcclusionMap", OverhaulCacheManager.GetCached<Texture>("MatHeightMap_" + name2));
                        }
                    }
                }
                else if (objectPlacedInLevel.LevelObjectEntry.PathUnderResources == "Prefabs/LevelObjects/Story_Chapter4/HumanFleet/HumanBattleCruiser")
                {
                    component.MaterialOptions[0].SetTexture("_OcclusionMap", OverhaulCacheManager.GetCached<Texture>("MatHeightMap_HumanFleetShips"));
                }
                else if (objectPlacedInLevel.LevelObjectEntry.PathUnderResources == "Prefabs/LevelObjects/Story_Chapter4/Dark_Wall2")
                {
                    objectPlacedInLevel.GetComponent<MeshRenderer>().material.SetTexture("_OcclusionMap", OverhaulCacheManager.GetCached<Texture>("MatHeightMap_DarkHallwayParts"));
                }
                else if (objectPlacedInLevel.LevelObjectEntry.PathUnderResources == "Prefabs/LevelObjects/Story_Chapter4/Dark_Wall1")
                {
                    objectPlacedInLevel.GetComponent<MeshRenderer>().material.SetTexture("_OcclusionMap", OverhaulCacheManager.GetCached<Texture>("MatHeightMap_DarkHallwayParts"));
                }
                else if (objectPlacedInLevel.LevelObjectEntry.PathUnderResources == "Prefabs/LevelObjects/Story_Chapter4/Dark_Wall3")
                {
                    objectPlacedInLevel.GetComponent<MeshRenderer>().material.SetTexture("_OcclusionMap", OverhaulCacheManager.GetCached<Texture>("MatHeightMap_DarkHallwayParts"));
                }
                else if (objectPlacedInLevel.LevelObjectEntry.PathUnderResources == "Prefabs/LevelObjects/Story_Chapter4/Dark_Door")
                {
                    foreach (MeshRenderer meshRenderer in objectPlacedInLevel.GetComponentsInChildren<MeshRenderer>())
                    {
                        meshRenderer.material.SetTexture("_OcclusionMap", OverhaulCacheManager.GetCached<Texture>("MatHeightMap_DarkHallwayParts"));
                    }
                }
            }
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
                if (GameUIThemeData == null)
                {
                    GameUIThemeData = ui.GameThemeData;
                }

                ui.GameThemeData.ButtonBackground[0].Color = new Color(0.19f, 0.37f, 0.88f, 1);
                ui.GameThemeData.ButtonBackground[1].Color = new Color(0.3f, 0.5f, 1, 1f);
                if (ui.GetComponent<UnityEngine.UI.Image>() != null && ui.GetComponent<UnityEngine.UI.Image>().sprite != null && ui.GetComponent<UnityEngine.UI.Image>().sprite.name == "UISprite")
                {
                    ui.GetComponent<UnityEngine.UI.Image>().sprite = ModLibrary.AssetLoader.GetObjectFromFile<Sprite>("cdo_rw_stuff", "CanvasRoundedUnityDarkEdge");
                }
            }
            if (id == "FixPerformanceStats")
            {
                (transform as RectTransform).anchoredPosition = new Vector2(0, 15);
            }
            if (id == "FixEscMenu")
            {
                TransformUtils.HideAllChildren(transform);
                transform.GetComponent<UnityEngine.UI.Image>().enabled = false;
            }
            if (id == "Planet_Earth")
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
