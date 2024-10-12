// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Interaction.Grab;
using Oculus.Interaction.Input;
using System.Collections;
using UnityEngine;

namespace Oculus.Interaction
{
    public class FingerGlowIndicator : MonoBehaviour
    {
        [SerializeField, Interface(typeof(IActiveState)), Optional]
        private MonoBehaviour _active;
        private IActiveState Active;

        [SerializeField, Interface(typeof(IActiveState)), Optional]
        private MonoBehaviour _flash;
        private IActiveState Flash;

        [SerializeField, Interface(typeof(IActiveState)), Optional]
        private MonoBehaviour _thumb, _index, _middle, _ring, _pinky;
        private IActiveState Thumb, Index, Middle, Ring, Pinky;

        [SerializeField]
        private MaterialPropertyBlockEditor _handMaterialPropertyBlockEditor;

        [SerializeField]
        private float _activeLerpSpeed = 5f;
        [SerializeField]
        private float _glowLerpSpeed = 5f;
        [SerializeField]
        private Color _fingerGlowColorActive = new Color(0.44f, 0, 1, 1);

        Coroutine _flashRoutine;

        #region public properties
        public float GlowLerpSpeed
        {
            get
            {
                return _glowLerpSpeed;
            }
            set
            {
                _glowLerpSpeed = value;
            }
        }

        public Color FingerGlowColorHover
        {
            get
            {
                return _fingerGlowColorActive;
            }
            set
            {
                _fingerGlowColorActive = value;
            }
        }
        #endregion

        private readonly int[] _handShaderGlowPropertyIds = new int[]
        {
            Shader.PropertyToID("_ThumbGlowValue"),
            Shader.PropertyToID("_IndexGlowValue"),
            Shader.PropertyToID("_MiddleGlowValue"),
            Shader.PropertyToID("_RingGlowValue"),
            Shader.PropertyToID("_PinkyGlowValue"),
        };

        private readonly int _fingerGlowColorPropertyId = Shader.PropertyToID("_FingerGlowColor");

        protected bool _started = false;

        private void Awake()
        {
            Active = _active as IActiveState;
            Flash = _flash as IActiveState;
            Thumb = _thumb as IActiveState;
            Index = _index as IActiveState;
            Middle = _middle as IActiveState;
            Ring = _ring as IActiveState;
            Pinky = _pinky as IActiveState;
        }

        protected virtual void Start()
        {
            this.BeginStart(ref _started);

            this.AssertField(_handMaterialPropertyBlockEditor, nameof(_handMaterialPropertyBlockEditor));

            this.EndStart(ref _started);
        }

        private void Update()
        {
            UpdateVisual();
        }

        private void UpdateVisual()
        {
            if (HandleFlash()) return;

            var propertyBlock = _handMaterialPropertyBlockEditor.MaterialPropertyBlock;

            var isActive = !_active || Active.Active;
            var targetColor = isActive ? _fingerGlowColorActive : Color.clear;

            Color color = propertyBlock.GetColor(_fingerGlowColorPropertyId);
            color = Color.Lerp(color, targetColor, _activeLerpSpeed * Time.deltaTime);
            propertyBlock.SetColor(_fingerGlowColorPropertyId, color);

            for (int i = 0; i < Constants.NUM_FINGERS; ++i)
            {
                var targetGlow = IsActive((HandFinger)i) ? 1 : 0;
                UpdateGlowValue(i, targetGlow);
            }

            _handMaterialPropertyBlockEditor.UpdateMaterialPropertyBlock();
        }

        private void UpdateGlowValue(int fingerIndex, float glowValue)
        {
            float currentGlowValue = _handMaterialPropertyBlockEditor.MaterialPropertyBlock.GetFloat(_handShaderGlowPropertyIds[fingerIndex]);
            float newGlowValue = Mathf.MoveTowards(currentGlowValue, glowValue, _glowLerpSpeed * Time.deltaTime);
            _handMaterialPropertyBlockEditor.MaterialPropertyBlock.SetFloat(_handShaderGlowPropertyIds[fingerIndex], newGlowValue);
        }

        bool IsActive(HandFinger handFinger)
        {
            switch (handFinger)
            {
                case HandFinger.Invalid: return false;
                case HandFinger.Thumb: return _thumb && Thumb.Active;
                case HandFinger.Index: return _index && Index.Active;
                case HandFinger.Middle: return _middle && Middle.Active;
                case HandFinger.Ring: return _ring && Ring.Active;
                case HandFinger.Pinky: return _pinky && Pinky.Active;
                default: throw new System.Exception("Invalid HandFinger");
            }
        }

        bool HandleFlash()
        {
            if (_flashRoutine != null) return true;

            var isActive = !_active || Active.Active;
            if (isActive && this.Flash.Active)
            {
                _flashRoutine = StartCoroutine(Flash());
                return true;
            }
            return false;

            IEnumerator Flash()
            {
                var propertyBlock = _handMaterialPropertyBlockEditor.MaterialPropertyBlock;
                Color startColor = propertyBlock.GetColor(_fingerGlowColorPropertyId);
                yield return StartCoroutine(TweenColor(startColor, Color.white, 0.2f));
                yield return StartCoroutine(TweenColor(Color.white, Color.clear, 0.5f));

                while (this.Flash.Active) yield return null;

                _flashRoutine = null;

                IEnumerator TweenColor(Color from, Color to, float duration)
                {
                    for (float time = 0; time < duration; time += Time.deltaTime)
                    {
                        SetColor(Color.Lerp(from, to, time / duration));
                        yield return null;
                    }
                    SetColor(to);

                    void SetColor(Color color)
                    {
                        propertyBlock.SetColor(_fingerGlowColorPropertyId, color);
                        for (int i = 0; i < Constants.NUM_FINGERS; ++i) UpdateGlowValue(i, 1);
                    }
                }
            }
        }

        #region Inject

        public void InjectAllGrabStrengthIndicator(MaterialPropertyBlockEditor handMaterialPropertyBlockEditor)
        {
            InjectHandMaterialPropertyBlockEditor(handMaterialPropertyBlockEditor);
        }

        public void InjectHandMaterialPropertyBlockEditor(MaterialPropertyBlockEditor handMaterialPropertyBlockEditor)
        {
            _handMaterialPropertyBlockEditor = handMaterialPropertyBlockEditor;
        }

        #endregion
    }
}
