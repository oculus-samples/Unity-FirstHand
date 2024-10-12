// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Interaction.Input;
using Oculus.Interaction.Locomotion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class LocomotionTutorial : MonoBehaviour
    {
        [SerializeField, Interface(typeof(ILocomotionEventHandler))]
        private MonoBehaviour _locomotionHandler;
        private ILocomotionEventHandler LocomotionHandler;

        [SerializeField, Interface(typeof(IActiveState))]
        private MonoBehaviour _rotateActiveState;
        private IActiveState RotateActiveState;

        [SerializeField]
        private Animator _teleportAnimator, _teleportGhostAnimator;
        [SerializeField]
        private Animator _rotateAnimator, _rotateGhostAnimator;

        [SerializeField]
        private VirtualActiveState _isTeachingTeleport, _isTeachingTurn;

        [SerializeField]
        private AudioTrigger _audio;

        [Header("Controller")]
        [SerializeField]
        private Animator _controllerTeleportAnimator;
        [SerializeField]
        private Animator _controllerRotateAnimator;

        [SerializeField, Interface(typeof(IActiveState))]
        private MonoBehaviour _isUsingController, _leftControllerInView, _rightControllerInView;
        private IActiveState IsUsingController, LeftControllerInView, RightControllerInView;

        [Header("Flipping")]
        [SerializeField]
        private GloveJointTracker _gloveJointTracker;

        [SerializeField, Interface(typeof(IActiveState))]
        private MonoBehaviour _isLeftTeleport, _isRightTeleport, _isLeftRotate, _isRightRotate;
        private IActiveState IsLeftTeleport, IsRightTeleport, IsLeftRotate, IsRightRotate;

        [SerializeField, Interface(typeof(IHand))]
        private MonoBehaviour _leftHand, _rightHand;
        private IHand LeftHand, RightHand;

        [SerializeField]
        private List<Transform> _flippable = new List<Transform>();

        [SerializeField]
        ProgressTracker _teleportProgress;
        [SerializeField]
        ProgressTracker _rotateProgress;

        private Handedness _handedness = Handedness.Right;
        private int _progressAnimatorID = Animator.StringToHash("Progress");
        private int _showAnimatorID = Animator.StringToHash("Show");
        private int _isLeftID = Animator.StringToHash("IsLeft");

        protected virtual void Awake()
        {
            LocomotionHandler = _locomotionHandler as ILocomotionEventHandler;
            RotateActiveState = _rotateActiveState as IActiveState;
            LeftHand = _leftHand as IHand;
            RightHand = _rightHand as IHand;
            IsLeftTeleport = _isLeftTeleport as IActiveState;
            IsRightTeleport = _isRightTeleport as IActiveState;
            IsLeftRotate = _isLeftRotate as IActiveState;
            IsRightRotate = _isRightRotate as IActiveState;

            IsUsingController = _isUsingController as IActiveState;
            LeftControllerInView = _leftControllerInView as IActiveState;
            RightControllerInView = _rightControllerInView as IActiveState;
        }

        public void SetHand(Handedness handedness)
        {
            if (_handedness == handedness) return;
            _handedness = handedness;

            bool isLeft = handedness == Handedness.Left;

            _flippable.ForEach(t =>
            {
                var scale = t.localScale;
                scale.x = Mathf.Abs(scale.x) * (isLeft ? -1 : 1);
                t.localScale = scale;
            });

            _gloveJointTracker.enabled = false;
            _gloveJointTracker.InjectHand(isLeft ? LeftHand : RightHand);
            _gloveJointTracker.enabled = true;

            _rotateGhostAnimator.SetBool(_isLeftID, isLeft);
        }

        void OnEnable()
        {
            StartCoroutine(Routine());
        }

        private void Update()
        {
            bool isRight = _isTeachingTeleport.Active && IsRightTeleport.Active || _isTeachingTurn.Active && IsRightRotate.Active;
            bool isLeft = _isTeachingTeleport.Active && IsLeftTeleport.Active || _isTeachingTurn.Active && IsLeftRotate.Active;
            bool isControllerLeft = LeftControllerInView.Active;
            bool isControllerRight = RightControllerInView.Active;

            var leftHand = IsUsingController.Active ? isControllerLeft : isLeft;
            var rightHand = IsUsingController.Active ? isControllerRight : isRight;
            // no hand is more dominant so do nothing
            if (leftHand == rightHand) return;

            SetHand(leftHand ? Handedness.Left : Handedness.Right);
        }

        IEnumerator Routine()
        {
            _teleportGhostAnimator.gameObject.SetActive(false);
            _rotateGhostAnimator.gameObject.SetActive(false);

            yield return new WaitForSeconds(1f);

            yield return StartCoroutine(TeleportRoutine());
            IEnumerator TeleportRoutine()
            {
                _isTeachingTeleport.Active = true;

                _teleportGhostAnimator.gameObject.SetActive(true);
                _teleportGhostAnimator.SetBool(_showAnimatorID, true);


                for (int progress = 1; progress < 4; progress++)
                {
                    _teleportAnimator.SetInteger(_progressAnimatorID, progress);
                    _controllerTeleportAnimator.SetInteger(_progressAnimatorID, progress);
                    yield return new WaitForTeleportEvent(LocomotionHandler);
                    _audio.PlayAudio();
                    _teleportProgress.SetProgress(_teleportProgress.Progress + 1);
                }

                _teleportAnimator.SetInteger(_progressAnimatorID, 4);
                _controllerTeleportAnimator.SetInteger(_progressAnimatorID, 4);
                _isTeachingTeleport.Active = false;

                _teleportGhostAnimator.SetBool(_showAnimatorID, false);
                yield return new WaitForSeconds(2.5f);
                _teleportGhostAnimator.gameObject.SetActive(false);
            }

            yield return StartCoroutine(RotateRoutine());
            IEnumerator RotateRoutine()
            {
                _isTeachingTurn.Active = true;

                _rotateGhostAnimator.gameObject.SetActive(true);
                _rotateGhostAnimator.SetBool(_showAnimatorID, true);
                SetAnimatorProgress(1);

                yield return new WaitUntil(() => RotateActiveState.Active);

                _audio.PlayAudio();

                SetAnimatorProgress(2);
                _rotateProgress.SetProgress(_rotateProgress.Progress + 1);
                yield return new WaitForRotateEvent(LocomotionHandler, -1);
                _audio.PlayAudio();

                SetAnimatorProgress(3);
                _rotateProgress.SetProgress(_rotateProgress.Progress + 1);
                yield return new WaitForRotateEvent(LocomotionHandler, 1);
                _audio.PlayAudio();

                _isTeachingTurn.Active = false;

                SetAnimatorProgress(4);
                _rotateProgress.SetProgress(_rotateProgress.Progress + 1);
                _rotateGhostAnimator.SetBool(_showAnimatorID, false);

                yield return new WaitForSeconds(1f);
                _rotateGhostAnimator.gameObject.SetActive(false);

                void SetAnimatorProgress(int progress)
                {
                    _rotateAnimator.SetInteger(_progressAnimatorID, progress);
                    _controllerRotateAnimator.SetInteger(_progressAnimatorID, progress);
                    _rotateGhostAnimator.SetInteger(_progressAnimatorID, progress);
                }
            }
        }

        class WaitForRotateEvent : WaitForLocomotionEvent
        {
            private int _direction;

            public WaitForRotateEvent(ILocomotionEventHandler locomotor, int direction) : base(locomotor)
            {
                _direction = direction;
            }

            protected override bool IsEventMatch(LocomotionEvent evnt)
            {
                if (evnt.Rotation <= LocomotionEvent.RotationType.None) return false;
                var angle = Vector3.SignedAngle(Vector3.forward, evnt.Pose.forward, Vector3.up);
                return Mathf.Sign(angle) == _direction;
            }
        }

        class WaitForTeleportEvent : WaitForLocomotionEvent
        {
            public WaitForTeleportEvent(ILocomotionEventHandler locomotor) : base(locomotor) { }

            protected override bool IsEventMatch(LocomotionEvent evnt)
            {
                return evnt.Translation > LocomotionEvent.TranslationType.None;
            }
        }

        abstract class WaitForLocomotionEvent : CustomYieldInstruction
        {
            ILocomotionEventHandler _locomotor;
            bool _keepWaiting = true;

            public WaitForLocomotionEvent(ILocomotionEventHandler locomotor)
            {
                _locomotor = locomotor;
                _locomotor.WhenLocomotionEventHandled += HandleEvent;
            }

            private void HandleEvent(LocomotionEvent arg1, Pose arg2)
            {
                if (!IsEventMatch(arg1)) return;

                _locomotor.WhenLocomotionEventHandled -= HandleEvent;
                _keepWaiting = false;
            }

            protected abstract bool IsEventMatch(LocomotionEvent evnt);

            public override bool keepWaiting => _keepWaiting;
        }
    }
}
