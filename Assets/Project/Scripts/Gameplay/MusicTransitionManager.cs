// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.Serialization;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Instructs the MasterMusic singleton on what music to play
    /// </summary>
    public class MusicTransitionManager : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("startMainLoopOnStart")]
        private bool _startMainLoopOnStart = true;
        [SerializeField, FormerlySerializedAs("mainLoop")]
        private string _mainLoop = "main";

        private void Start()
        {
            if (_startMainLoopOnStart) Play();
        }

        public void musicMainLoopPlay() => Play("main");

        public void musicBattleLoopPlay() => Play("battle");

        public void musicOutroPlay() => Play("outro");

        public void Play() => Play(_mainLoop);

        private void Play(string id)
        {
            MasterMusic.Instance.Play(id);
        }
    }
}
