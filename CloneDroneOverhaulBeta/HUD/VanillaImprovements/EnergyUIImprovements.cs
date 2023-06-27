using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD.Vanilla
{
    public class EnergyUIImprovements : OverhaulBehaviour
    {
        private ModdedObject m_ModdedObject;
        public OverhaulUI.PrefabAndContainer ErrorTextContainer;

        public List<string> SpawnedText = new List<string>();
        public List<ErrorTextInstanceBehaviour> Instances = new List<ErrorTextInstanceBehaviour>();

        public override void Start()
        {
            m_ModdedObject = base.GetComponent<ModdedObject>();
            ErrorTextContainer = new OverhaulUI.PrefabAndContainer(m_ModdedObject, 0, 1);
        }

        public void ShowText(string text)
        {
            if (SpawnedText.Contains(text))
            {
                foreach (ErrorTextInstanceBehaviour instance in Instances)
                {
                    if (!instance)
                        continue;

                    if (instance.Text == text)
                        instance.Repeat();
                }
                return;
            }

            ModdedObject moddedObject = ErrorTextContainer.CreateNew();
            moddedObject.transform.localPosition = Vector3.zero;
            moddedObject.GetComponent<Text>().text = text;
            ErrorTextInstanceBehaviour component = moddedObject.gameObject.AddComponent<ErrorTextInstanceBehaviour>();
            component.UIImprovements = this;
            component.Text = text;

            SpawnedText.Add(text);
        }

        public class ErrorTextInstanceBehaviour : OverhaulBehaviour
        {
            public EnergyUIImprovements UIImprovements;
            public string Text;

            private bool m_HasInitialized;
            private float m_SpawnTime;

            private int m_RepeatTimes;

            public void Repeat()
            {
                m_RepeatTimes++;
                Text text = base.GetComponent<Text>();
                text.text = Text + " (" + m_RepeatTimes + ")";
            }

            private void Update()
            {
                if (!m_HasInitialized)
                {
                    UIImprovements.Instances.Add(this);
                    m_RepeatTimes = 1;
                    m_SpawnTime = Time.unscaledTime;
                    m_HasInitialized = true;
                }

                if (Time.unscaledTime >= m_SpawnTime + 1.51f)
                {
                    if (UIImprovements && !string.IsNullOrEmpty(Text))
                        UIImprovements.SpawnedText.Remove(Text);

                    UIImprovements.Instances.Remove(this);
                    Destroy(base.gameObject);
                    return;
                }

                Vector3 position = base.transform.localPosition;
                position.y += 35f * Time.unscaledDeltaTime;
                base.transform.localPosition = position;
            }
        }
    }
}
