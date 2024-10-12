// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// AI action for the drone that handles deploying the gun and aiming and shooting at a target.
    /// Targets are defined using the "DroneBlasterTarget" GameObject tag.
    /// There is some built-in error in the aiming so that it doesn't always shoot perfectly at the target every time.
    /// </summary>
    public class DroneGunAction : DroneAction
    {
        [SerializeField] private Blaster _blaster;

        [SerializeField] private Transform _gunTransform;
        [SerializeField] private Quaternion _gunTransformRotationOffset = new Quaternion(x: -0.70710677f, y: 0, z: 0, w: 0.70710677f);

        private GameObject[] _blasterTargets;
        private Transform _currentBlasterTarget;

        // speed at which to blend in/out of animated vs aimed modes
        [SerializeField] private float _gunAimBlendSpeed = 2.0f;
        private float _gunAimBlendAmount;

        // max angle inaccuracy when aiming
        [SerializeField] private float _maxAimError = 20f;

        // how quickly the gun moves due to error
        [SerializeField] private float _aimErrorUpdateSpeed = 0.3f;

        [SerializeField] private float _delayBeforeShots = 0.5f;
        [SerializeField] private IntRange _shortsPerBurst;
        [SerializeField] private float _timeBetweenShots = 0.2f;

        private int _deployGunAnimationParameter;

        protected override void Awake()
        {
            base.Awake();

            _deployGunAnimationParameter = Animator.StringToHash("DeployGun");
            _blasterTargets = GameObject.FindGameObjectsWithTag("DroneBlasterTarget");
        }

        protected override bool CanPerformAction()
        {
            // This action can only be performed when the drone is in a volume with the "CanDeployGunInVolume" flag
            // set to true.
            return FlightController.Flying && FlightController.Moving && CurrentActionVolumesContains(x => x.CanDeployGunInVolume);
        }

        protected override void StartAction()
        {
            StartCoroutine(FiringSequence());
            IEnumerator FiringSequence()
            {
                FlightController.Move(false);

                ActionAnimator.SetBool(_deployGunAnimationParameter, true);

                // pick a random target to shoot at (assumes that targets aren't dynamically instantiated / destroyed)
                int randomTargetIndex = Random.Range(0, _blasterTargets.Length);
                _currentBlasterTarget = _blasterTargets[randomTargetIndex].transform;

                yield return WaitForSecondsNonAlloc.WaitForSeconds(_delayBeforeShots);
                while (ActionIsActive)
                {
                    int numShotsInBurst = _shortsPerBurst.Random();
                    for (int i = 0; i < numShotsInBurst; ++i)
                    {
                        if (ActionIsActive)
                        {
                            var projectile = _blaster.Fire();

                            // to make it easier for the user the first few shots dont do damage, theyre like warning shots to alert the user
                            if (i < 3) projectile.Lethal = false;

                            yield return WaitForSecondsNonAlloc.WaitForSeconds(_timeBetweenShots);
                        }
                    }
                }
            }
        }

        protected override void EndAction()
        {
            Debug.Log("End");
            ActionAnimator.SetBool(_deployGunAnimationParameter, false);
            FlightController.Move(true);
        }

        private void LateUpdate()
        {
            if (_currentBlasterTarget)
            {
                float aimBlendAmount = ActionIsActive ? 1f : 0;
                _gunAimBlendAmount = Mathf.Lerp(_gunAimBlendAmount, aimBlendAmount, Time.deltaTime * _gunAimBlendSpeed);

                // aim at the target
                Vector3 toTarget = _currentBlasterTarget.position - _gunTransform.position;
                Quaternion aimRotation = Quaternion.LookRotation(toTarget) * _gunTransformRotationOffset;

                // add some noise
                float yawNoise = (Mathf.PerlinNoise(Time.time * _aimErrorUpdateSpeed, 1.1234f) - 0.5f) * _maxAimError;
                float pitchNoise = (Mathf.PerlinNoise(Time.time * _aimErrorUpdateSpeed, 7.7721f) - 0.5f) * _maxAimError;
                Quaternion noiseRotation = Quaternion.Euler(pitchNoise, 0, yawNoise);
                aimRotation *= noiseRotation;

                Quaternion animatedRotation = _gunTransform.rotation;
                _gunTransform.rotation = Quaternion.Slerp(animatedRotation, aimRotation, _gunAimBlendAmount);
            }
        }


        protected override void HandleVolumeChanged()
        {
            if (CanPerformAction())
            {
                //StartAction();
            }
        }
    }
}
