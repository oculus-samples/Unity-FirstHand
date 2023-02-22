/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
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
