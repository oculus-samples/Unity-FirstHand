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
using System.Collections;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Sets a renderers uv tiling offset to match up with cells in a sprite sheet at a specified framerate, or scrolling
    /// </summary>
    public class UVAnimation : MonoBehaviour
    {
        static MaterialPropertyBlock _mpb;
        static MaterialPropertyBlock PropertyBlock => _mpb ??= new MaterialPropertyBlock();

        [SerializeField] string _uvPropertyName = "_BaseMap_ST";

        [Header("Flipbook")]
        [SerializeField] Vector2Int _cellCount = new Vector2Int(5, 5);
        [SerializeField] float _framesPerSecond = 10;

        [Header("Scroll")]
        [SerializeField] Vector2 _scroll = new Vector2(0, 0);

        private void OnEnable()
        {
            var renderer = GetComponent<Renderer>();
            var property = Shader.PropertyToID(_uvPropertyName);

            int totalCells = _cellCount.x * _cellCount.y;
            if (totalCells > 1)
            {
                var waitForSeconds = new WaitForSeconds(1 / _framesPerSecond);
                var tileSize = new Vector2(1f / _cellCount.x, 1f / _cellCount.y);
                var yRowOffset = _cellCount.y - 1;
                int frame = 0;

                StartCoroutine(Flipbook());
                IEnumerator Flipbook()
                {
                    while (isActiveAndEnabled)
                    {
                        var xOffset = (frame % _cellCount.x) * tileSize.x;
                        var yOffset = Mathf.Repeat(yRowOffset + (-frame / _cellCount.y), _cellCount.y) * tileSize.y;
                        Vector4 uv = new Vector4(tileSize.x, tileSize.y, xOffset, yOffset);
                        SetTileOffset(renderer, property, uv);

                        frame++;
                        yield return waitForSeconds;
                    }
                }
            }
            else
            {
                var tileOffset = renderer.sharedMaterial.GetVector(property);
                StartCoroutine(Scroll());
                IEnumerator Scroll()
                {
                    while (isActiveAndEnabled)
                    {
                        tileOffset.z += Time.deltaTime * _scroll.x;
                        tileOffset.w += Time.deltaTime * _scroll.y;
                        SetTileOffset(renderer, property, tileOffset);

                        yield return null;
                    }
                }
            }
        }

        private static void SetTileOffset(Renderer renderer, int property, Vector4 uv)
        {
            renderer.GetPropertyBlock(PropertyBlock);
            PropertyBlock.SetVector(property, uv);
            renderer.SetPropertyBlock(PropertyBlock);
        }
    }
}
