/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Use of the material below is subject to the terms of the MIT License
 * https://github.com/oculus-samples/Unity-FirstHand/tree/main/Assets/Project/LICENSE.txt
 */

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Utility extensions for Unity's Material class
    /// </summary>
    public static class MaterialExtensions
    {
        /// <summary>
        /// <c>value</c> will be set and will return true if the material has the float property
        /// </summary>
        public static bool TryGetFloat(this Material material, string name, out float value)
        {
            return TryGetFloat(material, Shader.PropertyToID(name), out value);
        }

        /// <summary>
        /// <c>value</c> will be set and will return true if the material has the float property
        /// </summary>
        public static bool TryGetFloat(this Material material, int hash, out float value)
        {
            bool hasProperty = material.HasProperty(hash);
            value = hasProperty ? material.GetFloat(hash) : default;
            return hasProperty;
        }
    }
}
