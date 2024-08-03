using OverhaulMod.UI;
using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OverhaulMod.Engine
{
    public class PressActionKeyObjectManager : Singleton<PressActionKeyObjectManager>
    {
        public static readonly Color BGGlowColor = new Color(0.65f, 0.75f, 1f, 0.3f);

        private List<LevelEditorUseButtonTrigger> m_triggers;

        private UIPressActionKeyDescription m_pressActionKeyDescription;

        private LevelEditorUseButtonTrigger m_prevNearestTrigger;

        public override void Awake()
        {
            base.Awake();

            m_triggers = new List<LevelEditorUseButtonTrigger>();
        }

        private void Update()
        {
            if (ModTime.hasFixedUpdated)
            {
                CharacterTracker characterTracker = CharacterTracker.Instance;

                float dist = float.MaxValue;
                LevelEditorUseButtonTrigger nearestTrigger = null;
                foreach (var trigger in m_triggers)
                {
                    if (!trigger || !trigger.CanBeActivated())
                        continue;

                    float newDist = characterTracker.GetDistanceToPlayer(trigger.transform.position);
                    if(newDist < dist)
                    {
                        dist = newDist;
                        nearestTrigger = trigger;
                    }
                }

                if(m_prevNearestTrigger != nearestTrigger)
                {
                    m_prevNearestTrigger = nearestTrigger;
                    if (nearestTrigger != null)
                    {
                        ShowDescription(nearestTrigger.Description);
                    }
                    else
                    {
                        HideDescription();
                    }
                }
            }
        }

        public void SetTriggerRegistered(LevelEditorUseButtonTrigger trigger, bool value)
        {
            if (trigger == null)
                return;

            if (value && !m_triggers.Contains(trigger))
                m_triggers.Add(trigger);
            else if(!value)
                m_triggers.Remove(trigger);
        }

        public void ShowDescription(string description)
        {
            UIPressActionKeyDescription actionDescription = m_pressActionKeyDescription;
            if (!actionDescription)
            {
                actionDescription = ModUIConstants.ShowPressActionKeyDescription();
                m_pressActionKeyDescription = actionDescription;
            }

            if (actionDescription)
                actionDescription.ShowText(description);
        }

        public void HideDescription()
        {
            UIPressActionKeyDescription actionDescription = m_pressActionKeyDescription;
            if (actionDescription)
                actionDescription.HideText();
        }
    }
}
