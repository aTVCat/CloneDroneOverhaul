using UnityEngine;

namespace CDOverhaul
{
    internal class ExclusiveColorDefinition
    {
        public string RelatedPlayFabId;

        public int ColorIndexToReplace;
        public Color NewColor;

        public ExclusiveColorDefinition(string playFabId, int replace, Color newColor)
        {
            RelatedPlayFabId = playFabId;
            ColorIndexToReplace = replace;
            NewColor = newColor;
        }

        public ExclusiveColorDefinition(string playFabId, int replace, string hex, float glowPercent)
        {
            RelatedPlayFabId = playFabId;
            ColorIndexToReplace = replace;

            Color color = hex.ToColor();
            color.a = 1f - glowPercent;
            NewColor = color;
        }

        public bool TryApplyColorOnRobot(FirstPersonMover firstPersonMover, Color currentColor, out Color color)
        {
            color = currentColor;
            if (!firstPersonMover || !firstPersonMover.IsPlayer())
                return false;
            string playFabID = GameModeManager.IsSinglePlayer() ? OverhaulPlayerIdentifier.GetLocalPlayFabID() : firstPersonMover.GetPlayFabID();

            if (string.IsNullOrEmpty(playFabID) || !RelatedPlayFabId.Contains(playFabID))
                return false;

            int index = 0;
            foreach (HumanFavouriteColor favColor in HumanFactsManager.Instance.FavouriteColors)
            {
                if (favColor == null)
                    continue;

                if (favColor.ColorValue.Equals(currentColor) && index == ColorIndexToReplace)
                {
                    color = NewColor;
                    return true;
                }
                index++;
            }
            return false;
        }
    }
}
