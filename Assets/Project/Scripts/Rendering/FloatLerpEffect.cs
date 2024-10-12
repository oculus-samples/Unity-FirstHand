// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class FloatLerpEffect : RendererEffect
    {
        [SerializeField]
        ReferenceActiveState _active;

        [SerializeField]
        FloatRange _range;

        [SerializeField]
        string _floatProperty;

        [SerializeField]
        float _duration = 0.5f;

        bool _wasActive;
        float _value;
        int _propertyID;

        private void Start()
        {
            if (!Application.isPlaying) return;

            _propertyID = Shader.PropertyToID(_floatProperty);
            _wasActive = _active;
            _value = _wasActive ? _range.Max : _range.Min;
        }

        protected override void Update()
        {
            if (!Application.isPlaying) return;

            if (_wasActive != _active)
            {
                _wasActive = _active;
                TweenRunner.Tween(_value, _wasActive ? _range.Max : _range.Min, _duration, x => _value = x);
            }
            base.Update();
        }

        protected override void UpdateProperties(MaterialPropertyBlock block)
        {
            if (Application.isPlaying)
            {
                block.SetFloat(_propertyID, _value);
            }
            base.UpdateProperties(block);
        }
    }
}
