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

using UnityEngine;

/*
-----------------------

 InspectorNoteAttribute()

-----------------------
*/
public class InspectorNoteAttribute : PropertyAttribute
{
    public readonly string header;
    public readonly string message;

    public InspectorNoteAttribute(string header, string message = "")
    {
        this.header = header;
        this.message = message;
    }
}

/*
-----------------------

 InspectorCommentAttribute()

-----------------------
*/
public class InspectorCommentAttribute : PropertyAttribute
{
    public readonly string message;

    public InspectorCommentAttribute(string message = "")
    {
        this.message = message;
    }
}
