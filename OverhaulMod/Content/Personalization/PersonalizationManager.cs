using ICSharpCode.SharpZipLib.Zip;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationManager : Singleton<PersonalizationManager>
    {
        public const string DATA_REFRESH_TIME_PLAYER_PREF_KEY = "CustomizationAssetsInfoRefreshDate";

        public const string ITEM_INFO_FILE = "itemInfo.json";

        public const string ASSETS_VERSION_FILE = "customizationAssetsInfo.json";

        public const string REMOTE_ASSETS_VERSION_FILE = "customizationAssetsInfo_remote.json";

        public const string CUSTOMIZATION_ASSETS_FILE_DOWNLOADED_EVENT = "CustomizationAssetsFileDownloaded";

        public static readonly string[] SupportedBodyParts = new string[]
        {
            "Head",
            "Arm"
        };

        private string m_assetsVersionFile;
        public string assetsVersionFile
        {
            get
            {
                if (m_assetsVersionFile == null)
                {
                    m_assetsVersionFile = Path.Combine(ModCore.customizationFolder, ASSETS_VERSION_FILE);
                }
                return m_assetsVersionFile;
            }
        }

        private string m_remoteAssetsVersionFile;
        public string remoteAssetsVersionFile
        {
            get
            {
                if (m_remoteAssetsVersionFile == null)
                {
                    m_remoteAssetsVersionFile = Path.Combine(ModCore.customizationFolder, REMOTE_ASSETS_VERSION_FILE);
                }
                return m_remoteAssetsVersionFile;
            }
        }

        public PersonalizationAssetsInfo localAssetsInfo { get; set; }

        public PersonalizationAssetsInfo remoteAssetsInfo { get; set; }

        public PersonalizationItemList itemList
        {
            get;
            private set;
        }

        private UnityWebRequest m_webRequest;

        public override void Awake()
        {
            base.Awake();

            PersonalizationItemList personalizationItemList = new PersonalizationItemList();
            personalizationItemList.Load();
            itemList = personalizationItemList;
        }

        private void Start()
        {
            RefreshLocalCustomizationAssetsVersion();
            RefreshRemoteCustomizationAssetsVersion(null);
        }

        public void DownloadCustomizationFile(Action<bool> callback)
        {
            _ = downloadCustomizationFileCoroutine(callback).Run();
        }

        private IEnumerator downloadCustomizationFileCoroutine(Action<bool> callback)
        {
            RepositoryManager.Instance.GetCustomFile($"https://github.com/aTVCat/Overhaul-Mod-Content/raw/main/content/customization.zip", delegate (byte[] bytes)
            {
                m_webRequest = null;
                try
                {
                    if (!Directory.Exists(ModCore.customizationFolder))
                    {
                        _ = Directory.CreateDirectory(ModCore.customizationFolder);
                    }
                    /*
                    else
                    {
                        foreach (var directory in Directory.GetDirectories(ModCore.customizationFolder))
                        {
                            Directory.Delete(directory, true);
                        }
                    }*/

                    string tempFile = Path.GetTempFileName();
                    ModIOUtils.WriteBytes(bytes, tempFile);

                    FastZip fastZip = new FastZip();
                    fastZip.ExtractZip(tempFile, ModCore.customizationFolder, null);

                    if (remoteAssetsInfo != null)
                    {
                        ModJsonUtils.WriteStream(assetsVersionFile, remoteAssetsInfo);
                        localAssetsInfo = remoteAssetsInfo;
                    }
                    else
                    {
                        RefreshRemoteCustomizationAssetsVersion(delegate (bool result)
                        {
                            if (result)
                            {
                                ModJsonUtils.WriteStream(assetsVersionFile, remoteAssetsInfo);
                                localAssetsInfo = remoteAssetsInfo;
                            }
                        }, true);
                    }

                    GlobalEventManager.Instance.Dispatch(CUSTOMIZATION_ASSETS_FILE_DOWNLOADED_EVENT);
                }
                catch (Exception exc)
                {
                    Debug.Log(exc);
                    callback?.Invoke(false);
                    return;
                }
                itemList.Load();
                callback?.Invoke(true);
            }, delegate
            {
                m_webRequest = null;
                callback?.Invoke(false);
            }, out UnityWebRequest unityWebRequest, -1);
            m_webRequest = unityWebRequest;
            yield break;
        }

        public float GetCustomizationFileDownloadProgress()
        {
            UnityWebRequest unityWebRequest = m_webRequest;
            if (unityWebRequest == null)
                return 0f;

            try
            {
                return unityWebRequest.downloadProgress;
            }
            catch
            {
                return 0;
            }
        }

        public void RefreshLocalCustomizationAssetsVersion()
        {
            string path = assetsVersionFile;
            if (!File.Exists(path))
                localAssetsInfo = null;
            else
            {
                PersonalizationAssetsInfo personalizationAssetsInfo;
                try
                {
                    personalizationAssetsInfo = ModJsonUtils.DeserializeStream<PersonalizationAssetsInfo>(path);
                    personalizationAssetsInfo.FixValues();
                    localAssetsInfo = personalizationAssetsInfo;
                }
                catch
                {
                    personalizationAssetsInfo = new PersonalizationAssetsInfo();
                    personalizationAssetsInfo.FixValues();
                    localAssetsInfo = personalizationAssetsInfo;
                }
            }
        }

        public void RefreshRemoteCustomizationAssetsVersion(Action<bool> callback, bool force = false)
        {
            remoteAssetsInfo = null;
            if (!force)
            {
                if (DateTime.TryParse(PlayerPrefs.GetString(DATA_REFRESH_TIME_PLAYER_PREF_KEY, "default"), out DateTime timeToRefreshData))
                    if (DateTime.Now < timeToRefreshData)
                    {
                        PersonalizationAssetsInfo personalizationAssetsInfo;
                        if (File.Exists(remoteAssetsVersionFile))
                        {
                            try
                            {
                                personalizationAssetsInfo = ModJsonUtils.Deserialize<PersonalizationAssetsInfo>(remoteAssetsVersionFile);
                                personalizationAssetsInfo.FixValues();
                            }
                            catch
                            {
                                personalizationAssetsInfo = new PersonalizationAssetsInfo();
                                personalizationAssetsInfo.FixValues();
                            }

                            remoteAssetsInfo = personalizationAssetsInfo;
                            callback?.Invoke(true);
                            return;
                        }
                    }
            }

            RepositoryManager.Instance.GetTextFile($"content/{ASSETS_VERSION_FILE}", delegate (string result)
            {
                PersonalizationAssetsInfo personalizationAssetsInfo;
                try
                {
                    personalizationAssetsInfo = ModJsonUtils.Deserialize<PersonalizationAssetsInfo>(result);
                    personalizationAssetsInfo.FixValues();
                }
                catch
                {
                    personalizationAssetsInfo = new PersonalizationAssetsInfo();
                    personalizationAssetsInfo.FixValues();
                }

                remoteAssetsInfo = personalizationAssetsInfo;
                ModJsonUtils.WriteStream(remoteAssetsVersionFile, personalizationAssetsInfo);
                PlayerPrefs.SetString(DATA_REFRESH_TIME_PLAYER_PREF_KEY, DateTime.Now.AddDays(5).ToString());

                callback?.Invoke(true);
            }, delegate (string error)
            {
                callback?.Invoke(false);
            }, out _);
        }

        public PersonalizationAssetsState GetPersonalizationAssetsState()
        {
            PersonalizationAssetsInfo localInfo = localAssetsInfo;
            PersonalizationAssetsInfo remoteInfo = remoteAssetsInfo;
            if (localInfo == null)
                return PersonalizationAssetsState.NotInstalled;

            if (remoteInfo == null || remoteInfo.AssetsVersion <= localInfo.AssetsVersion)
                return PersonalizationAssetsState.Installed;

            return PersonalizationAssetsState.NeedUpdate;
        }

        public bool IsDownloadingCustomizationFile()
        {
            return m_webRequest != null;
        }

        public void SetLocalAssetsVersion(string versionString, Action<bool> callback)
        {
            if (System.Version.TryParse(versionString, out System.Version version))
            {
                PersonalizationAssetsInfo personalizationAssetsInfo = localAssetsInfo;
                if (personalizationAssetsInfo == null)
                {
                    personalizationAssetsInfo = new PersonalizationAssetsInfo
                    {
                        AssetsVersion = version
                    };
                    personalizationAssetsInfo.FixValues();
                    localAssetsInfo = personalizationAssetsInfo;
                }
                ModJsonUtils.WriteStream(assetsVersionFile, personalizationAssetsInfo);

                callback?.Invoke(true);
            }
            else
            {
                callback?.Invoke(false);
            }
        }

        public System.Version GetLocalAssetsVersion()
        {
            return localAssetsInfo?.AssetsVersion;
        }

        public void ConfigureFirstPersonMover(FirstPersonMover firstPersonMover)
        {
            if (!firstPersonMover || !firstPersonMover.IsAlive())
                return;

            _ = firstPersonMover.gameObject.AddComponent<PersonalizationController>();
        }

        public bool CreateItem(string name, out PersonalizationItemInfo personalizationItem)
        {
            personalizationItem = null;
            if (name.IsNullOrEmpty())
                return false;

            string directoryName = name.Replace(" ", string.Empty);
            string directoryPath = ModCore.customizationFolder + directoryName + "/";
            string filesDirectoryPath = directoryPath + "files/";

            if (Directory.Exists(directoryPath))
                return false;

            if (!Directory.Exists(filesDirectoryPath))
                _ = Directory.CreateDirectory(filesDirectoryPath);

            _ = Directory.CreateDirectory(directoryPath);
            personalizationItem = new PersonalizationItemInfo()
            {
                Name = name,
                Description = "No description provided.",
                IsVerified = false,
                Category = PersonalizationCategory.WeaponSkins,
                EditorID = PersonalizationEditorManager.Instance.editorId,
                ItemID = Guid.NewGuid().ToString(),
                FolderPath = directoryPath
            };
            personalizationItem.FixValues();
            personalizationItem.SetAuthor(SteamFriends.GetPersonaName());
            itemList.Items.Add(personalizationItem);

            ModJsonUtils.WriteStream(directoryPath + ITEM_INFO_FILE, personalizationItem);
            return true;
        }

        public List<ColorPairFloat> GetColorPairsFromString(string dataString)
        {
            if (dataString.IsNullOrEmpty())
                return null;

            string[] split = dataString.Split('|');
            if (split.IsNullOrEmpty())
                return null;

            List<ColorPairFloat> list = new List<ColorPairFloat>();
            foreach (string oldAndNewColorsString in split)
            {
                if (oldAndNewColorsString.IsNullOrEmpty())
                    continue;

                string[] oldAndNewColors = oldAndNewColorsString.Split('-');
                if (oldAndNewColors.Length == 2)
                {
                    Color a = ModParseUtils.TryParseToColor(oldAndNewColors[0], Color.white);
                    Color b = ModParseUtils.TryParseToColor(oldAndNewColors[1], Color.white);
                    list.Add(new ColorPairFloat(a, b));
                }
            }
            return list;
        }

        public string GetStringFromColorPairs(List<ColorPairFloat> colorPairs)
        {
            if (colorPairs.IsNullOrEmpty())
                return null;

            int index = 0;

            StringBuilder stringBuilder = new StringBuilder();
            foreach (ColorPairFloat cp in colorPairs)
            {
                string colorA = ColorUtility.ToHtmlStringRGBA(cp.ColorA);
                string colorB = ColorUtility.ToHtmlStringRGBA(cp.ColorB);
                string colorsString = $"{colorA}-{colorB}".Replace("#", string.Empty);
                _ = stringBuilder.Append(colorsString);
                if (index + 1 != colorPairs.Count)
                    _ = stringBuilder.Append('|');

                index++;
            }
            return stringBuilder.ToString();
        }

        public static bool IsWeaponCustomizationSupported(WeaponType weaponType)
        {
            return weaponType != WeaponType.None
&& (weaponType == WeaponType.Sword
                || weaponType == WeaponType.Bow
                || weaponType == WeaponType.Hammer
                || weaponType == WeaponType.Spear
                || weaponType == WeaponType.Shield);
        }
    }
}
