using OverhaulMod.Utils;
using Steamworks;

namespace OverhaulMod.Content
{
    public class ExclusivePerkInfo
    {
        public ExclusivePerkType PerkType;

        public string DisplayName;
        public string Icon;
        public string Data;

        public ulong SteamID;
        public string PlayFabID;

        private bool m_hasDeserializedData;
        private object m_deserializedData;

        public void SerializeData()
        {
            Data = ModJsonUtils.Serialize(m_deserializedData);
        }

        public object DeserializeData()
        {
            if (m_hasDeserializedData)
                return m_deserializedData;

            if (!Data.IsNullOrEmpty())
            {
                switch (PerkType)
                {
                    case ExclusivePerkType.Color:
                        m_deserializedData = ModJsonUtils.Deserialize<ExclusivePerkColor>(Data);
                        break;
                    case ExclusivePerkType.Feature:
                        m_deserializedData = ModJsonUtils.Deserialize<int>(Data);
                        break;
                    default:
                        m_deserializedData = null;
                        break;
                }
            }
            else
            {
                m_deserializedData = null;
            }
            m_hasDeserializedData = true;

            return m_deserializedData;
        }

        public void SetData(object data)
        {
            m_deserializedData = data;
            m_hasDeserializedData = true;
        }

        public void SetDefaultData()
        {

            switch (PerkType)
            {
                case ExclusivePerkType.Color:
                    SetData(new ExclusivePerkColor());
                    break;
                case ExclusivePerkType.Feature:
                    SetData(0);
                    break;
                default:
                    SetData(null);
                    break;
            }
        }

        public bool HasSteamID()
        {
            return SteamID != default;
        }

        public bool HasPlayFabID()
        {
            return !PlayFabID.IsNullOrEmpty();
        }

        public bool IsUnlocked()
        {
            return IsUnlockedForUser(ModUserInfo.localPlayerPlayFabID, ModUserInfo.localPlayerSteamID);
        }

        public bool IsUnlockedForUser(string playFabId, CSteamID steamId)
        {
            if (ModBuildInfo.disableExclusivePerks) return false;

            bool hasSteamId = HasSteamID();
            bool hasPlayFabId = HasPlayFabID();
            return (!hasSteamId && !hasPlayFabId) || (hasPlayFabId && PlayFabID == playFabId) || (hasSteamId && SteamID == (ulong)steamId);
        }
    }
}
