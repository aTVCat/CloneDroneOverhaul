using System;
using UnityEngine;

namespace PicaVoxel
{
	[Serializable]
	public class BasicAnimator : MonoBehaviour
	{
		public float Interval;
		public bool PingPong;
		public bool Loop;
		public bool RandomStartFrame;
		public bool PlayOnAwake;
		public bool IsPlaying;
		public int CurrentFrame;
		public int NumFrames;
	}
}
