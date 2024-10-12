// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.Rendering.Universal.ShaderGraph;
using UnityEditor.ShaderGraph;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Modifies the blend modes of URP alpha and alpha premultipled shaders so they composite properly with the passthrough layer
    /// </summary>
    class MixedRealityShaderBlendModeFix
    {
        [InitializeOnLoadMethod]
        static void FixShaders()
        {
            int count = 0;
            PropertyInfo propertyInfo = typeof(RenderStateCollection.Item).GetProperty("descriptor", BindingFlags.Instance | BindingFlags.Public);
            FieldInfo backingField = GetBackingField(propertyInfo);

            foreach (RenderStateCollection.Item renderState in CoreRenderStates.Default)
            {
                if (count == 6) //Alpha blend
                {
                    backingField.SetValue(renderState, RenderState.Blend(Blend.SrcAlpha, Blend.OneMinusSrcAlpha, Blend.OneMinusDstAlpha, Blend.One));
                }
                if (count == 7) //Alpha premultiply
                {
                    backingField.SetValue(renderState, RenderState.Blend(Blend.One, Blend.OneMinusSrcAlpha, Blend.OneMinusDstAlpha, Blend.One));
                }
                count++;
            }
        }

        public static FieldInfo GetBackingField(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException(nameof(propertyInfo));
            if (!propertyInfo.CanRead || !propertyInfo.GetGetMethod(nonPublic: true).IsDefined(typeof(CompilerGeneratedAttribute), inherit: true))
                return null;
            var backingFieldName = GetBackingFieldName(propertyInfo.Name);
            var backingField = propertyInfo.DeclaringType?.GetField(backingFieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (backingField == null)
                return null;
            if (!backingField.IsDefined(typeof(CompilerGeneratedAttribute), inherit: true))
                return null;
            return backingField;
        }

        private static string GetBackingFieldName(string propertyName) => $"<{propertyName}>k__BackingField";
    }
}
