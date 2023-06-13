using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace PicaVoxel
{
	[Serializable]
	public class Volume : MonoBehaviour
	{
		public GameObject FramePrefab;
		public int XChunkSize;
		public int YChunkSize;
		public int ZChunkSize;
		public int XSize;
		public int YSize;
		public int ZSize;
		public float VoxelSize;
		public float OverlapAmount;
		public Vector3 Pivot;
		public int CurrentFrame;
		public List<Frame> Frames;
		public BoxCollider Hitbox;
		public MeshingMode MeshingMode;
		public MeshingMode MeshColliderMeshingMode;
		public bool GenerateMeshColliderSeparately;
		public Material Material;
		public PhysicMaterial PhysicMaterial;
		public bool CollisionTrigger;
		public CollisionMode CollisionMode;
		public float SelfShadingIntensity;
		public ShadowCastingMode CastShadows;
		public bool ReceiveShadows;
		public int ChunkLayer;
		public Color[] PaletteColors;
		public bool RuntimOnlyMesh;
	}
}
