// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Explode called elsewhere, but applies an explosive force to the attached object
    /// </summary>
    public class ExplodeWithForce : MonoBehaviour
    {
        [SerializeField]
        private FloatRange _radius = new FloatRange(1, 2);

        [SerializeField]
        private FloatRange _force = new FloatRange(1, 2);

        [SerializeField]
        private FloatRange _upwardsModifier = new FloatRange(0, 0);

        [SerializeField]
        private bool _withTorque = false;

        public void Explode()
        {
            ExplosiveForce.Explode(transform.position, _radius.Random(), _force.Random(), _upwardsModifier.Random(), _withTorque);
        }
    }
}
