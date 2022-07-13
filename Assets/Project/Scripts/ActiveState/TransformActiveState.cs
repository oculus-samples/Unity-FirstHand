/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Use of the material below is subject to the terms of the MIT License
 * https://github.com/oculus-samples/Unity-FirstHand/tree/main/Assets/Project/LICENSE.txt
 */

using System;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Becomes Active when a Transform's position/rotation/scale properties fall within the specified ranges
    /// </summary>
    public class TransformActiveState : MonoBehaviour, IActiveState
    {
        public Transform target;
        public Transform space;

        public Vector3Range PositionRange;
        public Vector3Range EulerAngleRange; //TODO better angle handling

        public bool Active
        {
            get
            {
                var pose = GetPose();
                return PositionRange.Contains(pose.position) &&
                    EulerAngleRange.Contains(pose.rotation.eulerAngles);
            }
        }

        private void Reset()
        {
            target = transform;
            space = transform.parent;
        }

        private void Awake()
        {
            target = target ? target : transform;
            space = space ? space : transform.parent;
        }

        public override string ToString()
        {
            var pose = GetPose();
            var nl = Environment.NewLine;
            return $"Active: {Active}{nl}" +
                $"Position: {pose.position} {PositionRange.Contains(pose.position)}{nl}" +
                $"Euler: {pose.rotation.eulerAngles} {EulerAngleRange.Contains(pose.rotation.eulerAngles)}";
        }

        Pose GetPose()
        {
            if (!target) { return Pose.identity; }

            var pose = target.GetPose();
            if (space)
            {
                return new Pose()
                {
                    position = space.InverseTransformPoint(pose.position),
                    rotation = Quaternion.Inverse(space.rotation) * pose.rotation
                };
            }
            else
            {
                return pose;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!target || EulerAngleRange.IsInfinity()) { return; }

            Vector3 min = RemoveInfinity(EulerAngleRange.Min);
            var minRot = (space ? space.rotation : Quaternion.identity) * Quaternion.Euler(min);

            Vector3 max = RemoveInfinity(EulerAngleRange.Max);
            var maxRot = (space ? space.rotation : Quaternion.identity) * Quaternion.Euler(max);

            Gizmos.DrawLine(target.position, target.position + minRot * Vector3.forward);
            Gizmos.DrawLine(target.position, target.position + maxRot * Vector3.forward);

            Vector3 RemoveInfinity(Vector3 v3)
            {
                for (int i = 0; i < 3; i++) if (float.IsInfinity(v3[i])) v3[i] = 0;
                return v3;
            }
        }
    }
}
