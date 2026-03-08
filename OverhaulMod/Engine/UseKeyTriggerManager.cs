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
        [ModSetting(ModSettingsConstants.ENABLE_PRESS_BUTTON_TRIGGER_DESCRIPTION_REWORK, false)]
        public static bool EnablePressButtonTriggerDescriptionRework;

        public static readonly Color BGGlowColor = new Color(0.65f, 0.75f, 1f, 0.3f);

        public static readonly Color FramingBoxSelectedColor = new Color(0f, 0.42f, 0.72f, 0.63f);

        public static readonly Color FramingBoxDeselectedColor = new Color(0.4f, 0.4f, 0.4f, 0.3f);

        private List<LevelEditorUseButtonTrigger> _triggers;

        private UIPressActionKeyDescription _pressActionKeyDescription;

        private LevelEditorUseButtonTrigger _prevNearestTrigger;

        private Coroutine _coroutine;

        private float _timeToHideText;

        public override void Awake()
        {
            base.Awake();

            _triggers = new List<LevelEditorUseButtonTrigger>();
            _timeToHideText = -1f;
        }

        private void Update()
        {
            if (_timeToHideText != -1f && Time.unscaledTime >= _timeToHideText)
            {
                _timeToHideText = -1f;
                HideDescription();
            }

            if (!EnablePressButtonTriggerDescriptionRework || !ModTime.Instance.HasFixedUpdatedThisFrame())
                return;

            List<LevelEditorUseButtonTrigger> list = _triggers;
            if (list.IsNullOrEmpty())
                return;

            CharacterTracker characterTracker = CharacterTracker.Instance;

            float dist = float.MaxValue;
            LevelEditorUseButtonTrigger nearestTrigger = null;
            for (int i = 0; i < list.Count; i++)
            {
                LevelEditorUseButtonTrigger trigger = list[i];
                if (!trigger || !trigger.CanBeActivated())
                    continue;

                float newDist = characterTracker.GetDistanceToPlayer(trigger.transform.position);
                if (newDist < dist)
                {
                    dist = newDist;
                    nearestTrigger = trigger;
                }
            }

            if (_prevNearestTrigger != nearestTrigger)
            {
                if (_coroutine != null)
                {
                    StopCoroutine(_coroutine);
                    _coroutine = null;
                }

                if (nearestTrigger == null)
                {
                    HideDescription();
                    _prevNearestTrigger = null;
                }
                else
                {
                    _coroutine = StartCoroutine(processTriggerCoroutine(nearestTrigger, _prevNearestTrigger));
                    _prevNearestTrigger = nearestTrigger;
                }
            }
        }

        private IEnumerator processTriggerCoroutine(LevelEditorUseButtonTrigger t, LevelEditorUseButtonTrigger prevNearestTrigger)
        {
            float timeout = Time.unscaledTime + 2f;
            while (Time.unscaledTime < timeout && t && !t._keyboardHint)
                yield return null;

            if (!EnablePressButtonTriggerDescriptionRework || (Time.unscaledTime > timeout && !t) || !t._keyboardHint)
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

            if (value && !_triggers.Contains(trigger))
                _triggers.Add(trigger);
            else if (!value)
                _ = _triggers.Remove(trigger);
        }

        public void SetNearestTriggerNull()
        {
            _prevNearestTrigger = null;
        }

        public void ShowThenHideDescription(string description, float time)
        {
            _timeToHideText = Time.unscaledTime + time;
            ShowDescription(description);
        }

        public void ShowDescription(string description)
        {
            UIPressActionKeyDescription actionDescription = _pressActionKeyDescription;
            if (!actionDescription)
            {
                actionDescription = ModUIConstants.ShowPressActionKeyDescription();
                _pressActionKeyDescription = actionDescription;
            }

            if (actionDescription)
                actionDescription.ShowText(description);
        }

        public void HideDescription()
        {
            UIPressActionKeyDescription actionDescription = _pressActionKeyDescription;
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
