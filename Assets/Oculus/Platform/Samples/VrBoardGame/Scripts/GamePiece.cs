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

namespace Oculus.Platform.Samples.VrBoardGame
{
    using UnityEngine;
    using System.Collections;

    public class GamePiece : MonoBehaviour
    {
        [SerializeField] private Piece m_type = Piece.A;

        // Prefab for the game pieces
        [SerializeField] private GameObject m_prefabA = null;
        [SerializeField] private GameObject m_prefabB = null;
        [SerializeField] private GameObject m_prefabPower = null;

        public enum Piece { A, B, PowerBall }

        private BoardPosition m_position;

        public Piece Type
        {
            get { return m_type; }
        }

        public BoardPosition Position
        {
            get { return m_position; }
            set { m_position = value; }
        }

        public GameObject Prefab
        {
            get
            {
                switch (m_type)
                {
                    case Piece.A: return m_prefabA;
                    case Piece.B: return m_prefabB;
                    default: return m_prefabPower;
                }
            }
        }

        public GameObject PrefabFor(Piece p)
        {
            switch (p)
            {
                case Piece.A: return m_prefabA;
                case Piece.B: return m_prefabB;
                default: return m_prefabPower;
            }
        }

    }
}
