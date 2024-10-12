// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Collections;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Trakcs the round time of the drone mini game as well ass player and drone scores
    /// </summary>
    public class DroneMiniGame : ActiveStateObserver, IActiveState
    {
        [SerializeField]
        private int _roundDuration = 60;
        [SerializeField]
        DroneFlightController _droneFlightController;
        [SerializeField]
        ProgressTracker _progressTracker;
        [SerializeField]
        int _endGameProgress = 99;
        [SerializeField]
        DroneTutorial _droneTutorial;

        [SerializeField]
        AudioTrigger _tickAudio;
        [SerializeField]
        AudioTrigger _winAudio;
        [SerializeField]
        AudioTrigger _loseAudio;

        public event Action WhenChanged = delegate { };

        private int _playerShots = 0;
        private int _droneShots = 0;

        public bool IsPlaying { get; private set; }
        public int PlayerScore { get; private set; }
        public int DroneScore { get; private set; }
        public float TimeLeft { get; private set; }
        public float DroneAccuracy => _droneShots <= 0 ? -1 : DroneScore / (float)_droneShots;
        public float PlayerAccuracy => _playerShots <= 0 ? -1 : PlayerScore / (float)_playerShots;

        bool IActiveState.Active => IsPlaying;

        public int RoundsPlayed { get; private set; } = 0;
        public bool IsTutorial { get; private set; }

        public DroneTutorial Tutorial => _droneTutorial;

        public int RoundsWon { get; private set; }

        protected override void HandleActiveStateChanged()
        {
            if (Active == IsPlaying) return;

            if (Active) { StartGame(); }
            else { EndGame(); }
        }

        public void StartGame()
        {
            _progressTracker.SetProgress(100);

            IsPlaying = true;
            WhenChanged();

            if (RoundsPlayed > 0)
            {
                StartRound();
            }
            else
            {
                StartTutorial();
            }
        }

        private void StartTutorial()
        {
            StartCoroutine(StartTutorialRoutine());
            IEnumerator StartTutorialRoutine()
            {
                IsTutorial = true;
                yield return StartCoroutine(_droneTutorial.TutorialRoutine());
                IsTutorial = false;
                StartRound();
            }
        }

        void StartRound()
        {
            PlayerScore = _playerShots = DroneScore = _droneShots = 0;
            TimeLeft = _roundDuration;

            ProjectileHitReaction.WhenAnyHit += IncrementDroneScore;
            BlasterProjectile.WhenAnyFire += UpdateAccuracy;

            WhenChanged();
            _droneFlightController.SetFlying(true);

            StartCoroutine(Countdown());
            IEnumerator Countdown()
            {
                yield return null;
                var oneSecond = new WaitForSeconds(1);
                while (Active && TimeLeft-- > 0)
                {
                    WhenChanged();
                    if (TimeLeft <= 10)
                    {
                        _tickAudio.Play();
                    }
                    yield return oneSecond;
                }

                if (Active)
                {
                    var audio = (PlayerScore > DroneScore) ? _winAudio : _loseAudio;
                    audio.Play();

                    if (PlayerScore > DroneScore) { RoundsWon++; }

                    SetEndGameProgress();
                }
            }
        }

        public void SetEndGameProgress()
        {
            _progressTracker.SetProgress(_endGameProgress, true);
        }

        void EndGame()
        {
            StopAllCoroutines();

            IsPlaying = false;
            TimeLeft = 0;
            RoundsPlayed++;
            WhenChanged();

            _droneFlightController.SetFlying(false);

            ProjectileHitReaction.WhenAnyHit -= IncrementDroneScore;
            BlasterProjectile.WhenAnyFire -= UpdateAccuracy;
        }

        private void OnDestroy() => EndGame();

        private void UpdateAccuracy(BlasterProjectile.Owner player)
        {
            IncrememtShots(player);
            WhenChanged?.Invoke();
        }

        private void IncrementDroneScore(ProjectileHitReaction hit)
        {
            if (hit is PlayerHitReaction)
            {
                DroneScore++;
                WhenChanged?.Invoke();
            }
            else if (hit is TargetProjectileReaction)
            {
                PlayerScore++;
                WhenChanged?.Invoke();
            }
        }

        private void IncrememtShots(BlasterProjectile.Owner obj)
        {
            if (obj == BlasterProjectile.Owner.Enemy)
            {
                _droneShots++;
            }
            else
            {
                _playerShots++;
            }
        }
    }
}
