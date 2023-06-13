using UnityEngine;

namespace PicaVoxel
{
	public class Exploder : MonoBehaviour
	{
		public enum ExplodeTargets
		{
			All = 0,
			AllButSelf = 1,
			SelfOnly = 2,
		}

		public enum ExplodeValueFilterOperation
		{
			LessThan = 0,
			LessThanOrEqualTo = 1,
			EqualTo = 2,
			GreaterThanOrEqualTo = 3,
			GreaterThan = 4,
		}

		public string Tag;
		public float ExplosionRadius;
		public float ParticleVelocity;
		public ExplodeTargets ExplodeTarget;
		public ExplodeValueFilterOperation ValueFilterOperation;
		public int ValueFilter;
	}
}
