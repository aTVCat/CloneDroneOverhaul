using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CloneDroneOverhaul.V3Tests.Gameplay
{
    public static class LevelConstructor
    {
        public const GameMode RANDOMLEVELGENERATION_GAMEMODE = (GameMode)4519;
        public static Vector3 LIFT_POS = new Vector3(80, 0, 0);

        public const int PLAINS_ROUGHNESS_INDEX = 0;
        public const int RANDOMVALUE1_INDEX = 1;
        public const int MAXLOWLEVELITERATIONS_SEED_INDEX = 3;

        public const string CUBE_PREFAB_PATH = "Prefabs/LevelObjects/Primitives/CubeFlat";

        private static List<LevelTransform> _levelTransforms = new List<LevelTransform>();

        private static bool _isBuildingALevel = false;

        public static void BuildALevel(LevelSettings settings, bool async)
        {
            StaticCoroutineRunner.StartStaticCoroutine(buildALevel(settings, async));
        }

        public static void DestroyAllPRGLevels()
        {
            if (_levelTransforms.Count == 0)
            {
                return;
            }
            foreach (LevelTransform t in _levelTransforms)
            {
                if (t.Transform != null) GameObject.Destroy(t.Transform.gameObject);
            }
            _levelTransforms.Clear();
        }

        private static IEnumerator buildALevel(LevelSettings settings, bool async)
        {
            if (_isBuildingALevel)
            {
                yield break;
            }
            _isBuildingALevel = true;

            LevelSettings buildSettings = settings;
            if (buildSettings == null)
            {
                buildSettings = new LevelSettings();
            }

            //Cleaning all levels
            LevelManager.Instance.CleanUpLevelThisFrame();
            DestroyAllPRGLevels();

            //Making level transform
            Transform levelTransform = new GameObject().transform;
            levelTransform.position = Vector3.zero;

            //Level description
            LevelTransform tDesc = new LevelTransform();
            tDesc.Transform = levelTransform;
            tDesc.ID = Random.Range(10000, 99999).ToString();
            tDesc.Enemies = new List<Character>();
            tDesc.Chunks = new List<LevelChunk>();
            _levelTransforms.Add(tDesc);

            //Arena appearence
            //LevelEditorArenaSettings arenaSettings = LevelEditorObjectPlacementManager.Instance.PlaceObjectInLevelRoot(new LevelObjectEntry() { PathUnderResources = CUBE_PREFAB_PATH }, levelTransform).GetComponent<LevelEditorArenaSettings>();

            /*
            LevelStructureBase prevStruct = null;

            for(int i = 0; i < Mathf.Clamp(buildSettings.SeedInt, 1, 3) * 10; i++)
            {
                yield return new WaitForEndOfFrame();
                Vector3 objectRandomPos = new Vector3(Random.Range(-88, 88), 0, Random.Range(-50, 50));
                PlatformWithRobot platform = null;
                GameObject gObj = new GameObject("PlatformWithRobot");
                gObj.transform.SetParent(levelTransform);
                gObj.transform.position = objectRandomPos;
                platform = gObj.AddComponent<PlatformWithRobot>();
                platform.BuildStructure(buildSettings.Seed);

                if (prevStruct != null && platform.WillCrossBounds(prevStruct))
                {
                    platform.gameObject.SetActive(false);
                }
                prevStruct = platform;
            }
            */

            LevelGenerationCompletion generationProgress = new LevelGenerationCompletion();
            generationProgress.Seed = buildSettings.SeedInt;

            //Chunk Generation
            Vector3 position = new Vector3(93, 0, -56);
            levelTransform.gameObject.name = "LevelChunks (Seed: " + buildSettings.Seed + ")";
            for (int z = -6; z < 7; z++)
            {
                position.z += 8;
                position.x = 93;
                generationProgress.CurrentChuckX = 0;
                yield return null;
                for (int x = 10; x > -11; x--)
                {
                    position.x -= 8;
                    Transform t = SpawnObject(CUBE_PREFAB_PATH, levelTransform);
                    t.position = position;
                    t.localScale = new Vector3(8, 1, 8);
                    t.gameObject.name = "Chunk" + t.position.ToString();

                    t.GetComponent<ReplaceMaterial>().Color = new Color(0.32f, Random.Range(0.40f, 0.50f), 0, 1);

                    LevelChunk chunk = t.gameObject.AddComponent<LevelChunk>();
                    tDesc.Chunks.Add(chunk);
                    chunk.ChunkData = new LevelGenerationCompletion(generationProgress);

                    generationProgress.CurrentChuckX++;
                    generationProgress.CurrentChunkIndex++;
                }
                generationProgress.CurrentChuckZ++;
            }

            Texture2D noise = GeneratePerlinNoise(generationProgress);
            foreach (LevelChunk c in tDesc.Chunks)
            {
                c.ApplyPerlinNoise(noise);
                c.ApplyExceptions();
            }

            _isBuildingALevel = false;
            yield break;
        }

        static Texture2D GeneratePerlinNoise(LevelGenerationCompletion completion)
        {
            Texture2D noiseTex = new Texture2D(completion.CurrentChuckX, completion.CurrentChuckZ);
            Color[] pix = new Color[completion.CurrentChunkIndex];
            float xOrg = 0;
            float yOrg = 0;
            float y = 0.0F;

            while (y < noiseTex.height)
            {
                float x = 0.0F;
                while (x < noiseTex.width)
                {
                    float xCoord = xOrg + x / noiseTex.width;
                    float yCoord = yOrg + y / noiseTex.height;
                    float sample = Mathf.PerlinNoise(xCoord, yCoord);
                    pix[(int)y * noiseTex.width + (int)x] = new Color(sample, sample, sample);
                    x++;
                }
                y++;
            }

            for (int i = 0; i < pix.Length; i++)
            {
                Color c = pix[i];
                float s = Random.Range(-0.2f, 0.4f);
                c = new Color(c.r + s, c.g + s, c.b + s, 1f);
            }

            noiseTex.SetPixels(pix);
            noiseTex.Apply();

            return noiseTex;
        }

        public static Transform SpawnObject(Transform initial, Transform parent, Vector3 position, Vector3 rotation)
        {
            Transform tr = GameObject.Instantiate<Transform>(initial, position, Quaternion.Euler(rotation.x, rotation.y, rotation.z), parent);
            return tr;
        }
        public static Transform SpawnObject(string path, Transform parent)
        {
            Transform tr = LevelEditorObjectPlacementManager.Instance.PlaceObjectInLevelRoot(new LevelObjectEntry() { PathUnderResources = path }, parent).transform;
            return tr;
        }

        public struct LevelTransform
        {
            public Transform Transform;
            public string ID;
            public List<Character> Enemies;
            public List<LevelChunk> Chunks;
        }

        public class LevelSettings
        {
            public string Seed;
            public int SeedInt;

            public LevelSettings()
            {
                SeedInt = Random.Range(100, 999);
                Seed = SeedInt.ToString(); // 1 - Ladder with platform spawn rate / 2 - Size /
            }
        }


        public class LevelGenerationSettings : LevelSettings
        {
            public bool HasPlatformInFrontOfLift { get; set; }
        }

        public class LevelGenerationCompletion
        {
            public int CurrentChunkIndex = 0;
            public int CurrentChuckX = 0;
            public int CurrentChuckZ = 0;
            public int Seed;

            public LevelGenerationCompletion(LevelGenerationCompletion data)
            {
                CurrentChunkIndex = data.CurrentChunkIndex;
                CurrentChuckX = data.CurrentChuckX;
                CurrentChuckZ = data.CurrentChuckZ;
                Seed = data.Seed;
            }

            public LevelGenerationCompletion()
            {
            }
        }

        public class LevelChunk : MonoBehaviour
        {
            public static readonly int[] LIFT_INDEXES = new int[] { 147, 126, 105 };

            public LevelGenerationCompletion ChunkData;

            public void ApplyPerlinNoise(Texture2D noise)
            {
                int rough = Mathf.Clamp(ChunkData.Seed.ToString()[LevelConstructor.PLAINS_ROUGHNESS_INDEX], 1, 9);

                int randomValueSeedValue = ChunkData.Seed.ToString()[LevelConstructor.RANDOMVALUE1_INDEX];
                float randomValue = 2.5f;
                if (randomValueSeedValue > 7)
                {
                    randomValue = 2.25f;
                }
                else if (randomValueSeedValue > 4)
                {
                    randomValue = 1.65f;
                }
                float shift = ChunkData.Seed.ToString()[LevelConstructor.PLAINS_ROUGHNESS_INDEX] == 6 ? -randomValue : randomValue;

                base.transform.localScale = new Vector3(8, noise.GetPixel(ChunkData.CurrentChuckX, ChunkData.CurrentChuckZ).r * rough * 2, 8);
                if (ChunkData.Seed.ToString()[LevelConstructor.PLAINS_ROUGHNESS_INDEX] > 4)
                {
                    base.transform.position += new Vector3(0, (base.transform.localScale.y / shift) - (1.5f + Random.Range(0.100f, 1.000f)), 0);
                }

                if (randomValueSeedValue > 5) if (ChunkData.CurrentChuckX > Random.Range(4, 7) && ChunkData.CurrentChuckZ < Random.Range(4, 7))
                    {
                        base.transform.position += new Vector3(0, base.transform.position.y * 2, 0);
                    }
            }

            public void ApplyExceptions()
            {
                if (LIFT_INDEXES[0] == ChunkData.CurrentChunkIndex || LIFT_INDEXES[1] == ChunkData.CurrentChunkIndex || LIFT_INDEXES[2] == ChunkData.CurrentChunkIndex)
                {
                    base.gameObject.SetActive(false);
                }
            }
        }

        public class LevelStructureBase : MonoBehaviour
        {
            public virtual void BuildStructure(string seed)
            {

            }

            public virtual Bounds GetStructureWorldBounds()
            {
                Bounds result = default(Bounds);

                result.center = base.transform.position;
                result.extents = GetBoundsExtendtion();

                return result;
            }

            protected virtual Vector3 GetBoundsExtendtion()
            {
                return Vector3.zero;
            }

            protected Bounds GetTransformBounds(Transform t)
            {
                Bounds bounds = new Bounds();
                foreach (Collider col in t.GetComponentsInChildren<Collider>())
                {
                    bounds.Encapsulate(col.bounds);
                }
                return bounds;
            }

            public Collider GetCollider()
            {
                return base.GetComponentInChildren<Collider>();
            }

            /// <summary>
            /// Check if sturcute collides with other one
            /// </summary>
            /// <param name="structure"></param>
            /// <returns></returns>
            public bool WillCrossBounds(LevelStructureBase structure)
            {
                return structure.GetStructureWorldBounds().Intersects(structure.GetStructureWorldBounds());
            }
        }

        public class PlatformWithRobot : LevelStructureBase
        {
            private const float WedgeInitScaleX = 0.16f;
            private const float WedgeInitScaleY = 0.16f;
            private const float WedgeInitScaleZ = 0.1f;

            private const float PlatformInitScaleX = 0.16f;
            private const float PlatformInitScaleY = 0.16f;
            private const float PlatformInitScaleZ = 0.16f;

            private const string WedgePrefabPath = "Prefabs/LevelObjects/Primitives/Wedge";
            private const string PlatformPrefabPath = "Prefabs/LevelObjects/Primitives/Platform";

            Transform _myWedge;
            Transform _myPlatform;

            public override void BuildStructure(string seed)
            {
                //Start building from platform
                float size = 0;
                //float.TryParse(seed[LevelConstructor.PLATFORM_SEED_SIZE_INDEX].ToString(), out size);
                size = Mathf.Clamp(size, 0.2f, 3);

                float sizeX = Mathf.Clamp(WedgeInitScaleX + size, 1.6f, 2.3f);
                float sizeY = Mathf.Clamp((size / 2), 0.2f, 0.55f);
                float sizeZ = Mathf.Clamp(WedgeInitScaleZ + size, 1.5f, 1.8f);

                //_myWedge = LevelConstructor.SpawnObject(WedgePrefabPath, this.transform);
                //_myWedge.transform.localPosition = Vector3.zero;
                //_myWedge.transform.localScale = new Vector3(sizeX, sizeY, sizeZ);

                Bounds bounds = GetStructureWorldBounds();

                _myPlatform = LevelConstructor.SpawnObject(PlatformPrefabPath, this.transform);
                _myPlatform.transform.localScale = new Vector3(sizeX, sizeY, sizeZ + 0.06f);
                //_myPlatform.transform.localPosition = new Vector3((bounds.min.x - (GetStructureBounds().center.x)), 0, 0);
                _myPlatform.transform.localPosition = Vector3.zero;
            }

            protected override Vector3 GetBoundsExtendtion()
            {
                Vector3 result = Vector3.zero;
                Transform t = base.transform;

                result.x = t.localScale.x;
                result.y = t.localScale.y;
                result.z = t.localScale.z;

                return result;
            }
        }
    }
}
