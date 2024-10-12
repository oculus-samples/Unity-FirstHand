// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Shows a 'ghost' hand visual to guide the player on what they should do next
    /// </summary>
    public class AnimatedGhostHand : ActiveStateObserver
    {
        [SerializeField] private AnimationClip _animationClip;
        [SerializeField] private Animation _animationComponent;
        [SerializeField] private FloatProperty _maxOpacity;
        [SerializeField] private float _fadeInTime = 0.5f;
        [SerializeField] private float _fadeOutTime = 0.5f;
        [SerializeField] private float _delayBeforeStart = 0f;
        [SerializeField] private float _delayBetweenLoops = 1.0f;
        [SerializeField] private float _trimClipFromStartTime = 0.0f;
        [SerializeField] private float _trimClipFromEndTime = 0.0f;
        [SerializeField] private float _playbackSpeed = 1;
        private AnimationState _animationState;

        [SerializeField] private MaterialPropertyBlockEditor _materialPropertyBlockEditor;
        private int _opacityShaderParameterId;
        private int _outlineOpacityShaderParameterId;

        private Coroutine _playLoop;
        private float _activeAlpha = 0;
        private float _animationAlpha = 0;

#if UNITY_EDITOR
        [SerializeField, Range(0f, 1f)] private float _editorPreview;
#endif

        protected override void Start()
        {
            base.Start();

            Assert.IsNotNull(_animationClip);
            Assert.IsNotNull(_animationComponent);
            _animationComponent.clip = _animationClip;
            _animationState = _animationComponent[_animationClip.name];
            _animationState.normalizedTime = 0f;
            Assert.IsNotNull(_animationState);

            Assert.IsNotNull(_materialPropertyBlockEditor);
            _opacityShaderParameterId = Shader.PropertyToID("_Opacity");
            _outlineOpacityShaderParameterId = Shader.PropertyToID("_OutlineOpacity");

            SetAlphaValue(0);

            HandleActiveStateChanged();
        }

        protected override void Update()
        {
            base.Update();
            _animationAlpha = GetAnimationAlpha();
            SetAlphaValue(_animationAlpha * _activeAlpha * _maxOpacity.Value);
        }

        private void OnDisable()
        {
            SetPlaying(false);
        }

        private float GetAnimationAlpha()
        {
            float animTime = _animationState.time - _trimClipFromStartTime;
            float inverseAnimTime = (_animationState.length - (_trimClipFromEndTime + _trimClipFromStartTime)) - animTime;
            float fadeIn = Mathf.Min(1, animTime / _fadeInTime);
            float fadeOut = Mathf.Min(1, inverseAnimTime / _fadeOutTime);
            return Mathf.Min(fadeIn, fadeOut);
        }

        protected override void HandleActiveStateChanged() => SetPlaying(Active);

        public void SetPlaying(bool play)
        {
            bool isPlaying = _playLoop != null;
            if (isPlaying == play) { return; }

            TweenRunner.Tween(_activeAlpha, Active ? 1 : 0, _fadeOutTime, x => _activeAlpha = x).SetID(this);

            if (play)
            {
                _playLoop = StartCoroutine(DelayAndThenPlay());
                IEnumerator DelayAndThenPlay()
                {
                    yield return new WaitForSeconds(_delayBeforeStart);
                    float endTime = _animationState.length - _trimClipFromEndTime;
                    while (true)
                    {
                        _animationComponent.Play();
                        _animationState.time = _trimClipFromStartTime;
                        _animationState.speed = _playbackSpeed;

                        yield return new WaitWhile(() => _animationState.time < endTime && _animationComponent.isPlaying);

                        _animationComponent.Stop();

                        yield return new WaitForSeconds(_delayBetweenLoops);
                    }
                }
            }
            else
            {
                StopCoroutine(_playLoop);
                _playLoop = null;
            }
        }

        private void SetAlphaValue(float alpha)
        {
            bool enableRenderers = alpha > 0;
            var renderers = _materialPropertyBlockEditor.Renderers;
            for (int i = 0; i < renderers.Count; i++)
            {
                renderers[i].enabled = enableRenderers;
            }

            if (enableRenderers)
            {
                _materialPropertyBlockEditor.MaterialPropertyBlock.SetFloat(_opacityShaderParameterId, alpha);
                _materialPropertyBlockEditor.MaterialPropertyBlock.SetFloat(_outlineOpacityShaderParameterId, alpha);
                _materialPropertyBlockEditor.UpdateMaterialPropertyBlock();
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying || !_animationComponent || !_animationClip)
            {
                return;
            }

            // clear the existing animation component state
            if (_animationClip != _animationComponent.clip && _animationComponent.clip)
            {
                _animationComponent.RemoveClip(_animationComponent.clip.name);
            }

            // set up the new animation clip and sample it at the given time
            _animationComponent.AddClip(_animationClip, _animationClip.name);
            _animationComponent.clip = _animationClip;
            _animationComponent.Play();
            _animationState = _animationComponent[_animationClip.name];
            _animationState.normalizedTime = _editorPreview;
            _animationComponent.Sample();
        }
#endif
    }
}
