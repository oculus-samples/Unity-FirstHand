// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Returns true when the ConicalFrustum contains the joint
    /// Useful for checking if the players hand is in thier field of view
    /// </summary>
    public class FrustumContainsJointActiveState : MonoBehaviour, IActiveState
    {
        [SerializeField, Interface(typeof(IHand))]
        private MonoBehaviour _hand;
        [SerializeField]
        private ConicalFrustum _frustum;
        [SerializeField]
        private HandJointId _joint;

        public IHand Hand { get; private set; }

        void Awake() => Hand = _hand as IHand;

        public bool Active => Hand.GetJointPose(_joint, out Pose pose) && _frustum.IsPointInConeFrustum(pose.position);
    }
}
