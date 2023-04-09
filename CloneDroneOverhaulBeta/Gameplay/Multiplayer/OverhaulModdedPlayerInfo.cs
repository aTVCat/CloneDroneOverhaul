using Bolt;
using CDOverhaul.Gameplay.Outfits;
using Newtonsoft.Json;
using System.Collections;
using UnityEngine;

namespace CDOverhaul.Gameplay.Multiplayer
{
    public class OverhaulModdedPlayerInfo : GlobalEventListener
    {
        public const string InfoReceivedEventString = "OnOverhaulPlayerInfoReceived";
        private static OverhaulModdedPlayerInfo m_LocalPlayerInfo;

        private MultiplayerPlayerInfoState m_State;

        public bool HasReceivedData { get; private set; }
        private float m_TimeToReceiveData;
        private bool m_HasReceivedRequestAnswer;
        private float m_TimeToStopWaiting;

        private Hashtable m_ReceivedHashtable;
        public string GetData(string id)
        {
            return m_ReceivedHashtable == null ? string.Empty : m_ReceivedHashtable[id].ToString();
        }
        public Hashtable GetHashtable()
        {
            return m_ReceivedHashtable;
        }

        private void Start()
        {
            m_State = base.GetComponent<MultiplayerPlayerInfoState>();
            if (m_State == null)
            {
                m_TimeToReceiveData = -1;
                return;
            }

            m_TimeToReceiveData = Time.time + 1f;
            if (m_State.state.PlayFabID.Equals(ExclusivityController.GetLocalPlayfabID())) m_LocalPlayerInfo = this;
        }

        private void Update()
        {
            float time = Time.time;
            if (m_TimeToReceiveData != -1 && time > m_TimeToReceiveData)
            {
                m_TimeToReceiveData = -1;
                m_TimeToStopWaiting = time + 5f;
                _ = StartCoroutine(getDataCoroutine());
            }
        }

        public void RefreshData()
        {
            GenericStringForModdingEvent newEvent = GenericStringForModdingEvent.Create(Bolt.GlobalTargets.AllClients);
            newEvent.EventData = "[OverhaulPlayerInfoRefresh]@" + ExclusivityController.GetLocalPlayfabID() + "@" + SerializeData();
            newEvent.Send();
        }

        private IEnumerator getDataCoroutine()
        {
            // Request data from specific player first
            GenericStringForModdingEvent newEvent = GenericStringForModdingEvent.Create(Bolt.GlobalTargets.AllClients);
            newEvent.EventData = "[OverhaulPlayerInfoRequest]@" + m_State.state.PlayFabID;
            newEvent.Send();
            yield return new WaitUntil(() => m_HasReceivedRequestAnswer || Time.time > m_TimeToStopWaiting);
            HasReceivedData = true;
            yield break;
        }

        public override void OnEvent(GenericStringForModdingEvent moddedEvent)
        {
            if (moddedEvent == null || string.IsNullOrEmpty(moddedEvent.EventData))
            {
                return;
            }

            if (moddedEvent.EventData.StartsWith("[OverhaulPlayerInfoRequest]@"))
            {
                if (!moddedEvent.EventData.Contains(ExclusivityController.GetLocalPlayfabID()))
                {
                    return;
                }

                GenericStringForModdingEvent newEvent = GenericStringForModdingEvent.Create(Bolt.GlobalTargets.AllClients);
                newEvent.EventData = "[OverhaulPlayerInfoAnswer]@" + ExclusivityController.GetLocalPlayfabID() + "@" + SerializeData();
                newEvent.Send();
            }
            bool isRefresh = moddedEvent.EventData.StartsWith("[OverhaulPlayerInfoRefresh]@");
            if (moddedEvent.EventData.StartsWith("[OverhaulPlayerInfoAnswer]@") || isRefresh)
            {
                string[] split = moddedEvent.EventData.Split('@');
                if (split[1].Equals(m_State.state.PlayFabID))
                {
                    m_ReceivedHashtable = JsonConvert.DeserializeObject<Hashtable>(split[2]);
                    m_HasReceivedRequestAnswer = true;
                    OverhaulEventManager.DispatchEvent(InfoReceivedEventString, m_ReceivedHashtable);
                }
            }
        }

        public static string SerializeData()
        {
            Hashtable newHashTable = GenerateNewHashtable();
            string serializedData = JsonConvert.SerializeObject(newHashTable);
            return serializedData;
        }

        public static Hashtable GenerateNewHashtable()
        {
            return new Hashtable
            {
                ["ID"] = ExclusivityController.GetLocalPlayfabID(),
                ["Skin.Sword"] = WeaponSkinsController.EquippedSwordSkin,
                ["Skin.Bow"] = WeaponSkinsController.EquippedBowSkin,
                ["Skin.Hammer"] = WeaponSkinsController.EquippedHammerSkin,
                ["Skin.Spear"] = WeaponSkinsController.EquippedSpearSkin,
                ["State.Status"] = PlayerStatusBehaviour.GetOwnStatus(),
                ["State.Version"] = OverhaulVersion.ModVersion.ToString(),
                [OutfitsWearer.IDInHashtable] = OutfitsController.EquippedAccessories,
                ["Custom.Data"] = string.Empty
            };
        }

        public static OverhaulModdedPlayerInfo GetLocalPlayerInfo()
        {
            return m_LocalPlayerInfo;
        }

        public static OverhaulModdedPlayerInfo GetPlayerInfo(FirstPersonMover mover)
        {
            if (!GameModeManager.IsMultiplayer() || MultiplayerPlayerInfoManager.Instance == null || mover == null)
            {
                return null;
            }

            string playfabID = mover.GetPlayFabID();
            if (string.IsNullOrEmpty(playfabID))
            {
                return null;
            }

            MultiplayerPlayerInfoState state = MultiplayerPlayerInfoManager.Instance.GetPlayerInfoState(playfabID);
            return state == null ? null : state.GetComponent<OverhaulModdedPlayerInfo>();
        }
    }
}