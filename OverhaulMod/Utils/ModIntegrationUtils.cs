using InternalModBot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UI;

namespace OverhaulMod.Utils
{
    public static class ModIntegrationUtils
    {
        private static bool s_hasLoaded;

        public static void Load()
        {
            if (s_hasLoaded)
                return;

            s_hasLoaded = true;
            SelectGarbageBotSkin.Load();
            SoundpackMod.Load();
        }

        public static class SoundpackMod
        {
            private static FieldInfo s_mainClassCurrentSoundpackIndexField;

            private static FieldInfo s_mainClassAvailableSoundPacksField;

            private static PropertyInfo s_soundpackStateProperty;

            public static void Load()
            {
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (assembly.GetName().Name != "SoundReplacingMod")
                        continue;

                    Type type = assembly.GetType("SoundReplacingMod.Main");
                    if (type != null)
                    {
                        s_mainClassCurrentSoundpackIndexField = type.GetField("_currentSoundPackIndex", BindingFlags.Static | BindingFlags.NonPublic);
                        s_mainClassAvailableSoundPacksField = type.GetField("_availableSoundPacks", BindingFlags.Static | BindingFlags.NonPublic);
                    }

                    Type type2 = assembly.GetType("SoundReplacingMod.SoundPack");
                    if (type2 != null)
                    {
                        s_soundpackStateProperty = type2.GetProperty("State", BindingFlags.Instance | BindingFlags.Public);
                    }
                }
            }

            public static bool HasLoadedSoundpack()
            {
                if (s_mainClassCurrentSoundpackIndexField == null || s_mainClassAvailableSoundPacksField == null || s_soundpackStateProperty == null)
                    return true;

                int soundpackIndex = (int)s_mainClassCurrentSoundpackIndexField.GetValue(null);
                if (soundpackIndex <= -1)
                    return true;

                IList list = (IList)s_mainClassAvailableSoundPacksField.GetValue(null);
                if (soundpackIndex >= list.Count)
                    return true;

                object soundpackObject = list[soundpackIndex];
                if (soundpackObject == null)
                    return true;

                byte loadingState = (byte)s_soundpackStateProperty.GetValue(soundpackObject);
                return loadingState == 2;
            }
        }

        public static class SelectGarbageBotSkin
        {
            private static MethodInfo s_getGarbageBotSkinOptionsMethod;

            private static MethodInfo s_hasLocalPlayerStatsMethod;

            private static PropertyInfo s_selectedGarbageBotSkinIndexProperty;

            private static readonly object[] s_getGarbageBotSkinOptionsMethodArgs = { true };

            private static bool s_modMethodsPresent;

            public static int selectedGarbageBotSkinIndex
            {
                get
                {
                    if (s_selectedGarbageBotSkinIndexProperty == null)
                        return -1;

                    return (int)s_selectedGarbageBotSkinIndexProperty.GetValue(null);
                }
                set
                {
                    if (s_selectedGarbageBotSkinIndexProperty == null)
                        return;

                    s_selectedGarbageBotSkinIndexProperty.SetValue(null, value);
                }
            }

            public static void Load()
            {
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (assembly.GetName().Name != "SelectGarbageBotSkins")
                        continue;

                    Type type = assembly.GetType("SelectGarbageBotSkins.ModCore");
                    if (type != null)
                    {
                        s_getGarbageBotSkinOptionsMethod = type.GetMethod("GetGarbageBotSkinOptions", BindingFlags.Static | BindingFlags.Public);
                        s_hasLocalPlayerStatsMethod = type.GetMethod("HasLocalPlayerStats", BindingFlags.Static | BindingFlags.Public);
                    }

                    Type type2 = assembly.GetType("SelectGarbageBotSkins.ModSettings");
                    if (type2 != null)
                    {
                        s_selectedGarbageBotSkinIndexProperty = type2.GetProperty("selectedGarbageBotSkinIndex", BindingFlags.Static | BindingFlags.Public);
                    }

                    s_modMethodsPresent = s_getGarbageBotSkinOptionsMethod != null && s_hasLocalPlayerStatsMethod != null && s_selectedGarbageBotSkinIndexProperty != null;
                }
            }

            public static List<Dropdown.OptionData> GetGarbageBotSkinOptions()
            {
                if (s_getGarbageBotSkinOptionsMethod == null)
                    return null;

                return (List<Dropdown.OptionData>)s_getGarbageBotSkinOptionsMethod.Invoke(null, s_getGarbageBotSkinOptionsMethodArgs);
            }

            public static bool HasLocalPlayerStats()
            {
                if (s_hasLocalPlayerStatsMethod == null)
                    return false;

                return (bool)s_hasLocalPlayerStatsMethod.Invoke(null, null);
            }

            public static bool IsModAvailable()
            {
                return s_modMethodsPresent && ModSpecialUtils.IsModEnabled("battle-royale-garbage-bot-selection");
            }
        }

        public static class ModBot
        {
            public static string GetModBotUsername()
            {
                return ModBotSignInUI._userName;
            }
        }
    }
}