using UnityEngine;

namespace CDOverhaul.HUD.Tooltips
{
    public abstract class OverhaulTooltip : OverhaulBehaviour
    {
        protected internal OverhaulTooltipsController TooltipsController;
        protected internal ModdedObject SpawnedTooltipModdedObject;

        public abstract void Initialize();
        public abstract void OnSpawnedTooltip();

        public abstract GameObject GetTooltipPrefab();

        protected void ShowTooltipsContainer()
        {
            if (!TooltipsController || !TooltipsController.TooltipsUI)
                return;

            TooltipsController.TooltipsUI.Show();
        }
    }
}
