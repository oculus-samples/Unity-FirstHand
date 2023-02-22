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
using System.Collections;

/// <summary>
/// This orientation handler doesn't actually do anything with the orientation at all; this is for users
/// who have a 360 setup and don't need to be concerned with choosing an orientation because they just
/// turn whatever direction they want.
/// </summary>
public class TeleportOrientationHandler360 : TeleportOrientationHandler
{
    protected override void InitializeTeleportDestination()
    {
    }

    protected override void UpdateTeleportDestination()
    {
        LocomotionTeleport.OnUpdateTeleportDestination(AimData.TargetValid, AimData.Destination, null, null);
    }
}
