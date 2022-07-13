/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Use of the material below is subject to the terms of the MIT License
 * https://github.com/oculus-samples/Unity-FirstHand/tree/main/Assets/Project/LICENSE.txt
 */

using Oculus.Interaction.Input;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Wraps several hands (e.g. controller and tracked) and picks the first active one
    /// Used as a 'master' hand, allowing HandRefs to reference a single IHand that could be the controller or tracked hand
    /// </summary>
    public class CompoundHandRef : MonoBehaviour, IHand
    {
        [SerializeField, Interface(typeof(IHand))]
        private List<MonoBehaviour> _hands;
        private List<IHand> Hands;

        [SerializeField]
        private Component[] _aspects = new Component[0];

        private void Awake() => Hands = _hands.ConvertAll(x => x as IHand);
        private IHand BestHand => Hands.Find(x => x.IsConnected) ?? NullHand.instance;

        public bool GetHandAspect<TComponent>(out TComponent foundComponent) where TComponent : class
        {
            for (int i = 0; i < _aspects.Length; i++)
            {
                foundComponent = _aspects[i] as TComponent;
                if (foundComponent != null)
                {
                    return true;
                }
            }

            if (BestHand.GetHandAspect(out foundComponent))
            {
                return true;
            }

            for (int i = 0; i < Hands.Count; i++)
            {
                if (Hands[i] != BestHand && Hands[i].GetHandAspect(out foundComponent))
                {
                    return true;
                }
            }

            return false;
        }

        public event Action WhenHandUpdated
        {
            add { BestHand.WhenHandUpdated += value; }
            remove { BestHand.WhenHandUpdated -= value; }
        }

        public Handedness Handedness => BestHand.Handedness;
        public bool IsConnected => BestHand.IsConnected;
        public bool IsHighConfidence => BestHand.IsHighConfidence;
        public bool IsDominantHand => BestHand.IsDominantHand;
        public float Scale => BestHand.Scale;
        public bool IsPointerPoseValid => BestHand.IsPointerPoseValid;
        public bool IsTrackedDataValid => BestHand.IsTrackedDataValid;
        public bool IsCenterEyePoseValid => BestHand.IsCenterEyePoseValid;
        public Transform TrackingToWorldSpace => BestHand.TrackingToWorldSpace;
        public int CurrentDataVersion => BestHand.CurrentDataVersion;
        public bool GetCenterEyePose(out Pose pose) => BestHand.GetCenterEyePose(out pose);
        public bool GetFingerIsHighConfidence(HandFinger finger) => BestHand.GetFingerIsHighConfidence(finger);
        public bool GetFingerIsPinching(HandFinger finger) => BestHand.GetFingerIsPinching(finger);
        public float GetFingerPinchStrength(HandFinger finger) => BestHand.GetFingerPinchStrength(finger);
        public bool GetIndexFingerIsPinching() => BestHand.GetIndexFingerIsPinching();
        public bool GetJointPose(HandJointId handJointId, out Pose pose) => BestHand.GetJointPose(handJointId, out pose);
        public bool GetJointPoseFromWrist(HandJointId handJointId, out Pose pose) => BestHand.GetJointPoseFromWrist(handJointId, out pose);
        public bool GetJointPoseLocal(HandJointId handJointId, out Pose pose) => BestHand.GetJointPoseLocal(handJointId, out pose);
        public bool GetJointPosesFromWrist(out ReadOnlyHandJointPoses jointPosesFromWrist) => BestHand.GetJointPosesFromWrist(out jointPosesFromWrist);
        public bool GetJointPosesLocal(out ReadOnlyHandJointPoses localJointPoses) => BestHand.GetJointPosesLocal(out localJointPoses);
        public bool GetPalmPoseLocal(out Pose pose) => BestHand.GetPalmPoseLocal(out pose);
        public bool GetPointerPose(out Pose pose) => BestHand.GetPointerPose(out pose);
        public bool GetRootPose(out Pose pose) => BestHand.GetRootPose(out pose);
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

        public bool GetCenterEyePose(out Pose pose)
        {
            pose = Pose.identity;
            return false;
        }
        public bool GetHandAspect<TComponent>(out TComponent foundComponent) where TComponent : class
        {
            foundComponent = null;
            return false;
        }
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
