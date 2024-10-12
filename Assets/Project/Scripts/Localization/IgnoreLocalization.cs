// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Ignores localization on component added
    /// </summary>
    public class IgnoreLocalization : MonoBehaviour
    {
        public static bool ShouldIgnore(TMPro.TMP_Text text)
        {
            var value = text.text;
            if (string.IsNullOrWhiteSpace(value)) return true;

            if (value.Equals("example text", System.StringComparison.OrdinalIgnoreCase)) return true;
            if (value.Equals("placeholder text", System.StringComparison.OrdinalIgnoreCase)) return true;
            if (value.Equals("interaction sdk", System.StringComparison.OrdinalIgnoreCase)) return true;
            if (value.Equals("isdk", System.StringComparison.OrdinalIgnoreCase)) return true;

            return text.TryGetComponent<IgnoreLocalization>(out var _);
        }
    }
}
