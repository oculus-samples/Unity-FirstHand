// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Clears a RenderTexture on enable
    /// This is useful for VideoPlayers that write into a texture, since usually it takes a moment to render the first frame of the video and shows black otherwise
    /// </summary>
    public class RenderTextureClearColor : MonoBehaviour
    {
        [SerializeField]
        RenderTexture _renderTexture;

        void OnEnable() => ClearOutRenderTexture(_renderTexture);

        void ClearOutRenderTexture(RenderTexture renderTexture)
        {
            RenderTexture rt = RenderTexture.active;
            RenderTexture.active = renderTexture;
            GL.Clear(true, true, Color.clear);
            RenderTexture.active = rt;
        }
    }
}
