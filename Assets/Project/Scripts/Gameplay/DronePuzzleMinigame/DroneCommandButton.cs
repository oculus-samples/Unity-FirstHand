// Copyright (c) Meta Platforms, Inc. and affiliates.

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Oculus.Interaction.ComprehensiveSample
{
    [RequireComponent(typeof(Button))]
    public class DroneCommandButton : MonoBehaviour, IUIListElementHandler
    {
        [SerializeField]
        private Image _icon;
        [SerializeField]
        private TextMeshProUGUI _description;

        private DroneCommand _command;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(PerformCommand);
        }

        private void PerformCommand() => DroneCommandHandler.Instance.PerformCommand(_command);

        public void SetCommand(DroneCommand command)
        {
            _command = command;
            _icon.sprite = command.Command.Icon;
            _description.SetText(command.ToString());
        }

        void IUIListElementHandler.HandleListElement(object element)
        {
            if (element is DroneCommand command) SetCommand(command);
        }
    }
}
