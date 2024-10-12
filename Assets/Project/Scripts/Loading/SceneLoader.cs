// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Threading.Tasks;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Loads the Levels by name in Start or on request
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField]
        private string _levelName;

        [SerializeField]
        private bool _startMode;

        private State _state = State.None;

        private void Start()
        {
            if (_startMode)
            {
                Load();
            }
        }


        private async Task PreloadAsync()
        {
            _state = State.Loaded;
            await MasterLoader.LoadLevel(_levelName);
        }

        private async Task ActivateAsync()
        {
            _state = State.Activated;
            var level = MasterLoader.GetLevel(_levelName);
            if (level.state == MasterLoader.Level.State.Loading) await level.CurrentTask();
            await MasterLoader.ActivateAndUnloadOthers(_levelName);
        }

        /// <summary>
        /// Called by UnityEvents
        /// </summary>
        public async void Preload() => await PreloadAsync();

        /// <summary>
        /// Called by UnityEvents
        /// </summary>
        public async void Load()
        {
            if (_state == State.None)
            {
                await PreloadAsync();
                await ActivateAsync();
            }
            else if (_state == State.Loaded)
            {
                await ActivateAsync();
            }
        }


        enum StartMode
        {
            None,
            Preload,
            Load
        }

        enum State
        {
            None,
            Loaded,
            Activated
        }
    }
}
