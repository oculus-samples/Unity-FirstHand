// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Interaction.HandGrab;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class RockCrush : MonoBehaviour, IHandGrabUseDelegate
    {
        [SerializeField]
        private AnimationCurve _crushIntensity;
        [SerializeField]
        private float _fragility = 1f;
        [SerializeField]
        private float _recovery = 1f;

        [SerializeField]
        private HandGrabInteractable _leftRockGrabs;
        [SerializeField]
        private HandGrabInteractable _rightRockGrabs;
        [SerializeField]
        private HandGrabInteractable _leftCrystalGrab;
        [SerializeField]
        private HandGrabInteractable _rightCrystalGrab;

        [SerializeField]
        AudioIntensity _pressureAudio;
        [SerializeField]
        TimelineIntensity _pressureTimeline;
        [SerializeField]
        TimelineIntensity _intensityTimeline;
        [SerializeField]
        PlayableDirector _breakTimeline;
        [SerializeField]
        GameObject _crystalInteractables;
        [SerializeField]
        PositionConstraint _crystalConstraint;
        [SerializeField]
        private GameObject _gloveDropZone;
        [SerializeField]
        private GameObject _rockInteractables;

        private float _lastTime;
        private bool _fired = false;

        private float _progress;

        public void BeginUse()
        {
            _lastTime = Time.timeSinceLevelLoad;
            _progress = 0f;
            _fired = false;
            _pressureAudio.StartAudio();
        }

        public float ComputeUseStrength(float strength)
        {
            float timeDelta = (Time.timeSinceLevelLoad - _lastTime);

            // paused?
            if (Mathf.Approximately(timeDelta, 0)) return 0;

            _lastTime = Time.timeSinceLevelLoad;
            strength = _crushIntensity.Evaluate(strength);
            if (strength >= _progress)
            {
                _progress = Mathf.Min(strength, _progress + _fragility * timeDelta);
            }
            else
            {
                _progress = Mathf.Max(strength, _progress - _recovery * timeDelta);
            }

            _pressureAudio.Intensity = _progress;
            _pressureTimeline.Intensity = _progress;
            _intensityTimeline.Intensity = _progress;

            if (_progress >= 1f && !_fired)
            {
                _fired = true;

                var isRightHand = _rightRockGrabs.SelectingInteractors.Count > 0;
                var interactable = isRightHand ? _rightCrystalGrab : _leftCrystalGrab;
                var interactor = (isRightHand ? _rightRockGrabs : _leftRockGrabs).SelectingInteractors.FirstOrDefault();

                _crystalConstraint.enabled = false;
                _breakTimeline.Play();
                _gloveDropZone.SetActive(true);
                _pressureAudio.EndAudio();
                _rockInteractables.SetActive(false);

                StartCoroutine(ForceGrabCrystal());
                IEnumerator ForceGrabCrystal()
                {
                    _crystalInteractables.SetActive(true);
                    yield return null; // need to wait _grabPoseFinder to be set up
                    InteractorExtensions.ForceSelect(interactor, interactable); // regrab it
                }
            }
            return _progress;
        }

        public void EndUse()
        {
            _progress = 0f;
            _pressureAudio.Intensity = _progress;
            _pressureTimeline.Intensity = _progress;
            _intensityTimeline.Intensity = _progress;
        }

        public void StartCrushed()
        {
            _crystalConstraint.enabled = false;
            _breakTimeline.Play();
            _gloveDropZone.SetActive(true);
            _rockInteractables.SetActive(false);
            _crystalInteractables.SetActive(true);
        }
    }
}
