// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Adds an impression to the player of damage via a vignette
    /// TODO muffle audio
    /// </summary>
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField]
        private float _health = 1;
        [SerializeField]
        private OVRVignette _vignette;
        [SerializeField]
        private FloatRange _vignetteRange = new FloatRange(65, 120);
        [SerializeField]
        private List<ParticleSystem> _particles = new List<ParticleSystem>();
        [SerializeField]
        private FloatRange _particlesRateRange = new FloatRange(0, 10);

        private object _damageID, _recoverID;

        private void Awake()
        {
            _damageID = GetInstanceID() + "damage";
            _recoverID = GetInstanceID() + "recover";

            if (!_vignette)
            {
                enabled = false;
            }
        }

        private void OnValidate()
        {
            if (!Application.isPlaying) return;

            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            if (!_vignette) { return; }

            float healthVisual = Mathf.InverseLerp(0.33f, 1, _health);

            _vignette.enabled = healthVisual < 1;
            _vignette.VignetteFieldOfView = _vignetteRange.Lerp((float)healthVisual);
            _particles.ForEach(x =>
            {
                var emission = x.emission;
                emission.rateOverTimeMultiplier = _particlesRateRange.Lerp(1 - healthVisual);
            });
        }

        private void SetHealth(float value)
        {
            _health = value;
            UpdateVisuals();
        }

        public void Damage()
        {
            // if we already got hit, skip that tween to get the health to the right value
            if (TweenRunner.TryGetTween(_damageID, out var tween)) tween.Skip();

            // tween to the new lower health
            float newHealth = _health - 0.1f;
            TweenRunner.Tween(_health, newHealth, 0.1f, SetHealth).SetID(_damageID);

            // after 0.5 seconds assuming we didnt get damaged again, recover full health
            TweenRunner.Tween(newHealth, 1, 0.5f, SetHealth).Delay(0.5f).SetID(_recoverID);
        }
    }
}
