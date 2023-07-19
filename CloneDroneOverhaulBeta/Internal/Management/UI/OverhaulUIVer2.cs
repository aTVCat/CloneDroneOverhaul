using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
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

            List<string> objectNames = new List<string>();
            Type type = GetType();

            int objectIndex = 0;
            foreach(UnityEngine.Object @object in MyModdedObject.objects)
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
                    if(objectReference != null)
                    {
                        int indexInModdedObject = objectNames.IndexOf(objectReference.ObjectName);
                        if (indexInModdedObject == -1)
                            throw new Exception("Could not find " + objectReference.ObjectName + " in " + type.Name + "!");

                        Type fieldType = info.FieldType;
                        UnityEngine.Object component = null;
                        if(fieldType == typeof(GameObject))
                        {
                            component = (MyModdedObject.GetObject(indexInModdedObject, typeof(Transform)) as Transform).gameObject;
                        }
                        else
                        {
                            component = MyModdedObject.GetObject(indexInModdedObject, fieldType);
                        }

                        if (!component)
                            throw new Exception("Could not find component for " + info.Name + " in " + type.Name + "!");

                        info.SetValue(this, component);
                    }

                    fieldIndex++;
                } while (fieldIndex < fields.Length);
            }
            Hide();
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