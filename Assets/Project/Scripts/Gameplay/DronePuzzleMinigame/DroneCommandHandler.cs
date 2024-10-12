// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class DroneCommandHandler : MonoBehaviour
    {
        public static DroneCommandHandler Instance { get; private set; }

        public DroneCommand _droneCommand;

        public List<DroneCommandSubject> _subjects = new List<DroneCommandSubject>();


        private bool _isBusyOverride;
        readonly List<DroneCommand> _commands = new List<DroneCommand>();

        public event Action WhenCommandsChanged;

        public bool IsBusy => _droneCommand != null || _isBusyOverride;

        private void Awake() => Instance = this;

        void Start()
        {
            _subjects.ForEach(x => x.WhenCommandsChanged += UpdateCommands);
        }

        private void UpdateCommands()
        {
            _commands.Clear();
            _subjects.ForEach(x => x.AddAvailableCommands(_commands));
            WhenCommandsChanged?.Invoke();
        }

        public bool PerformCommand(string subjectID, DroneCommandPreset command)
        {
            if (command == null) return false;

            var subject = _subjects.Find(x => x.Id == subjectID && x.HasAvailableCommand(command));
            if (!subject)
            {
                Debug.LogError($"Couldnt find {subjectID}, with command {command}");
                return false;
            }

            return PerformCommand(command, subject);
        }

        public bool PerformCommand(DroneCommand command) => PerformCommand(command.Command, command.Subject);

        public bool PerformCommand(DroneCommandPreset command, DroneCommandSubject subject)
        {
            bool success = subject && subject.TryPerformCommand(command);
            return success;
        }

        public void GetCommands(List<DroneCommand> commands)
        {
            commands.Clear();
            commands.AddRange(_commands);
        }

        public void SetBusy(float v)
        {
            _isBusyOverride = true;
            TweenRunner.DelayedCall(v, () => _isBusyOverride = false);
        }

        internal bool HasCommands()
        {
            return _commands.Count > 0;
        }
    }
}
