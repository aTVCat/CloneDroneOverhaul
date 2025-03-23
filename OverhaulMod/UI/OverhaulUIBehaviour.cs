using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class OverhaulUIBehaviour : ModBehaviour
    {
        private static readonly BindingFlags s_initializationBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public string Name;

        public bool IsElement;

        protected bool m_initialized;

        public bool visible
        {
            get
            {
                return gameObject.activeSelf;
            }
        }

        public bool visibleInHierarchy
        {
            get
            {
                return gameObject.activeInHierarchy;
            }
        }

        public virtual bool forceCancelHide
        {
            get
            {
                return false;
            }
        }

        public virtual bool enableCursor
        {
            get
            {
                return false;
            }
        }

        public virtual bool enableUIOverLogoMode
        {
            get
            {
                return false;
            }
        }

        public virtual bool hideTitleScreen
        {
            get
            {
                return false;
            }
        }

        public virtual bool refreshOnlyCursor
        {
            get
            {
                return false;
            }
        }

        public virtual bool closeOnEscapeButtonPress
        {
            get
            {
                return true;
            }
        }

        public void InitializeUI()
        {
            List<(FieldInfo, TabManagerAttribute)> tabManagers = new List<(FieldInfo, TabManagerAttribute)>();

            BindingFlags bindingFlags = s_initializationBindingFlags;
            Type localType = base.GetType();
            FieldInfo[] fields = localType.GetFields(bindingFlags);
            if (fields.Length != 0)
                for (int i = 0; i < fields.Length; i++)
                {
                    FieldInfo fieldInfo = fields[i];

                    TabManagerAttribute tabManagerAttribute = fieldInfo.GetCustomAttribute<TabManagerAttribute>();
                    if (tabManagerAttribute != null)
                    {
                        tabManagers.Add((fieldInfo, tabManagerAttribute));
                        continue;
                    }

                    UIElementAttribute elementAttribute = fieldInfo.GetCustomAttribute<UIElementAttribute>();
                    if (elementAttribute == null)
                        continue;

                    bool logErrorIfElementIsMissing = fieldInfo.GetCustomAttribute<UIElementIgnoreIfMissingAttribute>() == null;

                    GameObject elementObject = null;
                    elementObject = GetObject<GameObject>(elementAttribute.Name);
                    if (!elementObject)
                    {
                        if (logErrorIfElementIsMissing)
                            ModDebug.LogError($"{localType}: Could not find element \"{elementAttribute.Name}\"");

                        continue;
                    }

                    KeyBindSetterAttribute keyBindSetterAttribute = fieldInfo.GetCustomAttribute<KeyBindSetterAttribute>();
                    ColorPickerAttribute colorPickerAttribute = fieldInfo.GetCustomAttribute<ColorPickerAttribute>();
                    ShowTooltipOnHighLightAttribute showTooltipHighLightAttribute = fieldInfo.GetCustomAttribute<ShowTooltipOnHighLightAttribute>();
                    UIElementCallbackAttribute elementCallbackAttribute = fieldInfo.GetCustomAttribute<UIElementCallbackAttribute>();
                    ButtonWithSoundAttribute buttonWithSoundAttribute = fieldInfo.GetCustomAttribute<ButtonWithSoundAttribute>();

                    if (elementAttribute.DefaultActiveState != null)
                    {
                        elementObject.SetActive(elementAttribute.DefaultActiveState.Value);
                    }

                    if (showTooltipHighLightAttribute != null)
                    {
                        UIElementShowTooltipOnHightLight showTooltipOnHightLight = elementObject.AddComponent<UIElementShowTooltipOnHightLight>();
                        showTooltipOnHightLight.InitializeElement();
                        showTooltipOnHightLight.tooltipText = showTooltipHighLightAttribute.Text;
                        showTooltipOnHightLight.tooltipShowDuration = showTooltipHighLightAttribute.Duration;
                        showTooltipOnHightLight.textIsLocalizationId = showTooltipHighLightAttribute.TextIsLocalizationID;
                    }

                    if(buttonWithSoundAttribute != null && ModFeatures.IsEnabled(ModFeatures.FeatureType.UISounds))
                    {
                        ButtonWithSound buttonWithSound = elementObject.AddComponent<ButtonWithSound>();
                        buttonWithSound.Sound = buttonWithSoundAttribute.SoundType;
                    }

                    bool shouldGetComponent = false;
                    UnityEngine.Object element = null;
                    if (elementAttribute.ComponentToAdd != null)
                    {
                        UnityEngine.Object component = elementObject.AddComponent(elementAttribute.ComponentToAdd);
                        if (component is OverhaulUIBehaviour uib)
                            uib.InitializeElement();

                        if (component.GetType() == fieldInfo.FieldType)
                            element = component;
                        else
                            shouldGetComponent = true;
                    }
                    else if (colorPickerAttribute != null)
                    {
                        UIElementColorPickerButton colorPickerButton = elementObject.AddComponent<UIElementColorPickerButton>();
                        colorPickerButton.InitializeElement();
                        colorPickerButton.useAlpha = colorPickerAttribute.UseAlpha;
                        element = colorPickerButton;
                    }
                    else if (keyBindSetterAttribute != null)
                    {
                        UIElementKeyBindSetter keyBindSetter = elementObject.AddComponent<UIElementKeyBindSetter>();
                        keyBindSetter.InitializeElement();
                        keyBindSetter.key = keyBindSetterAttribute.DefaultKey;
                        keyBindSetter.defaultKey = keyBindSetterAttribute.DefaultKey;
                        element = keyBindSetter;
                    }
                    else
                    {
                        shouldGetComponent = true;
                    }

                    if (shouldGetComponent)
                    {
                        element = GetObject(elementAttribute.Name, fieldInfo.FieldType);
                        if (!element)
                        {
                            if (logErrorIfElementIsMissing)
                                ModDebug.LogError($"{localType}: Could not find {fieldInfo.FieldType} of element \"{elementAttribute.Name}\"");

                            continue;
                        }
                    }
                    fieldInfo.SetValue(this, element);

                    UIElementActionAttribute actionAttribute = fieldInfo.GetCustomAttribute<UIElementActionAttribute>();
                    if (actionAttribute != null)
                    {
                        MethodInfo methodInfo = null;
                        if (fieldInfo.FieldType == typeof(Button))
                        {
                            methodInfo = localType.GetMethod(actionAttribute.Name, bindingFlags);
                            if (methodInfo != null)
                            {
                                Button button = element as Button;
                                button.onClick.AddListener(delegate
                                {
                                    _ = methodInfo.Invoke(this, null);
                                });
                            }
                        }
                        else if (fieldInfo.FieldType == typeof(Dropdown))
                        {
                            methodInfo = localType.GetMethod(actionAttribute.Name, bindingFlags, null, new System.Type[] { typeof(int) }, null);
                            if (methodInfo != null)
                            {
                                Dropdown dropdown = element as Dropdown;
                                dropdown.onValueChanged.AddListener(delegate (int index)
                                {
                                    _ = methodInfo.Invoke(this, new object[] { index });
                                });
                            }
                        }
                        else if (fieldInfo.FieldType == typeof(Toggle))
                        {
                            methodInfo = localType.GetMethod(actionAttribute.Name, bindingFlags, null, new System.Type[] { typeof(bool) }, null);
                            if (methodInfo != null)
                            {
                                Toggle toggle = element as Toggle;
                                toggle.onValueChanged.AddListener(delegate (bool value)
                                {
                                    _ = methodInfo.Invoke(this, new object[] { value });
                                });
                            }
                        }
                        else if (fieldInfo.FieldType == typeof(InputField))
                        {
                            methodInfo = localType.GetMethod(actionAttribute.Name, bindingFlags, null, new System.Type[] { typeof(string) }, null);
                            if (methodInfo != null)
                            {
                                InputField inputField = element as InputField;
                                if (elementCallbackAttribute != null && elementCallbackAttribute.CallOnEndEdit)
                                {
                                    inputField.onEndEdit.AddListener(delegate (string value)
                                    {
                                        _ = methodInfo.Invoke(this, new object[] { value });
                                    });
                                }
                                else
                                {
                                    inputField.onValueChanged.AddListener(delegate (string value)
                                    {
                                        _ = methodInfo.Invoke(this, new object[] { value });
                                    });
                                }
                            }
                        }
                        else if (fieldInfo.FieldType == typeof(Slider))
                        {
                            methodInfo = localType.GetMethod(actionAttribute.Name, bindingFlags, null, new System.Type[] { typeof(float) }, null);
                            if (methodInfo != null)
                            {
                                Slider slider = element as Slider;
                                slider.onValueChanged.AddListener(delegate (float value)
                                {
                                    _ = methodInfo.Invoke(this, new object[] { value });
                                });

                                if (actionAttribute.UsePatch)
                                    _ = slider.gameObject.AddComponent<BetterSliderCallback>();
                            }
                        }
                        else if (fieldInfo.FieldType == typeof(UIElementColorPickerButton))
                        {
                            methodInfo = localType.GetMethod(actionAttribute.Name, bindingFlags, null, new System.Type[] { typeof(Color) }, null);
                            if (methodInfo != null)
                            {
                                UIElementColorPickerButton colorPickerButton = element as UIElementColorPickerButton;
                                colorPickerButton.onValueChanged.AddListener(delegate (Color value)
                                {
                                    _ = methodInfo.Invoke(this, new object[] { value });
                                });
                            }
                        }
                        else if (fieldInfo.FieldType == typeof(UIElementKeyBindSetter))
                        {
                            methodInfo = localType.GetMethod(actionAttribute.Name, bindingFlags, null, new System.Type[] { typeof(KeyCode) }, null);
                            if (methodInfo != null)
                            {
                                UIElementKeyBindSetter keyBindSetter = element as UIElementKeyBindSetter;
                                keyBindSetter.onValueChanged.AddListener(delegate (KeyCode value)
                                {
                                    _ = methodInfo.Invoke(this, new object[] { value });
                                });
                            }
                        }
                        else if (fieldInfo.FieldType == typeof(UIElementVector3Field))
                        {
                            methodInfo = localType.GetMethod(actionAttribute.Name, bindingFlags, null, new System.Type[] { typeof(Vector3) }, null);
                            if (methodInfo != null)
                            {
                                UIElementVector3Field keyBindSetter = element as UIElementVector3Field;
                                keyBindSetter.onValueChanged.AddListener(delegate (Vector3 value)
                                {
                                    _ = methodInfo.Invoke(this, new object[] { value });
                                });
                            }
                        }
                    }
                }

            foreach ((FieldInfo, TabManagerAttribute) tm in tabManagers)
            {
                TabManager tabManager = new TabManager();
                FieldInfo fi = tm.Item1;
                TabManagerAttribute tma = tm.Item2;

                GameObject prefab = null;
                Transform container = null;
                if (tma.PrefabFieldName != null || tma.ContainerFieldName != null)
                {
                    FieldInfo prefabField = localType.GetField(tma.PrefabFieldName, bindingFlags);
                    if (prefabField == null)
                        throw new Exception($"[TabManager] Could not find tab prefab (Field, {tma.PrefabFieldName})");
                    prefab = prefabField.GetValue(this) as GameObject;
                    if (!prefab)
                    {
                        prefab = (prefabField.GetValue(this) as Component).gameObject;
                        if (!prefab)
                            throw new Exception($"[TabManager] Could not find ModdedObject (Prefab, {tma.PrefabFieldName})");
                    }

                    FieldInfo containerField = localType.GetField(tma.ContainerFieldName, bindingFlags);
                    if (containerField == null)
                        throw new Exception($"[TabManager] Could not find tab container (Field, {tma.ContainerFieldName})");
                    container = containerField.GetValue(this) as Transform;
                    if (!container)
                        throw new Exception($"[TabManager] Could not find Transform (Prefab, {tma.ContainerFieldName})");
                }

                MethodInfo onTabCreatedMethod = localType.GetMethod(tma.OnTabCreatedCallbackMethodName, new System.Type[] { typeof(UIElementTab) });
                MethodInfo onTabSelectedMethod = localType.GetMethod(tma.OnTabSelectedCallbackMethodName, new System.Type[] { typeof(UIElementTab) });

                tabManager.PreconfiguredTabs = tma.Tabs;
                tabManager.Config(prefab, container, tma.ComponentType, delegate (UIElementTab tab)
                {
                    _ = (onTabCreatedMethod?.Invoke(this, new object[] { tab }));
                }, delegate (UIElementTab tab)
                {
                    _ = (onTabSelectedMethod?.Invoke(this, new object[] { tab }));
                });

                if (!tma.Tabs.IsNullOrEmpty())
                    foreach (string tab in tma.Tabs)
                        tabManager.AddTab(tab);

                fi.SetValue(this, tabManager);
            }

            m_initialized = true;
            OnInitialized();
        }

        public void InitializeElement()
        {
            IsElement = true;
            InitializeUI();
        }

        protected virtual void OnInitialized()
        {

        }

        public override void OnDestroy()
        {
            if (!IsElement)
                ModUIManager.Instance.RemoveFromList(this);
        }

        public virtual void Show()
        {
            base.gameObject.SetActive(true);
            if (!IsElement)
                ModUIManager.Instance.RefreshUI(refreshOnlyCursor);
        }

        public virtual void Hide()
        {
            base.gameObject.SetActive(false);
            if (!IsElement)
                ModUIManager.Instance.RefreshUI(refreshOnlyCursor);

            ModUIManager.Instance.RemoveUIFromLastShown(this);
        }

        public virtual void ToggleVisibility()
        {
            if (visible)
                Hide();
            else
                Show();
        }

        public void DestroyThis()
        {
            Destroy(base.gameObject);
        }
    }
}
