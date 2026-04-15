using ICSharpCode.SharpZipLib.Zip;
using OverhaulMod.Combat;
using OverhaulMod.Engine;
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
        public const string ASSETS_VERSION_FILE = "customizationAssetsInfo.json";

        public const string REMOTE_ASSETS_VERSION_FILE = "customizationAssetsInfo_remote.json";

        public const string CUSTOMIZATION_ASSETS_FILE_DOWNLOADED_EVENT = "CustomizationAssetsFileDownloaded";

        public const string ITEM_EQUIPPED_OR_UNEQUIPPED_EVENT = "PersonalizationItemEquippedOrUnequipped";

        public const string USER_INFO_FILE = "PersonalizationUserInfo.json";

        public static readonly WeaponType[] SupportedWeapons = new WeaponType[]
        {
            WeaponType.Sword,
            WeaponType.Bow,
            WeaponType.Hammer,
            WeaponType.Spear,
            ModWeaponsManager.SCYTHE_TYPE,
        };

        public static readonly string[] SupportedBodyParts = new string[]
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

        private string _assetsVersionFile;
        public string AssetsVersionFile
        {
            get
            {
                if (_assetsVersionFile == null)
                {
                    _assetsVersionFile = Path.Combine(ModCore.ContentFolder, ASSETS_VERSION_FILE);
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
                    _remoteAssetsVersionFile = Path.Combine(ModCore.ContentFolder, REMOTE_ASSETS_VERSION_FILE);
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

            ScheduledActionsManager scheduledActionsManager = ScheduledActionsManager.Instance;
            if (!scheduledActionsManager.ShouldExecuteAction(ScheduledActionType.RefreshCustomizationAssetsRemoteVersion))
                LoadRemoteCustomizationAssetsVersion();
            else
                RefreshRemoteCustomizationAssetsVersion(null);
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

        public void SetIsSelectingItems(bool value)
        {
            _isSelectingItems = value;
        }

        public bool IsSelectingItems() => _isSelectingItems;

        public void DownloadCustomizationFile(Action<string> callback)
        {
            _ = downloadCustomizationFileCoroutine(callback).Run();
        }

        private IEnumerator downloadCustomizationFileCoroutine(Action<string> callback)
        {
            RepositoryManager.Instance.GetCustomFile($"https://github.com/aTVCat/Overhaul-Mod-Content/raw/main/content/customization.zip", delegate (byte[] bytes)
            {
                _webRequest = null;
                try
                {
                    if (!Directory.Exists(ModCore.CustomizationFolder))
                    {
                        _ = Directory.CreateDirectory(ModCore.CustomizationFolder);
                    }
                    else
                    {
                        foreach (string folder in Directory.GetDirectories(ModCore.CustomizationFolder))
                        {
                            Directory.Delete(folder, true);
                        }
                    }

                    string tempFile = Path.GetTempFileName();
                    ModFileUtils.WriteBytes(bytes, tempFile);

                    FastZip fastZip = new FastZip();
                    fastZip.ExtractZip(tempFile, ModCore.CustomizationFolder, null);

                    if (RemoteAssetsInfo != null)
                    {
                        ModJsonUtils.WriteStream(AssetsVersionFile, RemoteAssetsInfo);
                        LocalAssetsInfo = RemoteAssetsInfo;
                    }
                    else
                    {
                        RefreshRemoteCustomizationAssetsVersion(delegate (bool result)
                        {
                            if (result)
                            {
                                ModJsonUtils.WriteStream(AssetsVersionFile, RemoteAssetsInfo);
                                LocalAssetsInfo = RemoteAssetsInfo;
                            }
                        });
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

        public void LoadLocalCustomizationAssetsVersion()
        {
            string path = AssetsVersionFile;
            if (!File.Exists(path))
                LocalAssetsInfo = null;
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
                LocalAssetsInfo = personalizationAssetsInfo;
            }
        }

        public void LoadRemoteCustomizationAssetsVersion()
        {
            string path = RemoteAssetsVersionFile;
            if (!File.Exists(path))
                RemoteAssetsInfo = null;
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

        public void RefreshRemoteCustomizationAssetsVersion(Action<bool> callback)
        {
            RemoteAssetsInfo = null;

            ScheduledActionsManager scheduledActionsManager = ScheduledActionsManager.Instance;
            RepositoryManager.Instance.GetTextFile($"content/{ASSETS_VERSION_FILE}", delegate (string result)
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

        public PersonalizationAssetsState GetPersonalizationAssetsState()
        {
            PersonalizationAssetsInfo localInfo = LocalAssetsInfo;
            PersonalizationAssetsInfo remoteInfo = RemoteAssetsInfo;
            if (localInfo == null)
                return PersonalizationAssetsState.NotInstalled;

            if (remoteInfo == null || remoteInfo.AssetVersionNumber <= localInfo.AssetVersionNumber)
                return PersonalizationAssetsState.Installed;

            return PersonalizationAssetsState.NeedUpdate;
        }

        public bool IsDownloadingCustomizationFile()
        {
            return _webRequest != null;
        }

        public bool SetLocalAssetsVersion(string versionString)
        {
            if (!int.TryParse(versionString, out int versionNumber))
                return false;

            return SetLocalAssetsVersion(versionNumber);
        }

        public bool SetLocalAssetsVersion(int versionNumber)
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
            ModJsonUtils.WriteStream(AssetsVersionFile, personalizationAssetsInfo);
            return true;
        }

        public int GetLocalAssetsVersion()
        {
            PersonalizationAssetsInfo assetsInfo = LocalAssetsInfo;
            if (assetsInfo == null)
                return -1;

            return assetsInfo.AssetVersionNumber;
        }

        private void loadUserInfoFile()
        {
            PersonalizationUserInfo personalizationUserInfo;
            string path = Path.Combine(ModDataManager.UserDataFolder, USER_INFO_FILE);
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
            if (personalizationUserInfo != null)
            {
                ModDataManager.SerializeToFile(USER_INFO_FILE, personalizationUserInfo, false);
            }
        }

        public void RefreshCustomizationOnAllRobots(bool onlyPlayers, bool onlyEnemies)
        {
            foreach (Character character in CharacterTracker.Instance.GetAllLivingCharacters())
            {
                if (!character || !character.IsAttachedAndAlive()) continue;

                bool isPlayer = character.IsPlayer();
                if ((onlyPlayers && !isPlayer) || (onlyEnemies && isPlayer)) continue;

                CharacterUpdateScheduler.Instance.UpdateCharacter(character, new CharacterUpdateRequest()
                {
                    UpdateWeaponSkins = true,
                    UpdateAccessories = true,
                    UpdatePets = true,
                    UpdateWeaponBag = true,
                });
            }
        }

        public static bool IsWeaponCustomizationSupported(WeaponType weaponType)
        {
            return weaponType != WeaponType.None && (weaponType == WeaponType.Sword
                || weaponType == WeaponType.Bow
                || weaponType == WeaponType.Hammer
                || weaponType == WeaponType.Spear
                || weaponType == WeaponType.Shield
                || weaponType == ModWeaponsManager.SCYTHE_TYPE);
        }

        public void DestroyWeaponSkinOnMainPlayer(WeaponType weaponType)
        {
            FirstPersonMover player = CharacterTracker.Instance.GetPlayerRobot();
            if (!player || !player.IsAttachedAndAlive()) return;

            PersonalizationController personalizationController = player.GetComponent<PersonalizationController>();
            if (!personalizationController) return;

            personalizationController.DestroyItem(personalizationController.GetSpawnedWeaponSkinInfo(weaponType));
        }

        public void EquipItem(PersonalizationItemInfo item)
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

                SetIsItemEquipped(item, true);
            }
            else if (item.Category == PersonalizationCategory.Accessories)
            {
                SetIsItemEquipped(item, !GetIsItemEquipped(item));
            }

            RefreshCustomizationOnAllRobots(false, false);
        }

        public static void SetIsItemEquipped(PersonalizationItemInfo item, bool value)
        {
            string id = item.ItemID;
            switch (item.Category)
            {
                case PersonalizationCategory.WeaponSkins:
                    PersonalizationUserInfo.SetWeaponSkin(item.Weapon, id);
                    break;
                case PersonalizationCategory.Accessories:
                    PersonalizationUserInfo.SetAccessoryEquipped(item.ItemID, value);
                    break;
            }

            GlobalEventManager.Instance.Dispatch(ITEM_EQUIPPED_OR_UNEQUIPPED_EVENT);
        }

        public static bool GetIsItemEquipped(PersonalizationItemInfo item)
        {
            string itemId = item.ItemID;
            switch (item.Category)
            {
                case PersonalizationCategory.WeaponSkins:
                    switch (item.Weapon)
                    {
                        case WeaponType.Sword:
                            return PersonalizationUserInfo.SwordSkin == itemId;
                        case WeaponType.Bow:
                            return PersonalizationUserInfo.BowSkin == itemId;
                        case WeaponType.Hammer:
                            return PersonalizationUserInfo.HammerSkin == itemId;
                        case WeaponType.Spear:
                            return PersonalizationUserInfo.SpearSkin == itemId;
                        case WeaponType.Shield:
                            return PersonalizationUserInfo.ShieldSkin == itemId;
                        case ModWeaponsManager.SCYTHE_TYPE:
                            return PersonalizationUserInfo.ScytheSkin == itemId;
                    }
                    return false;
                case PersonalizationCategory.Accessories:
                    return !itemId.IsNullOrEmpty() && PersonalizationUserInfo.Accessories.Contains(itemId);
            }
            return false;
        }
    }
}
