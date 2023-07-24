using Bolt;
using CDOverhaul.DevTools;
using CDOverhaul.Gameplay.Outfits;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace CDOverhaul.Gameplay.Multiplayer
{
    public class OverhaulPlayerInfo : GlobalEventListener
    {
        public const string InfoReceivedEventString = "OnOverhaulPlayerInfoReceived";

        public static OverhaulPlayerInfo LocalOverhaulPlayerInfo
        {
            get;
            private set;
        }

        public static MultiplayerPlayerInfoState LocalPlayerInfoState
        {
            get;
            private set;
        }

        private MultiplayerPlayerInfoState m_PlayerInfoState;

        public Hashtable Hashtable
        {
            get;
            private set;
        }

        public bool HasReceivedData
        {
            get => Hashtable != null;
        }

        public string GetData(string id) => !HasReceivedData ? string.Empty : Hashtable[id].ToString();
        public string GetUserFlags() => !HasReceivedData || !Hashtable.ContainsKey("State.Flags") ? string.Empty : Hashtable["State.Flags"].ToString();

        private void Start()
        {
            m_PlayerInfoState = base.GetComponent<MultiplayerPlayerInfoState>();
            if (m_PlayerInfoState)
            {
                if (m_PlayerInfoState.state.PlayFabID.Equals(OverhaulPlayerIdentifier.GetLocalPlayFabID()))
                {
                    LocalOverhaulPlayerInfo = this;
                    LocalPlayerInfoState = m_PlayerInfoState;
                }

                RequestData();
                return;
            }
        }

        public void CreateAndSendEvent(OverhaulPlayerInfoRefreshEventData data)
        {
            GenericStringForModdingEvent newEvent = GenericStringForModdingEvent.Create(GlobalTargets.AllClients, ReliabilityModes.ReliableOrdered);
            newEvent.EventData = OverhaulPlayerInfoController.PlayerInfoEventPrefix + OverhaulPlayerInfoController.PlayerInfoVersion;
            newEvent.BinaryData = data.SerializeObject();
            newEvent.Send();
        }

        public void RequestData()
        {
            Stopwatch stopwatch = OverhaulProfiler.StartTimer();
            if (OverhaulFeatureAvailabilitySystem.ImplementedInBuild.IsNewPlayerInfoSyncMechanismEnabled)
            {
                UnityEngine.Debug.LogWarning("REQUEST DATA " + OverhaulPlayerIdentifier.GetLocalPlayFabID() + " " + m_PlayerInfoState.state.PlayFabID);
                OverhaulPlayerInfoRefreshEventData eventData = new OverhaulPlayerInfoRefreshEventData
                {
                    SenderPlayFabID = OverhaulPlayerIdentifier.GetLocalPlayFabID(),
                    ReceiverPlayFabID = m_PlayerInfoState.state.PlayFabID,
                    IsRequest = true
                };
                CreateAndSendEvent(eventData);
                stopwatch.StopTimer("OverhaulPlayerInfo.RequestData (NEW)");
            }
            else
            {
                GenericStringForModdingEvent newEvent = GenericStringForModdingEvent.Create(Bolt.GlobalTargets.AllClients, ReliabilityModes.ReliableOrdered);
                newEvent.EventData = "[OverhaulPlayerInfoRequest]@" + m_PlayerInfoState.state.PlayFabID;
                newEvent.Send();
                stopwatch.StopTimer("OverhaulPlayerInfo.RequestData (OLD)");
            }
        }

        public void RefreshData()
        {
            Stopwatch stopwatch = OverhaulProfiler.StartTimer();
            if (OverhaulFeatureAvailabilitySystem.ImplementedInBuild.IsNewPlayerInfoSyncMechanismEnabled)
            {
                UnityEngine.Debug.LogWarning("REFRESH DATA");
                OverhaulPlayerInfoRefreshEventData eventData = new OverhaulPlayerInfoRefreshEventData
                {
                    SenderPlayFabID = OverhaulPlayerIdentifier.GetLocalPlayFabID(),
                    ReceiverPlayFabID = OverhaulPlayerInfoRefreshEventData.RECEIVER_EVERYONE,
                    Hashtable = CreateNewHashtable(),
                    IsAnswer = true
                };
                CreateAndSendEvent(eventData);
                stopwatch.StopTimer("OverhaulPlayerInfo.RefreshData (NEW)");
            }
            else
            {
                GenericStringForModdingEvent newEvent = GenericStringForModdingEvent.Create(Bolt.GlobalTargets.AllClients, ReliabilityModes.ReliableOrdered);
                newEvent.EventData = "[OverhaulPlayerInfoRefresh]@" + OverhaulPlayerIdentifier.GetLocalPlayFabID() + "@" + SerializeData();
                newEvent.Send();
                stopwatch.StopTimer("OverhaulPlayerInfo.RefreshData (OLD)");
            }
        }

        public override void OnEvent(GenericStringForModdingEvent moddedEvent)
        {
            if (moddedEvent == null || string.IsNullOrEmpty(moddedEvent.EventData))
                return;

            if (OverhaulFeatureAvailabilitySystem.ImplementedInBuild.IsNewPlayerInfoSyncMechanismEnabled)
            {
                if (moddedEvent.EventData.StartsWith(OverhaulPlayerInfoController.PlayerInfoEventPrefix))
                {
                    string[] split = moddedEvent.EventData.Split('@');
                    if (split[1] != OverhaulPlayerInfoController.PlayerInfoVersion)
                        return;

                    Stopwatch stopwatch = OverhaulProfiler.StartTimer();
                    OverhaulPlayerInfoRefreshEventData eventData;
                    try
                    {
                        eventData = moddedEvent.BinaryData.DeserializeObject<OverhaulPlayerInfoRefreshEventData>();
                        stopwatch.StopTimer("OverhaulPlayerInfo.OnEvent (NEW)");
                    }
                    catch
                    {
                        OverhaulWebhooksController.ExecuteErrorsWebhook("Could not deserialize OverhaulPlayerInfoRefreshEventData! Version: " + split[1]);
                        stopwatch.StopTimer("OverhaulPlayerInfo.OnEvent (NEW)");
                        return;
                    }

                    if(eventData == default)
                    {
                        OverhaulWebhooksController.ExecuteErrorsWebhook("Event data is DEFAULT! Version: " + split[1]);
                        return;
                    }

                    if(eventData.IsRequest && eventData.IsAnswer)
                    {
                        OverhaulWebhooksController.ExecuteErrorsWebhook("The event is defined as Answer and Request at the same time! Version: " + split[1]);
                        return;
                    }

                    Stopwatch stopwatch2 = OverhaulProfiler.StartTimer();
                    if (eventData.IsRequest)
                    {
                        if(eventData.ReceiverPlayFabID == OverhaulPlayerIdentifier.GetLocalPlayFabID())
                        {
                            UnityEngine.Debug.LogWarning("GET REQUEST DATA " + OverhaulPlayerIdentifier.GetLocalPlayFabID() + " " + eventData.SenderPlayFabID);
                            OverhaulPlayerInfoRefreshEventData newEventData = new OverhaulPlayerInfoRefreshEventData
                            {
                                SenderPlayFabID = OverhaulPlayerIdentifier.GetLocalPlayFabID(),
                                ReceiverPlayFabID = eventData.SenderPlayFabID,
                                Hashtable = CreateNewHashtable(),
                                IsAnswer = true
                            };
                            CreateAndSendEvent(newEventData);
                            stopwatch2.StopTimer("OverhaulPlayerInfo.OnEvent, Request (NEW)");
                        }
                        return;
                    }

                    if (eventData.IsAnswer)
                    {
                        UnityEngine.Debug.LogWarning("GET ANSWER DATA " + eventData.SenderPlayFabID + " " + eventData.ReceiverPlayFabID);
                        if (eventData.ReceiverPlayFabID == OverhaulPlayerIdentifier.GetLocalPlayFabID())
                        {
                            Hashtable = eventData.Hashtable;
                            OverhaulEventsController.DispatchEvent(InfoReceivedEventString, Hashtable);
                        }
                        else if (eventData.ReceiverPlayFabID == OverhaulPlayerInfoRefreshEventData.RECEIVER_EVERYONE && eventData.SenderPlayFabID == m_PlayerInfoState.state.PlayFabID)
                        {
                            Hashtable = eventData.Hashtable;
                            OverhaulEventsController.DispatchEvent(InfoReceivedEventString, Hashtable);
                        }
                        stopwatch2.StopTimer("OverhaulPlayerInfo.OnEvent, Answer (NEW)");
                        return;
                    }
                }
            }
            else
            {
                if (moddedEvent.EventData.StartsWith("[OverhaulPlayerInfoRequest]@"))
                {
                    if (!moddedEvent.EventData.Contains(OverhaulPlayerIdentifier.GetLocalPlayFabID()))
                        return;

                    Stopwatch stopwatch = OverhaulProfiler.StartTimer();
                    GenericStringForModdingEvent newEvent = GenericStringForModdingEvent.Create(Bolt.GlobalTargets.AllClients);
                    newEvent.EventData = "[OverhaulPlayerInfoAnswer]@" + OverhaulPlayerIdentifier.GetLocalPlayFabID() + "@" + SerializeData();
                    newEvent.Send();
                    stopwatch.StopTimer("OverhaulPlayerInfo.OnEvent, Request (OLD)");
                }

                bool isRefresh = moddedEvent.EventData.StartsWith("[OverhaulPlayerInfoRefresh]@");
                if (moddedEvent.EventData.StartsWith("[OverhaulPlayerInfoAnswer]@") || isRefresh)
                {
                    string[] split = moddedEvent.EventData.Split('@');
                    if (split[1].Equals(m_PlayerInfoState.state.PlayFabID))
                    {
                        Stopwatch stopwatch = OverhaulProfiler.StartTimer();
                        Hashtable = JsonConvert.DeserializeObject<Hashtable>(split[2]);
                        OverhaulEventsController.DispatchEvent(InfoReceivedEventString, Hashtable);
                        stopwatch.StopTimer("OverhaulPlayerInfo.OnEvent, Answer (OLD)");
                    }
                }
            }
        }

        public static Hashtable CreateNewHashtable(bool generateDefaultData = false, string replacedPlayfabID = "")
        {
            return new Hashtable
            {
                ["ID"] = generateDefaultData ? replacedPlayfabID : OverhaulPlayerIdentifier.GetLocalPlayFabID(),
                ["Skin.Sword"] = generateDefaultData ? "Default" : WeaponSkinsController.EquippedSwordSkin,
                ["Skin.Bow"] = generateDefaultData ? "Default" : WeaponSkinsController.EquippedBowSkin,
                ["Skin.Hammer"] = generateDefaultData ? "Default" : WeaponSkinsController.EquippedHammerSkin,
                ["Skin.Spear"] = generateDefaultData ? "Default" : WeaponSkinsController.EquippedSpearSkin,
                ["Outfits.Equipped"] = OutfitsController.EquippedAccessories,
                ["State.Status"] = PlayerStatusBehaviour.GetOwnStatus(),
                ["State.Flags"] = OverhaulPlayerInfoController.GetUserFlags(),
                ["State.Version"] = OverhaulVersion.ModVersion.ToString(),
                ["Custom.Data"] = string.Empty,
            };
        }

        [Obsolete("Version 1")]
        public static string SerializeData(bool generateDefaultData = false, string replacedPlayfabID = "")
        {
            Hashtable newHashTable = CreateNewHashtable(generateDefaultData, replacedPlayfabID);
            string serializedData = JsonConvert.SerializeObject(newHashTable);
            return serializedData;
        }

        public static OverhaulPlayerInfo GetOverhaulPlayerInfo(FirstPersonMover mover)
        {
            if (!GameModeManager.IsMultiplayer() || !MultiplayerPlayerInfoManager.Instance || !mover)
                return null;

            string playfabID = mover.GetPlayFabID();
            if (string.IsNullOrEmpty(playfabID))
                return null;

            MultiplayerPlayerInfoState state = MultiplayerPlayerInfoManager.Instance.GetPlayerInfoState(playfabID);
            return state == null ? null : state.GetComponent<OverhaulPlayerInfo>();
        }
    }
}