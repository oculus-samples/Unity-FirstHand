// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Returns true if the attached collider contains the main camera
    /// </summary>
    public class ContainsPlayerActiveState : MonoBehaviour, IActiveState
    {
        private static OVRCameraRig _mainCamera;

        [SerializeField]
        ActiveStateExpectation _head = ActiveStateExpectation.True;
        [SerializeField]
        ActiveStateExpectation _hands = ActiveStateExpectation.Any;
        [SerializeField]
        ActiveStateExpectation _rig = ActiveStateExpectation.Any;
        [SerializeField]
        private bool _workIfColliderIsDisabled;

        private Collider _collider;

        public bool Active
        {
            get
            {
                if (!_collider) return false;

                bool wasEnabled = _collider.enabled;
                _collider.enabled |= _workIfColliderIsDisabled;

                if (!_collider.enabled) return false;

                try
                {
                    if (!_mainCamera) _mainCamera = FindObjectOfType<OVRCameraRig>();
                    if (!_mainCamera) return false;

                    if (_head != ActiveStateExpectation.Any)
                    {
                        var playerPos = _mainCamera.centerEyeAnchor.transform.position;
                        if (!_head.Matches(Contains(playerPos))) return false;
                    }

                    if (_rig != ActiveStateExpectation.Any)
                    {
                        var rigPos = _mainCamera.trackingSpace.position;
                        if (!_rig.Matches(Contains(rigPos))) return false;
                    }

                    if (_hands != ActiveStateExpectation.Any)
                    {
                        Vector3 localOffset = new Vector3(-0.1f, 0, 0);
                        bool containsAny =
                            Contains(_mainCamera.leftHandAnchor.TransformPoint(localOffset)) ||
                            Contains(_mainCamera.rightHandAnchor.TransformPoint(localOffset)) ||
                            Contains(_mainCamera.leftControllerAnchor.TransformPoint(localOffset)) ||
                            Contains(_mainCamera.rightControllerAnchor.TransformPoint(localOffset));

                        if (!_hands.Matches(containsAny)) return false;
                    }

                    return true;
                }
                finally
                {
                    _collider.enabled = wasEnabled;
                }
            }
        }

        bool Contains(Vector3 point)
        {
            return _collider.IsPointWithinCollider(point);
        }

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        private void OnDrawGizmosSelected()
        {
            _collider = GetComponent<Collider>();
            if (!Active) return;

            Gizmos.color = Color.yellow;

            switch (_collider)
            {
                case BoxCollider box:
                    Gizmos.matrix = Matrix4x4.TRS(box.transform.position, box.transform.rotation, box.transform.lossyScale);
                    Gizmos.DrawWireCube(box.center, box.size);
                    break;
            }
        }
    }
}
