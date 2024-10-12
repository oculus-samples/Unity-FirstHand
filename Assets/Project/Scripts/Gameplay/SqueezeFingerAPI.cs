// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Interaction.Input;
using Oculus.Interaction.PoseDetection;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class SqueezeFingerAPI : MonoBehaviour, IFingerUseAPI
    {
        [SerializeField, Interface(typeof(IHand))]
        private MonoBehaviour _hand;
        private IHand Hand { get; set; }

        private static readonly Vector2[] CURL_RANGE = new Vector2[Constants.NUM_FINGERS]
        {
            new Vector2(200f, 222f),
            new Vector2(185f, 261f),
            new Vector2(185f, 260f),
            new Vector2(185f, 261f),
            new Vector2(185f, 258f),
        };

        private FingerShapes _fingerShapes = new FingerShapes();
        private float[] _squeezePerFinger = new float[Constants.NUM_FINGERS];

        private int _lastDataVersion = -1;

        protected virtual void Awake()
        {
            Hand = _hand as IHand;
        }

        public float GetFingerUseStrength(HandFinger finger)
        {
            if (_lastDataVersion != Hand.CurrentDataVersion)
            {
                _lastDataVersion = Hand.CurrentDataVersion;
                UpdateStrength(Hand);
            }
            return _squeezePerFinger[(int)finger];
        }

        private void UpdateStrength(IHand hand)
        {
            for (int i = 0; i < Constants.NUM_FINGERS; i++)
            {
                HandFinger fingerID = (HandFinger)i;
                float curlAngle = _fingerShapes.GetCurlValue(fingerID, hand);
                if (fingerID != HandFinger.Thumb)
                {
                    curlAngle = (curlAngle * 2 + _fingerShapes.GetFlexionValue(fingerID, hand)) / 3f;
                }

                Vector2 range = new Vector2(CURL_RANGE[i].x, CURL_RANGE[i].y - CURL_RANGE[i].x);
                _squeezePerFinger[i] = Mathf.Clamp01((curlAngle - range.x) / range.y);
            }
        }
    }
}
