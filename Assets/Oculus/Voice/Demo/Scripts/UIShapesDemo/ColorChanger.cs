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

using System;
using Facebook.WitAi;
using Facebook.WitAi.Lib;
using UnityEngine;

namespace Oculus.Voice.Demo.UIShapesDemo
{
    public class ColorChanger : MonoBehaviour
    {
        /// <summary>
        /// Sets the color of the specified transform.
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="color"></param>
        private void SetColor(Transform trans, Color color)
        {
            trans.GetComponent<Renderer>().material.color = color;
        }

        /// <summary>
        /// Directly processes a command result getting the slots with WitResult utilities
        /// </summary>
        /// <param name="commandResult">Result data from Wit.ai activation to be processed</param>
        public void UpdateColor(WitResponseNode commandResult)
        {
            string colorName = commandResult.GetFirstEntityValue("color:color");
            string shape = commandResult.GetFirstEntityValue("shape:shape");
            UpdateColor(colorName, shape);
        }

        /// <summary>
        /// Processes the values of a result handler with a color and shape filter.
        /// </summary>
        /// <param name="results">Results from result handler [0] color name, [1] shape</param>
        public void UpdateColor(string[] results)
        {
            var colorName = results[0];
            var shape = results[1];

            UpdateColor(colorName, shape);
        }

        /// <summary>
        /// Updates the color of a shape or all shapes
        /// </summary>
        /// <param name="colorName">The name of a color to be processed</param>
        /// <param name="shape">The shape name or if empty all shapes</param>
        public void UpdateColor(string colorName, string shape)
        {
            if (!ColorUtility.TryParseHtmlString(colorName, out var color)) return;

            if (string.IsNullOrEmpty(shape) || shape == "color")
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    SetColor(transform.GetChild(i), color);
                }
            }
            else
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform child = transform.GetChild(i);
                    if (String.Equals(shape, child.name,
                        StringComparison.CurrentCultureIgnoreCase))
                    {
                        SetColor(child, color);
                        break;
                    }
                }
            }
        }
    }
}
