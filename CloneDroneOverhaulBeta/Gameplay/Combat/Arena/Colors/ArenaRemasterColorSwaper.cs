using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.ArenaRemaster
{
    public class ArenaRemasterColorSwaper : OverhaulMonoBehaviour
    {
        private List<Material> m_ColorMaterials = new List<Material>();
        private Color m_TargetColor;
        private Color m_CurrentColor;
        private Color m_PrevColor;

        private List<Material> m_LightsMaterials = new List<Material>();

        public override void Start()
        {
            ModdedObject m = base.GetComponents<ModdedObject>()[1];
            foreach (Object obj in m.objects)
            {
                m_ColorMaterials.Add((obj as GameObject).GetComponent<Renderer>().material);
            }

            ModdedObject m2 = base.GetComponents<ModdedObject>()[2];
            foreach (Object obj in m2.objects)
            {
                m_LightsMaterials.Add((obj as GameObject).GetComponent<Renderer>().material);
            }

            OverhaulEventManager.AddEventListener(GlobalEvents.LevelEditorLevelOpened, onLoadedLevel, true);
            OverhaulEventManager.AddEventListener("LevelSpawned", onLoadedLevel, true);
        }

        protected override void OnDisposed()
        {
            OverhaulEventManager.RemoveEventListener(GlobalEvents.LevelEditorLevelOpened, onLoadedLevel, true);
            OverhaulEventManager.RemoveEventListener("LevelSpawned", onLoadedLevel, true);
        }

        public bool IsLerping()
        {
            return m_CurrentColor != m_TargetColor;
        }

        public void LerpMaterials()
        {
            Color targetColor;
            LevelEditorArenaSettings activeSettings = ArenaCustomizationManager.Instance.GetActiveSettings();
            targetColor = activeSettings.HighlightColor * Mathf.Max(0f, activeSettings.HighlightEmission);

            m_PrevColor = m_TargetColor;
            m_TargetColor = targetColor;
            Hashtable hash = ArenaCustomizationManager.Instance.UpgradeRoomColorChangeHash.GetHash();
            hash["from"] = 0f;
            hash["to"] = 1f;
            hash["onupdate"] = "onLerp";
            iTween.ValueTo(base.gameObject, hash);

            foreach(Material mat in m_LightsMaterials)
            {
                mat.SetColor("_EmissionColor", activeSettings.LightsColor * activeSettings.LightsEmission);
            }
        }

        private void onLerp(float value)
        {
            int i = 0;
            do
            {
                Color newColor = Color.Lerp(m_PrevColor, m_TargetColor, value);
                Material material = m_ColorMaterials[i];
                material.SetColor("_EmissionColor", newColor);
                if(i == 0)
                {
                    m_CurrentColor = newColor;
                }
                i++;

            } while (i < m_ColorMaterials.Count);
        }

        private void onLoadedLevel()
        {
            Singleton<DelegateScheduler>.Instance.Schedule(delegate
            {
                LerpMaterials();
            }, 0.15f);
        }
    }
}