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

        private bool _hasDeserializedData;
        private object _deserializedData;

        public void SerializeData()
        {
            Data = ModJsonUtils.Serialize(_deserializedData);
        }

        public object DeserializeData()
        {
            if (_hasDeserializedData)
                return _deserializedData;

            if (!Data.IsNullOrEmpty())
            {
                switch (PerkType)
                {
                    case ExclusivePerkType.Color:
                        _deserializedData = ModJsonUtils.Deserialize<ExclusivePerkColor>(Data);
                        break;
                    case ExclusivePerkType.Feature:
                        _deserializedData = ModJsonUtils.Deserialize<int>(Data);
                        break;
                    default:
                        _deserializedData = null;
                        break;
                }
            }
            else
            {
                _deserializedData = null;
            }
            _hasDeserializedData = true;

            return _deserializedData;
        }

        public void SetData(object data)
        {
            _deserializedData = data;
            _hasDeserializedData = true;
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
