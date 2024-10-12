// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class DirectorTimeActiveState : MonoBehaviour, IActiveState
    {
        [SerializeField]
        private PlayableDirector _playableDirector;

        [SerializeField]
        private ActiveStateExpectation _playing = ActiveStateExpectation.Any;

        [SerializeField,
            Tooltip("a custom time range, when the playable director's time " +
            "is in range this active state returns true. Set to infinity to check a timelines finished")]
        private FloatRange _timeRange;

        [SerializeField, Optional, Tooltip("When valid, at runtime populates the range from a marker name")]
        private string _markerName;

        public bool Active
        {
            get
            {
                if (!_playing.Matches(_playableDirector.state == PlayState.Playing))
                {
                    return false;
                }

                if (Application.isPlaying && !string.IsNullOrEmpty(_markerName))
                {
                    var time = MarkerTrack.GetTimeRange(_playableDirector, _markerName);
                    _timeRange.Min = (float)time.Start;
                    _timeRange.Max = (float)(time.Start + time.Duration);
                    _markerName = null;
                }

                if (_timeRange.IsPositiveInfinity)
                {
                    var time = _playableDirector.time;
                    var duration = _playableDirector.playableAsset.GetDurationFast();
                    return time >= duration - double.Epsilon;
                }

                return _timeRange.Contains((float)_playableDirector.time);
            }
        }
    }

    public static class TimelineExtensions
    {
        private static Dictionary<int, double> _durations = new Dictionary<int, double>();

        public static double GetDurationFast(this PlayableAsset playableAsset)
        {
            double duration;
            var id = playableAsset.GetInstanceID();
            if (!_durations.TryGetValue(id, out duration))
            {
                duration = playableAsset.duration;
                _durations[id] = duration;
            }
            return duration;
        }
    }
}
