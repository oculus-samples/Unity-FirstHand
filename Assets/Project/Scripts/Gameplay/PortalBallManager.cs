// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.Events;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Handles the behaviour of the portal balls
    /// </summary>
    public class PortalBallManager : MonoBehaviour
    {
        [SerializeField] GameObject _ballWithoutParticles;
        [SerializeField] ParticleSystem _ballParticles;
        [SerializeField] ParticleSystem _ballBurstParticles;
        [SerializeField] int _selfDestructTime = 40;
        [SerializeField] int _startHealth = 50;
        [SerializeField] AudioTrigger _audioLoop;
        [SerializeField] AudioTrigger _audioHit;
        [SerializeField] AudioTrigger _audioPop;
        [SerializeField] ReferenceActiveState _canBeDamaged = ReferenceActiveState.Optional();
        [SerializeField] UnityEvent _whenPopped;

        private float _defaultScale = -1;
        private int _ballHealth;
        private Tween _popAfterDelay;

        private void Awake()
        {
            _defaultScale = transform.localScale.x;
            SetHealth(_startHealth);

            if (_selfDestructTime > 0)
            {
                // after 40 seconds pop the bubble regardless of health
                _popAfterDelay = TweenRunner.DelayedCall(_selfDestructTime, () => SetHealth(0));
            }

            _audioLoop.Play();
        }

        private void OnDestroy()
        {
            _popAfterDelay?.Kill();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (_ballHealth <= 0 || !_canBeDamaged) return;

            SetHealth(_ballHealth - 1);

            if (_ballHealth > 0)
            {
                WobbleBubble();
            }
        }

        private void WobbleBubble()
        {
            // if we're already tween this we're already wobbling
            if (TweenRunner.IsTweening(this)) return;

            _audioHit.Play();

            TweenRunner.Tween01(0.6f, x =>
            {
                var weight = 1 - Mathf.Abs((x * 2f) - 1);
                var time = Time.time * 2;
                var noise = new Vector3(Mathf.PerlinNoise(time, 0), Mathf.PerlinNoise(time + 1, 0), Mathf.PerlinNoise(time + 2, 0)) * 2f - Vector3.one;
                var baseScale = Vector3.one * _defaultScale;
                var randomWobble = baseScale + noise * 0.3f;
                transform.localScale = Vector3.Lerp(baseScale, randomWobble, weight);

            }).SetID(this);
        }

        public void SetHealth(int health)
        {
            health = Mathf.Max(health, 0);
            if (_ballHealth == health) return;

            _ballHealth = health;
            if (_ballHealth <= 0)
            {
                // kill the tween used to pop it after a delay
                // stops TweenRunner thowing an exception
                _popAfterDelay?.Kill();

                _audioLoop.Stop();
                _audioPop.Play();
                _ballBurstParticles.Play();
                _ballWithoutParticles.SetActive(false);
                _whenPopped.Invoke();
                Destroy(gameObject, 2);
            }
        }

        // called by UnityEvent
        public void Spawn()
        {
            var go = Instantiate(gameObject, transform.position + transform.forward * 0.05f + Random.insideUnitSphere * 0.15f, transform.rotation);
            go.transform.localScale = Vector3.one * Random.Range(0.2f, 0.4f);
            go.SetActive(true);
            ExplosiveForce.Explode(transform.position - transform.forward * 0.3f, 0.4f, Random.Range(20, 30), 0);
        }
    }
}
