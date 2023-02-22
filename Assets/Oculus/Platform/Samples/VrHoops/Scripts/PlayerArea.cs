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
    using Oculus.Platform.Models;

    public class PlayerArea : MonoBehaviour
    {
        // the prefab for the ball that players will shoot
        [SerializeField] private GameObject m_ballPrefab = null;

        // cached gameobject that where the player camera will move to
        private GameObject m_playerHead;

        // cached Text component where we'll render the player's name
        private Text m_nameText;

        // cached component used to align the backboard movement between devices
        private P2PNetworkGoal m_p2pGoal;

        public Player Player
        {
            get { return m_playerHead.GetComponent<Player>(); }
        }

        public Text NameText
        {
            get { return m_nameText; }
        }

        void Awake()
        {
            m_playerHead = gameObject.transform.Find("Player Head").gameObject;
            m_nameText = gameObject.GetComponentsInChildren<Text>()[1];
            m_p2pGoal = gameObject.GetComponentInChildren<P2PNetworkGoal> ();
        }

        public T SetupForPlayer<T>(string name) where T : Player
        {
            var oldplayer = m_playerHead.GetComponent<Player>();
            if (oldplayer) Destroy(oldplayer);

            var player = m_playerHead.AddComponent<T>();
            player.BallPrefab = m_ballPrefab;
            m_nameText.text = name;

            if (player is RemotePlayer)
            {
                (player as RemotePlayer).Goal = m_p2pGoal;
                m_p2pGoal.SendUpdates = false;
            }
            else if (player is LocalPlayer)
            {
                m_p2pGoal.SendUpdates = true;
            }
            else
            {
                m_p2pGoal.SendUpdates = false;
            }

            return player;
        }
    }
}
