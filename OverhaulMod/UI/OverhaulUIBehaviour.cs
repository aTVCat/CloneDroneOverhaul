using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class OverhaulUIBehaviour : OverhaulBehaviour
    {
        internal string name
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

        public bool isElement
        {
            get;
            set;
        }

        public virtual bool enableCursorIfVisible
        {
            get
            {
                return false;
            }
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
                if(tabManagerAttribute != null)
                {
                    tabManagers.Add((fieldInfo, tabManagerAttribute));
                    continue;
                }

                UIElementAttribute elementAttribute = fieldInfo.GetCustomAttribute<UIElementAttribute>();
                if (elementAttribute != null)
                {
                    UnityEngine.Object unityObject = elementAttribute.HasIndex()
                        ? GetObject(elementAttribute.Index, fieldInfo.FieldType)
                        : GetObject(elementAttribute.Name, fieldInfo.FieldType);

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
                    }

                    fieldInfo.SetValue(this, unityObject);
                }
            }
            
            foreach((FieldInfo, TabManagerAttribute) tm in tabManagers)
            {
                TabManager tabManager = new TabManager();
                FieldInfo fi = tm.Item1;
                TabManagerAttribute tma = tm.Item2;

                ModdedObject prefab = null;
                Transform container = null;
                if (tma.PrefabFieldName != null || tma.ContainerFieldName != null)
                {
                    FieldInfo prefabField = localType.GetField(tma.PrefabFieldName, bindingFlags);
                    if (prefabField == null)
                        throw new Exception($"[TabManager] Could not find tab prefab (Field, {tma.PrefabFieldName})");
                    prefab = prefabField.GetValue(this) as ModdedObject;
                    if (!prefab)
                        throw new Exception($"[TabManager] Could not find ModdedObject (Prefab, {tma.PrefabFieldName})");

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
                    if (onTabCreatedMethod != null)
                        onTabCreatedMethod.Invoke(this, new object[] { tab });
                }, delegate (UIElementTab tab)
                {
                    if (onTabSelectedMethod != null)
                        onTabSelectedMethod.Invoke(this, new object[] { tab });
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
        }

        public virtual void Hide()
        {
            base.gameObject.SetActive(false);
            if (!isElement)
                ModCache.gameUIRoot.RefreshCursorEnabled();
        }

        public void SetTitleScreenButtonActive(bool value)
        {
            ModCache.titleScreenUI.setLogoAndRootButtonsVisible(GameModeManager.IsOnTitleScreen() && value);
        }
    }
}
