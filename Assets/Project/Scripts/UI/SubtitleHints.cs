// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Handles and hints in game and displays their subttitles when needed
    /// </summary>
    public class SubtitleHints : ActiveStateObserver
    {
        [SerializeField]
        private Transform _subtitlePosition;
        [SerializeField]
        private CharacterSubtitlePreset _preset;
        [SerializeField]
        private List<SubtitleInformation> _hints;
        [SerializeField]
        private float _delayBeforeHints;
        [SerializeField]
        private float _durationBetweenHints;
        [SerializeField]
        private float _delayBetweenLoops;
        [SerializeField]
        private bool _canLoop;
        [SerializeField]
        private bool _resumable;
        [SerializeField]
        private UnityEvent _whenActivated;

        private Coroutine _activeCoroutine;

        protected override void Start()
        {
            base.Start();
            if (Application.isEditor && Application.isPlaying)
            {
                _hints.ForEach(x => LocalizedText.GetSubtitle(x.Hint));
            }
        }

        protected override void HandleActiveStateChanged()
        {
            if (Active && _activeCoroutine == null)
            {
                _activeCoroutine = StartCoroutine(DisplayHint());
                IEnumerator DisplayHint()
                {
                    yield return new WaitForSeconds(_delayBeforeHints);
                    while (_resumable && !Active) yield return null;

                    do
                    {
                        for (int i = 0; i < _hints.Count; i++)
                        {
                            var hint = _hints[i];
                            DisplayActiveHint(hint);

                            yield return new WaitForSeconds(hint.HintDuration + _durationBetweenHints);

                            while (_resumable && !Active)
                            {
                                while (!Active) yield return null;
                                yield return new WaitForSeconds(_delayBeforeHints);
                            }
                        }

                        yield return new WaitForSeconds(_delayBetweenLoops);
                    }
                    while (_canLoop);

                    _activeCoroutine = null;
                }
            }
            else if (!_resumable && _activeCoroutine != null)
            {
                StopCoroutine(_activeCoroutine);
                _activeCoroutine = null;
            }
        }

        private void DisplayActiveHint(SubtitleInformation hint)
        {
            hint.Display(_preset, _subtitlePosition);
            _whenActivated.Invoke();
        }

    }

    [System.Serializable]
    public struct SubtitleInformation
    {
        [SerializeField, TextArea(3, 10)]
        private string _hint;
        public string Hint => _hint;

        [SerializeField]
        private float _hintDuration;
        public float HintDuration => GetDuration();

        [SerializeField, Optional]
        AudioTrigger _audioTrigger;
        public AudioTrigger AudioTrigger => _audioTrigger;

        [SerializeField]
        public UnityEvent WhenActivated;

        public void Display(CharacterSubtitlePreset preset, Transform locator)
        {
            SubtitleManager.Instance.ShowSubtitle(Hint, preset, GetDuration(), locator);

            if (AudioTrigger != null) AudioTrigger.Play();

            WhenActivated.Invoke();
        }

        /// <summary>
        /// If HintDuration is not defined, retuns an estimate based on the AudioTrigger duration and Wordcount
        /// </summary>
        private float GetDuration()
        {
            if (_hintDuration > 0) return _hintDuration;

            float estimate = SubtitleManager.GetDurationEstimate(_hint, 155);
            float audio = AudioTrigger ? AudioTrigger.MaxDuration() : 0;
            return Mathf.Max(estimate, audio);
        }
    }
}
