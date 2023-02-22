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

namespace Oculus.Platform.Samples.VrVoiceChat
{
    using UnityEngine;
    using System.Collections;
    using UnityEngine.UI;

    // Helper class to attach to the main camera that raycasts where the
    // user is looking to select/deselect Buttons.
    public class VREyeRaycaster : MonoBehaviour
    {
        [SerializeField] private UnityEngine.EventSystems.EventSystem m_eventSystem = null;

        private Button m_currentButton;

        void Update ()
        {
            RaycastHit hit;
            Button button = null;

            // do a forward raycast to see if we hit a Button
            if (Physics.Raycast(transform.position, transform.forward, out hit, 50f))
            {
                button = hit.collider.GetComponent<Button>();
            }

            if (button != null)
            {
                if (m_currentButton != button)
                {
                    m_currentButton = button;
                    m_currentButton.Select();
                }
            }
            else if (m_currentButton != null)
            {
                m_currentButton = null;
                if (m_eventSystem != null)
                {
                    m_eventSystem.SetSelectedGameObject(null);
                }
            }
        }
    }
}
