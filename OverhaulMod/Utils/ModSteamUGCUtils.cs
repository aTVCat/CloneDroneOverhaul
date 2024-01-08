using Steamworks;
using System;
using System.Collections.Generic;

namespace OverhaulMod.Utils
{
    public static class ModSteamUGCUtils
    {
        /// <summary>
        /// Amount of pages from last query execution
        /// </summary>
        public static int pageCount
        {
            get;
            private set;
        }
#if DEBUG

        public static List<SteamWorkshopItem> Debug_Items;
        public static SteamWorkshopItem Debug_Item;
        public static RequestParameters Debug_RequestParameters = new RequestParameters();
        public static string Debug_Error;
        public static bool Debug_LastQueryIsCached;
        public static int Debug_LastQueryResults;
        public static int Debug_LastQueryMatchingResults;

        public static void Debug_RequestAllWorkshopItems(EUGCQuery rankBy, EUGCMatchingUGCType itemsType, int page)
        {
            Debug_Items = null;
            Debug_Error = null;
            Debug_LastQueryIsCached = false;
            Debug_LastQueryResults = -1;
            Debug_LastQueryMatchingResults = -1;
            _ = GetAllWorkshopItems(rankBy, itemsType, page, Debug_RequestParameters, delegate (List<SteamWorkshopItem> list)
            {
                Debug_Items = list;
            }, delegate (string error)
            {
                Debug_Error = error;
            }, delegate (bool cached, int results, int matching)
            {
                Debug_LastQueryIsCached = cached;
                Debug_LastQueryResults = results;
                Debug_LastQueryMatchingResults = matching;
            });
        }

        public static void Debug_RequestAllWorkshopItems(CSteamID steamId, int page, EUserUGCList userList, EUGCMatchingUGCType itemsType, EUserUGCListSortOrder sortOrder)
        {
            Debug_Items = null;
            Debug_Error = null;
            Debug_LastQueryIsCached = false;
            Debug_LastQueryResults = -1;
            Debug_LastQueryMatchingResults = -1;
            _ = GetWorkshopUserItems(steamId, page, userList, itemsType, sortOrder, Debug_RequestParameters, delegate (List<SteamWorkshopItem> list)
            {
                Debug_Items = list;
            }, delegate (string error)
            {
                Debug_Error = error;
            }, delegate (bool cached, int results, int matching)
            {
                Debug_LastQueryIsCached = cached;
                Debug_LastQueryResults = results;
                Debug_LastQueryMatchingResults = matching;
            });
        }
#endif


        /// <summary>
        /// Execute <see cref="SteamUGC.CreateQueryAllUGCRequest(EUGCQuery, EUGCMatchingUGCType, AppId_t, AppId_t, uint)"/>
        /// </summary>
        /// <param name="rankBy"></param>
        /// <param name="itemsType"></param>
        /// <param name="page"></param>
        /// <param name="errorCallback"></param>
        /// <param name="debugCallback"></param>
        public static bool GetAllWorkshopItems(EUGCQuery rankBy, EUGCMatchingUGCType itemsType, int page, RequestParameters requestParameters, Action<List<SteamWorkshopItem>> callback, Action<string> errorCallback, Action<bool, int, int> debugCallback)
        {
            if (page < 1 || requestParameters == null)
                return false;

            UGCQueryHandle_t queryHandle = createAllWorkshopItemsRequest(rankBy, itemsType, page);
            requestParameters.ConfigureQuery(queryHandle);
            doRequestWorkshopItems(queryHandle, delegate (SteamUGCQueryCompleted_t queryResult, bool io)
            {
                onQueryCallback_ManyItems(queryResult, io, callback, errorCallback, debugCallback);
            });
            return true;
        }

        public static bool GetWorkshopUserItems(CSteamID steamId, int page, EUserUGCList userList, EUGCMatchingUGCType itemsType, EUserUGCListSortOrder sortOrder, RequestParameters requestParameters, Action<List<SteamWorkshopItem>> callback, Action<string> errorCallback, Action<bool, int, int> debugCallback)
        {
            if (page < 1 || requestParameters == null || steamId == default)
                return false;

            UGCQueryHandle_t queryHandle = createWorkshopUserItemsRequest(steamId, page, userList, itemsType, sortOrder);
            requestParameters.ConfigureQuery(queryHandle);
            doRequestWorkshopItems(queryHandle, delegate (SteamUGCQueryCompleted_t queryResult, bool io)
            {
                onQueryCallback_ManyItems(queryResult, io, callback, errorCallback, debugCallback);
            });
            return true;
        }

        private static void onQueryCallback_ManyItems(SteamUGCQueryCompleted_t queryResult, bool ioError, Action<List<SteamWorkshopItem>> callback, Action<string> errorCallback, Action<bool, int, int> debugInfo)
        {
            if (!checkQueryResultForErrors(queryResult, ioError, errorCallback))
            {
                releaseQueryHandle(queryResult.m_handle);
                return;
            }

            setPageAmount(queryResult.m_unTotalMatchingResults);
            callback?.Invoke(getQueryItems(queryResult));
            debugInfo?.Invoke(queryResult.m_bCachedData, (int)queryResult.m_unNumResultsReturned, (int)queryResult.m_unTotalMatchingResults);
            releaseQueryHandle(queryResult.m_handle);
        }

        private static void onQueryCallback_OneItem(SteamUGCQueryCompleted_t queryResult, bool ioError, Action<SteamWorkshopItem> callback, Action<string> errorCallback)
        {
            if (!checkQueryResultForErrors(queryResult, ioError, errorCallback))
            {
                releaseQueryHandle(queryResult.m_handle);
                return;
            }

            List<SteamWorkshopItem> list = getQueryItems(queryResult);
            if (list == null && list.Count != 0 && list[0] != null)
                callback?.Invoke(list[0]);
            else
                errorCallback?.Invoke("Item reference error.");
            releaseQueryHandle(queryResult.m_handle);
        }

        private static List<SteamWorkshopItem> getQueryItems(SteamUGCQueryCompleted_t queryResult)
        {
            List<SteamWorkshopItem> items = new List<SteamWorkshopItem>();
            uint num = 0;
            while (num < (long)(ulong)queryResult.m_unNumResultsReturned)
            {
                if (SteamUGC.GetQueryUGCResult(queryResult.m_handle, num, out SteamUGCDetails_t ugcDetails) && ugcDetails.m_eResult == EResult.k_EResultOK && !ugcDetails.m_bBanned)
                    items.Add(SteamWorkshopManager.WorkshopItemFromQueryDetails(queryResult, (int)num, ugcDetails));

                num++;
            }
            return items;
        }

        private static bool checkQueryResultForErrors(SteamUGCQueryCompleted_t queryResult, bool ioError, Action<string> errorCallback)
        {
            if (ioError)
            {
                errorCallback?.Invoke("I/O Error.");
                return false;
            }

            EResult result = queryResult.m_eResult;
            if (result != EResult.k_EResultOK)
            {
                errorCallback?.Invoke("Error: " + result + ".");
                return false;
            }
            return true;
        }

        private static UGCQueryHandle_t createAllWorkshopItemsRequest(EUGCQuery rankBy, EUGCMatchingUGCType itemsType, int page)
        {
            AppId_t appId = SteamUtils.GetAppID();
            return SteamUGC.CreateQueryAllUGCRequest(rankBy, itemsType, appId, appId, (uint)page);
        }

        private static UGCQueryHandle_t createWorkshopUserItemsRequest(CSteamID accountId, int page, EUserUGCList userList, EUGCMatchingUGCType itemsType, EUserUGCListSortOrder sortOrder)
        {
            AppId_t appId = SteamUtils.GetAppID();
            return SteamUGC.CreateQueryUserUGCRequest(accountId.GetAccountID(), userList, itemsType, sortOrder, appId, appId, (uint)page);
        }

        private static void doRequestWorkshopItems(UGCQueryHandle_t queryHandle, Action<SteamUGCQueryCompleted_t, bool> callback)
        {
            CallResult<SteamUGCQueryCompleted_t>.Create(null).Set(SteamUGC.SendQueryUGCRequest(queryHandle), delegate (SteamUGCQueryCompleted_t c, bool io)
            {
                callback?.Invoke(c, io);
            });
        }

        private static void releaseQueryHandle(UGCQueryHandle_t queryHandle)
        {
            _ = SteamUGC.ReleaseQueryUGCRequest(queryHandle);
        }

        private static void setPageAmount(uint matchingResults)
        {
            pageCount = (int)((matchingResults / 50U) + 1U);
        }

        public class RequestParameters
        {
            private List<string> m_requiredTags;

            private string m_searchText;

            private bool m_enableCaching;

            private bool m_returnChildren;

            private bool m_returnPreviews;

            private bool m_returnLongDescription;

            public static RequestParameters Create()
            {
                return new RequestParameters();
            }

            public void EnableCaching()
            {
                m_enableCaching = true;
            }

            public void ReturnChildren()
            {
                m_returnChildren = true;
            }

            public void ReturnPreviews()
            {
                m_returnPreviews = true;
            }

            public void ReturnLongDescription()
            {
                m_returnLongDescription = true;
            }

            public void RequireTags(List<string> tags)
            {
                m_requiredTags = tags;
            }

            public void SearchText(string text)
            {
                m_searchText = text;
            }

            public void ConfigureQuery(UGCQueryHandle_t queryHandle)
            {
                if (m_requiredTags != null && m_requiredTags.Count != 0)
                    foreach (string tag in m_requiredTags)
                        _ = SteamUGC.AddRequiredTag(queryHandle, tag);

                if (!string.IsNullOrEmpty(m_searchText))
                    _ = SteamUGC.SetSearchText(queryHandle, m_searchText);

                if (m_enableCaching)
                    _ = SteamUGC.SetAllowCachedResponse(queryHandle, 5);

                if (m_returnChildren)
                    _ = SteamUGC.SetReturnChildren(queryHandle, true);

                if (m_returnPreviews)
                    _ = SteamUGC.SetReturnAdditionalPreviews(queryHandle, true);

                if (m_returnLongDescription)
                    _ = SteamUGC.SetReturnLongDescription(queryHandle, true);
            }

            public bool IsCachingEnabled()
            {
                return m_enableCaching;
            }
        }
    }
}
