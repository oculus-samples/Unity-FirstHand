// Copyright (c) Meta Platforms, Inc. and affiliates.

/*
 *Copyright(c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
*you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 *Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace Oculus.Interaction.ComprehensiveSample
{
    public static class GraphicHelpers
    {
        private static int _mainTexProperty = Shader.PropertyToID("_MainTex");
        private static Dictionary<Type, PropertyInfo> _propCache = new Dictionary<Type, PropertyInfo>();

        public static void SetSprite(this Graphic graphic, Sprite sprite)
        {
            if (!CanSet(graphic)) { return; }

            switch (graphic)
            {
                case Image image: image.sprite = sprite; break;
                case Rectangle rectangle: rectangle.sprite = sprite; break;
                default: SetSpriteViaReflection(graphic, sprite); break;
            }
        }

        private static bool CanSet(Graphic graphic)
        {
            if (graphic == null) { return false; }

            Material material = graphic.material ? graphic.material : graphic.defaultMaterial ? graphic.defaultMaterial : graphic.materialForRendering;
            return material && material.HasProperty(_mainTexProperty);
        }

        public static bool IsRenderable(this Graphic graphic)
        {
            return !graphic.canvasRenderer.cull &&
                graphic.color.a > 0 &&
                graphic.canvasRenderer.GetAlpha() > 0 &&
                graphic.canvasRenderer.GetInheritedAlpha() > 0;
        }

        private static void SetSpriteViaReflection(Graphic instance, Sprite value)
        {
            if (TryGetSpriteProp(instance, out var setter))
            {
                setter.SetValue(instance, value);
            }
        }

        private static Sprite GetSpriteViaReflection(object instance)
        {
            if (TryGetSpriteProp(instance, out var getter))
            {
                return getter.GetValue(instance) as Sprite;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns true and a PropertyInfo for sprite on the specified type if one exists, uses a cache
        /// </summary>
        private static bool TryGetSpriteProp(object instance, out PropertyInfo prop)
        {
            if (instance == null) { prop = null; return false; }

            Type type = instance.GetType();
            if (!_propCache.ContainsKey(type))
            {
                PropertyInfo property = GetCompatableProp(type, "sprite") ?? GetCompatableProp(type, "Sprite");
                _propCache.Add(type, property);
            }
            prop = _propCache[type];
            return prop != null;
        }

        /// <summary>
        /// Returns a PropertyInfo of the type with the name that has get and set, otherwise null
        /// </summary>
        private static PropertyInfo GetCompatableProp(Type type, string name)
        {
            PropertyInfo property = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            bool isGoodProp = property != null && property.CanRead && property.CanWrite && typeof(Sprite).IsAssignableFrom(property.PropertyType);
            return isGoodProp ? property : null;
        }
    }
}
