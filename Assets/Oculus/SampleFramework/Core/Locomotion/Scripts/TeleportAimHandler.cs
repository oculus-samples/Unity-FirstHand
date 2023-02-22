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
using System.Collections.Generic;

/// <summary>
/// TeleportAimHandler will provide a series of points in the world that represent the series of line
/// segments (as few as one for a laser) which the player uses to determine where they are aiming for a teleport.
/// This is not the visualization of the aiming, it is merely the set of points representing the line, arc, or whatever
/// shape makes sense for the teleport aiming mechanism which is then used to perform collision detection with the world
/// in order to determine the final teleport target location.
/// </summary>
public abstract class TeleportAimHandler : TeleportSupport
{
    /// <summary>
    /// The LocomotionTeleport supports one aim handler at a time. Call the base OnEnable to make sure LocomotionTeleport
    /// is valid, then assign the current aim handler to this object.
    /// </summary>
    protected override void OnEnable()
    {
        base.OnEnable();
        LocomotionTeleport.AimHandler = this;
    }

    /// <summary>
    /// When this component is disabled, make sure to clear the LocomotionTeleport's aim handler but only if this is
    /// still the active handler. It's an unlikely edge case but it's more robust to make sure a different aim handler
    /// wasn't enabled before this was disabled.
    /// </summary>
    protected override void OnDisable()
    {
        if (LocomotionTeleport.AimHandler == this)
        {
            LocomotionTeleport.AimHandler = null;
        }
        base.OnDisable();
    }

    /// <summary>
    /// GetPoints provides the core purpose of this class: Return a sequence of points that represents the line segment
    /// or segments that should be collision tested against the world.
    /// </summary>
    /// <returns></returns>
    public abstract void GetPoints(List<Vector3> points);
}
