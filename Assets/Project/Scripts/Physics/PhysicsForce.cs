// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Applies a physics force to object with a set of settings
    /// </summary>
    public class PhysicsForce : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody _rigidbody;
        [SerializeField]
        private bool _loop;
        [SerializeField]
        private float _duration = 1;
        [SerializeField]
        private Vector3Range _force = Vector3Range.Zero;
        [SerializeField]
        private Vector3Range _torque = Vector3Range.Zero;
        [SerializeField]
        private List<Burst> _bursts = new List<Burst>();
        [SerializeField]
        private Space _space;
        public ForceMode Mode;

        float _startTime;

        private void OnEnable()
        {
            _startTime = Time.time;
        }

        private void FixedUpdate()
        {
            var localTime = Time.time - _startTime;
            if (!_loop && localTime > _duration) return;

            AddForceAndTorque(_rigidbody, _force.Random(), _torque.Random(), _space, Mode);

            var loopTime = localTime % _duration;
            _bursts.ForEach(x => x.Apply(loopTime, _rigidbody));
        }

        [Serializable]
        struct Burst
        {
            public float Time;
            public Vector3 Force;
            public Vector3 Torque;
            public float Duration;
            public Space Space;
            public ForceMode Mode;

            public void Apply(float time, Rigidbody rigidbody)
            {
                if (time >= Time && time < Time + Duration)
                {
                    AddForceAndTorque(rigidbody, Force, Torque, Space, Mode);
                }
            }
        }

        private static void AddForceAndTorque(Rigidbody rigidbody, Vector3 force, Vector3 torque, Space space, ForceMode mode)
        {
            if (space == Space.World)
            {
                rigidbody.AddForce(force, mode);
                rigidbody.AddTorque(torque, mode);
            }
            else
            {
                rigidbody.AddRelativeForce(force, mode);
                rigidbody.AddRelativeTorque(torque, mode);
            }
        }
    }
}
