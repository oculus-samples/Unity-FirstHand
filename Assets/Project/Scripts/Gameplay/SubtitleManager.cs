// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class SubtitleManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject _prefab;
        [SerializeField]
        private TextMeshProUGUI _text;
        [SerializeField]
        private Graphic _characterIcon;
        [SerializeField]
        private Rectangle _iconBackground, _textBackground, _iconBorder, _textBox;
        [SerializeField]
        private UICanvas _uiCanvas;
        [SerializeField]
        private float _typingSpeed = 0.001f;
        [SerializeField]
        private float _lookAwayDelay = 0.5f;
        [SerializeField]
        private Vector3 _offset;
        [SerializeField]
        private bool _log;
        [SerializeField]
        private bool _lockSubtitlesToFront;
        [SerializeField]
        private float _occludedAlpha = 0.3f;
        [SerializeField]
        private float _unoccludedAlpha = 0.8f;
        [SerializeField, Tooltip("When true a ray will be cast to check if the target is visible, otherwise just FOV will be used")]
        private bool _physicsOcclusion = true;

        private float _scale = 0.0005f;
        private Transform _head;
        private Transform _subtitlePosition;

        private bool _isOccluded;

        private float _occlusionPositionWeight = 0;

        static readonly char[] _dontEndWithThese = new char[] { ' ' };

        public Vector3 OccludedPosition => HeadPosition + Vector3.ClampMagnitude(Head.forward - Head.up * 0.3f, 0.65f);
        public Vector3 HeadPosition
        {
            get
            {
                return Head.position;
            }
        }

        public Transform Head
        {
            get
            {
                if (_head == null || _head == transform)
                {
                    var cam = Camera.main;
                    if (cam) _head = cam.transform;
                    else _head = transform;
                }
                return _head;
            }
        }

        private Coroutine _activeCoroutine;

        private static SubtitleManager _instance;

        public static SubtitleManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<SubtitleManager>();
                }
                return _instance;
            }
        }

        private void LateUpdate()
        {
            if (string.IsNullOrEmpty(_text.text))
            {
                _prefab.transform.localScale = Vector3.one * 0.0001f;
                return;
            }

            UpdateIsOccluded();
            UpdatePoseAndScale();

            bool somethingBetween = IsThereSomethingBetweenTheSubtitleAndTheCamera();
            var targetAlpha = somethingBetween ? _occludedAlpha : _unoccludedAlpha;
        }

        private void UpdatePoseAndScale()
        {
            var position = GetSubtitlePosition();
            var toPosition = position - HeadPosition;
            var rotation = Quaternion.LookRotation(toPosition);
            var scale = toPosition.magnitude * _scale * Vector3.one;
            _prefab.transform.SetPositionAndRotation(position, rotation);
            _prefab.transform.localScale = scale;
        }

        private Vector3 GetSubtitlePosition()
        {
            if (_occlusionPositionWeight >= 1 || !_subtitlePosition) return OccludedPosition;
            return Vector3.Lerp(_subtitlePosition.position + _offset, OccludedPosition, _occlusionPositionWeight);
        }

        public void ShowSubtitle(string subtitleContent, CharacterSubtitlePreset preset, float duration, Transform locator)
        {
            subtitleContent = LocalizedText.GetSubtitle(subtitleContent).TrimEnd(_dontEndWithThese);

            if (subtitleContent == _text.text) return;

            if (_log)
            {
                Debug.Log($"SUBTITLE {preset?.name}: \"{subtitleContent}\" ({duration})");
            }
            _subtitlePosition = locator;

            UpdateIsOccluded(true);
            UpdatePoseAndScale();

            _text.text = subtitleContent;

            if (preset)
            {
                preset.UpdateActiveSubtitle(_characterIcon, _iconBackground, _textBackground,
                    _iconBorder, _textBox);
            }
            _uiCanvas.Show(true);

            if (_activeCoroutine != null) StopCoroutine(_activeCoroutine);
            _activeCoroutine = StartCoroutine(TypewriterText(duration));
        }

        private IEnumerator TypewriterText(float duration)
        {
            var timeBetweenCharacters = new WaitForSeconds(_typingSpeed);
            int characterCount = _text.text.Length;

            for (int i = 0; i <= characterCount; i++)
            {
                _text.maxVisibleCharacters = i;
                yield return timeBetweenCharacters;
            }

            yield return new WaitForSeconds(duration - _typingSpeed * characterCount);
            _uiCanvas.Show(false);

            yield return new WaitForSeconds(_uiCanvas.Duration);
            _text.text = "";
            _activeCoroutine = null;
        }

        private void UpdateIsOccluded(bool instant = false)
        {
            bool isOccluded = _lockSubtitlesToFront || !_subtitlePosition || IsOccluded(_subtitlePosition);
            if (_isOccluded == isOccluded && !instant) return;

            _isOccluded = isOccluded;

            var target = _isOccluded ? 1 : 0;
            var duration = Mathf.Abs(_occlusionPositionWeight - target) * 0.3f;

            TweenRunner.Tween(_occlusionPositionWeight, target, duration, x => _occlusionPositionWeight = x)
                .Delay(_lookAwayDelay)
                .SetID(this)
                .SetEase(Tween.Ease.QuartInOut)
                .Skip(instant);
        }

        private bool IsOccluded(Transform point, float fov = 60)
        {
            var offsetPoint = point.position + _offset;
            Ray RayFromCamera = new Ray(Head.position, Head.forward);
            var isInView = ConeUtils.RayWithinCone(RayFromCamera, offsetPoint, fov / 2);

            if (!isInView) return true;

            float distance = Vector3.Distance(Head.position, point.position);
            float maxDistance = 6;

            if (distance > maxDistance) return true;

            if (_physicsOcclusion)
            {
                if (Physics.Linecast(Head.position, point.position, out var hit, ~(1 << LayerMask.NameToLayer("physics.knockable")), QueryTriggerInteraction.Ignore))
                {
                    return !HitSubtitleTarget(hit);
                }
            }

            return false;
        }

        private bool IsThereSomethingBetweenTheSubtitleAndTheCamera()
        {
            if (!_physicsOcclusion) return false;

            var direction = _prefab.transform.position - Head.position;
            if (Physics.SphereCast(Head.position, 0.25f / 2, direction, out var hit, direction.magnitude, ~(1 << LayerMask.NameToLayer("physics.knockable")), QueryTriggerInteraction.Ignore))
            {
                if (!HitSubtitleTarget(hit))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns true if the thing we hit is the thing we're trying to display subtitles on
        /// </summary>
        private bool HitSubtitleTarget(RaycastHit hit)
        {
            if (!_subtitlePosition) return false;
            if (!hit.collider.attachedRigidbody) return false;
            return _subtitlePosition.IsChildOf(hit.collider.attachedRigidbody.transform);
        }

        public static float GetDurationEstimate(string subtitle, int wordsPerMinute)
        {
            return GetWordCount(subtitle) / (wordsPerMinute / 60f);
        }

        public static int GetWordCount(string subtitle)
        {
            if (string.IsNullOrWhiteSpace(subtitle)) return 0;

            var wasWord = IsWord(subtitle[0]);
            var wordCount = wasWord ? 1 : 0;

            for (int i = 1; i < subtitle.Length; i++)
            {
                var isWord = IsWord(subtitle[i]);
                if (wasWord == isWord) continue;

                wasWord = isWord;
                if (isWord) wordCount++;
            }

            return wordCount;

            bool IsWord(char c) => c != ' ';
        }
    }
}
