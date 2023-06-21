﻿using CDOverhaul.HUD;
using System.Collections.Generic;
using System.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay.QualityOfLife
{
    public class LevelEditorFixes : OverhaulController
    {
        public static LevelEditorSelectionSettingsPanel SelectionSettingsPanel { get; private set; }

        public override void Initialize()
        {
            addUIs();
            fixValues();
            fixUI();

            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            DelegateScheduler.Instance.Schedule(LevelEditorSelectionSettingsPanel.RefreshSliders, 1f);
        }

        private void assignAnimationClip(AnimationClip clip, List<string> toAssignTo)
        {
            string text = clip.name.Replace("MechUpper_", "");
            text = text.Replace("MechLegs_", "");
            text = text.Replace("AnalysisBot_", "");
            text = text.Replace("AnalysisBot_", "");
            text = text.Replace("FleetOverseerUpper_", "FO_");

            if (text.EndsWith("_Multiplayer"))
                text = text.Replace("_Multiplayer", "");

            if (text.EndsWith("_Hammer"))
                text = text.Replace("_Hammer", "");

            if (text.EndsWith("_Spear"))
                text = text.Replace("_Spear", "");

            if (!toAssignTo.Contains(text))
                toAssignTo.Add(text);
        }

        private void fixValues()
        {
            if (WeaponManager.Instance != null && CharacterAnimationManager.Instance != null)
            {
                // More animations
                for (int i = 0; i < WeaponManager.Instance.DefaultUpperBodyAnimator.animationClips.Length; i++)
                    assignAnimationClip(WeaponManager.Instance.DefaultUpperBodyAnimator.animationClips[i], CharacterAnimationManager.Instance.UpperAnimationNames);

                CharacterAnimationManager.Instance.UpperAnimationNames.Sort();

                for (int i = 0; i < WeaponManager.Instance.DefaultLegsAnimator.animationClips.Length; i++)
                    assignAnimationClip(WeaponManager.Instance.DefaultLegsAnimator.animationClips[i], CharacterAnimationManager.Instance.LegsAnimationNames);

                CharacterAnimationManager.Instance.LegsAnimationNames.Sort();
            }
        }

        private void fixUI()
        {
            GameUIRoot uiRoot = GameUIRoot.Instance;
            if (uiRoot == null)
                return;

            LevelEditorUI levelEditorUI = uiRoot.LevelEditorUI;
            if (levelEditorUI == null || levelEditorUI.InspectorTransform == null)
                return;

            LevelEditorInspector inspector = levelEditorUI.InspectorTransform.GetComponent<LevelEditorInspector>();
            LevelEditorLibraryUI libraryUI = levelEditorUI.LibraryUI;
            if (inspector == null && libraryUI != null)
                return;

            CustomInspectorPropertyGroup group = inspector.CustomInspectorPropertyGroupPrefab;
            if (group == null)
                return;

            CustomInspectorMethodCalledFromAnimationDropdown calledFromAnimatonDropdown = group.CustomInspectorMethodCalledFromAnimationDropdownPrefab;
            if (calledFromAnimatonDropdown != null && calledFromAnimatonDropdown.DropdownField != null && calledFromAnimatonDropdown.Label != null)
            {
                (calledFromAnimatonDropdown.DropdownField.transform as RectTransform).sizeDelta = new Vector2(45f, (calledFromAnimatonDropdown.DropdownField.transform as RectTransform).sizeDelta.y);
                calledFromAnimatonDropdown.DropdownField.template.GetComponent<ScrollRect>().scrollSensitivity = 20;

                calledFromAnimatonDropdown.Label.resizeTextForBestFit = true;
                RectTransform label = calledFromAnimatonDropdown.Label.rectTransform;
                label.anchoredPosition = Vector2.zero;
                label.pivot = new Vector2(0f, 0.5f);
                label.sizeDelta = new Vector2(75f, 17f);
            }

            CustomInspectorDropdownField field = group.CustomFieldEditorDropdownPrefab;
            if (field != null && field.DropdownField != null && field.Label != null)
            {
                // Dropdown field patch
                field.Label.resizeTextForBestFit = true;
                RectTransform labelTransform = field.Label.transform as RectTransform;
                if (labelTransform != null)
                {
                    labelTransform.pivot = new Vector2(0f, 0.5f);
                    labelTransform.offsetMax = new Vector2(75f, 6f);
                    labelTransform.offsetMin = new Vector2(0f, -6f);
                    labelTransform.sizeDelta = new Vector2(75f, 12f);
                    labelTransform.anchoredPosition = Vector2.zero;
                }

                Transform dropdownTransform = field.DropdownField.transform;
                if (dropdownTransform is RectTransform)
                {
                    RectTransform rectT = dropdownTransform as RectTransform;
                    rectT.sizeDelta = new Vector2(45f, 0f);
                }

                Transform template = TransformUtils.FindChildRecursive(dropdownTransform, "Template");
                if (template != null && template is RectTransform)
                {
                    RectTransform rectT = template as RectTransform;
                    rectT.pivot = new Vector2(1f, 1f);
                    rectT.sizeDelta = new Vector2(80f, 150f);

                    template.GetComponent<ScrollRect>().scrollSensitivity = 25f;

                    RectTransform viewPort = TransformUtils.FindChildRecursive(rectT, "Viewport") as RectTransform;
                    Mask mask = viewPort.GetComponent<Mask>();
                    Destroy(mask);
                    _ = viewPort.gameObject.AddComponent<RectMask2D>(); // reduces lag
                }

                Transform checkMark = TransformUtils.FindChildRecursive(dropdownTransform, "Item Checkmark");
                if (checkMark != null && checkMark is RectTransform)
                {
                    RectTransform rectT = checkMark as RectTransform;
                    rectT.pivot = new Vector2(0.5f, 0.5f);
                    rectT.sizeDelta = new Vector2(3f, 13f);
                    rectT.anchoredPosition = new Vector2(6, 0);

                    Image image = rectT.GetComponent<Image>();
                    if (image != null)
                    {
                        image.sprite = null;
                        image.color = new Color(0f, 0f, 0f, 0.75f);
                    }
                }

                Transform background = TransformUtils.FindChildRecursive(dropdownTransform, "Item Background");
                if (background != null)
                {
                    Image image = background.GetComponent<Image>();
                    if (image != null)
                        image.color = Color.clear;
                }
            }

            CustomInspectorInputField ciInputField = group.CustomFieldEditorStringInputPrefab;
            if (ciInputField != null && ciInputField.InputField != null && ciInputField.Label != null)
            {
                InputField inputFieldo = ciInputField.InputField;
                inputFieldo.lineType = InputField.LineType.MultiLineNewline;
                inputFieldo.textComponent.alignment = TextAnchor.UpperLeft;
                RectTransform inputFieldoTransform = inputFieldo.transform as RectTransform;
                inputFieldoTransform.pivot = new Vector2(1f, 0.5f);
                inputFieldoTransform.anchorMax = new Vector2(1f, 1f);
                inputFieldoTransform.anchorMin = new Vector2(0f, 0f);
                inputFieldoTransform.sizeDelta = new Vector2(-80f, 0f);
                inputFieldoTransform.anchoredPosition = Vector2.zero;
                RectTransform textComponentTransform = inputFieldo.textComponent.rectTransform;
                textComponentTransform.anchorMax = new Vector2(1, 1);
                textComponentTransform.anchorMin = new Vector2(0, 0);
                textComponentTransform.offsetMax = new Vector2(0.5f, 0.5f);
                textComponentTransform.offsetMin = new Vector2(-0.5f, -0.5f);
                textComponentTransform.sizeDelta = new Vector2(-5f, -5f);
                textComponentTransform.anchoredPosition = Vector2.zero;

                (inputFieldo.placeholder as Text).color = new Color(1f, 1f, 1f, 0.5f);
                (inputFieldo.placeholder as Text).fontSize = 10;

                RectTransform inputFieldLabel = ciInputField.Label.rectTransform;
                inputFieldLabel.pivot = new Vector2(0f, 0.5f);
                inputFieldLabel.sizeDelta = new Vector2(75f, 17f);
                inputFieldLabel.anchoredPosition = Vector2.zero;

                LayoutElement lE = ciInputField.GetComponent<LayoutElement>();
                lE.minHeight = 75f;
            }

            // Library
            Mask mask2 = libraryUI.ScrollViewViewPort.GetComponent<Mask>();
            if (mask2 != null)
                Destroy(mask2);

            RectMask2D rectMask2D = libraryUI.ScrollViewViewPort.GetComponent<RectMask2D>();
            if (rectMask2D == null)
                rectMask2D = libraryUI.ScrollViewViewPort.gameObject.AddComponent<RectMask2D>();

            if (!OverhaulFeatureAvailabilitySystem.ImplementedInBuild.IsSelectionOutLineEnabled)
                return;

            RectTransform searchFieldRectT = libraryUI.SearchInput.transform as RectTransform;
            searchFieldRectT.pivot = new Vector2(0f, 0.5f);
            searchFieldRectT.anchorMax = new Vector2(1f, 1f);
            searchFieldRectT.anchorMin = new Vector2(0f, 0f);
            searchFieldRectT.offsetMax = new Vector2(-27f, -2f);
            searchFieldRectT.offsetMin = new Vector2(2f, 2f);
            searchFieldRectT.anchoredPosition = new Vector2(2f, 0f);
            searchFieldRectT.sizeDelta = new Vector2(-29f, -4f);

            RectTransform overhaulLVLEditorSettingsButtonTransform = new GameObject("OverhaulLevelEditorSettingsButton").AddComponent<RectTransform>();
            overhaulLVLEditorSettingsButtonTransform.SetParent(searchFieldRectT.parent, false);
            overhaulLVLEditorSettingsButtonTransform.localPosition = Vector3.zero;
            overhaulLVLEditorSettingsButtonTransform.localScale = Vector3.one;
            overhaulLVLEditorSettingsButtonTransform.eulerAngles = Vector3.zero;
            overhaulLVLEditorSettingsButtonTransform.pivot = new Vector2(1f, 0.5f);
            overhaulLVLEditorSettingsButtonTransform.anchorMax = new Vector2(1f, 1f);
            overhaulLVLEditorSettingsButtonTransform.anchorMin = new Vector2(0f, 0f);
            overhaulLVLEditorSettingsButtonTransform.offsetMax = new Vector2(-2f, -2f);
            overhaulLVLEditorSettingsButtonTransform.offsetMin = new Vector2(114f, 2f);
            overhaulLVLEditorSettingsButtonTransform.anchoredPosition = new Vector2(-2f, 0f);
            Image settingsButtonImage = overhaulLVLEditorSettingsButtonTransform.gameObject.AddComponent<Image>();
            settingsButtonImage.sprite = (libraryUI.SearchInput.targetGraphic as Image).sprite;
            settingsButtonImage.type = Image.Type.Sliced;

            if (SelectionSettingsPanel == null)
            {
                overhaulLVLEditorSettingsButtonTransform.gameObject.SetActive(false);
                return;
            }
            Button settingsButton = overhaulLVLEditorSettingsButtonTransform.gameObject.AddComponent<Button>();
            settingsButton.onClick.AddListener(SelectionSettingsPanel.ToggleVisibility);
        }

        private void addUIs()
        {
            if (OverhaulVersion.IsUpdate2Hotfix || GameUIRoot.Instance == null || GameUIRoot.Instance.LevelEditorUI == null)
                return;

            OverhaulCanvasController controller = OverhaulMod.Core.CanvasController;
            GameObject prefabToInstantiate = controller.GetHUDPrefab("LevelEditorUI_Settings");
            if (prefabToInstantiate == null)
                return;

            RectTransform panel = Instantiate(prefabToInstantiate, GameUIRoot.Instance.LevelEditorUI.transform).transform as RectTransform;
            panel.localPosition = Vector3.zero;
            panel.eulerAngles = Vector3.zero;
            panel.localScale = Vector3.one;
            panel.pivot = new Vector2(0f, 0.5f);
            panel.anchorMax = new Vector2(0f, 0.5f);
            panel.anchorMin = new Vector2(0f, 0.5f);
            panel.anchoredPosition = new Vector2(150f, -50f);
            SelectionSettingsPanel = panel.gameObject.AddComponent<LevelEditorSelectionSettingsPanel>();
        }

        public override string[] Commands() => null;
        public override string OnCommandRan(string[] command) => null;
    }
}
