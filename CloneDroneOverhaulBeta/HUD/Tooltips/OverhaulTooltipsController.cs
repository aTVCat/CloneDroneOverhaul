using CDOverhaul.Gameplay;
using CDOverhaul.Gameplay.Combat;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.HUD.Tooltips
{
    public class OverhaulTooltipsController : OverhaulGameplaySystem
    {
        public List<OverhaulTooltip> Tooltips
        {
            get;
            private set;
        }

        public OverhaulTooltipsUI TooltipsUI;

        public override void Start()
        {
            /*
            OverhaulCanvasManager canvasController = OverhaulCanvasManager.reference;
            if (!canvasController)
                return;

            TooltipsUI = canvasController.AddHUD<OverhaulTooltipsUI>(canvasController.HUDModdedObject.GetObject<ModdedObject>(18));*/

            AddTooltip<OverhaulCurrentWeaponTooltip>(TooltipsUI.MyModdedObject.GetObject<Transform>(1).gameObject);
            AddTooltip<OverhaulClosestPlayerTooltip>(TooltipsUI.MyModdedObject.GetObject<Transform>(2).gameObject);
        }

        public override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover, bool hasInitializedModel)
        {
            if (!hasInitializedModel)
                return;

            _ = firstPersonMover.gameObject.AddComponent<PlayerTooltipsBehaviour>();
        }

        public void AddTooltip<T>(GameObject gameObject) where T : OverhaulTooltip
        {
            T component = gameObject.AddComponent<T>();
            component.Initialize();
            component.gameObject.SetActive(false);
            component.transform.SetParent(TooltipsUI.MyModdedObject.GetObject<Transform>(0));
            OverhaulUIPanelScaler scaler = component.gameObject.AddComponent<OverhaulUIPanelScaler>();
            scaler.StartScale = Vector3.zero;
            scaler.TargetScale = Vector3.one;
            scaler.Multiplier = 15f;

            Tooltips.Add(component);
        }

        public T GetTooltip<T>() where T : OverhaulTooltip
        {
            foreach (OverhaulTooltip tooltip in Tooltips)
                if (tooltip is T)
                    return (T)tooltip;

            return null;
        }
    }
}
