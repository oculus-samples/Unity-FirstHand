// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Handles all completed experiences and states the overall game is in
    /// </summary>
    public class HubManager : MonoBehaviour
    {
        [SerializeField] ReferenceActiveState _clocktowerComplete;
        [SerializeField] ReferenceActiveState _streetComplete;
        [SerializeField] ReferenceActiveState _mixedRealityComplete;
        [SerializeField] ReferenceActiveState _hapticsComplete;

        [SerializeField] private PlayableDirector _introTimeline;
        [SerializeField] private PlayableDirector _clocktowerTimeline;
        [SerializeField] private PlayableDirector _streetTimeline;
        [SerializeField] private PlayableDirector _mixedRealityTimeline;
        [SerializeField] private PlayableDirector _hapticsTimeline;

        [SerializeField] private ReferenceActiveState _canFinalePlay;
        [SerializeField] private ProgressTracker _finaleProgress;

        private bool _finalePlaying;
        HubState _state;

        public HubState State => _state;

        public bool IsAnyPlaying => IsPlaying(_introTimeline) || IsPlaying(_clocktowerTimeline) ||
                    IsPlaying(_streetTimeline) || IsPlaying(_mixedRealityTimeline) || IsPlaying(_hapticsTimeline);

        IEnumerator Start()
        {
            LoadState();

            while (true)
            {
                if (ShouldPlay(true, ref _state.introPlayed))
                {
                    yield return new PlayDirectorAndWait(_introTimeline);
                }

                if (ShouldPlay(_clocktowerComplete, ref _state.clocktowerIntroPlayed))
                {
                    yield return new PlayDirectorAndWait(_clocktowerTimeline);
                }

                if (ShouldPlay(_streetComplete, ref _state.streetIntroPlayed))
                {
                    yield return new PlayDirectorAndWait(_streetTimeline);
                }

                if (ShouldPlay(_mixedRealityComplete, ref _state.mixedRealityIntroPlayed))
                {
                    yield return new PlayDirectorAndWait(_mixedRealityTimeline);
                }

                if (ShouldPlay(_hapticsComplete, ref _state.hapticsIntroPlayed))
                {
                    yield return new PlayDirectorAndWait(_hapticsTimeline);
                }

                if (_state.AllPlayed && !_finalePlaying && _canFinalePlay)
                {
                    _finalePlaying = true;
                    _finaleProgress.SetProgress(10);
                }

                yield return null;
            }
        }

        bool ShouldPlay(bool play, ref bool hasPlayed)
        {
            if (!play) return false;
            if (hasPlayed) return false;

            hasPlayed = true;
            SaveState();
            return true;
        }

        void LoadState()
        {
            var stateString = Store.GetString("hub.state");
            _state = string.IsNullOrEmpty(stateString) ? default : JsonUtility.FromJson<HubState>(stateString);
        }

        void SaveState()
        {
            var stateString = JsonUtility.ToJson(_state);
            Store.SetString("hub.state", stateString);
        }

        [ContextMenu("Set All Played")]
        void SetAllWatched()
        {
            _state.introPlayed = true;
            _state.clocktowerIntroPlayed = true;
            _state.streetIntroPlayed = true;
            _state.hapticsIntroPlayed = true;
            _state.mixedRealityIntroPlayed = true;
            SaveState();
        }

        [ContextMenu("Set All Unplayed")]
        void SetAllUnplayed()
        {
            _state.introPlayed = false;
            _state.clocktowerIntroPlayed = false;
            _state.streetIntroPlayed = false;
            _state.hapticsIntroPlayed = false;
            _state.mixedRealityIntroPlayed = false;
            SaveState();
        }

        [ContextMenu("Set Intro")]
        void SetAllIntrolayed()
        {
            _state.introPlayed = true;
            _state.clocktowerIntroPlayed = false;
            _state.streetIntroPlayed = false;
            _state.hapticsIntroPlayed = false;
            _state.mixedRealityIntroPlayed = false;
            SaveState();
        }

        [Serializable]
        public struct HubState
        {
            public bool introPlayed;
            public bool clocktowerIntroPlayed;
            public bool streetIntroPlayed;
            public bool mixedRealityIntroPlayed;
            public bool hapticsIntroPlayed;

            public bool AllPlayed =>
                introPlayed &&
                clocktowerIntroPlayed &&
                streetIntroPlayed &&
                mixedRealityIntroPlayed &&
                hapticsIntroPlayed;
        }

        class PlayDirectorAndWait : CustomYieldInstruction
        {
            private PlayableDirector _director;

            public PlayDirectorAndWait(PlayableDirector director)
            {
                _director = director;
                director.Play();
            }

            public override bool keepWaiting => IsPlaying(_director);
        }

        private static bool IsPlaying(PlayableDirector director)
        {
            return director.state == PlayState.Playing && director.time < director.duration - float.Epsilon;
        }
    }
}
