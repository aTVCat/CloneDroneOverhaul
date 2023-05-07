using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Workshop
{
    public static class OverhaulSteamBrowser
    {
        public static readonly AppId_t AppID = new AppId_t(597170U);

        private static readonly List<ulong> m_KnownUserIDs = new List<ulong>();

        #region Workshop Items

        public static void RequestItems(EUGCQuery query, EUGCMatchingUGCType typeOfContent, Action<OverhaulWorkshopRequestResult> completedCallback, OverhaulRequestProgressInfo progressInfo, string tag, int page, bool cache = false)
        {
            if (completedCallback == null)
            {
                return;
            }

            OverhaulWorkshopRequestResult requestResult = new OverhaulWorkshopRequestResult();
            OverhaulRequestProgressInfo.SetProgress(progressInfo, 0f);

            UGCQueryHandle_t request = SteamUGC.CreateQueryAllUGCRequest(query, typeOfContent, AppID, AppID, (uint)page);
            if (!CheckRequest(request))
            {
                requestResult.Error = true;
                Debug.LogWarning("[OverhaulMod] Request is invalid (Query: " + query + ", Content type: " + typeOfContent + ", Page: " + page + ", Cache: " + cache);
                completedCallback.Invoke(requestResult);
                return;
            }

            if (cache) SteamUGC.SetAllowCachedResponse(request, 6);
            SteamUGC.AddRequiredTag(request, tag);

            SendUGCRequest(request, requestResult, completedCallback, progressInfo);
        }

        public static void SendUGCRequest(UGCQueryHandle_t request, OverhaulWorkshopRequestResult requestResult, Action<OverhaulWorkshopRequestResult> completedCallback, OverhaulRequestProgressInfo progressInfo)
        {
            if (completedCallback == null)
            {
                return;
            }
            if (requestResult == null)
            {
                requestResult = new OverhaulWorkshopRequestResult();
            }

            if (!CheckRequest(request))
            {
                requestResult.Error = true;
                Debug.LogWarning("[OverhaulMod] Cannot send UGC request");
                completedCallback.Invoke(requestResult);
                return;
            }

            OverhaulRequestProgressInfo.SetProgress(progressInfo, 0.4f);
            SteamAPICall_t apiCall = SteamUGC.SendQueryUGCRequest(request);
            if (apiCall == SteamAPICall_t.Invalid)
            {
                requestResult.Error = true;
                Debug.LogWarning("[OverhaulMod] APICall is invalid");
                completedCallback.Invoke(requestResult);
                return;
            }
            CallResult<SteamUGCQueryCompleted_t> apiCallResult = CallResult<SteamUGCQueryCompleted_t>.Create(delegate (SteamUGCQueryCompleted_t queryResult, bool bIOFailure)
            {
                if (bIOFailure)
                {
                    requestResult.Error = true;
                    Debug.LogWarning("[OverhaulMod] APICallResult had IO error!");
                    completedCallback.Invoke(requestResult);
                    return;
                }

                OverhaulRequestProgressInfo.SetProgress(progressInfo, 0.7f);
                OverhaulWorkshopItem[] items = new OverhaulWorkshopItem[queryResult.m_unNumResultsReturned];
                int num = 0;
                while (num < (int)queryResult.m_unNumResultsReturned)
                {
                    if (SteamUGC.GetQueryUGCResult(queryResult.m_handle, (uint)num, out SteamUGCDetails_t steamUGCDetails_t) && steamUGCDetails_t.m_eResult == EResult.k_EResultOK && !steamUGCDetails_t.m_bBanned)
                    {
                        SteamUGC.GetQueryUGCPreviewURL(queryResult.m_handle, (uint)num, out string url, 4096U);
                        items[num] = new OverhaulWorkshopItem(steamUGCDetails_t)
                        {
                            ThumbnailURL = url
                        };
                    }
                    num++;
                }
                requestResult.QueryCompleted = queryResult;
                requestResult.ItemsReceived = items;
                OverhaulRequestProgressInfo.SetProgress(progressInfo, 1f);
                completedCallback.Invoke(requestResult);
            });
            apiCallResult.Set(apiCall);
        }

        public static bool CheckRequest(UGCQueryHandle_t request)
        {
            if (request == UGCQueryHandle_t.Invalid)
            {
                Debug.LogWarning("[OverhaulMod] Request is invalid");
                return false;
            }
            return true;
        }

        #endregion

        public static bool RequestInfoAboutUser(CSteamID userID)
        {
            if (m_KnownUserIDs.Contains(userID.m_SteamID))
            {
                return false;
            }

            bool result = SteamFriends.RequestUserInformation(userID, false);
            if (!m_KnownUserIDs.Contains(userID.m_SteamID))
            {
                m_KnownUserIDs.Add(userID.m_SteamID);
            }
            return result;
        }

        public static Texture GetUserAvatar(CSteamID userID)
        {
            if (!m_KnownUserIDs.Contains(userID.m_SteamID))
            {
                return null;
            }

            int d = SteamFriends.GetMediumFriendAvatar(userID);
            if (d == 0)
            {
                return null;
            }

            bool success = SteamUtils.GetImageSize(d, out uint width, out uint height);
            if (!success)
            {
                return null;
            }

            int size = (int)width * (int)height * 4;
            byte[] pixels = new byte[size];

            bool success2 = SteamUtils.GetImageRGBA(d, pixels, size);
            return !success2 ? null : (Texture)null;
        }
    }
}
