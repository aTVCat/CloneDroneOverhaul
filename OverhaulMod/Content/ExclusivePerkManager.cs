using OverhaulMod.Engine;
using OverhaulMod.Utils;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OverhaulMod.Content
{
    public class ExclusivePerkManager : Singleton<ExclusivePerkManager>
    {
        public const string FILE_NAME = "ExclusivePerks.json";

        private ExclusivePerkInfoList m_perksData;

        private string m_error;

        public override void Awake()
        {
            base.Awake();
            LoadDataFromDisk();

            ScheduledActionsManager scheduledActionsManager = ScheduledActionsManager.Instance;
            if (!ModUserInfo.isDeveloper && (m_perksData.List.Count == 0 || scheduledActionsManager.ShouldExecuteAction(ScheduledActionType.RefreshExclusivePerks)))
            {
                LoadDataFromRepository(delegate (string error)
                {
                    m_error = error;
                });
            }
        }

        public string GetError()
        {
            return m_error;
        }

        public void LoadDataFromDisk()
        {
            ExclusivePerkInfoList infoList;
            try
            {
                infoList = ModJsonUtils.DeserializeStream<ExclusivePerkInfoList>(Path.Combine(ModCore.modUserDataFolder, FILE_NAME));
            }
            catch
            {
                infoList = new ExclusivePerkInfoList();
            }
            infoList.FixValues();

            m_perksData = infoList;
        }

        public void LoadDataFromRepository(Action<string> callback)
        {
            RepositoryManager repositoryManager = RepositoryManager.Instance;
            repositoryManager.GetTextFile(FILE_NAME, delegate (string contents)
            {
                ModFileUtils.WriteText(contents, Path.Combine(ModCore.modUserDataFolder, FILE_NAME));

                ExclusivePerkInfoList contentInfoList = null;
                try
                {
                    contentInfoList = ModJsonUtils.Deserialize<ExclusivePerkInfoList>(contents);
                    contentInfoList.FixValues();
                }
                catch (Exception exc)
                {
                    callback?.Invoke(exc.ToString());
                    return;
                }

                m_perksData = contentInfoList;
                callback?.Invoke(null);

                ScheduledActionsManager.Instance.SetActionExecuted(ScheduledActionType.RefreshExclusivePerks);
            }, delegate (string error)
            {
                callback?.Invoke(error);
            }, out _, 20);
        }

        public ExclusivePerkInfoList GetPerkInfoList()
        {
            return m_perksData;
        }

        public List<ExclusivePerkInfo> GetPerks()
        {
            return m_perksData?.List;
        }

        public List<ExclusivePerkInfo> GetUnlockedPerks()
        {
            List<ExclusivePerkInfo> list = new List<ExclusivePerkInfo>();
            if (m_perksData != null && m_perksData.List != null && m_perksData.List.Count != 0)
                foreach (ExclusivePerkInfo info in m_perksData.List)
                {
                    if (info.IsUnlocked())
                        list.Add(info);
                }

            return list;
        }

        public List<ExclusivePerkInfo> GetUnlockedPerksForUser(string playFabId, CSteamID steamId)
        {
            List<ExclusivePerkInfo> list = new List<ExclusivePerkInfo>();
            if (m_perksData != null && m_perksData.List != null && m_perksData.List.Count != 0)
                foreach (ExclusivePerkInfo info in m_perksData.List)
                {
                    if (info.IsUnlockedForUser(playFabId, steamId))
                        list.Add(info);
                }

            return list;
        }

        public List<ExclusivePerkInfo> GetPerksOfType(ExclusivePerkType perkType)
        {
            List<ExclusivePerkInfo> list = new List<ExclusivePerkInfo>();
            if (m_perksData != null && m_perksData.List != null && m_perksData.List.Count != 0)
                foreach (ExclusivePerkInfo info in m_perksData.List)
                {
                    if (info.PerkType == perkType)
                        list.Add(info);
                }

            return list;
        }

        public List<ExclusivePerkInfo> GetUnlockedPerksOfType(ExclusivePerkType perkType)
        {
            List<ExclusivePerkInfo> list = new List<ExclusivePerkInfo>();
            if (m_perksData != null && m_perksData.List != null && m_perksData.List.Count != 0)
                foreach (ExclusivePerkInfo info in m_perksData.List)
                {
                    if (info.PerkType == perkType && info.IsUnlocked())
                        list.Add(info);
                }

            return list;
        }

        public List<ExclusivePerkInfo> GetUnlockedPerksOfTypeForUser(ExclusivePerkType perkType, string playFabId, CSteamID steamId)
        {
            List<ExclusivePerkInfo> list = new List<ExclusivePerkInfo>();
            if (m_perksData != null && m_perksData.List != null && m_perksData.List.Count != 0)
                foreach (ExclusivePerkInfo info in m_perksData.List)
                {
                    if (info.PerkType == perkType && info.IsUnlockedForUser(playFabId, steamId))
                        list.Add(info);
                }

            return list;
        }

        public void GetOverrideRobotColor(FirstPersonMover firstPersonMover, Color oldColor, out Color newColor)
        {
            newColor = oldColor;
            if (!firstPersonMover)
                return;

            string robotPlayFabId = GameModeManager.IsSinglePlayer() ? ModUserInfo.localPlayerPlayFabID : firstPersonMover.GetPlayFabID();
            if (robotPlayFabId.IsNullOrEmpty())
                return;

            foreach (ExclusivePerkInfo exclusiveContentInfo in GetPerksOfType(ExclusivePerkType.Color))
            {
                if (exclusiveContentInfo.PlayFabID == robotPlayFabId)
                {
                    object deserializedData = exclusiveContentInfo.DeserializeData();
                    if (deserializedData != null && deserializedData is ExclusivePerkColor perkColor)
                    {
                        int index = -1;
                        foreach (HumanFavouriteColor favColor in HumanFactsManager.Instance.FavouriteColors)
                        {
                            index++;
                            if (favColor.ColorValue == oldColor)
                                break;
                        }

                        if (index == perkColor.Index)
                        {
                            newColor = perkColor.NewColor;
                        }
                    }
                }
            }
        }

        public bool IsFeatureUnlocked(ModFeatures.FeatureType featureType)
        {
            foreach (ExclusivePerkInfo content in GetUnlockedPerksOfType(ExclusivePerkType.Feature))
            {
                object deserializedData = content.DeserializeData();
                if (deserializedData != null && deserializedData is int feature && feature == (int)featureType)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsLocalUserTheTester()
        {
            return HasUnlockedPerk(ExclusivePerkType.TesterRole);
        }

        public bool IsLocalUserAbleToVerifyItems()
        {
            return HasUnlockedPerk(ExclusivePerkType.CustomizationItemsVerifierRole);
        }

        public bool HasUnlockedPerk(ExclusivePerkType perkType)
        {
            foreach (ExclusivePerkInfo content in GetUnlockedPerks())
                if (content.PerkType == perkType)
                    return true;

            return false;
        }

        public bool HasUnlockedPerkForUser(ExclusivePerkType perkType, string playFabId, CSteamID steamId)
        {
            foreach (ExclusivePerkInfo content in GetUnlockedPerksForUser(playFabId, steamId))
                if (content.PerkType == perkType)
                    return true;

            return false;
        }
    }
}
