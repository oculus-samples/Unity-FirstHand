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

namespace Oculus.Platform
{
    using UnityEditor;
    using UnityEngine;

    class GUIHelper
    {
        public delegate void Worker();

        static void InOut(Worker begin, Worker body, Worker end)
        {
            try
            {
                begin();
                body();
            }
            finally
            {
                end();
            }
        }

        public static void HInset(int pixels, Worker worker)
        {
            InOut(
              () =>
              {
                  GUILayout.BeginHorizontal();
                  GUILayout.Space(pixels);
                  GUILayout.BeginVertical();
              },
              worker,
              () =>
              {
                  GUILayout.EndVertical();
                  GUILayout.EndHorizontal();
              }
            );
        }

        public delegate T ControlWorker<T>();
        public static T MakeControlWithLabel<T>(GUIContent label, ControlWorker<T> worker)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label);

            var result = worker();

            EditorGUILayout.EndHorizontal();
            return result;
        }
    }

}
