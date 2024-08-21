using OverhaulMod.UI;
using OverhaulMod.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.Engine
{
    public class UseKeyTriggerManager : Singleton<UseKeyTriggerManager>
    {
        [ModSetting(ModSettingsConstants.ENABLE_PRESS_BUTTON_TRIGGER_DESCRIPTION_REWORK, true)]
        public static bool EnablePressButtonTriggerDescriptionRework;

        public static readonly Color BGGlowColor = new Color(0.65f, 0.75f, 1f, 0.3f);

        public static readonly Color FramingBoxSelectedColor = new Color(0f, 0.42f, 0.72f, 0.63f);

        public static readonly Color FramingBoxDeselectedColor = new Color(0.4f, 0.4f, 0.4f, 0.3f);

        private List<LevelEditorUseButtonTrigger> m_triggers;

        private UIPressActionKeyDescription m_pressActionKeyDescription;

        private LevelEditorUseButtonTrigger m_prevNearestTrigger;

        private Coroutine m_coroutine;

        private float m_timeToHideText;

        public override void Awake()
        {
            base.Awake();

            m_triggers = new List<LevelEditorUseButtonTrigger>();
            m_timeToHideText = -1f;
        }

        private void Update()
        {
            if (m_timeToHideText != -1f && Time.unscaledTime >= m_timeToHideText)
            {
                m_timeToHideText = -1f;
                HideDescription();
            }

            if (!EnablePressButtonTriggerDescriptionRework || !ModTime.hasFixedUpdated)
                return;

            CharacterTracker characterTracker = CharacterTracker.Instance;

            float dist = float.MaxValue;
            LevelEditorUseButtonTrigger nearestTrigger = null;
            foreach (LevelEditorUseButtonTrigger trigger in m_triggers)
            {
                if (!trigger || !trigger.CanBeActivated())
                    continue;

                float newDist = characterTracker.GetDistanceToPlayer(trigger.transform.position);
                if (newDist < dist)
                {
                    dist = newDist;
                    nearestTrigger = trigger;
                }
            }

            if (m_prevNearestTrigger != nearestTrigger)
            {
                if(m_coroutine != null)
                {
                    StopCoroutine(m_coroutine);
                    m_coroutine = null;
                }

                if(nearestTrigger == null)
                {
                    HideDescription();
                    m_prevNearestTrigger = null;
                }
                else
                {
                    m_coroutine = StartCoroutine(processTriggerCoroutine(nearestTrigger, m_prevNearestTrigger));
                    m_prevNearestTrigger = nearestTrigger;
                }
            }
        }

        private IEnumerator processTriggerCoroutine(LevelEditorUseButtonTrigger t, LevelEditorUseButtonTrigger prevNearestTrigger)
        {
            float timeout = Time.unscaledTime + 2f;
            while (Time.unscaledTime < timeout && t && !t._keyboardHint)
                yield return null;

            if (!EnablePressButtonTriggerDescriptionRework || Time.unscaledTime > timeout && !t || !t._keyboardHint)
                yield break;

            SetFramingBoxSelectedColor(t._keyboardHint.transform, true);
            if (prevNearestTrigger && prevNearestTrigger._keyboardHint)
                SetFramingBoxSelectedColor(prevNearestTrigger._keyboardHint.transform, false);

            string description = t.Description;
            if (description.IsNullOrEmpty() || description.IsNullOrWhiteSpace())
            {
                HideDescription();
                yield break;
            }

            LocalizationManager localizationManager = LocalizationManager.Instance;
            if (localizationManager && localizationManager.HasTranslatedString(description))
                description = localizationManager.GetTranslatedString(description);

            ShowDescription(description);
            yield break;
        }

        public void SetTriggerRegistered(LevelEditorUseButtonTrigger trigger, bool value)
        {
            if (trigger == null)
                return;

            if (value && trigger.KeyboardHintPrefab)
                SetFramingBoxSelectedColor(trigger.KeyboardHintPrefab.transform, !EnablePressButtonTriggerDescriptionRework);

            if (value && !m_triggers.Contains(trigger))
                m_triggers.Add(trigger);
            else if (!value)
                _ = m_triggers.Remove(trigger);
        }

        public void SetNearestTriggerNull()
        {
            m_prevNearestTrigger = null;
        }

        public void ShowThenHideDescription(string description, float time)
        {
            m_timeToHideText = Time.unscaledTime + time;
            ShowDescription(description);
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

        public static void PatchKeyboardHint(Transform transform)
        {
            RectTransform bgGlow = TransformUtils.FindChildRecursive(transform, "BGGlow") as RectTransform;
            if (bgGlow)
            {
                bgGlow.localPosition = Vector3.zero;
                bgGlow.sizeDelta = Vector3.one * 250f;

                Image image = bgGlow.GetComponent<Image>();
                if (image)
                {
                    image.sprite = ModResources.Sprite(AssetBundleConstants.UI, "Glow-2-256x256");
                    image.color = UseKeyTriggerManager.BGGlowColor;
                }
            }
        }

        public static void SetFramingBoxSelectedColor(Transform keyboardHintTransform, bool value)
        {
            if (!keyboardHintTransform)
                return;

            Transform framingBox = TransformUtils.FindChildRecursive(keyboardHintTransform, "FramingBox");
            if (framingBox)
            {
                Image image = framingBox.GetComponent<Image>();
                if (image)
                {
                    image.color = value ? FramingBoxSelectedColor : FramingBoxDeselectedColor;
                }
            }
        }
    }
}
