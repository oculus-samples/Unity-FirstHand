// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Displays the app version
    /// </summary>
    public class VersionTextSetter : MonoBehaviour
    {
        void Start()
        {
            var text = GetComponent<TMPro.TextMeshProUGUI>();
            text.text = Application.version;
        }
    }
}
