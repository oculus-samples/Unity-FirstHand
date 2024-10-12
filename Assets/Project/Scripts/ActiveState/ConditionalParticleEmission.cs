// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Used to turn particle emission on/off based on an active state
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public class ConditionalParticleEmission : ActiveStateObserver
    {
        [SerializeField]
        private float _transitionDuration = 0.5f;

        private EmissionMultiplier _emission;

        protected override void Start()
        {
            base.Start();
            _emission = new EmissionMultiplier(GetComponent<ParticleSystem>().emission);
            _emission.RateMultiplier = Active ? 1 : 0;
        }

        protected override void HandleActiveStateChanged()
        {
            TweenRunner.Tween(_emission.RateMultiplier, Active ? 1 : 0, _transitionDuration, x => _emission.RateMultiplier = x).SetID(this);
        }
    }

    /// <summary>
    /// Wrapper for an emission module that saves the initial values
    /// </summary>
    //https://forum.unity.com/threads/particlesystem-rateovertimemultiplier-and-rateoverdistancemultiplier-not-working-as-expected.460672/#post-7395656
    public struct EmissionMultiplier
    {
        private ParticleSystem.EmissionModule _emission;

        private readonly ParticleSystem.MinMaxCurve _initialRateOverTime;
        private readonly ParticleSystem.MinMaxCurve _initialRateOverDistance;

        private float _multiplier;
        private float _overTimeMultiplier;
        private float _overDistanceMultiplier;

        public EmissionMultiplier(ParticleSystem.EmissionModule emission)
        {
            _initialRateOverDistance = emission.rateOverDistance;
            _initialRateOverTime = emission.rateOverTime;
            _emission = emission;
            _multiplier = _overTimeMultiplier = _overDistanceMultiplier = 1;
        }

        /// <summary>
        /// Globally multipled over the particle systems rates
        /// </summary>
        public float RateMultiplier
        {
            get => _multiplier;
            set
            {
                _multiplier = value;
                UpdateEmission();
            }
        }

        /// <summary>
        /// Multipled over the particle systems default rate over time
        /// </summary>
        public float RateOverTimeMultiplier
        {
            get => _overTimeMultiplier;
            set
            {
                _overTimeMultiplier = value;
                UpdateEmission();
            }
        }

        /// <summary>
        /// Multipled over the particle systems default rate over distance
        /// </summary>
        public float RateOverDistanceMultiplier
        {
            get => _overDistanceMultiplier;
            set
            {
                _overDistanceMultiplier = value;
                UpdateEmission();
            }
        }

        private void UpdateEmission()
        {
            float overDistance = _overDistanceMultiplier * _multiplier;
            switch (_initialRateOverDistance.mode)
            {
                case ParticleSystemCurveMode.Curve:
                case ParticleSystemCurveMode.TwoCurves:
                    _emission.rateOverDistanceMultiplier = overDistance;
                    break;
                default:
                    _emission.rateOverDistance = MultiplyConstant(_initialRateOverDistance, overDistance);
                    break;
            }

            float overTime = _overTimeMultiplier * _multiplier;
            switch (_initialRateOverTime.mode)
            {
                case ParticleSystemCurveMode.Curve:
                case ParticleSystemCurveMode.TwoCurves:
                    _emission.rateOverTimeMultiplier = overTime;
                    break;
                default:
                    _emission.rateOverTime = MultiplyConstant(_initialRateOverTime, overTime);
                    break;
            }
        }

        private static ParticleSystem.MinMaxCurve MultiplyConstant(ParticleSystem.MinMaxCurve minMaxCurve, float multiplier)
        {
            switch (minMaxCurve.mode)
            {
                case ParticleSystemCurveMode.Constant:
                    return minMaxCurve.constant * multiplier;
                case ParticleSystemCurveMode.TwoConstants:
                    return new ParticleSystem.MinMaxCurve(minMaxCurve.constantMin * multiplier, minMaxCurve.constantMax * multiplier);
                default:
                    return minMaxCurve;
            }
        }
    }
}
