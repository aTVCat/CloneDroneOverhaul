using System.Linq;
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
            "ARENA_THRONE_END_PARTS-3"
        };

        private Transform m_WorldRootTransform;

        private Transform m_ArenaFinalTransform;
        private Transform m_ArenaUpperInteriorTransform;

        private Transform m_LiftTransform;
        private Transform m_LiftWallTransform;

        private Transform m_ArenaMainTransform;

        private Transform m_EmperorSectionTransform;
        private Transform m_BattleCruiserTransform;

        private Transform m_CommentatorBoxTransform;
        private Transform m_CommentatronTransform;
        private Transform m_AnalysisBotTransform;

        private Transform m_ArenaCameraAnimatorTransform;
        private Transform m_CommentatorTargetTransform;

        private Transform m_ArenaGroundArrowsTransform;
        private Transform m_ReturnSignTransform;
        private Transform m_ArenaGroundArrowsSpotlightTransform;

        private Transform m_TVsTransform;
        private Transform m_GiantScreen2Transform;
        private Transform m_ArenaTVEndlessLevelTransform;

        private Transform m_GarbageShuteTransform;
        private Transform m_GarbageDoorTransform;
        private Transform m_GarbageDoor2Transform;

        private Transform m_OverhaulGarbageDoorTransform;

        private Material m_ArenaOverhaulMaterial;
        private Material m_ArenaLightsMaterial;

        // GiantScreen (2)
        private Vector3 m_OgGiantScreen2Position;
        private Vector3 m_OgGiantScreen2EulerAngles;
        private Vector3 m_OgGiantScreen2LocalScale;

        private Vector3 m_OgReturnSignPosition;
        private Vector3 m_OgReturnSignEulerAngles;

        private float m_TimeToRefreshVanillaArenaParts;

        public override void Initialize()
        {
            if (!OverhaulFeatureAvailabilitySystem.ImplementedInBuild.IsArenaOverhaulEnabled || !IsArenaOverhaulEnabled)
                return;

            m_WorldRootTransform = WorldRoot.Instance.transform;

            m_ArenaFinalTransform = m_WorldRootTransform.FindChildRecursive("ArenaFinal");
            m_ArenaUpperInteriorTransform = m_ArenaFinalTransform.FindChildRecursive("Arena2019");

            m_LiftTransform = m_WorldRootTransform.FindChildRecursive("LiftContainer");
            m_LiftWallTransform = m_LiftTransform.FindChildRecursive("LiftWall (1)");

            m_ArenaMainTransform = m_ArenaFinalTransform.FindChildRecursive("Arena");
            m_EmperorSectionTransform = m_ArenaMainTransform.FindChildRecursive("EmperorSection");
            m_EmperorSectionTransform.localPosition = new Vector3(-12.9f, 5.0925f, 1.663f);

            m_CommentatorBoxTransform = m_ArenaFinalTransform.FindChildRecursive("CommentatorBox");
            m_CommentatorBoxTransform.localPosition = new Vector3(-83.8f, 26.4f, 1.95f);
            m_CommentatorBoxTransform.localScale = Vector3.one * 1.3f;
            m_CommentatronTransform = m_CommentatorBoxTransform.FindChildRecursive("Commentatron");
            m_CommentatronTransform.localPosition = new Vector3(3.44f, 2, -3.71f);
            m_AnalysisBotTransform = m_CommentatorBoxTransform.FindChildRecursive("AnalysisBot");
            m_AnalysisBotTransform.localPosition = new Vector3(5.2f, 4.7f, 0.52f);

            m_ArenaCameraAnimatorTransform = m_ArenaFinalTransform.FindChildRecursive("ArenaCameraAnimator");
            m_CommentatorTargetTransform = m_ArenaCameraAnimatorTransform.FindChildRecursive("CommentatorTarget");
            m_CommentatorTargetTransform.localPosition = new Vector3(-62.55f, 38f, 0f);
            m_CommentatorTargetTransform.localEulerAngles = new Vector3(8f, 270f, 0f);

            m_TVsTransform = m_ArenaFinalTransform.FindChildRecursive("ArenaSideTVs");
            m_GiantScreen2Transform = m_TVsTransform.FindChildRecursive("GiantScreen (2)");
            m_OgGiantScreen2Position = m_GiantScreen2Transform.localPosition;
            m_OgGiantScreen2EulerAngles = m_GiantScreen2Transform.localEulerAngles;
            m_OgGiantScreen2LocalScale = m_GiantScreen2Transform.localScale;
            m_ArenaTVEndlessLevelTransform = m_TVsTransform.FindChildRecursive("ArenaTV_EndlessLevel");
            m_ArenaTVEndlessLevelTransform.localPosition = new Vector3(-76.5f, 25.15f, 0);
            m_ArenaTVEndlessLevelTransform.localEulerAngles = new Vector3(347.64f, 270f, 0f);

            m_ArenaGroundArrowsTransform = m_ArenaFinalTransform.FindChildRecursive("ArenaGroundArrows");
            m_ReturnSignTransform = m_ArenaGroundArrowsTransform.FindChildRecursive("ReturnSign");
            m_OgReturnSignPosition = m_ReturnSignTransform.localPosition;
            m_OgReturnSignEulerAngles = m_ReturnSignTransform.localEulerAngles;
            m_ArenaGroundArrowsSpotlightTransform = m_ArenaGroundArrowsTransform.FindChildRecursive("Spotlight");
            m_ArenaGroundArrowsSpotlightTransform.localPosition = new Vector3(60, 55, 0);
            m_ArenaGroundArrowsSpotlightTransform.localEulerAngles = new Vector3(47.5f, 90, 90);
            m_ArenaGroundArrowsSpotlightTransform.localScale = Vector3.one;

            m_GarbageShuteTransform = m_ArenaFinalTransform.FindChildRecursive("GarbageShute");
            m_GarbageDoorTransform = m_GarbageShuteTransform.FindChildRecursive("GarbageDoor2019");
            m_GarbageDoorTransform.GetComponent<Renderer>().enabled = false;
            m_GarbageDoor2Transform = m_GarbageShuteTransform.FindChildRecursive("GarbageDoor2019Static");
            m_GarbageDoor2Transform.GetComponent<Renderer>().enabled = false;

            GameObject gameObject = Instantiate(OverhaulAssetsController.GetAsset("ArenaOverhaul", OverhaulAssetPart.ArenaOverhaul), m_ArenaFinalTransform);
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localEulerAngles = Vector3.zero;
            gameObject.transform.localScale = Vector3.one;

            ModdedObject moddedObject = gameObject.GetComponent<ModdedObject>();
            m_OverhaulGarbageDoorTransform = moddedObject.GetObject<Transform>(0);

            m_ArenaOverhaulMaterial = gameObject.GetComponentInChildren<MeshRenderer>().sharedMaterial;
            m_ArenaOverhaulMaterial.shader = Shader.Find("Standard");
            m_ArenaLightsMaterial = gameObject.transform.GetChild(1).GetComponentInChildren<MeshRenderer>().sharedMaterial;
            m_ArenaLightsMaterial.shader = Shader.Find("Standard");

            _ = OverhaulEventsController.AddEventListener("ArenaSettingsRefreshed", onArenaSettingsUpdate, true);

            SetVanillaPartsActive(false);
            PatchVanillaParts(true);
            setUpBattleCruiser();
            onArenaSettingsUpdate();
        }

        public override void OnModDeactivated()
        {
            PatchVanillaParts(false);
        }

        public void PatchVanillaParts(bool overhaul)
        {
            if (!m_GiantScreen2Transform || !m_ReturnSignTransform)
                return;

            if (!overhaul)
            {
                m_GiantScreen2Transform.localPosition = m_OgGiantScreen2Position;
                m_GiantScreen2Transform.localEulerAngles = m_OgGiantScreen2EulerAngles;
                m_GiantScreen2Transform.localScale = m_OgGiantScreen2LocalScale;

                m_ReturnSignTransform.localPosition = m_OgReturnSignPosition;
                m_ReturnSignTransform.localEulerAngles = m_OgReturnSignEulerAngles;
                return;
            }
            m_GiantScreen2Transform.localPosition = new Vector3(82, 75);
            m_GiantScreen2Transform.localEulerAngles = new Vector3(345, 90);
            m_GiantScreen2Transform.localScale = Vector3.one * 4.4f;

            m_ReturnSignTransform.localPosition = new Vector3(83, 47, 0);
            m_ReturnSignTransform.localEulerAngles = new Vector3(350, 90, 0);
        }

        public void SetVanillaPartsActive(bool value)
        {
            for (int i = 0; i < m_ArenaUpperInteriorTransform.childCount; i++)
            {
                GameObject gameObject = m_ArenaUpperInteriorTransform.GetChild(i).gameObject;
                if (!IgnoredArenaInteriorParts.Contains(gameObject.name))
                {
                    gameObject.SetActive(value);
                }
                else
                {
                    if (gameObject.name == "ARENA_THRONE_END_PARTS-3")
                    {
                        gameObject.transform.localPosition = new Vector3(3.2f, 10f, 15.5f);
                    }
                }
            }
            m_LiftWallTransform.gameObject.SetActive(value);
        }

        private void setUpBattleCruiser()
        {
            m_BattleCruiserTransform = TransformUtils.FindChildRecursive(m_EmperorSectionTransform, "Battlecruiser");

            Transform transformBC = UnityEngine.Object.Instantiate<Transform>(EnemyFactory.Instance.Enemies[56].EnemyPrefab.GetComponent<BattleCruiserController>().CharacterModelPrefab.transform);
            foreach (MonoBehaviour behaviour in transformBC.GetComponentsInChildren<MonoBehaviour>())
                UnityEngine.Object.Destroy(behaviour);

            /*
            foreach (Renderer proj in transformBC.GetComponentsInChildren<Renderer>())
                proj.material = OverhaulAssetsController.GetAsset<Material>("M_NoFog", OverhaulAssetPart.Part2);
            */

            TransformUtils.HideAllChildren(transformBC);
            transformBC.GetChild(0).gameObject.SetActive(true);
            transformBC.SetParent(m_BattleCruiserTransform, false);
            transformBC.localPosition = new Vector3(0f, 0f, -0.65f);
            transformBC.localEulerAngles = Vector3.zero;
            transformBC.localScale = new Vector3(0.35f, 0.35f, 0.35f);

            m_BattleCruiserTransform.GetComponent<MeshRenderer>().enabled = false;
        }

        private void onArenaSettingsUpdate()
        {
            if (!LevelManager.Instance || !ArenaCustomizationManager.Instance || LevelManager.Instance.IsCurrentLevelHidingTheArena())
                return;

            LevelEditorArenaSettings activeSettings = ArenaCustomizationManager.Instance.GetActiveSettings();
            if (!activeSettings)
                return;

            if (m_ArenaOverhaulMaterial)
                m_ArenaOverhaulMaterial.SetColor("_EmissionColor", activeSettings.HighlightColor * activeSettings.HighlightEmission);
            if (m_ArenaLightsMaterial)
                m_ArenaLightsMaterial.SetColor("_EmissionColor", activeSettings.LightsColor * activeSettings.LightsEmission);
        }

        private void Update()
        {
            if (!OverhaulFeatureAvailabilitySystem.ImplementedInBuild.IsArenaOverhaulEnabled || !IsArenaOverhaulEnabled)
                return;

            if (!m_OverhaulGarbageDoorTransform || !m_GarbageDoorTransform)
                return;

            if (Time.unscaledTime >= m_TimeToRefreshVanillaArenaParts)
            {
                m_TimeToRefreshVanillaArenaParts = Time.unscaledTime + 5f;
                PatchVanillaParts(!GameModeManager.IsCoop());
            }
            Vector3 position = m_GarbageDoorTransform.position;
            position.x = -1.225f;
            position.y += 1.21f;
            position.z = 0f;
            m_OverhaulGarbageDoorTransform.localPosition = position;

            if (!m_ArenaUpperInteriorTransform)
                return;

            m_ArenaUpperInteriorTransform.gameObject.SetActive(!GameModeManager.IsInLevelEditor());
        }
    }
}
