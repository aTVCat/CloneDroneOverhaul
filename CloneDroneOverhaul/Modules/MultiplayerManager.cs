namespace CloneDroneOverhaul.Modules
{
    public class MultiplayerManager : ModuleBase
    {
        public override void Start()
        {
            Functions = new string[]
            {
                "onPlayerJoined",
                "Bolt.OnEvent"
            };
        }

        public override void RunFunction(string name, object[] arguments)
        {
            if (name == "onPlayerJoined")
            {
                MultiplayerPlayerInfoState stateLoc = MultiplayerPlayerInfoManager.Instance.GetLocalPlayerInfoState();
                if (stateLoc == null)
                {
                    return;
                }
                MultiplayerPlayerInfoState state = (MultiplayerPlayerInfoState)arguments[0];
                if (state.state.PlayFabID == stateLoc.state.PlayFabID)
                {
                    return;
                }
                CloneDroneOverhaul.UI.Notifications.Notification notif = new UI.Notifications.Notification();
                notif.SetUp(state.state.DisplayName + " Has joined", "", 5, new UnityEngine.Vector2(300, 52), new UnityEngine.Color(0.132743f, 0.1559941f, 0.1792453f, 0.6f), new UI.Notifications.Notification.NotificationButton[] { });
            }
        }

        public override void RunFunction<T>(string name, T obj)
        {
            if (name == Functions[1])
            {
                if (typeof(T) == typeof(MatchInstance))
                {
                    MatchInstance instance = obj as MatchInstance;
                    BaseStaticValues.GetInviteCode = instance.MatchID;
                }
            }
        }
    }
}
