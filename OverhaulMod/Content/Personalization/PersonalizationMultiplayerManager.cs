using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationMultiplayerManager : BoltGlobalEventListenerSingleton<PersonalizationMultiplayerManager>
    {
        public const string PLAYER_INFO_UPDATED_EVENT = "MultiplayerPlayerCustomizationInfoUpdated";

        public static readonly char Separator = '|';

        private static readonly string s_dataVersion = "0";

        private string m_prefixTrue, m_prefixFalse;

        private StringBuilder m_stringBuilder;

        private Dictionary<string, PersonalizationMultiplayerPlayerInfo> m_playerInfos;

        public override void Awake()
        {
            base.Awake();
            m_stringBuilder = new StringBuilder();
            m_prefixTrue = getPrefix(true, false);
            m_prefixFalse = getPrefix(false, false);
            m_playerInfos = new Dictionary<string, PersonalizationMultiplayerPlayerInfo>();
        }

        public void SendPlayerCustomizationDataEvent(bool sendForRecentlyConnectedPlayer, bool debug = false)
        {
            if (!BoltNetwork.IsClient && !debug)
                return;

            string swordSkin = normalizeId(PersonalizationController.SwordSkin);
            string bowSkin = normalizeId(PersonalizationController.BowSkin);
            string hammerSkin = normalizeId(PersonalizationController.HammerSkin);
            string spearSkin = normalizeId(PersonalizationController.SpearSkin);
            string shieldSkin = normalizeId(PersonalizationController.ShieldSkin);

            StringBuilder stringBuilder = m_stringBuilder;
            stringBuilder.Clear();
            stringBuilder.Append(getPrefix(sendForRecentlyConnectedPlayer));
            appendValue(stringBuilder, ModUserInfo.localPlayerPlayFabID, false);
            appendValue(stringBuilder, ModUserInfo.localPlayerSteamID.ToString(), false);
            appendValue(stringBuilder, s_dataVersion, false);
            appendValue(stringBuilder, swordSkin, false);
            appendValue(stringBuilder, bowSkin, false);
            appendValue(stringBuilder, hammerSkin, false);
            appendValue(stringBuilder, spearSkin, false);
            appendValue(stringBuilder, shieldSkin, true);

            if (debug)
            {
                Debug.Log(stringBuilder.ToString());
                return;
            }

            GenericStringForModdingEvent genericStringForModdingEvent = GenericStringForModdingEvent.Create(sendForRecentlyConnectedPlayer ? Bolt.GlobalTargets.Others : Bolt.GlobalTargets.AllClients, Bolt.ReliabilityModes.ReliableOrdered);
            genericStringForModdingEvent.EventData = stringBuilder.ToString();
            genericStringForModdingEvent.Send();
        }

        public override void OnEvent(GenericStringForModdingEvent evnt)
        {
            string eventData = evnt.EventData;
            if (eventData.StartsWith(m_prefixFalse))
            {
                SendPlayerCustomizationDataEvent(true);
                registerPlayerInfo(eventData.Substring(m_prefixFalse.Length));
            }
            else if (eventData.StartsWith(m_prefixTrue))
            {
                registerPlayerInfo(eventData.Substring(m_prefixTrue.Length));
            }
        }

        public PersonalizationMultiplayerPlayerInfo GetPlayInfo(string playFaId)
        {
            if (!m_playerInfos.TryGetValue(playFaId, out PersonalizationMultiplayerPlayerInfo playerInfo))
                return null;

            return playerInfo;
        }

        private void registerPlayerInfo(string rawData)
        {
            if(rawData.Length > 16)
            {
                string playFabId = rawData.Remove(16);
                if (m_playerInfos.ContainsKey(playFabId))
                {
                    m_playerInfos[playFabId].SetData(rawData);
                }
                else
                {
                    m_playerInfos.Add(playFabId, new PersonalizationMultiplayerPlayerInfo(rawData));
                }
                GlobalEventManager.Instance.Dispatch(PLAYER_INFO_UPDATED_EVENT, playFabId);
            }
        }

        private void appendValue(StringBuilder stringBuilder, string value, bool isLast)
        {
            stringBuilder.Append(value);
            if (!isLast)
                stringBuilder.Append(Separator);
        }

        private string getPrefix(bool value, bool useCache = true)
        {
            if (useCache)
            {
                return value ? m_prefixTrue : m_prefixFalse;
            }
            return $"[OverhaulV4_{value}] ";
        }

        private string normalizeId(string id)
        {
            if (id == null)
                id = string.Empty;

            return id;
        }

        public static bool CompareDataVersion(string dataVersion)
        {
            return dataVersion == s_dataVersion;
        }
    }
}
