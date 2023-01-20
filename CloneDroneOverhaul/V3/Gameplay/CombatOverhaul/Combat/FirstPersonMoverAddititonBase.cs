using System;
using System.Collections.Generic;
using UnityEngine;

namespace CloneDroneOverhaul.V3.Gameplay
{
    public class FirstPersonMoverAddititonBase : MonoBehaviour
    {
        public const string TEMPORAL_PREFIX = "FPMAddition_";

        public FirstPersonMover Owner { get; private set; }
        protected int OwnerInstanceID;
        private string _temporalString;

        public FPMoveCommand Command { get; private set; }

        public void Initialize(in FirstPersonMover mover)
        {
            if(mover == null)
            {
                return;
            }
            Owner = mover;
            OwnerInstanceID = Owner.GetInstanceID();
            _temporalString = TEMPORAL_PREFIX + OwnerInstanceID;
            OverhaulCacheAndGarbageController.AddTemporalObject<FirstPersonMoverAddititonBase>(this, _temporalString);
        }

        public void ReceiveCommand(in FPMoveCommand command)
        {
            Command = command;
            OnReceiveCommand(command);
        }

        protected virtual void OnReceiveCommand(in FPMoveCommand command)
        {

        }

        public virtual void OnUpgradesRefreshed(in UpgradeCollection collection)
        {

        }

        void OnDestroy()
        {
            if (OverhaulCacheAndGarbageController.ContainsTemporalObject(_temporalString))
            {
                OverhaulCacheAndGarbageController.RemoveTemporalObject(_temporalString);
            }
        }
    }
}
