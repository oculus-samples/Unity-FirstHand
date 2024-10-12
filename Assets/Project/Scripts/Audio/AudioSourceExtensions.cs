// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Audio source extension to handle calculating
    /// sound volume at a distance
    /// </summary>
    public static class AudioSourceExtensions
    {
        public static float CalculateVolumeAtDistance(this AudioSource source, float distance, float volumeRolloffScale = 1)
        {
            var rolloff = CalculateRolloffMultipler(source, distance, volumeRolloffScale);
            return source.volume * Mathf.Lerp(1, rolloff, source.spatialBlend);
        }

        public static float CalculateRolloffMultipler(this AudioSource source, float distance, float volumeRolloffScale = 1)
        {
            switch (source.rolloffMode)
            {
                case AudioRolloffMode.Logarithmic:
                    return distance < source.maxDistance ? source.minDistance * (1f / (1f + volumeRolloffScale * (distance - 1))) : 0;
                case AudioRolloffMode.Linear:
                    return Mathf.InverseLerp(source.maxDistance, source.minDistance, distance);
                case AudioRolloffMode.Custom:
                    var curve = source.GetCustomCurve(AudioSourceCurveType.CustomRolloff);
                    return curve.Evaluate(Mathf.InverseLerp(source.minDistance, source.maxDistance, distance));
                default:
                    return 1;
            }
        }
    }
}
