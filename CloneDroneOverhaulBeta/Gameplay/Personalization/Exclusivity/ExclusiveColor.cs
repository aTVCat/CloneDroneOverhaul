using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CDOverhaul
{
    internal class ExclusiveColor
    {
        public string RelatedPlayFabId;

        public int ColorIndexToReplace;
        public Color NewColor;

        public ExclusiveColor(string playFabId, int replace, Color newColor)
        {
            RelatedPlayFabId = playFabId;
            ColorIndexToReplace = replace;
            NewColor = newColor;
        }

        public ExclusiveColor(string playFabId, int replace, string hex, float glowPercent)
        {
            RelatedPlayFabId = playFabId;
            ColorIndexToReplace = replace;

            Color color = hex.ConvertToColor();
            color.a = 1f - glowPercent;
            NewColor = color;
        }

        public void TryApplyColorOnRobot(FirstPersonMover firstPersonMover, Color currentColor, out Color color, out bool applied)
        {
            applied = false;
            color = currentColor;
            if (!firstPersonMover)
                return;

            string playFabID = string.Empty;
            if (GameModeManager.IsSinglePlayer())
                playFabID = PlayFabDataController.GetLocalPlayFabID();
            else if (firstPersonMover.IsMainPlayer())
                playFabID = firstPersonMover.GetPlayFabID();

            if (!RelatedPlayFabId.Contains(playFabID))
                return;

            int index = 0;
            foreach (HumanFavouriteColor favColor in HumanFactsManager.Instance.FavouriteColors)
            {
                if (favColor == null)
                    continue;

                if (favColor.ColorValue.Equals(currentColor) && index == ColorIndexToReplace)
                {
                    applied = true;
                    color = NewColor;
                    break;
                }
                index++;
            }
        }
    }
}
