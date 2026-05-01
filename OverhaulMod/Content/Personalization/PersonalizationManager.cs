using ICSharpCode.SharpZipLib.Zip;
using OverhaulMod.Engine;
using OverhaulMod.Gameplay;
using OverhaulMod.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationManager : Singleton<PersonalizationManager>, IGameLoadListener
    {
        public const string ASSETS_INFO_FILE = "CustomizationAssetsVersion.json";

        public const string ASSETS_INFO_FILE_OLD = "customizationAssetsInfo.json";

        public const string REMOTE_ASSETS_INFO_FILE = "CustomizationAssetsVersion_Remote.json";

        public const string REMOTE_ASSETS_INFO_FILE_OLD = "customizationAssetsInfo_remote.json";

        public const string CUSTOMIZATION_ASSETS_FILE_DOWNLOADED_EVENT = "CustomizationAssetsFileDownloaded";

        public const string ITEM_EQUIPPED_OR_UNEQUIPPED_EVENT = "PersonalizationItemEquippedOrUnequipped";

        public static readonly HashSet<string> SupportedBodyParts = new HashSet<string>
        {
            "Head",
            "Torso",
            "Spine",
            "ArmUpperR",
            "ArmLowerR",
            "HandR",
            "ArmUpperL",
            "ArmLowerL",
            "HandL",
            "LegUpperR",
            "LegLowerR",
            "FootR",
            "LegUpperL",
            "LegLowerL",
            "FootL",
        };

        public static readonly HashSet<string> HeadBodyParts = new HashSet<string>
        {
            "Head",
        };

        public static readonly HashSet<string> TorsoBodyParts = new HashSet<string>
        {
            "Torso",
            "ArmUpperR",
            "ArmLowerR",
            "HandR",
            "ArmUpperL",
            "ArmLowerL",
            "HandL",
        };

        public static readonly HashSet<string> LegsBodyParts = new HashSet<string>
        {
            "Spine",
            "LegUpperR",
            "LegLowerR",
            "FootR",
            "LegUpperL",
            "LegLowerL",
            "FootL",
        };

        private string _assetsVersionFile;
        public string AssetsVersionFile
        {
            get
            {
                if (_assetsVersionFile == null)
                {
                    _assetsVersionFile = Path.Combine(ModDirectories.ContentFolder, ASSETS_INFO_FILE);
                }
                return _assetsVersionFile;
            }
        }

        private string _remoteAssetsVersionFile;
        public string RemoteAssetsVersionFile
        {
            get
            {
                if (_remoteAssetsVersionFile == null)
                {
                    _remoteAssetsVersionFile = Path.Combine(ModDirectories.ContentFolder, REMOTE_ASSETS_INFO_FILE);
                }
                return _remoteAssetsVersionFile;
            }
        }

        public PersonalizationAssetsInfo LocalAssetsInfo { get; set; }

        public PersonalizationAssetsInfo RemoteAssetsInfo { get; set; }

        public PersonalizationItemList ItemList;

        public PersonalizationUserInfo UserInfo;

        private UnityWebRequest _webRequest;

        private bool _isSelectingItems;

        public override void Awake()
        {
            base.Awake();

            PersonalizationItemList personalizationItemList = new PersonalizationItemList();
            personalizationItemList.Load();
            ItemList = personalizationItemList;

            loadUserInfoFile();
        }

        private void Start()
        {
            LoadLocalCustomizationAssetsVersion();
            LoadRemoteCustomizationAssetsVersion();

            if (!File.Exists(RemoteAssetsVersionFile) || ScheduledActionsManager.Instance.ShouldExecuteAction(ScheduledActionType.RefreshCustomizationAssetsRemoteVersion))
            {
                RefreshRemoteCustomizationAssetsVersion(null);
            }
        }

        public void OnGameLoaded()
        {
            _isSelectingItems = false;

            PersonalizationUserInfo userInfo = this.UserInfo;
            if (userInfo != null)
            {
                userInfo.RefreshAllItemsVerification();
                SaveUserInfo();
            }
        }

        public void EquipAndApplyItem(PersonalizationItemInfo item)
        {
            if (!item.IsCompatibleWithMod())
            {
                ModUIUtils.MessagePopupOK("Incompatible item!", $"This item is made for the new version of Overhaul mod.\nMake sure you're using the latest version of the mod.", 175f, true);
                return;
            }

            if (item.Category == PersonalizationCategory.WeaponSkins)
            {
                FirstPersonMover firstPersonMover = CharacterTracker.Instance.GetPlayerRobot();
                List<FirstPersonMover> clones = CloneManager.Instance._clones;

                List<FirstPersonMover> allPlayers = new List<FirstPersonMover>(clones);
                if (firstPersonMover && firstPersonMover.IsAttachedAndAlive())
                    allPlayers.Add(firstPersonMover);

                PersonalizationUserInfo.SetItemEquipped(item, true);
            }
            else if (item.Category == PersonalizationCategory.Accessories || item.Category == PersonalizationCategory.Pets)
            {
                PersonalizationUserInfo.SetItemEquipped(item, !PersonalizationUserInfo.IsItemEquipped(item));
            }

            GlobalEventManager.Instance.Dispatch(ITEM_EQUIPPED_OR_UNEQUIPPED_EVENT);

            RefreshCustomizationOnAllRobots(false, false, item.Category);
        }

        public static bool IsWeaponCustomizationSupported(WeaponType weaponType)
        {
            return weaponType != WeaponType.None &&
                  (weaponType == WeaponType.Sword
                || weaponType == WeaponType.Bow
                || weaponType == WeaponType.Hammer
                || weaponType == WeaponType.Spear
                || weaponType == WeaponType.Shield
                || weaponType == ModWeaponsManager.SCYTHE_TYPE);
        }

        public static bool IsBodyPartSupported(string bodyPart) => SupportedBodyParts.Contains(bodyPart);

        public void SetIsSelectingItems(bool value) => _isSelectingItems = value;

        public bool IsSelectingItems() => _isSelectingItems;

        public PersonalizationAssetsState GetPersonalizationAssetsState()
        {
            PersonalizationAssetsInfo localInfo = LocalAssetsInfo;
            PersonalizationAssetsInfo remoteInfo = RemoteAssetsInfo;
            if (localInfo == null)
                return PersonalizationAssetsState.NotInstalled;

            if (remoteInfo == null || localInfo.AssetVersionNumber >= remoteInfo.AssetVersionNumber)
                return PersonalizationAssetsState.Installed;

            return PersonalizationAssetsState.NeedUpdate;
        }

        public void DownloadCustomizationFile(Action<string> callback)
        {
            StartCoroutine(downloadCustomizationFileCoroutine(callback));
        }

        private IEnumerator downloadCustomizationFileCoroutine(Action<string> callback)
        {
            RepositoryManager.Instance.GetCustomFile($"https://github.com/aTVCat/Overhaul-Mod-Content/raw/main/content/{PersonalizationEditorDataManager.ITEMS_ARCHIVE_FILE}", delegate (byte[] bytes)
            {
                _webRequest = null;
                try
                {
                    if (!Directory.Exists(ModDirectories.CustomizationFolder))
                    {
                        _ = Directory.CreateDirectory(ModDirectories.CustomizationFolder);
                    }
                    else
                    {
                        foreach (string folder in Directory.GetDirectories(ModDirectories.CustomizationFolder))
                        {
                            Directory.Delete(folder, true);
                        }
                    }

                    string tempFile = Path.GetTempFileName();
                    ModFileUtils.WriteBytes(bytes, tempFile);

                    FastZip fastZip = new FastZip();
                    fastZip.ExtractZip(tempFile, ModDirectories.CustomizationFolder, null);

                    if (RemoteAssetsInfo != null)
                    {
                        ModJsonUtils.WriteStream(AssetsVersionFile, RemoteAssetsInfo);
                        LocalAssetsInfo = RemoteAssetsInfo;
                    }

                    ItemList.Load();
                    GlobalEventManager.Instance.Dispatch(CUSTOMIZATION_ASSETS_FILE_DOWNLOADED_EVENT);
                }
                catch (Exception exc)
                {
                    callback?.Invoke(exc.ToString());
                    return;
                }
                callback?.Invoke(null);
            }, delegate
            {
                string error = ModUnityUtils.GetWebRequestErrorString(_webRequest);
                _webRequest = null;

                callback?.Invoke(error);
            }, out UnityWebRequest unityWebRequest, -1);
            _webRequest = unityWebRequest;
            yield break;
        }

        public bool IsDownloadingCustomizationFile() => _webRequest != null;

        public float GetCustomizationFileDownloadProgress()
        {
            UnityWebRequest unityWebRequest = _webRequest;
            if (unityWebRequest == null)
                return 0f;

            try
            {
                return unityWebRequest.downloadProgress;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public void RefreshRemoteCustomizationAssetsVersion(Action<bool> callback)
        {
            RemoteAssetsInfo = null;

            ScheduledActionsManager scheduledActionsManager = ScheduledActionsManager.Instance;
            RepositoryManager.Instance.GetTextFile($"content/{ASSETS_INFO_FILE}", delegate (string result)
            {
                PersonalizationAssetsInfo personalizationAssetsInfo;
                try
                {
                    personalizationAssetsInfo = ModJsonUtils.Deserialize<PersonalizationAssetsInfo>(result);
                }
                catch (Exception)
                {
                    personalizationAssetsInfo = new PersonalizationAssetsInfo();
                }
                RemoteAssetsInfo = personalizationAssetsInfo;

                ModJsonUtils.WriteStream(RemoteAssetsVersionFile, personalizationAssetsInfo);
                scheduledActionsManager.SetActionExecuted(ScheduledActionType.RefreshCustomizationAssetsRemoteVersion);

                callback?.Invoke(true);
            }, delegate (string error)
            {
                callback?.Invoke(false);
            }, out _);
        }

        public void LoadRemoteCustomizationAssetsVersion()
        {
            string path = RemoteAssetsVersionFile;
            if (!File.Exists(path))
            {
                RemoteAssetsInfo = null;
            }
            else
            {
                PersonalizationAssetsInfo personalizationAssetsInfo;
                try
                {
                    personalizationAssetsInfo = ModJsonUtils.DeserializeStream<PersonalizationAssetsInfo>(path);
                }
                catch (Exception)
                {
                    personalizationAssetsInfo = new PersonalizationAssetsInfo();
                }
                RemoteAssetsInfo = personalizationAssetsInfo;
            }
        }

        public void LoadLocalCustomizationAssetsVersion()
        {
            string path = AssetsVersionFile;
            if (!File.Exists(path))
            {
                LocalAssetsInfo = null;
            }
            else
            {
                PersonalizationAssetsInfo personalizationAssetsInfo;
                try
                {
                    personalizationAssetsInfo = ModJsonUtils.DeserializeStream<PersonalizationAssetsInfo>(path);
                }
                catch (Exception)
                {
                    personalizationAssetsInfo = new PersonalizationAssetsInfo();
                }
                personalizationAssetsInfo.RefreshCounters(ItemList);
                LocalAssetsInfo = personalizationAssetsInfo;
            }
        }

        public void SetLocalAssetsVersion(int versionNumber)
        {
            PersonalizationAssetsInfo personalizationAssetsInfo = LocalAssetsInfo;
            if (personalizationAssetsInfo == null)
            {
                personalizationAssetsInfo = new PersonalizationAssetsInfo
                {
                    AssetVersionNumber = versionNumber
                };
                LocalAssetsInfo = personalizationAssetsInfo;
            }
            else
            {
                personalizationAssetsInfo.AssetVersionNumber = versionNumber;
            }
            personalizationAssetsInfo.RefreshCounters(ItemList);
            ModJsonUtils.WriteStream(AssetsVersionFile, personalizationAssetsInfo);
        }

        private void loadUserInfoFile()
        {
            PersonalizationUserInfo personalizationUserInfo;
            string path = Path.Combine(ModDirectories.ModUserDataFolder, PersonalizationUserInfo.USER_INFO_FILE);
            if (File.Exists(path))
            {
                try
                {
                    personalizationUserInfo = ModJsonUtils.DeserializeStream<PersonalizationUserInfo>(path);
                }
                catch (Exception)
                {
                    personalizationUserInfo = new PersonalizationUserInfo();
                }
            }
            else
            {
                personalizationUserInfo = new PersonalizationUserInfo();
            }

            personalizationUserInfo.FixValues();

            UserInfo = personalizationUserInfo;
        }

        public void SaveUserInfo()
        {
            PersonalizationUserInfo personalizationUserInfo = UserInfo;
            if (personalizationUserInfo != null) ModDataManager.SerializeToFile(PersonalizationUserInfo.USER_INFO_FILE, personalizationUserInfo, false);
        }

        public void RefreshCustomizationOnAllRobots(bool onlyPlayers, bool onlyEnemies, PersonalizationCategory category = PersonalizationCategory.None)
        {
            foreach (Character character in CharacterTracker.Instance.GetAllLivingCharacters())
            {
                if (!character || !character.IsAttachedAndAlive()) continue;

                bool isPlayer = character.IsPlayer();
                if ((onlyPlayers && !isPlayer) || (onlyEnemies && isPlayer)) continue;

                CharacterUpdateScheduler.Instance.UpdateCharacter(character, new CharacterUpdateRequest()
                {
                    UpdateWeaponSkins = category == PersonalizationCategory.WeaponSkins,
                    UpdateAccessories = category == PersonalizationCategory.Accessories,
                    UpdatePets = category == PersonalizationCategory.Pets,
                    UpdateWeaponBag = category == PersonalizationCategory.WeaponSkins,
                });
            }
        }
    }
}
