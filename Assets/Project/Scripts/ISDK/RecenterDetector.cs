// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Quests recentering callback was not being fired when the user recentered
    /// Probably a OS/OVRPlugin issue
    /// This class manually invokes recenter if it detects the user has rotated more than 45 degrees in a single frame
    /// </summary>
    public class RecenterDetector : MonoBehaviour
    {
        [SerializeField]
        OVRCameraRig _rig;

        Pose _lastPose;
        private static float _lastPauseTime;

        private void Start()
        {
            _lastPose = _rig.centerEyeAnchor.GetPose(Space.Self);
            _lastPauseTime = Time.time;
        }

        void Update()
        {
            var newPose = _rig.centerEyeAnchor.GetPose(Space.Self);

            bool shouldRecenter = Vector3.Angle(_lastPose.forward.SetY(0).normalized, newPose.forward.SetY(0).normalized) > 45; // turned 45 degrees in a single frame! must be a recenter
            shouldRecenter &= Time.time - _lastPauseTime > 1f; // ignore if device has just woken up from sleep
            _lastPose = newPose;
        }

        private void OnApplicationFocus(bool pause)
        {
            _lastPauseTime = Time.time;
        }
    }
}
