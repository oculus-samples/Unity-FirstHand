// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// pre-warms shaders
    /// </summary>
    public class ShaderPreWarm : MonoBehaviour
    {
        [SerializeField]
        ShaderVariantCollection _variantCollection;

        void Awake()
        {
            _variantCollection.WarmUp();
        }
    }
}
