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

namespace Oculus.Platform.Samples.VrHoops
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;

    // helper script to render fading flytext above an object
    public class FlyText : MonoBehaviour
    {
        // destory the gameobject after this many seconds
        private const float LIFESPAN = 3.0f;

        // how far to move upwards per frame
        private readonly Vector3 m_movePerFrame = 0.5f * Vector3.up;

        // actual destruction time
        private float m_eol;

        void Start()
        {
            m_eol = Time.time + LIFESPAN;
            GetComponent<Text>().CrossFadeColor(Color.black, LIFESPAN * 1.7f, false, true);
        }

        void Update()
        {
            if (Time.time < m_eol)
            {
                transform.localPosition += m_movePerFrame;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
