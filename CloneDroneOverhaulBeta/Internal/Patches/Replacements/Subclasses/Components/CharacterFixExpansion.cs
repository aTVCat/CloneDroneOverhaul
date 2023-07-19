using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul.Patches
{
    public class CharacterFixExpansion : OverhaulCharacterExpansion
    {
        public override void OnPreCommandExecute(FPMoveCommand command)
        {
            if (!Owner.IsMainPlayer())
                return;

            if (OverhaulCore.IsSteamOverlayOpened)
            {
                command.Input.VerticalMovement = 0f;
                command.Input.HorizontalMovement = 0f;
                command.Input.JetpackHeld = false;
                command.Input.AttackKeyDown = false;
                command.Input.AttackKeyHeld = false;
                command.Input.AttackKeyUp = false;
                return;
            }
        }
    }
}
