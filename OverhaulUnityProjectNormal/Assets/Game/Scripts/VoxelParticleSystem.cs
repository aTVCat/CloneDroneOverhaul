using UnityEngine;

namespace PicaVoxel
{
	public class VoxelParticleSystem : MonoBehaviour
	{
		public MinMaxRange ParticleLifetime;
		public int MaxBatchParticles;
		public ParticleSystem System;
		public float BounceMultiplier;
	}
}
