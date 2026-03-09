using Discord;
using OverhaulMod.Utils;

namespace OverhaulMod.Engine
{
    public class RichPresenceDiscord : RichPresenceBase
    {
        /// <summary>
        /// Overhaul mod Discord App ID
        /// </summary>
        public const long APP_ID = 1091373211163308073;
        public const CreateFlags CREATE_FLAG = CreateFlags.NoRequireDiscord;

        private Discord.Discord _client;
        private Activity _activity;
        private ActivityParty _party;
        private ActivitySecrets _secrets;
        private PartySize _partySize;
        private ActivityManager.UpdateActivityHandler _activityHandler;

        public override void Start()
        {
            TryInitializeDiscord();
        }

        public override void OnDestroy()
        {
            DisposeDiscordClient();
        }

        private void OnApplicationQuit()
        {
            DisposeDiscordClient();
        }

        public override void Update()
        {
            base.Update();
            try
            {
                if (_client != null)
                    _client.RunCallbacks();
            }
            catch
            {
            }
        }

        public override void RefreshInformation()
        {
            base.RefreshInformation();
            if (_client != null)
            {
                ActivityManager manager = _client.GetActivityManager();
                if (manager == null)
                    return;

                /*bool isInModdedMultiplayer = ModIntegrationUtils.ModdedMultiplayer.IsInModdedMultiplayer();
                string id = isInModdedMultiplayer ? $"{ModIntegrationUtils.ModdedMultiplayer.GetCurrentGameModeInfoID()}_{ModIntegrationUtils.ModdedMultiplayer.GetLobbyID()}" : null;

                PartySize partySize = _partySize;
                partySize.CurrentSize = isInModdedMultiplayer ? ModIntegrationUtils.ModdedMultiplayer.GetCurrentPlayerCount() : 0;
                partySize.MaxSize = isInModdedMultiplayer ? ModIntegrationUtils.ModdedMultiplayer.GetMaxPlayerCount() : 0;

                ActivitySecrets activitySecrets = _secrets;
                activitySecrets.Join = isInModdedMultiplayer ? $"lobby_{id}" : null;

                ActivityParty party = _party;
                party.Id = isInModdedMultiplayer ? $"cdo_{id}" : null;
                party.Size = partySize;*/

                Activity activity = _activity;
                activity.State = !gameModeDetailsString.IsNullOrEmpty() ? gameModeDetailsString : string.Empty;
                activity.Details = $"v{ModBuild.Version} · {gameModeString}";
                //activity.Party = party;
                //activity.Secrets = activitySecrets;

                manager.UpdateActivity(activity, _activityHandler);
            }
        }

        public void TryInitializeDiscord()
        {
            if (_client == null)
            {
                try
                {
                    Discord.Discord client = new Discord.Discord(APP_ID, (ulong)CREATE_FLAG);
#if DEBUG
                    client.SetLogHook(LogLevel.Debug, (level, message) =>
                    {
                        switch (level)
                        {
                            case LogLevel.Error:
                            case LogLevel.Warn:
                                ModDebug.LogWarning($"Discord RPC: {message}");
                                break;
                            default:
                                ModDebug.Log($"Discord RPC: {message}");
                                break;
                        }
                    });
#endif
                    /*RelationshipManager relationshipManager = client.GetRelationshipManager();
                    ActivityManager activityManager = client.GetActivityManager();
                    activityManager.OnActivityJoin += secret =>
                    {
                        string[] split = secret.Split('_');
                        string gameModeId = split[1];
                        string lobbyCode = split[2];

                    };
                    activityManager.OnActivityJoinRequest += (ref User user) =>
                    {
                        Relationship relationship = relationshipManager.Get(user.Id);
                        ActivityJoinRequestReply reply = ActivityJoinRequestReply.Ignore;

                        switch (relationship.Type)
                        {
                            case RelationshipType.Friend:
                            case RelationshipType.Implicit:
                            case RelationshipType.PendingOutgoing:
                                {
                                    reply = ActivityJoinRequestReply.Yes;
                                    break;
                                }
                        }

                        activityManager.SendRequestReply(user.Id, reply, _ => { });
                    };*/

                    _client = client;

                    Activity activity = new Activity()
                    {
                        Assets =
                        {
                            LargeImage = "defaultimage",
                            LargeText = "Overhaul Mod",
                        },
                    };
                    _activity = activity;
                }
                catch
                {
                    base.enabled = false;
                }
            }

            if (_activityHandler == null)
                _activityHandler = new ActivityManager.UpdateActivityHandler(handleActivityUpdate);
        }

        public void DisposeDiscordClient()
        {
            try
            {
                if (_client != null)
                {
                    _client.Dispose();
                    _client = null;
                }
            }
            catch
            {

            }
        }

        private void handleActivityUpdate(Result res)
        {
            if (res != Result.Ok)
                Destroy(this);
        }
    }
}
