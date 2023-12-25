using HarmonyLib;
using UnityEngine;

namespace OverhaulMod.Patches.Harmony
{
    [HarmonyPatch(typeof(PlanetCollider))]
    internal static class PlanetCollider_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("OnEnable")]
        private static void OnEnable_Postfix(PlanetCollider __instance)
        {
            if (!__instance.name.Contains("Earth"))
                return;

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
    }
}
