using Octokit;

namespace CDOverhaul
{
    internal class OverhaulGitHubController : ModController, IConsoleCommandReceiver
    {
        public static readonly string[] AllCommands = new string[]
        {
            ""
        };

        private static bool _hasInitializedStaticThings;

        public static OverhaulGitHubController Controller;
        public static GitHubClient GithubClient;

        public const bool IsScrapped = true;

        public static void InitializeStatic()
        {
            if (IsScrapped)
            {
                return;
            }
            Controller = ModControllerManager.NewController<OverhaulGitHubController>();

            if (_hasInitializedStaticThings)
            {
                return;
            }
            _hasInitializedStaticThings = true;
            GithubClient = new GitHubClient(new ProductHeaderValue("CloneDroneOverhaul"));
        }

        public override void Initialize()
        {
            _ = OverhaulConsoleController.AddListener(this);
        }

        private void OnDestroy()
        {
            _ = OverhaulConsoleController.RemoveListener(this);
        }

        public string[] Commands()
        {
            return AllCommands;
        }

        public string OnCommandRan(string[] command)
        {
            return ((IConsoleCommandReceiver)Controller).OnCommandRan(command);
        }
    }
}
