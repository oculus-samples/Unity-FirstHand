// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Interaction.HandGrab;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class BlimpController : MonoBehaviour, IHandGrabUseDelegate
    {
        [SerializeField]
        private Transform _leftThumb, _rightThumb;
        [SerializeField]
        private HandGrabInteractable _leftHandInteractable, _rightHandInteractable;
        [SerializeField]
        private Grabbable _grabbable;
        [SerializeField]
        private float _rotationSpeed, _moveSpeed;
        [SerializeField]
        private float _rotationSmoothing;
        [SerializeField]
        private ReferenceActiveState _isControllerActive;

        private Transform _activeThumb;

        [Header("Blimp")]
        [SerializeField]
        private GameObject _blimp;

        private Rigidbody _blimpRB;
        private bool _canMove;
        private Vector2 _activeControllerInput;

        private void Start()
        {
            _grabbable.WhenPointerEventRaised += UpdateActiveHand;
            _blimpRB = _blimp.GetComponent<Rigidbody>();
        }

        private void UpdateActiveHand(PointerEvent _)
        {
            var isRightHand = _rightHandInteractable.SelectingInteractors.Count > 0;
            _activeThumb = isRightHand ? _rightThumb : _leftThumb;
            _activeControllerInput = isRightHand ? OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick) : OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        }

        private void Update()
        {
            Gyroscope();

            if (_grabbable.SelectingPoints.Count == 0)
                return;

            if (!_isControllerActive.Active)
                BlimpHandRotation();
            else
                BlimoControllerRotation();

            MoveBlimp();
        }

        private void BlimoControllerRotation()
        {
            _blimpRB.AddTorque(_activeControllerInput.x * _rotationSpeed * Time.deltaTime * _blimp.transform.up);
            _blimpRB.AddTorque(_activeControllerInput.y * _rotationSpeed * Time.deltaTime * _blimp.transform.forward);
        }

        private void BlimpHandRotation()
        {
            Vector3 localSpace = transform.InverseTransformPoint(_activeThumb.position);
            var xValue = Mathf.Abs(localSpace.x);
            var yValue = Mathf.Abs(localSpace.z);

            if (xValue > 0.01f)
                _blimpRB.AddTorque(_rotationSpeed * localSpace.x * Time.deltaTime * _blimp.transform.up);
            if (yValue > 0.01f)
                _blimpRB.AddTorque(_rotationSpeed * localSpace.z * Time.deltaTime * _blimp.transform.right);
        }

        private void Gyroscope()
        {
            var targetRotaiton = Quaternion.LookRotation(_blimp.transform.forward.SetY(0), Vector3.up);
            targetRotaiton = Quaternion.Lerp(_blimp.transform.rotation, targetRotaiton, 1 - Mathf.Pow(_rotationSmoothing, Time.deltaTime));
            _blimpRB.MoveRotation(targetRotaiton);
        }

        private void MoveBlimp()
        {
            if (!_canMove)
                return;

            _blimpRB.velocity = _moveSpeed * Time.deltaTime * -_blimp.transform.right;
        }

        public void BeginUse() { _canMove = true; }
        public void EndUse() { _canMove = false; }
        public float ComputeUseStrength(float strength) { return strength; }
    }
}
