// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Haptics;
using Oculus.Interaction.HandGrab;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Handles the rails around the haptics scene to move the necessary
    /// objects around the level
    /// </summary>
    public class RailTrackHandler : MonoBehaviour
    {
        [SerializeField] private ReferenceActiveState _active;
        [SerializeField] private Transform _railPodTransform;
        [SerializeField] private PlayableDirector _timeline;
        [SerializeField, Optional] private string _clipName;
        [SerializeField] private bool IsAutoMove;
        [SerializeField] private ProgressTracker _tracker;
        [SerializeField] private int _progressToSet;
        [SerializeField] Grabbable _grabbable;
        [SerializeField] private bool _reverseOnRelease;
        [SerializeField] private bool _forceReleaseOnCompletion;
        [SerializeField] private ReferenceActiveState _startCompleted = ReferenceActiveState.Optional();
        [SerializeField] private bool _autoMoveDuringRuntime;
        [SerializeField] private float _speed = 1;
        [SerializeField] private float _smoothDamp = -1;

        [Header("Haptics")]
        [SerializeField, Optional] private DistanceHapticSource _hapticSource;
        [SerializeField, Optional] private HapticClip _completeClip;
        [SerializeField, Optional] private HapticClip _dragClip;
        [SerializeField] private UnityEvent _whenHaptics;

        private Vector3[] _points;
        private TimeRange _range;
        InteractionTracker _interactionTracker;

        double _targetTime = 0;
        Vector2 _velocity = Vector2.zero;

        AudioTrigger _audioTrigger;
        HapticClipPlayer _hapticPlayer;
        Controller? _hapticsHand;

        void Start()
        {
            // interaction tracker helps with getting a reference to the distance grab
            _interactionTracker = new InteractionTracker(_grabbable);
            _interactionTracker.WhenChanged += UpdateHandedness;

            _range = string.IsNullOrEmpty(_clipName) ? new TimeRange(0, _timeline.playableAsset.GetDurationFast(), "") : MarkerTrack.GetTimeRange(_timeline, _clipName);
            if (!IsAutoMove)
            {
                //The elements in _positionArray represent a path across the timeline
                _points = new Vector3[100];
                for (int i = 0; i < _points.Length; i++)
                {
                    _timeline.time = _range.Start + i / (_points.Length - 1f) * _range.Duration;
                    _timeline.Evaluate();
                    _points[i] = _railPodTransform.position;

                }
                _timeline.time = 0;
                if (_startCompleted.HasReference && _startCompleted)
                {
                    _timeline.time = _range.End;
                    _tracker.SetProgress(_progressToSet);
                }
                _timeline.Evaluate();

                if (_dragClip)
                {
                    _hapticPlayer = new HapticClipPlayer(_dragClip);
                    _hapticPlayer.isLooping = true;
                    _hapticPlayer.amplitude = 0f;
                }
            }

            if (IsAutoMove)
            {
                _targetTime = _range.End;
            }
        }

        private void UpdateHandedness()
        {
            Controller? hand = null;
            var found = _interactionTracker.TryGetSelectingInteractor<DistanceGrabInteractor>(out var grabber);
            if (found)
            {
                Debug.Log("Found DistanceGrabInteractor");
                var interactorGrabbable = grabber.GetComponentInParent<Grabbable>();
                var handGrabInteractables = interactorGrabbable.GetComponentsInChildren<HandGrabInteractable>();

                for (int i = 0; i < handGrabInteractables.Length; i++)
                {
                    var grab = handGrabInteractables[i];
                    if (grab.SelectingInteractors.Count <= 0 || grab.HandGrabPoses.Count <= 0 || !grab.UsesHandPose) continue;

                    hand = grab.HandGrabPoses[0].HandPose.Handedness == Oculus.Interaction.Input.Handedness.Left ? Controller.Left : Controller.Right;
                    Debug.Log("Found Handedness");
                    break;
                }

                Debug.Log("No Handedness");
            }

            if (hand != _hapticsHand)
            {
                _hapticPlayer.StopLoop();
                _hapticsHand = hand;
            }
        }

        void Update()
        {
            if (!_active || Time.timeScale == 0) return;

            UpdateIsAutoMove(_autoMoveDuringRuntime);

            if (!IsAutoMove)
            {
                // TryGetSelectingInteractor returns true if an interactor of the type is selecting
                if (_interactionTracker.TryGetSelectingInteractor<IDistanceInteractor>(out var distanceGrab))
                {
                    Ray ray = GetRayFromDistanceGrab(distanceGrab);

                    float bestDistance = float.MaxValue;
                    int bestIndex = -1;

                    for (int i = 0; i < _points.Length - 1; i++)
                    {
                        LineSegment line = new LineSegment(_points[i], _points[i + 1]);

                        float distanceToRay;
                        if (!line.IsDivergent(ray)) distanceToRay = line.GetDistanceToRay(ray);
                        else distanceToRay = line.GetDistanceToPoint(ray.GetPoint(100));

                        if (distanceToRay < bestDistance)
                        {
                            bestDistance = distanceToRay;
                            bestIndex = i;
                        }
                    }

                    LineSegment bestSegment = new LineSegment(_points[bestIndex], _points[bestIndex + 1]);

                    Vector3 pointOnRay = bestSegment.IsDivergent(ray) ? ray.GetPoint(100) : bestSegment.GetClosestPointOnRay(ray);
                    Vector3 pointOnLine = bestSegment.GetClosestPointOnLineUnbounded(pointOnRay);

                    // inverse lerp gives the value for t in Vector3.Lerp(_start, _end, t)
                    // used to know how far along the segment the point is
                    float t = bestSegment.InverseLerp(pointOnLine);

                    // the sample indicies are evenly spaced in time
                    // the index of the line and how far along it is can be used to
                    // get a normalized value across all the samples
                    float normalizedTime = (bestIndex + t) / (_points.Length - 1);

                    // TimeRange.Lerp just lerps from the range's start to end
                    _targetTime = _range.Lerp(normalizedTime);
                }
                else
                {
                    _targetTime = _timeline.time;
                    _grabbable.transform.position = _railPodTransform.position;

                    if (_reverseOnRelease)
                    {
                        _targetTime -= Time.deltaTime;
                    }
                }
            }

            if (IsAutoMove) { _targetTime = _range.End; }
            _targetTime = MoveTowards(_timeline.time, _targetTime, Time.deltaTime * _speed);
            double newTime = Math.Clamp(_targetTime, Math.Max(_range.Start, 0), Math.Min(_range.End, _timeline.playableAsset.GetDurationFast()));

            if (_hapticsHand.HasValue && _dragClip)
            {
                var deltaTime = Mathf.Clamp01(Mathf.Abs((float)(newTime - _timeline.time)));

                _hapticPlayer.PlayLoopWithAmplitudeAndFrequency(_hapticsHand.Value, deltaTime * 100, deltaTime * 500);
                Debug.Log($"Playing with {deltaTime * 50}");
            }

            if (newTime != _timeline.time)
            {
                bool evaluate = Math.Abs(newTime - _timeline.time) > 0.00001f;
                _timeline.time = newTime;

                if (evaluate)
                {
                    _timeline.DeferredEvaluate();
                }
            }

            if (Math.Abs(_timeline.time - _range.End) <= 0.0001f && _tracker.Progress < _progressToSet)
            {
                if (_hapticSource && _completeClip)
                {
                    _hapticSource.Clip = _completeClip;
                    _hapticSource.Play();
                }
                //clip completed, updating progress
                _tracker.SetProgress(_progressToSet);

                if (_forceReleaseOnCompletion)
                {
                    _interactionTracker.ForceUnselectAll();
                }

            }
        }

        private double MoveTowards(double curentTime, double targetTime, float maxDelta)
        {
            if (_smoothDamp > 0)
            {
                // Mathf.SmoothDamp doesnt support negative, so need to use Vector2 version
                var time2 = new Vector2((float)curentTime, 0);
                var target2 = new Vector2((float)targetTime, 0);
                return Vector2.SmoothDamp(time2, target2, ref _velocity, _smoothDamp, 4).x;
            }
            else
            {
                double delta = targetTime - curentTime;
                bool tooFar = Math.Abs(delta) > maxDelta;
                return tooFar ? curentTime + Math.Sign(delta) * maxDelta : targetTime;
            }
        }

        public Ray GetRayFromDistanceGrab(IDistanceInteractor distanceGrab)
        {
            // distanceGrab.Origin is the origin of the selection cone
            var origin = distanceGrab.Origin.position;
            // taking the direction from the grabbable accounts for offsets
            var direction = _grabbable.transform.position - origin;
            return new Ray(origin, direction);
        }

        public void UpdateIsAutoMove(bool value)
        {
            if (!value) return;

            if (_interactionTracker.TimesSelected > 0 && !_interactionTracker.IsSelected())
            {
                IsAutoMove = true;
                _targetTime = _range.End;
            }
        }
    }
}
