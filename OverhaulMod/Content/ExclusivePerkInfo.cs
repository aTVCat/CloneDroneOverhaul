using OverhaulMod.Utils;
using Steamworks;
using System;

namespace OverhaulMod.Content
{
    public class ExclusivePerkInfo
    {
        public ExclusivePerkType PerkType;

        public string DisplayName;
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

            switch (PerkType)
            {
                case ExclusivePerkType.Color:
                    m_deserializedData = ModJsonUtils.Deserialize<ExclusivePerkColor>(Data);
                    break;
            }
            m_hasDeserializedData = true;

            return m_deserializedData;
        }

        public void SetData(object data)
        {
            m_deserializedData = data;
            m_hasDeserializedData = true;
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
            bool hasSteamId = HasSteamID();
            bool hasPlayFabId = HasPlayFabID();
            return (!hasSteamId && !hasPlayFabId) || (hasPlayFabId && PlayFabID == ModUserInfo.localPlayerPlayFabID) || (hasSteamId && SteamID == (ulong)ModUserInfo.localPlayerSteamID);
        }
    }
}
