// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Total play timer
    /// </summary>
    public class PlayTimer : MonoBehaviour
    {
        private const string _key = "play_time";
        private float _startTime;

        void Start()
        {
            _startTime = Time.time;
        }

        private void OnDestroy()
        {
            var total = Store.GetFloat(_key);
            var timeForThisLevel = Time.time - _startTime;
            Store.SetFloat(_key, total + timeForThisLevel);
        }
    }
}
