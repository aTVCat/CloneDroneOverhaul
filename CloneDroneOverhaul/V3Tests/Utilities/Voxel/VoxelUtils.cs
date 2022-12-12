using ModLibrary;
using PicaVoxel;
using System.Collections.Generic;
using UnityEngine;

namespace CloneDroneOverhaul.V3Tests.Utilities
{
    public static class VoxelUtils
    {
        private static Material _defMat;

        public static Volume CreateVolume(in string volumeName, in int xSize, in int ySize, in int zSize)
        {
            if (_defMat == null)
            {
                _defMat = new Material(Shader.Find("PicaVoxel/PicaVoxel PBR OneMinus Alpha Emissive"));
            }

            bool prevState = InternalModBot.IgnoreCrashesManager.GetIsIgnoringCrashes();
            InternalModBot.IgnoreCrashesManager.SetIsIgnoringCrashes(true);

            GameObject gameObject = new GameObject(volumeName);

            Volume volume = null;

            GameObject hitBox = new GameObject("Hitbox");
            hitBox.transform.SetParent(gameObject.transform);
            hitBox.AddComponent<BoxCollider>();

            GameObject chunks = new GameObject("Chunks");
            GameObject chunk = new GameObject("Chunk");
            Chunk chunkComp = chunk.AddComponent<Chunk>();
            chunk.AddComponent<MeshRenderer>();
            chunk.AddComponent<MeshFilter>();

            GameObject frame = new GameObject("PicaVoxelFrame");
            Frame frameComp = frame.AddComponent<Frame>();
            frameComp.SetPrivateField<byte[]>("bserializedVoxels", new byte[0]);
            frameComp.ChunkPrefab = chunk;
            chunks.transform.SetParent(frame.transform);

            volume = gameObject.AddComponent<Volume>();
            volume.VoxelSize = 1f;
            volume.XSize = xSize;
            volume.YSize = ySize;
            volume.ZSize = zSize;
            volume.XChunkSize = xSize;
            volume.YChunkSize = ySize;
            volume.ZChunkSize = zSize;
            volume.MeshingMode = MeshingMode.Culled;
            volume.Frames = new List<Frame>();
            volume.CastShadows = UnityEngine.Rendering.ShadowCastingMode.On;
            volume.ChunkLayer = 18;
            volume.CollisionMode = CollisionMode.MeshColliderConvex;
            volume.CollisionTrigger = true;
            volume.CurrentFrame = 0;
            frame.transform.SetParent(volume.transform);
            volume.FramePrefab = frame;
            volume.Material = _defMat;
            volume.enabled = true;
            volume.GenerateBasic(FillMode.AllVoxels);
            volume.CreateChunks();

            frameComp.ParentVolume = volume;

            OverhaulMain.Timer.AddNoArgActionToCompleteNextFrame(delegate
            {
                InternalModBot.IgnoreCrashesManager.SetIsIgnoringCrashes(prevState);
            });

            return volume;
        }

        public static VoxReader.Interfaces.IVoxFile ReadVoxFile(in string path)
        {
            VoxReader.Interfaces.IVoxFile file = VoxReader.VoxReader.Read(path);
            return file;
        }

        public static void ApplyVoxFileToVolume(VoxReader.Interfaces.IVoxFile vox, Volume vol)
        {
            Color[] colors = new Color[vox.Palette.Colors.Length];

            int index = 0;
            foreach (VoxReader.Color col in vox.Palette.Colors)
            {
                Color newCol = new Color(col.R, col.G, col.B, col.A);
                colors[index] = newCol;
                index++;
            }

            vol.PaletteColors = colors;
        }
    }
}
