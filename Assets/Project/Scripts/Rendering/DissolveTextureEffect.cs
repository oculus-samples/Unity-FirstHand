// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Dissolve effect to be used on a texture
    /// </summary>
    public class DissolveTextureEffect : RendererEffect
    {
        private static readonly int _dissolveTexID = Shader.PropertyToID("Dissolve_Tex");
        private static readonly int _dissolveEdgeID = Shader.PropertyToID("Dissolve_Edge");
        private static readonly int _dissolveEdgeSettingsID = Shader.PropertyToID("Dissolve_EdgeSettings");
        private static readonly int _dissolveColorID = Shader.PropertyToID("Dissolve_Color");

        [SerializeField] private float _visibility = 1;
        [SerializeField] private Color _dissolveColor;
        [SerializeField, Tooltip("Inner Fade, Band Width, Outer Fade, Tiling")]
        private Vector4 _dissolveEdgeSettings;
        [SerializeField] private Texture _dissolveTex;

        [SerializeField]
        private ReferenceActiveState _active = ReferenceActiveState.Optional();
        [SerializeField]
        private float _duration = 1;
        [SerializeField]
        private bool _controlRendersEnabled = false;

        private bool _wasActive;
        private float _lastVisibility = -1;


        protected override void OnEnable()
        {
            base.OnEnable();

            if (Application.isPlaying && _active.HasReference)
            {
                _wasActive = _active;
                _visibility = _wasActive ? 1 : 0;
            }
        }

        protected override void Update()
        {
            if (Application.isPlaying && _active.HasReference)
            {
                if (_wasActive != _active)
                {
                    _wasActive = _active;
                    TweenRunner.Tween(_visibility, _wasActive ? 1 : 0, _duration, x => _visibility = x).SetID(this);
                }
            }

            base.Update();

            if (Application.isPlaying && _controlRendersEnabled && _lastVisibility != _visibility)
            {
                _lastVisibility = _visibility;
                var enableRenderers = _visibility > 0;

                for (int i = 0; i < _renderers.Length; i++)
                {
                    _renderers[i].enabled = enableRenderers;
                }
            }
        }

        protected override void UpdateMaterial(Material material)
        {
            base.UpdateMaterial(material);
            material.EnableKeyword("DISSOLVE_TEXTURE_ON");
        }

        protected override void UpdateProperties(MaterialPropertyBlock block)
        {
            base.UpdateProperties(block);
            block.SetTexture(_dissolveTexID, _dissolveTex);
            float edgeLength = (_dissolveEdgeSettings.y + _dissolveEdgeSettings.z) * (1 - _visibility);
            block.SetFloat(_dissolveEdgeID, _visibility - edgeLength);
            block.SetVector(_dissolveEdgeSettingsID, _dissolveEdgeSettings);
            block.SetColor(_dissolveColorID, _dissolveColor);
        }
    }
}
