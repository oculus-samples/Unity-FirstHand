// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    [CreateAssetMenu(fileName = "DroneCommand", menuName = "ScriptableObjects/Drone/DroneCommand")]
    public class DroneCommandPreset : ScriptableObject
    {
        [SerializeField]
        private Sprite _icon;
        public Sprite Icon => _icon;

        [SerializeField]
        private string _command;
        public string Command => _command;
    }
}
