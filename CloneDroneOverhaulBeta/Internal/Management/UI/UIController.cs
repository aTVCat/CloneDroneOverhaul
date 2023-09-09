using CDOverhaul.HUD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CDOverhaul
{
    public class UIController : OverhaulAdvancedBehaviour
    {
        private ModdedObject m_ModdedObject;
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

        private byte m_EnableCursorConditionID = 0;
        protected bool ShowCursor
        {
            get
            {
                return m_EnableCursorConditionID != 0;
            }
            set
            {
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

        private TitleScreenUI m_TitleScreen;
        public TitleScreenUI TitleScreen
        {
            get
            {
                if (!m_TitleScreen)
                    m_TitleScreen = GameUIRoot.Instance?.TitleScreenUI;

                return m_TitleScreen;
            }
        }

        public virtual void Initialize()
        {
            if (MyModdedObject.objects.IsNullOrEmpty())
            {
                Hide();
                return;
            }

            AssignValues(this);
            AddListeners();
            Hide();
        }

        public virtual void OnGetArguments(object[] args)
        {

        }

        protected override void OnDisposed()
        {
            OverhaulDisposable.AssignNullToAllVars(this);
            RemoveListeners();
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

                    UIElementReferenceAttribute objectReference = info.GetCustomAttribute<UIElementReferenceAttribute>();
                    if (objectReference != null)
                    {
                        int indexInModdedObject = objectReference.UsesIndexes ? objectReference.ObjectIndex : objectsByNames[objectReference.ObjectName];

                        Type targetFieldType = info.FieldType;
                        UnityEngine.Object fieldValueToAssign = null;

                        if(targetFieldType == typeof(UIElementDropdown))
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

                        if (!fieldValueToAssign)
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
                                        throw new Exception("Could not find method called " + actionReference.MethodNames + " to use for " + objectReference.ObjectName + "!");

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
                                        throw new Exception("Could not find method called " + actionReference.MethodNames + " to use for " + objectReference.ObjectName + "!");

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
                                        throw new Exception("Could not find method called " + actionReference.MethodNames + " to use for " + objectReference.ObjectName + "!");

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
                                        throw new Exception("Could not find method called " + actionReference.MethodNames + " to use for " + objectReference.ObjectName + "!");

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

        public virtual void Show()
        {
            base.gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            base.gameObject.SetActive(false);
        }

        protected void HideTitleScreenButtons()
        {
            if (TitleScreen)
                TitleScreen.SetLogoAndRootButtonsVisible(false);
        }

        protected void ShowTitleScreenButtons()
        {
            if (GameModeManager.IsOnTitleScreen() && TitleScreen)
                TitleScreen.SetLogoAndRootButtonsVisible(true);
        }
    }
}