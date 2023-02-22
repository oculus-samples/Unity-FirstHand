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

using UnityEditor;
using UnityEngine;

namespace OVR
{

/*
-----------------------

 MinMaxPropertyDrawer

-----------------------
*/
[CustomPropertyDrawer (typeof (MinMaxAttribute))]
public class MinMaxPropertyDrawer : PropertyDrawer {

    // Provide easy access to the MinMaxAttribute for reading information from it.
    MinMaxAttribute minMax { get { return ((MinMaxAttribute)attribute); } }

    /*
    -----------------------
    GetPropertyHeight()
    -----------------------
    */
    public override float GetPropertyHeight( SerializedProperty prop, GUIContent label ) {
        return base.GetPropertyHeight( prop, label ) * 2f;
    }

    /*
    -----------------------
    OnGUI()
    -----------------------
    */
    public override void OnGUI( Rect position, SerializedProperty property, GUIContent label ) {
        Rect sliderPosition = EditorGUI.PrefixLabel( position, label );
        SerializedProperty min = property.FindPropertyRelative( "x" );
        SerializedProperty max = property.FindPropertyRelative( "y" );

        // draw the range and the reset button first so that the slider doesn't grab all the input
        Rect rangePosition = sliderPosition;
        rangePosition.y += rangePosition.height * 0.5f;
        rangePosition.height *= 0.5f;
        Rect contentPosition = rangePosition;
        EditorGUI.indentLevel = 0;
        EditorGUIUtility.labelWidth = 30f;
        contentPosition.width *= 0.3f;
        EditorGUI.PropertyField(contentPosition, min, new GUIContent( "Min" ) );
        contentPosition.x += contentPosition.width + 20f;
        EditorGUI.PropertyField( contentPosition, max, new GUIContent( "Max" ) );
        contentPosition.x += contentPosition.width + 20f;
        contentPosition.width = 50.0f;
        if ( GUI.Button( contentPosition, "Reset" ) ) {
            min.floatValue = minMax.minDefaultVal;
            max.floatValue = minMax.maxDefaultVal;
        }
        float minValue = min.floatValue;
        float maxValue = max.floatValue;
#if UNITY_2017_1_OR_NEWER
        EditorGUI.MinMaxSlider( sliderPosition, GUIContent.none, ref minValue, ref maxValue, minMax.min, minMax.max );
#else
        EditorGUI.MinMaxSlider( GUIContent.none, sliderPosition, ref minValue, ref maxValue, minMax.min, minMax.max );
#endif
        // round to readable values
        min.floatValue = Mathf.Round( minValue / 0.01f ) * 0.01f;
        max.floatValue = Mathf.Round( maxValue / 0.01f ) * 0.01f;
    }

}

} // namespace OVR
