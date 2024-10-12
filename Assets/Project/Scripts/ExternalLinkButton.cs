// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Opens an external link when called
    /// </summary>
    public class ExternalLinkButton : MonoBehaviour
    {
        public string url;

        /// <summary>
        /// Called in Unity Event / Button
        /// </summary>
        public void OpenURL() => Application.OpenURL(url);
    }
}
