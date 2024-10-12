// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.UI;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class LazerProjectile : ActiveStateObserver
    {
        [SerializeField] Transform _target;
        [SerializeField] Transform _end;
        [SerializeField] ParticleSystem _chargeUpParticles;
        [SerializeField] Animator _effectAnimator;
        [SerializeField] Explosion _explosion;
        [SerializeField] float _rayCastDelay = 0.1f;
        [SerializeField] float _fireTime = 0.1f;
        [SerializeField] float _fadeOutTime = 0.1f;
        [SerializeField] float _delayBetweenShots = 0.5f;
        [SerializeField] AudioTrigger _chargeUpMono;
        [SerializeField] private AudioTrigger _chargeUpAmbix;
        [SerializeField, Optional] AudioTrigger _chargeUpComplete;
        [SerializeField] Image _batteryImage;
        [SerializeField] private float _fallRate = 2;

        private static Vector3[] _positions = new Vector3[2];

        protected override void HandleActiveStateChanged()
        {
            _effectAnimator.SetBool("Fire", Active);
        }

        private void OnDisable()
        {
            _effectAnimator.SetBool("Fire", false);
        }

        public void StartFire()
        {
            BlasterProjectile.WhenAnyFire(BlasterProjectile.Owner.Player);
            int randomIndex = Random.Range(0, _chargeUpMono.GetClips().Length);
            _chargeUpMono.PlayAudio(randomIndex);
            //_chargeUpAmbix.PlayAudio(randomIndex);
            _chargeUpParticles.Play();
        }

        public void Recharged()
        {
            TweenRunner.Tween(_batteryImage.fillAmount, 1, 0.3f, x => _batteryImage.fillAmount = x);
        }

        protected override void Update()
        {
            base.Update();

            Vector3 endPos = Vector3.Lerp(_target.position, transform.position + transform.forward * 20, 15 * Time.deltaTime);
            _target.position = endPos;
        }

        public void LazerCast()
        {
            LineBetweenTransforms.GetBezierPositions(transform, _target, _positions);

            Vector3 lineStart = _positions[0];
            Vector3 lineEnd = _positions[_positions.Length - 1];
            lineEnd += _positions[_positions.Length - 1] - _positions[_positions.Length - 2];

            _explosion.Explode(lineStart, lineEnd);

            if (Physics.Linecast(lineStart, lineEnd, out var raycastHit))
            {
                var componentSource = raycastHit.rigidbody ? raycastHit.rigidbody.gameObject : raycastHit.collider.gameObject;
                if (componentSource.TryGetComponent(out ProjectileHitReaction hit))
                {
                    hit.OnProjectileHit(BlasterProjectile.Owner.Player, null);
                }
            }

            _batteryImage.fillAmount -= Time.deltaTime * _fallRate;
        }
    }

    public interface IFireable
    {
        BlasterProjectile Fire();
    }
}
