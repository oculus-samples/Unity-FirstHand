// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField]
        GameObject _prefab;

        [SerializeField]
        FloatRange _timeBetweenSpawns = new FloatRange(2, 5);

        [SerializeField]
        ReferenceActiveState _active = ReferenceActiveState.Optional();

        private void OnEnable()
        {
            StartCoroutine(SpawnRoutine());
            IEnumerator SpawnRoutine()
            {
                while (true)
                {
                    yield return new WaitForSeconds(_timeBetweenSpawns.Random());
                    if (_active)
                    {
                        Instantiate(_prefab);
                    }
                }
            }
        }
    }
}
