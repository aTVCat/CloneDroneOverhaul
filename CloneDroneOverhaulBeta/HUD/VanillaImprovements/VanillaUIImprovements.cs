using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CDOverhaul.HUD.Vanilla
{
    public class VanillaUIImprovements : OverhaulUI
    {
        public static VanillaUIImprovements Instance;
        public static bool InstanceIsNull => Instance == null;

        private ModdedObject m_ModdedObject;
        public EnergyUIImprovements EnergyUI;

        public override void Initialize()
        {
            m_ModdedObject = base.GetComponent<ModdedObject>();
            EnergyUI = m_ModdedObject.GetObject<Transform>(0).gameObject.AddComponent<EnergyUIImprovements>();
            Instance = this;
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            Instance = null;
        }
    }
}
