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

/************************************************************************************

See SampleFramework license.txt for license terms.  Unless required by applicable law
or agreed to in writing, the sample code is provided “AS IS” WITHOUT WARRANTIES OR
CONDITIONS OF ANY KIND, either express or implied.  See the license for specific
language governing permissions and limitations under the license.

************************************************************************************/

using UnityEngine;
using UnityEngine.Assertions;

namespace OculusSampleFramework
{
    public class CowController : MonoBehaviour
    {
        [SerializeField] private Animation _cowAnimation = null;
        [SerializeField] private AudioSource _mooCowAudioSource = null;

        private void Start()
        {
            Assert.IsNotNull(_cowAnimation);
            Assert.IsNotNull(_mooCowAudioSource);
        }

        public void PlayMooSound()
        {
            _mooCowAudioSource.timeSamples = 0;
            _mooCowAudioSource.Play();
        }

        public void GoMooCowGo()
        {
            _cowAnimation.Rewind();
            _cowAnimation.Play();
        }
    }
}
