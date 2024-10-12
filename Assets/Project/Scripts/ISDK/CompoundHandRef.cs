// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Interaction.Input;
using System;
using System.Collections.Generic;
using UnityEngine;
using HandDataSource = Oculus.Interaction.Input.IDataSource<Oculus.Interaction.Input.HandDataAsset>;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Wraps several hands (e.g. controller and tracked) and picks the first active one
    /// Used as a 'master' hand, allowing HandRefs to reference a single IHand that could be the controller or tracked hand
    /// </summary>
    public class CompoundHandRef : MonoBehaviour, IHand, HandDataSource
    {
        [SerializeField, Interface(typeof(IHand))]
        private List<MonoBehaviour> _hands;
        private List<IHand> Hands;

        private IHand _bestHand = NullHand.instance;

        private void Awake() => Hands = _hands.ConvertAll(x => x as IHand);

        void Update()
        {
            var newBest = Hands.Find(x => x.IsConnected) ?? NullHand.instance;
            if (newBest != _bestHand)
            {
                _bestHand.WhenHandUpdated -= InvokeWhenHandUpdated;
                if (_bestHand is HandDataSource oldDataSource)
                {
                    oldDataSource.InputDataAvailable -= InvokeInputDataAvilable;
                }

                _bestHand = newBest;

                _bestHand.WhenHandUpdated += InvokeWhenHandUpdated;
                if (_bestHand is HandDataSource newDataSource)
                {
                    newDataSource.InputDataAvailable += InvokeInputDataAvilable;
                }

                //TODO should invoke the callbacks?
            }
        }

        private void InvokeInputDataAvilable()
        {
            _whenInputDataAvailable?.Invoke();
        }

        private void InvokeWhenHandUpdated()
        {
            _whenHandUpdated?.Invoke();
        }

        Action _whenHandUpdated;
        Action _whenInputDataAvailable;

        public event Action WhenHandUpdated
        {
            add { _whenHandUpdated += value; }
            remove { _whenHandUpdated -= value; }
        }

        event Action IDataSource.InputDataAvailable
        {
            add { _whenInputDataAvailable += value; }
            remove { _whenInputDataAvailable -= value; }
        }

        public Handedness Handedness => _bestHand.Handedness;
        public bool IsConnected => _bestHand.IsConnected;
        public bool IsHighConfidence => _bestHand.IsHighConfidence;
        public bool IsDominantHand => _bestHand.IsDominantHand;
        public float Scale => _bestHand.Scale;
        public bool IsPointerPoseValid => _bestHand.IsPointerPoseValid;
        public bool IsTrackedDataValid => _bestHand.IsTrackedDataValid;
        public int CurrentDataVersion => _bestHand.CurrentDataVersion;

        public bool GetFingerIsHighConfidence(HandFinger finger) => _bestHand.GetFingerIsHighConfidence(finger);
        public bool GetFingerIsPinching(HandFinger finger) => _bestHand.GetFingerIsPinching(finger);
        public float GetFingerPinchStrength(HandFinger finger) => _bestHand.GetFingerPinchStrength(finger);
        public bool GetIndexFingerIsPinching() => _bestHand.GetIndexFingerIsPinching();
        public bool GetJointPose(HandJointId handJointId, out Pose pose) => _bestHand.GetJointPose(handJointId, out pose);
        public bool GetJointPoseFromWrist(HandJointId handJointId, out Pose pose) => _bestHand.GetJointPoseFromWrist(handJointId, out pose);
        public bool GetJointPoseLocal(HandJointId handJointId, out Pose pose) => _bestHand.GetJointPoseLocal(handJointId, out pose);
        public bool GetJointPosesFromWrist(out ReadOnlyHandJointPoses jointPosesFromWrist) => _bestHand.GetJointPosesFromWrist(out jointPosesFromWrist);
        public bool GetJointPosesLocal(out ReadOnlyHandJointPoses localJointPoses) => _bestHand.GetJointPosesLocal(out localJointPoses);
        public bool GetPalmPoseLocal(out Pose pose) => _bestHand.GetPalmPoseLocal(out pose);
        public bool GetPointerPose(out Pose pose) => _bestHand.GetPointerPose(out pose);
        public bool GetRootPose(out Pose pose) => _bestHand.GetRootPose(out pose);

        HandDataAsset _invalid = new HandDataAsset();
        HandDataAsset HandDataSource.GetData()
        {
            return _bestHand is HandDataSource dataSource ? dataSource.GetData() : _invalid;
        }

        void IDataSource.MarkInputDataRequiresUpdate()
        {
            if (_bestHand is HandDataSource dataSource)
            {
                dataSource.MarkInputDataRequiresUpdate();
            }
        }
    }

    /// <summary>
    /// IHand that does nothing, can be used to prevent null reference exceptions
    /// </summary>
    public class NullHand : IHand
    {
        public static readonly NullHand instance = new NullHand();
        public Handedness Handedness => Handedness.Left;
        public bool IsConnected => false;
        public bool IsHighConfidence => false;
        public bool IsDominantHand => false;
        public float Scale => 1f;
        public bool IsPointerPoseValid => false;
        public bool IsTrackedDataValid => false;
        public bool IsCenterEyePoseValid => false;
        public Transform TrackingToWorldSpace => null;
        public int CurrentDataVersion => 0;
        public event Action WhenHandUpdated = delegate { };
        public bool GetFingerIsHighConfidence(HandFinger finger) => false;
        public bool GetFingerIsPinching(HandFinger finger) => false;
        public float GetFingerPinchStrength(HandFinger finger) => 0f;
        public bool GetIndexFingerIsPinching() => false;

        public bool GetJointPose(HandJointId handJointId, out Pose pose)
        {
            pose = Pose.identity;
            return false;
        }
        public bool GetJointPoseFromWrist(HandJointId handJointId, out Pose pose)
        {
            pose = Pose.identity;
            return false;
        }
        public bool GetJointPoseLocal(HandJointId handJointId, out Pose pose)
        {
            pose = Pose.identity;
            return false;
        }
        public bool GetJointPosesFromWrist(out ReadOnlyHandJointPoses jointPosesFromWrist)
        {
            jointPosesFromWrist = null;
            return false;
        }
        public bool GetJointPosesLocal(out ReadOnlyHandJointPoses localJointPoses)
        {
            localJointPoses = null;
            return false;
        }
        public bool GetPalmPoseLocal(out Pose pose)
        {
            pose = Pose.identity;
            return false;
        }
        public bool GetPointerPose(out Pose pose)
        {
            pose = Pose.identity;
            return false;
        }
        public bool GetRootPose(out Pose pose)
        {
            pose = Pose.identity;
            return false;
        }
    }
}
