// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Interaction.HandGrab;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class KiteMovementController : MonoBehaviour, IHandGrabUseDelegate
    {
        [SerializeField]
        private float _moveSpeed, _fallSpeed, _rotationSpeed;
        [SerializeField]
        private float _xPosConstraint, _yPosConstraint;
        [SerializeField]
        private GameObject _kite;
        [SerializeField]
        private GetDistanceFromPlayer _leftController, _rightController;

        private bool _canMove = false;

        [SerializeField]
        private bool _controllerInput;
        [SerializeField]
        private AudioTrigger _idleLoop;

        Vector3 _stringOrigin;
        float _turnInput = 0;
        float _angle = 0;

        public float TurnInput => _turnInput;
        public bool MovementEnabled => _canMove;

        void Update()
        {
            UpdateInput();
        }

        private void FixedUpdate()
        {
            UpdatePose();
            MovementAudio();
        }

        private void MovementAudio()
        {
            if (!_canMove) _idleLoop.Play();
            else _idleLoop.Stop();
        }

        private void UpdateInput()
        {
            if (_controllerInput)
            {
                // the center point between the 2 controllers gives us our frame of reference
                Vector3 centerPosition = (_leftController.Position + _rightController.Position) / 2f;
                Vector3 kitePosition = _kite.transform.position;
                Vector3 forward = (kitePosition - centerPosition).normalized;
                float leftDistance = Vector3.Dot(_leftController.Position - centerPosition, forward);
                float rightDistance = Vector3.Dot(_rightController.Position - centerPosition, forward);

                float angle = rightDistance - leftDistance;
                float normalized = Mathf.Clamp(angle / 0.2f, -1, 1);
                _turnInput = normalized;
                _stringOrigin = centerPosition;
            }
            else
            {
                _turnInput = Mathf.Lerp(-1, 1, UnityEngine.Input.mousePosition.x / Screen.width);
                _stringOrigin = Camera.main.transform.position;
            }
        }

        private void UpdatePose()
        {
            var toKite = _kite.transform.position - _stringOrigin;

            if (!_canMove)
            {
                _kite.transform.rotation = Quaternion.LookRotation(-toKite.SetY(0));
                return;
            }

            float maxAngle = 150;
            _angle = Mathf.Lerp(_angle, _turnInput * maxAngle, Time.deltaTime * _rotationSpeed);
            var rotationFromInput = Quaternion.AngleAxis(_angle, Vector3.up);
            var defaultRotation = Quaternion.LookRotation(Vector3.up, toKite);
            var finalRotation = defaultRotation * rotationFromInput;
            _kite.transform.rotation = finalRotation;

            var move = _moveSpeed * Time.deltaTime * _kite.transform.forward;
            var gravity = _fallSpeed * Time.deltaTime * Vector3.down;
            var delta = move + gravity;

            _kite.transform.position += delta;

            ApplyBounds();
        }

        private void ApplyBounds()
        {
            Vector3 position = _kite.transform.localPosition;

            position.x = Mathf.Clamp(position.x, -_xPosConstraint, _xPosConstraint);
            position.y = Mathf.Clamp(position.y, -_yPosConstraint, _yPosConstraint);
            position.z = 0;

            _kite.transform.localPosition = position;
        }

        public void CanMove(bool value)
        {
            _canMove = value;
        }

        public void BeginUse() { }
        public void EndUse() { }
        public float ComputeUseStrength(float strength) { return strength; }

        void OnDrawGizmos()
        {
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(_xPosConstraint * 2, _yPosConstraint * 2));
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(_xPosConstraint * 2 + 5, _yPosConstraint * 2 + 5));
        }
    }
}
