using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CDOverhaul.HUD.Tooltips
{
    public class OverhaulTooltipsController : OverhaulGameplayController
    {
        private List<OverhaulTooltip> m_Tooltips;

        public OverhaulTooltipsUI TooltipsUI;

        private GameObject m_TooltipsContainerGameObject;

        public override void Initialize()
        {
            base.Initialize();

            m_Tooltips = new List<OverhaulTooltip>();

            OverhaulCanvasController canvasController = GetController<OverhaulCanvasController>();
            if (canvasController == null)
                return;

            TooltipsUI = canvasController.AddHUD<OverhaulTooltipsUI>(canvasController.HUDModdedObject.GetObject<ModdedObject>(18));
            TooltipsUI.TooltipsController = this;

            m_TooltipsContainerGameObject = new GameObject("TooltipScripts");
            m_TooltipsContainerGameObject.transform.SetParent(base.transform.parent);

            AddTooltip<OverhaulCurrentWeaponTooltip>();
        }

        public override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover, bool hasInitializedModel)
        {
            if (!hasInitializedModel)
                return;
        }

        public void AddTooltip<T>() where T : OverhaulTooltip
        {
            T component = m_TooltipsContainerGameObject.GetComponent<T>();
            if (component)
                throw new Exception("A tooltip with the same type is already added! Type: " + typeof(T).ToString());

            component = m_TooltipsContainerGameObject.AddComponent<T>();
            component.TooltipsController = this;
            component.Initialize();

            m_Tooltips.Add(component);
            TooltipsUI.HaveToPopulateTooltips = true;
        }

        public List<OverhaulTooltip> GetTooltips() => m_Tooltips;

        public override string[] Commands() => null;
        public override string OnCommandRan(string[] command) => null;
    }
}
