using System.Reflection;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class ModUIBehaviour : OverhaulBehaviour
    {
        internal string fullName
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
            get;
            private set;
        }

        public void Initialize()
        {
            if (initialized)
                return;

            FieldInfo[] fields = base.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (FieldInfo fieldInfo in fields)
            {
                UIElementAttribute elementAttribute = fieldInfo.GetCustomAttribute<UIElementAttribute>();
                if (elementAttribute != null)
                {
                    object @object = elementAttribute.HasIndex()
                        ? GetObject(elementAttribute.Index, fieldInfo.FieldType)
                        : GetObject(elementAttribute.Name, fieldInfo.FieldType);

                    UIElementActionAttribute actionAttribute = fieldInfo.GetCustomAttribute<UIElementActionAttribute>();
                    if (actionAttribute != null)
                    {
                        if (fieldInfo.FieldType == typeof(Button))
                        {
                            MethodInfo methodInfo = base.GetType().GetMethod(actionAttribute.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                            if(methodInfo != null)
                            {
                                Button button = @object as Button;
                                button.onClick.AddListener(delegate
                                {
                                    _ = methodInfo.Invoke(this, null);
                                });
                            }
                        }
                    }

                    fieldInfo.SetValue(this, @object);
                }
            }

            OnInitialized();
            initialized = true;
        }

        protected virtual void OnInitialized()
        {

        }

        protected virtual void OnDestroy()
        {
            ModUIManager.Instance.RemoveFromList(this);
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
            if (!GameModeManager.IsOnTitleScreen())
            {
                return;
            }
            ModCache.titleScreenUI.setLogoAndRootButtonsVisible(value);
        }

        public virtual void OnEnable()
        {
            visible = true;
        }

        public virtual void OnDisable()
        {
            visible = false;
        }
    }
}
