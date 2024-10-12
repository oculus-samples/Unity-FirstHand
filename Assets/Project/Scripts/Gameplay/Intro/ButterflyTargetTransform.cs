// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class ButterflyTargetTransform : MonoBehaviour, IButterflyTarget
    {
        [SerializeField] ReferenceActiveState _active;

        public Pose Pose => GetPose();

        private Pose GetPose()
        {
            if (radius == 0)
            {
                return transform.GetPose();
            }
            else
            {
                var pose = new Pose(transform.position, Quaternion.LookRotation(transform.forward));
                pose.position += pose.up * radius;
                return pose;
            }
        }

        public bool Active => !_active.HasReference || _active;

        public float radius;

        public bool Occupied { get; set; }

        void OnEnable() => ButterflyController.Targets.Add(this);
        void OnDisable() => ButterflyController.Targets.Remove(this);
    }
}
