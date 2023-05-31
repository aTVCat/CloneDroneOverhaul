using Steamworks;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CDOverhaul.Workshop
{
    public class OverhaulWorkshopItem : OverhaulDisposable
    {
        protected override void OnDisposed()
        {
            OverhaulDisposable.AssignNullToAllVars(this);
        }

        public OverhaulWorkshopItem(SteamUGCDetails_t details, string thumbnailURL)
        {
            string fileSizeString = (details.m_nFileSize / (float)1024 / 1024).ToString();
            if (fileSizeString.Length > 6)
                fileSizeString = fileSizeString.Remove(6);

            TimeCreated = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(details.m_rtimeCreated).ToLocalTime();
            TimeUpdated = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(details.m_rtimeUpdated).ToLocalTime();

            OverhaulSteamBrowser.RequestInfoAboutUser(CreatorID);

            ItemDetails = details;
            ItemID = details.m_nPublishedFileId;
            ItemTags = details.m_rgchTags.Split(",".ToCharArray());
            ItemTitle = details.m_rgchTitle;
            ItemLongDescription = details.m_rgchDescription;
            CreatorID = (CSteamID)details.m_ulSteamIDOwner;
            CreatorNickname = SteamFriends.GetFriendPersonaName(CreatorID);

            ItemURL = "https://steamcommunity.com/sharedfiles/filedetails/?id=" + details.m_nPublishedFileId;
            PreviewURL = thumbnailURL;
            ItemAdditionalImages = new List<string>();
            ItemSizeString = fileSizeString + " MB";

            UpVotesCount = (int)details.m_unVotesUp;
            DownVotesCount = (int)details.m_unVotesDown;
            Stars = Mathf.Min((details.m_flScore * 5f) + 1f, 5f);

            RefreshInstallInfo();
        }

        public SteamWorkshopItem ToSteamWorkshopItem()
        {
            RefreshAllInfos();

            SteamWorkshopItem steamWorkshopItem = new SteamWorkshopItem
            {
                CreatorName = CreatorNickname,
                CreatorID = CreatorID.m_SteamID,
                WorkshopItemID = ItemID,
                Votes = TotalVotes,
                UpVotes = UpVotesCount,
                Description = ItemLongDescription,
                Title = ItemTitle,
                PreviewURL = PreviewURL,
                Folder = FolderPath,
                Children = new PublishedFileId_t[] { },
                Tags = ItemTags,
                TimeCreated = ItemDetails.m_rtimeCreated,
                TimeUpdated = ItemDetails.m_rtimeUpdated
            };
            return steamWorkshopItem;
        }

        public void RefreshAllInfos(Action callback = null)
        {
            RefreshCreatorInfo();
            RefreshInstallInfo();
            if (callback != null && DelegateScheduler.Instance != null) DelegateScheduler.Instance.Schedule(callback, 2f);
        }

        public void RefreshInstallInfo()
        {
            SteamUGC.GetItemInstallInfo(ItemDetails.m_nPublishedFileId, out ulong num, out string folder, 4096, out uint num2);
            FolderPath = folder;
        }

        public void RefreshCreatorInfo()
        {
            SteamFriends.RequestUserInformation(CreatorID, false);
            if (DelegateScheduler.Instance != null) DelegateScheduler.Instance.Schedule(delegate
            {
                if (!IsDisposed)
                {
                    CreatorNickname = SteamFriends.GetFriendPersonaName(CreatorID);
                }
            }, 1.5f);
        }

        public SteamUGCDetails_t ItemDetails
        {
            get;
            private set;
        }
        public PublishedFileId_t ItemID
        {
            get;
            private set;
        }
        public string[] ItemTags
        {
            get;
            private set;
        }

        public DateTime TimeCreated
        {
            get;
            private set;
        }
        public DateTime TimeUpdated
        {
            get;
            private set;
        }

        public string ItemTitle
        {
            get;
            private set;
        }
        public string ItemLongDescription
        {
            get;
            private set;
        }
        public CSteamID CreatorID
        {
            get;
            private set;
        }
        public string CreatorNickname
        {
            get;
            private set;
        }

        public string ItemURL
        {
            get;
            private set;
        }
        public string PreviewURL
        {
            get;
            private set;
        }
        public string FolderPath
        {
            get;
            private set;
        }
        public string ItemSizeString
        {
            get;
            private set;
        }

        public new List<string> ItemAdditionalImages
        {
            get;
            set;
        }

        public int TotalVotes => UpVotesCount + DownVotesCount;
        public int UpVotesCount
        {
            get;
            private set;
        }
        public int DownVotesCount
        {
            get;
            private set;
        }
        public float Stars
        {
            get;
            private set;
        }
    }
}