using ModLibrary;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul
{
    /// <summary>
    /// The exceptions that should occur only while testing!
    /// </summary>
    public static class OverhaulExceptions
    {
        public const string Prefix = "Overhaul mod: ";

        public const string Exc_SettingError = Prefix + "SettingError";
        public const string Exc_SettingSaveError = Prefix + "SettingSaveError";
        public const string Exc_SettingGetError = Prefix + "SettingGetError";

        public const string Exc_EventControllerUsedTooEarly = Prefix + "EventControllerUsedTooEarly";

        /// <summary>
        /// Just throw an exception
        /// </summary>
        /// <param name="exc"></param>
        /// <param name="message"></param>
        /// <exception cref="System.Exception"></exception>
        public static void ThrowException(in string exc, in string message = null)
        {
            if (message == null)
            {
                throw new System.Exception(exc);
            }
            throw new System.Exception(exc + " [" + message + "]");
        }

        /// <summary>
        /// Called when game crashed too early
        /// </summary>
        /// <param name="exc"></param>
        internal static void OnModEarlyCrash(in string exc)
        {
            OverhaulMod.IsCoreLoadedIncorrectly = true;
            ModdedObject obj = GameObject.Instantiate(AssetController.GetAsset<GameObject>("LoadErrorCanvas", OverhaulAssetsPart.Main)).GetComponent<ModdedObject>();
            obj.GetObject<Text>(0).text = exc;
            obj.GetObject<Button>(1).onClick.AddListener(delegate
            {
                TimeManager.Instance.OnGameUnPaused();
                GameUIRoot.Instance.ErrorWindow.Hide();
                ErrorManager.Instance.SetPrivateField<bool>("_hasCrashed", false);

                GameObject.Destroy(obj.gameObject);
            });
            _ = EnableCursorController.AddCondition();
        }
    }
}
