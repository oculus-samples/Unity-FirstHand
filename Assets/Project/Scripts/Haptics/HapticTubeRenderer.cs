// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Haptics;
using Oculus.Interaction.HandGrab;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Displays a tube renderer visual between the controller & kite
    /// </summary>
    public class HapticTubeRenderer : ActiveStateObserver
    {
        private const float _transitionTime = 0.2f;
        [SerializeField]
        TubeRenderer _tubeRenderer;
        [SerializeField]
        private float _catenary = 5;
        [SerializeField, Optional]
        private ReferenceDistanceHapticSource _override;
        [SerializeField]
        private Vector3 _gravityOverride;
        [SerializeField]
        private float _waveTime = 0.2f;
        [SerializeField]
        private float _hapticAmplitude = 0.2f;

        [SerializeField, Interface(typeof(IDistanceInteractor))]
        private MonoBehaviour _distanceInteractor;
        public IDistanceInteractor DistanceInteractor { get; private set; }

        TubePoint[] _tubePoints = new TubePoint[36];
        private IDistanceHapticSource _distanceHapticSource;

        float _lastChangeTime = -1;
        Vector3 _lastEnd = Vector3.zero;

        HapticEvents _hapticEvents = new HapticEvents();

        [SerializeField]
        Grabbable _grabbable;
        InteractionTracker _grabInteractions;

        bool _visible = false;
        TubePoint[] _invisibleTube = new TubePoint[2];

        private void Awake()
        {
            _grabInteractions = new InteractionTracker(_grabbable);

            DistanceInteractor = _distanceInteractor as IDistanceInteractor;

            var distanceGrabInteractor = DistanceInteractor as DistanceGrabInteractor;
            if (distanceGrabInteractor != null)
            {
                distanceGrabInteractor.WhenInteractableSet.Action += SetTarget;
                distanceGrabInteractor.WhenInteractableUnset.Action += ClearTarget;
            }

            var distanceHandGrabInteractor = DistanceInteractor as DistanceHandGrabInteractor;
            if (distanceHandGrabInteractor != null)
            {
                distanceHandGrabInteractor.WhenInteractableSet.Action += SetTarget;
                distanceHandGrabInteractor.WhenInteractableUnset.Action += ClearTarget;
            }

            if (_override.HasValue)
            {
                _distanceHapticSource = _override.HapticSource;
                _distanceHapticSource.WhenHaptics += AddHaptic;
                _lastChangeTime = -1;
            }

            _tubeRenderer.Tint = Color.black;
            _tubeRenderer.RenderTube(_invisibleTube);
        }

        private void ClearTarget(Component _)
        {
            if (_distanceHapticSource != null)
            {
                _distanceHapticSource.WhenHaptics -= AddHaptic;
                _lastEnd = _distanceHapticSource.Position;
                _lastChangeTime = Time.time;
            }
            _distanceHapticSource = null;
        }

        private void AddHaptic()
        {
            var clip = _distanceHapticSource.Clip;
            _hapticEvents.Add(clip, Time.time);

            if (_grabInteractions.TryGetSelectingHand(out var hand))
            {
                HandHaptics.Get(hand).PlayHaptic(clip);
            }
        }

        void SetTarget(Component interactable)
        {
            ClearTarget(default);
            _distanceHapticSource = interactable.GetComponent<IDistanceHapticSource>();
            _lastChangeTime = Time.time;
            if (_distanceHapticSource != null)
            {
                _distanceHapticSource.WhenHaptics += AddHaptic;
            }
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();

            var time = Mathf.Clamp01((Time.time - _lastChangeTime) / _transitionTime);
            Color color;

            if (Active || _override.HasValue)
            {
                color = Color.Lerp(Color.white, Color.white * 0.2f, time);
            }
            else
            {
                color = Color.Lerp(_tubeRenderer.Tint, Color.black, Time.deltaTime * 4);
            }

            _tubeRenderer.Tint = color;

            bool visible = color != Color.black;
            if (visible != _visible)
            {
                _visible = visible;
                if (!visible)
                {
                    _tubeRenderer.Tint = Color.black;
                    _tubeRenderer.RenderTube(_invisibleTube);
                }
            }

            if (_visible) // no need to update the arc if its invisible
            {
                UpdateArc(time);
            }

            // HACK to support autograb
            // DistanceGrabInteractor doesnt clear its candidate if its candidate gets disabled
            if (_distanceInteractor is DistanceGrabInteractor dg)
            {
                if (dg.Candidate && !dg.Candidate.isActiveAndEnabled)
                {
                    dg.SetComputeCandidateOverride(() => null);
                    dg.SetComputeShouldUnselectOverride(() => true);
                    dg.Drive();
                    dg.ClearComputeCandidateOverride();
                    dg.ClearComputeShouldUnselectOverride();
                }
            }
        }

        private void UpdateArc(float time)
        {
            var start = DistanceInteractor.Origin.position;
            var forward = DistanceInteractor.Origin.forward;

            var targetPosition = _lastEnd;

            var hasTarget = _distanceHapticSource != null;
            if (hasTarget)
            {
                targetPosition = _distanceHapticSource.Position;
            }

            if (hasTarget) time = 1 - time;

            var noTargetPosition = start + forward * 0.2f;
            var end = Vector3.Lerp(targetPosition, noTargetPosition, time);

            var line = end - start;
            float distance = line.magnitude;

            int pointCount = _tubePoints.Length;
            var segmentLength = distance / (pointCount - 1);
            _tubePoints[0].position = start;
            _tubePoints[pointCount - 1].position = end;

            Vector3 dir = line.normalized;
            float midPointCatenary = CalculateCatenary(-distance / 2);
            Vector3 gravityDirection = _gravityOverride.sqrMagnitude > 0f ? _gravityOverride : forward;

            Vector3 noiseDirection = forward;
            Vector3.OrthoNormalize(ref line, ref forward, ref noiseDirection);

            for (int i = 1; i < pointCount - 1; ++i)
            {
                Vector3 wirePoint = start + i * segmentLength * dir;
                float x = i * segmentLength - distance / 2;
                wirePoint += gravityDirection * (midPointCatenary - CalculateCatenary(x));

                if (hasTarget)
                {
                    var normalized = (pointCount - i) / (float)pointCount;
                    var timeOffset = normalized * _waveTime;
                    float hapticTime = Time.time - timeOffset;
                    float haptic = GetHapticWave(hapticTime);
                    wirePoint += noiseDirection * haptic;
                }

                _tubePoints[i].position = wirePoint;
            }

            float length = 0;
            _tubePoints[0].rotation = Quaternion.LookRotation(_tubePoints[1].position - _tubePoints[0].position);
            _tubePoints[0].relativeLength = 0;
            for (int i = 1; i < pointCount; i++)
            {
                var pointBefore = _tubePoints[i - 1];
                var point = _tubePoints[i];
                var direction = point.position - pointBefore.position;

                _tubePoints[i].rotation = Quaternion.LookRotation(direction);
                length = _tubePoints[i].relativeLength = length + direction.magnitude;
            }

            for (int i = 0; i < pointCount; i++)
            {
                _tubePoints[i].relativeLength /= length;
                _tubePoints[i].position = _tubeRenderer.transform.InverseTransformPoint(_tubePoints[i].position);
                _tubePoints[i].rotation = Quaternion.Inverse(_tubeRenderer.transform.rotation) * _tubePoints[i].rotation;
            }

            _tubeRenderer.RenderTube(_tubePoints);
        }

        private float GetHapticWave(float hapticTime)
        {
            var amp = _hapticEvents.GetAmpAtTime(hapticTime) * _hapticAmplitude;
            return amp > 0 ? (Mathf.PerlinNoise(hapticTime * 10, 0) * 2 - 1) * amp : 0;
        }

        public float CalculateCatenary(float x)
        {
            return _catenary * CosH(x / _catenary);
        }

        float CosH(float t)
        {
            return (Mathf.Exp(t) + Mathf.Exp(-t)) / 2;
        }

        protected override void HandleActiveStateChanged()
        {

        }

        class HapticEvents
        {
            int _head = 0;

            TimeRange[] _events = new TimeRange[5];

            public void Add(HapticClip clip, float time)
            {
                var duration = clip.GetDuration();
                _events[_head++] = new TimeRange()
                {
                    Start = time,
                    End = time + duration
                };
                if (_head == _events.Length) _head = 0;
            }

            public float GetAmpAtTime(float time)
            {
                for (int i = 0; i < _events.Length; i++)
                {
                    if (_events[i].Contains(time))
                    {
                        var halfLength = _events[i].Duration * 0.5;
                        var mid = _events[i].Start + halfLength;
                        return (float)(Math.Abs(mid - time) / halfLength);
                    }
                }
                return 0;
            }

            public override string ToString()
            {
                return string.Join(", ", _events);
            }
        }

        public override string ToString()
        {
            return _hapticEvents.ToString();
        }

#if UNITY_EDITOR
        [CustomPreview(typeof(HapticTubeRenderer))]
        class Preview : ObjectPreview
        {
            public override GUIContent GetPreviewTitle()
            {
                return new GUIContent("HTR");
            }

            public override void OnPreviewGUI(Rect r, GUIStyle background)
            {
                GUI.Label(r, target.ToString());
            }

            public override bool HasPreviewGUI()
            {
                return true;
            }
        }
#endif
    }
}
