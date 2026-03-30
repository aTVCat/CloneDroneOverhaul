using HarmonyLib;
using OverhaulMod.Engine;
using System.Collections.Generic;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(SocialButtonPanel))]
    internal static class SocialButtonPanel_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(SocialButtonPanel.ShowAndSelectFirstButton))]
        private static bool ShowAndSelectFirstButton_Prefix(SocialButtonPanel __instance)
        {
            if (__instance.gameObject == null || !__instance.gameObject) return false;

            __instance.InnerContainer.SetActive(true);

            SocialTabButton[] buttons = __instance.Buttons;
            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i]._panel == null) buttons[i].Initialize(__instance);
            }

            if (TitleScreenCustomizationManager.ShowSocialMediaPopups)
            {
                if (__instance._hasSelectedFirstButton) return false;
                __instance._hasSelectedFirstButton = true;

                foreach (SocialTabButton socialTabButton in __instance.Buttons)
                {
                    if (socialTabButton.WantsToBeShownFirst())
                    {
                        __instance.SelectSocialButton(socialTabButton);
                        return false;
                    }
                }

                List<SocialTabButton> list = new List<SocialTabButton>();
                foreach (SocialTabButton socialTabButton2 in __instance.Buttons)
                {
                    if (socialTabButton2.CanShowPanelOnStartup()) list.Add(socialTabButton2);
                }

                if (list.Count > 0)
                {
                    SocialTabButton socialTabButton3 = list[UnityEngine.Random.Range(0, list.Count)];
                    __instance.SelectSocialButton(socialTabButton3);
                }
            }
            else
            {
                __instance._hasSelectedFirstButton = false;
                foreach (SocialTabButton socialTabButton in __instance.Buttons)
                    socialTabButton.PopoutToShow.Hide();
            }
            return false;
        }
    }
}
