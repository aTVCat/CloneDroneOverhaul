using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SelectableUI : MonoBehaviour
{
    public bool SkipSelectionArrows;

    [Header("UI Theming")]
    public GameUIThemeData GameThemeData;

    [Header("Button Theming")]
    public Outline OutlineToColor;

    public Text TextToColor;

    public Image ImageToColor;

    [Header("Slider Theming")]
    public Image SliderHandle;

    public Image SliderFill;

    public Image SliderSlideArea;

    [Header("Checkbox Theming")]
    public Image CheckboxBackground;

    public Text CheckboxLabel;

    public Text AlternativeCheckboxLabel;

    [Header("Dropdown Theming")]
    public Image DropdownItemBackground;

    public Text DropdownItemText;

    private UISelectionState _currentState;

    private Animator _selectionCornersAnimator;

    private void Start()
    {
        this.addHelperIfDropdown();
        this.addHelperIfDropdownItem();
    }

    private void OnEnable()
    {
        this.RefreshToInitialState();
    }

    private void OnDisable()
    {
        this.goToState(UISelectionState.None);
        this.hideCornersAnimator();
    }

    public void RefreshToInitialState()
    {
        this.goToState(UISelectionState.None);
    }

    private void addHelperIfDropdown()
    {

    }

    private void addHelperIfDropdownItem()
    {

    }

    public void OnSelect(BaseEventData eventData)
    {
        this.goToState(UISelectionState.Selected);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        this.goToState(UISelectionState.None);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.goToState(UISelectionState.None);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        this.goToState(UISelectionState.Selected);
    }

    private void goToState(UISelectionState newState)
    {
        this.onStateExit(this._currentState);
        this._currentState = newState;
        this.onStateEnter(newState);
    }

    private void onStateEnter(UISelectionState stateEntering)
    {
        this.updateColorsToState(stateEntering);
        switch (stateEntering)
        {
        case UISelectionState.None:
        case UISelectionState.MouseOver:
            break;
        case UISelectionState.Selected:
            if (!this.SkipSelectionArrows && this.GameThemeData.SelectionCornerPrefab != null)
            {
                this.getEnabledCornersAnimator().Play("ButtonSelected");
            }
            break;
        default:
            return;
        }
    }

    private void onStateExit(UISelectionState stateExiting)
    {
        switch (stateExiting)
        {
        case UISelectionState.None:
        case UISelectionState.MouseOver:
            break;
        case UISelectionState.Selected:
            if (!this.SkipSelectionArrows && this.GameThemeData.SelectionCornerPrefab != null)
            {
                Animator enabledCornersAnimator = this.getEnabledCornersAnimator();
                if (enabledCornersAnimator.gameObject.activeInHierarchy)
                {
                    enabledCornersAnimator.Play("ButtonDeselected");
                }
            }
            break;
        default:
            return;
        }
    }

    private Animator getEnabledCornersAnimator()
    {
        if (this._selectionCornersAnimator == null && this.GameThemeData.SelectionCornerPrefab != null)
        {
            Transform transform = UnityEngine.Object.Instantiate<GameObject>(this.GameThemeData.SelectionCornerPrefab, base.transform).transform;
            transform.SetAsLastSibling();
            this._selectionCornersAnimator = transform.GetComponent<Animator>();
        }
        this._selectionCornersAnimator.enabled = true;
        this._selectionCornersAnimator.gameObject.SetActive(true);
        return this._selectionCornersAnimator;
    }

    private void hideCornersAnimator()
    {
        if (this._selectionCornersAnimator != null)
        {
            this._selectionCornersAnimator.enabled = false;
            this._selectionCornersAnimator.gameObject.SetActive(false);
        }
    }

    private void updateColorsToState(UISelectionState selectionState)
    {
        if (this.GameThemeData == null)
        {
            return;
        }
        this.colorImageForState(selectionState, this.ImageToColor, this.GameThemeData.ButtonBackground);
        this.colorTextForState(selectionState, this.TextToColor, this.GameThemeData.ButtonTextColor);
        this.colorOutlineForState(selectionState, this.OutlineToColor, this.GameThemeData.ButtonTextOutline);
        this.colorImageForState(selectionState, this.SliderFill, this.GameThemeData.SliderFill);
        this.colorImageForState(selectionState, this.SliderSlideArea, this.GameThemeData.SliderSlideArea);
        this.colorImageForState(selectionState, this.SliderHandle, this.GameThemeData.SliderHandle);
        this.colorImageForState(selectionState, this.CheckboxBackground, this.GameThemeData.CheckboxBackground);
        this.colorTextForState(selectionState, this.CheckboxLabel, this.GameThemeData.CheckboxLabel);
        this.colorTextForState(selectionState, this.AlternativeCheckboxLabel, this.GameThemeData.CheckboxLabel);
        this.colorImageForState(selectionState, this.DropdownItemBackground, this.GameThemeData.DropdownItemBackground);
        this.colorTextForState(selectionState, this.DropdownItemText, this.GameThemeData.DropdownItemText);
    }

    private void colorTextForState(UISelectionState selectionState, Text textToColor, ColorForState[] colors)
    {
        if (textToColor != null)
        {
            ColorForState colorForState2 = colors.FirstOrDefault((ColorForState colorForState) => colorForState.State == selectionState);
            if (colorForState2 != null)
            {
                Color color = colorForState2.Color;
                textToColor.color = color;
            }
        }
    }

    private void colorImageForState(UISelectionState selectionState, Image imageToColor, ColorForState[] colors)
    {
        if (imageToColor != null)
        {
            ColorForState colorForState2 = colors.FirstOrDefault((ColorForState colorForState) => colorForState.State == selectionState);
            if (colorForState2 != null)
            {
                Color color = colorForState2.Color;
                imageToColor.color = color;
            }
        }
    }

    private void colorOutlineForState(UISelectionState selectionState, Outline imageToColor, ColorForState[] colors)
    {
        if (imageToColor != null)
        {
            ColorForState colorForState2 = colors.FirstOrDefault((ColorForState colorForState) => colorForState.State == selectionState);
            if (colorForState2 != null)
            {
                Color color = colorForState2.Color;
                imageToColor.effectColor = color;
            }
        }
    }

    public void OnSubmit(BaseEventData eventData)
    {

    }

    public void OnCancel(BaseEventData eventData)
    {

    }
}
