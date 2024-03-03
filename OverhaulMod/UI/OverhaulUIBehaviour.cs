using OverhaulMod.UI.Attributes;
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
        public string uiName
        {
            get;
            set;
        }

        public bool isElement
        {
            get;
            private set;
        }

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

        public virtual bool dontRefreshUI
        {
            get
            {
                return false;
            }
        }

        public void InitializeUI()
        {
            List<(FieldInfo, TabManagerAttribute)> tabManagers = new List<(FieldInfo, TabManagerAttribute)>();

            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            Type localType = base.GetType();
            FieldInfo[] fields = localType.GetFields(bindingFlags);
            foreach (FieldInfo fieldInfo in fields)
            {
                TabManagerAttribute tabManagerAttribute = fieldInfo.GetCustomAttribute<TabManagerAttribute>();
                if (tabManagerAttribute != null)
                {
                    tabManagers.Add((fieldInfo, tabManagerAttribute));
                    continue;
                }

                UIElementAttribute elementAttribute = fieldInfo.GetCustomAttribute<UIElementAttribute>();
                if (elementAttribute != null)
                {
                    ColorPickerAttribute colorPickerAttribute = fieldInfo.GetCustomAttribute<ColorPickerAttribute>();
                    ShowTooltipOnHighLightAttribute showTooltipHighLightAttribute = fieldInfo.GetCustomAttribute<ShowTooltipOnHighLightAttribute>();
                    UIElementCallbackAttribute elementCallbackAttribute = fieldInfo.GetCustomAttribute<UIElementCallbackAttribute>();

                    GameObject gameObject = null;
                    UnityEngine.Object unityObject = null;
                    if (elementAttribute.ComponentToAdd != null)
                    {
                        gameObject = GetObject<GameObject>(elementAttribute.Name);
                        if (!gameObject)
                        {
                            Debug.LogError($"{localType}: Could not find GameObject \"{elementAttribute.Name}\" ({elementAttribute.Index})");
                            return;
                        }

                        UnityEngine.Object component = gameObject.AddComponent(elementAttribute.ComponentToAdd);
                        if (component is OverhaulUIBehaviour uib)
                            uib.InitializeElement();

                        if (component.GetType() == fieldInfo.FieldType)
                            unityObject = component;
                    }
                    else if (colorPickerAttribute != null)
                    {
                        gameObject = GetObject<GameObject>(elementAttribute.Name);
                        if (!gameObject)
                        {
                            Debug.LogError($"{localType}: Could not find GameObject \"{elementAttribute.Name}\" ({elementAttribute.Index})");
                            return;
                        }

                        UIElementColorPickerButton colorPickerButton = gameObject.AddComponent<UIElementColorPickerButton>();
                        colorPickerButton.InitializeElement();
                        colorPickerButton.useAlpha = colorPickerAttribute.UseAlpha;
                        unityObject = colorPickerButton;
                    }

                    if (!unityObject)
                    {
                        unityObject = elementAttribute.HasIndex() ? GetObject(elementAttribute.Index, fieldInfo.FieldType) : GetObject(elementAttribute.Name, fieldInfo.FieldType);
                        if (!unityObject)
                        {
                            Debug.LogError($"{localType}: Could not find object \"{elementAttribute.Name}\" ({elementAttribute.Index})");
                            return;
                        }
                    }

                    if (gameObject == null)
                    {
                        if (unityObject.GetType() == typeof(GameObject))
                            gameObject = unityObject as GameObject;
                        else if (unityObject is Behaviour)
                            gameObject = (unityObject as Behaviour).gameObject;
                    }

                    if (gameObject)
                    {
                        if (elementAttribute.DefaultActiveState != null)
                        {
                            gameObject.SetActive(elementAttribute.DefaultActiveState.Value);
                        }

                        if (showTooltipHighLightAttribute != null)
                        {
                            UIElementShowTooltipOnHightLight showTooltipOnHightLight = gameObject.AddComponent<UIElementShowTooltipOnHightLight>();
                            showTooltipOnHightLight.InitializeElement();
                            showTooltipOnHightLight.tooltipText = showTooltipHighLightAttribute.Text;
                            showTooltipOnHightLight.tooltipShowDuration = showTooltipHighLightAttribute.Duration;
                        }
                    }

                    UIElementActionAttribute actionAttribute = fieldInfo.GetCustomAttribute<UIElementActionAttribute>();
                    if (actionAttribute != null)
                    {
                        MethodInfo methodInfo = null;
                        if (fieldInfo.FieldType == typeof(Button))
                        {
                            methodInfo = localType.GetMethod(actionAttribute.Name, bindingFlags);
                            if (methodInfo != null)
                            {
                                Button button = unityObject as Button;
                                button.onClick.AddListener(delegate
                                {
                                    _ = methodInfo.Invoke(this, null);
                                });
                            }
                        }
                        else if (fieldInfo.FieldType == typeof(Dropdown))
                        {
                            methodInfo = localType.GetMethod(actionAttribute.Name, new System.Type[] { typeof(int) });
                            if (methodInfo != null)
                            {
                                Dropdown dropdown = unityObject as Dropdown;
                                dropdown.onValueChanged.AddListener(delegate (int index)
                                {
                                    _ = methodInfo.Invoke(this, new object[] { index });
                                });
                            }
                        }
                        else if (fieldInfo.FieldType == typeof(Toggle))
                        {
                            methodInfo = localType.GetMethod(actionAttribute.Name, new System.Type[] { typeof(bool) });
                            if (methodInfo != null)
                            {
                                Toggle toggle = unityObject as Toggle;
                                toggle.onValueChanged.AddListener(delegate (bool value)
                                {
                                    _ = methodInfo.Invoke(this, new object[] { value });
                                });
                            }
                        }
                        else if (fieldInfo.FieldType == typeof(InputField))
                        {
                            methodInfo = localType.GetMethod(actionAttribute.Name, new System.Type[] { typeof(string) });
                            if (methodInfo != null)
                            {
                                InputField inputField = unityObject as InputField;
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
                            methodInfo = localType.GetMethod(actionAttribute.Name, new System.Type[] { typeof(float) });
                            if (methodInfo != null)
                            {
                                Slider slider = unityObject as Slider;
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
                            methodInfo = localType.GetMethod(actionAttribute.Name, new System.Type[] { typeof(Color) });
                            if (methodInfo != null)
                            {
                                UIElementColorPickerButton colorPickerButton = unityObject as UIElementColorPickerButton;
                                colorPickerButton.onValueChanged.AddListener(delegate (Color value)
                                {
                                    _ = methodInfo.Invoke(this, new object[] { value });
                                });
                            }
                        }
                    }

                    fieldInfo.SetValue(this, unityObject);
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

            OnInitialized();
        }

        public void InitializeElement()
        {
            isElement = true;
            InitializeUI();
        }

        protected virtual void OnInitialized()
        {

        }

        public override void OnDestroy()
        {
            if (!isElement)
                ModUIManager.Instance.RemoveFromList(this);
        }

        public virtual void Show()
        {
            base.gameObject.SetActive(true);
            if (!isElement)
                ModUIManager.Instance.RefreshUI(dontRefreshUI);
        }

        public virtual void Hide()
        {
            base.gameObject.SetActive(false);
            if (!isElement)
                ModUIManager.Instance.RefreshUI(dontRefreshUI);
        }

        public virtual void ToggleVisibility()
        {
            if (visible)
                Hide();
            else
                Show();
        }
    }
}
