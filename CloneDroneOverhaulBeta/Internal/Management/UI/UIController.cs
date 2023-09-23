using CDOverhaul.HUD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static KopiLua.Lua;

namespace CDOverhaul
{
    public class UIController : OverhaulAdvancedBehaviour
    {
        private ModdedObject m_ModdedObject;

        private TitleScreenUI m_TitleScreen;

        private byte m_EnableCursorConditionID = 0;

        private bool m_WereTitleScreenButtonsActive;

        public TitleScreenUI TitleScreen
        {
            get
            {
                if (!m_TitleScreen)
                    m_TitleScreen = GameUIRoot.Instance?.TitleScreenUI;

                return m_TitleScreen;
            }
        }

        public ModdedObject MyModdedObject
        {
            get
            {
                if (IsDisposedOrDestroyed())
                    return null;

                if (m_ModdedObject == null)
                    m_ModdedObject = base.GetComponent<ModdedObject>();

                return m_ModdedObject;
            }
        }

        private bool m_HideCursor;
        protected bool ShowCursor
        {
            get
            {
                bool useBetterMethod = OverhaulFeaturesSystem.Implemented.IsImplemented(OverhaulFeaturesSystem.Implemented.NEW_CURSOR_HIDING_METHOD);
                if (useBetterMethod)
                {
                    return m_HideCursor;
                }
                return m_EnableCursorConditionID != 0;
            }
            set
            {
                bool useBetterMethod = OverhaulFeaturesSystem.Implemented.IsImplemented(OverhaulFeaturesSystem.Implemented.NEW_CURSOR_HIDING_METHOD);
                if (useBetterMethod)
                {
                    m_HideCursor = value;
                    GameUIRoot gameUIRoot = GameUIRoot.Instance;
                    if (gameUIRoot)
                    {
                        gameUIRoot.RefreshCursorEnabled();
                    }
                    return;
                }

                if (value && !IsDisposedOrDestroyed())
                {
                    if (m_EnableCursorConditionID != 0)
                        return;

                    m_EnableCursorConditionID = EnableCursorController.DisableCursor();
                }
                else
                {
                    if (m_EnableCursorConditionID == 0)
                        return;

                    EnableCursorController.TryEnableCursor(m_EnableCursorConditionID);
                    m_EnableCursorConditionID = 0;
                }
            }
        }

        public virtual void Initialize()
        {
            InternalModBot.RegisterShouldCursorBeEnabledDelegate.Register(ShouldHideCursor);
            if (!MyModdedObject.objects.IsNullOrEmpty())
            {
                AssignValues(this);
            }
            AddListeners();
        }

        protected override void OnDisposed()
        {
            RemoveListeners();
            ShowCursor = false;
            InternalModBot.RegisterShouldCursorBeEnabledDelegate.UnRegister(ShouldHideCursor);
            OverhaulDisposable.AssignNullToAllVars(this);
        }

        public virtual void OnGetArguments(object[] args)
        {

        }

        protected virtual bool ShouldHideCursor() => ShowCursor;

        protected virtual bool HideTitleScreen() => false;
        protected virtual bool WaitForEscapeKeyToHide() => false;
        protected virtual bool RememberTitleScreenButtonsState() => true;
        protected virtual bool AutoCursorManagement() => true;

        public virtual void Update()
        {
            if (WaitForEscapeKeyToHide() && Input.GetKeyDown(KeyCode.Escape))
                Hide();
        }

        public virtual void Show()
        {
            if (IsDisposedOrDestroyed())
                return;

            base.gameObject.SetActive(true);
            if (HideTitleScreen())
                HideTitleScreenButtons();
            if (AutoCursorManagement())
                ShowCursor = true;
        }

        public virtual void Hide()
        {
            if (IsDisposedOrDestroyed())
                return;

            base.gameObject.SetActive(false);
            if (HideTitleScreen())
                ShowTitleScreenButtons();
            if (AutoCursorManagement())
                ShowCursor = false;
        }

        protected void HideTitleScreenButtons()
        {
            if (TitleScreen)
            {
                m_WereTitleScreenButtonsActive = TitleScreen.RootButtonsContainerBG.activeSelf;
                TitleScreen.setLogoAndRootButtonsVisible(false);
            }
        }

        protected void ShowTitleScreenButtons()
        {
            if (GameModeManager.IsOnTitleScreen() && TitleScreen)
                TitleScreen.setLogoAndRootButtonsVisible(RememberTitleScreenButtonsState() ? m_WereTitleScreenButtonsActive : true);
        }

        public static void AssignValues(OverhaulBehaviour behaviour)
        {
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            ModdedObject moddedObject = behaviour.GetComponent<ModdedObject>();
            Dictionary<string, int> objectsByNames = new Dictionary<string, int>();
            Type type = behaviour.GetType();

            // Get all objects
            int objectIndex = 0;
            foreach (UnityEngine.Object @object in moddedObject.objects)
            {
                if (@object)
                    objectsByNames.Add(@object.name, objectIndex);

                objectIndex++;
            }

            // Actions with objects
            List<FieldInfo> fields = type.GetFields(bindingFlags).ToList();
            fields.AddRange(type.BaseType.GetFields(bindingFlags).ToList());
            if (!fields.IsNullOrEmpty())
            {
                int fieldIndex = 0;
                do
                {
                    FieldInfo info = fields[fieldIndex];
                    Type targetFieldType = info.FieldType;
                    object fieldValueToAssign = null;
                    bool skip = false;

                    if (targetFieldType == typeof(PrefabContainer))
                    {
                        PrefabContainerAttribute prefabContainerAttribute = info.GetCustomAttribute<PrefabContainerAttribute>();
                        if (prefabContainerAttribute != null)
                        {
                            fieldValueToAssign = new PrefabContainer(moddedObject.GetObject<ModdedObject>(prefabContainerAttribute.PrefabIndex), moddedObject.GetObject<Transform>(prefabContainerAttribute.ContainerIndex));
                        }

                        skip = true;
                        info.SetValue(behaviour, fieldValueToAssign);
                    }

                    if (skip)
                    {
                        fieldIndex++;
                        continue;
                    }

                    UIElementReferenceAttribute objectReference = info.GetCustomAttribute<UIElementReferenceAttribute>();
                    if (objectReference != null)
                    {
                        int indexInModdedObject = objectReference.UsesIndexes ? objectReference.ObjectIndex : objectsByNames[objectReference.ObjectName];
                        if (targetFieldType == typeof(UIElementDropdown))
                        {
                            GameObject gameObject = moddedObject.GetObject<Transform>(indexInModdedObject).gameObject;
                            fieldValueToAssign = UIUtility.InitDropdown(gameObject);
                        }
                        else
                        {
                            fieldValueToAssign = targetFieldType == typeof(GameObject) ? moddedObject.GetObject<Transform>(indexInModdedObject).gameObject : moddedObject.GetObject(indexInModdedObject, targetFieldType);
                        }

                        // Add components
                        UIElementComponentsAttribute objectComponents = info.GetCustomAttribute<UIElementComponentsAttribute>();
                        if (objectComponents != null)
                        {
                            GameObject gameObject = moddedObject.GetObject<Transform>(indexInModdedObject).gameObject;
                            foreach (Type componentType in objectComponents.Components)
                            {
                                Component addedComponent = gameObject.AddComponent(componentType);
                                if (targetFieldType == componentType)
                                    fieldValueToAssign = addedComponent;
                            }
                        }

                        // Set is active
                        UIElementDefaultVisibilityStateAttribute defaultVisibility = info.GetCustomAttribute<UIElementDefaultVisibilityStateAttribute>();
                        if (defaultVisibility != null)
                            moddedObject.GetObject<Transform>(indexInModdedObject).gameObject.SetActive(defaultVisibility.ShouldBeActive);

                        if (fieldValueToAssign == null)
                            throw new Exception("Could not find component for " + info.Name + " in " + type.Name + "!");
                        else
                            info.SetValue(behaviour, fieldValueToAssign);

                        // Assign action to button
                        UIElementActionReferenceAttribute actionReference = info.GetCustomAttribute<UIElementActionReferenceAttribute>();
                        if (actionReference != null)
                        {
                            if (targetFieldType == typeof(Button))
                            {
                                Button button = fieldValueToAssign as Button;
                                foreach (string methodName in actionReference.MethodNames)
                                {
                                    MethodInfo methodInfo = type.GetMethod(methodName, bindingFlags);
                                    if (methodInfo == null)
                                        throw new Exception("Could not find method called " + actionReference.MethodNames + " to use for button " + objectReference.ObjectName + "!");

                                    button.AddOnClickListener(delegate
                                    {
                                        _ = methodInfo.Invoke(behaviour, null);
                                    });
                                }
                            }
                            else if (targetFieldType == typeof(Toggle))
                            {
                                Toggle toggle = fieldValueToAssign as Toggle;
                                foreach (string methodName in actionReference.MethodNames)
                                {
                                    MethodInfo methodInfo = type.GetMethod(methodName, bindingFlags, null, new Type[] { typeof(bool) }, null);
                                    if (methodInfo == null)
                                        throw new Exception("Could not find method called " + actionReference.MethodNames + " to use for toggle " + objectReference.ObjectName + "!");

                                    toggle.onValueChanged.AddListener(delegate (bool value)
                                    {
                                        _ = methodInfo.Invoke(behaviour, new object[] { value });
                                    });
                                }
                            }
                            else if (targetFieldType == typeof(InputField))
                            {
                                InputField inputField = fieldValueToAssign as InputField;
                                foreach (string methodName in actionReference.MethodNames)
                                {
                                    MethodInfo methodInfo = type.GetMethod(methodName, bindingFlags, null, new Type[] { typeof(string) }, null);
                                    if (methodInfo == null)
                                        throw new Exception("Could not find method called " + actionReference.MethodNames + " to use for inputfield " + objectReference.ObjectName + "!");

                                    inputField.onEndEdit.AddListener(delegate (string value)
                                    {
                                        _ = methodInfo.Invoke(behaviour, new object[] { value });
                                    });
                                }
                            }
                            else if (targetFieldType == typeof(Dropdown))
                            {
                                Dropdown dropdown = fieldValueToAssign as Dropdown;
                                foreach (string methodName in actionReference.MethodNames)
                                {
                                    MethodInfo methodInfo = type.GetMethod(methodName, bindingFlags, null, new Type[] { typeof(int) }, null);
                                    if (methodInfo == null)
                                        throw new Exception("Could not find method called " + actionReference.MethodNames + " to use for dropdown " + objectReference.ObjectName + "!");

                                    dropdown.onValueChanged.AddListener(delegate (int value)
                                    {
                                        _ = methodInfo.Invoke(behaviour, new object[] { value });
                                    });
                                }
                            }
                        }
                    }

                    fieldIndex++;
                } while (fieldIndex < fields.Count);
            }
        }

        public static void AssignActionToButton(ModdedObject moddedObject, string objectName, Action action)
        {
            foreach (UnityEngine.Object @object in moddedObject.objects)
            {
                if (@object && @object.name == objectName)
                {
                    Button component = (@object as GameObject).GetComponent<Button>();
                    if (component)
                    {
                        component.AddOnClickListener(new UnityAction(action));
                    }
                    else
                    {
                        throw new NullReferenceException("AssignActionToButton: " + objectName + " is not a button!");
                    }
                    return;
                }
            }
            throw new NullReferenceException("AssignActionToButton: Could not find button called " + objectName + "!");
        }

        public static void AssignActionToButton(ModdedObject moddedObject, int index, Action action)
        {
            Button component = (moddedObject.objects[index] as GameObject).GetComponent<Button>();
            if (component)
                component.AddOnClickListener(new UnityAction(action));
        }

        public static void AssignActionToToggle(ModdedObject moddedObject, string objectName, Action<bool> action)
        {
            foreach (UnityEngine.Object @object in moddedObject.objects)
            {
                if (@object && @object.name == objectName)
                {
                    Toggle component = (@object as GameObject).GetComponent<Toggle>();
                    if (component)
                    {
                        component.onValueChanged.AddListener(new UnityAction<bool>(action));
                    }
                    else
                    {
                        throw new NullReferenceException("AssignActionToToggle: " + objectName + " is not a toggle!");
                    }
                    return;
                }
            }
            throw new NullReferenceException("AssignActionToToggle: Could not find toggle called " + objectName + "!");
        }

        public static T AddComponentToGameObject<T>(ModdedObject moddedObject, string objectName) where T : Component
        {
            foreach (UnityEngine.Object @object in moddedObject.objects)
            {
                if (@object && @object is GameObject && @object.name == objectName)
                {
                    return (@object as GameObject).AddComponent<T>();
                }
            }
            throw new NullReferenceException("AddComponentToGameObject: Could not find GameObject called " + objectName + "!");
        }
    }
}