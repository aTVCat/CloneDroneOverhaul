using OverhaulMod.Utils;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace OverhaulMod.Engine
{
    public class ArenaRemodelManager : Singleton<ArenaRemodelManager>, IGameLoadListener
    {
        [ModSettingRequireRestart]
        [ModSetting(ModSettingIDs.ENABLE_ARENA_REMODEL, true)]
        public static bool EnableRemodel;

        public readonly string[] IgnoredParts = new string[]
        {
            "ARENA_THRONE_END_PARTS-3"
        };

        private GameObject _newArenaObject;

        private Transform _worldRootTransform;

        private Transform _arenaFinalTransform;
        private Transform _arenaUpperInteriorTransform;

        private Transform _liftTransform;
        private Transform _liftWallTransform;

        private Transform _arenaMainTransform;

        private Transform _emperorSectionTransform;
        private Transform _battleCruiserTransform;

        private Transform _commentatorBoxTransform;
        private Transform _commentatronTransform;
        private Transform _analysisBotTransform;

        private Transform _arenaCameraAnimatorTransform;
        private Transform _commentatorTargetTransform;

        private Transform _arenaGroundArrowsTransform;
        private Transform _returnSignTransform;
        private Transform _arenaGroundArrowsSpotlightTransform;

        private Transform _tvsTransform;
        private Transform _giantScreen2Transform;
        private Transform _arenaTVEndlessLevelTransform;

        private Transform _garbageShuteTransform;
        private Transform _garbageDoorTransform;
        private Transform _garbageDoor2Transform;

        private Transform _overhaulGarbageDoorTransform;

        private Material _arenaOverhaulMaterial;
        private Material _arenaLightsMaterial;

        private Vector3 _ogGiantScreen2Position;
        private Vector3 _ogGiantScreen2EulerAngles;
        private Vector3 _ogGiantScreen2LocalScale;

        private Vector3 _ogReturnSignPosition;
        private Vector3 _ogReturnSignEulerAngles;

        private void Start()
        {
            RefreshArenaLook();
        }

        private void Update()
        {
            if (_overhaulGarbageDoorTransform && _garbageDoorTransform)
            {
                Vector3 position = _garbageDoorTransform.position;
                position.x = -1.225f;
                position.y += 1.21f;
                position.z = 0f;
                _overhaulGarbageDoorTransform.localPosition = position;
            }
        }

        public void OnGameLoaded()
        {
            RefreshArenaLook();
        }

        public void FixLiftInCoop()
        {
            if (GameModeManager.IsCoop()) StartCoroutine(waitThenFixArenaLiftInCoop());
        }

        public void RefreshArenaLook()
        {
            if (!EnableRemodel || _newArenaObject)
                return;

            _worldRootTransform = WorldRoot.Instance.transform;

            _arenaFinalTransform = _worldRootTransform.FindChildRecursive("ArenaFinal");
            _arenaUpperInteriorTransform = _arenaFinalTransform.FindChildRecursive("Arena2019");

            _liftTransform = _worldRootTransform.FindChildRecursive("LiftContainer");
            _liftWallTransform = _liftTransform.FindChildRecursive("LiftWall (1)");

            _arenaMainTransform = _arenaFinalTransform.FindChildRecursive("Arena");
            _emperorSectionTransform = _arenaMainTransform.FindChildRecursive("EmperorSection");

            _commentatorBoxTransform = _arenaFinalTransform.FindChildRecursive("CommentatorBox");
            _commentatronTransform = _commentatorBoxTransform.FindChildRecursive("Commentatron");
            if (!_commentatronTransform)
                _commentatronTransform = _commentatorBoxTransform.FindChildRecursive("Commentatron_Xmas(Clone)");

            _analysisBotTransform = _commentatorBoxTransform.FindChildRecursive("AnalysisBot");
            if (!_analysisBotTransform)
                _analysisBotTransform = _commentatorBoxTransform.FindChildRecursive("AnalysisBot_Xmas(Clone)");

            _arenaCameraAnimatorTransform = _arenaFinalTransform.FindChildRecursive("ArenaCameraAnimator");
            if (!_arenaCameraAnimatorTransform)
            {
                _arenaCameraAnimatorTransform = _worldRootTransform.FindChildRecursive("ArenaCameraAnimator");
            }

            _commentatorTargetTransform = _arenaCameraAnimatorTransform.FindChildRecursive("CommentatorTarget");

            _tvsTransform = _arenaFinalTransform.FindChildRecursive("ArenaSideTVs");
            _giantScreen2Transform = _tvsTransform.FindChildRecursive("GiantScreen (2)");
            _ogGiantScreen2Position = _giantScreen2Transform.localPosition;
            _ogGiantScreen2EulerAngles = _giantScreen2Transform.localEulerAngles;
            _ogGiantScreen2LocalScale = _giantScreen2Transform.localScale;
            _arenaTVEndlessLevelTransform = _tvsTransform.FindChildRecursive("ArenaTV_EndlessLevel");

            _arenaGroundArrowsTransform = _arenaFinalTransform.FindChildRecursive("ArenaGroundArrows");
            _returnSignTransform = _arenaGroundArrowsTransform.FindChildRecursive("ReturnSign");
            _ogReturnSignPosition = _returnSignTransform.localPosition;
            _ogReturnSignEulerAngles = _returnSignTransform.localEulerAngles;
            _arenaGroundArrowsSpotlightTransform = _arenaGroundArrowsTransform.FindChildRecursive("Spotlight");

            _garbageShuteTransform = _arenaFinalTransform.FindChildRecursive("GarbageShute");
            _garbageDoorTransform = _garbageShuteTransform.FindChildRecursive("GarbageDoor2019");
            _garbageDoorTransform.GetComponent<Renderer>().enabled = false;
            _garbageDoor2Transform = _garbageShuteTransform.FindChildRecursive("GarbageDoor2019Static");
            _garbageDoor2Transform.GetComponent<Renderer>().enabled = false;

            GameObject gameObject = Instantiate(ModResources.Prefab(AssetBundleConstants.MODELS, "ArenaOverhaul"), _arenaFinalTransform);
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localEulerAngles = Vector3.zero;
            gameObject.transform.localScale = Vector3.one;
            _newArenaObject = gameObject;

            ModdedObject moddedObject = gameObject.GetComponent<ModdedObject>();
            _overhaulGarbageDoorTransform = moddedObject.GetObject<Transform>(0);

            _arenaOverhaulMaterial = gameObject.GetComponentInChildren<MeshRenderer>().sharedMaterial;
            _arenaOverhaulMaterial.shader = Shader.Find("Standard");
            _arenaLightsMaterial = gameObject.transform.GetChild(1).GetComponentInChildren<MeshRenderer>().sharedMaterial;
            _arenaLightsMaterial.shader = Shader.Find("Standard");

            GlobalEventManager.Instance.AddEventListener(GlobalEvents.ArenaSettingsRefreshed, onArenaSettingsUpdate);

            SetUpperInteriorActive(true);
            SetVanillaPartsActive(false);
            PatchVanillaParts(true);
            setUpBattleCruiser();
            onArenaSettingsUpdate();
        }

        public void PatchVanillaParts(bool overhaul)
        {
            if (!_giantScreen2Transform || !_returnSignTransform)
                return;

            if (!overhaul)
            {
                _giantScreen2Transform.localPosition = _ogGiantScreen2Position;
                _giantScreen2Transform.localEulerAngles = _ogGiantScreen2EulerAngles;
                _giantScreen2Transform.localScale = _ogGiantScreen2LocalScale;

                _returnSignTransform.localPosition = _ogReturnSignPosition;
                _returnSignTransform.localEulerAngles = _ogReturnSignEulerAngles;
                return;
            }
            _giantScreen2Transform.localPosition = new Vector3(82, 75);
            _giantScreen2Transform.localEulerAngles = new Vector3(345, 90);
            _giantScreen2Transform.localScale = Vector3.one * 4.4f;

            _returnSignTransform.localPosition = new Vector3(83, 47, 0);
            _returnSignTransform.localEulerAngles = new Vector3(350, 90, 0);

            _arenaTVEndlessLevelTransform.localPosition = new Vector3(-76.5f, 25.15f, 0);
            _arenaTVEndlessLevelTransform.localEulerAngles = new Vector3(347.64f, 270f, 0f);

            _arenaGroundArrowsSpotlightTransform.localPosition = new Vector3(60, 55, 0);
            _arenaGroundArrowsSpotlightTransform.localEulerAngles = new Vector3(47.5f, 90, 90);
            _arenaGroundArrowsSpotlightTransform.localScale = Vector3.one;

            _commentatorTargetTransform.localPosition = new Vector3(-62.55f, 38f, 0f);
            _commentatorTargetTransform.localEulerAngles = new Vector3(8f, 270f, 0f);

            if (_analysisBotTransform)
                _analysisBotTransform.localPosition = new Vector3(5.2f, 4.7f, 0.52f);
            if (_commentatronTransform)
                _commentatronTransform.localPosition = new Vector3(3.44f, 2, -3.71f);

            _commentatorBoxTransform.localPosition = new Vector3(-83.8f, 26.4f, 1.95f);
            _commentatorBoxTransform.localScale = Vector3.one * 1.3f;

            _emperorSectionTransform.localPosition = new Vector3(-12.9f, 5.0925f, 1.663f);
        }

        public void SetVanillaPartsActive(bool value)
        {
            for (int i = 0; i < _arenaUpperInteriorTransform.childCount; i++)
            {
                GameObject gameObject = _arenaUpperInteriorTransform.GetChild(i).gameObject;
                if (!IgnoredParts.Contains(gameObject.name))
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
            _liftWallTransform.gameObject.SetActive(value);
        }

        public void SetUpperInteriorActive(bool value)
        {
            if (_arenaUpperInteriorTransform) _arenaUpperInteriorTransform.gameObject.SetActive(value);
        }

        private void setUpBattleCruiser()
        {
            _battleCruiserTransform = TransformUtils.FindChildRecursive(_emperorSectionTransform, "Battlecruiser");
            if (!_battleCruiserTransform)
                return;

            Transform transformBC = Instantiate(EnemyFactory.Instance.GetEnemyConfiguration(EnemyType.BattlecruiserFlagship).EnemyPrefab.GetComponent<BattleCruiserController>().CharacterModelPrefab.transform);
            foreach (MonoBehaviour behaviour in transformBC.GetComponentsInChildren<MonoBehaviour>())
            {
                if (behaviour is MechBodyPart mbp)
                {
                    mbp.CanBeDamaged = false;
                }
                else if (behaviour is OverrideBodyPartMaterials obpm)
                {
                    obpm.Dead = obpm.NotTakingDamage;
                }
                else if (behaviour is PicaVoxel.Volume volume)
                {
                    volume.CollisionMode = PicaVoxel.CollisionMode.None;
                }

                if (!(behaviour is ReplaceVoxelColor || behaviour is ReplaceVoxelColorsInChildren || behaviour is OverrideBodyPartMaterials || behaviour is PicaVoxel.Volume || behaviour is PicaVoxel.Frame || behaviour is PicaVoxel.Chunk || behaviour is MechBodyPart))
                    DestroyImmediate(behaviour);
            }

            foreach (Collider collider in transformBC.GetComponentsInChildren<Collider>())
            {
                collider.enabled = false;
            }

            TransformUtils.HideAllChildren(transformBC);
            transformBC.GetChild(0).gameObject.SetActive(true);
            transformBC.SetParent(_battleCruiserTransform, false);
            transformBC.localPosition = new Vector3(0f, 0f, -0.65f);
            transformBC.localEulerAngles = Vector3.zero;
            transformBC.localScale = new Vector3(0.35f, 0.35f, 0.35f);

            _battleCruiserTransform.GetComponent<MeshRenderer>().enabled = false;
        }

        private IEnumerator waitThenFixArenaLiftInCoop()
        {
            yield return new WaitForSeconds(3f);
            if (!ArenaLiftManager.Instance) yield break;

            ArenaLift lift = ArenaLiftManager.Instance.Lift;
            if (lift && (lift._state == null || lift._stateHolder == null))
            {
                foreach (MovingPlatformStateHolder sh in Resources.FindObjectsOfTypeAll<MovingPlatformStateHolder>())
                {
                    BoltEntity boltEntity = sh.GetComponent<BoltEntity>();
                    if (!boltEntity || !boltEntity.IsAttached)
                        continue;

                    if (sh.state.UniqueIndex == lift.GetUniqueIndex())
                    {
                        lift._state = sh.state;
                        lift._stateHolder = sh;
                        break;
                    }
                }
            }
            yield break;
        }


        private void onArenaSettingsUpdate()
        {
            LevelEditorArenaSettings activeSettings = ArenaCustomizationManager.Instance.GetActiveSettings();
            if (activeSettings)
            {
                if (_arenaOverhaulMaterial) _arenaOverhaulMaterial.SetColor("_EmissionColor", activeSettings.HighlightColor * activeSettings.HighlightEmission);
                if (_arenaLightsMaterial) _arenaLightsMaterial.SetColor("_EmissionColor", activeSettings.LightsColor * activeSettings.LightsEmission);
            }
        }
    }
}