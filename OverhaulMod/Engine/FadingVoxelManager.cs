using PicaVoxel;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Engine
{
    public class FadingVoxelManager : Singleton<FadingVoxelManager>
    {
        private List<FadingVoxel> m_voxelsToFade;

        public float multiplier
        {
            get;
            set;
        } = 0.8f;

        public float timeToDestroyOffset
        {
            get;
            set;
        } = 1f;

        private void Start()
        {
            m_voxelsToFade = new List<FadingVoxel>();
        }

        private void Update()
        {
            if(ModTime.hasFixedUpdated && ModTime.fixedFrameCount % 10 == 0)
            {
                UpdateFading();
            }
        }

        public void AddFadingVoxel(PicaVoxelPoint picaVoxelPoint, MechBodyPart mechBodyPart, float timeToDestroy)
        {
            if (m_voxelsToFade.Count > 750)
                return;

            FadingVoxel fadingVoxel = new FadingVoxel()
            {
                Point = picaVoxelPoint,
                BodyPart = mechBodyPart,
                TimeToDestroy = timeToDestroy
            };
            m_voxelsToFade.Add(fadingVoxel);
        }

        public void UpdateFading()
        {
            if (m_voxelsToFade == null || m_voxelsToFade.Count == 0)
                return;

            int index = 0;
            List<int> indicesToRemove = new List<int>();
            foreach (FadingVoxel fadingVoxel in m_voxelsToFade)
            {
                if(fadingVoxel.TimeToDestroy <= Time.time)
                {
                    indicesToRemove.Add(index);
                    index++;
                    continue;
                }

                MechBodyPart mechBodyPart = fadingVoxel.BodyPart;
                if (mechBodyPart)
                {
                    Frame frame = mechBodyPart._currentFrame;
                    if (frame)
                    {
                        Voxel? voxelAtArrayPosition = frame.GetVoxelAtArrayPosition(fadingVoxel.Point);
                        if(voxelAtArrayPosition != null)
                        {
                            Voxel voxel = voxelAtArrayPosition.Value;
                            if (voxel.Active)
                            {
                                Color color = voxel.Color;
                                color *= multiplier;
                                voxel.Color = color;
                                frame.SetVoxelAtArrayPosition(fadingVoxel.Point, voxel);
                            }
                        }
                    }
                }
                index++;
            }

            if(indicesToRemove.Count != 0)
            {
                int i = indicesToRemove.Count-1;
                do
                {
                    m_voxelsToFade.RemoveAt(indicesToRemove[i]);
                    i--;
                } while (i > -1);
            }
        }
    }
}
