using CloneDroneOverhaul.PooledPrefabs;
using ModLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CloneDroneOverhaul.Modules
{
    public class VoxelEffectsModule : ModuleBase
    {
        private List<CutVoxel> CuttingVoxels = new List<CutVoxel>();
        private List<BurntVoxel> BurntVoxels = new List<BurntVoxel>();
        private int UpdatesToNextDisconnect = 3;
        private bool isCalculatingVoxels;

        public override bool ShouldWork()
        {
            return true;
        }

        public override void OnManagedUpdate()
        {
            return;
            if (!isCalculatingVoxels)
            {
                StaticCoroutineRunner.StartStaticCoroutine(OnUpdateMeThread());
            }
        }

        public IEnumerator OnUpdateMeThread()
        {
            isCalculatingVoxels = true;
            int voxelsPast = 0;
            for (int i = CuttingVoxels.Count - 1; i > -1; i--)
            {
                if (i > CuttingVoxels.Count - 1)
                {
                    isCalculatingVoxels = false;
                    yield break;
                }
                voxelsPast++;
                if (voxelsPast >= 100)
                {
                    yield return new WaitForEndOfFrame();
                    voxelsPast = 0;
                }
                CutVoxel cut = CuttingVoxels[i];
                if (cut.BodyPart != null && cut.Voxel != null && Time.time < cut.TimeToDispose)
                {
                    bool canManage = true;
                    foreach (VoxelBeingDestroyed voxelToDestroy in cut.BodyPart.GetPrivateField<Dictionary<int, VoxelBeingDestroyed>>("_voxelsToDestroy").Values.ToList<VoxelBeingDestroyed>())
                    {
                        if (IsSamePoint(voxelToDestroy.VoxelPoint, cut.Voxel))
                        {
                            canManage = false;
                            break;
                        }
                    }

                    if (canManage)
                    {
                        MechBodyPart mainPart = cut.BodyPart;
                        PicaVoxel.PicaVoxelPoint mainPoint = cut.Voxel;
                        PicaVoxel.Voxel? mainVoxel = cut.BodyPart.GetMyVolume.GetVoxelAtWorldPosition(cut.BodyPart.GetMyVolume.GetCurrentFrame().GetVoxelWorldPosition(cut.Voxel));

                        Color32 voxColor = mainVoxel.Value.Color;

                        float r = voxColor.r;
                        float g = voxColor.g;
                        float b = voxColor.b;
                        float a = voxColor.a;
                        float colorLength = r + g + b;

                        if (colorLength > 4)
                        {
                            if (colorLength > 60 && UnityEngine.Random.Range(0, 5) > 3)
                            {
                                //CutFireSparksWorseVFX.SpawnObject(cut.BodyPart.GetMyVolume.GetCurrentFrame().GetVoxelWorldPosition(cut.Voxel), Vector3.zero, Color.clear);
                            }

                            float num = UnityEngine.Random.Range(0.9520f, 0.9580f);
                            float num2 = num - 0.4000f;
                            if (colorLength > 110)
                            {
                                voxColor = new Color32((byte)(r * num), (byte)(g * num), (byte)(b * num), (byte)a);
                            }
                            else
                            {
                                voxColor = new Color32((byte)(r * num2), (byte)(g * num2), (byte)(b * num2), (byte)a);
                            }
                            cut.BodyPart.GetMyVolume.GetCurrentFrame().SetVoxelAtArrayPosition(cut.Voxel, new PicaVoxel.Voxel
                            {
                                Color = voxColor,
                                State = mainVoxel.Value.State,
                                Value = 1
                            });
                        }
                        else
                        {
                            BurnVoxelsArround(mainPoint, mainPart);
                            SetVoxelAsBurnt(mainPoint, mainPart);
                            RemoveVoxel(i);
                        }

                    }
                }
                else
                {
                    RemoveVoxel(i);
                }
            }

            UpdatesToNextDisconnect--;
            if (UpdatesToNextDisconnect == 0)
            {
                UpdatesToNextDisconnect = 3;
                if (BurntVoxels.Count > 0)
                {
                    foreach (BurntVoxel burntVox in BurntVoxels)
                    {
                        if (burntVox.BodyPart != null && burntVox.Points != null)
                        {
                            burntVox.BodyPart.DisconnectSegmentsAfterCut(burntVox.Points, burntVox.BodyPart.GetLastDamageByAttackID(), Vector3.zero, false, null, DamageSourceType.None);
                        }
                    }
                    BurntVoxels.Clear();
                }
            }
            isCalculatingVoxels = false;
            yield break;
        }

        public void AddNewCutVoxel(PicaVoxel.PicaVoxelPoint voxel, MechBodyPart bodyPart, bool setOnFireAround)
        {
            CutVoxel newCutVoxel = new CutVoxel
            {
                Voxel = voxel,
                BodyPart = bodyPart,
                TimeToDispose = Time.time + 7.3f
            };

            if (!IsDuplicateOfCuttingVoxel(newCutVoxel))
            {
                CuttingVoxels.Add(newCutVoxel);
                if (UnityEngine.Random.Range(0, 10) > 8)
                {
                    Vector3 vector = newCutVoxel.BodyPart.GetMyVolume.GetCurrentFrame().GetVoxelWorldPosition(newCutVoxel.Voxel);
                    //Transform trans = FireSmokeVFX.SpawnObject(vector, new Vector3(270, 0, 0), Color.clear, newCutVoxel.BodyPart.transform, true);
                    //if (trans != null)
                    //{
                    //    newCutVoxel.MyVFX = trans.GetComponent<ParticleSystem>();
                    //    if (newCutVoxel.MyVFX != null)
                    //    {
                    //        newCutVoxel.MyVFX.Play();
                    //    }
                    //}
                }
            }
            else
            {
                return;
            }

            bodyPart.GetMyVolume.GetCurrentFrame().SetVoxelAtArrayPosition(voxel, new PicaVoxel.Voxel
            {
                Color = AttackManager.Instance.BodyOnFireColor,
                State = PicaVoxel.VoxelState.Active,
                Value = 1
            });
        }

        private bool IsDuplicateOfCuttingVoxel(CutVoxel cVox)
        {
            for (int i = CuttingVoxels.Count - 1; i > -1; i--)
            {
                CutVoxel cut = CuttingVoxels[i];
                PicaVoxel.PicaVoxelPoint point = cut.Voxel;
                if (IsSamePoint(cVox.Voxel, point) && cVox.BodyPart == cut.BodyPart)
                {
                    return true;
                }
            }
            return false;
        }

        public void RemoveVoxel(int index)
        {
            CutVoxel voxell = CuttingVoxels[index];
            StopVFX(voxell);
            PicaVoxel.PicaVoxelPoint point = voxell.Voxel;
            PicaVoxel.Frame frame = null;
            if (point == null || voxell.BodyPart == null || voxell.BodyPart.GetMyVolume == null || voxell.BodyPart.GetMyVolume.GetCurrentFrame() == null)
            {
                CuttingVoxels.RemoveAt(index);
                return;
            }
            frame = voxell.BodyPart.GetMyVolume.GetCurrentFrame();

            Vector3 vector = frame.GetVoxelWorldPosition(point);
            Vector3 vector2 = vector.normalized;
            if (UnityEngine.Random.Range(0, 5) > 3)
            {
                PicaVoxel.VoxelParticleSystem.Instance.SpawnSingle(CuttingVoxels[index].BodyPart.GetMyVolume.GetVoxelWorldPosition(CuttingVoxels[index].Voxel.X, CuttingVoxels[index].Voxel.Y, CuttingVoxels[index].Voxel.Z), new Color(0.02f, 0.02f, 0.02f, 1f), CuttingVoxels[index].BodyPart.GetVoxelSize() * 0.75f, 1f * vector2);
            }
            CuttingVoxels[index].BodyPart.GetMyVolume.GetCurrentFrame().SetVoxelAtArrayPosition(point, new PicaVoxel.Voxel
            {
                State = PicaVoxel.VoxelState.Hidden,
                Value = 1
            });

            CuttingVoxels.RemoveAt(index);
            return;
        }

        private void StopVFX(CutVoxel i)
        {
            if (i.MyVFX != null)
            {
                i.MyVFX.Stop();
                SimplePooledPrefabInstance ppinst = i.MyVFX.GetComponent<SimplePooledPrefabInstance>();
                if (ppinst != null)
                {
                    ppinst.ReturnToPool(true);
                }
            }
        }

        private void SetVoxelAsBurnt(PicaVoxel.PicaVoxelPoint point, MechBodyPart part)
        {
            bool foundEntry = false;
            foreach (BurntVoxel burnt in BurntVoxels)
            {
                if (burnt.BodyPart == part)
                {
                    burnt.Points.Add(point);
                    foundEntry = true;
                }
            }
            if (!foundEntry && (part != null || point == null))
            {
                BurntVoxel burntVox = new BurntVoxel
                {
                    BodyPart = part,
                    Points = new List<PicaVoxel.PicaVoxelPoint>()
                };
                burntVox.Points.Add(point);
                BurntVoxels.Add(burntVox);
            }
        }

        public bool CannotHideVoxelNow(PicaVoxel.PicaVoxelPoint pPoint, MechBodyPart part)
        {
            if (GameModeManager.IsMultiplayer())
            {
                return false;
            }
            for (int i = 0; i < CuttingVoxels.Count; i++)
            {
                PicaVoxel.PicaVoxelPoint point = CuttingVoxels[i].Voxel;
                if (IsSamePoint(pPoint, point) && part == CuttingVoxels[i].BodyPart)
                {
                    return true;
                }
            }
            return false;
        }

        private void BurnVoxelsArround(PicaVoxel.PicaVoxelPoint voxel, MechBodyPart bodyPart)
        {
            BurnVoxelAt(bodyPart, voxel, 1, 0, 0);
            BurnVoxelAt(bodyPart, voxel, -1, 0, 0);
            BurnVoxelAt(bodyPart, voxel, 0, 1, 0);
            BurnVoxelAt(bodyPart, voxel, 0, -1, 0);
            BurnVoxelAt(bodyPart, voxel, 0, 0, 1);
            BurnVoxelAt(bodyPart, voxel, 0, 0, -1);
            BurnVoxelAt(bodyPart, voxel, 2, 0, 0);
            BurnVoxelAt(bodyPart, voxel, -2, 0, 0);
            BurnVoxelAt(bodyPart, voxel, 0, 2, 0);
            BurnVoxelAt(bodyPart, voxel, 0, -2, 0);
            BurnVoxelAt(bodyPart, voxel, 0, 0, 2);
            BurnVoxelAt(bodyPart, voxel, 0, 0, -2);
        }

        private void BurnVoxelAt(MechBodyPart bodyPart, PicaVoxel.PicaVoxelPoint voxel, int xOff, int yOff, int zOff)
        {
            PicaVoxel.PicaVoxelPoint newPoint = new PicaVoxel.PicaVoxelPoint(voxel.X + xOff, voxel.Y + yOff, voxel.Z + zOff); // More *phantom* fire
            PicaVoxel.Voxel? vox = bodyPart.GetMyVolume.GetCurrentFrame().GetVoxelAtArrayPosition(newPoint);
            if (vox != null && !CannotHideVoxelNow(newPoint, bodyPart))
            {
                float num = UnityEngine.Random.Range(0.50f, 0.66f);
                Color32 voxColor = new Color32((byte)(vox.Value.Color.r * num), (byte)(vox.Value.Color.g * num), (byte)(vox.Value.Color.b * num), vox.Value.Color.a);
                bodyPart.GetMyVolume.SetVoxelAtArrayPosition(newPoint, new PicaVoxel.Voxel
                {
                    Color = voxColor,
                    State = vox.Value.State,
                    Value = vox.Value.Value
                });
            }
        }

        private bool IsSamePoint(PicaVoxel.PicaVoxelPoint voxel1, PicaVoxel.PicaVoxelPoint voxel2)
        {
            return voxel1.X == voxel2.X && voxel1.Y == voxel2.Y && voxel1.Z == voxel2.Z;
        }

        private bool IsDuplicateOfCuttingVoxel(PicaVoxel.PicaVoxelPoint voxel, MechBodyPart part)
        {
            for (int i = 0; i < CuttingVoxels.Count; i++)
            {
                PicaVoxel.PicaVoxelPoint point = CuttingVoxels[i].Voxel;
                if (IsSamePoint(voxel, point) && part == CuttingVoxels[i].BodyPart)
                {
                    return true;
                }
            }
            return false;
        }

        public void RemoveAllRefrencesOfBodyPart(MechBodyPart part)
        {
            for (int i = CuttingVoxels.Count - 1; i > 0; i--)
            {
                if (CuttingVoxels[i].BodyPart == part)
                {
                    CuttingVoxels.RemoveAt(i);
                }
            }
        }

        private struct BurntVoxel : IDisposable
        {

            public List<PicaVoxel.PicaVoxelPoint> Points;
            public MechBodyPart BodyPart;

            public void Dispose()
            {

            }

        }


        private struct CutVoxel : IDisposable
        {

            public PicaVoxel.PicaVoxelPoint Voxel;
            public MechBodyPart BodyPart;
            public ParticleSystem MyVFX;
            public float TimeToDispose;

            public float ColorMagnitude
            {
                get
                {
                    PicaVoxel.Voxel? vox = BodyPart.GetMyVolume.GetCurrentFrame().GetVoxelAtArrayPosition(Voxel);
                    return vox.Value.Color.r + vox.Value.Color.g + vox.Value.Color.b;
                }
            }

            public Color32 Color
            {
                get
                {
                    Color32 orig = BodyPart.GetMyVolume.GetCurrentFrame().GetVoxelAtArrayPosition(Voxel).Value.Color;
                    Color32 col = new Color32(orig.r, orig.g, orig.b, 255);
                    return col;
                }
            }

            public void Dispose()
            {

            }

        }
    }
}
