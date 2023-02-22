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

    // Uses two triggers to detect that a basket is made by traveling from top to bottom
    // through the hoop.
    public class DetectBasket : MonoBehaviour
    {
        private enum BasketPhase { NONE, TOP, BOTH, BOTTOM }

        private BasketPhase m_phase = BasketPhase.NONE;

        private Player m_owningPlayer;

        public Player Player
        {
            set { m_owningPlayer = value; }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.name == "Basket Top" && m_phase == BasketPhase.NONE)
            {
                m_phase = BasketPhase.TOP;
            }
            else if (other.gameObject.name == "Basket Bottom" && m_phase == BasketPhase.TOP)
            {
                m_phase = BasketPhase.BOTH;
            }
            else
            {
                m_phase = BasketPhase.NONE;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.name == "Basket Top" && m_phase == BasketPhase.BOTH)
            {
                m_phase = BasketPhase.BOTTOM;
            }
            else if (other.gameObject.name == "Basket Bottom" && m_phase == BasketPhase.BOTTOM)
            {
                m_phase = BasketPhase.NONE;

                switch (PlatformManager.CurrentState)
                {
                    case PlatformManager.State.PLAYING_A_LOCAL_MATCH:
                    case PlatformManager.State.PLAYING_A_NETWORKED_MATCH:
                        if (m_owningPlayer)
                        {
                            m_owningPlayer.Score += 2;
                        }
                        break;
                }
            }
            else
            {
                m_phase = BasketPhase.NONE;
            }
        }
    }
}
