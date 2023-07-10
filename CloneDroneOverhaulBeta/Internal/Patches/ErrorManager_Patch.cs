using CDOverhaul.HUD;
using HarmonyLib;
using UnityEngine;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(ErrorManager))]
    internal static class ErrorManager_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("HandleLog")]
        private static bool HandleLog_Prefix(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception)
            {
                string report = "**" + logString + "**\n" + stackTrace;
                if (OverhaulCrashPreventionController.TryPreventCrash(report) || OverhaulWebhooksController.HasExcludedError(report))
                    return false;

                OverhaulCrashScreen crashScreen = OverhaulCrashScreen.Instance;
                if (crashScreen)
                {
                    crashScreen.Show(logString, stackTrace);
                }

                string gamemode = string.Empty;
                string gameVer = string.Empty;
                string levelID = string.Empty;
                string langID = string.Empty;
                try
                {
                    gameVer = VersionNumberManager.Instance.GetVersionString();
                    gamemode = GameFlowManager.Instance.GetCurrentGameMode().ToString();
                    langID = LocalizationManager.Instance.GetCurrentLanguageCode();
                    levelID = LevelManager.Instance.GetCurrentLevelID();
                }
                catch
                {
                }

                if (report.Length > 1400)
                    report = report.Remove(1400);

                report += string.Format("\n**GM: {0}, LvlID: {1}, Ver: {2}, LangID: {3}, Time: {4}**", new object[] { gamemode, levelID, gameVer, langID, Mathf.RoundToInt(Time.timeSinceLevelLoad) });

                OverhaulWebhooksController.ExecuteCrashReportsWebhook(report);
            }
            return true;
        }
    }
}
