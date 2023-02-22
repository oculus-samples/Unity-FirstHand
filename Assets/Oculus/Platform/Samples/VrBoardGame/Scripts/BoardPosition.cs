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

    // This behaviour is attached to GameObjects whose collision mesh
    // describes a specific position on the GameBoard.  The collision
    // mesh doesn't need to fully cover the board position, but enough
    // for eye raycasts to detect that the user is looking there.
    public class BoardPosition : MonoBehaviour {

        [SerializeField] [Range(0,2)] public int x = 0;
        [SerializeField] [Range(0,2)] public int y = 0;
    }
}
