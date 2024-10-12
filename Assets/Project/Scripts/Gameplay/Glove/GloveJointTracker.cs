// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Interaction.Input;
using UnityEngine;
using UnityEngine.Profiling;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Updates the position of this transform to the position provided by the IHand for the specified joint.
    /// If an offsetSource is provided it will used to generate a pose to offset the joint pose by.
    ///
    /// Use the offsetSource when working with a custom hand whose bones do not align with the Oculus hand;
    /// bring the oculus hand into the prefab, align the custom hand with the oculus hand and assign the
    /// appropriate joint transform as the offset source.
    /// </summary>
    public class GloveJointTracker : MonoBehaviour
    {
        [SerializeField, Interface(typeof(IHand))]
        private MonoBehaviour _hand;
        public IHand Hand;

        [SerializeField]
        private Track _source;

        [SerializeField, Tooltip("The distance to drag behind the hand, if the distance " +
            "between this and the joint is less than this this transform wont move")]
        private float _dragDistance = 0;

        private Pose _offset;

        private void Awake()
        {
            Hand = _hand as IHand;
            _offset = _source.GetOffset(transform);
        }

        private void Update()
        {
            UpdatePose();
        }

        public void UpdatePose()
        {
            Profiler.BeginSample("GloveJointTracker.GetJointPose");
            if (!Hand.GetJointPose(_source.joint, out var pose))
            {
                //transform.position = Vector3.down * 1000;
                Profiler.EndSample();
                return;
            }
            Profiler.EndSample();

            if (Hand.Handedness == Handedness.Left)
            {
                pose.rotation = pose.rotation * Quaternion.Euler(0, 0, 180);
            }
            var offsetPose = _offset.GetTransformedBy(pose);

            if (_dragDistance > 0)
            {
                offsetPose.position = Vector3.MoveTowards(offsetPose.position, transform.position, _dragDistance);
            }

            transform.SetPose(offsetPose);

        }

        public void InjectHand(IHand hand)
        {
            _hand = hand as MonoBehaviour;
            Hand = hand;
        }

        [System.Serializable]
        struct Track
        {
            public HandJointId joint;
            [Optional]
            public Transform offsetSource;

            public Pose GetOffset(Transform forTransform)
            {
                return offsetSource ? offsetSource.Delta(forTransform) : Pose.identity;
            }
        }
    }
}
