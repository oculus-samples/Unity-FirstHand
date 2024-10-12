// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Interaction.Input;
using Oculus.Interaction.PoseDetection;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Handles the functionality for detecting when the users hand is
    /// in the glove, then enabling the joint tracking
    /// </summary>
    public class GloveBehaviour : MonoBehaviour, IActiveState
    {
        [SerializeField, Interface(typeof(IHand))]
        private MonoBehaviour _hand;
        public IHand Hand;

        /// <summary>
        /// Not used, would be for checking the hand has the right pose
        /// </summary>
        [SerializeField]
        ShapeRecognizerActiveState _shapeRecognizer;

        /// <summary>
        /// While not worn, compared to the pose of the hand to determine if it should attach
        /// </summary>
        [SerializeField]
        Transform _wristComparision;

        private List<GloveJointTracker> _joints = new List<GloveJointTracker>();
        private List<Renderer> _renderers = new List<Renderer>();

        private float _angleThreshold = 20;
        private float _positionThreshold = 0.1f;

        [SerializeField]
        Animator[] _recoilAnimators;

        [SerializeField]
        bool _isOpen;
        [SerializeField]
        bool _isWorn;
        [SerializeField]
        ReferenceActiveState _isRenderable;

        bool _wasRenderable = true;

        public bool IsOpen { get => _isOpen; private set => _isOpen = value; }
        public bool IsWorn { get => _isWorn; private set => _isWorn = value; }

        [SerializeField]
        private ReferenceActiveState _activeState;

        public bool Active => _activeState.Active;

        private void Awake()
        {
            Hand = _hand as IHand;

            GetComponentsInChildren(true, _joints);
            GetComponentsInChildren(true, _renderers);

            SetWorn(_isWorn);
            SetOpen(_isOpen);
        }

        private void Update()
        {
            if (IsOpen && !IsWorn)
            {
                WearIfHandInGlove();
            }

            bool isRenderable = _isRenderable;
            if (isRenderable != _wasRenderable)
            {
                _wasRenderable = isRenderable;
                _renderers.ForEach(x => x.enabled = isRenderable);
            }
        }

        private void WearIfHandInGlove()
        {
            Hand.GetJointPose(HandJointId.HandStart, out var pose);
            if (Hand.Handedness == Handedness.Left)
            {
                pose.rotation = pose.rotation * Quaternion.Euler(0, 0, 180);
            }

            if ((pose.position - _wristComparision.position).sqrMagnitude < _positionThreshold &&
                Quaternion.Angle(pose.rotation, _wristComparision.rotation) < _angleThreshold && Active)
            {
                SetWorn(true);
            }
        }

        public void SetOpen(bool value)
        {
            IsOpen = value;
        }

        public void SetWorn(bool value)
        {
            IsWorn = value;
            if (IsWorn)
            {
                SetOpen(false);
                transform.SetParent(null);
            }
            _joints.ForEach(x => x.enabled = IsWorn);
            //_handVisual.ForceOffVisibility = IsWorn;
        }

        public void Recoil()
        {
            for (int i = 0; i < _recoilAnimators.Length; i++)
            {
                _recoilAnimators[i].SetTrigger("Recoil");
            }
        }
    }
}
