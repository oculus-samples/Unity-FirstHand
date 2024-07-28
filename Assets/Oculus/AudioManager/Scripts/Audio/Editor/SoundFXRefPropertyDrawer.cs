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

    SoundFXRefPropertyDrawer

    -----------------------
    */
    [CustomPropertyDrawer(typeof(SoundFXRef))]
    public class SoundFXRefPropertyDrawer : PropertyDrawer
    {

        static private GUIStyle disabledStyle = null;

        /*
        -----------------------
        OnGUI()
        -----------------------
        */
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            int idx = 0;
            Rect buttonPosition = position;
            buttonPosition.x = position.x + position.width - 40f;
            buttonPosition.width = 20f;
            position.width = buttonPosition.x - position.x - 2f;
            SerializedProperty nameProp = prop.FindPropertyRelative("soundFXName");
            if (AudioManager.GetGameObject() == null)
            {
                if (disabledStyle == null)
                {
                    disabledStyle = new GUIStyle();
                    disabledStyle.normal.textColor = Color.gray;
                }
                EditorGUI.LabelField(position, label.text, nameProp.stringValue, disabledStyle);
            }
            else
            {
                string[] soundFXNames = AudioManager.GetSoundFXNames(nameProp.stringValue, out idx);

                idx = EditorGUI.Popup(position, label.text, idx, soundFXNames);
                nameProp.stringValue = AudioManager.NameMinusGroup(soundFXNames[idx]);
                // play button
                if (GUI.Button(buttonPosition, "\u25BA"))
                {
                    if (AudioManager.IsSoundPlaying(nameProp.stringValue))
                    {
                        AudioManager.StopSound(nameProp.stringValue);
                    }
                    else
                    {
                        AudioManager.PlaySound(nameProp.stringValue);
                    }
                }
                buttonPosition.x += 22.0f;
                // select audio manager
                if (GUI.Button(buttonPosition, "\u2630"))
                {
                    Selection.activeGameObject = AudioManager.GetGameObject();
                }

            }
        }
    }

} // namespace OVR
