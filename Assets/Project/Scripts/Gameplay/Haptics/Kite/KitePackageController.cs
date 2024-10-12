// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class KitePackageController : MonoBehaviour
    {
        [Header("Collection")]
        [SerializeField]
        private List<PackagesInteractable> _instances;
        [SerializeField]
        private List<Transform> _targetPositions;
        [SerializeField]
        private int _numbrOfRounds = 3;
        [SerializeField]
        private float _timeBetweenRounds = 2;
        [SerializeField]
        private int _numberToCollectPerRound = 10;
        [SerializeField]
        private KiteTail _kiteTail;

        [Header("Drop Off")]
        [SerializeField]
        private TriggerZone _collectionTriggerZone;
        [SerializeField]
        private PlayableDirector _collectionTimeline;
        [SerializeField]
        private Transform _collectionFollowSource;

        private int _collectedCount = 0;
        private int _round = -1;
        private TriggerZoneList<KiteTail> _kitesInDropOffZone;

        public int CollectedCount => _collectedCount;
        public int TargetCollectedCount => _numberToCollectPerRound;

        public bool IsPlaying { get; private set; }
        public bool HasTimer { get; private set; }
        public float Time { get; private set; } = -1;
        public int GamesCompleted { get; private set; }
        public float Round => _round;
        public float MaxRounds => _numbrOfRounds;

        public event Action WhenChanged;

        private void Start()
        {
            _kitesInDropOffZone = new TriggerZoneList<KiteTail>(_collectionTriggerZone);
        }

        public void PlayCollectMinigame()
        {
            StartCoroutine(CollectMinigameRoutine());
            IEnumerator CollectMinigameRoutine()
            {
                _round = -1;
                IsPlaying = true;

                for (int i = 0; i < _numbrOfRounds; i++)
                {
                    _round++;
                    _collectedCount = 0;
                    WhenChanged?.Invoke();

                    CreateEnoughPackages();

                    ListenToPackageCollection(true);

                    yield return new WaitUntil(() => _collectedCount == _numberToCollectPerRound);

                    ListenToPackageCollection(false);

                    yield return new WaitUntil(() => _kitesInDropOffZone.Count > 0);

                    _collectionTimeline.Stop();
                    _collectionTimeline.Play();

                    _kiteTail.TransferTo(_collectionFollowSource, (float)_collectionTimeline.duration);

                    yield return new WaitForSeconds(_timeBetweenRounds);
                }

                IsPlaying = false;
                GamesCompleted++;
                WhenChanged?.Invoke();
            }
        }

        private void ListenToPackageCollection(bool listen)
        {
            _instances.ForEach(x =>
            {
                x.WhenPackageIsCollected -= HandlePackageCollected;
                if (listen) x.WhenPackageIsCollected += HandlePackageCollected;
            });
        }

        void HandlePackageCollected(PackagesInteractable x)
        {
            _collectedCount++;
            _kiteTail.GrowTail();

            WhenChanged?.Invoke();

            CreateEnoughPackages();
        }

        private void CreateEnoughPackages()
        {
            for (int i = 0; i < _instances.Count; i++)
            {
                var activePackageCount = GetActivePackageCount();

                if (activePackageCount == _instances.Count) return;
                if (_collectedCount + activePackageCount >= _numberToCollectPerRound) return;

                var package = GetUnusedPackage();
                var target = GetRandomUnusedTarget();
                package.SetTargetAndShow(target);
            }
        }

        private Transform GetRandomUnusedTarget()
        {
            // pick a random place to start in the array
            int start = UnityEngine.Random.Range(0, _targetPositions.Count);

            for (int i = 0; i < _targetPositions.Count; i++)
            {
                // starting from the random place loop over the array
                int targetIndex = (i + start) % _targetPositions.Count;

                var target = _targetPositions[targetIndex];

                if (_instances.TrueForAll(x => x.Target != target))
                {
                    return target;
                }
            }

            throw new Exception();
        }

        private int GetActivePackageCount()
        {
            int result = 0;
            for (int i = 0; i < _instances.Count; i++)
            {
                if (_instances[i].gameObject.activeSelf) result++;
            }
            return result;
        }

        private PackagesInteractable GetUnusedPackage()
        {
            for (int i = 0; i < _instances.Count; i++)
            {
                if (!_instances[i].gameObject.activeSelf) return _instances[i];
            }
            return null;
        }
    }
}
