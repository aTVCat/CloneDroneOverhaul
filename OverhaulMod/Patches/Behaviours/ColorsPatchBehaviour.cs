using OverhaulMod.Engine;
using OverhaulMod.Utils;
using UnityEngine;

namespace OverhaulMod.Patches.Behaviours
{
    internal class ColorsPatchBehaviour : GamePatchBehaviour
    {
        [ModSetting(ModSettingsConstants.CHANGE_HIT_COLORS, true)]
        public static bool ChangeColors;

        private static readonly Color _overhaulHitColor = new Color(4f, 0.65f, 0.35f, 0.2f);

        private static readonly Color _overhaulBodyOnFireColor = new Color(1f, 0.42f, 0.22f, 0.1f);

        private Color _originalHitColor, _originalBodyOnFireColor;

        public override void Patch()
        {
            AttackManager attackManager = AttackManager.Instance;
            _originalHitColor = attackManager.HitColor;
            _originalBodyOnFireColor = attackManager.BodyOnFireColor;

            GlobalEventManager.Instance.AddEventListener(ModSettingsManager.SETTING_CHANGED_EVENT, Refresh);

            Refresh();
        }

        public override void Unpatch()
        {
            GlobalEventManager.Instance.RemoveEventListener(ModSettingsManager.SETTING_CHANGED_EVENT, Refresh);
            Refresh(false);
        }

        public void Refresh()
        {
            Refresh(null);
        }

        public void Refresh(bool? forceValue = null)
        {
            AttackManager attackManager = AttackManager.Instance;
            if (attackManager)
            {
                bool switchColors = forceValue == null ? ChangeColors : forceValue.Value;

                attackManager.HitColor = switchColors ? _overhaulHitColor : _originalHitColor;
                attackManager.BodyOnFireColor = switchColors ? _overhaulBodyOnFireColor : _originalBodyOnFireColor;
            }
        }
    }
}
