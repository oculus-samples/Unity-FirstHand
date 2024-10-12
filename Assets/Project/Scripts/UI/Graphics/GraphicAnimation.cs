// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Loops through graphic animations
    /// </summary>
    public class GraphicAnimation : MonoBehaviour
    {
        [SerializeField]
        private List<Frame> _frames;
        [SerializeField]
        private bool _loop;
        [SerializeField]
        private bool _timescaleIndependent;

        private void OnEnable()
        {
            StartCoroutine(AnimationRoutine());
            IEnumerator AnimationRoutine()
            {
                var graphic = GetComponent<Graphic>();
                int frameIndex = 0;
                while (_loop || frameIndex < _frames.Count)
                {
                    Frame frame = _frames[frameIndex++ % _frames.Count];

                    if (graphic.IsRenderable())
                    {
                        graphic.SetSprite(frame.sprite);
                    }

                    var nextFramee = GetTime() + frame.duration;
                    do yield return null;
                    while (GetTime() < nextFramee);
                }

                float GetTime() => _timescaleIndependent ? Time.unscaledTime : Time.time;
            }
        }

        [Serializable]
        struct Frame
        {
            public Sprite sprite;
            public float duration;
        }
    }
}
