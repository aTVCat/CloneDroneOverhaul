using OverhaulMod.Utils;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OverhaulMod.Engine
{
    public class ArenaAudienceManager : Singleton<ArenaAudienceManager>, IGameLoadListener
    {
        public const string FILE_NAME = "arenaAudiencePlacement.json";

        private List<ArenaAudienceLinePoint> _linePoints;

        private List<AudiencePlacementLine> _customPlacementLines;
        private List<AudiencePlacementLine> _customVipPlacementLines;

        private AudiencePlacementLine[] _vanillaPlacementLines;
        private AudiencePlacementLine[] _vanillaVipPlacementLines;

        private ArenaAudienceLinePointInfoList _infoList;

        public void OnGameLoaded()
        {
            if (_linePoints == null)
                _linePoints = new List<ArenaAudienceLinePoint>();

            if (_customPlacementLines == null)
                _customPlacementLines = new List<AudiencePlacementLine>();

            if (_customVipPlacementLines == null)
                _customVipPlacementLines = new List<AudiencePlacementLine>();

            if (_infoList == null)
                LoadCustomPlacementLinesFile();

            ClearCustomPlacementLines();
            GameObject gameObject = new GameObject("OverhaulAudienceLines");
            CreateCustomPlacementLines(gameObject.transform);
            SetAudiencePlacement(ArenaRemodelManager.EnableRemodel, true);
        }

        public void AddLinePoint(ArenaAudienceLinePoint arenaAudienceLine)
        {
            if (!_linePoints.Contains(arenaAudienceLine))
                _linePoints.Add(arenaAudienceLine);
        }

        public void RemoveLinePoint(ArenaAudienceLinePoint arenaAudienceLine)
        {
            _ = _linePoints.Remove(arenaAudienceLine);
        }

        public void ClearLinesList(List<AudiencePlacementLine> list)
        {
            if (!list.IsNullOrEmpty())
            {
                foreach (AudiencePlacementLine line in list)
                {
                    if (line)
                    {
                        if (line.StartPos)
                            Destroy(line.StartPos.gameObject);

                        if (line.EndPos)
                            Destroy(line.EndPos.gameObject);

                        Destroy(line.gameObject);
                    }
                }
                list.Clear();
            }
        }

        public void LoadCustomPlacementLinesFile()
        {
            string path = Path.Combine(ModCore.DataFolder, FILE_NAME);

            ArenaAudienceLinePointInfoList arenaAudienceLinePointInfoList;
            try
            {
                arenaAudienceLinePointInfoList = ModJsonUtils.DeserializeStream<ArenaAudienceLinePointInfoList>(path);
            }
            catch
            {
                arenaAudienceLinePointInfoList = new ArenaAudienceLinePointInfoList();
            }
            arenaAudienceLinePointInfoList.FixValues();
            _infoList = arenaAudienceLinePointInfoList;
        }

        public void CreateCustomPlacementLines(Transform parent)
        {
            if (_infoList == null || _infoList.Points.IsNullOrEmpty())
                return;

            Dictionary<int, (ArenaAudienceLinePointInfo, ArenaAudienceLinePointInfo)> dictionary = new Dictionary<int, (ArenaAudienceLinePointInfo, ArenaAudienceLinePointInfo)>();
            foreach (ArenaAudienceLinePointInfo pointInfo in _infoList.Points)
            {
                if (!dictionary.ContainsKey(pointInfo.ID))
                {
                    if (pointInfo.IsEnd)
                    {
                        dictionary.Add(pointInfo.ID, (null, pointInfo));
                    }
                    else
                    {
                        dictionary.Add(pointInfo.ID, (pointInfo, null));
                    }
                }
                else
                {
                    (ArenaAudienceLinePointInfo, ArenaAudienceLinePointInfo) tuple = dictionary[pointInfo.ID];
                    if (pointInfo.IsEnd)
                    {
                        tuple.Item2 = pointInfo;
                    }
                    else
                    {
                        tuple.Item1 = pointInfo;
                    }
                    dictionary[pointInfo.ID] = tuple;
                }
            }

            foreach (KeyValuePair<int, (ArenaAudienceLinePointInfo, ArenaAudienceLinePointInfo)> keyValue in dictionary)
            {
                (ArenaAudienceLinePointInfo, ArenaAudienceLinePointInfo) tuple = keyValue.Value;
                if (tuple.Item1 != null && tuple.Item2 != null)
                {
                    AudiencePlacementLine audiencePlacementLine = new GameObject($"AudiencePlacementLine {keyValue.Key}").AddComponent<AudiencePlacementLine>();
                    audiencePlacementLine.transform.SetParent(parent, true);
                    audiencePlacementLine.IsLower = tuple.Item1.IsLower && tuple.Item2.IsLower;
                    audiencePlacementLine.IsMiddle = tuple.Item1.IsMiddle && tuple.Item2.IsMiddle;
                    audiencePlacementLine.IsTop = tuple.Item1.IsTop && tuple.Item2.IsTop;

                    Transform transform = new GameObject($"AudienceLinePoint (ID {keyValue.Key}, Start)").transform;
                    transform.SetParent(parent, true);
                    transform.position = new Vector3(tuple.Item1.PositionX, tuple.Item1.PositionY, tuple.Item1.PositionZ);
                    transform.eulerAngles = new Vector3(tuple.Item1.EulerAnglesX, tuple.Item1.EulerAnglesY, tuple.Item1.EulerAnglesZ);
                    audiencePlacementLine.StartPos = transform;

                    Transform transform2 = new GameObject($"AudienceLinePoint (ID {keyValue.Key}, End)").transform;
                    transform2.SetParent(parent, true);
                    transform2.position = new Vector3(tuple.Item2.PositionX, tuple.Item2.PositionY, tuple.Item2.PositionZ);
                    transform2.eulerAngles = new Vector3(tuple.Item2.EulerAnglesX, tuple.Item2.EulerAnglesY, tuple.Item2.EulerAnglesZ);
                    audiencePlacementLine.EndPos = transform2;

                    if (tuple.Item1.IsVIP && tuple.Item2.IsVIP)
                    {
                        _customVipPlacementLines.Add(audiencePlacementLine);
                    }
                    else
                    {
                        _customPlacementLines.Add(audiencePlacementLine);
                    }
                }
            }
        }

        public void ClearCustomPlacementLines()
        {
            ClearLinesList(_customPlacementLines);
            ClearLinesList(_customVipPlacementLines);
        }

        public void SaveCustomPlacementLines()
        {
            if (_linePoints.IsNullOrEmpty())
                return;

            ArenaAudienceLinePointInfoList arenaAudienceLinePointInfoList = new ArenaAudienceLinePointInfoList();
            arenaAudienceLinePointInfoList.FixValues();

            foreach (ArenaAudienceLinePoint point in _linePoints)
            {
                ArenaAudienceLinePointInfo arenaAudienceLinePointInfo = new ArenaAudienceLinePointInfo(point);
                arenaAudienceLinePointInfoList.Points.Add(arenaAudienceLinePointInfo);
            }

            string path = Path.Combine(ModCore.DataFolder, FILE_NAME);
            ModJsonUtils.WriteStream(path, arenaAudienceLinePointInfoList);
        }

        public void SetAudiencePlacement(bool overhaul, bool refreshNow)
        {
            AudienceManager audienceManager = AudienceManager.Instance;

            bool endSearch = false;
            foreach (AudienceReactionConfiguration reaction in audienceManager.Reactions)
            {
                if (reaction != null && !reaction.Variants.IsNullOrEmpty())
                {
                    foreach (AudienceMatrixTransformerList variant in reaction.Variants)
                    {
                        if (variant != null && !variant.MatrixTransformerPrefabs.IsNullOrEmpty())
                        {
                            foreach (AudienceMatrixTransformer prefab in variant.MatrixTransformerPrefabs)
                            {
                                if (prefab && prefab is WaveAudienceMatrixTransformer wave)
                                {
                                    wave.WaveStartPositionX = -150f;
                                    wave.WaveEndPositionX = 150f;
                                    endSearch = true;
                                    break;
                                }
                            }
                        }
                        if (endSearch)
                            break;
                    }
                }
                if (endSearch)
                    break;
            }

            AudienceRoot2019 audienceRoot2019 = audienceManager.AudienceRoot;

            if (_vanillaPlacementLines == null)
            {
                _vanillaPlacementLines = audienceRoot2019.NormalPlacementLines;
            }

            if (_vanillaVipPlacementLines == null)
            {
                _vanillaVipPlacementLines = audienceRoot2019.VipPlacementLines;
            }

            if (overhaul)
            {
                audienceRoot2019.NormalPlacementLines = _customPlacementLines.ToArray();
                audienceRoot2019.VipPlacementLines = _customVipPlacementLines.ToArray();
            }
            else
            {
                audienceRoot2019.NormalPlacementLines = _vanillaPlacementLines;
                audienceRoot2019.VipPlacementLines = _vanillaVipPlacementLines;
            }

            if (refreshNow)
            {
                ArenaCustomizationManager.Instance.RefreshArenaAppearance(true);
                return;
            }
            ArenaCustomizationManager.Instance.RefreshArenaAppearanceNextFrame(true);
        }

        public void PatchAudienceRotation()
        {
            AudienceManager audienceManager = AudienceManager.Instance;
            if (audienceManager && !audienceManager._matrixLists.IsNullOrEmpty())
            {
                foreach (AudienceBotMatrixList audienceBotMatrixList in audienceManager._matrixLists)
                {
                    for (int i = 0; i < audienceBotMatrixList.StartMatricies.Length; i++)
                    {
                        Matrix4x4 matrix = audienceBotMatrixList.StartMatricies[i];
                        Vector4 col = matrix.GetColumn(3);
                        Vector3 pos = (Vector3)col;

                        float xForward = -80f;
                        float xBackward = 91f;

                        if (pos.y < 45f && pos.y > 35f)
                        {
                            xForward = -90f;
                            xBackward = 100f;
                        }
                        else if (pos.y < 55f && pos.y > 45f)
                        {
                            xForward = -100f;
                            xBackward = 110f;
                        }

                        if (pos.x < xForward)
                        {
                            Vector3 eulerAngles = matrix.rotation.eulerAngles;
                            eulerAngles.y = 180f;
                            matrix.SetTRS(pos, Quaternion.Euler(eulerAngles), matrix.lossyScale);
                        }
                        if (pos.x > xBackward)
                        {
                            Vector3 eulerAngles = matrix.rotation.eulerAngles;
                            eulerAngles.y = 0f;
                            matrix.SetTRS(pos, Quaternion.Euler(eulerAngles), matrix.lossyScale);
                        }
                        audienceBotMatrixList.StartMatricies[i] = matrix;
                    }
                }
            }
        }
    }
}
