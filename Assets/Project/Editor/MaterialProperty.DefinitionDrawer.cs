// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEditor;

namespace Oculus.Interaction.ComprehensiveSample
{
    [CustomPropertyDrawer(typeof(MaterialProperty.Definition))]
    class DefinitionDrawer : PropertyDrawer
    {
        private static float singleLineHeight => EditorGUIUtility.singleLineHeight;
        private static float spacing => EditorGUIUtility.standardVerticalSpacing;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => (singleLineHeight * 3) + (spacing * 2);

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent _)
        {
            var propProp = property.FindPropertyRelative("_property");
            EditorGUI.PropertyField(rect.TakeTop(singleLineHeight, out rect, spacing), propProp);

            var typeProp = property.FindPropertyRelative("_type");
            EditorGUI.PropertyField(rect.TakeTop(singleLineHeight, out rect, spacing), typeProp);

            if (!typeProp.hasMultipleDifferentValues)
            {
                MaterialProperty.Type type = (MaterialProperty.Type)typeProp.enumValueIndex;
                ClearTextureReference(property, type);

                var valueProp = property.FindPropertyRelative(ValuePropName(type));

                if (type != MaterialProperty.Type.Vector)
                {
                    EditorGUI.PropertyField(rect, valueProp, true);
                }
                else
                {
                    // PropertyField doesn't draw Vectors nicely
                    GUIContent label = new GUIContent(valueProp.displayName, valueProp.tooltip);
                    valueProp.vector4Value = EditorGUI.Vector4Field(rect, label, valueProp.vector4Value);
                }
            }
        }

        /// <summary>
        /// Clears the texture reference if the property type isnt 'texture' to prevent textures getting included in builds
        /// </summary>
        private static void ClearTextureReference(SerializedProperty property, MaterialProperty.Type type)
        {
            if (type == MaterialProperty.Type.Texture) { return; }
            property.FindPropertyRelative(ValuePropName(MaterialProperty.Type.Texture)).objectReferenceValue = null;
        }

        private static string ValuePropName(MaterialProperty.Type type)
        {
            switch (type)
            {
                case MaterialProperty.Type.Color: return "_colorValue";
                case MaterialProperty.Type.Float: return "_floatValue";
                case MaterialProperty.Type.Vector: return "_vectorValue";
                case MaterialProperty.Type.Texture: return "_textureValue";
                default: throw new System.Exception($"Can't handle material property type {type}");
            }
        }
    }
}
