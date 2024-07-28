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
    using System.Collections;

    // This class listens for Input events to shoot a ball, and also notifies the P2PManager when
    // ball or scores needs to be synchronized to remote players.
    public class LocalPlayer : Player
    {

        public override uint Score
        {
            set
            {
                base.Score = value;

                if (PlatformManager.CurrentState == PlatformManager.State.PLAYING_A_NETWORKED_MATCH)
                {
                    PlatformManager.P2P.SendScoreUpdate(base.Score);
                }
            }
        }

        void Update()
        {
            GameObject newball = null;

            // if the player is holding a ball
            if (HasBall)
            {
                // check to see if the User is hitting the shoot button
                if (Input.GetButton("Fire1") || Input.GetKey(KeyCode.Space))
                {
                    newball = ShootBall();
                }
            }
            // spawn a new held ball if we can
            else
            {
                newball = CheckSpawnBall();
            }

            if (newball && PlatformManager.CurrentState == PlatformManager.State.PLAYING_A_NETWORKED_MATCH)
            {
                PlatformManager.P2P.AddNetworkBall(newball);
            }
        }
    }
}
