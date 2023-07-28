namespace CDOverhaul.Gameplay.Combat
{
    public class OverhaulAITunner : OverhaulAdvancedCharacterExpansion
    {
        private bool m_HasTunnedValues;

        public override void OnPreAIUpdate(AISwordsmanController aiController, out bool continueExecution)
        {
            continueExecution = true;
            if (!m_HasTunnedValues && !GameModeManager.IsMultiplayer())
            {
                //aiController.UsePredictiveAiming = true;
                aiController.AimHeightRange.Min = 0.45f;
                aiController.AimHeightRange.Max = 0.55f;

                m_HasTunnedValues = true;
            }
        }
    }
}
