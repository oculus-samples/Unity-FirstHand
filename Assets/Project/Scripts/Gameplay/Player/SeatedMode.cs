// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Interaction.Locomotion;
using System.Collections;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Mirrors the OVRRigs position when it locomotes, then offsets it vertically if seated mode is enabled
    /// </summary>
    public class SeatedMode : MonoBehaviour
    {
        private const string PLAYERPREFS_KEY = "settings.seated_mode";

        [SerializeField]
        private OVRCameraRig _rig;
        [SerializeField]
        private PlayerLocomotor _locomotor;
        [SerializeField]
        private Transform _averageEyeLevel; //assumed to be a child
        [SerializeField]
        private float _seatedEyeHeight = 1.63f;

        private static bool? _isOn;
        public static bool IsOn
        {
            get
            {
                if (!_isOn.HasValue) _isOn = Store.GetInt(PLAYERPREFS_KEY) > 0;
                return _isOn.Value;
            }
        }

        private IEnumerator Start()
        {
            yield return null; //takes 2 frame for the camera to get its position
            yield return null;
            _locomotor.WhenLocomotionEventHandled += SyncPosition;
            SyncPosition();
            UpdateCameraRigHeight();
        }

        private void Update()
        {
            var eyePose = PoseUtils.Delta(_rig.transform, _rig.centerEyeAnchor);
            eyePose.position = eyePose.position.SetY(_seatedEyeHeight);
            _averageEyeLevel.SetPose(eyePose, Space.Self);
        }

        private void OnDestroy()
        {
            _locomotor.WhenLocomotionEventHandled -= SyncPosition;
        }

        private void SyncPosition(LocomotionEvent locomotion, Pose _)
        {
            if (locomotion.IsTeleport())
            {
                SyncPosition();
            }
            else if (locomotion.IsSnapTurn())
            {
                transform.rotation = _rig.transform.rotation;
            }
        }

        private void SyncPosition()
        {
            transform.SetPose(_rig.transform.GetPose());
            UpdateCameraRigHeight();
        }

        private void UpdateCameraRigHeight()
        {
            if (IsOn)
            {
                float playerHeight = _rig.centerEyeAnchor.position.y - _rig.transform.position.y;
                float diff = Mathf.Max(_seatedEyeHeight - playerHeight, 0);
                _rig.transform.position = transform.position + Vector3.up * diff;
            }
            else
            {
                _rig.transform.SetPose(transform.GetPose());
            }
        }

        public static void SetSeatedMode(bool value)
        {
            _isOn = value;
            Store.SetInt(PLAYERPREFS_KEY, value ? 1 : 0);

            var instances = FindObjectsOfType<SeatedMode>();
            for (int i = 0; i < instances.Length; i++)
            {
                instances[i].UpdateCameraRigHeight();
            }
        }
    }
}
