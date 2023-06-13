using System;
using UnityEngine;

namespace PicaVoxel
{
	[Serializable]
	public struct Voxel
	{
		public Voxel(byte[] bytes) : this()
		{
		}

		public VoxelState State;
		public byte Value;
		public Color32 Color;
	}
}
