using Bolt;
using CDOverhaul.Gameplay.Outfits;
using CDOverhaul.Gameplay.Pets;
using System.Collections;
using System.Collections.Generic;

namespace CDOverhaul.Gameplay.Multiplayer
{
    public class OverhaulPlayerInfo : GlobalEventListener
    {
        public const string InfoReceivedEventString = "OnOverhaulPlayerInfoReceived";
        public const string PlayerDataUpdateEventString = "OnPlayerDataUpdate";

        public static readonly List<OverhaulPlayerInfo> AllOverhaulPlayerInfos = new List<OverhaulPlayerInfo>();

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
            AllOverhaulPlayerInfos.Add(this);
            m_PlayerInfoState = base.GetComponent<MultiplayerPlayerInfoState>();
            if (m_PlayerInfoState)
            {
                if (m_PlayerInfoState.state.PlayFabID == OverhaulPlayerIdentifier.GetLocalPlayFabID())
                {
                    LocalOverhaulPlayerInfo = this;
                    LocalPlayerInfoState = m_PlayerInfoState;
                }

                RequestData();
                return;
            }
        }

        private void OnDestroy()
        {
            _ = AllOverhaulPlayerInfos.Remove(this);
        }

        public void CreateAndSendEvent(OverhaulPlayerInfoRefreshEventData data)
        {
            data.SenderPlayFabID = OverhaulPlayerIdentifier.GetLocalPlayFabID();

            GenericStringForModdingEvent newEvent = GenericStringForModdingEvent.Create(GlobalTargets.AllClients, ReliabilityModes.ReliableOrdered);
            newEvent.EventData = OverhaulPlayerInfoController.PlayerInfoEventPrefix + OverhaulPlayerInfoController.PlayerInfoVersion;
            newEvent.BinaryData = data.SerializeObject();
            newEvent.Send();
        }

        public void RequestData()
        {
            UnityEngine.Debug.LogWarning("REQUEST DATA " + OverhaulPlayerIdentifier.GetLocalPlayFabID() + " " + m_PlayerInfoState.state.PlayFabID);

            OverhaulPlayerInfoRefreshEventData eventData = new OverhaulPlayerInfoRefreshEventData
            {
                ReceiverPlayFabID = m_PlayerInfoState.state.PlayFabID,
                IsRequest = true
            };
            CreateAndSendEvent(eventData);
        }

        public void RefreshData()
        {
            UnityEngine.Debug.LogWarning("REFRESH DATA");

            OverhaulPlayerInfoRefreshEventData eventData = new OverhaulPlayerInfoRefreshEventData
            {
                ReceiverPlayFabID = OverhaulPlayerInfoRefreshEventData.RECEIVER_EVERYONE,
                Hashtable = CreateNewHashtable(),
                IsAnswer = true
            };
            CreateAndSendEvent(eventData);
        }

        public void OnGenericStringEvent(OverhaulPlayerInfoRefreshEventData eventData)
        {
            if (eventData.IsRequest)
            {
                UnityEngine.Debug.LogWarning("GET REQUEST DATA " + OverhaulPlayerIdentifier.GetLocalPlayFabID() + " " + eventData.SenderPlayFabID);

                OverhaulPlayerInfoRefreshEventData newEventData = new OverhaulPlayerInfoRefreshEventData
                {
                    ReceiverPlayFabID = eventData.SenderPlayFabID,
                    Hashtable = CreateNewHashtable(),
                    IsAnswer = true
                };
                CreateAndSendEvent(newEventData);
            }
            else if (eventData.IsAnswer)
            {
                if (m_PlayerInfoState.state.PlayFabID == eventData.SenderPlayFabID)
                {
                    UnityEngine.Debug.LogWarning("GET ANSWER DATA " + eventData.SenderPlayFabID + " " + eventData.ReceiverPlayFabID);

                    Hashtable = eventData.Hashtable;
                    OverhaulEventsController.DispatchEvent(InfoReceivedEventString, Hashtable);
                    OverhaulEventsController.DispatchEvent(PlayerDataUpdateEventString, m_PlayerInfoState.state.PlayFabID);
                    return;
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
                ["Pets.Equipped"] = PetsController.EquippedPets,
                ["State.Status"] = PlayerStatusBehaviour.GetOwnStatus(),
                ["State.Flags"] = OverhaulPlayerInfoController.GetUserFlags(),
                ["State.Version"] = OverhaulVersion.ModVersion.ToString(),
                ["Custom.Data"] = string.Empty,
            };
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