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

            FieldInfo[] fields = base.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (FieldInfo fieldInfo in fields)
            {
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
                    }

                    fieldInfo.SetValue(this, unityObject);
                }
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
        }

        public void SetTitleScreenButtonActive(bool value)
        {
            ModCache.titleScreenUI.setLogoAndRootButtonsVisible(GameModeManager.IsOnTitleScreen() && value);
        }
    }
}
