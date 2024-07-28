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

    SoundFXPropertyDrawer

    -----------------------
    */
    [CustomPropertyDrawer(typeof(SoundFX))]
    public class SoundFXPropertyDrawer : PropertyDrawer
    {

        static float lineHeight = EditorGUIUtility.singleLineHeight + 2.0f;

        static string[] props = new string[] { "name", "playback", "volume", "pitchVariance", "falloffDistance", "falloffCurve", "reverbZoneMix", "spread", "pctChanceToPlay", "priority", "delay", "looping", "ospProps", "soundClips" };

        /*
        -----------------------
        OnGUI()
        -----------------------
        */
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {

            EditorGUILayout.BeginVertical();
            for (int i = 0; i < props.Length; i++)
            {
                EditorGUI.indentLevel = 2;
                SerializedProperty property = prop.FindPropertyRelative(props[i]);
                if (props[i] == "reverbZoneMix")
                {
                    EditorGUILayout.BeginHorizontal();
                    SerializedProperty reverbCurve = prop.FindPropertyRelative("reverbZoneMix");
                    EditorGUILayout.PropertyField(reverbCurve, true, GUILayout.Width(Screen.width - 130.0f));
                    if (GUILayout.Button("Reset", GUILayout.Width(50.0f)))
                    {
                        reverbCurve.animationCurveValue = new AnimationCurve(new Keyframe[2] { new Keyframe(0f, 1.0f), new Keyframe(1f, 1f) });
                    }
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.PropertyField(property, true, GUILayout.Width(Screen.width - 80.0f));
                    position.y += lineHeight + 4.0f;
                    if (props[i] == "falloffCurve")
                    {
                        if (property.enumValueIndex == (int)AudioRolloffMode.Custom)
                        {
                            EditorGUILayout.PropertyField(prop.FindPropertyRelative("volumeFalloffCurve"), true, GUILayout.Width(Screen.width - 80.0f));
                            position.y += lineHeight + 4.0f;
                        }
                    }
                }
            }
            EditorGUILayout.EndVertical();
            GUILayout.Space(5.0f);
        }

        /*
        -----------------------
        GetPropertyHeight()
        -----------------------
        */
        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            return base.GetPropertyHeight(prop, label);
        }

    }

} // namespace OVR
