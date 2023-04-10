using UnityEngine;

namespace CDOverhaul.Gameplay.Mindspace
{
    public class MindspaceOverhaulController : OverhaulGameplayController
    {
        public override void Initialize()
        {
            base.Initialize();

            fixSkybox();
        }

        private void fixSkybox()
        {
            SkyBoxManager.Instance.LevelConfigurableSkyboxes[8].SetColor("_Tint", new Color(0.6f, 0.73f, 2f, 1f));
        }
    }
}