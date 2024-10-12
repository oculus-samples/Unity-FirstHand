// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class DroneCommandSubject : MonoBehaviour
    {
        [SerializeField]
        private string _id;
        public string Id => _id;

        [SerializeField]
        private string _displayName;
        public string DisplayName => _displayName;

        public bool HasAvailableCommands => _actions.TrueForAny(x => x.Available);

        [SerializeField, FormerlySerializedAs("_presets")]
        private List<DroneSubjectAction> _actions = new List<DroneSubjectAction>();

        [SerializeField]
        private StringPropertyRef _locationProp;

        public event Action WhenCommandsChanged;

        private List<DroneCommand> _droneCommands;

        private void Awake()
        {
            _droneCommands = _actions.ConvertAll(x => new DroneCommand(x.Command, this, x.CustomLabel, x.CustomIcon));
        }

        private void Update()
        {
            bool changed = false;

            for (int i = 0; i < _actions.Count; i++)
            {
                var action = _actions[i];
                changed |= action.TryUpdateAvailable();
                _actions[i] = action;
            }

            if (changed)
            {
                Debug.Log("Commands changed", this);
                WhenCommandsChanged?.Invoke();
            }
        }

        public void AddAvailableCommands(List<DroneCommand> droneCommands)
        {
            if (!isActiveAndEnabled) return;

            for (int i = 0; i < _actions.Count; i++)
            {
                DroneSubjectAction action = _actions[i];
                if (action.Available)
                {
                    droneCommands.Add(_droneCommands[i]);
                }
            }
        }

        public bool TryPerformCommand(DroneCommandPreset command)
        {
            if (DroneCommandHandler.Instance.IsBusy) return false;

            if (TryGetAction(command, out var action, out var droneCommand))
            {
                float duration = 2;

                if (action.HasLocation)
                {
                    _locationProp.Value = action.Location;
                }

                if (action.HasTimeline)
                {
                    action.Timeline.Play();
                    duration = (float)action.Timeline.duration;
                }

                DroneCommandHandler.Instance._droneCommand = droneCommand;
                TweenRunner.DelayedCall(duration, () => DroneCommandHandler.Instance._droneCommand = null);

                return true;
            }

            return false;
        }

        private bool TryGetAction(DroneCommandPreset command, out DroneSubjectAction action, out DroneCommand commandSub)
        {
            for (int i = 0; i < _actions.Count; i++)
            {
                DroneSubjectAction a = _actions[i];
                if (a.Command == command && a.Available)
                {
                    action = a;
                    commandSub = _droneCommands[i];
                    return true;
                }
            }

            action = default;
            commandSub = default;
            return false;
        }

        public bool HasAvailableCommand(DroneCommandPreset preset) => TryGetAction(preset, out _, out _);

        [Serializable]
        public struct DroneSubjectAction
        {
            [FormerlySerializedAs("_activeState")]
            public ReferenceActiveState Available;
            [FormerlySerializedAs("_preset")]
            public DroneCommandPreset Command;
            public PlayableDirector Timeline;
            public string Location;
            public string CustomLabel;

            private bool _wasAvailable;

            public bool HasLocation => !string.IsNullOrEmpty(Location);
            public bool HasTimeline => Timeline != null;

            public Sprite CustomIcon;

            public bool TryUpdateAvailable()
            {
                ReferenceActiveState available = Available;
                if (_wasAvailable != available)
                {
                    _wasAvailable = available;
                    return true;
                }
                return false;
            }
        }
    }
}
