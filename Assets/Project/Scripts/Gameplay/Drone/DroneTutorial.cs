// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class DroneTutorial : MonoBehaviour
    {
        [SerializeField]
        DroneFlightController _droneFlightController;
        [SerializeField]
        DroneGunAction _droneGunAction;
        [SerializeField]
        DroneTargetAction _droneTargetAction;
        [SerializeField]
        Animator _flightAnimator;
        [SerializeField]
        Animator _droneAnimator;
        [SerializeField]
        ReferenceActiveState _shieldDeployed;
        [SerializeField]
        UICanvas _shootInstructions;
        [SerializeField]
        UICanvas _blockInstructions;
        [SerializeField]
        UICanvas _scoreInstructions;
        [SerializeField]
        private ProgressTracker _progressTracker;

        public bool IsIntroComplete { get; private set; }

        public IEnumerator TutorialRoutine()
        {
            IsIntroComplete = false;

            _flightAnimator.SetBool("Tutorial", true);
            _droneFlightController.SetFlying(true);

            yield return WaitForSecondsNonAlloc.WaitForSeconds(8);

            IsIntroComplete = true;

            _droneTargetAction.PerformAction();
            _shootInstructions.Show(true);

            bool hasHitTarget = false;
            void SetHitTarget(ProjectileHitReaction hit) => hasHitTarget = hit is TargetProjectileReaction;

            ProjectileHitReaction.WhenAnyHit += SetHitTarget;
            yield return new WaitWhile(() => !hasHitTarget);
            ProjectileHitReaction.WhenAnyHit -= SetHitTarget;

            _shootInstructions.Show(false);

            _droneAnimator.SetBool("DeployGun", true);
            yield return WaitForSecondsNonAlloc.WaitForSeconds(1);

            _blockInstructions.Show(true);

            yield return new WaitWhile(() => !_shieldDeployed);

            _droneGunAction.PerformAction();

            yield return WaitForSecondsNonAlloc.WaitForSeconds(4);
            _blockInstructions.Show(false);

            yield return WaitForSecondsNonAlloc.WaitForSeconds(0.5f);
            _scoreInstructions.Show(true);
            yield return WaitForSecondsNonAlloc.WaitForSeconds(4);
            _scoreInstructions.Show(false);

            _flightAnimator.SetBool("Tutorial", false);
            _progressTracker.SetProgress(105);
        }
    }
}
