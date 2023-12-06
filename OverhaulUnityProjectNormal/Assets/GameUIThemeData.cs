using System;
using UnityEngine;

[CreateAssetMenu]
public class GameUIThemeData : ScriptableObject
{
    public GameObject SelectionCornerPrefab;

    [Header("Button Theming")]
    public ColorForState[] ButtonBackground;

    public ColorForState[] ButtonTextOutline;

    public ColorForState[] ButtonTextColor;

    [Header("Slider Theming")]
    public ColorForState[] SliderHandle;

    public ColorForState[] SliderFill;

    public ColorForState[] SliderSlideArea;

    [Header("Checkbox Theming")]
    public ColorForState[] CheckboxBackground;

    public ColorForState[] CheckboxLabel;

    [Header("Dropdown Theming")]
    public ColorForState[] DropdownItemBackground;

    public ColorForState[] DropdownItemText;
}
