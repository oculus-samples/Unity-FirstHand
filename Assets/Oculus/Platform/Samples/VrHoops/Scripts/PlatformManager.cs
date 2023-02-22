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
    using Oculus.Platform;
    using Oculus.Platform.Models;

    public class PlatformManager : MonoBehaviour
    {
        private static PlatformManager s_instance;
        private MatchmakingManager m_matchmaking;
        private P2PManager m_p2p;
        private LeaderboardManager m_leaderboards;
        private AchievementsManager m_achievements;
        private State m_currentState;

        // my Application-scoped Oculus ID
        private ulong m_myID;

        // my Oculus user name
        private string m_myOculusID;

        void Update()
        {
            m_p2p.UpdateNetwork();
            m_leaderboards.CheckForUpdates();
        }

        #region Initialization and Shutdown

        void Awake()
        {
            // make sure only one instance of this manager ever exists
            if (s_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            s_instance = this;
            DontDestroyOnLoad(gameObject);

            Core.Initialize();
            m_matchmaking = new MatchmakingManager();
            m_p2p = new P2PManager();
            m_leaderboards = new LeaderboardManager();
            m_achievements = new AchievementsManager();
        }


        void Start()
        {
            // First thing we should do is perform an entitlement check to make sure
            // we successfully connected to the Oculus Platform Service.
            Entitlements.IsUserEntitledToApplication().OnComplete(IsEntitledCallback);
        }

        void IsEntitledCallback(Message msg)
        {
            if (msg.IsError)
            {
                TerminateWithError(msg);
                return;
            }

            // Next get the identity of the user that launched the Application.
            Users.GetLoggedInUser().OnComplete(GetLoggedInUserCallback);
        }

        void GetLoggedInUserCallback(Message<User> msg)
        {
            if (msg.IsError)
            {
                TerminateWithError(msg);
                return;
            }

            m_myID = msg.Data.ID;
            m_myOculusID = msg.Data.OculusID;

            TransitionToState(State.WAITING_TO_PRACTICE_OR_MATCHMAKE);
            Achievements.CheckForAchievmentUpdates();
        }

        // In this example, for most errors, we terminate the Application.  A full App would do
        // something more graceful.
        public static void TerminateWithError(Message msg)
        {
            Debug.Log("Error: " + msg.GetError().Message);
            UnityEngine.Application.Quit();
        }

        public void QuitButtonPressed()
        {
            UnityEngine.Application.Quit();
        }

        void OnApplicationQuit()
        {
            // be a good matchmaking citizen and leave any queue immediately
            Matchmaking.LeaveQueue();
        }

        #endregion

        #region Properties

        public static MatchmakingManager Matchmaking
        {
            get { return s_instance.m_matchmaking; }
        }

        public static P2PManager P2P
        {
            get { return s_instance.m_p2p; }
        }

        public static LeaderboardManager Leaderboards
        {
            get { return s_instance.m_leaderboards; }
        }

        public static AchievementsManager Achievements
        {
            get { return s_instance.m_achievements; }
        }

        public static State CurrentState
        {
            get { return s_instance.m_currentState; }
        }

        public static ulong MyID
        {
            get
            {
                if (s_instance != null)
                {
                    return s_instance.m_myID;
                }
                else
                {
                    return 0;
                }
            }
        }

        public static string MyOculusID
        {
            get
            {
                if (s_instance != null && s_instance.m_myOculusID != null)
                {
                    return s_instance.m_myOculusID;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        #endregion

        #region State Management

        public enum State
        {
            // loading platform library, checking application entitlement,
            // getting the local user info
            INITIALIZING,

            // waiting on the user to join a matchmaking queue or play a practice game
            WAITING_TO_PRACTICE_OR_MATCHMAKE,

            // waiting for the match to start or viewing results
            MATCH_TRANSITION,

            // actively playing a practice match
            PLAYING_A_LOCAL_MATCH,

            // actively playing an online match
            PLAYING_A_NETWORKED_MATCH,
        };

        public static void TransitionToState(State newState)
        {
            if (s_instance && s_instance.m_currentState != newState)
            {
                s_instance.m_currentState = newState;
            }
        }

        #endregion
    }
}
