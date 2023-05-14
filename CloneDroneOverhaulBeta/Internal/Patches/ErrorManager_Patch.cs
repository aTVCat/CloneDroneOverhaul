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
            /*
            if (logString.Contains("ReplaceModelWithVariantMatching") || stackTrace.Contains("ReplaceModelWithVariantMatching"))
            {
                return false;
            }*/

            if (type == LogType.Error || type == LogType.Exception)
            {
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

                string report = "**" + logString + "**\n" + stackTrace;
                if (report.Length > 990)
                {
                    report = report.Remove(990);
                }
                report += string.Format("\n**GM: {0}, LevelID: {1}, GameVer: {2}, LangID: {3}, GameTime: {4}**", new object[] { gamemode, levelID, gameVer, langID, Time.timeSinceLevelLoad });

                OverhaulWebhooksController.ExecuteCrashReportsWebhook(report);
            }
            return true;
        }
    }
}
