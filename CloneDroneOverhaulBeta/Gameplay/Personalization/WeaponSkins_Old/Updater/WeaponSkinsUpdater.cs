using CDOverhaul.NetworkAssets;
using ModLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace CDOverhaul.Gameplay
{
    public static class WeaponSkinsUpdater
    {
        public static bool HasUpdates
        {
            get;
            private set;
        }

        public static bool UnableToCheck
        {
            get;
            private set;
        }

        public static bool WaitsToBeUpdated
        {
            get;
            private set;
        }

        public static bool IsDownloadingUpdateFiles
        {
            get;
            private set;
        }

        public static bool IsDownloadingSkinsVersionFile
        {
            get;
            private set;
        }

        private static readonly Dictionary<string, List<WeaponSkinsImportedItemDefinition>> m_SkinsWaitingABToDownload = new Dictionary<string, List<WeaponSkinsImportedItemDefinition>>();

        public static float GetUpdateFilesDownloadProgress()
        {
            return IsDownloadingSkinsVersionFile && m_SkinsVersionFileDH != null
                ? m_SkinsVersionFileDH.Progress
                : !IsDownloadingUpdateFiles
                ? 0f
                : m_SkinsImportFileDH == null || m_SkinsAssetBundleFileDH == null
                ? 1f
                : (m_SkinsAssetBundleFileDH.Progress + m_SkinsImportFileDH.Progress) / 2;
        }

        public static float TimeUpdatesGotRefreshed = 0f;
        public static float TimeToAllowUpdateRefreshing = 1f;
        public static bool IsAbleToRefreshUpdates => WaitsToBeUpdated || Time.unscaledTime >= TimeToAllowUpdateRefreshing;
        public static float CooldownFillAmount => !WaitsToBeUpdated ? 1f - Mathf.Clamp01((Time.unscaledTime - TimeUpdatesGotRefreshed) / (TimeToAllowUpdateRefreshing - TimeUpdatesGotRefreshed)) : 0f;

        private static OverhaulDownloadInfo m_SkinsVersionFileDH;
        private static OverhaulDownloadInfo m_SkinsImportFileDH;
        private static OverhaulDownloadInfo m_SkinsAssetBundleFileDH;

        private static string m_DownloadedVersionString;

        private static readonly bool[] m_DownloadProgress = new bool[2] { false, false };
        private static Action m_OnEndedUpdatingSkins;

        public const string SkinsAssetBundleFileAddress = "https://raw.githubusercontent.com/aTVCat/CloneDroneOverhaul/MayBranch/CloneDroneOverhaulBeta/CompiledBuild/CloneDroneOverhaul/overhaulassets_skins";

        public const string SkinsImportFileAddress = "https://raw.githubusercontent.com/aTVCat/CloneDroneOverhaul/MayBranch/CloneDroneOverhaulBeta/CompiledBuild/CloneDroneOverhaul/Assets/Download/Permanent/ImportedSkins.json";
        public const string SkinsVersionFileAddress = "https://raw.githubusercontent.com/aTVCat/CloneDroneOverhaul/MayBranch/CloneDroneOverhaulBeta/CompiledBuild/CloneDroneOverhaul/SkinsVersion.txt";

        public static string GetFullStateString()
        {
            return GetAssetBundleFileStateString() + '\n' + GetUpdateStateString();
        }

        public static string GetAssetBundleFileStateString()
        {
            return OverhaulLocalizationManager.GetTranslation("FileVersion:") + WeaponSkinsController.GetSkinsFileVersion();
        }

        public static string GetUpdateStateString()
        {
            return !HasUpdates ? UnableToCheck ? "Error".AddColor(Color.red) : OverhaulLocalizationManager.GetTranslation("NoUpdatesAvailable") : OverhaulLocalizationManager.GetTranslation("UpdatesAvailable").AddColor(UnityEngine.Color.red);
        }

        public static string GetUpdateButtonText()
        {
            return WaitsToBeUpdated ? OverhaulLocalizationManager.GetTranslation("UpdateSkins") : OverhaulLocalizationManager.GetTranslation("CheckForUpdates");
        }

        public static void RefreshUpdates(Action onRefreshed)
        {
            if (!IsAbleToRefreshUpdates)
            {
                onRefreshed?.Invoke();
                return;
            }
            TimeUpdatesGotRefreshed = Time.unscaledTime;
            TimeToAllowUpdateRefreshing = Time.unscaledTime + 10f;

            IsDownloadingUpdateFiles = true;
            if (WaitsToBeUpdated && !UnableToCheck)
            {
                m_OnEndedUpdatingSkins = onRefreshed;
                m_SkinsImportFileDH = OverhaulNetworkAssetsController.DownloadData(SkinsImportFileAddress, onDownloadedSkinsImportFile);
                m_SkinsAssetBundleFileDH = OverhaulNetworkAssetsController.DownloadData(SkinsAssetBundleFileAddress, onDownloadedSkinsAssetBundleFile);
                return;
            }

            IsDownloadingSkinsVersionFile = true;
            UnityWebRequest.ClearCookieCache();
            Action action = new Action(onDownloadedSkinsVersionFile).Combine(onRefreshed);
            m_SkinsVersionFileDH = OverhaulNetworkAssetsController.DownloadData(SkinsVersionFileAddress, action);
        }

        private static void onDownloadedSkinsImportFile()
        {
            if (m_SkinsImportFileDH == null || m_SkinsImportFileDH.Error)
            {
                UnableToCheck = true;
                endUpdating(false);
                return;
            }

            m_DownloadProgress[1] = true;
            File.WriteAllText(OverhaulMod.Core.ModDirectory + "Assets/Download/Permanent/ImportedSkins.json", m_SkinsImportFileDH.DownloadedText);

            if (!m_DownloadProgress.Contains(false) && m_OnEndedUpdatingSkins != null)
            {
                File.WriteAllText(OverhaulMod.Core.ModDirectory + "SkinsVersion.txt", m_DownloadedVersionString);
                endUpdating();
            }
        }

        private static void onDownloadedSkinsAssetBundleFile()
        {
            if (m_SkinsAssetBundleFileDH == null || m_SkinsAssetBundleFileDH.Error)
            {
                UnableToCheck = true;
                endUpdating(false);
                return;
            }

            m_DownloadProgress[0] = true;
            AssetLoader.ClearCache();
            File.WriteAllBytes(OverhaulMod.Core.ModDirectory + "overhaulassets_skins", m_SkinsAssetBundleFileDH.DownloadedData);

            if (!m_DownloadProgress.Contains(false) && m_OnEndedUpdatingSkins != null)
            {
                File.WriteAllText(OverhaulMod.Core.ModDirectory + "SkinsVersion.txt", m_DownloadedVersionString);
                endUpdating();
            }
        }

        private static void onDownloadedSkinsVersionFile()
        {
            HasUpdates = false;
            UnableToCheck = false;
            IsDownloadingUpdateFiles = false;
            IsDownloadingSkinsVersionFile = false;

            if (m_SkinsVersionFileDH == null || m_SkinsVersionFileDH.Error)
            {
                UnableToCheck = true;
                endUpdating(false);
                return;
            }
            m_DownloadedVersionString = m_SkinsVersionFileDH.DownloadedText;
            m_DownloadedVersionString = System.Text.RegularExpressions.Regex.Replace(m_DownloadedVersionString, @"\r\n?|\n", string.Empty);
            System.Version downloadedVersion = System.Version.Parse(m_DownloadedVersionString);
            System.Version currentVersion = System.Version.Parse(WeaponSkinsController.GetSkinsFileVersion());
            HasUpdates = currentVersion < downloadedVersion;
            if (HasUpdates)
            {
                WaitsToBeUpdated = true;
            }
        }

        private static void endUpdating(bool success = true)
        {
            m_OnEndedUpdatingSkins?.Invoke();

            IsDownloadingUpdateFiles = false;
            if (!success)
            {
                return;
            }

            WeaponSkinsController.DeleteCustomAssetBundleFiles();
            WeaponSkinsController.ReloadAllModels();
            HasUpdates = false;
            WaitsToBeUpdated = false;

            m_DownloadProgress[0] = false;
            m_DownloadProgress[1] = false;
        }

        public static void DownloadAssetBundleThenAddSkin(WeaponSkinsImportedItemDefinition importedSkin, string assetBundle)
        {
            if (!m_SkinsWaitingABToDownload.ContainsKey(assetBundle))
            {
                m_SkinsWaitingABToDownload.Add(assetBundle, new List<WeaponSkinsImportedItemDefinition>() { importedSkin });

                OverhaulDownloadInfo h = null;
                h = OverhaulNetworkAssetsController.DownloadAndSaveData("https://raw.githubusercontent.com/aTVCat/CloneDroneOverhaul/MayBranch/CloneDroneOverhaulBeta/CompiledBuild/CloneDroneOverhaul/" + assetBundle, OverhaulMod.Core.ModDirectory, assetBundle, delegate
                {
                    WeaponSkinsController c = OverhaulController.Get<WeaponSkinsController>();
                    if (h != null && !h.Error && c != null && m_SkinsWaitingABToDownload.ContainsKey(assetBundle) && !m_SkinsWaitingABToDownload[assetBundle].IsNullOrEmpty())
                    {
                        foreach (WeaponSkinsImportedItemDefinition def in m_SkinsWaitingABToDownload[assetBundle])
                        {
                            if (OverhaulAssetsController.DoesAssetBundleExist(assetBundle))
                                c.ImportSkin(def, assetBundle);
                        }
                    }
                    //m_SkinsWaitingABToDownload.Remove(assetBundle);
                });
            }
            else
            {
                List<WeaponSkinsImportedItemDefinition> list = m_SkinsWaitingABToDownload[assetBundle];
                list.Add(importedSkin);
            }
        }
    }
}
