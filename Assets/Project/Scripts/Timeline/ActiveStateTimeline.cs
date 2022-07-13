/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Use of the material below is subject to the terms of the MIT License
 * https://github.com/oculus-samples/Unity-FirstHand/tree/main/Assets/Project/LICENSE.txt
 */

using UnityEngine;
using UnityEngine.Playables;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Plays a timeline to a specified time when active, then plays to the end when inactive
    /// </summary>
    public class ActiveStateTimeline : ActiveStateObserver
    {
        [SerializeField]
        PlayableDirector _playableDirector;
        [SerializeField]
        float _activeTime = 0.5f;

        protected override void Reset()
        {
            base.Reset();
            _playableDirector = GetComponent<PlayableDirector>();
            _playableDirector.extrapolationMode = DirectorWrapMode.Hold;
        }

        protected override void Start()
        {
            base.Start();
            UpdateTimeline();
        }

        protected override void HandleActiveStateChanged()
        {
            UpdateTimeline();
        }

        private void UpdateTimeline()
        {
            if (!_playableDirector.playableGraph.IsValid()) { _playableDirector.RebuildGraph(); }

            if (_activeTime > 0)
            {
                PlayTimlineUsingMiddleStoppingPoint(Active);
            }
            else
            {
                PlayTimeline(Active);
            }
        }

        /// <summary>
        /// When active is true, plays the timeline to the time specified by _activeTime
        /// When active is false, and if the timeline has been played, Resumes to play to the end
        ///
        /// An example usage could be a door Timeline that contains a 1 second open animation and a 1 second close animation sequentially
        /// By specifying _activeTime as 1 second, when the state becomes true the timeline will play up to the end of the door open clip
        /// When it becomes false the timeline will play to the end of the door closed clip.
        ///
        /// This is preferable to playing the timeline backwards
        /// programatically as Audio tracks would not play
        /// </summary>
        private void PlayTimlineUsingMiddleStoppingPoint(bool toActiveTime)
        {
            var assetDuration = _playableDirector.playableAsset.duration;

            // set the duration of the playable director to the mddle or end
            _playableDirector.playableGraph.GetRootPlayable(0).SetDuration(toActiveTime ? _activeTime : assetDuration);

            // if we want to play to the middle but we're at the end reset the time to 0
            if (toActiveTime && _playableDirector.time >= assetDuration - double.Epsilon)
            {
                _playableDirector.time = 0;
            }

            if (!toActiveTime && _playableDirector.time < double.Epsilon)
            {
                // we wanted to go to the end and we were at the start,
                // Evaluate without playing (assumes the timeline is a loop i.e. start and end look the same)
                _playableDirector.Evaluate();
            }
            else
            {
                _playableDirector.Pause();
                _playableDirector.Play();
            }
        }

        /// <summary>
        /// Plays the timeline without using a middle stopping point
        /// </summary>
        private void PlayTimeline(bool play)
        {
            if (play)
            {
                if (_playableDirector.time >= _playableDirector.duration - double.Epsilon)
                {
                    _playableDirector.time = 0;
                }
                _playableDirector.Play();
            }
        }
    }
}
