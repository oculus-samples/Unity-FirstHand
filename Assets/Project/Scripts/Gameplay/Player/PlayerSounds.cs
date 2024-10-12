// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Interaction.Locomotion;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Oculus.Interaction.Locomotion.LocomotionEvent;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Plays audio based on locomotion events, uses different sounds for landing on different surfaces
    /// </summary>
    public class PlayerSounds : MonoBehaviour
    {
        [SerializeField]
        private PlayerLocomotor _playerLocomotor;

        [Header("Base")]
        [SerializeField]
        private AudioTrigger _teleport;
        [SerializeField]
        private AudioTrigger _leftTurn;
        [SerializeField]
        private AudioTrigger _rightTurn;

        [Header("Surfaces")]
        [SerializeField]
        private float _longDistanceThreshold = 3;
        [SerializeField]
        private float _fallDistanceTheshold = 2;
        [SerializeField]
        private SurfaceSounds _defaultSurfaceSounds;
        [SerializeField]
        private List<MaterialSoundPair> _surfacePairs = new List<MaterialSoundPair>();

        public static bool SkipNext { get; set; }

        private void Start()
        {
            _playerLocomotor.WhenLocomotionEventHandled += HandleLocomotionEvent;
        }

        private void HandleLocomotionEvent(LocomotionEvent locomotionEvent, Pose delta)
        {
            if (SkipNext)
            {
                SkipNext = false;
                return;
            }

            if (locomotionEvent.IsSnapTurn())
            {
                HandleTurn(delta.rotation);
            }

            if (locomotionEvent.IsTeleport())
            {
                HandleMove(delta.position);
            }
        }

        private void HandleTurn(Quaternion quaternion)
        {
            var direction = Mathf.Sign(quaternion.eulerAngles.y);
            if (direction == -1) _leftTurn.Play();
            if (direction == 1) _rightTurn.Play();
        }

        private void HandleMove(Vector3 movement)
        {
            _teleport.Play();

            var surfaceSounds = GetSurfaceSoundsAtPosition(transform.position + Vector3.up * 0.1f);

            var audio = GetAudioForDelta(surfaceSounds, movement);
            if (audio == null) audio = GetAudioForDelta(_defaultSurfaceSounds, movement);

            audio.transform.position = transform.position;
            audio.Play();
        }

        private SurfaceSounds GetSurfaceSoundsAtPosition(Vector3 position)
        {
            if (Physics.Raycast(position, Vector3.down, out var hit))
            {
                if (hit.collider.TryGetComponent<SurfaceType>(out var surfaceType))
                {
                    var index = _surfacePairs.FindIndex(x => x.SurfaceTag == surfaceType.Preset);
                    if (index != -1)
                    {
                        return _surfacePairs[index].SurfaceSounds;
                    }
                }
            }
            return _defaultSurfaceSounds;
        }

        private AudioTrigger GetAudioForDelta(SurfaceSounds sounds, Vector3 delta)
        {
            if (delta.y < -_fallDistanceTheshold) return sounds.Fall;
            if (delta.sqrMagnitude > _longDistanceThreshold * _longDistanceThreshold) return sounds.Long;
            return sounds.Short;
        }

        [Serializable]
        struct MaterialSoundPair
        {
            public SurfaceTag SurfaceTag;
            public SurfaceSounds SurfaceSounds;
        }

        [Serializable]
        struct SurfaceSounds
        {
            public AudioTrigger Short;
            public AudioTrigger Long;
            public AudioTrigger Fall;
        }
    }

    public static class LocomotionEventExtensions
    {
        const TranslationType _teleportTypes = TranslationType.Absolute | TranslationType.AbsoluteEyeLevel | TranslationType.Relative;
        const RotationType _snapTypes = RotationType.Absolute | RotationType.Relative;

        public static bool IsTeleport(this LocomotionEvent locomotionEvent)
        {
            return (locomotionEvent.Translation & _teleportTypes) != 0;
        }

        public static bool IsSnapTurn(this LocomotionEvent locomotionEvent)
        {
            return (locomotionEvent.Rotation & _snapTypes) != 0;
        }
    }
}
