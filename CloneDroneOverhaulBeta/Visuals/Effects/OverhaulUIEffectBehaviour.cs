using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Visuals
{
    public class OverhaulUIEffectBehaviour : OverhaulBehaviour
    {
        protected OverhaulCameraManager CameraManager;

        public override void Start()
        {
            CameraManager = OverhaulCameraManager.reference;
        }
    }
}
