using HarmonyLib;
using Newtonsoft.Json;
using OverhaulMod.Engine;
using System;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.Utils
{
    public static class ModCache
    {
        private static string[] s_commandLineArgs;
        public static string[] CommandLineArgs
        {
            get
            {
                if (s_commandLineArgs == null)
                {
                    s_commandLineArgs = Environment.GetCommandLineArgs();
                }
                return s_commandLineArgs;
            }
        }

        private static Assembly s_modAssembly;
        public static Assembly ModAssembly
        {
            get
            {
                if (s_modAssembly == null)
                {
                    s_modAssembly = Assembly.GetExecutingAssembly();
                }
                return s_modAssembly;
            }
        }

        private static AssemblyName s_modAssemblyName;
        public static AssemblyName ModAssemblyName
        {
            get
            {
                if (s_modAssemblyName == null)
                {
                    s_modAssemblyName = ModAssembly?.GetName();
                }
                return s_modAssemblyName;
            }
        }

        private static DataRepository s_dataRepository;
        public static DataRepository DataRepository
        {
            get
            {
                if (!s_dataRepository)
                {
                    s_dataRepository = DataRepository.Instance;
                }
                return s_dataRepository;
            }
        }

        private static JsonSerializerSettings s_jsonSerializerSettings;
        public static JsonSerializerSettings JsonSerializerSettings
        {
            get
            {
                if (s_jsonSerializerSettings == null)
                {
                    s_jsonSerializerSettings = DataRepository.GetSettings().Clone();
                }
                return s_jsonSerializerSettings;
            }
        }

        private static JsonSerializerSettings s_jsonSerializerSettingsFormatted;
        public static JsonSerializerSettings JsonSerializerSettingsFormatted
        {
            get
            {
                if (s_jsonSerializerSettingsFormatted == null)
                {
                    s_jsonSerializerSettingsFormatted = DataRepository.GetSettings().Clone();
                    s_jsonSerializerSettingsFormatted.Formatting = Formatting.Indented;
                }
                return s_jsonSerializerSettingsFormatted;
            }
        }

        private static Encoding s_utf8Encoding;
        public static Encoding UTF8Encoding
        {
            get
            {
                if (s_utf8Encoding == null)
                {
                    s_utf8Encoding = Encoding.UTF8;
                }
                return s_utf8Encoding;
            }
        }

        private static GameUIRoot s_uiRoot;
        public static GameUIRoot UIRoot
        {
            get
            {
                if (!s_uiRoot)
                {
                    s_uiRoot = GameUIRoot.Instance;
                }
                return s_uiRoot;
            }
        }


        private static Canvas s_uiRootCanvas;
        public static Canvas UIRootCanvas
        {
            get
            {
                if (!s_uiRootCanvas)
                {
                    s_uiRootCanvas = UIRoot.GetComponent<Canvas>();
                }
                return s_uiRootCanvas;
            }
        }

        private static CanvasScaler s_uiRootCanvasScaler;
        public static CanvasScaler UIRootCanvasScaler
        {
            get
            {
                if (!s_uiRootCanvasScaler)
                {
                    s_uiRootCanvasScaler = UIRoot.GetComponent<CanvasScaler>();
                }
                return s_uiRootCanvasScaler;
            }
        }

        private static Camera s_uiRootCamera;
        public static Camera GameUIRootCamera
        {
            get
            {
                if (!s_uiRootCamera)
                {
                    s_uiRootCamera = UIRootCanvas.worldCamera;
                }
                return s_uiRootCamera;
            }
        }

        private static TitleScreenUI s_titleScreenUI;
        public static TitleScreenUI TitleScreenUI
        {
            get
            {
                if (!s_titleScreenUI)
                {
                    s_titleScreenUI = UIRoot?.TitleScreenUI;
                }
                return s_titleScreenUI;
            }
        }


        private static GameObject s_titleScreenRootButtonsBG;
        public static GameObject TitleScreenRootButtonsBG
        {
            get
            {
                if (!s_titleScreenRootButtonsBG)
                {
                    s_titleScreenRootButtonsBG = TitleScreenUI?.RootButtonsContainerBG;
                }
                return s_titleScreenRootButtonsBG;
            }
        }

        private static SettingsMenu s_settingsMenu;
        public static SettingsMenu SettingsMenu
        {
            get
            {
                if (!s_settingsMenu)
                {
                    s_settingsMenu = UIRoot?.SettingsMenu;
                }
                return s_settingsMenu;
            }
        }

        private static AttackManager s_attackManager;
        public static AttackManager AttackManager
        {
            get
            {
                if (!s_attackManager)
                {
                    s_attackManager = AttackManager.Instance;
                }
                return s_attackManager;
            }
        }

        private static AudioManager s_audioManager;
        public static AudioManager AudioManager
        {
            get
            {
                if (!s_audioManager)
                {
                    s_audioManager = AudioManager.Instance;
                }
                return s_audioManager;
            }
        }

        private static AudioLibrary s_audioLibrary;
        public static AudioLibrary AudioLibrary
        {
            get
            {
                if (!s_audioLibrary)
                {
                    s_audioLibrary = AudioLibrary.Instance;
                }
                return s_audioLibrary;
            }
        }

        private static GlobalFireParticleSystem s_globalFireParticleSystem;
        public static GlobalFireParticleSystem GlobalFireParticleSystem
        {
            get
            {
                if (!s_globalFireParticleSystem)
                {
                    s_globalFireParticleSystem = GlobalFireParticleSystem.Instance;
                }
                return s_globalFireParticleSystem;
            }
        }

        private static VoxelFadingManager s_fadingVoxelManager;
        public static VoxelFadingManager FadingVoxelManager
        {
            get
            {
                if (!s_fadingVoxelManager)
                {
                    s_fadingVoxelManager = VoxelFadingManager.Instance;
                }
                return s_fadingVoxelManager;
            }
        }

        private static PhotoManager s_photoManager;
        public static PhotoManager PhotoManager
        {
            get
            {
                if (!s_photoManager)
                {
                    s_photoManager = PhotoManager.Instance;
                }
                return s_photoManager;
            }
        }

        private static MethodInfo s_unityTimeFixedUnscaledDeltaTimePropertyGetter;
        public static MethodInfo UnityTimeFixedUnscaledDeltaTimePropertyGetter
        {
            get
            {
                if (s_unityTimeFixedUnscaledDeltaTimePropertyGetter == null)
                {
                    s_unityTimeFixedUnscaledDeltaTimePropertyGetter = AccessTools.DeclaredPropertyGetter(typeof(Time), nameof(Time.fixedUnscaledDeltaTime));
                }
                return s_unityTimeFixedUnscaledDeltaTimePropertyGetter;
            }
        }

        private static MethodInfo s_unityTimeUnscaledDeltaTimePropertyGetter;
        public static MethodInfo UnityTimeUnscaledDeltaTimePropertyGetter
        {
            get
            {
                if (s_unityTimeUnscaledDeltaTimePropertyGetter == null)
                {
                    s_unityTimeUnscaledDeltaTimePropertyGetter = AccessTools.DeclaredPropertyGetter(typeof(Time), nameof(Time.unscaledDeltaTime));
                }
                return s_unityTimeUnscaledDeltaTimePropertyGetter;
            }
        }

        private static MethodInfo s_unityInputGetMouseButtonMethod;
        public static MethodInfo UnityInputGetMouseButtonMethod
        {
            get
            {
                if (s_unityInputGetMouseButtonMethod == null)
                {
                    s_unityInputGetMouseButtonMethod = AccessTools.DeclaredMethod(typeof(Input), nameof(Input.GetMouseButton));
                }
                return s_unityInputGetMouseButtonMethod;
            }
        }

        private static MethodInfo s_unityInputGetMouseButtonDownMethod;
        public static MethodInfo UnityInputGetMouseButtonDownMethod
        {
            get
            {
                if (s_unityInputGetMouseButtonDownMethod == null)
                {
                    s_unityInputGetMouseButtonDownMethod = AccessTools.DeclaredMethod(typeof(Input), nameof(Input.GetMouseButtonDown));
                }
                return s_unityInputGetMouseButtonDownMethod;
            }
        }

        private static MethodInfo s_setTimeScaleForSeconds;
        public static MethodInfo SetTimeScaleForSeconds
        {
            get
            {
                if (s_setTimeScaleForSeconds == null)
                {
                    s_setTimeScaleForSeconds = AccessTools.DeclaredMethod(typeof(TimeManager), nameof(TimeManager.SetTimeScaleForSeconds));
                }
                return s_setTimeScaleForSeconds;
            }
        }

        public static GameUIThemeData UIThemeData { get; set; }
    }
}