// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class MarkerTrack : PlayableBehaviour
    {
        public static TimeRange GetTimeRange(PlayableDirector director, string name) => GetTimeRange(director.playableAsset as TimelineAsset, name);

        public static TimeRange GetTimeRange(TimelineAsset timeline, string name)
        {
            foreach (var track in timeline.GetOutputTracks())
            {
                if (!IsValid(track)) continue;

                foreach (var clip in track.GetClips())
                {
                    if (clip.asset is MarkerTrackAsset && clip.displayName == name)
                    {
                        return new TimeRange(clip.start, clip.end, clip.displayName);
                    }
                }
            }
            throw new System.Exception($"Couldnt find {name}");
        }

        public static List<TimeRange> GetTimeRanges(PlayableDirector director) => GetTimeRanges(director.playableAsset as TimelineAsset);

        public static List<TimeRange> GetTimeRanges(TimelineAsset timeline)
        {
            var result = new List<TimeRange>();
            foreach (var track in timeline.GetOutputTracks())
            {
                if (!IsValid(track)) continue;

                foreach (var clip in track.GetClips())
                {
                    if (clip.asset is MarkerTrackAsset)
                    {
                        result.Add(new TimeRange(clip.start, clip.end, clip.displayName));
                    }
                }
            }
            result.Sort((x, y) => x.Start.CompareTo(y.Start));
            return result;
        }

        private static bool IsValid(TrackAsset track)
        {
            return track.hasClips && track is PlayableTrack;
        }
    }

    public struct TimeRange
    {
        public double Start;
        public double End;
        public string TimelineName;
        public double Duration => End - Start;

        public TimeRange(double start, double end, string name)
        {
            Start = start;
            End = end;
            TimelineName = name;
        }

        public double Lerp(float t)
        {
            return Start + Duration * t;
        }

        public bool Contains(float t) => Start <= t && t <= End;

        public override string ToString()
        {
            return $"({Start}-{End})";
        }
    }
}
