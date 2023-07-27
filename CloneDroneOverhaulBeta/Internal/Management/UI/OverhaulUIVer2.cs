using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CDOverhaul
{
    public class OverhaulUIVer2 : OverhaulUI
    {
        private TitleScreenUI m_TitleScreen;
        public TitleScreenUI TitleScreen
        {
            get
            {
                if (!m_TitleScreen)
                    m_TitleScreen = GameUIRoot.Instance.TitleScreenUI;

                return m_TitleScreen;
            }
        }

        public override void Initialize()
        {
            if (MyModdedObject.objects.IsNullOrEmpty())
            {
                Hide();
                return;
            }

            AssignVariables(this);
            Hide();
        }

        public static void AssignVariables(OverhaulBehaviour behaviour)
        {
            ModdedObject moddedObject = behaviour.GetComponent<ModdedObject>();
            List<string> objectNames = new List<string>();
            Type type = behaviour.GetType();

            int objectIndex = 0;
            foreach (UnityEngine.Object @object in moddedObject.objects)
            {
                if (@object)
                {
                    if (objectNames.Contains(@object.name))
                    {
                        throw new Exception("There's more than 1 object called " + @object.name + " in " + type.Name + "! Index: " + objectIndex);
                    }
                    objectNames.Add(@object.name);
                }
                objectIndex++;
            }

            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (!fields.IsNullOrEmpty())
            {
                int fieldIndex = 0;
                do
                {
                    FieldInfo info = fields[fieldIndex];
                    ObjectReferenceAttribute objectReference = info.GetCustomAttribute<ObjectReferenceAttribute>();
                    if (objectReference != null)
                    {
                        int indexInModdedObject = objectNames.IndexOf(objectReference.ObjectName);
                        if (indexInModdedObject == -1)
                            throw new Exception("Could not find " + objectReference.ObjectName + " in " + type.Name + "!");

                        Type fieldType = info.FieldType;
                        UnityEngine.Object component = fieldType == typeof(GameObject)
                            ? (moddedObject.GetObject(indexInModdedObject, typeof(Transform)) as Transform).gameObject
                            : moddedObject.GetObject(indexInModdedObject, fieldType);
                        ObjectComponentsAttribute objectComponents = info.GetCustomAttribute<ObjectComponentsAttribute>();
                        if (objectComponents != null)
                        {
                            GameObject gameObject = moddedObject.GetObject<Transform>(indexInModdedObject).gameObject;
                            foreach (Type componentType in objectComponents.Components)
                            {
                                Component addedComponent = gameObject.AddComponent(componentType);
                                if (fieldType == componentType)
                                {
                                    component = addedComponent;
                                }
                            }
                        }

                        if (!component)
                        {
                            throw new Exception("Could not find component for " + info.Name + " in " + type.Name + "!");
                        }

                        ButtonActionReferenceAttribute buttonActionReference = info.GetCustomAttribute<ButtonActionReferenceAttribute>();
                        if (buttonActionReference != null)
                        {
                            if (!(component is Button))
                                throw new Exception("Could not assign onClick action to " + objectReference.ObjectName + " since it is not a button!");

                            MethodInfo methodInfo = type.GetMethod(buttonActionReference.MethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                            if (methodInfo == null)
                                throw new Exception("Could not find method called " + buttonActionReference.MethodName + " to use for " + objectReference.ObjectName + "!");

                            Button button = component as Button;
                            button.onClick.AddListener(delegate
                            {
                                _ = methodInfo.Invoke(behaviour, null);
                            });
                        }

                        ToggleActionReferenceAttribute toggleActionReference = info.GetCustomAttribute<ToggleActionReferenceAttribute>();
                        if (toggleActionReference != null)
                        {
                            if (!(component is Toggle))
                                throw new Exception("Could not assign onValueChanged action to " + objectReference.ObjectName + " since it is not a toggle!");

                            MethodInfo methodInfo = type.GetMethod(buttonActionReference.MethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { typeof(bool) }, null);
                            if (methodInfo == null)
                                throw new Exception("Could not find method called " + buttonActionReference.MethodName + " to use for " + objectReference.ObjectName + "!");

                            Toggle toggle = component as Toggle;
                            toggle.onValueChanged.AddListener(delegate (bool value)
                            {
                                _ = methodInfo.Invoke(behaviour, new object[] { value });
                            });
                        }

                        info.SetValue(behaviour, component);
                    }

                    fieldIndex++;
                } while (fieldIndex < fields.Length);
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