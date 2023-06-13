using System;
using UnityEngine;

namespace PicaVoxel
{
	[Serializable]
	public class RandomDeformer : MonoBehaviour
	{
		public PicaVoxelBox ConstrainBox;
		public bool ConstrainToBox;
		public bool AddVoxels;
		public float Interval;
		public int NumVoxels;
	}
}
