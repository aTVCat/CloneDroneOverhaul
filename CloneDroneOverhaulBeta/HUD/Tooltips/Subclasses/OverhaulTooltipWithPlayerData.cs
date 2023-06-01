using CDOverhaul.Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD.Tooltips
{
    public class OverhaulTooltipWithPlayerData : OverhaulTooltip
    {
        protected FirstPersonMover Player;

        public override GameObject GetTooltipPrefab() => null;
        public override void Initialize() { }
        protected override void OnDisposed() => OverhaulDisposable.AssignNullToAllVars(this);
        public override void OnSpawnedTooltip() { }

        protected void RefreshPlayer(Action actionToInvokeIfNewPlayer = null)
        {
            if (!Player || !Player.IsAlive())
                if (CharacterTracker.Instance != null && Time.frameCount % 30 == 0)
                {
                    FirstPersonMover newPlayer = CharacterTracker.Instance.GetPlayerRobot();
                    if(newPlayer != Player)
                    {
                        Player = newPlayer;
                        actionToInvokeIfNewPlayer?.Invoke();
                    }
                }
        }
    }
}
