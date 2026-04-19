using OverhaulMod.Utils;
using System.Collections.Generic;
using System.Text;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationMultiplayerManager : Singleton<PersonalizationMultiplayerManager>
    {
        public const string PLAYER_INFO_UPDATED_EVENT = "MultiplayerPlayerCustomizationInfoUpdated";

        public static readonly char Separator = '|';

        private static readonly string s_dataVersion = "0";

        private string _prefixTrue, _prefixFalse;

        private StringBuilder _stringBuilder;

        private Dictionary<string, PersonalizationMultiplayerPlayerInfo> _playerInfos;

        public override void Awake()
        {
            base.Awake();
            _stringBuilder = new StringBuilder();
            _prefixTrue = getPrefix(true, false);
            _prefixFalse = getPrefix(false, false);
            _playerInfos = new Dictionary<string, PersonalizationMultiplayerPlayerInfo>();
        }

        public void SendPlayerCustomizationDataEvent(bool sendForRecentlyConnectedPlayer)
        {
            string swordSkin = normalizeId(PersonalizationUserInfo.SwordSkin);
            string bowSkin = normalizeId(PersonalizationUserInfo.BowSkin);
            string hammerSkin = normalizeId(PersonalizationUserInfo.HammerSkin);
            string spearSkin = normalizeId(PersonalizationUserInfo.SpearSkin);
            string shieldSkin = normalizeId(PersonalizationUserInfo.ShieldSkin);
            string scytheSkin = normalizeId(PersonalizationUserInfo.ScytheSkin);
            string accessories = normalizeId(PersonalizationUserInfo.Accessories);
            string pets = normalizeId(PersonalizationUserInfo.Pets);

            StringBuilder stringBuilder = _stringBuilder;
            _ = stringBuilder.Clear();
            _ = stringBuilder.Append(getPrefix(sendForRecentlyConnectedPlayer));
            appendValue(stringBuilder, ModUserInfo.localPlayerPlayFabID, false);
            appendValue(stringBuilder, ModUserInfo.localPlayerSteamID.ToString(), false);
            appendValue(stringBuilder, s_dataVersion, false);
            appendValue(stringBuilder, swordSkin, false);
            appendValue(stringBuilder, bowSkin, false);
            appendValue(stringBuilder, hammerSkin, false);
            appendValue(stringBuilder, spearSkin, false);
            appendValue(stringBuilder, shieldSkin, false);
            appendValue(stringBuilder, scytheSkin, false);
            appendValue(stringBuilder, accessories, false);
            appendValue(stringBuilder, pets, true);

            _ = GenericStringForModdingEvent.Post(sendForRecentlyConnectedPlayer ? Bolt.GlobalTargets.Others : Bolt.GlobalTargets.AllClients, Bolt.ReliabilityModes.ReliableOrdered, stringBuilder.ToString());
        }

        public void OnEvent(GenericStringForModdingEvent evnt)
        {
            string eventData = evnt.EventData;
            if (eventData.StartsWith(_prefixFalse))
            {
                SendPlayerCustomizationDataEvent(true);
                registerPlayerInfo(eventData.Substring(_prefixFalse.Length));
            }
            else if (eventData.StartsWith(_prefixTrue))
            {
                registerPlayerInfo(eventData.Substring(_prefixTrue.Length));
            }
        }

        public PersonalizationMultiplayerPlayerInfo GetPlayInfo(string playFaId)
        {
            if (playFaId == null)
                return null;

            if (!_playerInfos.TryGetValue(playFaId, out PersonalizationMultiplayerPlayerInfo playerInfo))
                return null;

            return playerInfo;
        }

        private void registerPlayerInfo(string rawData)
        {
            ModDebug.Log(rawData);
            if (rawData.Length > 16)
            {
                string playFabId = rawData.Remove(16);
                ModDebug.Log(playFabId);

                if (_playerInfos.ContainsKey(playFabId))
                {
                    _playerInfos[playFabId].SetData(rawData);
                }
                else
                {
                    _playerInfos.Add(playFabId, new PersonalizationMultiplayerPlayerInfo(rawData));
                }
                GlobalEventManager.Instance.Dispatch(PLAYER_INFO_UPDATED_EVENT, playFabId);
            }
        }

        private void appendValue(StringBuilder stringBuilder, string value, bool isLast)
        {
            _ = stringBuilder.Append(value);
            if (!isLast) _ = stringBuilder.Append(Separator);
        }

        private string getPrefix(bool value, bool useCache = true)
        {
            if (useCache)
            {
                return value ? _prefixTrue : _prefixFalse;
            }
            return $"[OverhaulV4_{value.ToString().ToLower()}] ";
        }

        private string normalizeId(string id)
        {
            if (id.IsNullOrEmpty()) id = "_";
            return id;
        }

        public static bool CompareDataVersion(string dataVersion) => dataVersion == s_dataVersion;
    }
}