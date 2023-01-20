using CloneDroneOverhaul.V3.HUD;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CloneDroneOverhaul.LevelEditor
{
    public class UILevelEditorV2 : V3_ModHUDBase
    {
        public static bool IsEnabled { get; private set; }

        /// <summary>
        /// Default color for selectable UI elements
        /// </summary>
        public static Color UIDefaultColor
        {
            get
            {
                ColorUtility.TryParseHtmlString("#31373F", out Color result);
                return result; //286ea03e-b667-46ae-8c12-95eb08c412e4
            }
        }

        /// <summary>
        /// Color for selected UI elements
        /// </summary>
        public static Color UISelectedColor
        {
            get
            {
                ColorUtility.TryParseHtmlString("#5B6D85", out Color result);
                return result;
            }
        }

        private bool _hasInitialized;
        public bool IsReadyToUse => _hasInitialized;

        /// <summary>
        /// All HUDs that level editor has
        /// </summary>
        private Dictionary<Type, UILevelEditorHUDBase> _HUDs = new Dictionary<Type, UILevelEditorHUDBase>()
        {
            { typeof(UIToolbar), null },
            { typeof(UILibrary), null },
        };

        private void Start()
        {
            base.gameObject.SetActive(false);
        }

        public override void OnSettingRefreshed(in string settingName, in object value)
        {
            if (settingName == "Levels.Editor.New Level Editor")
            {
                IsEnabled = (bool)value;
            }
        }

        public void Initialize()
        {
            if (MyModdedObject == null)
            {
                throw new NullReferenceException("Overhaul level editor: No base UI modded object found.");
            }

            _HUDs[typeof(UIToolbar)] = MyModdedObject.GetObjectFromList<Transform>(0).gameObject.AddComponent<UIToolbar>();
            _HUDs[typeof(UILibrary)] = MyModdedObject.GetObjectFromList<Transform>(1).gameObject.AddComponent<UILibrary>();

            base.gameObject.SetActive(true);

            _hasInitialized = true;
        }

        /// <summary>
        /// Get level editor HUD
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetHUD<T>() where T : UILevelEditorHUDBase
        {
            return (T)_HUDs[typeof(T)];
        }
    }
}
