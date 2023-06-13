using UnityEngine;

namespace PicaVoxel
{
	public class Frame : ManagedBehaviour
	{
		public GameObject ChunkPrefab;
		public Volume ParentVolume;
		public int XSize;
		public int YSize;
		public int ZSize;
		[SerializeField]
		private byte[] bserializedVoxels;
	}
}
