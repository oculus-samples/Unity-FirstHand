// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Plays a timeline in steps as ActiveStates become true
    /// </summary>
    public class ActiveStateTimelineSequence : MonoBehaviour
    {
        [SerializeField]
        private PlayableDirector _playableDirector;

        [SerializeField, Tooltip("When true the timeline will jump to the highest active time in Start")]
        private bool _skipOnLoad = true;

        [SerializeField]
        private List<ActiveStateTime> _activeStateTimes = new List<ActiveStateTime>();

        [SerializeField, Tooltip("Special case, used to not skip the first segment on load")]
        private int _skipOnLoadFrom = 0;

        [SerializeField]
        private bool _fixControlTracksInStart = false;

        [SerializeField, Tooltip("Determines if the playable director should pause to save performance, or simply hold")]
        private HoldMode _holdMode = HoldMode.Pause;

        public PlayableDirector PlayableDirector => _playableDirector;
        public List<ActiveStateTime> ActiveStateTimes => _activeStateTimes;

        private int _currentIndex = -1;

        private IEnumerator Start()
        {
            yield return null;
            if (_skipOnLoad)
            {
                UpdateStepIndex();

                if (_currentIndex >= _skipOnLoadFrom)
                {
                    SetDirectorTime(_activeStateTimes[_currentIndex].Time);
                }
            }

            if (_fixControlTracksInStart)
            {
                FixControlTracksStart(_playableDirector);
            }
        }

        private void OnEnable()
        {
            if (_currentIndex >= 0)
            {
                PlayStep(_activeStateTimes[_currentIndex]);
            }
        }

        private void LateUpdate()
        {
            UpdateStepIndex();
            FixSkipsAndLoops();
        }

        private void UpdateStepIndex()
        {
            int index = GetHighestActiveStepIndex();

            if (index < 0 || index == _currentIndex)
                return;

            _currentIndex = index;

            PlayStep(_activeStateTimes[_currentIndex]);
        }

        /// <summary>
        /// Detects if the current ActiveStateTime should loop and sets its time back to the start of the loop
        /// This is preferable to WrapMod.Loop since that always goes to the start
        /// </summary>
        private void FixSkipsAndLoops()
        {
            if (_currentIndex < 0) return;

            TrySkipToCurrentStep();

            var step = _activeStateTimes[_currentIndex];
            var stepEndTime = ClampToAssetDuration(step.Time);

            if (step.WrapMode == DirectorWrapMode.Loop && _playableDirector.time >= stepEndTime)
            {
                var loopStartTime = _currentIndex > 0 ? _activeStateTimes[_currentIndex - 1].Time : 0;
                SetDirectorTime(loopStartTime);
            }
            if (step.WrapMode == DirectorWrapMode.Hold && _holdMode == HoldMode.Pause && _playableDirector.time >= stepEndTime)
            {
                _playableDirector.Pause();
            }
        }

        private void TrySkipToCurrentStep()
        {
            // find which section we're currently playing based on the director time
            // this can be different to the current 'active' index, because it takes time for the playhead to reach the active step
            var playheadStepIndex = GetCurrentlyPlayingIndex();

            // we're up to date, do nothing
            if (playheadStepIndex < 0 || playheadStepIndex >= _currentIndex) return;

            // if the playhead is not caught up to the current step, check if the playhead is in a skippable step
            var playheadStep = _activeStateTimes[playheadStepIndex];
            if (!playheadStep.Skippable) return;

            // Special case when the current step and the playhead step have the same ActiveState.
            // Using the same ActiveState is commonly used to to play one long section then loop a short section.
            // In this case we prevent skipping, as skipping would always skip the long section
            if (playheadStep.IsSameActiveState(_activeStateTimes[_currentIndex]))
            {
                return;
            }

            // all good to skip!
            SetDirectorTime(playheadStep.Time + float.Epsilon);
        }

        /// <summary>
        /// Returns the index of the last sequential ActiveStateTime thats Active
        /// </summary>
        private int GetHighestActiveStepIndex()
        {
            int lastIndex = _activeStateTimes.Count - 1;
            for (int i = 0; i <= lastIndex; i++)
            {
                ActiveStateTime activeStateTime = _activeStateTimes[i];
                if (!activeStateTime.Active)
                {
                    var result = i - 1;
                    return result;
                }
            }
            return lastIndex;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public int GetCurrentlyPlayingIndex()
        {
            var directorTime = _playableDirector.time;
            for (int i = 0; i < _activeStateTimes.Count; i++)
            {
                if (_activeStateTimes[i].Time > directorTime)
                {
                    return i;
                }
            }
            return -1;
        }

        // called by UnityEvent
        public void ForceSkip()
        {
            UpdateStepIndex();
            SetDirectorTime(_activeStateTimes[_currentIndex].Time);
        }

        private void PlayStep(ActiveStateTime step)
        {
            if (!_playableDirector.playableGraph.IsValid())
            {
                _playableDirector.RebuildGraph();
            }

            // set the duration of the playable director to the mddle or end
            _playableDirector.playableGraph.GetRootPlayable(0).SetDuration(ClampToAssetDuration(step.Time));
            _playableDirector.extrapolationMode = DirectorWrapMode.Hold; // we manually loop in LateUpdate
            _playableDirector.Play();
        }

        private double ClampToAssetDuration(double time) => Math.Min(_playableDirector.playableAsset.GetDurationFast() - float.Epsilon, time);

        private void SetDirectorTime(double time)
        {
            var timeBefore = _playableDirector.time;
            var clampedTime = ClampToAssetDuration(time);

            _playableDirector.time = clampedTime;
            FixControlTracks(_playableDirector, timeBefore, clampedTime);
        }

        private static void FixControlTracksStart(PlayableDirector director)
        {
            var timeline = (TimelineAsset)director.playableAsset;
            if (!timeline) return;

            foreach (var output in timeline.outputs)
            {
                // do nothing on tracks that are not control tracks
                if (output.sourceObject is not ControlTrack controlTrack) continue;

                var clips = new List<TimelineClip>(controlTrack.GetClips());
                clips.Sort((x, y) => x.start.CompareTo(y.start));
                for (int i = clips.Count - 1; i >= 0; i--)
                {
                    var clip = clips[i];

                    // do nothing on clips that are not control clips or dont control directors
                    if (clip.asset is not ControlPlayableAsset controlClip || !controlClip.updateDirector) continue;

                    // try get the PlayableDirector, do nothing if there isnt one
                    var controlObject = controlClip.sourceGameObject.Resolve(director);
                    if (!controlObject) continue;
                    if (!controlObject.TryGetComponent<PlayableDirector>(out var controlDirector)) continue;

                    // work out what time the end of the clip sets the director to
                    var startTime = clip.clipIn;
                    var assetEndTime = controlDirector.playableAsset.GetDurationFast();
                    var clipEndTime = clip.duration + clip.clipIn;
                    var endTime = Math.Min(assetEndTime, clipEndTime);

                    var posInClip = Mathf.InverseLerp((float)clip.start, (float)clip.end, (float)director.time);
                    var posInTimeline = Mathf.Lerp((float)startTime, (float)endTime, posInClip);

                    bool clipControlsActiveState = controlClip.active;
                    if (clipControlsActiveState) controlObject.SetActive(true);

                    if (!controlDirector.playableGraph.IsValid()) controlDirector.RebuildGraph();

                    controlDirector.time = posInTimeline;

                    using (AudioTriggerITimeControl.Mute())
                    {
                        controlDirector.Evaluate();
                    }

                    if (clipControlsActiveState) controlObject.SetActive(false);

                    // the nested director may itself have nested directors, so fix those too
                    FixControlTracksStart(controlDirector);
                }
            }
        }

        private static void FixControlTracks(PlayableDirector director, double timeBefore, double timeAfter, bool requireSkip = true)
        {
            var timeline = (TimelineAsset)director.playableAsset;
            if (!timeline) return;

            foreach (var output in timeline.outputs)
            {
                // do nothing on tracks that are not control tracks
                if (output.sourceObject is not ControlTrack controlTrack) continue;

                foreach (var clip in controlTrack.GetClips())
                {
                    // do nothing on clips that are not control clips or dont control directors
                    if (clip.asset is not ControlPlayableAsset controlClip || !controlClip.updateDirector) continue;

                    // do nothing if we've not skipped past the end of the clip
                    var hasBeenSkipped = timeBefore < clip.end && timeAfter > clip.end;
                    if (!hasBeenSkipped && requireSkip) continue;

                    // try get the PlayableDirector, do nothing if there isnt one
                    var controlObject = controlClip.sourceGameObject.Resolve(director);
                    if (!controlObject || !controlObject.TryGetComponent<PlayableDirector>(out var controlDirector)) continue;

                    // work out what time the end of the clip sets the director to
                    var currentTime = controlDirector.time;
                    var assetEndTime = controlDirector.playableAsset.GetDurationFast();
                    var clipEndTime = clip.duration + clip.clipIn;
                    var endTime = Math.Min(assetEndTime, clipEndTime);

                    bool clipControlsActiveState = controlClip.active;
                    if (clipControlsActiveState) controlObject.SetActive(true);

                    if (!controlDirector.playableGraph.IsValid()) controlDirector.RebuildGraph();

                    controlDirector.time = endTime;
                    controlDirector.Evaluate();

                    if (clipControlsActiveState) controlObject.SetActive(false);

                    // the nested director may itself have nested directors, so fix those too
                    FixControlTracks(controlDirector, currentTime, endTime);
                }
            }
        }

        enum HoldMode
        {
            /// <summary>
            /// The timeline is paused when a hold point is reached. This is better performance as it stops evaluating
            /// </summary>
            Pause,
            /// <summary>
            /// The timeline holds when a hold point is reached, it will continue to Evaluate
            /// </summary>
            Hold
        }
    }

    [Serializable]
    public struct ActiveStateTime
    {
        [SerializeField]
        private string _name;
        [SerializeField]
        private ReferenceActiveState _activeStateRef;
        [SerializeField]
        private double _time;
        [SerializeField]
        private DirectorWrapMode _wrapMode;
        [SerializeField]
        private bool _skippable;

        public ActiveStateTime(string name, ReferenceActiveState activeStateRef = default, double time = 0, DirectorWrapMode wrapMode = default, bool skippable = default)
        {
            _name = name;
            _activeStateRef = activeStateRef;
            _time = time;
            _wrapMode = wrapMode;
            _skippable = skippable;
        }

        public double Time
        {
            get => _time;
            set => _time = value;
        }

        public DirectorWrapMode WrapMode => _wrapMode;
        public bool Skippable => _skippable;
        public bool Active => _activeStateRef.Active;
        public string Name => _name;

        public bool IsSameActiveState(ActiveStateTime other)
        {
            return _activeStateRef.ReferenceEquals(other._activeStateRef);
        }
    }
}
