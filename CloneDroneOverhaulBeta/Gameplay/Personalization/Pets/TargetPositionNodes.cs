using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CDOverhaul.Gameplay.Pets
{
    public static class TargetPositionNodes
    {
        public const string Offset = "TOffset";

        public const string OwnerTransformRight = "owner.TRight";
        public const string OwnerTransformLeft = "owner.TLeft";
        public const string OwnerTransformUp = "owner.TUp";
        public const string OwnerTransformDown = "owner.TDown";
        public const string OwnerTransformForward = "owner.TForward";
        public const string OwnerTransformBackward = "owner.TBackward";

        public static Vector3 GetVector(Tuple<string, Vector3>[] nodes, FirstPersonMover firstPersonMover)
        {
            if (nodes.IsNullOrEmpty() || !firstPersonMover)
                return Vector3.zero;

            Transform transform = firstPersonMover.transform;
            Vector3 result = Vector3.zero;
            foreach(Tuple<string, Vector3> tuple in nodes)
            {
                if(tuple.Item1 == Offset)
                    result += tuple.Item2;

                if (tuple.Item1 == OwnerTransformRight)
                    result += transform.right * tuple.Item2.x;
                if (tuple.Item1 == OwnerTransformLeft)
                    result -= transform.right * tuple.Item2.x;

                if (tuple.Item1 == OwnerTransformForward)
                    result += transform.forward * tuple.Item2.x;
                if (tuple.Item1 == OwnerTransformBackward)
                    result -= transform.forward * tuple.Item2.x;

                if (tuple.Item1 == OwnerTransformUp)
                    result += transform.up * tuple.Item2.x;
                if (tuple.Item1 == OwnerTransformDown)
                    result -= transform.up * tuple.Item2.x;
            }
            return result;
        }
    }
}
