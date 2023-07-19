using CDOverhaul.Gameplay.Combat;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.HUD.Tooltips
{
    public class OverhaulTooltipsController : OverhaulGameplayController
    {
        public List<OverhaulTooltip> Tooltips
        {
            get;
            private set;
        } = new List<OverhaulTooltip>();

        public OverhaulTooltipsUI TooltipsUI;

        public override void Initialize()
        {
            base.Initialize();

            OverhaulCanvasController canvasController = OverhaulMod.Core.CanvasController;
            if (!canvasController)
                return;

            TooltipsUI = canvasController.AddHUD<OverhaulTooltipsUI>(canvasController.HUDModdedObject.GetObject<ModdedObject>(18));

            AddTooltip<OverhaulCurrentWeaponTooltip>(TooltipsUI.MyModdedObject.GetObject<Transform>(1).gameObject);
            AddTooltip<OverhaulClosestPlayerTooltip>(TooltipsUI.MyModdedObject.GetObject<Transform>(2).gameObject);
        }

        public override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover, bool hasInitializedModel)
        {
            if (!hasInitializedModel)
                return;

            firstPersonMover.gameObject.AddComponent<PlayerTooltipsBehaviour>();
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
