// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class DroneCommandUI : MonoBehaviour
    {
        [SerializeField]
        private UIList _contentUIList;
        [SerializeField]
        private UICanvas _commandsCanvas;
        [SerializeField]
        private HorizontalLayoutGroup _layoutGroup;

        private List<DroneCommand> _commands = new List<DroneCommand>();

        Task<bool> _fadeUITask;

        private void Start()
        {
            DroneCommandHandler.Instance.WhenCommandsChanged += UpdateUI;
        }

        private async void UpdateUI()
        {
            if (_fadeUITask != null && !_fadeUITask.IsCompleted) return;

            _fadeUITask = _commandsCanvas.ShowAsync(false);

            if (await _fadeUITask)
            {
                DroneCommandHandler.Instance.GetCommands(_commands);
                _contentUIList.SetList(_commands, true);
                FixLayoutGroup();

                await _commandsCanvas.ShowAsync(true);
            }
        }

        private void FixLayoutGroup()
        {
            _layoutGroup.enabled = false;
            _layoutGroup.enabled = true;
        }
    }
}
