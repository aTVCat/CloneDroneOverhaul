using CDOverhaul.Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CDOverhaul.Graphics.ArenaOverhaul
{
    public class ArenaOverhaulController : OverhaulController
    {
        [OverhaulSettingWithNotification(1)]
        [OverhaulSetting("Mod.Arena.Interior overhaul", true)]
        public static bool IsArenaOverhaulEnabled;

        public readonly string[] IgnoredArenaInteriorParts = new string[]
        {
            "ARENA_THRONE_END_PARTS-0",
            "ARENA_THRONE_END_PARTS-2",
            "ARENA_THRONE_END_PARTS-3",
            "ArenaThroneSectionLights"
        };

        private Transform m_WorldRootTransform;

        private Transform m_ArenaFinalTransform;
        private Transform m_ArenaUpperInteriorTransform;

        private Transform m_LiftTransform;
        private Transform m_LiftWallTransform;

        private Transform m_TVsTransform;
        private Transform m_GiantScreen2Transform;

        private Material m_ArenaOverhaulMaterial;

        private Vector3 m_OgGiantScreen2Position;
        private Vector3 m_OgGiantScreen2EulerAngles;
        private Vector3 m_OgGiantScreen2LocalScale;

        public override void Initialize()
        {
            m_WorldRootTransform = WorldRoot.Instance.transform;

            m_ArenaFinalTransform = m_WorldRootTransform.FindChildRecurisve("ArenaFinal");
            m_ArenaUpperInteriorTransform = m_ArenaFinalTransform.FindChildRecurisve("Arena2019");

            m_LiftTransform = m_WorldRootTransform.FindChildRecurisve("LiftContainer");
            m_LiftWallTransform = m_LiftTransform.FindChildRecurisve("LiftWall (1)");

            m_TVsTransform = m_ArenaFinalTransform.FindChildRecurisve("ArenaSideTVs");
            m_GiantScreen2Transform = m_TVsTransform.FindChildRecurisve("GiantScreen (2)");
            m_OgGiantScreen2Position = m_GiantScreen2Transform.localPosition;
            m_OgGiantScreen2EulerAngles = m_GiantScreen2Transform.localEulerAngles;
            m_OgGiantScreen2LocalScale = m_GiantScreen2Transform.localScale;

            GameObject gameObject = Instantiate(OverhaulAssetsController.GetAsset("ArenaOverhaul", OverhaulAssetPart.ArenaOverhaul), m_ArenaFinalTransform);
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localEulerAngles = Vector3.zero;
            gameObject.transform.localScale = Vector3.one;
            gameObject.SetActive(IsArenaOverhaulEnabled);

            m_ArenaOverhaulMaterial = gameObject.GetComponentInChildren<MeshRenderer>().sharedMaterial;

            OverhaulEventsController.AddEventListener("ArenaSettingsRefreshed", onArenaSettingsUpdate, true);
            OverhaulEventsController.AddEventListener(OverhaulGameplayCoreController.GamemodeChangedEventString, onGamemodeUpdate, true);

            SetVanillaPartsActive(!IsArenaOverhaulEnabled);
            PatchGiantScreen2(IsArenaOverhaulEnabled);
        }

        public override void OnModDeactivated()
        {
            PatchGiantScreen2(false);
        }

        public void PatchGiantScreen2(bool overhaul)
        {
            if (!overhaul)
            {
                m_GiantScreen2Transform.localPosition = m_OgGiantScreen2Position;
                m_GiantScreen2Transform.localEulerAngles = m_OgGiantScreen2EulerAngles;
                m_GiantScreen2Transform.localScale = m_OgGiantScreen2LocalScale;
                return;
            }
            m_GiantScreen2Transform.localPosition = new Vector3(82, 75);
            m_GiantScreen2Transform.localEulerAngles = new Vector3(345, 90);
            m_GiantScreen2Transform.localScale = Vector3.one * 4.4f;
        }

        public void SetVanillaPartsActive(bool value)
        {
            for(int i = 0; i < m_ArenaUpperInteriorTransform.childCount; i++)
            {
                GameObject gameObject = m_ArenaUpperInteriorTransform.GetChild(i).gameObject;
                if (!IgnoredArenaInteriorParts.Contains(gameObject.name))
                {
                    gameObject.SetActive(value);
                }
            }
        }

        private void onArenaSettingsUpdate()
        {
            if (LevelManager.Instance.IsCurrentLevelHidingTheArena())
                return;

            LevelEditorArenaSettings activeSettings = ArenaCustomizationManager.Instance.GetActiveSettings();
            m_ArenaOverhaulMaterial.SetColor("_EmissionColor", activeSettings.HighlightColor * activeSettings.HighlightEmission);
        }

        private void onGamemodeUpdate()
        {
            PatchGiantScreen2(!GameModeManager.IsMultiplayer());
        }
    }
}
