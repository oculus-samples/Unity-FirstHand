// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.Playables;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Tracks if the players hands are performing the door gestures within a zone
    /// </summary>
    public class GesturesWithinTriggerArea : MonoBehaviour
    {
        [SerializeField]
        private ReferenceActiveState _leftHandActive, _rightHandActive;
        public ReferenceActiveState LeftHandActive => _leftHandActive;
        public ReferenceActiveState RightHandActive => _rightHandActive;

        [SerializeField]
        private ReferenceActiveState _leftHandInZone, _rightHandInZone;
        public ReferenceActiveState LeftHandInZone => _leftHandInZone;
        public ReferenceActiveState RightHandInZone => _rightHandInZone;

        [Header("References")]
        [SerializeField]
        private ReferenceActiveState _checkForGestures;
        [SerializeField]
        private ProgressTracker _progressTrackerRef;
        [SerializeField]
        private int _progressAfterGestures = 330;
        [SerializeField]
        private GameObject _knockArea;

        [SerializeField] PlayableDirector _revertDirector;
        [SerializeField] int _revertProgress;
        [SerializeField] float _revertTime;
        [SerializeField] ReferenceActiveState _revertState;

        private void Update()
        {
            if (!_checkForGestures) return;
            if (TweenRunner.IsTweening(this)) return;
            if (!_leftHandInZone || !_rightHandInZone) return;

            if (_revertState)
            {
                _revertDirector.playableGraph.GetRootPlayable(0).SetDuration(_revertDirector.playableAsset.duration);
                _revertDirector.playableGraph.GetRootPlayable(0).SetSpeed(0);
                _revertDirector.time = _revertTime;
                _progressTrackerRef.SetProgress(_revertProgress, true);
                TweenRunner.Tween(_revertTime, _revertDirector.playableAsset.duration, _revertDirector.playableAsset.duration - _revertTime,
                    x =>
                    {
                        _revertDirector.time = x;
                        _revertDirector.Evaluate();
                    }).SetUpdate(Tween.UpdateTime.LateUpdate)
                    .OnComplete(() =>
                    {
                        _revertDirector.time = 0;
                        _revertDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
                        _revertDirector.Evaluate();
                        _revertDirector.Play();
                    })
                    .SetID(this);
            }

            if (!_leftHandActive || !_rightHandActive) return;

            _progressTrackerRef.SetProgress(_progressAfterGestures);
            _knockArea.SetActive(false);
        }
    }
}
