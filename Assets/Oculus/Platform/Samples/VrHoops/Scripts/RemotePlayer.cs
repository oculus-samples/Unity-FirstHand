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
    using Oculus.Platform.Models;

    public class RemotePlayer : Player
    {
        private User m_user;
        private P2PNetworkGoal m_goal;

        public User User
        {
            set { m_user = value; }
        }

        public ulong ID
        {
            get { return m_user.ID; }
        }

        public P2PNetworkGoal Goal
        {
            get { return m_goal; }
            set { m_goal = value; }
        }

        public override uint Score
        {
            set
            {
                // For now we ignore the score determined from locally scoring backets.
                // To get an indication of how close the physics simulations were between devices,
                // or whether the remote player was cheating, an estimate of the score could be
                // kept and compared against what the remote player was sending us.
            }
        }

        public void ReceiveRemoteScore(uint score)
        {
            base.Score = score;
        }
    }
}
