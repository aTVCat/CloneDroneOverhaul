using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace OverhaulMod.UI
{
    public class UIDeveloperMenu : ModBehaviour
    {
        [ModSetting(ModSettingIDs.ENABLE_DEBUG_MENU, true)]
        public static bool EnableDebugMenu;

        private static readonly Dictionary<string, string> s_debugValues = new Dictionary<string, string>();
        private StringBuilder _stringBuilder;

        public static bool Enabled;

        public override void Start()
        {
            _stringBuilder = new StringBuilder();
        }

        public void OnGUI()
        {
            if (!ModBuild.IsDebugBuild || !EnableDebugMenu)
                return;

            GUILayout.BeginArea(new Rect(10f, 100f, 200f, 200f));
            if (s_debugValues == null || s_debugValues.Count == 0)
            {
                GUILayout.EndArea();
                return;
            }

            int index = 0;
            _ = _stringBuilder.Clear();
            foreach (KeyValuePair<string, string> keyValue in s_debugValues)
            {
                _ = _stringBuilder.Append(keyValue.Key);
                _ = _stringBuilder.Append(": ");
                _ = _stringBuilder.Append(keyValue.Value);
                if (index < s_debugValues.Count - 1)
                    _ = _stringBuilder.Append("\n");

                index++;
            }
            _ = GUILayout.TextArea(_stringBuilder.ToString());
            GUILayout.EndArea();
        }

        public override void Update()
        {
            if (!ModBuild.IsDebugBuild && !ModUserInfo.IsDeveloper)
                return;

            if (Input.GetKeyDown(KeyCode.Alpha7) && InputManager.Instance.GetKeyMode() != KeyMode.EditingInputField)
            {
                bool value = !Enabled;
                Enabled = value;
                if (value)
                {
                    if (!Cursor.visible || GameModeManager.IsOnTitleScreen())
                        _ = ModUIConstants.ShowDebugMenu();
                }
                else
                {
                    ModUIConstants.HideDebugMenu();
                }
            }
        }

        public static void SetKeyValue(string key, string value)
        {
            if (!ModBuild.IsDebugBuild)
                return;

            if (!s_debugValues.ContainsKey(key))
            {
                s_debugValues.Add(key, value);
                return;
            }
            s_debugValues[key] = value;
        }
    }
}
