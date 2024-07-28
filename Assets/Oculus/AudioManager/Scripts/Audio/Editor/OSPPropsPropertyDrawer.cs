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
using UnityEditor;
using System.Collections;

namespace OVR
{

    /*
    -----------------------

    OSPPropsPropertyDrawer

    -----------------------
    */
    [CustomPropertyDrawer(typeof(OSPProps))]
    public class OSPPropsPropertyDrawer : PropertyDrawer
    {

        static float lineHeight = EditorGUIUtility.singleLineHeight + 2.0f;
        static float indent = 32.0f;
        // TODO - some day just enumerate these
        static string[] props = new string[] { "useFastOverride", "gain", "enableInvSquare", "volumetric", "invSquareFalloff" };
        static string[] names = new string[] { "Reflections Enabled", "Gain", "Enable Oculus Atten.", "Volumetric", "Range" };
        static int[] lines = new int[] { 1, 1, 1, 1, 2, 2 };
        /*
        -----------------------
        OnGUI()
        -----------------------
        */
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            SerializedProperty playSpatializedProp = prop.FindPropertyRelative("enableSpatialization");
            position.height = lineHeight;
            EditorGUI.PropertyField(position, playSpatializedProp);
            if (playSpatializedProp.boolValue)
            {
                position.y += lineHeight + 4.0f;
                Rect posLine = position;
                posLine.x += indent;
                posLine.width -= indent;
                posLine.height = 1f;
                GUI.Box(posLine, "");
                position.y -= 10.0f;
                for (int i = 0; i < props.Length; i++)
                {
                    position.y += lineHeight;
                    position.height = (lineHeight * lines[i]);
                    SerializedProperty sibling = prop.FindPropertyRelative(props[i]);
                    EditorGUI.PropertyField(position, sibling, new GUIContent(names[i]));
                }
            }
        }

        /*
        -----------------------
        GetPropertyHeight()
        -----------------------
        */
        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            SerializedProperty playSpatializedProp = prop.FindPropertyRelative("enableSpatialization");
            if (!playSpatializedProp.boolValue)
            {
                return base.GetPropertyHeight(prop, label);
            }
            else
            {
                return base.GetPropertyHeight(prop, label) + (lineHeight * (props.Length + 1)) + 16.0f;
            }
        }

    }

} // namespace OVR
