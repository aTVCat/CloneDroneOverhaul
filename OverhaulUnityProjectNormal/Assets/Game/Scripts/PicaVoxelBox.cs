using System;

namespace PicaVoxel
{
	[Serializable]
	public class PicaVoxelBox
	{
		public PicaVoxelBox(PicaVoxelPoint corner1, PicaVoxelPoint corner2)
		{
		}

		public PicaVoxelPoint BottomLeftFront;
		public PicaVoxelPoint TopRightBack;
	}
}
