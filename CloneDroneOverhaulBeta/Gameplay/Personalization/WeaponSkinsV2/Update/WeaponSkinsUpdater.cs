using CDOverhaul.NetworkAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private static OverhaulNetworkDownloadHandler m_SkinsVersionFileDH;
        private static OverhaulNetworkDownloadHandler m_SkinsImportFileDH;
        private static OverhaulNetworkDownloadHandler m_SkinsAssetBundleFileDH;

        private static string m_DownloadedVersionString;

        private static readonly bool[] m_DownloadProgress = new bool[2] { false, false };
        private static Action m_OnEndedUpdatingSkins;

        public const string SkinsAssetBundleFileAddress = "https://raw.githubusercontent.com/aTVCat/CloneDroneOverhaul/Ver1And2Features/CloneDroneOverhaulBeta/CompiledBuild/CloneDroneOverhaul/overhaulassets_skins";

        public const string SkinsImportFileAddress = "https://raw.githubusercontent.com/aTVCat/CloneDroneOverhaul/Ver1And2Features/CloneDroneOverhaulBeta/CompiledBuild/CloneDroneOverhaul/Assets/Download/Permanent/ImportedSkins.json";
        public const string SkinsVersionFileAddress = "https://raw.githubusercontent.com/aTVCat/CloneDroneOverhaul/Ver1And2Features/CloneDroneOverhaulBeta/CompiledBuild/CloneDroneOverhaul/SkinsVersion.txt";

        public static string GetFullStateString()
        {
            return GetAssetBundleFileStateString() + '\n' + GetUpdateStateString();
        }

        public static string GetAssetBundleFileStateString()
        {
            return OverhaulLocalizationController.GetTranslation("FileVersion:") + WeaponSkinsController.GetSkinsFileVersion();
        }

        public static string GetUpdateStateString()
        {
            return !HasUpdates ? UnableToCheck ? "Error".AddColor(Color.red) : OverhaulLocalizationController.GetTranslation("NoUpdatesAvailable") : OverhaulLocalizationController.GetTranslation("UpdatesAvailable").AddColor(UnityEngine.Color.red);
        }

        public static string GetUpdateButtonText()
        {
            return WaitsToBeUpdated ? OverhaulLocalizationController.GetTranslation("UpdateSkins") : OverhaulLocalizationController.GetTranslation("CheckForUpdates");
        }

        public static void RefreshUpdates(Action onRefreshed)
        {
            if (WaitsToBeUpdated && !UnableToCheck)
            {
                m_OnEndedUpdatingSkins = onRefreshed;
                m_SkinsImportFileDH = OverhaulNetworkController.DownloadFile(SkinsImportFileAddress, onDownloadedSkinsImportFile);
                m_SkinsAssetBundleFileDH = OverhaulNetworkController.DownloadFile(SkinsAssetBundleFileAddress, onDownloadedSkinsAssetBundleFile);
                return;
            }

            UnityWebRequest.ClearCookieCache();
            Action action = new Action(onDownloadedSkinsVersionFile).Combine(onRefreshed);
            m_SkinsVersionFileDH = OverhaulNetworkController.DownloadFile(SkinsVersionFileAddress, action);
        }

        private static void onDownloadedSkinsImportFile()
        {
            if (m_SkinsImportFileDH == null || m_SkinsImportFileDH.Error)
            {
                UnableToCheck = true;
                return;
            }

            m_DownloadProgress[1] = true;
            File.WriteAllText(OverhaulMod.Core.ModDirectory + "Assets/Download/Permanent/ImportedSkins.json", m_SkinsImportFileDH.DownloadedText);

            if (!m_DownloadProgress.Contains(false) && m_OnEndedUpdatingSkins != null)
            {
                File.WriteAllText(OverhaulMod.Core.ModDirectory + "SkinsVersion.txt", m_DownloadedVersionString);
                m_OnEndedUpdatingSkins.Invoke();

                WeaponSkinsController.ReloadAllModels();
                HasUpdates = false;
                WaitsToBeUpdated = false;
            }
        }

        private static void onDownloadedSkinsAssetBundleFile()
        {
            if (m_SkinsAssetBundleFileDH == null || m_SkinsAssetBundleFileDH.Error)
            {
                UnableToCheck = true;
                return;
            }

            m_DownloadProgress[0] = true;
            File.WriteAllBytes(OverhaulMod.Core.ModDirectory + "overhaulassets_skins", m_SkinsAssetBundleFileDH.DownloadedData);

            if (!m_DownloadProgress.Contains(false) && m_OnEndedUpdatingSkins != null)
            {
                File.WriteAllText(OverhaulMod.Core.ModDirectory + "SkinsVersion.txt", m_DownloadedVersionString);
                m_OnEndedUpdatingSkins.Invoke();

                WeaponSkinsController.ReloadAllModels();
                HasUpdates = false;
                WaitsToBeUpdated = false;
            }
        }

        private static void onDownloadedSkinsVersionFile()
        {
            HasUpdates = false;
            UnableToCheck = false;

            if (m_SkinsVersionFileDH == null || m_SkinsVersionFileDH.Error)
            {
                UnableToCheck = true;
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
    }
}
