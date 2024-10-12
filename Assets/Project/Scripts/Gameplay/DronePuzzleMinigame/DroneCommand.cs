// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class DroneCommand
    {
        private DroneCommandPreset _command;
        private DroneCommandSubject _subject;
        private string _customLabelString;
        private Sprite _customSprite;

        public DroneCommandPreset Command => _command;
        public DroneCommandSubject Subject => _subject;

        public override string ToString() => string.IsNullOrEmpty(_customLabelString) ? $"\"{_command.Command} {_subject.DisplayName}\"" : _customLabelString;

        public Sprite Icon => _customSprite ?? Command.Icon;

        public DroneCommand(DroneCommandPreset command, DroneCommandSubject subject)
        {
            _command = command;
            _subject = subject;
        }

        public DroneCommand(DroneCommandPreset command, DroneCommandSubject subject, string customLabelString, Sprite customSprite = null) : this(command, subject)
        {
            _customLabelString = customLabelString;
            _customSprite = customSprite;
        }

        public void Perform() => _subject.TryPerformCommand(_command);
    }
}
