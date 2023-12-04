using Bolt;
using CDOverhaul.Gameplay.Outfits;
using CDOverhaul.Gameplay.Pets;
using System.Collections;
using System.Collections.Generic;

namespace CDOverhaul.Gameplay.Multiplayer
{
    public class OverhaulPlayerInfo : GlobalEventListener
    {
        public const string GOT_INFO_EVENT = "PlayerInfoGet";
        public const string UPDATE_INFO_EVENT = "PlayerInfoUpdate";

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

        public void Initialize(MultiplayerPlayerInfoState infoState)
        {
            AllOverhaulPlayerInfos.Add(this);
            m_PlayerInfoState = infoState;

            if (infoState.state.PlayFabID == OverhaulPlayerIdentifier.GetLocalPlayFabID())
            {
                LocalOverhaulPlayerInfo = this;
                LocalPlayerInfoState = infoState;
            }
            RequestData();
        }

        private void OnDestroy()
        {
            _ = AllOverhaulPlayerInfos.Remove(this);
        }

        public void CreateAndSendEvent(OverhaulPlayerInfoRefreshEventData data)
        {
            data.SenderPlayFabID = OverhaulPlayerIdentifier.GetLocalPlayFabID();

            GenericStringForModdingEvent newEvent = GenericStringForModdingEvent.Create(GlobalTargets.AllClients, ReliabilityModes.ReliableOrdered);
            newEvent.EventData = OverhaulPlayerInfosSystem.EVENT_PREFIX + OverhaulPlayerInfosSystem.VERSION;
            newEvent.BinaryData = data.SerializeObject();
            newEvent.Send();
        }

        public void RequestData()
        {
            OverhaulPlayerInfoRefreshEventData eventData = new OverhaulPlayerInfoRefreshEventData
            {
                ReceiverPlayFabID = m_PlayerInfoState.state.PlayFabID,
                IsRequest = true
            };
            CreateAndSendEvent(eventData);
        }

        public void UpdateData()
        {
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
                    Hashtable = eventData.Hashtable;
                    OverhaulEvents.DispatchEvent(GOT_INFO_EVENT, Hashtable);
                    OverhaulEvents.DispatchEvent(UPDATE_INFO_EVENT, m_PlayerInfoState.state.PlayFabID);
                    return;
                }
            }
        }

        public static Hashtable CreateNewHashtable()
        {
            return new Hashtable
            {
                ["ID"] = OverhaulPlayerIdentifier.GetLocalPlayFabID(),
                ["Skin.Sword"] = WeaponSkinsController.EquippedSwordSkin,
                ["Skin.Bow"] = WeaponSkinsController.EquippedBowSkin,
                ["Skin.Hammer"] = WeaponSkinsController.EquippedHammerSkin,
                ["Skin.Spear"] = WeaponSkinsController.EquippedSpearSkin,
                ["Skin.Shield"] = string.Empty,
                ["Skin.Arrow"] = string.Empty,
                ["Outfits.Equipped"] = OutfitsSystem.EquippedAccessories,
                ["Pets.Equipped"] = PetSystem.EquippedPets,
                ["State.Status"] = PlayerStatusBehaviour.GetOwnStatus(),
                ["State.Flags"] = OverhaulPlayerInfosSystem.GetUserFlags(),
                ["State.Version"] = OverhaulVersion.modVersion.ToString(),
                ["Custom.Data"] = string.Empty,
            };
        }

        public static OverhaulPlayerInfo GetOverhaulPlayerInfo(FirstPersonMover mover)
        {
            return GameModeManager.IsSinglePlayer() || !mover
                ? null
                : (MultiplayerPlayerInfoManager.Instance.GetPlayerInfoState(mover.GetPlayFabID())?.GetComponent<OverhaulPlayerInfo>());
        }
    }
}