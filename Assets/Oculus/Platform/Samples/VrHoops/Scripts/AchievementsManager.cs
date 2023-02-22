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
    using Oculus.Platform;
    using Oculus.Platform.Models;

    public class AchievementsManager
    {
        // API NAME defined on the dashboard for the achievement
        private const string LIKES_TO_WIN = "LIKES_TO_WIN";

        // true if the local user hit the achievement Count setup on the dashboard
        private bool m_likesToWinUnlocked;

        public bool LikesToWin
        {
            get { return m_likesToWinUnlocked; }
        }

        public void CheckForAchievmentUpdates()
        {
            Achievements.GetProgressByName(new string[]{ LIKES_TO_WIN }).OnComplete(
                (Message<AchievementProgressList> msg) =>
                {
                    foreach (var achievement in msg.Data)
                    {
                        if (achievement.Name == LIKES_TO_WIN)
                        {
                            m_likesToWinUnlocked = achievement.IsUnlocked;
                        }
                    }
                }
            );
        }

        public void RecordWinForLocalUser()
        {
            Achievements.AddCount(LIKES_TO_WIN, 1);
            CheckForAchievmentUpdates();
        }
    }
}
