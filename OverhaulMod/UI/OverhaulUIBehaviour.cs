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
        internal string uiName
        {
            get;
            set;
        }

        protected bool initialized
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

        public bool isElement
        {
            get;
            set;
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

        public bool hasEverShowed
        {
            get;
            protected set;
        }

        public bool showedFromCode
        {
            get;
            set;
        }

        public void InitializeUI()
        {
            if (initialized)
                return;

            UIAttribute uiAttribute = base.GetType().GetCustomAttribute<UIAttribute>();
            if (uiAttribute != null && uiAttribute.FixOutlines)
            {
                /*
                foreach (Outline component in base.GetComponentsInChildren<Outline>())
                {
                    GameObject gameObject = component.gameObject;
                    Color effectColor = component.effectColor;
                    Vector2 effectDist = component.effectDistance;
                    bool useGraphicAlpha = component.useGraphicAlpha;
                    Destroy(component);

                    ToJOutline betterOutline = gameObject.AddComponent<ToJOutline>();
                    betterOutline.effectColor = effectColor;
                    betterOutline.effectDistance = effectDist;
                    betterOutline.useGraphicAlpha = useGraphicAlpha;
                }*/
            }

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

                ColorPickerAttribute colorPickerAttribute = fieldInfo.GetCustomAttribute<ColorPickerAttribute>();
                ShowTooltipOnHighLightAttribute showTooltipHighLightAttribute = fieldInfo.GetCustomAttribute<ShowTooltipOnHighLightAttribute>();
                UIElementCallbackAttribute elementCallbackAttribute = fieldInfo.GetCustomAttribute<UIElementCallbackAttribute>();

                UIElementAttribute elementAttribute = fieldInfo.GetCustomAttribute<UIElementAttribute>();
                if (elementAttribute != null)
                {
                    UnityEngine.Object unityObject = null;
                    if (elementAttribute.ComponentToAdd != null)
                    {
                        UnityEngine.Object obj = GetObject<GameObject>(elementAttribute.Name).AddComponent(elementAttribute.ComponentToAdd);
                        if (obj is OverhaulUIBehaviour uib)
                            uib.InitializeElement();

                        unityObject = obj.GetType() == fieldInfo.FieldType
                            ? obj
                            : elementAttribute.HasIndex()
                        ? GetObject(elementAttribute.Index, fieldInfo.FieldType)
                        : GetObject(elementAttribute.Name, fieldInfo.FieldType);
                    }
                    else if (colorPickerAttribute != null)
                    {
                        unityObject = GetObject<GameObject>(elementAttribute.Name).AddComponent<UIElementColorPickerButton>();
                        UIElementColorPickerButton colorPickerButton = unityObject as UIElementColorPickerButton;
                        colorPickerButton.InitializeElement();
                        colorPickerButton.useAlpha = colorPickerAttribute.UseAlpha;
                    }
                    else
                    {
                        unityObject = elementAttribute.HasIndex()
                        ? GetObject(elementAttribute.Index, fieldInfo.FieldType)
                        : GetObject(elementAttribute.Name, fieldInfo.FieldType);
                    }

                    if(showTooltipHighLightAttribute != null)
                    {
                        GameObject gameObject = null;
                        if (unityObject is GameObject go)
                            gameObject = go;
                        else if (unityObject is Component c)
                            gameObject = c.gameObject;

                        UIElementShowTooltipOnHightLight showTooltipOnHightLight = gameObject.AddComponent<UIElementShowTooltipOnHightLight>();
                        showTooltipOnHightLight.InitializeElement();
                        showTooltipOnHightLight.tooltipText = showTooltipHighLightAttribute.Text;
                        showTooltipOnHightLight.tooltipShowDuration = showTooltipHighLightAttribute.Duration;
                    }

                    if (!unityObject)
                    {
                        Debug.LogError(string.Format("{0}: Could not find object with name/index {1}/{2}", new object[]
                        {
                            GetType(),
                            elementAttribute.Name,
                            elementAttribute.Index
                        }));
                        continue;
                    }

                    if (elementAttribute.DefaultActiveState != null)
                    {
                        bool value = elementAttribute.DefaultActiveState.Value;
                        GameObject gameObject = null;

                        if (unityObject.GetType() == typeof(GameObject))
                            gameObject = unityObject as GameObject;
                        else if (unityObject is Behaviour)
                            gameObject = (unityObject as Behaviour).gameObject;

                        if (gameObject)
                            gameObject.SetActive(value);
                    }

                    UIElementActionAttribute actionAttribute = fieldInfo.GetCustomAttribute<UIElementActionAttribute>();
                    if (actionAttribute != null)
                    {
                        if (fieldInfo.FieldType == typeof(Button))
                        {
                            MethodInfo methodInfo = base.GetType().GetMethod(actionAttribute.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
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
                            MethodInfo methodInfo = base.GetType().GetMethod(actionAttribute.Name, new System.Type[] { typeof(int) });
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
                            MethodInfo methodInfo = base.GetType().GetMethod(actionAttribute.Name, new System.Type[] { typeof(bool) });
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
                            MethodInfo methodInfo = base.GetType().GetMethod(actionAttribute.Name, new System.Type[] { typeof(string) });
                            if (methodInfo != null)
                            {
                                InputField inputField = unityObject as InputField;
                                if(elementCallbackAttribute != null && elementCallbackAttribute.CallOnEndEdit)
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
                            MethodInfo methodInfo = base.GetType().GetMethod(actionAttribute.Name, new System.Type[] { typeof(float) });
                            if (methodInfo != null)
                            {
                                Slider slider = unityObject as Slider;
                                slider.onValueChanged.AddListener(delegate (float value)
                                {
                                    _ = methodInfo.Invoke(this, new object[] { value });
                                });

                                if(actionAttribute.UsePatch)
                                    slider.gameObject.AddComponent<BetterSliderCallback>();
                            }
                        }
                        else if (fieldInfo.FieldType == typeof(UIElementColorPickerButton))
                        {
                            MethodInfo methodInfo = base.GetType().GetMethod(actionAttribute.Name, new System.Type[] { typeof(Color) });
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
            initialized = true;
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
            {
                ModUIManager.Instance.RemoveFromList(this);
            }
        }

        public virtual void Show()
        {
            base.gameObject.SetActive(true);
            if (!isElement)
                ModUIManager.Instance.RefreshUI(dontRefreshUI);

            hasEverShowed = true;
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
